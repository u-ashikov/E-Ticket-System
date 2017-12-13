namespace ETicketSystem.Services.Contracts
{
	using Models.User;
	using System.Threading.Tasks;

	public interface IUserService
    {
		Task<RegularUserProfileServiceModel> GetRegularUserProfileDetailsAsync(string id);
	}
}
