namespace ETicketSystem.Web.Models.Users
{
	using Pagination;
	using Services.Models.Ticket;
	using System.Collections.Generic;

	public class UserTickets
    {
		public IEnumerable<UserTicketListingServiceModel> Tickets { get; set; }

		public PaginationViewModel Pagination { get; set; }
    }
}
