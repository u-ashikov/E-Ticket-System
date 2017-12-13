namespace ETicketSystem.Services.Implementations
{
	using AutoMapper.QueryableExtensions;
	using Contracts;
	using Data.Models;
	using ETicketSystem.Data;
	using Microsoft.EntityFrameworkCore;
	using Models.Ticket;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class TicketService : ITicketService
    {
		private readonly ETicketSystemDbContext db;

		public TicketService(ETicketSystemDbContext db)
		{
			this.db = db;
		}

		public bool Add(int routeId, DateTime departureTime, IEnumerable<int> seats, string userId)
		{
			var route = this.db
				.Routes
				.Include(r=>r.Tickets)
				.FirstOrDefault(r => r.Id == routeId);

			if (route == null)
			{
				return false;
			}

			foreach (var seatNumber in seats)
			{
				if (seatNumber < 1 || seatNumber > (int)route.BusType)
				{
					return false;
				}

				this.db.Tickets.Add(new Ticket()
				{
					RouteId = routeId,
					SeatNumber = seatNumber,
					UserId = userId,
					DepartureTime = departureTime
				});
			}

			this.db.SaveChanges();

			return true;
		}

		public IEnumerable<UserTicketListingServiceModel> GetUserTickets(string id, int page, int pageSize = 10) =>
			this.db.Tickets
				.Where(t => t.UserId == id)
				.OrderByDescending(t=>t.DepartureTime)
				.Skip((page-1) * pageSize)
				.Take(pageSize)
				.ProjectTo<UserTicketListingServiceModel>()
				.ToList();

		public int UserTicketsCount(string id) =>
			this.db.Tickets
				.Count(t => t.UserId == id);
	}
}
