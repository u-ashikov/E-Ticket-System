namespace ETicketSystem.Web.Areas.Admin.Controllers
{
	using ETicketSystem.Common.Constants;
	using ETicketSystem.Common.Enums;
	using ETicketSystem.Services.Contracts;
	using Microsoft.AspNetCore.Mvc;

	public class AdminCompaniesController : BaseAdminController
    {
		private readonly IAdminCompanyService companies;

		public AdminCompaniesController(IAdminCompanyService companies)
		{
			this.companies = companies;
		}

		[Route(WebConstants.Route.AllCompanies)]
		public IActionResult All() => View(this.companies.All());

		[Route(WebConstants.Route.ApproveCompany)]
		public IActionResult Approve(string companyId)
		{
			if (!this.companies.CompanyExists(companyId))
			{
				this.GenerateAlertMessage(string.Format(WebConstants.Error.NonExistingCompany, companyId), Alert.Danger);
				return RedirectToAction(nameof(All));
			}

			var isApproved = this.companies.Approve(companyId);
			var companyName = this.companies.GetCompanyName(companyId);

			if (!isApproved)
			{
				this.GenerateAlertMessage(string.Format(WebConstants.Error.CompanyAlreadyApproved, companyName), Alert.Warning);
			}
			else
			{
				this.GenerateAlertMessage(string.Format(WebConstants.Error.CompanyApproved, companyName), Alert.Success);
			}

			return RedirectToAction(nameof(All));
		}
    }
}
