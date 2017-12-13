namespace ETicketSystem.Services.Implementations
{
	using AutoMapper;
	using AutoMapper.QueryableExtensions;
	using Contracts;
	using ETicketSystem.Data;
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

		public IEnumerable<RouteSearchListingServiceModel> GetSearchedRoutes(int startTown, int endTown, DateTime date, string companyId)
		{
			var routes = this.db.Routes
								.Include(r=>r.StartStation)
								.Include(r=>r.EndStation)
								.Include(r => r.Company)
								.Where(r=> r.StartStation.TownId == startTown
									&& r.EndStation.TownId == endTown)
								.ToList();

			if (!string.IsNullOrEmpty(companyId))
			{
				routes = routes.Where(r => r.CompanyId == companyId).ToList();
			}

			if (date.Date > DateTime.UtcNow.Date)
			{
				return Mapper.Map<IEnumerable<RouteSearchListingServiceModel>>(routes
					.Where(r => r.DepartureTime >= new TimeSpan(0, 0, 0))
							.OrderBy(r => r.DepartureTime)
							.ToList());
			}

			return Mapper.Map<IEnumerable<RouteSearchListingServiceModel>>(routes
						.Where(r => r.DepartureTime > DateTime.UtcNow.TimeOfDay)
						.OrderBy(r => r.DepartureTime)
						.ToList());
		}

		public RouteBookTicketInfoServiceModel GetRouteTicketBookingInfo(int id, DateTime date) =>
			this.db
				.Routes
					.Include(r => r.Company)
					.Include(r => r.Tickets)
				.Where(r => r.Id == id)
				.ProjectTo<RouteBookTicketInfoServiceModel>(new { ticketDate = date})
				.FirstOrDefault();

		public bool RouteExists(int id, TimeSpan departureTime) =>
			this.db.Routes
				.Any(r => r.Id == id && r.DepartureTime == departureTime);

		public int GetSearchedRoutesCount(int startTown, int endTown, DateTime date, string companyId)
		{
			var routes = this.db.Routes
							.Where(r => r.StartStation.TownId == startTown
							&& r.EndStation.TownId == endTown)
							.AsQueryable();

			if (date.Date > DateTime.UtcNow.Date)
			{
				routes = routes
							.Where(r => r.DepartureTime >= new TimeSpan(0, 0, 0))
							.AsQueryable();
			}
			else
			{
				routes = routes
							.Where(r => r.DepartureTime > DateTime.UtcNow.TimeOfDay)
							.AsQueryable();
			}

			if (!string.IsNullOrEmpty(companyId))
			{
				return routes.Count(r => r.CompanyId == companyId);
			}

			return routes.Count();
		}
	}
}
