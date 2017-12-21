namespace ETicketSystem.Services.Implementations
{
	using AutoMapper.QueryableExtensions;
	using Contracts;
	using Data;
	using Microsoft.EntityFrameworkCore;
	using Models.Route;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class RouteService : IRouteService
	{
		private readonly ETicketSystemDbContext db;

		public RouteService(ETicketSystemDbContext db)
		{
			this.db = db;
		}

		public IEnumerable<RouteSearchListingServiceModel> GetSearchedRoutes(int startTown, int endTown, DateTime date, string companyId, int page, int pageSize = 10)
		{
			var routes = this.db.Routes
								.Include(r=>r.StartStation)
								.Include(r=>r.EndStation)
								.Include(r => r.Company)
								.Where(r=> r.StartStation.TownId == startTown
									&& r.EndStation.TownId == endTown && r.IsActive)
								.AsQueryable();

			if (!string.IsNullOrEmpty(companyId) && this.db.Companies.Any(c=>c.Id == companyId))
			{
				routes = routes.Where(r => r.CompanyId == companyId).AsQueryable();
			}

			if (date.Date > DateTime.UtcNow.ToLocalTime().Date)
			{
				routes = routes
							.Where(r => r.DepartureTime >= new TimeSpan(0, 0, 0))
							.AsQueryable();
			}
			else if (date.Date == DateTime.UtcNow.ToLocalTime().Date)
			{
				routes = routes
						.Where(r => r.DepartureTime > DateTime.UtcNow.ToLocalTime().TimeOfDay)
						.AsQueryable();
			}

			return  routes
						.OrderBy(r => r.DepartureTime)
						.Skip((page - 1) * pageSize)
						.Take(pageSize)
						.ProjectTo<RouteSearchListingServiceModel>()
						.ToList();
		}

		public RouteBookTicketInfoServiceModel GetRouteTicketBookingInfo(int id, DateTime date) =>
			this.db
				.Routes
					.Include(r => r.Company)
					.Include(r => r.Tickets)
				.Where(r => r.Id == id && r.IsActive)
				.ProjectTo<RouteBookTicketInfoServiceModel>(new { ticketDate = date})
				.FirstOrDefault();

		public bool RouteExists(int id, TimeSpan departureTime) =>
			this.db.Routes
				.Any(r => r.Id == id && r.DepartureTime == departureTime);

		public int GetSearchedRoutesCount(int startTown, int endTown, DateTime date, string companyId)
		{
			var routes = this.db.Routes
							.Where(r => r.StartStation.TownId == startTown
							&& r.EndStation.TownId == endTown && r.IsActive)
							.AsQueryable();

			if (date.Date > DateTime.UtcNow.ToLocalTime().Date)
			{
				routes = routes
							.Where(r => r.DepartureTime >= new TimeSpan(0, 0, 0))
							.AsQueryable();
			}
			else if (date.Date == DateTime.UtcNow.ToLocalTime().Date)
			{
				routes = routes
							.Where(r => r.DepartureTime > DateTime.UtcNow.ToLocalTime().TimeOfDay)
							.AsQueryable();
			}

			if (!string.IsNullOrEmpty(companyId) && this.db.Companies.Any(c=>c.Id == companyId))
			{
				return routes.Count(r => r.CompanyId == companyId);
			}

			return routes.Count();
		}
	}
}
