namespace ETicketSystem.Data.Models
{
	using Microsoft.AspNetCore.Identity;
	using System;
	using System.Collections.Generic;

	public class User : IdentityUser
    {
		public List<Review> Reviews { get; set; } = new List<Review>();

		public DateTime? RegistrationDate { get; set; }
	}
}
