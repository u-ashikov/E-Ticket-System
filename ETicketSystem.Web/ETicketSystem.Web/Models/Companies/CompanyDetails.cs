namespace ETicketSystem.Web.Models.Companies
{
	using Reviews;
	using Routes;
	using Services.Models.Company;

	public class CompanyDetails
    {
		public CompanyDetailsServiceModel Company { get; set; }

		public SearchRouteFormModel SearchForm { get; set; }

		public ReviewFormModel ReviewForm { get; set; }

		public AllCompanyReviews Reviews { get; set; }
	}
}
