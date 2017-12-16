namespace ETicketSystem.Services.Admin.Contracts
{
	using Models;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	public interface IAdminUserService
    {
		Task<IEnumerable<AdminUserListingServiceModel>> AllAsync(string searchTerm, int page, int pageSize = 10);

		int TotalUsers(string searchTerm);
    }
}
