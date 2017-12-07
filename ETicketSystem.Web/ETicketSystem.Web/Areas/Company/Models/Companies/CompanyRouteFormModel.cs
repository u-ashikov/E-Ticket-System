namespace ETicketSystem.Web.Areas.Company.Models.Companies
{
	using ETicketSystem.Common.Constants;
	using ETicketSystem.Data.Enums;
	using Microsoft.AspNetCore.Mvc.Rendering;
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;

	public class CompanyRouteFormModel : IValidatableObject
    {
		[Display(Name = WebConstants.FieldDisplay.RouteStartStation)]
		public int StartStation { get; set; }

		[Display(Name = WebConstants.FieldDisplay.RouteEndStation)]
		public int EndStation { get; set; }

		[DataType(DataType.Time)]
		[Display(Name = WebConstants.FieldDisplay.RouteDepartureTime)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
		public DateTime DepartureTime { get; set; }

		[DataType(DataType.Time)]
		public TimeSpan Duration { get; set; }

		[Required]
		[Display(Name = WebConstants.FieldDisplay.RouteBusType)]
		public BusType BusType { get; set; }

		[Range(DataConstants.Route.PriceMinValue, double.MaxValue)]
		public decimal Price { get; set; }

		public List<SelectListItem> TownsStations { get; set; }

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if (this.StartStation == this.EndStation)
			{
				yield return new ValidationResult(WebConstants.Error.StartStationEqualToEndStation);
			}
		}
	}
}
