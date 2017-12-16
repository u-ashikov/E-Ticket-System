namespace ETicketSystem.Web.Models.Reviews
{
	using Common.Constants;
	using System.ComponentModel.DataAnnotations;

	public class AddReviewFormModel
    {
		public string CompanyId { get; set; }

		[Required]
		[MinLength(DataConstants.Review.DescriptionMinLength)]
		[MaxLength(DataConstants.Review.DescriptionMaxLength)]
		public string Description { get; set; }
    }
}
