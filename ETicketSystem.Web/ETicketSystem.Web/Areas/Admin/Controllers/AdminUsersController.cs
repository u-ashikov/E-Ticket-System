namespace ETicketSystem.Web.Areas.Admin.Controllers
{
	using Common.Constants;
	using Common.Enums;
	using Data.Models;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using Models.AdminUsers;
	using Services.Admin.Contracts;
	using System.Threading.Tasks;
	using Web.Models.Pagination;

	public class AdminUsersController : BaseAdminController
    {
		public IAdminUserService users;

		private readonly UserManager<User> userManager;

		public AdminUsersController(IAdminUserService users, UserManager<User> userManager)
		{
			this.users = users;
			this.userManager = userManager;
		}

		[Route(WebConstants.Routing.AllUsers)]
		public async Task<IActionResult> All(string searchTerm, int page = 1)
		{
			if (page < 1)
			{
				return RedirectToAction(nameof(All), new { page = 1, searchTerm });
			}

			var users = await this.users.AllAsync(searchTerm, page, WebConstants.Pagination.AdminUsersListing);

			var usersPagination = new PaginationViewModel()
			{
				Action = nameof(All),
				Controller = WebConstants.Controller.AdminUsers,
				CurrentPage = page,
				PageSize = WebConstants.Pagination.AdminUsersListing,
				SearchTerm = searchTerm,
				TotalElements = this.users.TotalUsers(searchTerm)
			};

			if (page > usersPagination.TotalPages && usersPagination.TotalPages != 0)
			{
				return RedirectToAction(nameof(All), new { page = usersPagination.TotalPages, searchTerm });
			}

			return View(new AllUsers()
			{
				Users = users,
				Pagination = usersPagination
			});
		}

		[HttpPost]
		public async Task<IActionResult> ChangeRolesAsync(string userId, Role role, bool isRemove)
		{
			var user = await this.userManager.FindByIdAsync(userId);

			if (user == null)
			{
				this.GenerateAlertMessage(string.Format(WebConstants.Message.NonExistingEntity, nameof(WebConstants.Entity.User), userId), Alert.Danger);

				return RedirectToAction(nameof(All));
			}

			bool isInRole = await this.userManager.IsInRoleAsync(user, role.ToString());

			if (isRemove)
			{
				if (!isInRole)
				{
					this.GenerateAlertMessage(string.Format(WebConstants.Message.UserNotInRole, user.UserName, role.ToString()), Alert.Warning);

					return RedirectToAction(nameof(All));
				}

				await this.userManager.RemoveFromRoleAsync(user, role.ToString());

				this.GenerateAlertMessage(string.Format(WebConstants.Message.UserRemovedFromRole, user.UserName, role.ToString()), Alert.Success);
			}
			else
			{
				if (isInRole)
				{
					this.GenerateAlertMessage(string.Format(WebConstants.Message.UserAlreadyInRole, user.UserName, role.ToString()), Alert.Warning);

					return RedirectToAction(nameof(All));
				}

				await this.userManager.AddToRoleAsync(user, role.ToString());

				this.GenerateAlertMessage(string.Format(WebConstants.Message.UserAddedToRole, user.UserName, role.ToString()), Alert.Success);
			}

			return RedirectToAction(nameof(All));
		}
    }
}
