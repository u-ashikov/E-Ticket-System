﻿namespace ETicketSystem.Web.Areas.Company.Models.Companies
{
	using ETicketSystem.Common.Constants;
	using ETicketSystem.Data.Enums;
	using Microsoft.AspNetCore.Mvc.Rendering;
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;

	public class CompanyRouteFormModel
    {
		public int StartStation { get; set; }

		public int EndStation { get; set; }

		[DataType(DataType.Time)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
		public DateTime DepartureTime { get; set; }

		[DataType(DataType.Time)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
		public DateTime Duration { get; set; }

		[Required]
		public BusType BusType { get; set; }

		[Range(DataConstants.Route.PriceMinValue, double.MaxValue)]
		public decimal Price { get; set; }

		public List<SelectListItem> TownsStations { get; set; }
	}
}
