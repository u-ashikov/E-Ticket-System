namespace ETicketSystem.Web.Models.Account
{
	using ETicketSystem.Common.Constants;
	using Microsoft.AspNetCore.Mvc.Rendering;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;

	public class RegisterCompanyViewModel
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
		[StringLength(DataConstants.Company.NameMaxLength, ErrorMessage = WebConstants.Error.CompanyNameLength,MinimumLength = DataConstants.Company.NameMinLength)]
		public string Name { get; set; }

		[MaxLength(DataConstants.Company.LogoMaxLength)]
		public byte[] Logo { get; set; }

		[Required]
		[StringLength(DataConstants.Company.DescriptionMaxLength, ErrorMessage = WebConstants.Error.CompanyDescriptionLength, MinimumLength = DataConstants.Company.DescriptionMinLength)]
		public string Description { get; set; }

		[Required]
		[RegularExpression(WebConstants.RegexPattern.CompanyUniqueReferenceNumber, ErrorMessage = WebConstants.Error.UniqueReferenceNumberFormat)]
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
		[Display(Name = WebConstants.FieldDisplay.PhoneNumber)]
		public string PhoneNumber { get; set; }

		public List<SelectListItem> Towns { get; set; } = new List<SelectListItem>();
	}
}
