namespace ETicketSystem.Web.Models.Account
{
	using ETicketSystem.Common.Constants;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.Rendering;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;

	public class RegisterCompanyFormModel : IValidatableObject
    {
		[Required]
		[StringLength(DataConstants.User.UsernameMaxLength, ErrorMessage = WebConstants.Error.UsernameLength, MinimumLength = DataConstants.User.UsernameMinLength)]
		public string Username { get; set; }

		[Required]
		[RegularExpression(WebConstants.RegexPattern.Email, ErrorMessage = WebConstants.Error.InvalidEmail)]
		public string Email { get; set; }

		[Required]
		[StringLength(DataConstants.User.PasswordMaxLength, ErrorMessage = WebConstants.Error.PasswordLength, MinimumLength = DataConstants.User.PasswordMinLength)]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = WebConstants.FieldDisplay.ConfirmPassword)]
		[Compare(nameof(Password), ErrorMessage = WebConstants.Error.PasswordsMissmatch)]
		public string ConfirmPassword { get; set; }

		[Required]
		[Remote(action:WebConstants.Action.VerifyCompanyName,controller: WebConstants.Controller.Account)]
		[StringLength(DataConstants.Company.NameMaxLength, ErrorMessage = WebConstants.Error.CompanyNameLength,MinimumLength = DataConstants.Company.NameMinLength)]
		public string Name { get; set; }

		public IFormFile Logo { get; set; }

		[Required]
		[StringLength(DataConstants.Company.DescriptionMaxLength, ErrorMessage = WebConstants.Error.CompanyDescriptionLength, MinimumLength = DataConstants.Company.DescriptionMinLength)]
		public string Description { get; set; }

		[Required]
		[RegularExpression(WebConstants.RegexPattern.CompanyUniqueReferenceNumber, ErrorMessage = WebConstants.Error.UniqueReferenceNumberFormat)]
		[Remote(action: WebConstants.Action.VerifyUrn,controller:WebConstants.Controller.Account)]
		[Display(Name = WebConstants.FieldDisplay.UniqueReferenceNumber)]
		public string UniqueReferenceNumber { get; set; }

		[Required]
		[MaxLength(DataConstants.Company.ChiefNameMaxLength, ErrorMessage = WebConstants.Error.CompanyChiefNameMaxLength)]
		[Display(Name = WebConstants.FieldDisplay.ChiefFirstName)]
		public string ChiefFirstName { get; set; }

		[Required]
		[MaxLength(DataConstants.Company.ChiefNameMaxLength, ErrorMessage = WebConstants.Error.CompanyChiefNameMaxLength)]
		[Display(Name = WebConstants.FieldDisplay.ChiefLastName)]
		public string ChiefLastName { get; set; }

		[Required]
		[StringLength(DataConstants.Company.AddressMaxLength, ErrorMessage = WebConstants.Error.CompanyAddressLength, MinimumLength = DataConstants.Company.AddressMinLength)]
		public string Address { get; set; }

		public int Town { get; set; }

		[Required]
		[RegularExpression(WebConstants.RegexPattern.Phone, ErrorMessage =  WebConstants.Error.PhoneNumberFormat)]
		[Remote(action:WebConstants.Action.VerifyPhoneNumber,controller:WebConstants.Controller.Account)]
		[Display(Name = WebConstants.FieldDisplay.PhoneNumber)]
		public string PhoneNumber { get; set; }

		public List<SelectListItem> Towns { get; set; } = new List<SelectListItem>();

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if (this.Logo != null)
			{
				if (this.Logo.Length > DataConstants.Company.LogoMaxLength)
				{
					yield return new ValidationResult(WebConstants.Error.LogoMaxLength);
				}

				if (!this.Logo.FileName.EndsWith(WebConstants.PictureFormat.Jpg)
					&& !this.Logo.FileName.EndsWith(WebConstants.PictureFormat.Bmp)
					&& !this.Logo.FileName.EndsWith(WebConstants.PictureFormat.Png))
				{
					yield return new ValidationResult(WebConstants.Error.LogoAvailableFormats);
				}
			}
		}
	}
}
