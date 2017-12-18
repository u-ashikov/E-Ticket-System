namespace ETicketSystem.Web.Areas.Admin.Controllers
{
	using Common.Constants;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using Services.Contracts;
	using Web.Controllers;

	[Route(WebConstants.Routing.Admin)]
	[Area(WebConstants.Area.Admin)]
	[Authorize(Roles = AdminConstants.Role)]
	public abstract class BaseAdminController : BaseController
    {
		protected BaseAdminController() { }

		protected BaseAdminController(ITownService towns)
			:base(towns) { }
    }
}
