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
	using Services.Models.Route;
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
		[Route(WebConstants.Routing.RoutesSearch)]
		public IActionResult Search(int startTown, int endTown, DateTime date, string companyId, int page = 1)
		{
			if (!this.towns.TownExistsById(startTown) || !this.towns.TownExistsById(endTown))
			{
				this.GenerateAlertMessage(WebConstants.Message.InvalidTown, Alert.Danger);
				return this.RedirectToHome();
			}

			if (date.Date < DateTime.UtcNow.ToLocalTime().Date)
			{
				this.GenerateAlertMessage(WebConstants.Message.InvalidDate, Alert.Danger);
				return this.RedirectToHome();
			}

			if (page < 1)
			{
				return RedirectToAction(nameof(Search), new { startTown = startTown, endTown = endTown, date = date.ToYearMonthDayFormat(), companyId = companyId, page = 1 });
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

			var routes = this.routes.GetSearchedRoutes(startTown, endTown, date, companyId, page, WebConstants.Pagination.SearchedRoutesPageSize)
				.ToList();

			routes
				.ForEach(r => r.ReservedTickets = this.tickets.GetRouteReservedTicketsCount(r.Id, new DateTime(date.Year, date.Month, date.Day, r.DepartureTime.Hours, r.DepartureTime.Minutes, r.DepartureTime.Seconds)));

			return View(new SearchedRoutes()
			{
				Routes = routes,
				Towns = this.GenerateSelectListTowns(),
				Companies = this.GenerateSelectListCompanies(),
				StartTown = startTown,
				EndTown = endTown,
				Date = date,
				CompanyId = companyId,
				Pagination = routesPagination
			});
		}

		[Route(WebConstants.Routing.BookRouteTicket)]
		public IActionResult BookTicket(int id, TimeSpan departureTime, DateTime date, string companyId)
		{
			var departureDateTime = new DateTime(date.Year, date.Month, date.Day, departureTime.Hours, departureTime.Minutes, departureTime.Seconds);

			if (!this.routes.RouteExists(id, departureTime) || departureDateTime < DateTime.UtcNow.ToLocalTime())
			{
				this.GenerateAlertMessage(WebConstants.Message.InvalidRoute, Alert.Danger);
				return this.RedirectToHome();
			}

			var form = new BookTicketFormModel();

			var routeTicketBookingInfo = this.routes.GetRouteTicketBookingInfo(id, departureDateTime);

			this.GenerateBusSchemaSeats(form, routeTicketBookingInfo);

			form.BusSeats = (int)routeTicketBookingInfo.BusType;
			form.DepartureDateTime = departureDateTime;
			form.Duration = routeTicketBookingInfo.Duration;
			form.RouteId = id;
			form.StartTownId = routeTicketBookingInfo.StartTownId;
			form.EndTownId = routeTicketBookingInfo.EndTownId;
			form.CompanyName = routeTicketBookingInfo.CompanyName;
			form.CompanyId = companyId;
			form.StartStation = routeTicketBookingInfo.StartStation;
			form.EndStation = routeTicketBookingInfo.EndStation;

			return View(form);
		}

		[HttpPost]
		[Route(WebConstants.Routing.BookRouteTicket)]
		public IActionResult BookTicket(BookTicketFormModel form)
		{
			if (!ModelState.IsValid)
			{
				ModelState.AddModelError(string.Empty, WebConstants.Message.NoneSelectedSeats);
				this.GenerateBusSchemaSeats(form, this.routes.GetRouteTicketBookingInfo(form.RouteId, form.DepartureDateTime));
				return View(form);
			}

			if (!this.routes.RouteExists(form.RouteId, form.DepartureDateTime.TimeOfDay) || form.DepartureDateTime < DateTime.UtcNow.ToLocalTime())
			{
				this.GenerateAlertMessage(WebConstants.Message.InvalidRoute, Alert.Danger);
				return this.RedirectToHome();
			}

			var alreadyReservedTickets = this.tickets.GetAlreadyReservedTickets(form.RouteId, form.DepartureDateTime);

			if (alreadyReservedTickets.Count() == form.BusSeats)
			{
				this.GenerateAlertMessage(string.Format(WebConstants.Message.RouteSoldOut, form.StartStation, form.EndStation, form.DepartureDateTime.Date.ToYearMonthDayFormat(), form.DepartureDateTime.TimeOfDay), Alert.Warning);

				return RedirectToAction(nameof(Search), new { startTown = form.StartTownId, endTown = form.EndTownId, date = form.DepartureDateTime.Date, companyId = form.CompanyId });
			}

			var reservedTickets = form.Seats
										.Where(s => s.Checked && !s.Disabled)
										.Select(s => s.Value)
										.ToList();

			var matchingSeats = reservedTickets.Intersect(alreadyReservedTickets).ToList();

			if (matchingSeats.Count > 0)
			{
				ModelState.AddModelError(string.Empty, string.Format(WebConstants.Message.SeatsAlreadyTaken,string.Join(", ",matchingSeats)));
				return View(form);
			}

			var success = this.tickets.Add(form.RouteId, form.DepartureDateTime, reservedTickets, this.userManager.GetUserId(User));

			if (!success)
			{
				this.GenerateAlertMessage(WebConstants.Message.InvalidRoute, Alert.Danger);
				return this.RedirectToHome();
			}

			this.GenerateAlertMessage(string.Format(WebConstants.Message.SuccessfullyTicketReservation, string.Join(", ", reservedTickets), form.StartStation, form.EndStation, form.DepartureDateTime), Alert.Success);

			return RedirectToHome();
		}

		private void GenerateBusSchemaSeats(BookTicketFormModel form, RouteBookTicketInfoServiceModel info)
		{
			for (int i = 1; i <= (int)info.BusType; i++)
			{
				form.Seats.Add(new BusSeat()
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
				Text = WebConstants.SelectListDefaultItem.All,
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
