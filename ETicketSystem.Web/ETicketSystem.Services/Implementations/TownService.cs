namespace ETicketSystem.Services.Implementations
{
	using AutoMapper.QueryableExtensions;
	using ETicketSystem.Data;
	using ETicketSystem.Services.Contracts;
	using ETicketSystem.Services.Models.Town;
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
	}
}
