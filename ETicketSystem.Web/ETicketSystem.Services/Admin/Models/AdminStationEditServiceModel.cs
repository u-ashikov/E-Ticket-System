namespace ETicketSystem.Services.Admin.Models
{
	using Common.Automapper;
	using Data.Models;

	public class AdminStationEditServiceModel : IMapFrom<Station>
	{
		public int Id { get; set; }

		public int TownId { get; set; }

		public string Name { get; set; }

		public string Phone { get; set; }
	}
}
