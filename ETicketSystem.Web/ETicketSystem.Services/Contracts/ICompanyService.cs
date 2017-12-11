namespace ETicketSystem.Services.Contracts
{
	using Models.Company;
	using System.Collections.Generic;

	public interface ICompanyService
    {
		IEnumerable<CompanyListingServiceModel> All(int page, int pageSize = 10);

		bool IsCompanyNameRegistered(string name);

		bool IsUniqueReferenceNumberRegistered(string uniqueReferenceNumber);

		bool IsCompanyPhoneNumberRegistered(string phoneNumber);

		bool IsApproved(string companyId);

		int TotalCompanies();
    }
}
