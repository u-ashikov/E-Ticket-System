namespace ETicketSystem.Web.Controllers
{
	using Common.Constants;
	using Common.Enums;
	using Data.Models;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using Models.Users;
	using Services.Contracts;
	using System.Linq;
	using System.Threading.Tasks;

	public class UsersController : BaseController
    {
		private readonly IUserService users;

		private readonly UserManager<User> userManager;

		public UsersController(IUserService users, UserManager<User> userManager)
		{
			this.users = users;
			this.userManager = userManager;
		}

		public async Task<IActionResult> Profile(string id)
		{
			if (this.userManager.GetUserId(User) != id)
			{
				this.GenerateAlertMessage(WebConstants.Message.NotProfileOwner, Alert.Danger);
				return RedirectToHome();
			}

			var user = await this.users.GetRegularUserProfileDetailsAsync(id);

			if (user == null)
			{
				this.GenerateAlertMessage(string.Format(WebConstants.Message.NonExistingUser, id), Alert.Danger);
				return RedirectToHome();
			}

			return View(user);
		}

		[Route(WebConstants.Route.EditUser)]
		public IActionResult EditUser(string id)
		{
			if (!this.users.UserExists(id))
			{
				this.GenerateAlertMessage(string.Format(WebConstants.Message.NonExistingUser, id), Alert.Danger);
				return RedirectToHome();
			}

			if (this.userManager.GetUserId(User) != id)
			{
				this.GenerateAlertMessage(string.Format(WebConstants.Message.NotProfileOwner, id), Alert.Danger);
				return RedirectToHome();
			}

			var user = this.users.GetRegularUserProfileToEdit(id);

			return View(new EditUserProfileFormModel()
			{
				Username = user.Username,
				Email = user.Email
			});
		}

		[HttpPost]
		[Route(WebConstants.Route.EditUser)]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditUser(string id, EditUserProfileFormModel profile)
		{
			if (!ModelState.IsValid)
			{
				return View(profile);
			}

			if (!this.users.UserExists(id) || this.userManager.GetUserId(User) != id)
			{
				return BadRequest();
			}

			var errors = await this.users.EditRegularUserAsync(id, profile.Username, profile.Email, profile.NewPassword, profile.Password);

			if (errors.Count() != 0)
			{
				foreach (var error in errors)
				{
					ModelState.AddModelError("", error.Description);
				}

				return View(profile);
			}

			this.GenerateAlertMessage(WebConstants.Message.ProfileEdited, Alert.Success);

			return RedirectToAction(nameof(Profile), new { id = id });
		}
	}
}
