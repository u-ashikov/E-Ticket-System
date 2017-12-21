namespace ETicketSystem.Web.Controllers
{
	using Common.Constants;
	using Common.Enums;
	using Data.Models;
	using Infrastructure.Extensions;
	using Microsoft.AspNetCore.Authentication;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Logging;
	using Models.Account;
	using Services.Contracts;
	using System;
	using System.Threading.Tasks;

	[Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : BaseController
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly ILogger logger;
		private readonly ICompanyService companies;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<AccountController> logger,
			ITownService towns,
			ICompanyService companies)
			:base(towns)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
			this.companies = companies;
        }

        [TempData]
        public string ErrorMessage { get; set; }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginFormModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var result = await this.signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    this.logger.LogInformation("User logged in.");
                    return RedirectToLocal(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    this.logger.LogWarning("User account locked out.");
                    return RedirectToAction(nameof(Lockout));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
        {
            var user = await this.signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var model = new LoginWith2faViewModel { RememberMe = rememberMe };
            ViewData["ReturnUrl"] = returnUrl;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await this.signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{this.userManager.GetUserId(User)}'.");
            }

            var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await this.signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, model.RememberMachine);

            if (result.Succeeded)
            {
                this.logger.LogInformation("User with ID {UserId} logged in with 2fa.", user.Id);
                return RedirectToLocal(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                this.logger.LogWarning("User with ID {UserId} account locked out.", user.Id);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                this.logger.LogWarning("Invalid authenticator code entered for user with ID {UserId}.", user.Id);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

		[HttpGet]
		[AllowAnonymous]
		public IActionResult RegistrationType() => View();

		[HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterUser(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser(RegisterUserFormModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new RegularUser
				{
					UserName = model.Username,
					Email = model.Email,
					FirstName = model.FirstName,
					LastName = model.LastName,
					Gender = model.Gender,
					RegistrationDate = DateTime.UtcNow.ToLocalTime()
				};

                var result = await this.userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    this.logger.LogInformation("User created a new account with password.");

                    var code = await this.userManager.GenerateEmailConfirmationTokenAsync(user);

                    await this.signInManager.SignInAsync(user, isPersistent: false);
                    this.logger.LogInformation("User created a new account with password.");
                    return RedirectToLocal(returnUrl);
                }
                AddErrors(result);
            }

            return View(model);
        }

		[HttpGet]
		[AllowAnonymous]
		public IActionResult RegisterCompany() => 
			View(new RegisterCompanyFormModel()
			{
				Towns = this.GenerateSelectListTowns()
			});

		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> RegisterCompany(RegisterCompanyFormModel model, string returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;

			if (ModelState.IsValid)
			{
				var company = new Company
				{
					UserName = model.Username,
					Email = model.Email,
					Address = model.Address,
					ChiefFirstName = model.ChiefFirstName,
					ChiefLastName = model.ChiefLastName,
					Description = model.Description,
					Name = model.Name,
					PhoneNumber = model.PhoneNumber,
					TownId = model.Town,
					UniqueReferenceNumber = model.UniqueReferenceNumber,
					Logo = model.Logo.GetFormFileBytes(),
					RegistrationDate = DateTime.UtcNow
				};

				var result = await this.userManager.CreateAsync(company, model.Password);

				if (result.Succeeded)
				{
					await this.userManager.AddToRoleAsync(company, Role.Company.ToString());

					this.logger.LogInformation("Company created a new account with password.");

					var code = await this.userManager.GenerateEmailConfirmationTokenAsync(company);

					await this.signInManager.SignInAsync(company, isPersistent: false);
					this.logger.LogInformation("Company created a new account with password.");
					return RedirectToLocal(returnUrl);
				}

				AddErrors(result);
			}

			model.Towns = this.GenerateSelectListTowns();
			return View(model);
		}

		[HttpPost]
        public async Task<IActionResult> Logout()
        {
            await this.signInManager.SignOutAsync();
            this.logger.LogInformation("User logged out.");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }


        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

		[AllowAnonymous]
		public IActionResult VerifyCompanyName(string name)
		{
			if (this.companies.IsCompanyNameRegistered(name))
			{
				return Json(data: WebConstants.Message.CompanyNameAlreadyTaken);
			}

			return Json(true);
		}

		[AllowAnonymous]
		public IActionResult VerifyUrn(string uniqueReferenceNumber)
		{
			if (this.companies.IsUniqueReferenceNumberRegistered(uniqueReferenceNumber))
			{
				return Json(data: WebConstants.Message.CompanyUrnAlreadyTaken);
			}

			return Json(true);
		}

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}
