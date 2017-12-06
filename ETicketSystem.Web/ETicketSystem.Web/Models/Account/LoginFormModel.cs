namespace ETicketSystem.Web.Models.Account
{
	using ETicketSystem.Common.Constants;
	using System.ComponentModel.DataAnnotations;

	public class LoginFormModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = WebConstants.FieldDisplay.RememberMe)]
        public bool RememberMe { get; set; }
    }
}
