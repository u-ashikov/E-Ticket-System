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
		public IActionResult Add(AddReviewFormModel model)
		{
			var userId = this.userManager.GetUserId(User);

			bool success = this.reviews.Add(model.CompanyId, userId, model.Description);

			if (!success)
			{
				this.GenerateAlertMessage(WebConstants.Message.UnableToAddReview, Alert.Warning);
				return Redirect($"/companies/details/{model.CompanyId}");
			}

			this.GenerateAlertMessage(WebConstants.Message.ReviewAdded, Alert.Success);

			return RedirectToAction(nameof(CompaniesController.Details), "Details",new { id = model.CompanyId});
		}
    }
}
