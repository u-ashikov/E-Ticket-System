namespace ETicketSystem.Services.Contracts
{
	using Models.Town;
	using System.Collections.Generic;

	public interface ITownService
    {
		IEnumerable<TownBaseServiceModel> GetTownsListItems();

		IEnumerable<TownStationsServiceModel> GetTownsWithStations();

		string GetTownNameByStationId(int id);

		string GetTownNameById(int id);

		bool TownExistsById(int id);
    }
}
