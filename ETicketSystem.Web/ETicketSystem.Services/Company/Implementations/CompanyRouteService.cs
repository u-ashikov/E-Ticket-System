namespace ETicketSystem.Services.Company.Implementations
{
	using AutoMapper.QueryableExtensions;
	using Contracts;
	using Data;
	using Data.Enums;
	using Data.Models;
	using Microsoft.EntityFrameworkCore;
	using Models;
	using System;
	using System.Linq;

	public class CompanyRouteService : ICompanyRouteService
    {
		private readonly ETicketSystemDbContext db;

		public CompanyRouteService(ETicketSystemDbContext db)
		{
			this.db = db;
		}

		public bool Add(int startStation, int endStation, TimeSpan departureTime, TimeSpan duration, BusType busType, decimal price, string companyId)
		{
			var company = this.db.Companies
				.Include(c=>c.Routes)
				.FirstOrDefault(c=>c.Id == companyId);

			if (this.db.Routes.Any(r=>r.StartStationId == startStation && r.EndStationId == endStation && r.DepartureTime == departureTime && r.CompanyId == companyId))
			{
				return false;
			}

			company.Routes.Add(new Route()
			{
				StartStationId = startStation,
				EndStationId = endStation,
				DepartureTime = departureTime,
				Duration = duration,
				BusType = busType,
				Price = price,
				IsActive = true
			});

			this.db.SaveChanges();
			return true;
		}

		public CompanyRoutesServiceModel All(int startTown, int endTown, DateTime? date,string companyId, int page, int pageSize = 10)
		{
			var companyRoutes = this.db
				.Companies
				.Include(c => c.Routes)
				.Where(c => c.Id == companyId)
				.ProjectTo<CompanyRoutesServiceModel>()
				.FirstOrDefault();

			var currentTime = new TimeSpan(DateTime.UtcNow.ToLocalTime().Hour, DateTime.UtcNow.ToLocalTime().Minute, DateTime.UtcNow.ToLocalTime().Second);

			if (startTown != 0 && endTown != 0)
			{
				companyRoutes.Routes = companyRoutes.Routes.Where(r => r.StartTown == startTown && r.EndTown == endTown).ToList();
			}

			if (date != null)
			{
				if (date.Value.Date > DateTime.UtcNow.ToLocalTime().Date)
				{
					companyRoutes.Routes = companyRoutes.Routes.Where(r => r.StartTown == startTown && r.EndTown == endTown && r.DepartureTime >= new TimeSpan(0, 0, 0));
				}
				else if (date.Value.Date == DateTime.UtcNow.ToLocalTime().Date)
				{
					companyRoutes.Routes = companyRoutes.Routes.Where(r => r.StartTown == startTown && r.EndTown == endTown && r.DepartureTime >= currentTime);
				}
			}		

			companyRoutes.Routes = companyRoutes
					.Routes
					.OrderBy(r=>r.StartTown)
					.ThenBy(r=>r.EndTown)
					.ThenBy(r=>r.DepartureTime)
					.Skip((page - 1) * pageSize)
					.Take(pageSize);

			return companyRoutes;
		}

		public CompanyRouteEditServiceModel GetRouteToEdit(string companyId, int routeId) =>
			this.db
				.Routes
				.Where(r => r.Id == routeId && r.CompanyId == companyId)
				.ProjectTo<CompanyRouteEditServiceModel>()
				.FirstOrDefault();

		public bool Edit(int routeId, int startStation, int endStation, TimeSpan departureTime, TimeSpan duration, BusType busType, decimal price, string companyId)
		{
			var route = this.db.Routes.FirstOrDefault(r => r.Id == routeId && r.CompanyId == companyId);

			if (route == null)
			{
				return false;
			}

			route.DepartureTime = departureTime;
			route.Duration = duration;
			route.BusType = busType;
			route.Price = price;

			this.db.SaveChanges();

			return true;
		}

		public bool RouteAlreadyExist(int routeId,int startStation, int endStation, TimeSpan departureTime, string companyId) =>
			this.db.Routes.Any(r => r.Id != routeId && r.StartStationId == startStation && r.EndStationId == endStation && r.DepartureTime == departureTime && r.CompanyId == companyId);

		public bool ChangeStatus(int routeId, string companyId)
		{
			var route = this.db
				.Routes
				.FirstOrDefault(r => r.Id == routeId && r.CompanyId == companyId);

			if (route == null)
			{
				return false;
			}

			route.IsActive = !route.IsActive;
			this.db.SaveChanges();

			return true;
		}

		public CompanyRouteBaseSerivceModel GetRouteBaseInfo(int routeId, string companyId) =>
			this.db.Routes
				.Where(r => r.Id == routeId && r.CompanyId == companyId)
				.ProjectTo<CompanyRouteBaseSerivceModel>()
				.FirstOrDefault();

		public int TotalRoutes(int startTown, int endTown, DateTime? date,string companyId)
		{
			var company = this.db
							.Companies
							.Include(c=>c.Routes)
								.ThenInclude(r=>r.StartStation)
							.Include(c=>c.Routes)
								.ThenInclude(r=>r.EndStation)
							.FirstOrDefault(c => c.Id == companyId);

			if (company == null)
			{
				return 0;
			}

			var currentTime = new TimeSpan(DateTime.Now.ToLocalTime().Hour, DateTime.Now.ToLocalTime().Minute, DateTime.Now.ToLocalTime().Second);

			if (startTown != 0 && endTown != 0)
			{
				company.Routes = company.Routes.Where(r => r.StartStation.TownId == startTown && r.EndStation.TownId == endTown).ToList();
			}

			if (date != null)
			{
				if (date.Value > DateTime.UtcNow.ToLocalTime())
				{
					return company.Routes.Count(r => r.StartStation.TownId == startTown && r.EndStation.TownId == endTown && r.DepartureTime >= new TimeSpan(0, 0, 0));
				}
				else if (date.Value.Date == DateTime.UtcNow.ToLocalTime().Date)
				{
					return company.Routes.Count(r => r.StartStation.TownId == startTown && r.EndStation.TownId == endTown && r.DepartureTime >= currentTime);
				}
			}

			return company.Routes.Count();
		}
		
		public bool HasReservedTickets(int routeId, string companyId)
		{
			return this.db.Tickets.Any(t => t.RouteId == routeId && t.Route.CompanyId == companyId && t.DepartureTime >= DateTime.UtcNow.ToLocalTime());
		}

		public bool IsRouteOwner(int id, string companyId) =>
			this.db.Routes.Any(r => r.CompanyId == companyId && r.Id == id);
	}
}
