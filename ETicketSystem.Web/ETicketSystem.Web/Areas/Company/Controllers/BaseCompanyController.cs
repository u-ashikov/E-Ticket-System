namespace ETicketSystem.Web.Areas.Company.Controllers
{
	using Common.Constants;
	using Services.Contracts;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using Web.Controllers;

	[Route(WebConstants.Routing.Company)]
	[Area(WebConstants.Area.Company)]
	[Authorize(Roles = WebConstants.Role.CompanyRole)]
	public abstract class BaseCompanyController : BaseController
    {
		protected BaseCompanyController() { }

		protected BaseCompanyController(ITownService towns)
			: base(towns) { }
    }
}
