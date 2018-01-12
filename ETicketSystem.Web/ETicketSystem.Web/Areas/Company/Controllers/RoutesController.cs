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
	using Web.Infrastructure.Extensions;
	using Web.Models.Pagination;

	public class RoutesController : BaseCompanyController
    {
		private readonly ICompanyRouteService routes;

		private readonly ICompanyService companies;

		private readonly IStationService stations;

		private readonly UserManager<User> userManager;

		public RoutesController(ICompanyRouteService routes, ITownService towns, UserManager<User> userManager, ICompanyService companies, IStationService stations)
			:base(towns)
		{
			this.routes = routes;
			this.userManager = userManager;
			this.companies = companies;
			this.stations = stations;
		}

		[Route(WebConstants.Routing.AllCompanyRoutes)]
		public IActionResult All(int startTown, int endTown, DateTime? date, int page = 1)
		{
			if (page < 1)
			{
				return RedirectToAction(nameof(All), new { startTown, endTown, date = date.HasValue ? date.Value.ToYearMonthDayFormat() : null, page = 1 });
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
				return RedirectToAction(nameof(All), new { startTown, endTown, date = date.HasValue ? date.Value.ToYearMonthDayFormat() : null, page = routesPagination.TotalPages });
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

		[Route(WebConstants.Routing.AddCompanyRoute)]
		public IActionResult Add()
		{
			var companyId = this.userManager.GetUserId(User);

			if (!this.companies.IsApproved(companyId))
			{
				this.GenerateAlertMessage(WebConstants.Message.CompanyNotApproved, Alert.Warning);

				return this.RedirectToAction(nameof(All));
			}

			if (this.companies.IsBlocked(companyId))
			{
				this.GenerateAlertMessage(WebConstants.Message.AddRouteCompanyBlocked, Alert.Warning);

				return this.RedirectToAction(nameof(All));
			}

			var townsWithStationsList = this.GenerateTownStationsSelectListItems();

			return View(new RouteFormModel()
			{
				TownsStations = townsWithStationsList
			});
		}

		[HttpPost]
		[Route(WebConstants.Routing.AddCompanyRoute)]
		public IActionResult Add(RouteFormModel model)
		{
			var companyId = this.userManager.GetUserId(User);

			if (!this.companies.IsApproved(companyId))
			{
				this.GenerateAlertMessage(WebConstants.Message.CompanyNotApproved, Alert.Warning);

				return this.RedirectToAction(nameof(All));
			}

			if (this.companies.IsBlocked(companyId))
			{
				this.GenerateAlertMessage(WebConstants.Message.AddRouteCompanyBlocked, Alert.Warning);

				return this.RedirectToAction(nameof(All));
			}

			if (!this.stations.StationExist(model.StartStation) || !this.stations.StationExist(model.EndStation))
			{
				this.GenerateAlertMessage(WebConstants.Message.InvalidStation, Alert.Warning);

				return this.RedirectToAction(nameof(All));
			}

			if (model.StartStation == model.EndStation || this.stations.AreStationsInSameTown(model.StartStation, model.EndStation))
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

			var isAdded = this.routes.Add(model.StartStation, model.EndStation, model.DepartureTime.TimeOfDay, model.Duration, model.BusType, model.Price, companyId);

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

		[Route(WebConstants.Routing.EditCompanyRoute)]
		public IActionResult Edit(int id)
		{
			var companyId = this.userManager.GetUserId(User);

			if (!this.routes.IsRouteOwner(id,companyId))
			{
				this.GenerateAlertMessage(WebConstants.Message.InvalidRoute, Alert.Danger);

				return RedirectToAction(nameof(All));
			}

			if (!this.companies.IsApproved(companyId))
			{
				this.GenerateAlertMessage(WebConstants.Message.ChangeRouteCompanyNotApproved, Alert.Warning);

				return this.RedirectToAction(nameof(All));
			}

			if (this.companies.IsBlocked(companyId))
			{
				this.GenerateAlertMessage(WebConstants.Message.ChangeRouteCompanyBlocked, Alert.Warning);

				return this.RedirectToAction(nameof(All));
			}

			var routeToEdit = this.routes.GetRouteToEdit(companyId, id);

			if (routeToEdit == null)
			{
				this.GenerateAlertMessage(WebConstants.Message.InvalidRoute, Alert.Danger);

				return RedirectToAction(nameof(All));
			}

			var startTownName = this.towns.GetTownNameByStationId(routeToEdit.StartStationId);
			var endTownName = this.towns.GetTownNameByStationId(routeToEdit.EndStationId);

			if (this.routes.HasReservedTickets(id, companyId))
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
		[Route(WebConstants.Routing.EditCompanyRoute)]
		public IActionResult Edit(RouteFormModel model, int id)
		{
			var companyId = this.userManager.GetUserId(User);

			if (!this.routes.IsRouteOwner(id, companyId))
			{
				this.GenerateAlertMessage(WebConstants.Message.InvalidRoute, Alert.Danger);

				return RedirectToAction(nameof(All));
			}

			if (!this.companies.IsApproved(companyId))
			{
				this.GenerateAlertMessage(WebConstants.Message.ChangeRouteCompanyNotApproved, Alert.Warning);
				return this.RedirectToAction(nameof(All));
			}

			if (this.companies.IsBlocked(companyId))
			{
				this.GenerateAlertMessage(WebConstants.Message.ChangeRouteCompanyBlocked, Alert.Warning);
				return this.RedirectToAction(nameof(All));
			}

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

			if (this.routes.HasReservedTickets(id, companyId))
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

			this.GenerateAlertMessage(string.Format(WebConstants.Message.RouteEdited, startTownName, endTownName), Alert.Success);

			return RedirectToAction(nameof(All));
		}

		[Route(WebConstants.Routing.ChangeCompanyRouteStatus)]
		public IActionResult ChangeStatus(int id)
		{
			var companyId = this.userManager.GetUserId(User);

			if (!this.routes.IsRouteOwner(id, companyId))
			{
				this.GenerateAlertMessage(WebConstants.Message.InvalidRoute, Alert.Danger);

				return RedirectToAction(nameof(All));
			}

			if (!this.companies.IsApproved(companyId))
			{
				this.GenerateAlertMessage(WebConstants.Message.ChangeRouteCompanyNotApproved, Alert.Warning);
				return this.RedirectToAction(nameof(All));
			}

			if (this.companies.IsBlocked(companyId))
			{
				this.GenerateAlertMessage(WebConstants.Message.ChangeRouteCompanyBlocked, Alert.Warning);
				return this.RedirectToAction(nameof(All));
			}

			var routeInfo = this.routes.GetRouteBaseInfo(id, companyId);

			if (this.routes.HasReservedTickets(id, companyId))
			{
				this.GenerateAlertMessage(string.Format(WebConstants.Message.DeactivateRouteWithTickets, routeInfo.StartStationTownName, routeInfo.EndStationTownName), Alert.Danger);

				return RedirectToAction(nameof(All));
			}

			var success = this.routes.ChangeStatus(id, companyId);

			routeInfo = this.routes.GetRouteBaseInfo(id, companyId);

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
