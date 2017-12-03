namespace ETicketSystem.Data.Models
{
	using ETicketSystem.Common.Constants;
	using ETicketSystem.Data.Enums;
	using Microsoft.AspNetCore.Identity;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;

	public class User : IdentityUser
    {
		[Required]
		[MaxLength(DataConstants.User.NameMaxLength)]
		public string FirstName { get; set; }

		[Required]
		[MaxLength(DataConstants.User.NameMaxLength)]
		public string LastName { get; set; }

		[Required]
		public Gender Gender { get; set; }

		public List<Review> Reviews { get; set; } = new List<Review>();

		public List<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
