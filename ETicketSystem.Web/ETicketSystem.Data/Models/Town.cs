namespace ETicketSystem.Data.Models
{
	using Common.Constants;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;

	public class Town
    {
		public int Id { get; set; }

		[Required]
		[MaxLength(DataConstants.Town.TownMaxNameLength)]
		public string Name { get; set; }

		public List<Station> Stations { get; set; } = new List<Station>();

		public List<Company> Companies { get; set; } = new List<Company>();
    }
}
