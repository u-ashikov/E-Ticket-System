namespace ETicketSystem.Web.Controllers
{
	using Common.Constants;
	using Common.Enums;
	using Data.Models;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using Models.Reviews;
	using Services.Contracts;

	[Authorize]
	public class ReviewsController : BaseController
    {
		private readonly UserManager<User> userManager;

		private readonly IReviewService reviews;

		public ReviewsController(UserManager<User> userManager, IReviewService reviews)
		{
			this.userManager = userManager;
			this.reviews = reviews;
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Add(ReviewFormModel model)
		{
			var userId = this.userManager.GetUserId(User);

			bool success = this.reviews.Add(model.CompanyId, userId, model.Description);

			if (!success)
			{
				this.GenerateAlertMessage(WebConstants.Message.UnableToAddReview, Alert.Warning);

				return RedirectToAction(WebConstants.Action.Details, WebConstants.Controller.Companies, new { id = model.CompanyId });
			}

			this.GenerateAlertMessage(string.Format(WebConstants.Message.EntityCreated,WebConstants.Entity.Review), Alert.Success);

			return RedirectToAction(WebConstants.Action.Details, WebConstants.Controller.Companies, new { id = model.CompanyId });
		}

		[Authorize(Roles = WebConstants.Role.ModeratorRole)]
		public IActionResult Edit(int id)
		{
			var review = this.reviews.GetReviewToEdit(id);

			if (review == null)
			{
				this.GenerateAlertMessage(string.Format(WebConstants.Message.NonExistingEntity,WebConstants.Entity.Review,id), Alert.Warning);
				return RedirectToHome();
			}

			return View(new ReviewFormModel()
			{
				CompanyId = review.CompanyId,
				Description = review.Description
			});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = WebConstants.Role.ModeratorRole)]
		public IActionResult Edit(int id,ReviewFormModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			bool success = this.reviews.Edit(id, model.Description);

			if (!success)
			{
				this.GenerateAlertMessage(string.Format(WebConstants.Message.NonExistingEntity,WebConstants.Entity.Review,id), Alert.Warning);
				return RedirectToHome();
			}

			this.GenerateAlertMessage(WebConstants.Message.ReviewEdited, Alert.Success);

			return RedirectToAction(WebConstants.Action.Details, WebConstants.Controller.Companies, new { id = model.CompanyId });
		}

		[Authorize(Roles = WebConstants.Role.ModeratorRole)]
		public IActionResult Delete(int id, string companyId, bool confirm)
		{
			if (confirm)
			{
				bool success = this.reviews.Delete(id);

				if (!success)
				{
					this.GenerateAlertMessage(string.Format(WebConstants.Message.NonExistingEntity, WebConstants.Entity.Review, id), Alert.Warning);

					return RedirectToHome();
				}
			}

			this.GenerateAlertMessage(WebConstants.Message.ReviewDeleted, Alert.Success);

			return RedirectToAction(WebConstants.Action.Details, WebConstants.Controller.Companies, new { id = companyId });
		}
    }
}
