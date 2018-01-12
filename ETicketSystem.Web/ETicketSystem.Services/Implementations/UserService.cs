namespace ETicketSystem.Services.Implementations
{
	using AutoMapper.QueryableExtensions;
	using Common.Constants;
	using Company.Models;
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

        public CompanyProfileBaseServiceModel GetCompanyProfileDetails(string id) =>
            this.db
                .Companies
                .Where(c => c.Id == id)
                .ProjectTo<CompanyProfileBaseServiceModel>()
                .FirstOrDefault();

        public CompanyProfileServiceModel GetCompanyUserProfileToEdit(string id) =>
			this.db.Companies
				.Where(c => c.Id == id)
				.ProjectTo<CompanyProfileServiceModel>()
				.FirstOrDefault();

		public async Task<IEnumerable<IdentityError>> EditRegularUserAsync(string id, string username, string email,string newPassword, string oldPassword)
		{
			var user = this.db.Users.Find(id);
			var errors = new HashSet<IdentityError>();

			await this.EditUserBaseInfo(id, username, email, oldPassword, newPassword, errors, user);

			return errors;
		}

		public async Task<IEnumerable<IdentityError>> EditCompanyAsync(string id, string username, string email, string newPassword, string oldPassword, string description, byte[] logo, string phone)
		{
			var company = this.db.Companies.Find(id);
			var errors = new HashSet<IdentityError>();

			await this.EditUserBaseInfo(id, username, email, oldPassword, newPassword, errors, company);

			if (logo != null)
			{
				company.Logo = logo;
			}

			company.Description = description;

			this.db.SaveChanges();

			var phoneToken = await this.userManager.GenerateChangePhoneNumberTokenAsync(company, phone);

			await this.userManager.ChangePhoneNumberAsync(company, phone, phoneToken);

			return errors;
		}

		public byte[] GetCompanyLogo(string id)
		{
			var company = this.db.Companies.FirstOrDefault(c => c.Id == id);

			if (company == null)
			{
				return null;
			}

			return company.Logo;
		}

		private async Task EditUserBaseInfo(string id,string username, string email, string oldPassword, string newPassword, HashSet<IdentityError> errors, User user)
		{
			if (string.IsNullOrEmpty(username))
			{
				errors.Add(new IdentityError()
				{
					Description = WebConstants.Message.EmptyUsername
				});

				return;
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

					return;
				}
			}

			if ((!string.IsNullOrEmpty(oldPassword) && string.IsNullOrEmpty(newPassword))
				|| (string.IsNullOrEmpty(oldPassword) && !string.IsNullOrEmpty(newPassword)))
			{
				errors.Add(new IdentityError()
				{
					Description = WebConstants.Message.BothPasswordFieldsRequired
				});

				return;
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

					return;
				}

				var passwordChanged = await this.userManager.ChangePasswordAsync(user, oldPassword, newPassword);

				if (!passwordChanged.Succeeded)
				{
					foreach (var error in passwordChanged.Errors)
					{
						errors.Add(error);
					}

					return;
				}
			}
		}
	}
}
