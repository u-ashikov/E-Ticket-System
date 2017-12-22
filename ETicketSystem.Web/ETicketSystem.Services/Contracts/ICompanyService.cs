namespace ETicketSystem.Services.Contracts
{
	using Models.Company;
	using System.Collections.Generic;

	public interface ICompanyService
    {
		IEnumerable<CompanyListingServiceModel> All(int page, string searchTerm, int pageSize = 10);

		bool IsCompanyNameRegistered(string name);

		bool IsUniqueReferenceNumberRegistered(string uniqueReferenceNumber);

		bool IsApproved(string companyId);

		bool IsBlocked(string companyId);

		bool Exists(string companyId);

		int TotalCompanies(string searchTerm);

		IEnumerable<CompanyBaseServiceModel> GetCompaniesSelectListItems();

		CompanyDetailsServiceModel CompanyDetails(string id);
    }
}
