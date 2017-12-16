namespace ETicketSystem.Services.Implementations
{
	using Contracts;
	using Data;
	using Data.Models;
	using System;
	using System.Linq;

	public class ReviewService : IReviewService
	{
		private readonly ETicketSystemDbContext db;

		public ReviewService(ETicketSystemDbContext db)
		{
			this.db = db;
		}

		public bool Add(string companyId, string userId, string description)
		{
			if (!this.db.Tickets.Any(t=>t.UserId == userId && t.Route.CompanyId == companyId))
			{
				return false;
			}

			var review = new Review()
			{
				CompanyId = companyId,
				Description = description,
				UserId = userId,
				PublishDate = DateTime.UtcNow.ToLocalTime()
			};

			this.db.Reviews.Add(review);
			this.db.SaveChanges();

			return true;
		}
	}
}
