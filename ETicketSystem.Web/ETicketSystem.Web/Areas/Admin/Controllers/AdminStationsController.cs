namespace ETicketSystem.Web.Areas.Admin.Controllers
{
	using Common.Constants;
	using Common.Enums;
	using Microsoft.AspNetCore.Mvc;
	using Models.AdminStations;
	using Services.Admin.Contracts;
	using Services.Contracts;

	public class AdminStationsController : BaseAdminController
    {
		private readonly IAdminStationService stations;

		public AdminStationsController(ITownService towns, IAdminStationService stations)
			:base(towns)
		{
			this.stations = stations;
		}

		[Route(WebConstants.Route.AddStation)]
		public IActionResult Add() => View(new StationFormModel()
		{
			Towns = this.GenerateSelectListTowns()
		});

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route(WebConstants.Route.AddStation)]
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

			return RedirectToHome();
		}

		[Route(WebConstants.Route.EditStation)]
		public IActionResult Edit(int id)
		{
			var station = this.stations.GetStationToEdit(id);

			if (station == null)
			{
				this.GenerateAlertMessage(string.Format(WebConstants.Message.NonExistingStation, id), Alert.Warning);
				return Redirect(WebConstants.Route.AdminAllTownsUrl);
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
		[Route(WebConstants.Route.EditStation)]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(StationFormModel model)
		{
			if (!this.stations.StationExists(model.Id))
			{
				this.GenerateAlertMessage(string.Format(WebConstants.Message.NonExistingStation, model.Id), Alert.Warning);
				return Redirect(WebConstants.Route.AdminAllTownsUrl);
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

			this.GenerateAlertMessage(WebConstants.Message.StationEdited, Alert.Success);

			return Redirect(WebConstants.Route.AdminAllTownsUrl);
		}
	}
}
