namespace ETicketSystem.Web.Models.Reviews
{
	using Pagination;
	using Services.Models.Review;
	using System.Collections.Generic;

	public class AllCompanyReviews
    {
		public IEnumerable<ReviewInfoServiceModel> Reviews { get; set; }

		public PaginationViewModel Pagination { get; set; }
    }
}
