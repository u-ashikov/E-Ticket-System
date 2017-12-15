namespace ETicketSystem.Services.Admin.Contracts
{
	using Models;
	using System.Collections.Generic;

	public interface IAdminTownService
    {
		IEnumerable<AdminTownListingServiceModel> All(int page, int pageSize = 10);

		IEnumerable<AdminTownStationsServiceModel> TownStations(int id);

		int TotalTowns();

		bool TownExists(int id);
    }
}
