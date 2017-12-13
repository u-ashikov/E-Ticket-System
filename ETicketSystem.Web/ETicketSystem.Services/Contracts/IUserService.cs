namespace ETicketSystem.Services.Contracts
{
	using Microsoft.AspNetCore.Identity;
	using Models.User;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	public interface IUserService
    {
		Task<RegularUserProfileServiceModel> GetRegularUserProfileDetailsAsync(string id);

		bool UserExists(string id);

		Task<IEnumerable<IdentityError>> EditRegularUserAsync(string id, string username, string email, string newPassword, string oldPassword);

		RegularUserProfileServiceModel GetRegularUserProfileToEdit(string id);
	}
}
