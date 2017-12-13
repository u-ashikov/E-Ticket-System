namespace ETicketSystem.Web.Areas.Admin.Controllers
{
	using Common.Constants;
	using Common.Enums;
	using ETicketSystem.Web.Models.Pagination;
	using Microsoft.AspNetCore.Mvc;
	using Models.AdminCompanies;
	using Services.Admin.Contracts;

	public class AdminCompaniesController : BaseAdminController
    {
		private readonly IAdminCompanyService companies;

		public AdminCompaniesController(IAdminCompanyService companies)
		{
			this.companies = companies;
		}

		[Route(WebConstants.Route.AllCompanies)]
		public IActionResult All(CompanyStatus filter,int page = 1)
		{
			if (page < 1)
			{
				return RedirectToAction(nameof(All), new { filter = filter});
			}

			var companiesPagination = new PaginationViewModel()
			{
				Action = nameof(All),
				Controller = WebConstants.Controller.AdminCompanies,
				CurrentPage = page,
				PageSize = WebConstants.Pagination.AdminCompaniesListing,
				TotalElements = this.companies.TotalCompanies(filter.ToString()),
				SearchTerm = filter.ToString()
			};

			if (page > companiesPagination.TotalPages && companiesPagination.TotalPages != 0)
			{
				return RedirectToAction(nameof(All), new { page = companiesPagination.TotalPages, filter = filter });
			}

			return View(new AllCompanies()
			{
				Companies = this.companies.All(page, filter.ToString(), WebConstants.Pagination.AdminCompaniesListing),
				Pagination = companiesPagination,
				Filter = filter
			});
		}

		[Route(WebConstants.Route.ApproveCompany)]
		public IActionResult Approve(string companyId)
		{
			if (!this.companies.CompanyExists(companyId))
			{
				this.GenerateAlertMessage(string.Format(WebConstants.Message.NonExistingCompany, companyId), Alert.Danger);
				return RedirectToAction(nameof(All));
			}

			var isApproved = this.companies.Approve(companyId);
			var companyName = this.companies.GetCompanyName(companyId);

			if (!isApproved)
			{
				this.GenerateAlertMessage(string.Format(WebConstants.Message.CompanyAlreadyApproved, companyName), Alert.Warning);
			}
			else
			{
				this.GenerateAlertMessage(string.Format(WebConstants.Message.CompanyApproved, companyName), Alert.Success);
			}

			return RedirectToAction(nameof(All));
		}
    }
}
