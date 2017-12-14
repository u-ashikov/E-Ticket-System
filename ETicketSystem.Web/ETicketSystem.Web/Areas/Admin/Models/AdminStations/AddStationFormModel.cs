﻿namespace ETicketSystem.Web.Areas.Admin.Models.AdminStations
{
	using Common.Constants;
	using Microsoft.AspNetCore.Mvc.Rendering;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;

	public class AddStationFormModel
    {
		[Required]
		[MaxLength(DataConstants.Station.NameMaxLength)]
		public string Name { get; set; }

		[Display(Name = WebConstants.FieldDisplay.Town)]
		public int TownId { get; set; }

		public IEnumerable<SelectListItem> Towns { get; set; }

		[Required]
		[MaxLength(DataConstants.Station.PhoneMaxLength)]
		public string Phone { get; set; }
	}
}
