namespace ETicketSystem.Services.Models.Station
{
	using Data.Models;
	using ETicketSystem.Common.Automapper;

	public class StationBaseServiceModel : IMapFrom<Station>
    {
		public int Id { get; set; }

		public string Name { get; set; }
    }
}
