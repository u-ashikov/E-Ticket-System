namespace ETicketSystem.Web.Areas.Admin.Models.AdminTowns
{
	using Common.Constants;
	using System.ComponentModel.DataAnnotations;

	public class AddTownFormModel
    {
		[Required]
		[RegularExpression(WebConstants.RegexPattern.Name, ErrorMessage = WebConstants.Message.NameContainOnlyLetters)]
		[MaxLength(DataConstants.Town.TownMaxNameLength, ErrorMessage = WebConstants.Message.TownNameMaxLength)]
		public string Name { get; set; }
	}
}
