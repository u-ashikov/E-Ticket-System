namespace ETicketSystem.Web.Models.Routes
{
	using Infrastructure.Attributes.Validation;
	using System;
	using System.Collections.Generic;

	public class BookTicketFormModel
    {
		public int RouteId { get; set; }

		public string CompanyId { get; set; }

		public string CompanyName { get; set; }

		public DateTime DepartureDateTime { get; set; }

		public TimeSpan Duration { get; set; }

		public int BusSeats { get; set; }

		public string StartStation { get; set; }

		public string EndStation { get; set; }

		public int StartTownId { get; set; }

		public int EndTownId { get; set; }

		[AtLeastOneRequired]
		public List<BusSeat> Seats { get; set; } = new List<BusSeat>();
    }
}
