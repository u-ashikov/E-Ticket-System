namespace ETicketSystem.Test.Web.Controllers
{
	using ETicketSystem.Common.Constants;
	using ETicketSystem.Common.Enums;
	using ETicketSystem.Services.Contracts;
	using ETicketSystem.Services.Models.Review;
	using ETicketSystem.Web.Controllers;
	using ETicketSystem.Web.Models.Reviews;
	using Fixtures;
	using FluentAssertions;
	using Infrastructure;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using Mock;
	using Mocks;
	using Moq;
	using System.Linq;
	using Xunit;

	using static Common.TestConstants;

	public class ReviewsControllerTest : BaseControllerTest, IClassFixture<UserManagerGetUserIdFixture>
	{
		private const string ReviewDescription = "Some test description just testing";

		private const string ReviewAuthor = "TestUserId";

		private const int ReviewId = 10;

		private readonly UserManagerGetUserIdFixture fixture;

		private readonly Mock<IReviewService> reviewService = ReviewServiceMock.New;

		private readonly Mock<ICompanyService> companyService = CompanyServiceMock.New;

		public ReviewsControllerTest(UserManagerGetUserIdFixture fixture)
		{
			this.fixture = fixture;
		}

		[Fact]
		public void ControllerShouldBeForAuthorizedUsers()
		{
			//Arrange
			var controller = new ReviewsController(null,null,null);

			//Act
			var attributes = controller.GetType().GetCustomAttributes(true);

			//Assert
			attributes.Any(a => a.GetType() == typeof(AuthorizeAttribute));
		}

		[Fact]
		public void Post_ShouldReturnViewForInvalidModelState()
		{
			//Arrange
			var controller = new ReviewsController(null,null,null);
			controller.ModelState.AddModelError(string.Empty, "Error");

			//Act
			var result = controller.Add(new ReviewFormModel());

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model;
			model.Should().BeOfType<ReviewFormModel>();
		}

		[Fact]
		public void Post_ShouldRedirectToHomeWhenCompanyDoesNotExist()
		{
			//Arrange
			var controller = new ReviewsController(this.fixture.UserManagerMockInstance.Object, null, this.companyService.Object);

			this.companyService.Setup(c => c.Exists(It.IsAny<string>()))
				.Returns(false);

			this.PrepareTempData();

			controller.TempData = this.tempData.Object;

			//Act
			var form = this.GetReviewForm();
			var result = controller.Add(form);

			//Assert
			this.AssertRedirectToHome(result);
			this.customMessage.Should().Be(WebConstants.Message.InvalidCompany);
		}

		[Fact]
		public void Post_ShouldRedirectToCompanyDetailsWhenCompanyOwnerTryToAddReviewToHisCompany()
		{
			//Arrange
			var controller = new ReviewsController(this.fixture.UserManagerMockInstance.Object, null,this.companyService.Object);

			this.companyService.Setup(c => c.Exists(It.IsAny<string>()))
				.Returns(true);

			this.PrepareTempData();

			controller.TempData = this.tempData.Object;

			//Act
			var form = this.GetOwnerFilledReviewForm();
			var result = controller.Add(form);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			var model = result.As<RedirectToActionResult>();
			model.ActionName.Should().Be(WebConstants.Action.Details);
			model.ControllerName.Should().Be(WebConstants.Controller.Companies);
			model.RouteValues.Keys.Should().Contain(RouteValueKeyId);
			model.RouteValues.Values.Should().Contain(form.CompanyId);
			this.customMessage.Should().Be(WebConstants.Message.OwnerAddReview);
		}

		[Fact]
		public void Post_ShouldRedirectToCompanyDetailsWhenReviewAuthorHaventUsedCompanyServicesYet()
		{
			//Arrange
			var controller = new ReviewsController(this.fixture.UserManagerMockInstance.Object, this.reviewService.Object, this.companyService.Object);

			this.reviewService.Setup(r => r.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns(false);

			this.companyService.Setup(c => c.Exists(It.IsAny<string>()))
				.Returns(true);

			this.PrepareTempData();

			controller.TempData = this.tempData.Object;

			//Act
			var form = this.GetReviewForm();
			var result = controller.Add(form);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			var model = result.As<RedirectToActionResult>();
			model.ActionName.Should().Be(WebConstants.Action.Details);
			model.ControllerName.Should().Be(WebConstants.Controller.Companies);
			model.RouteValues.Keys.Should().Contain(RouteValueKeyId);
			model.RouteValues.Values.Should().Contain(form.CompanyId);
			this.customMessage.Should().Be(WebConstants.Message.UnableToAddReview);
		}

		[Fact]
		public void Post_ShouldRedirectToCompanyDetailsAndCreateReviewWithValidData()
		{
			//Arrange
			var controller = new ReviewsController(this.fixture.UserManagerMockInstance.Object, this.reviewService.Object, this.companyService.Object);

			this.reviewService.Setup(r => r.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.Returns(true);

			this.companyService.Setup(c => c.Exists(It.IsAny<string>()))
				.Returns(true);

			this.PrepareTempData();

			controller.TempData = this.tempData.Object;

			//Act
			var form = this.GetReviewForm();
			var result = controller.Add(form);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			var model = result.As<RedirectToActionResult>();
			model.ActionName.Should().Be(WebConstants.Action.Details);
			model.ControllerName.Should().Be(WebConstants.Controller.Companies);
			model.RouteValues.Keys.Should().Contain(RouteValueKeyId);
			model.RouteValues.Values.Should().Contain(form.CompanyId);
			this.customMessage.Should().Be(string.Format(WebConstants.Message.EntityCreated, WebConstants.Entity.Review));
		}

		[Fact]
		public void Get_EditReviewShouldBeOnlyForModerators()
		{
			//Arrange
			var controller = new ReviewsController(null, null, null);
			var action = controller.GetType().GetMethods().FirstOrDefault(a=>a.Name == WebConstants.Action.Edit);

			//Act
			var attributes = action.GetCustomAttributes(true);

			//Assert
			var authorizeAttribute = attributes.FirstOrDefault(a => a.GetType() == typeof(AuthorizeAttribute)).As<AuthorizeAttribute>();
			authorizeAttribute.Roles.Should().Be(Role.Moderator.ToString());
		}

		[Fact]
		public void Get_EditReviewShouldRedirectToHomeWhenReviewDoesNotExist()
		{
			//Arrange
			ReviewEditServiceModel reviewToEdit = null;
			var controller = new ReviewsController(null, this.reviewService.Object, null);

			this.reviewService.Setup(r => r.GetReviewToEdit(It.IsAny<int>()))
				.Returns(reviewToEdit);

			this.PrepareTempData();

			controller.TempData = this.tempData.Object;

			//Act
			var result = controller.Edit(ReviewId);

			//Assert
			this.AssertRedirectToHome(result);
			this.customMessage.Should().Be(string.Format(WebConstants.Message.NonExistingEntity, WebConstants.Entity.Review, ReviewId));
		}

		[Fact]
		public void Get_EditReviewShouldReturnValidViewWithModelToEdit()
		{
			//Arrange
			var controller = new ReviewsController(null, this.reviewService.Object, null);
			this.reviewService.Setup(r => r.GetReviewToEdit(It.IsAny<int>()))
				.Returns(this.GetReviewToEdit());

			//Act
			var result = controller.Edit(ReviewId);

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model;
			model.As<ReviewFormModel>().CompanyId.Should().Be(CompanyId);
			model.As<ReviewFormModel>().Description.Should().Be(ReviewDescription);
		}

		[Fact]
		public void Post_EditReviewShouldBeOnlyForModerators()
		{
			//Arrange
			var controller = new ReviewsController(null, null, null);
			var action = controller
				.GetType()
				.GetMethods()
				.FirstOrDefault(a => a.Name == WebConstants.Action.Edit 
				&& a.GetCustomAttributes(true).Any(attr=>attr.GetType() == typeof(HttpPostAttribute)));

			//Act
			var attributes = action.GetCustomAttributes(true);

			//Assert
			var authorizeAttribute = attributes.FirstOrDefault(a => a.GetType() == typeof(AuthorizeAttribute)).As<AuthorizeAttribute>();
			authorizeAttribute.Roles.Should().Be(Role.Moderator.ToString());
		}

		[Fact]
		public void Post_EditReviewShouldReturnViewForInvalidModelState()
		{
			//Arrange
			var controller = new ReviewsController(null, this.reviewService.Object, null);
			controller.ModelState.AddModelError(string.Empty, "Error");

			//Act
			var form = this.GetReviewForm();
			var result = controller.Edit(ReviewId, form);

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model;
			model.As<ReviewFormModel>().CompanyId.Should().Be(form.CompanyId);
			model.As<ReviewFormModel>().Description.Should().Be(form.Description);
		}

		[Fact]
		public void Post_EditReviewShouldRedirectToHomeWhenReviewDoesNotExist()
		{
			//Arrange
			var controller = new ReviewsController(null, this.reviewService.Object, null);

			this.reviewService.Setup(r => r.Edit(It.IsAny<int>(), It.IsAny<string>()))
				.Returns(false);

			this.PrepareTempData();

			controller.TempData = this.tempData.Object;

			//Act
			var form = this.GetReviewForm();
			var result = controller.Edit(ReviewId, form);

			//Assert
			this.AssertRedirectToHome(result);
			this.customMessage.Should().Be(string.Format(WebConstants.Message.NonExistingEntity, WebConstants.Entity.Review, ReviewId));
		}

		[Fact]
		public void Post_EditReviewShouldRedirectToCompanyDetailsWithValidData()
		{
			//Arrange
			var controller = new ReviewsController(null, this.reviewService.Object, null);

			this.reviewService.Setup(r => r.Edit(It.IsAny<int>(), It.IsAny<string>()))
				.Returns(true);

			this.PrepareTempData();

			controller.TempData = this.tempData.Object;

			//Act
			var form = this.GetReviewForm();
			var result = controller.Edit(ReviewId, form);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			var model = result.As<RedirectToActionResult>();
			model.ActionName.Should().Be(WebConstants.Action.Details);
			model.ControllerName.Should().Be(WebConstants.Controller.Companies);
			model.RouteValues.Keys.Should().Contain(RouteValueKeyId);
			model.RouteValues.Values.Should().Contain(form.CompanyId);
			this.customMessage.Should().Be(string.Format(WebConstants.Message.EntityEdited, WebConstants.Entity.Review));
		}

		[Fact]
		public void Post_DeleteReviewShouldBeOnlyForModerators()
		{
			//Arrange
			var controller = new ReviewsController(null, null, null);
			var action = controller
				.GetType()
				.GetMethods()
				.FirstOrDefault(a => a.Name == WebConstants.Action.Delete);

			//Act
			var attributes = action.GetCustomAttributes(true);

			//Assert
			var authorizeAttribute = attributes.FirstOrDefault(a => a.GetType() == typeof(AuthorizeAttribute)).As<AuthorizeAttribute>();
			authorizeAttribute.Roles.Should().Be(Role.Moderator.ToString());
		}

		[Fact]
		public void Post_DeleteReviewShouldRedirectToCompanyDetailsWhenNotConfirmed()
		{
			//Arrange
			var controller = new ReviewsController(null, this.reviewService.Object, null);

			//Act
			var result = controller.Delete(ReviewId,CompanyId,false);

			//Assert
			this.AssertRedirectToCompanyDetails(result);
		}

		[Fact]
		public void Post_DeleteReviewShouldRedirectToHomeForNonExistingReview()
		{
			//Arrange
			var controller = new ReviewsController(null, this.reviewService.Object, null);

			this.reviewService.Setup(r => r.Delete(It.IsAny<int>()))
				.Returns(false);

			this.PrepareTempData();

			controller.TempData = this.tempData.Object;

			//Act
			var result = controller.Delete(ReviewId, CompanyId, true);

			//Assert
			this.AssertRedirectToHome(result);
			this.customMessage.Should().Be(string.Format(WebConstants.Message.NonExistingEntity, WebConstants.Entity.Review, ReviewId));
		}

		[Fact]
		public void Post_DeleteReviewShouldRedirectToCompanyDetailsWithValidData()
		{
			//Arrange
			var controller = new ReviewsController(null, this.reviewService.Object, null);

			this.reviewService.Setup(r => r.Delete(It.IsAny<int>()))
				.Returns(true);

			this.PrepareTempData();

			controller.TempData = this.tempData.Object;

			//Act
			var result = controller.Delete(ReviewId, CompanyId, true);

			//Assert
			this.AssertRedirectToCompanyDetails(result);
		}

		private void AssertRedirectToCompanyDetails(IActionResult result)
		{
			result.Should().BeOfType<RedirectToActionResult>();
			var model = result.As<RedirectToActionResult>();
			model.ActionName.Should().Be(WebConstants.Action.Details);
			model.ControllerName.Should().Be(WebConstants.Controller.Companies);
		}

		private ReviewFormModel GetOwnerFilledReviewForm()
		{
			return new ReviewFormModel()
			{
				CompanyId = UserId,
				Description = ReviewDescription
			};
		}

		private ReviewFormModel GetReviewForm()
		{
			return new ReviewFormModel()
			{
				CompanyId = ReviewAuthor,
				Description = ReviewDescription
			};
		}

		private ReviewEditServiceModel GetReviewToEdit()
		{
			return new ReviewEditServiceModel()
			{
				Id = ReviewId,
				CompanyId = CompanyId,
				Description = ReviewDescription
			};
		}
	}
}
