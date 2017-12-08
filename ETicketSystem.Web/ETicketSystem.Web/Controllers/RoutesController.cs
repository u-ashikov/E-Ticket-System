namespace ETicketSystem.Web.Controllers
{
	using ETicketSystem.Common.Constants;
	using ETicketSystem.Common.Enums;
	using ETicketSystem.Services.Contracts;
	using Microsoft.AspNetCore.Mvc;
	using Models.Routes;

	public class RoutesController : BaseController
    {
		private readonly ITownService towns;

		private readonly IRouteService routes;

		public RoutesController(ITownService towns, IRouteService routes)
		{
			this.towns = towns;
			this.routes = routes;
		}

		[Route(WebConstants.Route.RoutesSearch)]
		public IActionResult Search(SearchRouteFormModel model)
		{
			if (!this.towns.TownExists(model.StartTown) || !this.towns.TownExists(model.StartTown))
			{
				this.GenerateAlertMessage(WebConstants.Message.InvalidTown,Alert.Danger);
				return RedirectToAction(nameof(HomeController.Index), WebConstants.Controller.Home);
			}

			return View(this.routes.GetSearchedRoutes(model.StartTown, model.EndTown, model.Date));
		}
	}
}
