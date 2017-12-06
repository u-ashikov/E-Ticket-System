namespace ETicketSystem.Web.Areas.Admin.Controllers
{
	using ETicketSystem.Common.Constants;
	using ETicketSystem.Services.Contracts;
	using Microsoft.AspNetCore.Mvc;

	public class AdminCompaniesController : AdminController
    {
		private readonly IAdminCompanyService companies;

		public AdminCompaniesController(IAdminCompanyService companies)
		{
			this.companies = companies;
		}

		[Route(WebConstants.Route.AllCompanies)]
		public IActionResult All() => View(this.companies.All());
    }
}
