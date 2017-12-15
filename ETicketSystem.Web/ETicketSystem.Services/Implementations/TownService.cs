namespace ETicketSystem.Services.Implementations
{
	using AutoMapper.QueryableExtensions;
	using Contracts;
	using ETicketSystem.Data;
	using Microsoft.EntityFrameworkCore;
	using Models.Town;
	using System.Collections.Generic;
	using System.Linq;

	public class TownService : ITownService
	{
		private readonly ETicketSystemDbContext db;

		public TownService(ETicketSystemDbContext db)
		{
			this.db = db;
		}

		public IEnumerable<TownBaseServiceModel> GetTownsListItems() =>
			this.db.Towns
				.OrderBy(t=>t.Name)
				.ProjectTo<TownBaseServiceModel>()
				.ToList();

		public IEnumerable<TownStationsServiceModel> GetTownsWithStations() =>
			this.db.Towns
				.OrderBy(t=>t.Name)
				.Include(t=>t.Stations)
				.ProjectTo<TownStationsServiceModel>()
				.ToList();

		public string GetTownNameByStationId(int id)
		{
			var town = this.db.Towns.FirstOrDefault(t => t.Stations.Any(s => s.Id == id));

			if (town != null)
			{
				return town.Name;
			}

			return null;
		}

		public string GetTownNameById(int id)
		{
			var town = this.db.Towns.FirstOrDefault(t => t.Id == id);

			if (town != null)
			{
				return town.Name;
			}

			return null;
		}

		public bool TownExistsById(int id) =>
			this.db.Towns
				.Any(t => t.Id == id);
	}
}
