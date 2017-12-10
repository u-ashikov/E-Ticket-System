namespace ETicketSystem.Services.Implementations
{
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

		public IEnumerable<RouteSearchListingServiceModel> GetSearchedRoutes(int startTown, int endTown, DateTime date)
		{
			if (date.Date > DateTime.UtcNow.Date)
			{
				return this.db
							.Routes
							.Include(c=>c.Company)
							.Where(r => r.StartStation.TownId == startTown
									&& r.EndStation.TownId == endTown
									&& r.DepartureTime >= new TimeSpan(0,0,0))
							.OrderBy(r=>r.DepartureTime)
							.ProjectTo<RouteSearchListingServiceModel>()
							.ToList();
			}

			return this.db
						.Routes
						.Include(c => c.Company)
						.Where(r => r.StartStation.TownId == startTown
								&& r.EndStation.TownId == endTown
								&& r.DepartureTime > DateTime.UtcNow.TimeOfDay)
						.OrderBy(r => r.DepartureTime)
						.ProjectTo<RouteSearchListingServiceModel>()
						.ToList();
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
	}
}
