namespace ETicketSystem.Services.Admin.Contracts
{
	using Models;
	using System.Collections.Generic;

	public interface IAdminTownService
    {
		IEnumerable<AdminTownListingServiceModel> All(int page, string searchTerm, int pageSize = 10);

		void Add(string name);

		bool TownExists(int id);

		bool TownExistsByName(string name);

		IEnumerable<AdminTownStationsServiceModel> TownStations(int id);

		int TotalTowns(string searchTerm);
	}
}
