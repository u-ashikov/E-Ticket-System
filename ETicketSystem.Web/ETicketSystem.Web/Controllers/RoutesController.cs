namespace ETicketSystem.Web.Controllers
{
	using ETicketSystem.Common.Constants;
	using ETicketSystem.Services.Contracts;
	using ETicketSystem.Web.Models.Routes;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.Rendering;
	using System.Collections.Generic;

	public class RoutesController : BaseController
    {
		private readonly ITownService towns;

		public RoutesController(ITownService towns)
		{
			this.towns = towns;
		}

		public IActionResult Search(SearchRouteFormModel model)
		{
			if (!ModelState.IsValid)
			{
				model.Towns = this.GenerateSelectListTowns();
				return RedirectToAction(nameof(HomeController.Index),WebConstants.Controller.Home);
			}

			return null;
		}

		private List<SelectListItem> GenerateSelectListTowns()
		{
			var list = new List<SelectListItem>();
			var towns = this.towns.GetTownsListItems();

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
