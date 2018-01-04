namespace ETicketSystem.Test.Web.Areas.Admin.Controllers
{
	using ETicketSystem.Common.Constants;
	using ETicketSystem.Common.Enums;
	using ETicketSystem.Services.Admin.Contracts;
	using ETicketSystem.Services.Admin.Models;
	using ETicketSystem.Web.Areas.Admin.Controllers;
	using ETicketSystem.Web.Areas.Admin.Models.AdminCompanies;
	using FluentAssertions;
	using Infrastructure;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using Mocks.Admin;
	using Moq;
	using System.Collections.Generic;
	using System.Linq;
	using Xunit;

	using static Common.TestConstants;

	public class AdminCompaniesControllerTest : BaseControllerTest
    {
		private const int TotalCompaniesCount = 15;

		private const int FirstPage = 1;

		private const int LastPage = 2;

		private const string CompanyStatusBlocked = "blocked";

		private const string CompanyStatusUnblocked = "unblocked";

		private readonly Mock<IAdminCompanyService> companyService = AdminCompanyServiceMock.New;

		[Fact]
		public void ControllerShouldBeInAdminArea()
		{
			//Arrange
			var controller = new AdminCompaniesController(null);

			//Act
			var attributes = controller.GetType().GetCustomAttributes(true);

			//Assert
			attributes.Any(a => a.GetType() == typeof(AreaAttribute));
			var areaAttribute = attributes.FirstOrDefault(a => a.GetType() == typeof(AreaAttribute));
			areaAttribute.As<AreaAttribute>().RouteValue.Should().Be(WebConstants.Area.Admin);
		}

		[Fact]
		public void ControllerShouldBeForAuthorizedUsers()
		{
			//Arrange
			var controller = new AdminCompaniesController(null);

			//Act
			var attributes = controller.GetType().GetCustomAttributes(true);

			//Assert
			attributes.Any(a => a.GetType() == typeof(AuthorizeAttribute));
		}

		[Fact]
		public void ControllerShouldBeForAdminsOnly()
		{
			//Arrange
			var controller = new AdminCompaniesController(null);

			//Act
			var attributes = controller.GetType().GetCustomAttributes(true);

			//Assert
			var authorizeAttribute = attributes.FirstOrDefault(a => a.GetType() == typeof(AuthorizeAttribute)).As<AuthorizeAttribute>();
			authorizeAttribute.Roles.Should().Be(Role.Administrator.ToString());
		}

		[Theory]
		[InlineData(0)]
		[InlineData(-1)]
		public void All_WithPageLessOrEqualToZeroShouldRedirectToAllCompanies(int page)
		{
			//Arrange
			var controller = new AdminCompaniesController(null);
			var filter = CompanyStatus.All;

			//Act
			var result = controller.All(filter, page);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			var model = result.As<RedirectToActionResult>();
			model.ActionName.Should().Be(WebConstants.Action.AdminAllCompanies);
			model.RouteValues.Keys.Should().Contain(RouteValueKeyAdminCompaniesFilter);
			model.RouteValues.Values.Should().Contain(filter);
		}

		[Theory]
		[InlineData((CompanyStatus)(-1))]
		[InlineData((CompanyStatus)(10))]
		public void All_WithCorrectPageAndNonExistingFilterShouldRedirectToAllCompaniesAndSetFilterToAll(CompanyStatus filter)
		{
			//Arrange
			var controller = new AdminCompaniesController(null);

			//Act
			var result = controller.All(filter);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			var model = result.As<RedirectToActionResult>();
			model.ActionName.Should().Be(WebConstants.Action.AdminAllCompanies);
			model.RouteValues.Keys.Should().Contain(RouteValueKeyAdminCompaniesFilter);
			model.RouteValues.Values.Should().Contain(CompanyStatus.All);
		}

		[Fact]
		public void All_WithCorrectFilterAndPageGreaterThanTotalPagesShouldRedirectToAllCompaniesAndLastPage()
		{
			const int Page = 10;

			//Arrange
			var controller = new AdminCompaniesController(this.companyService.Object);

			this.companyService.Setup(c => c.All(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
				.Returns(this.GetAllCompanies());

			this.companyService.Setup(c => c.TotalCompanies(It.IsAny<string>()))
				.Returns(TotalCompaniesCount);

			//Act
			var result = controller.All(CompanyStatus.All,Page);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			var model = result.As<RedirectToActionResult>();
			model.ActionName.Should().Be(WebConstants.Action.AdminAllCompanies);
			model.RouteValues.Keys.Should().Contain(RouteValueKeyAdminCompaniesFilter);
			model.RouteValues.Values.Should().Contain(CompanyStatus.All);
			model.RouteValues.Keys.Should().Contain(RouteValueKeyPage);
			model.RouteValues.Values.Should().Contain(LastPage);
		}

		[Fact]
		public void All_WithCorrectFilterAndPageShouldReturnNoCompaniesWhenThereAreNoRegisteredCompanies()
		{
			//Arrange
			var controller = new AdminCompaniesController(this.companyService.Object);

			this.companyService.Setup(c => c.All(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
				.Returns(new List<AdminCompanyListingServiceModel>());

			this.companyService.Setup(c => c.TotalCompanies(It.IsAny<string>()))
				.Returns(0);

			//Act
			var result = controller.All(CompanyStatus.All);

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model.As<AllCompanies>();
			model.Companies.Should().HaveCount(0);
			model.Filter.Should().Be(CompanyStatus.All);
			model.Pagination.CurrentPage.Should().Be(FirstPage);
			model.Pagination.NextPage.Should().Be(0);
			model.Pagination.PreviousPage.Should().Be(FirstPage);
			model.Pagination.SearchTerm.Should().Be(CompanyStatus.All.ToString());
		}

		[Fact]
		public void All_WithFilterForAllCompaniesAndPageShouldReturnCorrectData()
		{
			//Arrange
			var controller = new AdminCompaniesController(this.companyService.Object);

			this.companyService.Setup(c => c.All(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
				.Returns(this.GetAllCompanies());

			this.companyService.Setup(c => c.TotalCompanies(It.IsAny<string>()))
				.Returns(TotalCompaniesCount);

			//Act
			var result = controller.All(CompanyStatus.All);

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model.As<AllCompanies>();
			model.Companies.Should().HaveCount(TotalCompaniesCount);
			model.Filter.Should().Be(CompanyStatus.All);
			model.Pagination.CurrentPage.Should().Be(FirstPage);
			model.Pagination.NextPage.Should().Be(LastPage);
			model.Pagination.PreviousPage.Should().Be(FirstPage);
			model.Pagination.SearchTerm.Should().Be(CompanyStatus.All.ToString());
		}

		[Fact]
		public void All_WithFilterForBlockedCompaniesAndPageShouldReturnCorrectData()
		{
			const int BlockedCompaniesCount = 5;

			//Arrange
			var controller = new AdminCompaniesController(this.companyService.Object);

			this.companyService.Setup(c => c.All(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
				.Returns(this.GetBlockedCompanies());

			this.companyService.Setup(c => c.TotalCompanies(It.IsAny<string>()))
				.Returns(BlockedCompaniesCount);

			//Act
			var result = controller.All(CompanyStatus.Blocked);

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model.As<AllCompanies>();
			model.Companies.Should().HaveCount(BlockedCompaniesCount);
			model.Filter.Should().Be(CompanyStatus.Blocked);
			model.Pagination.CurrentPage.Should().Be(FirstPage);
			model.Pagination.NextPage.Should().Be(FirstPage);
			model.Pagination.PreviousPage.Should().Be(FirstPage);
			model.Pagination.SearchTerm.Should().Be(CompanyStatus.Blocked.ToString());
		}

		[Fact]
		public void All_WithFilterForApprovedCompaniesAndPageShouldReturnCorrectData()
		{
			const int ApprovedCompaniesCount = 5;

			//Arrange
			var controller = new AdminCompaniesController(this.companyService.Object);

			this.companyService.Setup(c => c.All(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
				.Returns(this.GetApprovedCompanies());

			this.companyService.Setup(c => c.TotalCompanies(It.IsAny<string>()))
				.Returns(ApprovedCompaniesCount);

			//Act
			var result = controller.All(CompanyStatus.Approved);

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model.As<AllCompanies>();
			model.Companies.Should().HaveCount(ApprovedCompaniesCount);
			model.Filter.Should().Be(CompanyStatus.Approved);
			model.Pagination.CurrentPage.Should().Be(FirstPage);
			model.Pagination.NextPage.Should().Be(FirstPage);
			model.Pagination.PreviousPage.Should().Be(FirstPage);
			model.Pagination.SearchTerm.Should().Be(CompanyStatus.Approved.ToString());
		}

		[Fact]
		public void All_WithFilterForUnapprovedCompaniesAndPageShouldReturnCorrectData()
		{
			const int UnapprovedCompaniesCount = 5;

			//Arrange
			var controller = new AdminCompaniesController(this.companyService.Object);

			this.companyService.Setup(c => c.All(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
				.Returns(this.GetUnapprovedCompanies());

			this.companyService.Setup(c => c.TotalCompanies(It.IsAny<string>()))
				.Returns(UnapprovedCompaniesCount);

			//Act
			var result = controller.All(CompanyStatus.Unapproved);

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model.As<AllCompanies>();
			model.Companies.Should().HaveCount(UnapprovedCompaniesCount);
			model.Filter.Should().Be(CompanyStatus.Unapproved);
			model.Pagination.CurrentPage.Should().Be(FirstPage);
			model.Pagination.NextPage.Should().Be(FirstPage);
			model.Pagination.PreviousPage.Should().Be(FirstPage);
			model.Pagination.SearchTerm.Should().Be(CompanyStatus.Unapproved.ToString());
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("Company_Id")]
		public void Approve_WithInvalidCompanyIdShouldRedirectToAllCompanies(string companyId)
		{
			//Arrange
			var controller = new AdminCompaniesController(this.companyService.Object);

			this.companyService.Setup(c => c.CompanyExists(It.IsAny<string>()))
				.Returns(false);

			this.PrepareTempData();

			controller.TempData = this.tempData.Object;

			//Act
			var result = controller.Approve(companyId,CompanyStatus.Unapproved,MinPageSize);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			var model = result.As<RedirectToActionResult>();
			model.ActionName.Should().Be(WebConstants.Action.AdminAllCompanies);
			this.customMessage.Should().Be(string.Format(WebConstants.Message.NonExistingEntity, nameof(WebConstants.Entity.Company), companyId));
		}

		[Fact]
		public void Approve_WithAlreadyApprovedCompanyShouldRedirectToAllCompanies()
		{
			//Arrange
			var controller = new AdminCompaniesController(this.companyService.Object);

			this.companyService.Setup(c => c.CompanyExists(It.IsAny<string>()))
				.Returns(true);

			this.companyService.Setup(c => c.Approve(It.IsAny<string>()))
				.Returns(false);

			this.companyService.Setup(c => c.GetCompanyName(It.IsAny<string>()))
				.Returns(CompanyName);

			this.PrepareTempData();

			controller.TempData = this.tempData.Object;

			//Act
			var result = controller.Approve(CompanyId, CompanyStatus.Unapproved, MinPageSize);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			var model = result.As<RedirectToActionResult>();
			model.ActionName.Should().Be(WebConstants.Action.AdminAllCompanies);
			model.RouteValues.Keys.Should().Contain(RouteValueKeyPage);
			model.RouteValues.Values.Should().Contain(MinPageSize);
			model.RouteValues.Keys.Should().Contain(RouteValueKeyAdminCompaniesFilter);
			model.RouteValues.Values.Should().Contain(CompanyStatus.Unapproved);
			this.customMessage.Should().Be(string.Format(WebConstants.Message.CompanyAlreadyApproved, CompanyName));
		}

		[Fact]
		public void Approve_WithValidDataShouldRedirectToAllCompanies()
		{
			//Arrange
			var controller = new AdminCompaniesController(this.companyService.Object);

			this.companyService.Setup(c => c.CompanyExists(It.IsAny<string>()))
				.Returns(true);

			this.companyService.Setup(c => c.Approve(It.IsAny<string>()))
				.Returns(true);

			this.companyService.Setup(c => c.GetCompanyName(It.IsAny<string>()))
				.Returns(CompanyName);

			this.PrepareTempData();

			controller.TempData = this.tempData.Object;

			//Act
			var result = controller.Approve(CompanyId, CompanyStatus.Unapproved, MinPageSize);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			var model = result.As<RedirectToActionResult>();
			model.ActionName.Should().Be(WebConstants.Action.AdminAllCompanies);
			model.RouteValues.Keys.Should().Contain(RouteValueKeyPage);
			model.RouteValues.Values.Should().Contain(MinPageSize);
			model.RouteValues.Keys.Should().Contain(RouteValueKeyAdminCompaniesFilter);
			model.RouteValues.Values.Should().Contain(CompanyStatus.Unapproved);
			this.customMessage.Should().Be(string.Format(WebConstants.Message.CompanyApproved, CompanyName));
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("Company_Id")]
		public void Block_WithInvalidCompanyIdShouldRedirectToAllCompanies(string companyId)
		{
			//Arrange
			var controller = new AdminCompaniesController(this.companyService.Object);

			this.companyService.Setup(c => c.CompanyExists(It.IsAny<string>()))
				.Returns(false);

			this.PrepareTempData();

			controller.TempData = this.tempData.Object;

			//Act
			var result = controller.Block(companyId, CompanyStatus.Unapproved, MinPageSize);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			var model = result.As<RedirectToActionResult>();
			model.ActionName.Should().Be(WebConstants.Action.AdminAllCompanies);
			this.customMessage.Should().Be(string.Format(WebConstants.Message.NonExistingEntity, nameof(WebConstants.Entity.Company), companyId));
		}

		[Fact]
		public void Block_UnapprovedCompanyShouldRedirectToAllCompanies()
		{
			//Arrange
			var controller = new AdminCompaniesController(this.companyService.Object);

			this.companyService.Setup(c => c.CompanyExists(It.IsAny<string>()))
				.Returns(true);

			this.companyService.Setup(c => c.ChangeStatus(It.IsAny<string>()))
				.Returns(false);

			this.PrepareTempData();

			controller.TempData = this.tempData.Object;

			//Act
			var result = controller.Block(CompanyId, CompanyStatus.All, MinPageSize);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			var model = result.As<RedirectToActionResult>();
			model.ActionName.Should().Be(WebConstants.Action.AdminAllCompanies);
			this.customMessage.Should().Be(string.Format(WebConstants.Message.BlockCompanyUnavailable, CompanyId));
		}

		[Fact]
		public void Block_WithValidDataShouldRedirectToAllCompanies()
		{
			//Arrange
			var controller = new AdminCompaniesController(this.companyService.Object);

			this.companyService.Setup(c => c.CompanyExists(It.IsAny<string>()))
				.Returns(true);

			this.companyService.Setup(c => c.ChangeStatus(It.IsAny<string>()))
				.Returns(true);

			this.companyService.Setup(c => c.GetCompanyName(It.IsAny<string>()))
				.Returns(CompanyName);

			this.companyService.Setup(c => c.GetBlockStatus(It.IsAny<string>()))
				.Returns(CompanyStatusBlocked);

			this.PrepareTempData();

			controller.TempData = this.tempData.Object;

			//Act
			var result = controller.Block(CompanyId, CompanyStatus.All, MinPageSize);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			var model = result.As<RedirectToActionResult>();
			model.ActionName.Should().Be(WebConstants.Action.AdminAllCompanies);
			model.RouteValues.Keys.Should().Contain(RouteValueKeyPage);
			model.RouteValues.Values.Should().Contain(MinPageSize);
			model.RouteValues.Keys.Should().Contain(RouteValueKeyAdminCompaniesFilter);
			model.RouteValues.Values.Should().Contain(CompanyStatus.All);
			this.customMessage.Should().Be(string.Format(WebConstants.Message.CompanyStatusChanged, CompanyName, CompanyStatusBlocked));
		}

		[Fact]
		public void UnBlock_WithValidDataShouldRedirectToAllCompanies()
		{
			//Arrange
			var controller = new AdminCompaniesController(this.companyService.Object);

			this.companyService.Setup(c => c.CompanyExists(It.IsAny<string>()))
				.Returns(true);

			this.companyService.Setup(c => c.ChangeStatus(It.IsAny<string>()))
				.Returns(true);

			this.companyService.Setup(c => c.GetCompanyName(It.IsAny<string>()))
				.Returns(CompanyName);

			this.companyService.Setup(c => c.GetBlockStatus(It.IsAny<string>()))
				.Returns(CompanyStatusUnblocked);

			this.PrepareTempData();

			controller.TempData = this.tempData.Object;

			//Act
			var result = controller.Block(CompanyId, CompanyStatus.All, MinPageSize);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			var model = result.As<RedirectToActionResult>();
			model.ActionName.Should().Be(WebConstants.Action.AdminAllCompanies);
			model.RouteValues.Keys.Should().Contain(RouteValueKeyPage);
			model.RouteValues.Values.Should().Contain(MinPageSize);
			model.RouteValues.Keys.Should().Contain(RouteValueKeyAdminCompaniesFilter);
			model.RouteValues.Values.Should().Contain(CompanyStatus.All);
			this.customMessage.Should().Be(string.Format(WebConstants.Message.CompanyStatusChanged, CompanyName, CompanyStatusUnblocked));
		}

		private IEnumerable<AdminCompanyListingServiceModel> GetAllCompanies()
		{
			var list = new List<AdminCompanyListingServiceModel>();

			for (int i = 0; i < 15; i++)
			{
				list.Add(new AdminCompanyListingServiceModel()
				{
					Id = i.ToString(),
					Name = $"Company {i}"
				});
			}

			return list;
		}

		private IEnumerable<AdminCompanyListingServiceModel> GetBlockedCompanies()
		{
			var list = new List<AdminCompanyListingServiceModel>();

			for (int i = 0; i < 5; i++)
			{
				list.Add(new AdminCompanyListingServiceModel()
				{
					Id = i.ToString(),
					Name = $"Company {i}",
					IsBlocked = true
				});
			}

			return list;
		}

		private IEnumerable<AdminCompanyListingServiceModel> GetApprovedCompanies()
		{
			var list = new List<AdminCompanyListingServiceModel>();

			for (int i = 0; i < 5; i++)
			{
				list.Add(new AdminCompanyListingServiceModel()
				{
					Id = i.ToString(),
					Name = $"Company {i}",
					IsApproved = true
				});
			}

			return list;
		}

		private IEnumerable<AdminCompanyListingServiceModel> GetUnapprovedCompanies()
		{
			var list = new List<AdminCompanyListingServiceModel>();

			for (int i = 0; i < 5; i++)
			{
				list.Add(new AdminCompanyListingServiceModel()
				{
					Id = i.ToString(),
					Name = $"Company {i}",
					IsApproved = false
				});
			}

			return list;
		}
	}
}
