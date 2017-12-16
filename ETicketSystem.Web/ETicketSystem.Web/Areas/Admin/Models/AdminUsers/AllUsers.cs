namespace ETicketSystem.Web.Areas.Admin.Models.AdminUsers
{
	using Services.Admin.Models;
	using System.Collections.Generic;
	using Web.Models.Pagination;

	public class AllUsers
    {
		public IEnumerable<AdminUserListingServiceModel> Users { get; set; }

		public PaginationViewModel Pagination { get; set; }
    }
}
