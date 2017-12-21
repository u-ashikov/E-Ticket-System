namespace ETicketSystem.Web.Models.Account
{
	using Common.Constants;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.Rendering;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;

	public class RegisterCompanyFormModel : IValidatableObject
    {
		[Required]
		[StringLength(DataConstants.User.UsernameMaxLength, ErrorMessage = WebConstants.Message.UsernameLength, MinimumLength = DataConstants.User.UsernameMinLength)]
		public string Username { get; set; }

		[Required]
		[RegularExpression(WebConstants.RegexPattern.Email, ErrorMessage = WebConstants.Message.InvalidEmail)]
		public string Email { get; set; }

		[Required]
		[StringLength(DataConstants.User.PasswordMaxLength, ErrorMessage = WebConstants.Message.PasswordLength, MinimumLength = DataConstants.User.PasswordMinLength)]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = WebConstants.FieldDisplay.ConfirmPassword)]
		[Compare(nameof(Password), ErrorMessage = WebConstants.Message.PasswordsMissmatch)]
		public string ConfirmPassword { get; set; }

		[Required]
		[RegularExpression(WebConstants.RegexPattern.Name, ErrorMessage = WebConstants.Message.NameContainOnlyLetters)]
		[Remote(action:WebConstants.Action.VerifyCompanyName,controller: WebConstants.Controller.Account)]
		[StringLength(DataConstants.Company.NameMaxLength, ErrorMessage = WebConstants.Message.CompanyNameLength,MinimumLength = DataConstants.Company.NameMinLength)]
		public string Name { get; set; }

		public IFormFile Logo { get; set; }

		[Required]
		[StringLength(DataConstants.Company.DescriptionMaxLength, ErrorMessage = WebConstants.Message.CompanyDescriptionLength, MinimumLength = DataConstants.Company.DescriptionMinLength)]
		public string Description { get; set; }

		[Required]
		[RegularExpression(WebConstants.RegexPattern.CompanyUniqueReferenceNumber, ErrorMessage = WebConstants.Message.UniqueReferenceNumberFormat)]
		[Remote(action: WebConstants.Action.VerifyUrn,controller:WebConstants.Controller.Account)]
		[Display(Name = WebConstants.FieldDisplay.UniqueReferenceNumber)]
		public string UniqueReferenceNumber { get; set; }

		[Required]
		[RegularExpression(WebConstants.RegexPattern.Name, ErrorMessage = WebConstants.Message.NameContainOnlyLetters)]
		[MaxLength(DataConstants.Company.ChiefNameMaxLength, ErrorMessage = WebConstants.Message.CompanyChiefNameMaxLength)]
		[Display(Name = WebConstants.FieldDisplay.ChiefFirstName)]
		public string ChiefFirstName { get; set; }

		[Required]
		[RegularExpression(WebConstants.RegexPattern.Name, ErrorMessage = WebConstants.Message.NameContainOnlyLetters)]
		[MaxLength(DataConstants.Company.ChiefNameMaxLength, ErrorMessage = WebConstants.Message.CompanyChiefNameMaxLength)]
		[Display(Name = WebConstants.FieldDisplay.ChiefLastName)]
		public string ChiefLastName { get; set; }

		[Required]
		[StringLength(DataConstants.Company.AddressMaxLength, ErrorMessage = WebConstants.Message.CompanyAddressLength, MinimumLength = DataConstants.Company.AddressMinLength)]
		public string Address { get; set; }

		public int Town { get; set; }

		[Required]
		[RegularExpression(WebConstants.RegexPattern.Phone, ErrorMessage =  WebConstants.Message.PhoneNumberFormat)]
		[Display(Name = WebConstants.FieldDisplay.PhoneNumber)]
		public string PhoneNumber { get; set; }

		public List<SelectListItem> Towns { get; set; } = new List<SelectListItem>();

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if (this.Logo != null)
			{
				if (this.Logo.Length > DataConstants.Company.LogoMaxLength)
				{
					yield return new ValidationResult(WebConstants.Message.LogoMaxLength);
				}

				if (!this.Logo.FileName.ToLower().EndsWith(WebConstants.PictureFormat.Jpg)
					&& !this.Logo.FileName.ToLower().EndsWith(WebConstants.PictureFormat.Bmp)
					&& !this.Logo.FileName.ToLower().EndsWith(WebConstants.PictureFormat.Png))
				{
					yield return new ValidationResult(WebConstants.Message.LogoAvailableFormats);
				}
			}
		}
	}
}
