namespace ETicketSystem.Web.Models.Routes
{
	using System.Collections.Generic;

	public class BookTicketFormModel
    {
		public List<BookSeatViewModel> Seats { get; set; } = new List<BookSeatViewModel>();
    }
}
