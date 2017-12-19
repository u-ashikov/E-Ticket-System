namespace ETicketSystem.Services.Contracts
{
	using Models.Ticket;
	using System;
	using System.Collections.Generic;

	public interface ITicketService
    {
		bool Add(int routeId, DateTime departureTime, IEnumerable<int> seats, string userId);

		IEnumerable<UserTicketListingServiceModel> GetUserTickets(string id, int startTown, int endTown, string companyId, DateTime? date, int page, int pageSize = 10);

		IEnumerable<int> GetAlreadyReservedTickets(int routeId, DateTime departureTime);

		byte[] GetPdfTicket(int ticketId, string userId);

		int GetRouteReservedTicketsCount(int routeId, DateTime departureTime);

		bool TicketExists(int id);

		bool IsTicketOwner(int id, string userId);

		bool CancelTicket(int id, string userId);

		bool IsCancelled(int id);

		TicketDownloadInfoServiceModel GetTicketDownloadInfo(int id, string userId);

		int UserTicketsCount(string id, int startTown, int endTown, string companyId, DateTime? date);
	}
}
