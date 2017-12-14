namespace ETicketSystem.Web.Areas.Admin.Controllers
{
	using ETicketSystem.Common.Constants;
	using ETicketSystem.Common.Enums;
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
		public IActionResult Add() => View(new AddStationFormModel()
		{
			Towns = this.GenerateSelectListTowns()
		});

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route(WebConstants.Route.AddStation)]
		public IActionResult Add(AddStationFormModel model)
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

			this.GenerateAlertMessage(string.Format(WebConstants.Message.StationCreated,model.Name, townName), Alert.Success);

			return RedirectToHome();
		}
    }
}
