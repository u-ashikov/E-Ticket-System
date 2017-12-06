namespace ETicketSystem.Services.Contracts
{
	using ETicketSystem.Services.Models.Company;
	using System.Collections.Generic;

	public interface IAdminCompanyService
    {
		IEnumerable<CompanyListingServiceModel> All();
	}
}
