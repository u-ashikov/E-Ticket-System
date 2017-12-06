namespace ETicketSystem.Services.Models.Town
{
	using Station;
	using System.Collections.Generic;

	public class TownStationsServiceModel : TownBaseServiceModel
    {
		public IEnumerable<StationBaseServiceModel> Stations { get; set; }
    }
}
