namespace ETicketSystem.Services.Contracts
{
	public interface IStationService
    {
		bool AreStationsInSameTown(int startStation, int endStation);

		bool StationExist(int id);
	}
}
