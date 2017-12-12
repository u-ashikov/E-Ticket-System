namespace ETicketSystem.Services.Implementations
{
	using AutoMapper.QueryableExtensions;
	using Contracts;
	using Data;
	using Models.Company;
	using System.Collections.Generic;
	using System.Linq;

	public class CompanyService : ICompanyService
    {
		private readonly ETicketSystemDbContext db;

		public CompanyService(ETicketSystemDbContext db)
		{
			this.db = db;
		}

		public IEnumerable<CompanyListingServiceModel> All(int page, string searchTerm, int pageSize = 10)
		{
			var companies = this.db.Companies
					.OrderBy(c=>c.Name)
					.ProjectTo<CompanyListingServiceModel>()
					.AsQueryable();

			if (!string.IsNullOrEmpty(searchTerm))
			{
				companies = companies
						.Where(c => c.Name.ToLower().Contains(searchTerm.ToLower()));
			}

			return companies
					.Skip((page - 1) * pageSize)
					.Take(pageSize)
					.ToList();
		}

		public bool IsCompanyNameRegistered(string name) =>
			this.db.Companies.Any(c => c.Name.ToLower() == name.ToLower());

		public bool IsUniqueReferenceNumberRegistered(string uniqueReferenceNumber) =>
			this.db.Companies.Any(c => c.UniqueReferenceNumber == uniqueReferenceNumber);

		public bool IsCompanyPhoneNumberRegistered(string phoneNumber) =>
			this.db.Companies.Any(c => c.PhoneNumber == phoneNumber);

		public bool IsApproved(string companyId) =>
			this.db.Companies.FirstOrDefault(c => c.Id == companyId).IsApproved;

		public int TotalCompanies(string searchTerm)
		{
			if (!string.IsNullOrEmpty(searchTerm))
			{
				return this.db.Companies.Count(c=>c.Name.ToLower().Contains(searchTerm.ToLower()));
			}

			return this.db.Companies.Count();
		}
	}
}
