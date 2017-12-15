namespace ETicketSystem.Web.Areas.Admin.Models.AdminStations
{
	using Services.Admin.Models;
	using System.Collections.Generic;
	using Web.Models.Pagination;

	public class AllStations
    {
		public IEnumerable<AdminStationListingServiceModel> Stations { get; set; }

		public PaginationViewModel Pagination { get; set; }
    }
}
