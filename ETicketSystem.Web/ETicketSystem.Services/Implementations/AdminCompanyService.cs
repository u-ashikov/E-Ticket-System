namespace ETicketSystem.Services.Implementations
{
	using AutoMapper.QueryableExtensions;
	using ETicketSystem.Data;
	using ETicketSystem.Services.Contracts;
	using ETicketSystem.Services.Models.Company;
	using System.Collections.Generic;
	using System.Linq;

	public class AdminCompanyService : IAdminCompanyService
    {
		private readonly ETicketSystemDbContext db;

		public AdminCompanyService(ETicketSystemDbContext db)
		{
			this.db = db;
		}

		public IEnumerable<CompanyListingServiceModel> All() =>
			this.db.Companies
				.OrderBy(c => c.RegistrationDate)
				.ProjectTo<CompanyListingServiceModel>()
				.ToList();
	}
}
