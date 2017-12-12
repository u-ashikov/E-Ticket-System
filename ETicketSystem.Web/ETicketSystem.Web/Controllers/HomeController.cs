﻿namespace ETicketSystem.Web.Controllers
{
	using Microsoft.AspNetCore.Mvc;
	using Models;
	using Models.Routes;
	using Services.Contracts;
	using System.Diagnostics;

	public class HomeController : BaseController
    {
		public HomeController(ITownService towns)
			:base(towns) {}

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
    }
}
