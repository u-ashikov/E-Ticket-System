namespace ETicketSystem.Test.Web.Controllers
{
	using ETicketSystem.Common.Constants;
	using ETicketSystem.Services.Contracts;
	using ETicketSystem.Web.Controllers;
	using ETicketSystem.Web.Models.Companies;
	using FluentAssertions;
	using Infrastructure;
	using Microsoft.AspNetCore.Mvc;
	using Mock;
	using Mocks;
	using Moq;
	using Services.Models.Company;
	using System.Collections.Generic;
	using System.Linq;
	using Xunit;

	using static Common.TestConstants;

	public class CompaniesControllerTest : BaseControllerTest
	{
		private const int TotalElements = 20;

		private const int MinPage = 1;

		private const int LastPage = 2;

		private const int ReviewsCount = 10;

		private readonly Mock<ICompanyService> companyService = CompanyServiceMock.New;

		private readonly Mock<IReviewService> reviewService = ReviewServiceMock.New;

		private readonly Mock<ITownService> townService = TownServiceMock.New;

		[Fact]
		public void All_WithNoCompaniesShouldReturnCountZero()
		{
			//Arrange
			var emptyCompanyList = new List<CompanyListingServiceModel>();

			this.companyService.Setup(c => c.All(It.IsAny<int>(),It.IsAny<string>(),It.IsAny<int>()))
				.Returns(emptyCompanyList);

			var controller = new CompaniesController(companyService.Object, null, null);

			//Act
			var result = controller.All(string.Empty);

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model;
			model.Should().BeOfType<AllCompanies>();
			model.As<AllCompanies>().Companies.Should().HaveCount(0);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(-1)]
		public void All_WithValidCompaniesAndNoSearchTermWithPageSizeLessOrEqualToZeroShouldRedirectToAll(int page)
		{
			//Arrange
			this.companyService.Setup(c => c.All(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
				.Returns(this.GetAllCompanies());

			var controller = new CompaniesController(companyService.Object, null, null);

			//Act
			var result = controller.All(string.Empty, page);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(CompaniesController.All));
			result.As<RedirectToActionResult>().RouteValues[RouteValueKeySearchTerm].Should().Be(string.Empty);
		}

		[Theory]
		[InlineData(0, "Some company")]
		[InlineData(-1, "Some company")]
		public void All_WithValidCompaniesAndSearchTermWithPageSizeLessOrEqualToZeroShouldRedirectToAll(int page, string searchTerm)
		{
			//Arrange
			this.companyService.Setup(c => c.All(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
				.Returns(this.GetAllCompanies());

			var controller = new CompaniesController(companyService.Object, null, null);

			//Act
			var result = controller.All(searchTerm, page);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(CompaniesController.All));
			result.As<RedirectToActionResult>().RouteValues[RouteValueKeySearchTerm].Should().Be(searchTerm);
			result.As<RedirectToActionResult>().RouteValues[RouteValueKeyPage].Should().Be(null);
		}

		[Fact]
		public void All_WithValidCompaniesAndSearchTermWithGreaterThanTotalPagesPageShouldRedirectToAllAndLastPage()
		{
			//Arrange
			this.companyService.Setup(c => c.All(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
				.Returns(this.GetAllCompanies());

			this.companyService.Setup(c => c.TotalCompanies(It.IsAny<string>()))
				.Returns(this.GetAllCompanies().Count);

			var controller = new CompaniesController(companyService.Object, null, null);

			//Act
			var result = controller.All(string.Empty, 100);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(CompaniesController.All));
			result.As<RedirectToActionResult>()
				.RouteValues[RouteValueKeySearchTerm]
				.Should().Be(string.Empty);
			result.As<RedirectToActionResult>()
				.RouteValues[RouteValueKeyPage]
				.Should().Be(LastPage);
		}

		[Fact]
		public void All_WithValidCompaniesAndGreaterThanTotalPagesPageShouldRedirectToAllAndLastPage()
		{
			const string SearchTerm = "Some company";

			//Arrange
			this.companyService.Setup(c => c.All(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
				.Returns(this.GetAllCompanies());

			this.companyService.Setup(c => c.TotalCompanies(It.IsAny<string>()))
				.Returns(this.GetAllCompanies().Count);

			var controller = new CompaniesController(companyService.Object, null, null);

			//Act
			var result = controller.All(SearchTerm, 100);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(CompaniesController.All));
			result.As<RedirectToActionResult>()
				.RouteValues[RouteValueKeySearchTerm]
				.Should().Be(SearchTerm);
			result.As<RedirectToActionResult>()
				.RouteValues[RouteValueKeyPage]
				.Should().Be(LastPage);
		}

		[Fact]
		public void All_WithValidCompaniesAndPageAndNoSearchTermShouldReturnCountTwenty()
		{
			//Arrange
			this.companyService.Setup(c => c.All(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
				.Returns(this.GetAllCompanies());

			this.companyService.Setup(c => c.TotalCompanies(It.IsAny<string>()))
				.Returns(TotalElements);

			var controller = new CompaniesController(companyService.Object, null, null);

			//Act
			var result = controller.All(string.Empty, MinPage);

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model;
			model.Should().BeOfType<AllCompanies>();
			model.As<AllCompanies>().Companies.Should().HaveCount(TotalElements);
		}

		[Fact]
		public void All_WithValidCompaniesAndPageAndNoSearchTermShouldReturnOrderedCompaniesByName()
		{
			//Arrange
			this.companyService.Setup(c => c.All(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
				.Returns(this.GetAllCompanies());

			this.companyService.Setup(c => c.TotalCompanies(It.IsAny<string>()))
				.Returns(TotalElements);

			var controller = new CompaniesController(companyService.Object, null, null);

			//Act
			var result = controller.All(string.Empty, MinPage);

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model;
			model.Should().BeOfType<AllCompanies>();
			this.AssertCompaniesEqualityAndOrder(this.GetAllCompanies(), model.As<AllCompanies>().Companies.ToList());
		}

		[Theory]
		[InlineData("Some company")]
		[InlineData(null)]
		[InlineData("   ")]
		[InlineData("")]
		public void All_WithValidCompaniesAndPageAndInvalidSearchTermShouldReturnNoResults(string searchTerm)
		{
			//Arrange
			this.companyService.Setup(c => c.All(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
				.Returns(new List<CompanyListingServiceModel>());

			this.companyService.Setup(c => c.TotalCompanies(It.IsAny<string>()))
				.Returns(0);

			var controller = new CompaniesController(companyService.Object, null, null);

			//Act
			var result = controller.All(searchTerm, MinPage);

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model;
			model.Should().BeOfType<AllCompanies>();
			model.As<AllCompanies>().Pagination.TotalElements.Should().Be(0);
			model.As<AllCompanies>().Pagination.TotalPages.Should().Be(0);
			model.As<AllCompanies>().Pagination.SearchTerm.Should().Be(searchTerm);
			model.As<AllCompanies>().Companies.Should().HaveCount(0);
		}

		[Fact]
		public void All_WithValidCompaniesAndPageAndSearchTermShouldReturnFoundResults()
		{
			const string SearchedCompanyName = "Company 9";

			const string CompanyDescription = "Description";

			const string CompanyId = "9";

			//Arrange
			this.companyService.Setup(c => c.All(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
				.Returns(new List<CompanyListingServiceModel>()
				{
					new CompanyListingServiceModel()
					{
						Description = CompanyDescription,
						Id = CompanyId,
						Name = SearchedCompanyName
					}
				});

			this.companyService.Setup(c => c.TotalCompanies(It.IsAny<string>()))
				.Returns(1);

			var controller = new CompaniesController(companyService.Object, null, null);

			//Act
			var result = controller.All(SearchedCompanyName, MinPage);

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model;
			model.Should().BeOfType<AllCompanies>();
			model.As<AllCompanies>().Pagination.TotalElements.Should().Be(1);
			model.As<AllCompanies>().Pagination.TotalPages.Should().Be(MinPage);
			model.As<AllCompanies>().Pagination.SearchTerm.Should().Be(SearchedCompanyName);
			model.As<AllCompanies>().Companies.First().Should().BeOfType<CompanyListingServiceModel>();
			model.As<AllCompanies>().Companies.Should().HaveCount(1);
			model.As<AllCompanies>().Companies.First().Name.Should().Be(SearchedCompanyName);
		}

		[Fact]
		public void Details_WithNonExistingCompanyShouldRedirectToAllCompaniesWithAlertMessage()
		{
			//Arrange
			CompanyDetailsServiceModel company = null;

			this.companyService.Setup(c => c.CompanyDetails(It.IsAny<string>()))
				.Returns(company);

			var controller = new CompaniesController(companyService.Object, null, null);

			this.PrepareTempData();

			controller.TempData = tempData.Object;

			//Act
			var result = controller.Details(CompanyId);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			this.customMessage.Should().Be(string.Format(WebConstants.Message.NonExistingEntity, nameof(WebConstants.Entity.Company), CompanyId));
			(result as RedirectToActionResult).ActionName.Should().Be(nameof(CompaniesController.All));
		}

		[Theory]
		[InlineData(0)]
		[InlineData(-1)]
		public void Details_WithExistingCompanyAndNegativeReviewsPageShouldRedirectToCompanyDetails(int page)
		{
			//Arrange
			CompanyDetailsServiceModel company = new CompanyDetailsServiceModel();

			this.companyService.Setup(c => c.CompanyDetails(It.IsAny<string>()))
				.Returns(company);

			var controller = new CompaniesController(companyService.Object, null, null);

			//Act
			var result = controller.Details(CompanyId, page);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			(result as RedirectToActionResult).ActionName.Should().Be(nameof(CompaniesController.Details));
			(result as RedirectToActionResult).RouteValues[RouteValueKeyId].Should().Be(CompanyId);
			(result as RedirectToActionResult).RouteValues[RouteValueKeyPage].Should().Be(MinPageSize);
		}

		[Fact]
		public void Details_WithExistingCompanyAndGreaterThanTotalReviewsPagesCountShouldRedirectToCompanyDetails()
		{
			const int Page = 7;

			//Arrange
			CompanyDetailsServiceModel company = new CompanyDetailsServiceModel();

			this.reviewService.Setup(r => r.TotalReviews(It.IsAny<string>()))
				.Returns(ReviewsCount);

			this.companyService.Setup(c => c.CompanyDetails(It.IsAny<string>()))
				.Returns(company);

			var controller = new CompaniesController(companyService.Object, null, reviewService.Object);

			//Act
			var result = controller.Details(CompanyId, Page);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			(result as RedirectToActionResult).ActionName.Should().Be(nameof(CompaniesController.Details));
			(result as RedirectToActionResult).RouteValues[RouteValueKeyId].Should().Be(CompanyId);
			(result as RedirectToActionResult).RouteValues[RouteValueKeyPage].Should().Be(LastPage);
		}

		[Fact]
		public void Details_WithExistingCompanyShouldReturnView()
		{
			//Arrange
			CompanyDetailsServiceModel company = this.GetCompanyDetailsServiceModel();

			reviewService.Setup(r => r.TotalReviews(It.IsAny<string>()))
				.Returns(ReviewsCount);

			companyService.Setup(c => c.CompanyDetails(It.IsAny<string>()))
				.Returns(company);

			var controller = new CompaniesController(companyService.Object, townService.Object, reviewService.Object);

			//Act
			var result = controller.Details(CompanyId, MinPageSize);

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model;

			model.Should().BeOfType<CompanyDetails>();

			model.As<CompanyDetails>().SearchForm.CompanyId.Should().Be(CompanyId);
			model.As<CompanyDetails>().ReviewForm.CompanyId.Should().Be(CompanyId);
			model.As<CompanyDetails>().Reviews.Pagination.CurrentPage.Should().Be(MinPageSize);
			model.As<CompanyDetails>().Reviews.Pagination.NextPage.Should().Be(LastPage);
			model.As<CompanyDetails>().Reviews.Pagination.TotalPages.Should().Be(LastPage);
			model.As<CompanyDetails>().Reviews.Pagination.TotalElements.Should().Be(ReviewsCount);

			this.AssertCompayDetailsViewModel(this.GetCompanyDetailsServiceModel(), model.As<CompanyDetails>());
		}

		[Fact]
		public void Details_WithExistingCompanyAndLastReviewsPageShouldReturnView()
		{
			//Arrange
			CompanyDetailsServiceModel company = this.GetCompanyDetailsServiceModel();

			reviewService.Setup(r => r.TotalReviews(It.IsAny<string>()))
				.Returns(ReviewsCount);

			companyService.Setup(c => c.CompanyDetails(It.IsAny<string>()))
				.Returns(company);

			var controller = new CompaniesController(companyService.Object, townService.Object, reviewService.Object);

			//Act
			var result = controller.Details(CompanyId, LastPage);

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model;

			model.Should().BeOfType<CompanyDetails>();

			model.As<CompanyDetails>().SearchForm.CompanyId.Should().Be(CompanyId);
			model.As<CompanyDetails>().ReviewForm.CompanyId.Should().Be(CompanyId);
			model.As<CompanyDetails>().Reviews.Pagination.CurrentPage.Should().Be(LastPage);
			model.As<CompanyDetails>().Reviews.Pagination.NextPage.Should().Be(LastPage);
			model.As<CompanyDetails>().Reviews.Pagination.TotalPages.Should().Be(LastPage);
			model.As<CompanyDetails>().Reviews.Pagination.TotalElements.Should().Be(ReviewsCount);

			this.AssertCompayDetailsViewModel(this.GetCompanyDetailsServiceModel(), model.As<CompanyDetails>());
		}

		private IList<CompanyListingServiceModel> GetAllCompanies()
		{
			var companies = new List<CompanyListingServiceModel>();

			for (int i = 0; i < 20; i++)
			{
				companies.Add(new CompanyListingServiceModel()
				{
					Description = $"Description {i}",
					Id = i.ToString(),
					Logo = null,
					Name = $"Company {i}"
				});
			}

			return companies;
		}

		private void AssertCompayDetailsViewModel(CompanyDetailsServiceModel companyDetailsServiceModel, CompanyDetails companyDetails)
		{
			companyDetailsServiceModel.Chief.Should().Be(companyDetails.Company.Chief);
			companyDetailsServiceModel.Description.Should().Be(companyDetails.Company.Description);
			companyDetailsServiceModel.Name.Should().Be(companyDetails.Company.Name);
			companyDetailsServiceModel.PhoneNumber.Should().Be(companyDetails.Company.PhoneNumber);
			companyDetailsServiceModel.TicketsSold.Should().Be(companyDetails.Company.TicketsSold);
			companyDetailsServiceModel.Town.Should().Be(companyDetails.Company.Town);
		}

		private CompanyDetailsServiceModel GetCompanyDetailsServiceModel()
		{
			return new CompanyDetailsServiceModel()
			{
				Chief = "Ivan Ivanov",
				Description = "Some description",
				Logo = null,
				Name = "Aguila",
				PhoneNumber = "0898914141",
				TicketsSold = 100,
				Town = "Sofia"
			};
		}

		private void AssertCompaniesEqualityAndOrder(IList<CompanyListingServiceModel> list, IList<CompanyListingServiceModel> allCompanies)
		{
			for (int i = 0; i < list.Count; i++)
			{
				list[i].Name.Should().Be(allCompanies[i].Name);
			}
		}
	}
}
