namespace ETicketSystem.Web.Models.Reviews
{
	using Common.Constants;
	using System.ComponentModel.DataAnnotations;

	public class ReviewFormModel
    {
		public string CompanyId { get; set; }

		[Required]
		[StringLength(DataConstants.Review.DescriptionMaxLength,MinimumLength = DataConstants.Review.DescriptionMinLength,ErrorMessage = WebConstants.Message.CompanyReviewDescriptionLength)]
		public string Description { get; set; }
    }
}
