namespace ETicketSystem.Services.Admin.Implementations
{
	using Contracts;
	using Data;
	using Data.Models;
	using System.Linq;

	public class AdminStationService : IAdminStationService
	{
		private readonly ETicketSystemDbContext db;

		public AdminStationService(ETicketSystemDbContext db)
		{
			this.db = db;
		}

		public bool Add(string name, int townId, string phone)
		{
			if (this.db.Stations.Any(s=>s.Name.ToLower() == name.ToLower() && s.TownId == townId))
			{
				return false;
			}

			var station = new Station()
			{
				Name = name,
				TownId = townId,
				Phone = phone
			};

			this.db.Stations.Add(station);
			this.db.SaveChanges();

			return true;
		}
	}
}
