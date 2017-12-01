namespace ETicketSystem.Web.Models.Account
{
	using System.ComponentModel.DataAnnotations;

	public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
