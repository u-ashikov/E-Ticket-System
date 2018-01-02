namespace ETicketSystem.Test.Web.Controllers
{
	using ETicketSystem.Common.Constants;
	using ETicketSystem.Web.Controllers;
	using ETicketSystem.Web.Models.Companies;
	using FluentAssertions;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.ViewFeatures;
	using Mock;
	using Mocks;
	using Moq;
	using Services.Models.Company;
	using System.Collections.Generic;
	using System.Linq;
	using Xunit;

	public class CompaniesControllerTest
	{
		private const int Page = 1;

		private const string CompanyId = "SomeId";

		private const string RouteValueKeyId = "id";

		private const string RouteValueKeyPage = "page";

		private const string RouteValueKeySearchTerm = "searchTerm";

		[Fact]
		public void All_WithNoCompaniesShouldReturnCountZero()
		{
			//Arrange
			var companyService = CompanyServiceMock.New;
			var emptyCompanyList = new List<CompanyListingServiceModel>();

			companyService.Setup(c => c.All(It.IsAny<int>(),It.IsAny<string>(),It.IsAny<int>()))
				.Returns(emptyCompanyList);

			var controller = new CompaniesController(companyService.Object, null, null);

			//Act
			var result = controller.All(string.Empty);

			//Assert
			result.Should().BeOfType<ViewResult>();
			object model = result.As<ViewResult>().Model;
			model.Should().BeOfType<AllCompanies>();
			model.As<AllCompanies>().Companies.Should().HaveCount(0);
		}

		[Fact]
		public void All_WithValidCompaniesAndNegativePageShouldRedirectToAll()
		{
			//Arrange
			var companyService = CompanyServiceMock.New;

			companyService.Setup(c => c.All(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
				.Returns(this.GetAllCompanies());

			var controller = new CompaniesController(companyService.Object, null, null);

			//Act
			var result = controller.All(string.Empty, -1);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(CompaniesController.All));
			result.As<RedirectToActionResult>().RouteValues[RouteValueKeySearchTerm].Should().Be(string.Empty);
		}

		[Fact]
		public void All_WithValidCompaniesAndGreaterThanTotalPagesPageShouldRedirectToAllAndLastPage()
		{
			//Arrange
			var companyService = CompanyServiceMock.New;

			companyService.Setup(c => c.All(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
				.Returns(this.GetAllCompanies());

			companyService.Setup(c => c.TotalCompanies(It.IsAny<string>()))
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
				.Should().Be(1);
		}

		[Fact]
		public void All_WithValidCompaniesAndPageAndNoSearchTermShouldReturnCount10()
		{
			//Arrange
			var companyService = CompanyServiceMock.New;

			companyService.Setup(c => c.All(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
				.Returns(this.GetAllCompanies());

			companyService.Setup(c => c.TotalCompanies(It.IsAny<string>()))
				.Returns(10);

			var controller = new CompaniesController(companyService.Object, null, null);

			//Act
			var result = controller.All(string.Empty, 1);

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model;
			model.Should().BeOfType<AllCompanies>();
			model.As<AllCompanies>().Companies.Should().HaveCount(10);
		}

		[Fact]
		public void All_WithValidCompaniesAndPageAndNoSearchTermShouldReturnOrderedCompaniesByName()
		{
			//Arrange
			var companyService = CompanyServiceMock.New;

			companyService.Setup(c => c.All(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
				.Returns(this.GetAllCompanies());

			companyService.Setup(c => c.TotalCompanies(It.IsAny<string>()))
				.Returns(10);

			var controller = new CompaniesController(companyService.Object, null, null);

			//Act
			var result = controller.All(string.Empty, 1);

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model;
			model.Should().BeOfType<AllCompanies>();
			this.AssertCompaniesEqualityAndOrder(this.GetAllCompanies(), model.As<AllCompanies>().Companies.ToList());
		}

		[Fact]
		public void All_WithValidCompaniesAndPageAndSearchTermShouldReturnFoundResults()
		{
			const string SearchedCompanyName = "Company 9";

			const string CompanyDescription = "Description";

			const string CompanyId = "9";

			//Arrange
			var companyService = CompanyServiceMock.New;

			companyService.Setup(c => c.All(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
				.Returns(new List<CompanyListingServiceModel>()
				{
					new CompanyListingServiceModel()
					{
						Description = CompanyDescription,
						Id = CompanyId,
						Name = SearchedCompanyName
					}
				});

			companyService.Setup(c => c.TotalCompanies(It.IsAny<string>()))
				.Returns(1);

			var controller = new CompaniesController(companyService.Object, null, null);

			//Act
			var result = controller.All(SearchedCompanyName, 1);

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model;
			model.Should().BeOfType<AllCompanies>();
			model.As<AllCompanies>().Pagination.TotalElements.Should().Be(1);
			model.As<AllCompanies>().Pagination.TotalPages.Should().Be(1);
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
			var companyService = CompanyServiceMock.New;

			companyService.Setup(c => c.CompanyDetails(It.IsAny<string>()))
				.Returns(company);

			var controller = new CompaniesController(companyService.Object, null, null);
			var tempData = new Mock<ITempDataDictionary>();

			string errorMessage = null;

			tempData
				.SetupSet(t => t[WebConstants.TempDataKey.Message] = It.IsAny<string>())
				.Callback((string key, object msg) => errorMessage = msg as string);

			controller.TempData = tempData.Object;

			//Act
			var result = controller.Details(CompanyId);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			errorMessage.Should().Be(string.Format(WebConstants.Message.NonExistingEntity, nameof(WebConstants.Entity.Company), CompanyId));
			(result as RedirectToActionResult).ActionName.Should().Be(nameof(CompaniesController.All));
		}

		[Fact]
		public void Details_WithExistingCompanyAndNegativeReviewsPageShouldRedirectToCompanyDetails()
		{
			//Arrange
			CompanyDetailsServiceModel company = new CompanyDetailsServiceModel();
			var companyService = CompanyServiceMock.New;

			companyService.Setup(c => c.CompanyDetails(It.IsAny<string>()))
				.Returns(company);

			var controller = new CompaniesController(companyService.Object, null, null);

			//Act
			var result = controller.Details(CompanyId, -1);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			(result as RedirectToActionResult).ActionName.Should().Be(nameof(CompaniesController.Details));
			(result as RedirectToActionResult).RouteValues[RouteValueKeyId].Should().Be(CompanyId);
			(result as RedirectToActionResult).RouteValues[RouteValueKeyPage].Should().Be(Page);
		}

		[Fact]
		public void Details_WithExistingCompanyAndGreaterThanTotalReviewsPagesCountShouldRedirectToCompanyDetails()
		{
			//Arrange
			CompanyDetailsServiceModel company = new CompanyDetailsServiceModel();
			var companyService = CompanyServiceMock.New;
			var reviewService = ReviewServiceMock.New;

			reviewService.Setup(r => r.TotalReviews(It.IsAny<string>()))
				.Returns(4);

			companyService.Setup(c => c.CompanyDetails(It.IsAny<string>()))
				.Returns(company);

			var controller = new CompaniesController(companyService.Object, null, reviewService.Object);

			//Act
			var result = controller.Details(CompanyId, 7);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			(result as RedirectToActionResult).ActionName.Should().Be(nameof(CompaniesController.Details));
			(result as RedirectToActionResult).RouteValues[RouteValueKeyId].Should().Be(CompanyId);
			(result as RedirectToActionResult).RouteValues[RouteValueKeyPage].Should().Be(1);
		}

		[Fact]
		public void Details_WithExistingCompanyShouldReturnView()
		{
			//Arrange
			CompanyDetailsServiceModel company = this.GetCompanyDetailsServiceModel();
			var companyService = CompanyServiceMock.New;
			var reviewService = ReviewServiceMock.New;
			var townService = TownServiceMock.New;

			reviewService.Setup(r => r.TotalReviews(It.IsAny<string>()))
				.Returns(4);

			companyService.Setup(c => c.CompanyDetails(It.IsAny<string>()))
				.Returns(company);

			var controller = new CompaniesController(companyService.Object, townService.Object, reviewService.Object);

			//Act
			var result = controller.Details(CompanyId, 1);

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model;

			model.Should().BeOfType<CompanyDetails>();

			this.AssertCompayDetailsViewModel(this.GetCompanyDetailsServiceModel(), model.As<CompanyDetails>());
		}

		private IList<CompanyListingServiceModel> GetAllCompanies()
		{
			var companies = new List<CompanyListingServiceModel>();

			for (int i = 0; i < 10; i++)
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
			list = list.OrderBy(c => c.Name).ToList();

			for (int i = 0; i < list.Count; i++)
			{
				list[i].Name.Should().Be(allCompanies[i].Name);
			}
		}
	}
}
