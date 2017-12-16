namespace ETicketSystem.Services.Company.Contracts
{
	using Models;

	public interface ICompanyProfileService
    {
		CompanyProfileBaseServiceModel ProfileDetails(string id);
    }
}
