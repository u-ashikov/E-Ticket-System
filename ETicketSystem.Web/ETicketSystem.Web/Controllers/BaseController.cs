namespace ETicketSystem.Web.Controllers
{
	using Common.Constants;
	using Common.Enums;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.Rendering;
	using Services.Contracts;
	using System.Collections.Generic;

	public abstract class BaseController : Controller
    {
		private readonly ITownService towns;

		public BaseController() { }

		public BaseController(ITownService towns)
			:this()
		{
			this.towns = towns;
		}

		protected void GenerateAlertMessage(string message, Alert alertType)
		{
			TempData[WebConstants.TempDataKey.AlertType] = alertType.ToString().ToLower();
			TempData[WebConstants.TempDataKey.Message] = message;
		}

		protected IActionResult RedirectToHome()
		{
			return RedirectToAction(nameof(HomeController.Index), WebConstants.Controller.Home);
		}

		protected List<SelectListItem> GenerateSelectListTowns()
		{
			var list = new List<SelectListItem>();
			var towns = this.towns.GetTownsListItems();

			list.Add(new SelectListItem()
			{
				Disabled = true,
				Text = " -- Select town -- ",
				Value = "0",
				Selected = true
			});

			foreach (var t in towns)
			{
				list.Add(new SelectListItem()
				{
					Text = t.Name,
					Value = t.Id.ToString()
				});
			}

			return list;
		}
	}

}
