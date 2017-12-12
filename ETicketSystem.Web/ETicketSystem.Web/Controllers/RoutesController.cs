namespace ETicketSystem.Web.Controllers
{
	using Common.Constants;
	using Common.Enums;
	using Data.Models;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using Models.Routes;
	using Services.Contracts;
	using System;
	using System.Linq;

	[Authorize]
	public class RoutesController : BaseController
    {
		private readonly ITownService towns;

		private readonly IRouteService routes;

		private readonly ITicketService tickets;

		private readonly ICompanyService companies;

		private readonly UserManager<User> userManager;

		public RoutesController(ITownService towns, IRouteService routes, ITicketService tickets, UserManager<User> userManager)
		{
			this.towns = towns;
			this.routes = routes;
			this.tickets = tickets;
			this.userManager = userManager;
		}

		[Route(WebConstants.Route.RoutesSearch)]
		[AllowAnonymous]
		public IActionResult Search(SearchRouteFormModel model)
		{
			if (!this.towns.TownExists(model.StartTown) || !this.towns.TownExists(model.EndTown))
			{
				this.GenerateAlertMessage(WebConstants.Message.InvalidTown,Alert.Danger);
				return this.RedirectToHome();
			}

			if (model.Date.Date < DateTime.UtcNow.Date)
			{
				this.GenerateAlertMessage(WebConstants.Message.InvalidDate, Alert.Danger);
				return this.RedirectToHome();
			}

			return View(this.routes.GetSearchedRoutes(model.StartTown, model.EndTown, model.Date,model.CompanyId));
		}

		[Route(WebConstants.Route.BookRouteTicket)]
		public IActionResult BookTicket(int id, TimeSpan departureTime, DateTime date)
		{
			var departureDateTime = new DateTime(date.Year, date.Month, date.Day, departureTime.Hours, departureTime.Minutes, departureTime.Seconds);

			if (!this.routes.RouteExists(id, departureTime) || departureDateTime < DateTime.UtcNow)
			{
				this.GenerateAlertMessage(WebConstants.Message.InvalidRoute, Alert.Danger);
				return this.RedirectToHome();
			}

			var form = new BookTicketFormModel();

			var info = this.routes.GetRouteTicketBookingInfo(id, departureDateTime);

			this.GenerateBusSchemaSeats(form, info);

			form.BusSeats = (int)info.BusType;
			form.DepartureDateTime = departureDateTime;
			form.Duration = info.Duration;
			form.RouteId = id;
			form.StartTownId = info.StartTownId;
			form.EndTownId = info.EndTownId;
			form.CompanyName = info.CompanyName;
			form.StartStation = info.StartStation;
			form.EndStation = info.EndStation;

			return View(form);
		}

		[HttpPost]
		[Route(WebConstants.Route.BookRouteTicket)]
		[ValidateAntiForgeryToken]
		public IActionResult BookTicket(BookTicketFormModel model)
		{
			if (!ModelState.IsValid)
			{
				ModelState.AddModelError(string.Empty, WebConstants.Message.NoneSelectedSeats);
				this.GenerateBusSchemaSeats(model, this.routes.GetRouteTicketBookingInfo(model.RouteId,model.DepartureDateTime));
				return View(model);
			}

			var reservedTickets = model.Seats.Where(s => s.Checked).Select(s => s.Value).ToList();

			var success = this.tickets.Add(model.RouteId, model.DepartureDateTime, reservedTickets, this.userManager.GetUserId(User));

			if (!success)
			{
				this.GenerateAlertMessage(WebConstants.Message.InvalidRoute, Alert.Danger);
				return this.RedirectToHome();
			}

			this.GenerateAlertMessage(string.Format(WebConstants.Message.SuccessfullyTicketReservation, string.Join(", ", reservedTickets), model.StartStation, model.EndStation, model.DepartureDateTime), Alert.Success);

			return RedirectToHome();
		}

		private void GenerateBusSchemaSeats(BookTicketFormModel form, Services.Models.Route.RouteBookTicketInfoServiceModel info)
		{
			for (int i = 1; i <= (int)info.BusType; i++)
			{
				form.Seats.Add(new BookSeatViewModel()
				{
					Value = i,
					Checked = false,
					Text = i.ToString(),
					Disabled = info.ReservedTickets.Any(n => n == i)
				});
			}
		}
	}
}
