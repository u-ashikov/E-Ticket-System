namespace ETicketSystem.Web.Areas.Company.Controllers
{
	using Common.Constants;
	using Common.Enums;
	using Data.Models;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using Services.Company.Contracts;

	public class ProfileController : BaseCompanyController
    {
		private readonly ICompanyProfileService profile;

		private readonly UserManager<User> userManager;

		public ProfileController(ICompanyProfileService profile, UserManager<User> userManager)
		{
			this.profile = profile;
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

			return View(this.profile.ProfileDetails(companyId));
		}
	}
}
