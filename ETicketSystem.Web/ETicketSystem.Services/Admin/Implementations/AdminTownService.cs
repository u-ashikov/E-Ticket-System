namespace ETicketSystem.Services.Admin.Implementations
{
	using AutoMapper.QueryableExtensions;
	using Contracts;
	using Data;
	using Data.Models;
	using Models;
	using System.Collections.Generic;
	using System.Linq;

	public class AdminTownService : IAdminTownService
    {
		private readonly ETicketSystemDbContext db;

		public AdminTownService(ETicketSystemDbContext db)
		{
			this.db = db;
		}

		public IEnumerable<AdminTownListingServiceModel> All(int page, string searchTerm, int pageSize = 10)
		{
			var towns = this.db.Towns.AsQueryable();

			if (!string.IsNullOrEmpty(searchTerm))
			{
				towns = towns.Where(t => t.Name.ToLower().Contains(searchTerm.ToLower()));
			}

			return
				towns
					.OrderBy(t => t.Name)
					.Skip((page - 1) * pageSize)
					.Take(pageSize)
					.ProjectTo<AdminTownListingServiceModel>()
					.ToList();
		}

		public void Add(string name)
		{
			this.db.Towns.Add(new Town() { Name = name });
			this.db.SaveChanges();
		}

		public IEnumerable<AdminTownStationsServiceModel> TownStations(int id) =>
			this.db.Stations
				.Where(s => s.TownId == id)
				.ProjectTo<AdminTownStationsServiceModel>()
				.ToList();

		public int TotalTowns(string searchTerm)
		{
			if (string.IsNullOrEmpty(searchTerm))
			{
				return this.db.Towns.Count();
			}

			return this.db.Towns.Count(t => t.Name.ToLower().Contains(searchTerm.ToLower()));
		}

		public bool TownExists(int id) => this.db.Towns.Any(t => t.Id == id);

		public bool TownExistsByName(string name) =>
			this.db.Towns
				.Any(t => t.Name.ToLower() == name.ToLower());
	}
}
