namespace ETicketSystem.Services.Admin.Implementations
{
	using AutoMapper.QueryableExtensions;
	using Contracts;
	using Data;
	using Data.Models;
	using Models;
	using System.Collections.Generic;
	using System.Linq;

	public class AdminStationService : IAdminStationService
	{
		private readonly ETicketSystemDbContext db;

		public AdminStationService(ETicketSystemDbContext db)
		{
			this.db = db;
		}

		public IEnumerable<AdminStationListingServiceModel> All(string searchTerm, int page = 1, int pageSize = 10)
		{
			var stations = this.db.Stations.AsQueryable();

			if (!string.IsNullOrEmpty(searchTerm))
			{
				stations = stations.Where(s => s.Name.ToLower().Contains(searchTerm.ToLower()));
			}

			return stations
					.OrderBy(s => s.Name)
					.Skip((page - 1) * pageSize)
					.Take(pageSize)
					.ProjectTo<AdminStationListingServiceModel>()
					.ToList();
		}

		public bool Add(string name, int townId, string phone)
		{
			if (this.db.Stations.Any(s => s.Name.ToLower() == name.ToLower() && s.TownId == townId))
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

		public AdminStationEditServiceModel GetStationToEdit(int id) =>
			this.db.Stations
				.Where(s => s.Id == id)
				.ProjectTo<AdminStationEditServiceModel>()
				.FirstOrDefault();

		public bool StationExists(int id) => this.db.Stations.Any(s => s.Id == id);

		public bool EditedStationIsSame(int id, string name, string phone, int townId)
		{
			var stationToCompare = this.db.Stations.Find(id);

			return stationToCompare.Name.ToLower() == name.ToLower()
				&& stationToCompare.Phone == phone
				&& stationToCompare.TownId == townId;
		}

		public bool Edit(int stationId,string name, string phone, int townId)
		{
			if (this.db.Stations.Any(s=>s.Id != stationId && s.Name.ToLower() == name.ToLower() && s.TownId == townId))
			{
				return false;
			}

			var stationToEdit = this.db.Stations.FirstOrDefault(s => s.Id == stationId);

			stationToEdit.Name = name;
			stationToEdit.Phone = phone;

			this.db.SaveChanges();

			return true;
		}

		public int TotalStations(string searchTerm)
		{
			if (string.IsNullOrEmpty(searchTerm))
			{
				return this.db.Stations.Count();
			}

			return this.db.Stations.Count(s => s.Name.ToLower().Contains(searchTerm.ToLower()));
		}
	}
}