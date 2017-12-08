namespace ETicketSystem.Web.Controllers
{
	using ETicketSystem.Services.Contracts;
	using ETicketSystem.Web.Models;
	using ETicketSystem.Web.Models.Routes;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.Rendering;
	using System.Collections.Generic;
	using System.Diagnostics;

	public class HomeController : Controller
    {
		private readonly ITownService towns;

		public HomeController(ITownService towns)
		{
			this.towns = towns;
		}

		public IActionResult Index() =>
			View(new SearchRouteFormModel()
			{
				Towns = this.GenerateSelectListTowns()
			});

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

		private List<SelectListItem> GenerateSelectListTowns()
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
