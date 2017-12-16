namespace ETicketSystem.Services.Company.Implementations
{
	using AutoMapper.QueryableExtensions;
	using Contracts;
	using Data;
	using Models;
	using System.Linq;

	public class CompanyProfileService : ICompanyProfileService
	{
		private readonly ETicketSystemDbContext db;

		public CompanyProfileService(ETicketSystemDbContext db)
		{
			this.db = db;
		}

		public CompanyProfileBaseServiceModel ProfileDetails(string id) =>
			this.db
				.Companies
				.Where(c => c.Id == id)
				.ProjectTo<CompanyProfileBaseServiceModel>()
				.FirstOrDefault();
	}
}
