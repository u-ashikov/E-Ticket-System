namespace ETicketSystem.Web.Areas.Company.Controllers
{
	using ETicketSystem.Common.Constants;
	using ETicketSystem.Web.Controllers;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;

	[Area(WebConstants.Area.Company)]
	[Authorize(Roles = WebConstants.Role.CompanyRole)]
	public class CompaniesController : BaseController
    {

    }
}
