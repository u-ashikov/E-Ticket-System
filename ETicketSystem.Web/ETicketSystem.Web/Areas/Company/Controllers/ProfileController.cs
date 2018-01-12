namespace ETicketSystem.Web.Areas.Company.Controllers
{
    using Common.Constants;
    using Common.Enums;
    using Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Models.Profile;
    using Services.Contracts;
    using System.Linq;
    using System.Threading.Tasks;
    using Web.Infrastructure.Extensions;

    public class ProfileController : BaseCompanyController
    {
		private readonly IUserService users;

		private readonly UserManager<User> userManager;

		public ProfileController(IUserService users, UserManager<User> userManager)
		{
			this.users = users;
			this.userManager = userManager;
		}

		[Route(WebConstants.Routing.CompanyProfile)]
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

		[Route(WebConstants.Routing.EditCompanyProfile)]
		public IActionResult Edit(string id)
		{
			var company = this.users.GetCompanyUserProfileToEdit(id);

			if (company == null)
			{
				this.GenerateAlertMessage(string.Format(WebConstants.Message.NonExistingEntity,WebConstants.Entity.Company, id), Alert.Danger);

				return RedirectToHome();
			}

			if (this.userManager.GetUserId(User) != id)
			{
				this.GenerateAlertMessage(string.Format(WebConstants.Message.NotProfileOwner, id), Alert.Danger);

				return RedirectToHome();
			}

			return View(new EditCompanyProfileFormModel()
			{
				Username = company.Username,
				Email = company.Email,
				Description = company.Description,
				CurrentLogo =company.Logo,
				PhoneNumber = company.PhoneNumber
			});
		}

		[HttpPost]
		[Route(WebConstants.Routing.EditCompanyProfile)]
		public async Task<IActionResult> Edit(string id, EditCompanyProfileFormModel profile)
		{
			if (!ModelState.IsValid)
			{
				profile.CurrentLogo = this.users.GetCompanyLogo(id);

				return View(profile);
			}

			if (!this.users.UserExists(id) || this.userManager.GetUserId(User) != id)
			{
				return BadRequest();
			}

			var errors = await this.users.EditCompanyAsync(id,profile.Username,profile.Email,profile.NewPassword,profile.Password,profile.Description,profile.Logo.GetFormFileBytes(), profile.PhoneNumber);

			if (errors.Count() != 0)
			{
				foreach (var error in errors)
				{
					ModelState.AddModelError("", error.Description);
				}

				profile.CurrentLogo = this.users.GetCompanyLogo(id);

				return View(profile);
			}

			this.GenerateAlertMessage(WebConstants.Message.ProfileEdited, Alert.Success);

			return RedirectToAction(nameof(Index), new { id });
		}
	}
}
