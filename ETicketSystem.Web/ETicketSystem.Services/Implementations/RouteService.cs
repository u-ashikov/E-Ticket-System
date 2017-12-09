namespace ETicketSystem.Services.Implementations
{
	using AutoMapper.QueryableExtensions;
	using ETicketSystem.Data;
	using ETicketSystem.Services.Contracts;
	using ETicketSystem.Services.Models.Route;
	using Microsoft.EntityFrameworkCore;
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
	}
}
