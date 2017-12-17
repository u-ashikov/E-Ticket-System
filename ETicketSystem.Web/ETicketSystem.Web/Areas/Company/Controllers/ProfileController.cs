namespace ETicketSystem.Web.Areas.Company.Controllers
{
	using Common.Constants;
	using Common.Enums;
	using Data.Models;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using Services.Contracts;

	public class ProfileController : BaseCompanyController
    {
		private readonly IUserService users;

		private readonly UserManager<User> userManager;

		public ProfileController(IUserService users, UserManager<User> userManager)
		{
			this.users = users;
			this.userManager = userManager;
		}

		[Route(WebConstants.Route.CompanyProfile)]
		public IActionResult Index(string id)
		{
			var companyId = this.userManager.GetUserId(User);

			if (companyId != id)
			{
				this.GenerateAlertMessage(WebConstants.Message.NotProfileOwner, Alert.Danger);
				return RedirectToHome();
			}

			return View(this.users.GetCompanyProfileDetails(companyId));
		}
	}
}
