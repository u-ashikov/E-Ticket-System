namespace ETicketSystem.Services.Admin.Contracts
{
	using Models;

	public interface IAdminStationService
    {
		bool Add(string name, int townId, string phone);

		bool Edit(int stationId, string name, string phone, int townId);

		bool StationExists(int id);

		bool EditedStationIsSame(int id, string name, string phone, int townId);

		AdminStationEditServiceModel GetStationToEdit(int id);
	}
}
