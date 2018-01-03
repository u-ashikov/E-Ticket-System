namespace ETicketSystem.Test.Web.Controllers
{
	using ETicketSystem.Common.Constants;
	using ETicketSystem.Services.Contracts;
	using ETicketSystem.Services.Models.Company;
	using ETicketSystem.Services.Models.Ticket;
	using ETicketSystem.Services.Models.Town;
	using ETicketSystem.Services.Models.User;
	using ETicketSystem.Test.Mock;
	using ETicketSystem.Web.Controllers;
	using ETicketSystem.Web.Models.Pagination;
	using ETicketSystem.Web.Models.Users;
	using Fixtures;
	using FluentAssertions;
	using Infrastructure;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.Rendering;
	using Mocks;
	using Moq;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Xunit;

	using static Common.TestConstants;

	public class UsersControllerTest : BaseControllerTest, IClassFixture<UserManagerGetUserIdFixture>
	{
		private const string UserEmail = "user@user.com";

		private const string UserFirstName = "Ivan";

		private const string UserLastName = "Ivanov";

		private const string NotProfileOwnerUserId = "SomeTestUserId";

		private const string UserOldPassword = "Some password12";

		private const string UserNewPassword = "New password12";

		private const int StartTownId = 1;

		private const int EndTownId = 2;

		private const int MaxPageSize = 2;

		private readonly Mock<IUserService> userService = UserServiceMock.New;

		private readonly UserManagerGetUserIdFixture fixture;

		private readonly Mock<ITicketService> ticketService = TicketServiceMock.New;

		private readonly Mock<ITownService> townService = TownServiceMock.New;

		private readonly Mock<ICompanyService> companyService = CompanyServiceMock.New;

		public UsersControllerTest(UserManagerGetUserIdFixture fixture)
		{
			this.fixture = fixture;
		}

		[Fact]
		public void ControllerShouldBeForAuthorizedUsers()
		{
			//Arrange
			var controller = new UsersController(null,null,null,null,null);

			//Act
			var attributes = controller.GetType().GetCustomAttributes(true);

			//Assert
			attributes.Any(a => a.GetType() == typeof(AuthorizeAttribute));
		}

		[Theory]
		[InlineData("   ")]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("TestUserId")]
		public async Task ProfileShouldRedirectToHomeWhenUserIdIsInvalidOrIsNotProfileOwner(string userId)
		{
			//Arrange
			var controller = new UsersController(null, null, this.fixture.UserManagerMockInstance.Object, null, null);

			this.PrepareTempData();

			controller.TempData = this.tempData.Object;

			//Act
			var result = await controller.Profile(userId);

			//Assert
			this.AssertRedirectToHome(result);
			this.customMessage.Should().Be(WebConstants.Message.NotProfileOwner);
		}

		[Fact]
		public async Task ProfileShouldRedirectToHomeWhenUserDoesNotExist()
		{
			//Arrange
			RegularUserProfileServiceModel user = null;
			var controller = new UsersController(this.userService.Object, null, this.fixture.UserManagerMockInstance.Object, null, null);

			this.userService.Setup(u => u.GetRegularUserProfileDetailsAsync(It.IsAny<string>()))
				.ReturnsAsync(user);

			this.PrepareTempData();

			controller.TempData = this.tempData.Object;

			//Act
			var result = await controller.Profile(UserId);

			//Assert
			this.AssertRedirectToHome(result);
			this.customMessage.Should().Be(string.Format(WebConstants.Message.NonExistingEntity, WebConstants.Entity.User, UserId));
		}

		[Fact]
		public async Task ProfileShouldReturnViewWithUserInfoWithCorrectData()
		{
			//Arrange
			var controller = new UsersController(this.userService.Object, null, this.fixture.UserManagerMockInstance.Object, null, null);

			this.userService.Setup(u => u.GetRegularUserProfileDetailsAsync(It.IsAny<string>()))
				.ReturnsAsync(this.GetUserProfile());

			//Act
			var result = await controller.Profile(UserId);

			//Assert
			result.Should().BeOfType<ViewResult>();
			RegularUserProfileServiceModel model = result.As<ViewResult>().Model.As<RegularUserProfileServiceModel>();
			model.Email.Should().Be(UserEmail);
			model.FirstName.Should().Be(UserFirstName);
			model.LastName.Should().Be(UserLastName);
			model.Id.Should().Be(UserId);
			model.Username.Should().Be(UserUsername);
		}

		[Theory]
		[InlineData("   ")]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("TestUserId")]
		public void Get_EditUserShouldRedirectToHomeWhenUserDoesNotExistOrUserIdIsInvalid(string userId)
		{
			//Arrange
			var controller = new UsersController(this.userService.Object,null,null,null,null);

			this.userService.Setup(u => u.UserExists(It.IsAny<string>()))
				.Returns(false);

			this.PrepareTempData();

			controller.TempData = this.tempData.Object;

			//Act
			var result = controller.EditUser(userId);

			//Assert
			this.AssertRedirectToHome(result);
			this.customMessage.Should().Be(string.Format(WebConstants.Message.NonExistingEntity, WebConstants.Entity.User, userId));
		}

		[Theory]
		[InlineData("   ")]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("TestUserId")]
		public void Get_EditUserShouldRedirectToHomeWhenUserIdIsInvalidOrIsNotProfileOwner(string userId)
		{
			//Arrange
			var controller = new UsersController(this.userService.Object, null, this.fixture.UserManagerMockInstance.Object, null, null);

			this.userService.Setup(u => u.UserExists(It.IsAny<string>()))
				.Returns(true);

			this.PrepareTempData();

			controller.TempData = this.tempData.Object;

			//Act
			var result = controller.EditUser(userId);

			//Assert
			this.AssertRedirectToHome(result);
			this.customMessage.Should().Be(string.Format(WebConstants.Message.NotProfileOwner, userId));
		}

		[Fact]
		public void Get_EditUserShouldReturnViewWithUserInfoWithCorrectData()
		{
			//Arrange
			var controller = new UsersController(this.userService.Object, null, this.fixture.UserManagerMockInstance.Object, null, null);

			this.userService.Setup(u => u.UserExists(It.IsAny<string>()))
				.Returns(true);

			this.userService.Setup(u => u.GetRegularUserProfileToEdit(It.IsAny<string>()))
				.Returns(this.GetUserProfile());

			//Act
			var result = controller.EditUser(UserId);

			//Assert
			result.Should().BeOfType<ViewResult>();
			EditUserProfileFormModel model = result.As<ViewResult>().Model.As<EditUserProfileFormModel>();
			model.Email.Should().Be(UserEmail);
			model.Username.Should().Be(UserUsername);
		}

		[Fact]
		public async Task Post_EditUserShouldReturnViewWhenModelStateIsInvalid()
		{
			//Arrange
			var controller = new UsersController(null, null, null, null, null);
			controller.ModelState.AddModelError(string.Empty, "Error");

			//Act
			var result = await controller.EditUser(UserId, this.GetEditUserProfileForm());

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model.As<EditUserProfileFormModel>();
			model.Username.Should().Be(UserUsername);
			model.Email.Should().Be(UserEmail);
		}

		[Theory]
		[InlineData("")]
		[InlineData("    ")]
		[InlineData(null)]
		[InlineData("TestUserId")]
		public async Task Post_EditUserShouldReturnBadRequestWithInvalidUserId(string userId)
		{
			//Arrange
			var controller = new UsersController(this.userService.Object, null, null, null, null);

			this.userService.Setup(u => u.UserExists(It.IsAny<string>()))
				.Returns(false);

			//Act
			var result = await controller.EditUser(userId, this.GetEditUserProfileForm());

			//Assert
			result.Should().BeOfType<BadRequestResult>();
		}

		[Fact]
		public async Task Post_EditUserShouldReturnBadRequestWhenUserDoesNotExist()
		{
			//Arrange
			var controller = new UsersController(this.userService.Object, null, null, null, null);

			this.userService.Setup(u => u.UserExists(It.IsAny<string>()))
				.Returns(false);

			//Act
			var result = await controller.EditUser(UserId, this.GetEditUserProfileForm());

			//Assert
			result.Should().BeOfType<BadRequestResult>();
		}

		[Fact]
		public async Task Post_EditUserShouldReturnBadRequestWhenUserIsNotProfileOwner()
		{
			//Arrange
			var controller = new UsersController(this.userService.Object, null, this.fixture.UserManagerMockInstance.Object, null, null);

			this.userService.Setup(u => u.UserExists(It.IsAny<string>()))
				.Returns(true);

			//Act
			var result = await controller.EditUser(NotProfileOwnerUserId, this.GetEditUserProfileForm());

			//Assert
			result.Should().BeOfType<BadRequestResult>();
		}

		[Fact]
		public async Task Post_EditUserShouldReturnUserProfileWhenThereAreErrorsDuringEditingProfile()
		{
			//Arrange
			var controller = new UsersController(this.userService.Object, null, this.fixture.UserManagerMockInstance.Object, null, null);

			this.userService.Setup(u => u.UserExists(It.IsAny<string>()))
				.Returns(true);

			this.userService.Setup(u => u.EditRegularUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync(new List<IdentityError>() { new IdentityError() { Description = string.Empty} });

			//Act
			var result = await controller.EditUser(UserId, this.GetEditUserProfileForm());

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model.As<EditUserProfileFormModel>();
			model.Email.Should().Be(UserEmail);
			model.Username.Should().Be(UserUsername);
			model.Password.Should().Be(UserOldPassword);
			model.NewPassword.Should().Be(UserNewPassword);
		}

		[Fact]
		public async Task Post_EditUserShouldRedirectToUserProfileWithCorrectData()
		{
			//Arrange
			var controller = new UsersController(this.userService.Object, null, this.fixture.UserManagerMockInstance.Object, null, null);

			this.userService.Setup(u => u.UserExists(It.IsAny<string>()))
				.Returns(true);

			this.userService.Setup(u => u.EditRegularUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync(new List<IdentityError>());

			this.PrepareTempData();

			controller.TempData = this.tempData.Object;

			//Act
			var result = await controller.EditUser(UserId, this.GetEditUserProfileForm());

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			var model = result.As<RedirectToActionResult>();
			model.RouteValues.Keys.Should().Contain(RouteValueKeyId);
			model.RouteValues.Values.Should().Contain(UserId);
			this.customMessage.Should().Be(WebConstants.Message.ProfileEdited);
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData(NotProfileOwnerUserId)]
		public void MyTicketsShouldRedirectToHomeWhenUserIsNotSameWithLoggedInUser(string userId)
		{
			//Arrange
			var controller = new UsersController(null,null,this.fixture.UserManagerMockInstance.Object,null,null);

			this.PrepareTempData();

			controller.TempData = this.tempData.Object;

			//Act
			var result = controller.MyTickets(userId, 0, 0, string.Empty, null);

			//Assert
			this.AssertRedirectToHome(result);
			this.customMessage.Should().Be(WebConstants.Message.NotProfileOwner);
		}

		[Theory]
		[InlineData(-1)]
		[InlineData(0)]
		public void MyTicketsShouldRedirectToUserTicketsWithPageLessThanOrEqualToZero(int page)
		{
			//Arrange
			var controller = new UsersController(null, null, this.fixture.UserManagerMockInstance.Object, null, null);

			//Act
			var result = controller.MyTickets(UserId, StartTownId, EndTownId, CompanyId, null,page);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			var model = result.As<RedirectToActionResult>();
			model.ActionName.Should().Be(WebConstants.Action.MyTickets);
			model.RouteValues.Keys.Should().Contain(RouteValueStartTownKey);
			model.RouteValues.Values.Should().Contain(StartTownId);
			model.RouteValues.Keys.Should().Contain(RouteValueEndTownKey);
			model.RouteValues.Values.Should().Contain(EndTownId);
			model.RouteValues.Keys.Should().Contain(RouteValueKeyId);
			model.RouteValues.Values.Should().Contain(UserId);
			model.RouteValues.Keys.Should().Contain(RouteValueDateKey);
			model.RouteValues[RouteValueDateKey].Should().Be(null);
			model.RouteValues.Keys.Should().Contain(RouteValueKeyPage);
			model.RouteValues.Values.Should().Contain(MinPageSize);
		}

		[Fact]
		public void MyTicketsShouldReturnNoTicketsWhenTownDoesNotExist()
		{
			//Arrange
			UsersController controller = this.PrepareControllerWithAllFeatures();

			this.ticketService.Setup(t => t.GetUserTickets(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns(new List<UserTicketListingServiceModel>());

			this.ticketService.Setup(t => t.UserTicketsCount(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime>()))
				.Returns(0);

			this.PrepareTownsAndCompaniesSelectLists();

			//Act
			var result = controller.MyTickets(UserId, StartTownId, EndTownId, CompanyId, null);

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model.As<UserTickets>();
			model.Tickets.Should().HaveCount(0);
			model.Pagination.CurrentPage.Should().Be(MinPageSize);
			model.Pagination.PreviousPage.Should().Be(MinPageSize);
			model.Pagination.TotalElements.Should().Be(0);
			model.CompanyId.Should().Be(CompanyId);
			model.StartTown.Should().Be(StartTownId);
			model.EndTown.Should().Be(EndTownId);
			model.Date.Should().BeNull();
		}

		[Fact]
		public void MyTicketsShouldRedirectToUserTicketsWhenPageIsGreaterThanTotalPages()
		{
			const int Page = 10;

			//Arrange
			UsersController controller = this.PrepareControllerWithAllFeatures();
			var tickets = this.GetUserTickets();

			this.ticketService.Setup(t => t.GetUserTickets(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns(tickets);

			this.ticketService.Setup(t => t.UserTicketsCount(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime?>()))
				.Returns(this.GetUserTickets().Count());

			this.PrepareTownsAndCompaniesSelectLists();

			//Act
			var result = controller.MyTickets(UserId, StartTownId, EndTownId, CompanyId, null, Page);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			var model = result.As<RedirectToActionResult>();
			model.ActionName.Should().Be(WebConstants.Action.MyTickets);
			model.RouteValues.Keys.Should().Contain(RouteValueStartTownKey);
			model.RouteValues.Values.Should().Contain(StartTownId);
			model.RouteValues.Keys.Should().Contain(RouteValueEndTownKey);
			model.RouteValues.Values.Should().Contain(EndTownId);
			model.RouteValues.Keys.Should().Contain(RouteValueKeyId);
			model.RouteValues.Values.Should().Contain(UserId);
			model.RouteValues.Keys.Should().Contain(RouteValueDateKey);
			model.RouteValues[RouteValueDateKey].Should().Be(null);
			model.RouteValues.Keys.Should().Contain(RouteValueKeyPage);
			model.RouteValues.Values.Should().Contain(MaxPageSize);
		}

		[Fact]
		public void MyTicketsShouldReturnCorrectDataWithCorrectInput()
		{
			const int TicketsCount = 30;

			//Arrange
			UsersController controller = this.PrepareControllerWithAllFeatures();
			var tickets = this.GetUserTickets();

			this.ticketService.Setup(t => t.GetUserTickets(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns(tickets);

			this.ticketService.Setup(t => t.UserTicketsCount(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<DateTime?>()))
				.Returns(this.GetUserTickets().Count());

			this.PrepareTownsAndCompaniesSelectLists();

			//Act
			var result = controller.MyTickets(UserId, StartTownId, EndTownId, CompanyId, null);

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model.As<UserTickets>();
			model.Pagination.CurrentPage.Should().Be(MinPageSize);
			model.Tickets.Should().HaveCount(TicketsCount);
			model.Pagination.NextPage.Should().Be(MaxPageSize);
			model.Pagination.PreviousPage.Should().Be(MinPageSize);
			model.StartTown.Should().Be(StartTownId);
			model.EndTown.Should().Be(EndTownId);
			model.CompanyId.Should().Be(CompanyId);
			model.Date.Should().BeNull();
		}

		private RegularUserProfileServiceModel GetUserProfile()
		{
			return new RegularUserProfileServiceModel()
			{
				Email = UserEmail,
				FirstName = UserFirstName,
				LastName = UserLastName,
				Id = UserId,
				Username = UserUsername
			};
		}

		private EditUserProfileFormModel GetEditUserForm (RegularUserProfileServiceModel user)
		{
			return new EditUserProfileFormModel()
			{
				Username = user.Username,
				Email = user.Email
			};
		}

		private EditUserProfileFormModel GetEditUserProfileForm()
		{
			return new EditUserProfileFormModel()
			{
				Username = UserUsername,
				Email = UserEmail,
				Password = UserOldPassword,
				NewPassword = UserNewPassword
			};
		}

		private IEnumerable<UserTicketListingServiceModel> GetUserTickets()
		{
			var list = new List<UserTicketListingServiceModel>();

			for (int i = 0; i < 30; i++)
			{
				list.Add(new UserTicketListingServiceModel()
				{
					Id = i,
					CompanyId = i % 2 == 0 ? $"Company {i}" : $"Company {i-1}",
					DepartureTime = DateTime.UtcNow.ToLocalTime().AddDays(i * -1 + i * 2)
				});
			}

			return list;
		}

		private IEnumerable<TownBaseServiceModel> GetTowns()
		{
			var list = new List<TownBaseServiceModel>();

			for (int i = 0; i < 10; i++)
			{
				list.Add(new TownBaseServiceModel()
				{
					Id = i,
					Name = $"Town {i}"
				});
			}

			return list;
		}

		private IEnumerable<CompanyBaseServiceModel> GetCompanies()
		{
			var list = new List<CompanyBaseServiceModel>();

			for (int i = 0; i < 10; i++)
			{
				list.Add(new CompanyBaseServiceModel()
				{
					Id = i.ToString(),
					Name = $"Company {i}"
				});
			}

			return list;
		}

		private UsersController PrepareControllerWithAllFeatures()
		{
			return new UsersController(null, this.townService.Object, this.fixture.UserManagerMockInstance.Object, this.ticketService.Object, this.companyService.Object);
		}

		private void PrepareTownsAndCompaniesSelectLists()
		{
			this.townService.Setup(t => t.GetTownsListItems())
							.Returns(this.GetTowns());

			this.companyService.Setup(c => c.GetCompaniesSelectListItems())
				.Returns(this.GetCompanies());
		}
	}
}
