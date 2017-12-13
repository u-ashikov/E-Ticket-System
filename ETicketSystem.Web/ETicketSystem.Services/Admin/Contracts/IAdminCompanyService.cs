namespace ETicketSystem.Services.Admin.Contracts
{
	using Models;
	using System.Collections.Generic;

	public interface IAdminCompanyService
    {
		IEnumerable<AdminCompanyListingServiceModel> All(int page, string filter, int pageSize = 10);

		bool CompanyExists(string id);

		bool Approve(string id);

		bool ChangeStatus(string id);

		string GetBlockStatus(string id);

		string GetCompanyName(string id);

		int TotalCompanies(string filter);
	}
}
