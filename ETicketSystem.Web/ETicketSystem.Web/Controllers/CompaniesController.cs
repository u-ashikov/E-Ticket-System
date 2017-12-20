namespace ETicketSystem.Web.Controllers
{
	using Common.Constants;
	using Common.Enums;
	using Microsoft.AspNetCore.Mvc;
	using Models.Companies;
	using Models.Pagination;
	using Models.Reviews;
	using Models.Routes;
	using Services.Contracts;

	public class CompaniesController : BaseController
    {
		private readonly ICompanyService companies;

		private readonly IReviewService reviews;

		public CompaniesController(ICompanyService companies,ITownService towns, IReviewService reviews)
			:base(towns)
		{
			this.companies = companies;
			this.reviews = reviews;
		}

		public IActionResult All(string searchTerm, int page = 1)
		{
			if (page < 1)
			{
				return RedirectToAction(nameof(All),new { searchTerm = searchTerm});
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
				return RedirectToAction(nameof(All), new { searchTerm = searchTerm,page = companiesPagination.TotalPages });
			}

			return View(new AllCompanies()
			{
				Companies = this.companies.All(page,searchTerm, WebConstants.Pagination.CompaniesPageSize),
				Pagination = companiesPagination
			});
		}

		public IActionResult Details(string id, int page = 1)
		{
			var company = this.companies.CompanyDetails(id);

			if (company == null)
			{
				this.GenerateAlertMessage(string.Format(WebConstants.Message.NonExistingEntity,nameof(WebConstants.Entity.Company),id), Alert.Warning);

				return RedirectToAction(nameof(All));
			}

			if (page < 1)
			{
				return RedirectToAction(nameof(Details), new { id = id, page = 1 });
			}

			var reviewsPagination = new PaginationViewModel()
			{
				Action = nameof(Details),
				Controller = WebConstants.Controller.Companies,
				CurrentPage = page,
				PageSize = WebConstants.Pagination.CompanyReviewsListing,
				TotalElements = this.reviews.TotalReviews(id)
			};

			if (page > reviewsPagination.TotalPages && reviewsPagination.TotalPages != 0)
			{
				return RedirectToAction(nameof(Details), new { id = id, page = reviewsPagination.TotalPages });
			}

			return View(new CompanyDetails()
			{
				Company = company,
				SearchForm = new SearchRouteFormModel()
				{
					CompanyId = id,
					Towns = this.GenerateSelectListTowns()
				},
				ReviewForm = new ReviewFormModel()
				{
					CompanyId = id
				},
				Reviews = new AllCompanyReviews()
				{
					Reviews = this.reviews.All(id,page,WebConstants.Pagination.CompanyReviewsListing),
					Pagination = reviewsPagination
				}
			});
		}
    }
}
