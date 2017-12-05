namespace ETicketSystem.Services.Models.Town
{
	using Data.Models;
	using ETicketSystem.Common.Automapper;

	public class TownBaseServiceModel : IMapFrom<Town>
    {
		public int Id { get; set; }

		public string Name { get; set; }
    }
}
