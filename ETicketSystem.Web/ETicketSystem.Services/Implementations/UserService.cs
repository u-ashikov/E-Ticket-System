namespace ETicketSystem.Services.Implementations
{
	using AutoMapper.QueryableExtensions;
	using Common.Constants;
	using Contracts;
	using Data;
	using Data.Models;
	using Microsoft.AspNetCore.Identity;
	using Models.User;
	using System.Collections.Generic;
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

		public bool UserExists(string id) =>
			this.db.Users.Any(u => u.Id == id);

		public RegularUserProfileServiceModel GetRegularUserProfileToEdit(string id) =>
			this.db.Users
				.Where(u => u.Id == id)
				.ProjectTo<RegularUserProfileServiceModel>()
				.FirstOrDefault();

		public async Task<IEnumerable<IdentityError>> EditRegularUserAsync(string id, string username, string email,string newPassword, string oldPassword)
		{
			var user = this.db.Users.Find(id);
			var errors = new HashSet<IdentityError>();

			if (string.IsNullOrEmpty(username))
			{
				errors.Add(new IdentityError()
				{
					Description = WebConstants.Message.EmptyUsername
				});

				return errors;
			}

			user.UserName = username;

			if (!string.IsNullOrEmpty(email))
			{
				var emailToken = await this.userManager.GenerateChangeEmailTokenAsync(user, email);
				var emailChanged = await this.userManager.ChangeEmailAsync(user, email, emailToken);

				if (!emailChanged.Succeeded)
				{
					foreach (var error in emailChanged.Errors)
					{
						errors.Add(error);
					}

					return errors;
				}
			}

			if ((!string.IsNullOrEmpty(oldPassword) && string.IsNullOrEmpty(newPassword))
				|| (string.IsNullOrEmpty(oldPassword) && !string.IsNullOrEmpty(newPassword)))
			{
				errors.Add(new IdentityError()
				{
					Description = WebConstants.Message.BothPasswordFieldsRequired
				});

				return errors;
			}

			if (!string.IsNullOrEmpty(newPassword) && !string.IsNullOrEmpty(oldPassword))
			{
				bool oldPasswordMatch = await this.userManager.CheckPasswordAsync(user, oldPassword);

				if (!oldPasswordMatch)
				{
					errors.Add(new IdentityError()
					{
						Description = WebConstants.Message.IncorrectOldPassword
					});

					return errors;
				}

				var passwordChanged = await this.userManager.ChangePasswordAsync(user, oldPassword, newPassword);

				if (!passwordChanged.Succeeded)
				{
					foreach (var error in passwordChanged.Errors)
					{
						errors.Add(error);
					}

					return errors;
				}
			}

			return errors;
		}
	}
}
