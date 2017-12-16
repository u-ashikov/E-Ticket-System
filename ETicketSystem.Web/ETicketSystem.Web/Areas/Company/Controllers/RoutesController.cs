namespace ETicketSystem.Web.Areas.Company.Controllers
{
	using Common.Constants;
	using Common.Enums;
	using Data.Models;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.Rendering;
	using Models.Routes;
	using Services.Company.Contracts;
	using Services.Contracts;
	using System;
	using System.Collections.Generic;
	using Web.Models.Pagination;

	public class RoutesController : BaseCompanyController
    {
		private readonly ICompanyRouteService routes;

		private readonly ICompanyService companies;

		private readonly UserManager<User> userManager;

		public RoutesController(ICompanyRouteService routes, ITownService towns, UserManager<User> userManager, ICompanyService companies)
			:base(towns)
		{
			this.routes = routes;
			this.userManager = userManager;
			this.companies = companies;
		}

		[Route(WebConstants.Route.AllCompanyRoutes)]
		public IActionResult All(int startTown, int endTown, DateTime date, int page = 1)
		{
			if (page < 1)
			{
				return RedirectToAction(nameof(All), new { startTown = startTown, endTown = endTown, date = date, page = 1 });
			}

			var companyId = this.userManager.GetUserId(User);
			var routes = this.routes.All(startTown,endTown, date, companyId, page, WebConstants.Pagination.CompanyRoutesListing);

			var routesPagination = new PaginationViewModel()
			{
				Action = nameof(All),
				Controller = WebConstants.Controller.Routes,
				CurrentPage = page,
				PageSize = WebConstants.Pagination.CompanyRoutesListing,
				TotalElements = this.routes.TotalRoutes(startTown,endTown,date,companyId)
			};

			if (page > routesPagination.TotalPages && routesPagination.TotalPages != 0)
			{
				return RedirectToAction(nameof(All), new { startTown = startTown, endTown = endTown, date = date, page = routesPagination.TotalPages });
			}

			return View(new AllRoutes()
			{
				Date = date,
				EndTown = endTown,
				StartTown = startTown,
				Pagination = routesPagination,
				Routes = routes.Routes,
				Towns = this.GenerateSelectListTowns()
			});
		}

		[Route(WebConstants.Route.AddCompanyRoute)]
		public IActionResult Add()
		{
			if (!this.companies.IsApproved(this.userManager.GetUserId(User)))
			{
				this.GenerateAlertMessage(WebConstants.Message.NotApproved, Alert.Warning);
				return this.RedirectToHome();
			}

			if (this.companies.IsBlocked(this.userManager.GetUserId(User)))
			{
				this.GenerateAlertMessage(WebConstants.Message.Blocked, Alert.Warning);
				return this.RedirectToHome();
			}

			var townsWithStationsList = this.GenerateTownStationsSelectListItems();

			return View(new RouteFormModel()
			{
				TownsStations = townsWithStationsList
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route(WebConstants.Route.AddCompanyRoute)]
		public IActionResult Add(RouteFormModel model)
		{
			if (!this.companies.IsApproved(this.userManager.GetUserId(User)))
			{
				this.GenerateAlertMessage(WebConstants.Message.NotApproved, Alert.Warning);
				return this.RedirectToHome();
			}

			if (model.StartStation == model.EndStation)
			{
				ModelState.AddModelError(string.Empty,WebConstants.Message.StartStationEqualToEndStation);
				model.TownsStations = this.GenerateTownStationsSelectListItems();
				return View(model);
			}

			if (!ModelState.IsValid)
			{
				model.TownsStations = this.GenerateTownStationsSelectListItems();
				return View(model);
			}

			var isAdded = this.routes.Add(model.StartStation, model.EndStation, model.DepartureTime.TimeOfDay, model.Duration, model.BusType, model.Price, this.userManager.GetUserId(User));

			if (!isAdded)
			{
				model.TownsStations = this.GenerateTownStationsSelectListItems();
				ModelState.AddModelError(string.Empty, WebConstants.Message.CompanyRouteDuplication);
				return View(model);
			}

			var startTownName = this.towns.GetTownNameByStationId(model.StartStation);
			var endTownName = this.towns.GetTownNameByStationId(model.EndStation);

			this.GenerateAlertMessage(string.Format(WebConstants.Message.RouteAdded, startTownName, endTownName), Alert.Success);

			return RedirectToAction(nameof(All));
		}

		[Route(WebConstants.Route.EditCompanyRoute)]
		public IActionResult Edit(int id)
		{
			var routeToEdit = this.routes.GetRouteToEdit(this.userManager.GetUserId(User), id);

			if (routeToEdit == null)
			{
				this.GenerateAlertMessage(WebConstants.Message.InvalidRoute, Alert.Danger);

				return RedirectToAction(nameof(All));
			}

			var startTownName = this.towns.GetTownNameByStationId(routeToEdit.StartStationId);
			var endTownName = this.towns.GetTownNameByStationId(routeToEdit.EndStationId);

			if (this.routes.HasReservedTickets(id))
			{
				this.GenerateAlertMessage(string.Format(WebConstants.Message.EditRouteWithTickets, startTownName, endTownName), Alert.Danger);

				return RedirectToAction(nameof(All));
			}

			return View(new RouteFormModel()
			{
				StartStation = routeToEdit.StartStationId,
				EndStation = routeToEdit.EndStationId,
				DepartureTime = routeToEdit.DepartureTime,
				Duration = routeToEdit.Duration,
				BusType = routeToEdit.BusType,
				Price = routeToEdit.Price,
				IsEdit = true,
				TownsStations = this.GenerateTownStationsSelectListItems()
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route(WebConstants.Route.EditCompanyRoute)]
		public IActionResult Edit(RouteFormModel model, int id)
		{
			var companyId = this.userManager.GetUserId(User);

			if (!ModelState.IsValid)
			{
				model.TownsStations = this.GenerateTownStationsSelectListItems();
				model.IsEdit = true;
				return View(model);
			}

			var alreadyExists = this.routes.RouteAlreadyExist(id,model.StartStation, model.EndStation, model.DepartureTime.TimeOfDay, companyId);

			if (alreadyExists)
			{
				model.TownsStations = this.GenerateTownStationsSelectListItems();
				model.IsEdit = true;
				ModelState.AddModelError(string.Empty, WebConstants.Message.CompanyRouteDuplication);
				return View(model);
			}

			var startTownName = this.towns.GetTownNameByStationId(model.StartStation);
			var endTownName = this.towns.GetTownNameByStationId(model.EndStation);

			if (this.routes.HasReservedTickets(id))
			{
				this.GenerateAlertMessage(string.Format(WebConstants.Message.EditRouteWithTickets, startTownName, endTownName), Alert.Danger);

				return RedirectToAction(nameof(All));
			}

			var success = this.routes.Edit(id,model.StartStation,model.EndStation, model.DepartureTime.TimeOfDay, model.Duration, model.BusType, model.Price, companyId);

			if (!success)
			{
				this.GenerateAlertMessage(WebConstants.Message.InvalidRoute, Alert.Danger);

				return RedirectToAction(nameof(All));
			}

			this.GenerateAlertMessage(string.Format(WebConstants.Message.SuccessfullyEditedRoute, startTownName, endTownName), Alert.Success);

			return RedirectToAction(nameof(All));
		}

		[Route(WebConstants.Route.ChangeCompanyRouteStatus)]
		public IActionResult ChangeStatus(int id)
		{
			var routeInfo = this.routes.GetRouteBaseInfo(id, this.userManager.GetUserId(User));

			if (this.routes.HasReservedTickets(id))
			{
				this.GenerateAlertMessage(string.Format(WebConstants.Message.DeactivateRouteWithTickets, routeInfo.StartStationTownName, routeInfo.EndStationTownName), Alert.Danger);

				return RedirectToAction(nameof(All));
			}

			var success = this.routes.ChangeStatus(id, this.userManager.GetUserId(User));

			routeInfo = this.routes.GetRouteBaseInfo(id, this.userManager.GetUserId(User));

			if (!success)
			{
				this.GenerateAlertMessage(WebConstants.Message.InvalidRoute,Alert.Danger);
				return RedirectToAction(nameof(All));
			}

			this.GenerateAlertMessage(string.Format(WebConstants.Message.RouteStatusChanged, routeInfo.StartStationTownName,routeInfo.EndStationTownName,routeInfo.DepartureTime,routeInfo.Status), Alert.Success);

			return RedirectToAction(nameof(All));
		}

		private List<SelectListItem> GenerateTownStationsSelectListItems()
		{
			var list = new List<SelectListItem>();
			var towns = this.towns.GetTownsWithStations();

			foreach (var town in towns)
			{
				var optGroup = new SelectListGroup()
				{
					Name = town.Name
				};

				foreach (var station in town.Stations)
				{
					list.Add(new SelectListItem()
					{
						Text = station.Name,
						Value = station.Id.ToString(),
						Group = optGroup
					});
				}
			}

			return list;
		}
    }
}
