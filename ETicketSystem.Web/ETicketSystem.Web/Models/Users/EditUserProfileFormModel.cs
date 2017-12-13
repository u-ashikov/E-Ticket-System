namespace ETicketSystem.Web.Models.Users
{
	using Common.Constants;
	using System.ComponentModel.DataAnnotations;

	public class EditUserProfileFormModel
    {
		[Required]
		[RegularExpression(WebConstants.RegexPattern.Email, ErrorMessage = WebConstants.Message.InvalidEmail)]
		public string Email { get; set; }

		[Required]
		[StringLength(DataConstants.User.UsernameMaxLength, ErrorMessage = WebConstants.Message.UsernameLength, MinimumLength = DataConstants.User.UsernameMinLength)]
		public string Username { get; set; }

		[StringLength(DataConstants.User.PasswordMaxLength, ErrorMessage = WebConstants.Message.PasswordLength, MinimumLength = DataConstants.User.PasswordMinLength)]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[StringLength(DataConstants.User.PasswordMaxLength, ErrorMessage = WebConstants.Message.PasswordLength, MinimumLength = DataConstants.User.PasswordMinLength)]
		[DataType(DataType.Password)]
		[Display(Name = WebConstants.FieldDisplay.NewPassword)]
		public string NewPassword { get; set; }
	}
}
