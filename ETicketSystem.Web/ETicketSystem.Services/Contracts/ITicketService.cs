﻿namespace ETicketSystem.Services.Contracts
{
	using Models.Ticket;
	using System;
	using System.Collections.Generic;

	public interface ITicketService
    {
		bool Add(int routeId, DateTime departureTime, IEnumerable<int> seats, string userId);

		IEnumerable<UserTicketListingServiceModel> GetUserTickets(string id, int startTown, int endTown, string companyId, DateTime? date, int page, int pageSize = 10);

		byte[] GetPdfTicket(int ticketId, string userId);

		int UserTicketsCount(string id, int startTown, int endTown, string companyId, DateTime? date);
	}
}
