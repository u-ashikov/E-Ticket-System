namespace ETicketSystem.Web.Models.Routes
{
	using Common.Constants;
	using Microsoft.AspNetCore.Mvc.Rendering;
	using Models.Pagination;
	using Services.Models.Route;
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;

	public class SearchedRoutes
    {
		public IEnumerable<RouteSearchListingServiceModel> Routes;

		public PaginationViewModel Pagination { get; set; }

		[Display(Name = WebConstants.FieldDisplay.StartDestination)]
		public int StartTown { get; set; }

		[Display(Name = WebConstants.FieldDisplay.EndDestination)]
		public int EndTown { get; set; }

		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:MM-dd-yyyy}", ApplyFormatInEditMode = true)]
		public DateTime Date { get; set; }

		[Display(Name = WebConstants.FieldDisplay.Company)]
		public string CompanyId { get; set; }

		public IEnumerable<SelectListItem> Towns { get; set; }

		public IEnumerable<SelectListItem> Companies { get; set; }
	}
}
