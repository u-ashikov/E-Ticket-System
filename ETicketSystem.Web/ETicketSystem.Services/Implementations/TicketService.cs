namespace ETicketSystem.Services.Implementations
{
	using AutoMapper.QueryableExtensions;
	using Common.Constants;
	using Contracts;
	using Data;
	using Data.Models;
	using Microsoft.EntityFrameworkCore;
	using Models.Ticket;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class TicketService : ITicketService
    {
		private readonly ETicketSystemDbContext db;

		private readonly IPdfGenerator pdfGenerator;

		public TicketService(ETicketSystemDbContext db, IPdfGenerator pdfGenerator)
		{
			this.db = db;
			this.pdfGenerator = pdfGenerator;
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

		public byte[] GetPdfTicket(int ticketId, string userId)
		{
			var ticket = this.db.Tickets
				.Where(t => t.Id == ticketId && t.UserId == userId)
				.Select(t => new
				{
					Company = t.Route.Company.Name,
					Route = $"{t.Route.StartStation.Town.Name}, {t.Route.StartStation.Name} -> {t.Route.EndStation.Town.Name}, {t.Route.EndStation.Name}",
					Seat = t.SeatNumber,
					DepartureTime = t.DepartureTime
				})
				.FirstOrDefault();

			if (ticket == null)
			{
				return null;
			}

			return this.pdfGenerator.GeneratePdfFromHtml(string.Format(WebConstants.Pdf.Ticket, ticket.Company, ticket.Route, ticket.Seat, ticket.DepartureTime));
		}
	}
}
