namespace ETicketSystem.Data.Models
{
	using Common.Constants;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;

	public class Company : User
	{
		[Required]
		[MinLength(DataConstants.Company.NameMinLength)]
		[MaxLength(DataConstants.Company.NameMaxLength)]
		public string Name { get; set; }

		[MaxLength(DataConstants.Company.LogoMaxLength)]
		public byte[] Logo { get; set; }

		[Required]
		[MinLength(DataConstants.Company.DescriptionMinLength)]
		[MaxLength(DataConstants.Company.DescriptionMaxLength)]
		public string Description { get; set; }

		[Required]
		[MinLength(DataConstants.Company.UniqueReferenceNumberMinLength)]
		[MaxLength(DataConstants.Company.UniqueReferenceNumberMaxLength)]
		public string UniqueReferenceNumber { get; set; }

		[Required]
		[MaxLength(DataConstants.Company.ChiefNameMaxLength)]
		public string ChiefFirstName { get; set; }

		[Required]
		[MaxLength(DataConstants.Company.ChiefNameMaxLength)]
		public string ChiefLastName { get; set; }

		[Required]
		[MinLength(DataConstants.Company.AddressMinLength)]
		[MaxLength(DataConstants.Company.AddressMaxLength)]
		public string Address { get; set; }

		public int TownId { get; set; }

		public Town Town { get; set; }

		public bool IsApproved { get; set; }

		public bool IsBlocked { get; set; }

		public List<Route> Routes { get; set; } = new List<Route>();
	}
}
