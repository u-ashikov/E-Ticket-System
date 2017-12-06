namespace ETicketSystem.Services.Contracts
{
	using ETicketSystem.Services.Models.Company;
	using System.Collections.Generic;

	public interface IAdminCompanyService
    {
		IEnumerable<CompanyListingServiceModel> All();

		bool CompanyExists(string id);

		bool Approve(string id);

		string GetCompanyName(string id);
	}
}
