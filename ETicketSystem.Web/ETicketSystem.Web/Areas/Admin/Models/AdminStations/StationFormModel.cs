namespace ETicketSystem.Web.Areas.Admin.Models.AdminStations
{
	using Common.Constants;
	using Microsoft.AspNetCore.Mvc.Rendering;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;

	public class StationFormModel
    {
		public int Id { get; set; }

		[Required]
		[MaxLength(DataConstants.Station.NameMaxLength, ErrorMessage = WebConstants.Message.StationNameMaxLength)]
		public string Name { get; set; }

		[Display(Name = WebConstants.FieldDisplay.Town)]
		public int TownId { get; set; }

		public IEnumerable<SelectListItem> Towns { get; set; }

		[Required]
		[RegularExpression(WebConstants.RegexPattern.Phone, ErrorMessage = WebConstants.Message.PhoneNumberFormat)]
		[MaxLength(DataConstants.Station.PhoneMaxLength, ErrorMessage = WebConstants.Message.StationPhoneMaxLength)]
		public string Phone { get; set; }

		public bool IsEdit { get; set; }
	}
}
