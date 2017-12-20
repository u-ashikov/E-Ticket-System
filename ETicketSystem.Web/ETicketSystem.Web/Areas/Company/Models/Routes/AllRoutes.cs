namespace ETicketSystem.Web.Areas.Company.Models.Routes
{
	using Common.Constants;
	using Microsoft.AspNetCore.Mvc.Rendering;
	using Services.Company.Models;
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;
	using Web.Models.Pagination;

	public class AllRoutes
    {
		public IEnumerable<CompanyRouteListingServiceModel> Routes { get; set; }

		public PaginationViewModel Pagination { get; set; }

		[Display(Name = WebConstants.FieldDisplay.StartDestination)]
		public int StartTown { get; set; }

		[Display(Name = WebConstants.FieldDisplay.EndDestination)]
		public int EndTown { get; set; }

		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
		public DateTime? Date { get; set; }

		public IEnumerable<SelectListItem> Towns { get; set; }
	}
}
