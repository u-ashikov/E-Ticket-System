namespace ETicketSystem.Web.Areas.Admin.Controllers
{
	using Common.Constants;
	using Data.Models;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using Models.AdminUsers;
	using Services.Admin.Contracts;
	using System.Linq;
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

		[Route(WebConstants.Route.AllUsers)]
		public async Task<IActionResult> All(string searchTerm, int page = 1)
		{
			if (page < 1)
			{
				return RedirectToAction(nameof(All), new { page = 1, searchTerm = searchTerm });
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

			if (page > usersPagination.TotalElements && usersPagination.TotalPages != 0)
			{
				return RedirectToAction(nameof(All), new { page = usersPagination.TotalPages, searchTerm = searchTerm });
			}

			return View(new AllUsers()
			{
				Users = users,
				Pagination = usersPagination
			});
		}
    }
}
