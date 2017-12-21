namespace ETicketSystem.Services.Implementations
{
	using AutoMapper.QueryableExtensions;
	using Contracts;
	using Data;
	using Data.Models;
	using Models.Review;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class ReviewService : IReviewService
	{
		private readonly ETicketSystemDbContext db;

		public ReviewService(ETicketSystemDbContext db)
		{
			this.db = db;
		}

		public IEnumerable<ReviewInfoServiceModel> All(string companyId, int page = 1, int pageSize = 10) =>
			this.db
				.Reviews
				.Where(r => r.CompanyId == companyId && !r.IsDeleted)
				.OrderByDescending(r=>r.PublishDate)
				.Skip((page-1)*pageSize)
				.Take(pageSize)
				.ProjectTo<ReviewInfoServiceModel>()
				.ToList();

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

		public bool Edit(int id, string description)
		{
			var reviewToEdit = this.db.Reviews.Find(id);

			if (reviewToEdit == null)
			{
				return false;
			}

			reviewToEdit.Description = description;
			this.db.SaveChanges();

			return true;
		}

		public bool Delete(int id)
		{
			var reviewToDelete = this.db.Reviews.Find(id);

			if (reviewToDelete == null || reviewToDelete.IsDeleted)
			{
				return false;
			}

			reviewToDelete.IsDeleted = true;
			this.db.SaveChanges();

			return true;
		}

		public ReviewEditServiceModel GetReviewToEdit(int id) =>
			this.db
				.Reviews
				.Where(r => r.Id == id)
				.ProjectTo<ReviewEditServiceModel>()
				.FirstOrDefault();

		public int TotalReviews(string companyId) => this.db.Reviews.Count(r => r.CompanyId == companyId);
	}
}
