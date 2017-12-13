namespace ETicketSystem.Services.Admin.Implementations
{
	using AutoMapper.QueryableExtensions;
	using Contracts;
	using ETicketSystem.Common.Enums;
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

		public IEnumerable<AdminCompanyListingServiceModel> All(int page, string filter, int pageSize = 10)
		{
			var companies = this.db.Companies
				.OrderBy(c => c.RegistrationDate)
				.AsQueryable();

			switch (filter)
			{
				case "Approved":
					companies = companies.Where(c => c.IsApproved);
					break;
				case "Banned":
					companies = companies.Where(c => c.IsBlocked);
					break;
				case "Unapproved":
					companies = companies.Where(c => !c.IsApproved);
					break;
				default:
					break;
			}

			return companies
						.Skip((page - 1) * pageSize)
						.Take(pageSize)
						.ProjectTo<AdminCompanyListingServiceModel>()
						.ToList();
		}

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

		public int TotalCompanies(string filter)
		{
			if (string.IsNullOrEmpty(filter) || filter == CompanyStatus.All.ToString())
			{
				return this.db.Companies.Count();
			}

			if (filter == CompanyStatus.Approved.ToString())
			{
				return this.db.Companies.Count(c => c.IsApproved);
			}

			if (filter == CompanyStatus.Unapproved.ToString())
			{
				return this.db.Companies.Count(c => !c.IsApproved);
			}

			return this.db.Companies.Count(c => c.IsBlocked);
		}
	}
}
