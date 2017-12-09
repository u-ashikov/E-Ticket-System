namespace ETicketSystem.Web.Controllers
{
	using ETicketSystem.Common.Constants;
	using ETicketSystem.Common.Enums;
	using Microsoft.AspNetCore.Mvc;

	public abstract class BaseController : Controller
    {
		protected void GenerateAlertMessage(string message, Alert alertType)
		{
			TempData[WebConstants.TempDataKey.AlertType] = alertType.ToString().ToLower();
			TempData[WebConstants.TempDataKey.Message] = message;
		}

		protected IActionResult RedirectToHome()
		{
			return RedirectToAction(nameof(HomeController.Index), WebConstants.Controller.Home);
		}
	}
}
