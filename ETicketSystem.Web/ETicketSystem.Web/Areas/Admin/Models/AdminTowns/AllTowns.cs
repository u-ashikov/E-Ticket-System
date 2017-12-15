namespace ETicketSystem.Web.Areas.Admin.Models.AdminTowns
{
	using Services.Admin.Models;
	using System.Collections.Generic;
	using Web.Models.Pagination;

	public class AllTowns
    {
		public IEnumerable<AdminTownListingServiceModel> Towns { get; set; }

		public PaginationViewModel Pagination { get; set; }
    }
}
