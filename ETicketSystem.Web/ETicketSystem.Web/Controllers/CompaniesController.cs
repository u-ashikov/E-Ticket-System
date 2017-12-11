namespace ETicketSystem.Web.Controllers
{
	using Common.Constants;
	using Microsoft.AspNetCore.Mvc;
	using Models.Companies;
	using Models.Pagination;
	using Services.Contracts;

	public class CompaniesController : BaseController
    {
		private readonly ICompanyService companies;

		public CompaniesController(ICompanyService companies)
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
				SearchTerm = searchTerm
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
    }
}
