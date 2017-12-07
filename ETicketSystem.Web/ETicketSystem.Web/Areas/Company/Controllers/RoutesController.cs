namespace ETicketSystem.Web.Areas.Company.Controllers
{
	using Common.Constants;
	using Data.Models;
	using ETicketSystem.Common.Enums;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.Rendering;
	using Models.Routes;
	using Services.Company.Contracts;
	using Services.Contracts;
	using System.Collections.Generic;
	using Web.Controllers;

	[Route(WebConstants.Route.Company)]
	[Area(WebConstants.Area.Company)]
	[Authorize(Roles = WebConstants.Role.CompanyRole)]
	public class RoutesController : BaseController
    {
		private readonly ICompanyRouteService routes;

		private readonly ITownService towns;

		private readonly ICompanyService companies;

		private readonly UserManager<User> userManager;

		public RoutesController(ICompanyRouteService routes, ITownService towns, UserManager<User> userManager, ICompanyService companies)
		{
			this.routes = routes;
			this.towns = towns;
			this.userManager = userManager;
			this.companies = companies;
		}

		[Route(WebConstants.Route.AllCompanyRoutes)]
		public IActionResult All()
		{
			var companyId = this.userManager.GetUserId(User);

			return View(this.routes.All(companyId));
		}

		[Route(WebConstants.Route.AddCompanyRoute)]
		public IActionResult Add()
		{
			if (!this.companies.IsApproved(this.userManager.GetUserId(User)))
			{
				this.GenerateAlertMessage(WebConstants.Message.NotApproved, Alert.Warning);
				return RedirectToAction(nameof(HomeController.Index), WebConstants.Controller.Home);
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
				return RedirectToAction(nameof(HomeController.Index), WebConstants.Controller.Home);
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
		[Route(WebConstants.Route.EditCompanyRoute)]
		public IActionResult Edit(RouteFormModel model, int id)
		{
			var companyId = this.userManager.GetUserId(User);

			if (!ModelState.IsValid)
			{
				model.TownsStations = this.GenerateTownStationsSelectListItems();
				return View(model);
			}

			var alreadyExists = this.routes.RouteAlreadyExist(model.StartStation, model.EndStation, model.DepartureTime.TimeOfDay, companyId);

			if (alreadyExists)
			{
				model.TownsStations = this.GenerateTownStationsSelectListItems();
				ModelState.AddModelError(string.Empty, WebConstants.Message.CompanyRouteDuplication);
				return View(model);
			}

			var success = this.routes.Edit(id,model.StartStation,model.EndStation, model.DepartureTime.TimeOfDay, model.Duration, model.BusType, model.Price, companyId);

			if (!success)
			{
				this.GenerateAlertMessage(WebConstants.Message.InvalidRoute, Alert.Danger);

				return RedirectToAction(nameof(All));
			}

			var startTownName = this.towns.GetTownNameByStationId(model.StartStation);
			var endTownName = this.towns.GetTownNameByStationId(model.EndStation);

			this.GenerateAlertMessage(string.Format(WebConstants.Message.SuccessfullyEditedRoute, startTownName, endTownName), Alert.Success);

			return RedirectToAction(nameof(All));
		}

		[Route(WebConstants.Route.ChangeCompanyRouteStatus)]
		public IActionResult ChangeStatus(int id)
		{
			var success = this.routes.ChangeStatus(id, this.userManager.GetUserId(User));

			if (!success)
			{
				this.GenerateAlertMessage(WebConstants.Message.InvalidRoute,Alert.Danger);
				return RedirectToAction(nameof(All));
			}

			var route = this.routes.GetRouteBaseInfo(id, this.userManager.GetUserId(User));

			this.GenerateAlertMessage(string.Format(WebConstants.Message.RouteStatusChanged, route.StartStationTownName,route.EndStationTownName,route.DepartureTime,route.Status), Alert.Success);

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
