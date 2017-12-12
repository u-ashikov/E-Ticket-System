namespace ETicketSystem.Web.Models.Pagination
{
	using System;

	public class PaginationViewModel
    {
		public string Controller { get; set; }

		public string Action { get; set; }

		public string SearchTerm { get; set; }

		public int TotalElements { get; set; }

		public int CurrentPage { get; set; }

		public int PageSize { get; set; }

		public int TotalPages => (int)Math.Ceiling(this.TotalElements / (double)this.PageSize);

		public int NextPage => this.CurrentPage >= this.TotalPages ? this.TotalPages : this.CurrentPage + 1;

		public int PreviousPage => this.CurrentPage <= 1 ? 1 : this.CurrentPage - 1;
    }
}
