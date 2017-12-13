namespace ETicketSystem.Web.Models.Routes
{
	using Models.Pagination;
	using Services.Models.Route;
	using System.Collections.Generic;

	public class SearchedRoutes
    {
		public IEnumerable<RouteSearchListingServiceModel> Routes;

		public PaginationViewModel Pagination { get; set; }

		public SearchRouteFormModel SearchForm { get; set; }
	}
}
