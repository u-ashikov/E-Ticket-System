namespace ETicketSystem.Services.Admin.Contracts
{
	using Models;
	using System.Collections.Generic;

	public interface IAdminTownService
    {
		IEnumerable<AdminTownListingServiceModel> All(int page, string searchTerm, int pageSize = 10);

		IEnumerable<AdminTownStationsServiceModel> TownStations(int id);

		int TotalTowns(string searchTerm);

		bool TownExists(int id);
	}
}
