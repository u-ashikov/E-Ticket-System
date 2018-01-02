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

			var reservedSeats = route.Tickets
				.Where(t=>t.DepartureTime == departureTime && !t.IsCancelled)
				.Select(t => t.SeatNumber)
				.ToList();

			foreach (var seatNumber in seats)
			{
				if (seatNumber < 1 || seatNumber > (int)route.BusType || reservedSeats.Contains(seatNumber))
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

		public IEnumerable<int> GetAlreadyReservedTickets(int routeId, DateTime departureTime)
		{
			var route = this.db
				.Routes
				.Include(r => r.Tickets)
				.FirstOrDefault(r => r.Id == routeId);

			return route.Tickets
						.Where(t => t.DepartureTime == departureTime && !t.IsCancelled)
						.Select(t => t.SeatNumber)
						.ToList();
		}

		public IEnumerable<UserTicketListingServiceModel> GetUserTickets(string id, int startTown, int endTown, string companyId, DateTime? date, int page, int pageSize = 10)
		{
			var tickets = this.db.Tickets
				.Where(t => t.UserId == id && !t.IsCancelled)
				.OrderByDescending(t => t.DepartureTime)
				.AsQueryable();

			tickets = this.FilterUserTickets(startTown, endTown, companyId, date, tickets);

			return tickets
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.ProjectTo<UserTicketListingServiceModel>()
				.ToList();
		}

		public int UserTicketsCount(string id, int startTown, int endTown, string companyId, DateTime? date)
		{
			var tickets = this.db.Tickets
				.Where(t => t.UserId == id && !t.IsCancelled)
				.AsQueryable();

			tickets = this.FilterUserTickets(startTown, endTown, companyId, date, tickets);

			return tickets.Count();
		}

		public byte[] GetPdfTicket(int ticketId, string userId)
		{
			var ticket = this.db.Tickets
				.Where(t => t.Id == ticketId && t.UserId == userId && !t.IsCancelled)
				.ProjectTo<TicketPdfServiceModel>()
				.FirstOrDefault();

			if (ticket == null)
			{
				return null;
			}

			return this.pdfGenerator.GeneratePdfFromHtml(string.Format(WebConstants.Pdf.Ticket, ticket.Company, ticket.Route, ticket.Seat, ticket.DepartureTime));
		}

		public TicketDownloadInfoServiceModel GetTicketDownloadInfo(int id, string userId) =>
			this.db.Tickets
				.Where(t => t.Id == id && t.UserId == userId)
				.ProjectTo<TicketDownloadInfoServiceModel>()
				.FirstOrDefault();

		public int GetRouteReservedTicketsCount(int routeId, DateTime departureTime) =>
			this.db.Tickets
				.Count(t => t.RouteId == routeId && t.DepartureTime == departureTime && !t.IsCancelled);

		public bool TicketExists(int id) =>
			this.db.Tickets.Any(t=>t.Id == id);

		public bool IsTicketOwner(int id, string userId) =>
			this.db.Tickets.Any(t => t.Id == id && t.UserId == userId);

		public bool IsCancelled(int id) =>
			this.db.Tickets.Find(id).IsCancelled;

		public bool CancelTicket(int id, string userId)
		{
			var ticket = this.db.Tickets.FirstOrDefault(t => t.Id == id && t.UserId == userId);
			var currentDateTime = DateTime.UtcNow.ToLocalTime();

			TimeSpan timeDifference = ticket.DepartureTime - currentDateTime;

			if (timeDifference.TotalMinutes >=  WebConstants.Ticket.CancelationMinutesDifference && !ticket.IsCancelled)
			{
				ticket.IsCancelled = true;
				this.db.SaveChanges();

				return true;
			}

			return false;
		}

		private IQueryable<Ticket> FilterUserTickets(int startTown, int endTown, string companyId, DateTime? date, IQueryable<Ticket> tickets)
		{
			if (this.db.Towns.Any(t => t.Id == startTown))
			{
				tickets = tickets
							.Where(t => t.Route.StartStation.TownId == startTown);
			}

			if (this.db.Towns.Any(t => t.Id == endTown))
			{
				tickets = tickets
							.Where(t => t.Route.EndStation.TownId == endTown);
			}

			if (companyId != null && this.db.Companies.Any(c=>c.Id == companyId))
			{
				tickets = tickets
							.Where(t => t.Route.CompanyId == companyId);
			}

			if (date != null)
			{
				tickets = tickets
							.Where(t => t.DepartureTime.Date == date);
			}

			return tickets;
		}
	}
}
