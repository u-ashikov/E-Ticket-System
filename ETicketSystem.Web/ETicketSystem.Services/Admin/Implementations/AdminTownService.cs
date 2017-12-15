namespace ETicketSystem.Services.Admin.Implementations
{
	using AutoMapper.QueryableExtensions;
	using Contracts;
	using ETicketSystem.Data;
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

		public IEnumerable<AdminTownListingServiceModel> All(int page, int pageSize = 10) =>
			this.db.Towns
				.OrderBy(t=>t.Name)
				.Skip((page-1)*pageSize)
				.Take(pageSize)
				.ProjectTo<AdminTownListingServiceModel>()
				.ToList();

		public AdminTownStationsServiceModel TownStations(int id) =>
			this.db.Towns
				.Where(t => t.Id == id)
				.ProjectTo<AdminTownStationsServiceModel>()
				.FirstOrDefault();

		public int TotalTowns() => this.db.Towns.Count();

		public bool TownExists(int id) => this.db.Towns.Any(t => t.Id == id);
	}
}
