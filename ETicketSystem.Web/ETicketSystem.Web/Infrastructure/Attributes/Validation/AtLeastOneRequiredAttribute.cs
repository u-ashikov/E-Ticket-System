namespace ETicketSystem.Web.Infrastructure.Attributes.Validation
{
	using Models.Routes;
	using System.ComponentModel.DataAnnotations;
	using System.Linq;

	public class AtLeastOneRequiredAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext context)
		{
			var bookTicketForm = (BookTicketFormModel)context.ObjectInstance;

			if (bookTicketForm.Seats.Any(v => v.Checked))
			{
				return ValidationResult.Success;
			}

			return new ValidationResult(ErrorMessage);
		}
	}
}
