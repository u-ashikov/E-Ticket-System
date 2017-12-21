namespace ETicketSystem.Data.Models
{
	using Common.Constants;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;

	public class Station
    {
		public int Id { get; set; }

		[Required]
		[MaxLength(DataConstants.Station.NameMaxLength)]
		public string Name { get; set; }

		public int TownId { get; set; }

		public Town Town { get; set; }

		[MaxLength(DataConstants.Station.PhoneMaxLength)]
		public string Phone { get; set; }

		public List<Route> ArrivalRoutes { get; set; } = new List<Route>();

		public List<Route> DepartureRoutes { get; set; } = new List<Route>();
	}
}
