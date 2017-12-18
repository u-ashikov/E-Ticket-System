namespace ETicketSystem.Web.Areas.Admin.Controllers
{
	using Common.Constants;
	using Common.Enums;
	using Microsoft.AspNetCore.Mvc;
	using Models.AdminCompanies;
	using Services.Admin.Contracts;
	using System;
	using Web.Models.Pagination;

	public class AdminCompaniesController : BaseAdminController
    {
		private readonly IAdminCompanyService companies;

		public AdminCompaniesController(IAdminCompanyService companies)
		{
			this.companies = companies;
		}

		[Route(WebConstants.Routing.AdminAllCompanies)]
		public IActionResult All(CompanyStatus filter,int page = 1)
		{
			if (page < 1)
			{
				return RedirectToAction(nameof(All), new { filter = filter});
			}

			if (!Enum.IsDefined(typeof(CompanyStatus),filter))
			{
				filter = CompanyStatus.All;
				return RedirectToAction(nameof(All), new { filter = filter });
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

		[Route(WebConstants.Routing.AdminApproveCompany)]
		public IActionResult Approve(string companyId, CompanyStatus filter, int page)
		{
			if (!this.companies.CompanyExists(companyId))
			{
				this.GenerateAlertMessage(string.Format(WebConstants.Message.NonExistingEntity,nameof(WebConstants.Entity.Company), companyId), Alert.Danger);
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

			return RedirectToAction(nameof(All), new { page = page, filter = filter});
		}

		[Route(WebConstants.Routing.AdminBlockCompany)]
		public IActionResult Block(string companyId, CompanyStatus filter, int page)
		{
			if (!this.companies.CompanyExists(companyId))
			{
				this.GenerateAlertMessage(string.Format(WebConstants.Message.NonExistingEntity, nameof(WebConstants.Entity.Company), companyId), Alert.Danger);
				return RedirectToAction(nameof(All));
			}

			bool statusChanged = this.companies.ChangeStatus(companyId);

			if (!statusChanged)
			{
				this.GenerateAlertMessage(string.Format(WebConstants.Message.BlockCompanyUnavailable, companyId), Alert.Danger);
				return RedirectToAction(nameof(All));
			}

			var companyName = this.companies.GetCompanyName(companyId);
			var companyStatus = this.companies.GetBlockStatus(companyId);

			this.GenerateAlertMessage(string.Format(WebConstants.Message.CompanyStatusChanged, companyName, companyStatus), Alert.Success);

			return RedirectToAction(nameof(All), new { page = page, filter = filter});
		}
    }
}
