namespace ETicketSystem.Web.Models.Companies
{
	using Routes;
	using Services.Models.Company;

	public class CompanyDetailsViewModel
    {
		public CompanyDetailsServiceModel Company { get; set; }

		public SearchRouteFormModel SearchForm { get; set; }
    }
}
