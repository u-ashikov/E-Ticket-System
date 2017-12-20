namespace ETicketSystem.Services.Implementations
{
	using Contracts;
	using Data;
	using System.Linq;

	public class StationService : IStationService
    {
		private readonly ETicketSystemDbContext db;

		public StationService(ETicketSystemDbContext db)
		{
			this.db = db;
		}

		public bool StationExist(int id) =>
			this.db.Stations.Any(s => s.Id == id);

		public bool AreStationsInSameTown(int startStation, int endStation) =>
			this.db.Stations.Find(startStation).TownId == this.db.Stations.Find(endStation).TownId;
	}
}
