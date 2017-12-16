namespace ETicketSystem.Services.Admin.Implementations
{
	using AutoMapper.QueryableExtensions;
	using Contracts;
	using Data;
	using Data.Models;
	using Microsoft.AspNetCore.Identity;
	using Models;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	public class AdminUserService : IAdminUserService
	{
		private readonly ETicketSystemDbContext db;

		private readonly UserManager<User> userManager;

		public AdminUserService(ETicketSystemDbContext db, UserManager<User> userManager)
		{
			this.db = db;
			this.userManager = userManager;
		}

		public async Task<IEnumerable<AdminUserListingServiceModel>> AllAsync(string searchTerm,int page, int pageSize = 10)
		{
			var users = this.db.RegularUsers.AsQueryable();

			if (!string.IsNullOrEmpty(searchTerm))
			{
				users = users.Where(u => u.UserName.ToLower().Contains(searchTerm.ToLower()) || u.FirstName.ToLower().Contains(searchTerm.ToLower()) || u.LastName.ToLower().Contains(searchTerm.ToLower()));
			}

			var allUsers =  users
						.OrderBy(u => u.UserName)
						.ThenBy(u => u.FirstName)
						.ThenBy(u => u.LastName)
						.Skip((page - 1) * pageSize)
						.Take(pageSize)
						.ProjectTo<AdminUserListingServiceModel>()
						.ToList();

			foreach (var user in allUsers)
			{
				user.Roles = await this.userManager.GetRolesAsync(this.db.RegularUsers.Find(user.Id));
			}

			return allUsers;
		}

		public int TotalUsers(string searchTerm)
		{
			if (string.IsNullOrEmpty(searchTerm))
			{
				return this.db.RegularUsers.Count();
			}

			return this.db.RegularUsers.Count(u => u.UserName.ToLower().Contains(searchTerm.ToLower()) || u.FirstName.ToLower().Contains(searchTerm.ToLower()) || u.LastName.ToLower().Contains(searchTerm.ToLower()));
		}
	}
}
