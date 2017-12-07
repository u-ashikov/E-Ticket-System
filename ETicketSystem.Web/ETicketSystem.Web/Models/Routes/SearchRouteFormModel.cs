namespace ETicketSystem.Web.Models.Routes
{
	using System;
	using System.ComponentModel.DataAnnotations;

	public class SearchRouteFormModel
    {
		public int StartTown { get; set; }

		public int EndTown { get; set; }

		[DataType(DataType.Date)]
		public DateTime Date { get; set; }
    }
}
