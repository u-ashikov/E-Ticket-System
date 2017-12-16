namespace ETicketSystem.Web.Models.Companies
{
	using Reviews;
	using Routes;
	using Services.Models.Company;
	using Services.Models.Review;
	using System.Collections.Generic;

	public class CompanyDetailsViewModel
    {
		public CompanyDetailsServiceModel Company { get; set; }

		public SearchRouteFormModel SearchForm { get; set; }

		public AddReviewFormModel ReviewForm { get; set; }

		public IEnumerable<ReviewInfoServiceModel> Reviews { get; set; }
	}
}
