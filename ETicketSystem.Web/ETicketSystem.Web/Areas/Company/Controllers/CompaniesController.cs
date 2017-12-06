namespace ETicketSystem.Web.Areas.Company.Controllers
{
	using ETicketSystem.Common.Constants;
	using ETicketSystem.Services.Company.Contracts;
	using ETicketSystem.Services.Contracts;
	using ETicketSystem.Web.Areas.Company.Models.Companies;
	using ETicketSystem.Web.Controllers;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.Rendering;
	using System;
	using System.Collections.Generic;

	[Route("Company")]
	[Area(WebConstants.Area.Company)]
	[Authorize(Roles = WebConstants.Role.CompanyRole)]
	public class CompaniesController : BaseController
    {
		private readonly ICompanyRouteService companies;

		private readonly ITownService towns;

		public CompaniesController(ICompanyRouteService companies, ITownService towns)
		{
			this.companies = companies;
			this.towns = towns;
		}

		[Route("Routes/Add")]
		public IActionResult AddRoute()
		{
			var townsWithStationsList = this.GenerateTownStationsSelectListItems();

			return View(new CompanyRouteFormModel()
			{
				TownsStations = townsWithStationsList
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("Routes/Add")]
		public IActionResult AddRoute(CompanyRouteFormModel model)
		{
			if (!ModelState.IsValid)
			{
				model.TownsStations = this.GenerateTownStationsSelectListItems();
				return View(model);
			}

			//if (model.DepartureTime.TimeOfDay >= new TimeSpan(12,00,00) && model.ArrivalTime.TimeOfDay >= new TimeSpan(12, 00, 00) && model.DepartureTime.TimeOfDay <= new TimeSpan(23,59,59) && model.ArrivalTime.TimeOfDay <= new TimeSpan(23, 59, 59))
			//{
			//	if (model.DepartureTime.TimeOfDay >= model.ArrivalTime.TimeOfDay)
			//	{
			//		return View();
			//	}
			//}

			//if (model.DepartureTime.TimeOfDay >= new TimeSpan(00, 00, 00) && model.ArrivalTime.TimeOfDay >= new TimeSpan(00, 00, 00) && model.DepartureTime.TimeOfDay <= new TimeSpan(11, 59, 59) && model.ArrivalTime.TimeOfDay <= new TimeSpan(11, 59, 59))
			//{
			//	if (model.DepartureTime.TimeOfDay >= model.ArrivalTime.TimeOfDay)
			//	{
			//		return View();
			//	}
			//}

			return null;
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
