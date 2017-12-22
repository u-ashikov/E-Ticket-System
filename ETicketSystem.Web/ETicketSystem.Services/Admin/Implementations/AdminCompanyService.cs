namespace ETicketSystem.Services.Admin.Implementations
{
	using AutoMapper.QueryableExtensions;
	using Common.Enums;
	using Contracts;
	using Data;
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
				.OrderBy(c=>c.RegistrationDate)
				.ThenBy(c => c.Name)
				.AsQueryable();

			if (filter == CompanyStatus.Approved.ToString())
			{
				companies = companies.Where(c => c.IsApproved);
			}
			else if (filter == CompanyStatus.Unapproved.ToString())
			{
				companies = companies.Where(c => !c.IsApproved);
			}
			else if (filter == CompanyStatus.Blocked.ToString())
			{
				companies = companies.Where(c => c.IsBlocked);
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

		public bool ChangeStatus(string id)
		{
			var company = this.db.Companies.FirstOrDefault(c=>c.Id == id && c.IsApproved);

			if (company == null)
			{
				return false;
			}

			if (company.IsBlocked)
			{
				company.IsBlocked = false;
			}
			else
			{
				company.IsBlocked = true;
			}

			this.db.SaveChanges();

			return true;
		}

		public string GetBlockStatus(string id)
		{
			var company = this.db.Companies.FirstOrDefault(c => c.Id == id);

			if (company == null)
			{
				return string.Empty;
			}

			return company.IsBlocked ? CompanyStatus.Blocked.ToString().ToLower() : string.Concat("un", CompanyStatus.Blocked.ToString().ToLower());
		}
	}
}
