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
		[StringLength(DataConstants.Company.NameMaxLength,MinimumLength = DataConstants.Company.NameMinLength)]
		public string Name { get; set; }

		[MaxLength(DataConstants.Company.LogoMaxLength)]
		public byte[] Logo { get; set; }

		[Required]
		[StringLength(DataConstants.Company.DescriptionMaxLength,MinimumLength = DataConstants.Company.DescriptionMinLength)]
		public string Description { get; set; }

		[Required]
		[StringLength(DataConstants.Company.UniqueReferenceNumberMaxLength,MinimumLength = DataConstants.Company.UniqueReferenceNumberMinLength)]
		[Display(Name = WebConstants.FieldDisplay.UniqueReferenceNumber)]
		public string UniqueReferenceNumber { get; set; }

		[Required]
		[MaxLength(DataConstants.Company.ChiefNameMaxLength)]
		[Display(Name = WebConstants.FieldDisplay.ChiefFirstName)]
		public string ChiefFirstName { get; set; }

		[Required]
		[MaxLength(DataConstants.Company.ChiefNameMaxLength)]
		[Display(Name = WebConstants.FieldDisplay.ChiefLastName)]
		public string ChiefLastName { get; set; }

		[Required]
		[StringLength(DataConstants.Company.AddressMaxLength,MinimumLength = DataConstants.Company.AddressMinLength)]
		public string Address { get; set; }

		public int Town { get; set; }

		public List<SelectListItem> Towns { get; set; } = new List<SelectListItem>();
	}
}
