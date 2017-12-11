namespace ETicketSystem.Services.Implementations
{
	using Data;
	using Contracts;
	using Models.Company;
	using System.Collections.Generic;
	using System.Linq;
	using AutoMapper.QueryableExtensions;

	public class CompanyService : ICompanyService
    {
		private readonly ETicketSystemDbContext db;

		public CompanyService(ETicketSystemDbContext db)
		{
			this.db = db;
		}

		public IEnumerable<CompanyListingServiceModel> All(int page, int pageSize = 10) =>
			this.db.Companies
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.OrderBy(c => c.Name)
				.ProjectTo<CompanyListingServiceModel>()
				.ToList();

		public bool IsCompanyNameRegistered(string name) =>
			this.db.Companies.Any(c => c.Name.ToLower() == name.ToLower());

		public bool IsUniqueReferenceNumberRegistered(string uniqueReferenceNumber) =>
			this.db.Companies.Any(c => c.UniqueReferenceNumber == uniqueReferenceNumber);

		public bool IsCompanyPhoneNumberRegistered(string phoneNumber) =>
			this.db.Companies.Any(c => c.PhoneNumber == phoneNumber);

		public bool IsApproved(string companyId) =>
			this.db.Companies.FirstOrDefault(c => c.Id == companyId).IsApproved;

		public int TotalCompanies() =>
			this.db.Companies.Count();
	}
}
