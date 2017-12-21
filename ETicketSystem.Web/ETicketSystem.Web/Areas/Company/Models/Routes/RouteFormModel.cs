namespace ETicketSystem.Web.Areas.Company.Models.Routes
{
	using Common.Constants;
	using Data.Enums;
	using Microsoft.AspNetCore.Mvc.Rendering;
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;

	public class RouteFormModel : IValidatableObject
    {
		[Display(Name = WebConstants.FieldDisplay.RouteStartStation)]
		public int StartStation { get; set; }

		[Display(Name = WebConstants.FieldDisplay.RouteEndStation)]
		public int EndStation { get; set; }

		[DataType(DataType.Time)]
		[Display(Name = WebConstants.FieldDisplay.RouteDepartureTime)]
		[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = DataConstants.Route.DepartureTimeFormat)]
		public DateTime DepartureTime { get; set; }

		[DataType(DataType.Time)]
		public TimeSpan Duration { get; set; }

		[Required]
		[Display(Name = WebConstants.FieldDisplay.RouteBusType)]
		public BusType BusType { get; set; }

		[Range(DataConstants.Route.PriceMinValue, double.MaxValue,ErrorMessage = WebConstants.Message.PositivePrice)]
		public decimal Price { get; set; }

		public bool IsEdit { get; set; }

		public List<SelectListItem> TownsStations { get; set; }

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if (this.Duration == new TimeSpan(0,0,0))
			{
				yield return new ValidationResult(WebConstants.Message.RouteDurationZeroLength);
			}
		}
	}
}
