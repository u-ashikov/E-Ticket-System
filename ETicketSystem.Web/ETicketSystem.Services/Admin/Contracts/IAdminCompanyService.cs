namespace ETicketSystem.Services.Admin.Contracts
{
	using ETicketSystem.Services.Admin.Models;
	using System.Collections.Generic;

	public interface IAdminCompanyService
    {
		IEnumerable<AdminCompanyListingServiceModel> All();

		bool CompanyExists(string id);

		bool Approve(string id);

		string GetCompanyName(string id);
	}
}
