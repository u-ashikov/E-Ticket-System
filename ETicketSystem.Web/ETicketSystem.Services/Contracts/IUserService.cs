namespace ETicketSystem.Services.Contracts
{
	using Company.Models;
	using Microsoft.AspNetCore.Identity;
	using Models.User;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	public interface IUserService
    {
		Task<RegularUserProfileServiceModel> GetRegularUserProfileDetailsAsync(string id);

		CompanyProfileBaseServiceModel GetCompanyProfileDetails(string id);

		bool UserExists(string id);

		Task<IEnumerable<IdentityError>> EditRegularUserAsync(string id, string username, string email, string newPassword, string oldPassword);

		Task<IEnumerable<IdentityError>> EditCompanyAsync(string id, string username, string email, string newPassword, string oldPassword, string description, byte[] Logo, string phone);

		RegularUserProfileServiceModel GetRegularUserProfileToEdit(string id);

		byte[] GetCompanyLogo(string id);

		CompanyProfileServiceModel GetCompanyUserProfileToEdit(string id);
	}
}
