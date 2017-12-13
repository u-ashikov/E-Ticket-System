namespace ETicketSystem.Web.Controllers
{
	using Common.Constants;
	using Common.Enums;
	using Data.Models;
	using ETicketSystem.Web.Models.Pagination;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using Models.Users;
	using Services.Contracts;
	using System.Linq;
	using System.Threading.Tasks;

	[Authorize]
	public class UsersController : BaseController
    {
		private readonly IUserService users;

		private readonly ITicketService tickets;

		private readonly UserManager<User> userManager;

		public UsersController(IUserService users, UserManager<User> userManager, ITicketService tickets)
		{
			this.users = users;
			this.userManager = userManager;
			this.tickets = tickets;
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

		[Route(WebConstants.Route.UserTickets)]
		public IActionResult MyTickets(string id, int page = 1)
		{
			if (this.userManager.GetUserId(User) != id)
			{
				this.GenerateAlertMessage(WebConstants.Message.NotProfileOwner, Alert.Danger);
				return RedirectToHome();
			}

			return View(new UserTickets()
			{
				Tickets = this.tickets.GetUserTickets(id,page,WebConstants.Pagination.UserTicketsPageSize),
				Pagination = new PaginationViewModel()
				{
					Action = nameof(MyTickets),
					Controller = WebConstants.Controller.Users,
					CurrentPage = page,
					PageSize = WebConstants.Pagination.UserTicketsPageSize,
					TotalElements = this.tickets.UserTicketsCount(id)
				}
			});
		}
	}
}
