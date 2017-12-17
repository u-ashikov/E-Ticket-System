namespace ETicketSystem.Services.Contracts
{
	using Models.Review;
	using System.Collections.Generic;

	public interface IReviewService
    {
		IEnumerable<ReviewInfoServiceModel> All(string companyId, int page = 1, int pageSize = 10);

		bool Add(string companyId, string userId, string description);

		int TotalReviews(string companyId);
	}
}
