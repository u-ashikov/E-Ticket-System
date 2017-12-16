﻿namespace ETicketSystem.Web.Controllers
{
	using Common.Constants;
	using Common.Enums;
	using ETicketSystem.Web.Models.Reviews;
	using Microsoft.AspNetCore.Mvc;
	using Models.Companies;
	using Models.Pagination;
	using Models.Routes;
	using Services.Contracts;

	public class CompaniesController : BaseController
    {
		private readonly ICompanyService companies;

		public CompaniesController(ICompanyService companies,ITownService towns)
			:base(towns)
		{
			this.companies = companies;
		}

		public IActionResult All(string searchTerm, int page = 1)
		{
			if (page < 1)
			{
				return RedirectToAction(nameof(All));
			}

			var companiesPagination = new PaginationViewModel()
			{
				CurrentPage = page,
				PageSize = WebConstants.Pagination.CompaniesPageSize,
				TotalElements = this.companies.TotalCompanies(searchTerm),
				SearchTerm = searchTerm,
				Action = nameof(All),
				Controller = WebConstants.Controller.Companies
			};

			if (page > companiesPagination.TotalPages && companiesPagination.TotalPages != 0)
			{
				return RedirectToAction(nameof(All), new { page = companiesPagination.TotalPages });
			}

			return View(new AllCompaniesViewModel()
			{
				Companies = this.companies.All(page,searchTerm, WebConstants.Pagination.CompaniesPageSize),
				Pagination = companiesPagination
			});
		}

		public IActionResult Details(string id)
		{
			var company = this.companies.CompanyDetails(id);

			if (company == null)
			{
				this.GenerateAlertMessage(string.Format(WebConstants.Message.NonExistingCompany,id), Alert.Warning);

				return RedirectToAction(nameof(All));
			}

			return View(new CompanyDetailsViewModel()
			{
				Company = company,
				SearchForm = new SearchRouteFormModel()
				{
					CompanyId = id,
					Towns = this.GenerateSelectListTowns()
				},
				ReviewForm = new AddReviewFormModel()
				{
					CompanyId = id
				},
				Reviews = company.Reviews
			});
		}
    }
}
