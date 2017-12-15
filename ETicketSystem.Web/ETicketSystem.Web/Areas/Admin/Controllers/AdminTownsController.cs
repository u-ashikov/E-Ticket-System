namespace ETicketSystem.Web.Areas.Admin.Controllers
{
	using Common.Constants;
	using ETicketSystem.Common.Enums;
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
		public IActionResult All(int page = 1)
		{
			var towns = this.adminTowns.All(page, WebConstants.Pagination.AdminTownsListing);

			return View(new AllTowns()
			{
				Towns = towns,
				Pagination = new PaginationViewModel()
				{
					Action = nameof(All),
					Controller = WebConstants.Controller.AdminTowns,
					CurrentPage = page,
					PageSize = WebConstants.Pagination.AdminTownsListing,
					TotalElements = this.adminTowns.TotalTowns()
				}
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
