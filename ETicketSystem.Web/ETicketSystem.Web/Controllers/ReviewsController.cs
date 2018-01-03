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

		private readonly ICompanyService companies;

		public ReviewsController(UserManager<User> userManager, IReviewService reviews, ICompanyService companies)
		{
			this.userManager = userManager;
			this.reviews = reviews;
			this.companies = companies;
		}

		[HttpPost]
		public IActionResult Add(ReviewFormModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			if (!this.companies.Exists(model.CompanyId))
			{
				this.GenerateAlertMessage(WebConstants.Message.InvalidCompany, Alert.Danger);

				return RedirectToAction(WebConstants.Action.Index,WebConstants.Controller.Home);
			}

			var userId = this.userManager.GetUserId(User);

			if (model.CompanyId == userId)
			{
				this.GenerateAlertMessage(WebConstants.Message.OwnerAddReview, Alert.Warning);

				return RedirectToAction(WebConstants.Action.Details, WebConstants.Controller.Companies, new { id = model.CompanyId });
			}

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

			this.GenerateAlertMessage(string.Format(WebConstants.Message.EntityEdited, WebConstants.Entity.Review), Alert.Success);

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

				this.GenerateAlertMessage(string.Format(WebConstants.Message.EntityDeleted, WebConstants.Entity.Review), Alert.Success);
			}

			return RedirectToAction(WebConstants.Action.Details, WebConstants.Controller.Companies, new { id = companyId });
		}
    }
}
