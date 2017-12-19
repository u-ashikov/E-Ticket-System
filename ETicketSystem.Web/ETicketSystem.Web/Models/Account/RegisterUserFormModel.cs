namespace ETicketSystem.Web.Models.Account
{
	using Common.Constants;
	using Data.Enums;
	using System.ComponentModel.DataAnnotations;

	public class RegisterUserFormModel
    {
        [Required]
        [RegularExpression(WebConstants.RegexPattern.Email,ErrorMessage = WebConstants.Message.InvalidEmail)]
        public string Email { get; set; }

		[Required]
		[StringLength(DataConstants.User.UsernameMaxLength,ErrorMessage = WebConstants.Message.UsernameLength,MinimumLength = DataConstants.User.UsernameMinLength)]
		public string Username { get; set; }

		[Required]
		[RegularExpression(WebConstants.RegexPattern.Name, ErrorMessage = WebConstants.Message.NameContainOnlyLetters)]
		[Display(Name = WebConstants.FieldDisplay.FirstName)]
		[MaxLength(DataConstants.User.NameMaxLength, ErrorMessage = WebConstants.Message.RegularUserNameMaxLength)]
		public string FirstName { get; set; }

		[Required]
		[RegularExpression(WebConstants.RegexPattern.Name, ErrorMessage = WebConstants.Message.NameContainOnlyLetters)]
		[Display(Name = WebConstants.FieldDisplay.LastName)]
		[MaxLength(DataConstants.User.NameMaxLength, ErrorMessage = WebConstants.Message.RegularUserNameMaxLength)]
		public string LastName { get; set; }

        [Required]
        [StringLength(DataConstants.User.PasswordMaxLength, ErrorMessage = WebConstants.Message.PasswordLength, MinimumLength = DataConstants.User.PasswordMinLength)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = WebConstants.FieldDisplay.ConfirmPassword)]
        [Compare(nameof(Password), ErrorMessage = WebConstants.Message.PasswordsMissmatch)]
        public string ConfirmPassword { get; set; }

		public Gender Gender { get; set; }
    }
}
