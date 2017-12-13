namespace ETicketSystem.Web.Areas.Admin.Models.AdminCompanies
{
	using Common.Enums;
	using Services.Admin.Models;
	using System.Collections.Generic;
	using Web.Models.Pagination;

	public class AllCompanies
    {
		public IEnumerable<AdminCompanyListingServiceModel> Companies { get; set; }

		public CompanyStatus Filter { get; set; }

		public PaginationViewModel Pagination { get; set; }
    }
}
