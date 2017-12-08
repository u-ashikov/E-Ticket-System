namespace ETicketSystem.Web.Models.Routes
{
	using ETicketSystem.Common.Constants;
	using Microsoft.AspNetCore.Mvc.Rendering;
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;

	public class SearchRouteFormModel : IValidatableObject
    {
		[Display(Name = WebConstants.FieldDisplay.StartDestination)]
		public int StartTown { get; set; }

		[Display(Name = WebConstants.FieldDisplay.EndDestination)]
		public int EndTown { get; set; }

		[DataType(DataType.Date)]
		public DateTime Date { get; set; }

		public IEnumerable<SelectListItem> Towns { get; set; }

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if (this.StartTown == this.EndTown)
			{
				yield return new ValidationResult(WebConstants.Message.StartStationEqualToEndStation);
			}
		}
	}
}
