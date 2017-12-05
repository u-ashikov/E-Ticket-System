namespace ETicketSystem.Web.Models.Account
{
	using ETicketSystem.Common.Constants;
	using ETicketSystem.Data.Enums;
	using System.ComponentModel.DataAnnotations;

	public class RegisterUserViewModel
    {
        [Required]
        [RegularExpression(WebConstants.RegexPattern.Email,ErrorMessage = WebConstants.Error.InvalidEmail)]
        public string Email { get; set; }

		[Required]
		[StringLength(DataConstants.User.UsernameMaxLength,ErrorMessage = WebConstants.Error.UsernameLength,MinimumLength = DataConstants.User.UsernameMinLength)]
		public string Username { get; set; }

		[Required]
		[Display(Name = WebConstants.FieldDisplay.FirstName)]
		[MaxLength(DataConstants.User.NameMaxLength)]
		public string FirstName { get; set; }

		[Required]
		[Display(Name = WebConstants.FieldDisplay.LastName)]
		[MaxLength(DataConstants.User.NameMaxLength)]
		public string LastName { get; set; }

        [Required]
        [StringLength(DataConstants.User.PasswordMaxLength, ErrorMessage = WebConstants.Error.PasswordLength, MinimumLength = DataConstants.User.PasswordMinLength)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = WebConstants.FieldDisplay.ConfirmPassword)]
        [Compare(nameof(Password), ErrorMessage = WebConstants.Error.PasswordsMissmatch)]
        public string ConfirmPassword { get; set; }

		public Gender Gender { get; set; }
    }
}
