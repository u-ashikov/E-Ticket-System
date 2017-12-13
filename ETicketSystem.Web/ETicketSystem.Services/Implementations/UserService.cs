namespace ETicketSystem.Services.Implementations
{
	using AutoMapper.QueryableExtensions;
	using Contracts;
	using Data;
	using Data.Models;
	using Microsoft.AspNetCore.Identity;
	using Models.User;
	using System.Linq;
	using System.Threading.Tasks;

	public class UserService : IUserService
    {
		private readonly ETicketSystemDbContext db;

		private readonly UserManager<User> userManager;

		public UserService(ETicketSystemDbContext db, UserManager<User> userManager)
		{
			this.db = db;
			this.userManager = userManager;
		}

		public async Task<RegularUserProfileServiceModel> GetRegularUserProfileDetailsAsync(string id)
		{
			var user = this.db.RegularUsers
							.Where(u => u.Id == id)
							.ProjectTo<RegularUserProfileServiceModel>()
							.FirstOrDefault();

			user.Roles = await this.userManager.GetRolesAsync(this.db.RegularUsers.Find(id));

			return user;
		}
	}
}
