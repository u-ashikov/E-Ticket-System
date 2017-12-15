namespace ETicketSystem.Web.Areas.Admin.Controllers
{
	using Common.Constants;
	using Common.Enums;
	using Microsoft.AspNetCore.Mvc;
	using Models.AdminTowns;
	using Services.Admin.Contracts;
	using Web.Models.Pagination;

	public class AdminTownsController : BaseAdminController
	{
		private readonly IAdminTownService adminTowns;

		public AdminTownsController(IAdminTownService adminTowns)
		{
			this.adminTowns = adminTowns;
		}

		[Route(WebConstants.Route.AllTowns)]
		public IActionResult All(string searchTerm,int page = 1)
		{
			if (page < 1)
			{
				return RedirectToAction(nameof(All), new { page = 1, searchTerm = searchTerm });
			}

			var towns = this.adminTowns.All(page, searchTerm, WebConstants.Pagination.AdminTownsListing);
			var townsPagination = new PaginationViewModel()
			{
				Action = nameof(All),
				Controller = WebConstants.Controller.AdminTowns,
				CurrentPage = page,
				PageSize = WebConstants.Pagination.AdminTownsListing,
				TotalElements = this.adminTowns.TotalTowns(searchTerm),
				SearchTerm = searchTerm
			};

			if (page > townsPagination.TotalPages && townsPagination.TotalPages != 0)
			{
				return RedirectToAction(nameof(All), new { page = townsPagination.TotalPages, searchTerm = searchTerm });
			}

			return View(new AllTowns()
			{
				Towns = towns,
				Pagination = townsPagination
			});
		}

		[Route(WebConstants.Route.TownStations)]
		public IActionResult TownStations(int id)
		{
			if (!this.adminTowns.TownExists(id))
			{
				this.GenerateAlertMessage(string.Format(WebConstants.Message.NonExistingTown, id), Alert.Success);

				return RedirectToAction(nameof(All));
			}

			return Json(this.adminTowns.TownStations(id));
		}
    }
}
