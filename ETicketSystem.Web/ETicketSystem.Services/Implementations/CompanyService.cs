namespace ETicketSystem.Services.Implementations
{
	using ETicketSystem.Data;
	using ETicketSystem.Services.Contracts;
	using System.Linq;

	public class CompanyService : ICompanyService
    {
		private readonly ETicketSystemDbContext db;

		public CompanyService(ETicketSystemDbContext db)
		{
			this.db = db;
		}

		public bool IsCompanyNameRegistered(string name) =>
			this.db.Companies.Any(c => c.Name.ToLower() == name.ToLower());

		public bool IsUniqueReferenceNumberRegistered(string uniqueReferenceNumber) =>
			this.db.Companies.Any(c => c.UniqueReferenceNumber == uniqueReferenceNumber);

		public bool IsCompanyPhoneNumberRegistered(string phoneNumber) =>
			this.db.Companies.Any(c => c.PhoneNumber == phoneNumber);

		public bool IsApproved(string companyId) =>
			this.db.Companies.FirstOrDefault(c => c.Id == companyId).IsApproved;
	}
}
