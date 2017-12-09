namespace ETicketSystem.Web.Controllers
{
	using ETicketSystem.Common.Constants;
	using ETicketSystem.Common.Enums;
	using ETicketSystem.Services.Contracts;
	using Microsoft.AspNetCore.Mvc;
	using Models.Routes;
	using System;

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

			if (model.Date.Date < DateTime.UtcNow.Date)
			{
				this.GenerateAlertMessage(WebConstants.Message.InvalidDate, Alert.Danger);
				return RedirectToAction(nameof(HomeController.Index), WebConstants.Controller.Home);
			}

			return View(this.routes.GetSearchedRoutes(model.StartTown, model.EndTown, model.Date));
		}

		[Route(WebConstants.Route.BookRouteTicket)]
		public IActionResult BookTicket(int id, int startTown, int endTown, DateTime date)
		{
			var form = new BookTicketFormModel();

			for (int i = 1; i <= 40; i++)
			{
				form.Seats.Add(new BookSeatViewModel()
				{
					Value = i,
					Checked = false,
					Text = i.ToString()
				});
			}


			return View(form);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult BuyTicket(BookTicketFormModel model)
		{
			return null;
		}
	}
}
