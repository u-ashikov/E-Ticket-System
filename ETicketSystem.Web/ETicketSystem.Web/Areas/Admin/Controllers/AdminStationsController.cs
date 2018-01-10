namespace ETicketSystem.Web.Areas.Admin.Controllers
{
	using Common.Constants;
	using Common.Enums;
	using Microsoft.AspNetCore.Mvc;
	using Models.AdminStations;
	using Services.Admin.Contracts;
	using Services.Contracts;
	using Web.Models.Pagination;

	public class AdminStationsController : BaseAdminController
    {
		private readonly IAdminStationService stations;

		public AdminStationsController(ITownService towns, IAdminStationService stations)
			:base(towns)
		{
			this.stations = stations;
		}

		[Route(WebConstants.Routing.AdminAllStations)]
		public IActionResult All(string searchTerm,int page = 1)
		{
			if (page < 1)
			{
				return RedirectToAction(nameof(All), new { page = 1, searchTerm = searchTerm });
			}

			var stations = this.stations.All(searchTerm, page, WebConstants.Pagination.AdminStationsListing);
			var stationsPagination = new PaginationViewModel()
			{
				Action = nameof(All),
				Controller = WebConstants.Controller.AdminStations,
				CurrentPage = page,
				PageSize = WebConstants.Pagination.AdminStationsListing,
				SearchTerm = searchTerm,
				TotalElements = this.stations.TotalStations(searchTerm)
			};

			if (page > stationsPagination.TotalPages && stationsPagination.TotalPages != 0)
			{
				return RedirectToAction(nameof(All), new { page = stationsPagination.TotalPages, searchTerm = searchTerm });
			}

			return View(new AllStations()
			{
				Stations = stations,
				Pagination = stationsPagination
			});
		}

		[Route(WebConstants.Routing.AdminAddStation)]
		public IActionResult Add() => View(new StationFormModel()
		{
			Towns = this.GenerateSelectListTowns()
		});

		[HttpPost]
		[Route(WebConstants.Routing.AdminAddStation)]
		public IActionResult Add(StationFormModel model)
		{
			if (!ModelState.IsValid)
			{
				model.Towns = this.GenerateSelectListTowns();
				return View(model);
			}

			bool success = this.stations.Add(model.Name, model.TownId, model.Phone);
			var townName = this.towns.GetTownNameById(model.TownId);

			if (!success)
			{
				this.GenerateAlertMessage(string.Format(WebConstants.Message.StationAlreadyExists, model.Name, townName), Alert.Warning);
				model.Towns = this.GenerateSelectListTowns();

				return View(model);
			}

			this.GenerateAlertMessage(string.Format(WebConstants.Message.StationCreated, model.Name, townName), Alert.Success);

			return RedirectToAction(nameof(All));
		}

		[Route(WebConstants.Routing.AdminEditStation)]
		public IActionResult Edit(int id)
		{
			var station = this.stations.GetStationToEdit(id);

			if (station == null)
			{
				this.GenerateAlertMessage(string.Format(WebConstants.Message.NonExistingEntity, nameof(WebConstants.Entity.Station), id), Alert.Warning);
				return RedirectToAction(nameof(All));
			}

			return View(new StationFormModel()
			{
				Id = station.Id,
				IsEdit = true,
				Name = station.Name,
				Phone = station.Phone,
				Towns = this.GenerateSelectListTowns(),
				TownId = station.TownId
			});
		}

		[HttpPost]
		[Route(WebConstants.Routing.AdminEditStation)]
		public IActionResult Edit(StationFormModel model)
		{
			if (!this.stations.StationExists(model.Id))
			{
				this.GenerateAlertMessage(string.Format(WebConstants.Message.NonExistingEntity,nameof(WebConstants.Entity.Station), model.Id), Alert.Warning);

				return Redirect(WebConstants.Routing.AdminAllTownsUrl);
			}

			if (this.stations.EditedStationIsSame(model.Id,model.Name,model.Phone,model.TownId))
			{
				this.GenerateAlertMessage(WebConstants.Message.NoChangesFound, Alert.Warning);
				model.IsEdit = true;
				model.Towns = this.GenerateSelectListTowns();

				return View(model);
			}

			if (!ModelState.IsValid)
			{
				model.Towns = this.GenerateSelectListTowns();
				model.IsEdit = true;
				return View(model);
			}

			bool success = this.stations.Edit(model.Id, model.Name, model.Phone, model.TownId);
			string townName = this.towns.GetTownNameById(model.TownId);

			if (!success)
			{
				this.GenerateAlertMessage(string.Format(WebConstants.Message.StationAlreadyExists, model.Name, townName), Alert.Warning);
				model.IsEdit = true;
				model.Towns = this.GenerateSelectListTowns();
				return View(model);
			}

			this.GenerateAlertMessage(string.Format(WebConstants.Message.EntityEdited,nameof(WebConstants.Entity.Station)), Alert.Success);

			return RedirectToAction(nameof(All));
		}
	}
}
