namespace ETicketSystem.Web.Controllers
{
	using Common.Constants;
	using Common.Enums;
	using Data.Models;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using Services.Contracts;
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
    }
}
