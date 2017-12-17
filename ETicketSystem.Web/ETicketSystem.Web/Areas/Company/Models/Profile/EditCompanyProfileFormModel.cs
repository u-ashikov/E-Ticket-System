namespace ETicketSystem.Web.Areas.Company.Models.Profile
{
	using Common.Constants;
	using Microsoft.AspNetCore.Http;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;

	public class EditCompanyProfileFormModel : IValidatableObject
    {
		[Required]
		[StringLength(DataConstants.User.UsernameMaxLength, ErrorMessage = WebConstants.Message.UsernameLength, MinimumLength = DataConstants.User.UsernameMinLength)]
		public string Username { get; set; }

		[Required]
		[RegularExpression(WebConstants.RegexPattern.Email, ErrorMessage = WebConstants.Message.InvalidEmail)]
		public string Email { get; set; }

		public byte[] CurrentLogo { get; set; }

		public IFormFile Logo { get; set; }

		[Required]
		[StringLength(DataConstants.Company.DescriptionMaxLength, ErrorMessage = WebConstants.Message.CompanyDescriptionLength, MinimumLength = DataConstants.Company.DescriptionMinLength)]
		public string Description { get; set; }

		[StringLength(DataConstants.User.PasswordMaxLength, ErrorMessage = WebConstants.Message.PasswordLength, MinimumLength = DataConstants.User.PasswordMinLength)]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[StringLength(DataConstants.User.PasswordMaxLength, ErrorMessage = WebConstants.Message.PasswordLength, MinimumLength = DataConstants.User.PasswordMinLength)]
		[DataType(DataType.Password)]
		[Display(Name = WebConstants.FieldDisplay.NewPassword)]
		public string NewPassword { get; set; }

		[Required]
		[RegularExpression(WebConstants.RegexPattern.Phone, ErrorMessage = WebConstants.Message.PhoneNumberFormat)]
		[Display(Name = WebConstants.FieldDisplay.PhoneNumber)]
		public string PhoneNumber { get; set; }

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if (this.Logo != null)
			{
				if (this.Logo.Length > DataConstants.Company.LogoMaxLength)
				{
					yield return new ValidationResult(WebConstants.Message.LogoMaxLength);
				}

				if (!this.Logo.FileName.EndsWith(WebConstants.PictureFormat.Jpg)
					&& !this.Logo.FileName.EndsWith(WebConstants.PictureFormat.Bmp)
					&& !this.Logo.FileName.EndsWith(WebConstants.PictureFormat.Png))
				{
					yield return new ValidationResult(WebConstants.Message.LogoAvailableFormats);
				}
			}
		}
	}
}
