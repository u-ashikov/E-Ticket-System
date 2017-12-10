namespace ETicketSystem.Web.Infrastructure.Attributes.Validation
{
	using ETicketSystem.Web.Models.Routes;
	using System.ComponentModel.DataAnnotations;
	using System.Linq;

	public class AtLeastOneRequiredAttribute : ValidationAttribute
	{
		protected override ValidationResult IsValid(object value, ValidationContext context)
		{
			var vm = (BookTicketFormModel)context.ObjectInstance;

			if (vm.Seats.Any(v => v.Checked))
			{
				return ValidationResult.Success;
			}

			return new ValidationResult(ErrorMessage);
		}
	}
}
