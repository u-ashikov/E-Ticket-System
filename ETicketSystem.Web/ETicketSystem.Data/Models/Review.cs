namespace ETicketSystem.Data.Models
{
	using Common.Constants;
	using System;
	using System.ComponentModel.DataAnnotations;

	public class Review
    {
		public int Id { get; set; }

		[Required]
		[MinLength(DataConstants.Review.DescriptionMinLength)]
		[MaxLength(DataConstants.Review.DescriptionMaxLength)]
		public string Description { get; set; }

		public string UserId { get; set; }

		public RegularUser User { get; set; }

		public string CompanyId { get; set; }

		public Company Company { get; set; }

		public DateTime PublishDate { get; set; }

		public bool IsDeleted { get; set; }
	}
}
