namespace ETicketSystem.Services.Admin.Implementations
{
	using AutoMapper.QueryableExtensions;
	using Contracts;
	using ETicketSystem.Data;
	using Models;
	using System.Collections.Generic;
	using System.Linq;

	public class AdminCompanyService : IAdminCompanyService
    {
		private readonly ETicketSystemDbContext db;

		public AdminCompanyService(ETicketSystemDbContext db)
		{
			this.db = db;
		}

		public IEnumerable<AdminCompanyListingServiceModel> All() =>
			this.db.Companies
				.OrderBy(c => c.RegistrationDate)
				.ProjectTo<AdminCompanyListingServiceModel>()
				.ToList();

		public bool CompanyExists(string id) => this.db.Companies.Any(c => c.Id == id);

		public bool Approve(string id)
		{
			var company = this.db.Companies.FirstOrDefault(c => c.Id == id);

			if (company.IsApproved)
			{
				return false;
			}

			company.IsApproved = true;
			this.db.SaveChanges();

			return true;
		}

		public string GetCompanyName(string id)
		{
			if (!this.CompanyExists(id))
			{
				return string.Empty;
			}

			return this.db.Companies.FirstOrDefault(c => c.Id == id).Name;
		}
	}
}
