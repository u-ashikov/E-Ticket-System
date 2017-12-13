namespace ETicketSystem.Web.Areas.Admin.Controllers
{
	using Common.Constants;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using Web.Controllers;

	[Route(WebConstants.Route.Admin)]
	[Area(WebConstants.Area.Admin)]
	[Authorize(Roles = AdminConstants.Role)]
	public abstract class BaseAdminController : BaseController
    {
    }
}
