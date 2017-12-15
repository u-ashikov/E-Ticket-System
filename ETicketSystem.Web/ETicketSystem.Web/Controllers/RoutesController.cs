namespace ETicketSystem.Web.Controllers
{
	using Common.Constants;
	using Common.Enums;
	using Data.Models;
	using Infrastructure.Extensions;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.Rendering;
	using Models.Pagination;
	using Models.Routes;
	using Services.Contracts;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	[Authorize]
	public class RoutesController : BaseController
    {
		private readonly IRouteService routes;

		private readonly ITicketService tickets;

		private readonly ICompanyService companies;

		private readonly UserManager<User> userManager;

		public RoutesController(ITownService towns, IRouteService routes, ITicketService tickets, ICompanyService companies,UserManager<User> userManager)
			:base(towns)
		{
			this.routes = routes;
			this.tickets = tickets;
			this.userManager = userManager;
			this.companies = companies;
		}

		[AllowAnonymous]
		[Route(WebConstants.Route.RoutesSearch)]
		public IActionResult Search(int startTown, int endTown, DateTime date, string companyId, int page = 1)
		{
			if (page < 1)
			{
				return RedirectToAction(nameof(Search), new { startTown = startTown, endTown = endTown, date = date.ToYearMonthDayFormat(), companyId = companyId, page = 1 });
			}

			if (!this.towns.TownExistsById(startTown) || !this.towns.TownExistsById(endTown))
			{
				this.GenerateAlertMessage(WebConstants.Message.InvalidTown, Alert.Danger);
				return this.RedirectToHome();
			}

			if (date.Date < DateTime.UtcNow.Date)
			{
				this.GenerateAlertMessage(WebConstants.Message.InvalidDate, Alert.Danger);
				return this.RedirectToHome();
			}

			var routesPagination = new PaginationViewModel()
			{
				Action = nameof(Search),
				Controller = WebConstants.Controller.Routes,
				CurrentPage = page,
				PageSize = WebConstants.Pagination.SearchedRoutesPageSize,
				TotalElements = this.routes.GetSearchedRoutesCount(startTown, endTown, date, companyId)
			};

			if (page > routesPagination.TotalPages && routesPagination.TotalPages != 0)
			{
				return RedirectToAction(nameof(Search), new { startTown = startTown, endTown = endTown, date = date.ToYearMonthDayFormat(), companyId = companyId, page = routesPagination.TotalPages });
			}

			return View(new SearchedRoutes()
			{
				Routes = this.routes.GetSearchedRoutes(startTown, endTown, date, companyId, page, WebConstants.Pagination.SearchedRoutesPageSize),
				Towns = this.GenerateSelectListTowns(),
				Companies = this.GenerateSelectListCompanies(),
				StartTown = startTown,
				EndTown = endTown,
				Date = date,
				CompanyId = companyId,
				Pagination = routesPagination
			});
		}

		[Route(WebConstants.Route.BookRouteTicket)]
		public IActionResult BookTicket(int id, TimeSpan departureTime, DateTime date, string companyId)
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
			form.CompanyId = companyId;
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

		private List<SelectListItem> GenerateSelectListCompanies()
		{
			var list = new List<SelectListItem>();
			var companies = this.companies.GetCompaniesSelectListItems();

			list.Add(new SelectListItem()
			{
				Text = " -- All -- ",
				Value = string.Empty,
				Selected = true
			});

			foreach (var c in companies)
			{
				list.Add(new SelectListItem()
				{
					Text = c.Name,
					Value = c.Id.ToString()
				});
			}

			return list;
		}
	}
}
