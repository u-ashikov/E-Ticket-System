namespace ETicketSystem.Web.Areas.Admin.Controllers
{
	using ETicketSystem.Common.Constants;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;

	[Route("Admin")]
	[Area(WebConstants.Area.Admin)]
	[Authorize(Roles = WebConstants.Admin.Role)]
	public abstract class AdminController : Controller
    {
    }
}
