namespace ETicketSystem.Services.Admin.Contracts
{
	using Models;
	using System.Collections.Generic;

	public interface IAdminStationService
    {
		IEnumerable<AdminStationListingServiceModel> All(string searchTerm,int page = 1, int pageSize = 10);

		bool Add(string name, int townId, string phone);

		bool Edit(int stationId, string name, string phone, int townId);

		bool StationExists(int id);

		bool EditedStationIsSame(int id, string name, string phone, int townId);

		AdminStationEditServiceModel GetStationToEdit(int id);

		int TotalStations(string searchTerm);
	}
}
