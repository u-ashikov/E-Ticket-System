namespace ETicketSystem.Web.Areas.Admin.Models.AdminTowns
{
	using Common.Constants;
	using System.ComponentModel.DataAnnotations;

	public class AddTownFormModel
    {
		[Required]
		[MaxLength(DataConstants.Town.TownMaxNameLength, ErrorMessage = WebConstants.Message.TownNameMaxLength)]
		public string Name { get; set; }
	}
}
