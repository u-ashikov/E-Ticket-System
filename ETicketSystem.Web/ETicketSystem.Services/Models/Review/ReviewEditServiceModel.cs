namespace ETicketSystem.Services.Models.Review
{
	using Common.Automapper;
	using Data.Models;

	public class ReviewEditServiceModel : IMapFrom<Review>
    {
		public int Id { get; set; }

		public string CompanyId { get; set; }

		public string Description { get; set; }
    }
}
