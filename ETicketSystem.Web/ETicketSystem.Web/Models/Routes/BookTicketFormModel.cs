namespace ETicketSystem.Web.Models.Routes
{
	using System;
	using System.Collections.Generic;

	public class BookTicketFormModel
    {
		public int RouteId { get; set; }

		public DateTime DepartureTime { get; set; }

		public List<BookSeatViewModel> Seats { get; set; } = new List<BookSeatViewModel>();
    }
}
