namespace ETicketSystem.Web.Models.Companies
{
	using Models.Pagination;
	using Services.Models.Company;
	using System.Collections.Generic;

	public class AllCompanies
    {
		public IEnumerable<CompanyListingServiceModel> Companies;

		public PaginationViewModel Pagination { get; set; }
    }
}
