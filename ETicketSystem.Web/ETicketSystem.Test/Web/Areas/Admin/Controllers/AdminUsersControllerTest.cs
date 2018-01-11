namespace ETicketSystem.Test.Web.Areas.Admin.Controllers
{
    using ETicketSystem.Common.Constants;
    using ETicketSystem.Common.Enums;
    using ETicketSystem.Data.Models;
    using ETicketSystem.Services.Admin.Contracts;
    using ETicketSystem.Services.Admin.Models;
    using ETicketSystem.Web.Areas.Admin.Controllers;
    using ETicketSystem.Web.Areas.Admin.Models.AdminUsers;
    using FluentAssertions;
    using Infrastructure;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Mocks.Admin;
    using Moq;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Web.Controllers.Fixtures;
    using Xunit;

    using static Common.TestConstants;

    public class AdminUsersControllerTest : BaseControllerTest, IClassFixture<UserManagerGetUserIdFixture>
    {
        private const int MaxPageSize = 2;

        private const int TotalUsers = 30;

        private readonly UserManagerGetUserIdFixture fixture;

        private readonly Mock<IAdminUserService> userService = AdminUserServiceMock.New;

        public AdminUsersControllerTest(UserManagerGetUserIdFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void ControllerShouldBeForAuthenticatedUsersOnly()
        {
            //Arrange
            var controller = new AdminUsersController(null,null);

            //Act
            var attributes = controller.GetType().GetCustomAttributes(true);

            //Assert
            attributes.Any(a => a.GetType() == typeof(AuthorizeAttribute));
        }

        [Fact]
        public void ControllerShouldBeForAdminsOnly()
        {
            //Arrange
            var controller = new AdminUsersController(null, null);

            //Act
            var attributes = controller.GetType().GetCustomAttributes(true);
            var authorizeAttribute = attributes.FirstOrDefault(a => a.GetType() == typeof(AuthorizeAttribute));

            //Assert
            authorizeAttribute.As<AuthorizeAttribute>().Roles.Should().Be(Role.Administrator.ToString());
        }

        [Fact]
        public void ControllerShouldBeInAdminsArea()
        {
            //Arrange
            var controller = new AdminUsersController(null, null);

            //Act
            var attributes = controller.GetType().GetCustomAttributes(true);
            var areaAttribute = attributes.FirstOrDefault(a => a.GetType() == typeof(AreaAttribute));

            //Assert
            areaAttribute.As<AreaAttribute>().RouteValue.Should().Be(WebConstants.Area.Admin);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task All_WithPageLessOrEqualToZeroShouldRedirectToAllUsers(int page)
        {
            //Arrange
            var controller = new AdminUsersController(null,null);

            //Act
            var result = await controller.All(string.Empty, page);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var model = result.As<RedirectToActionResult>();
            model.ActionName.Should().Be(WebConstants.Action.AdminAllUsers);
            model.RouteValues.Keys.Should().Contain(RouteValueKeyPage);
            model.RouteValues.Values.Should().Contain(MinPageSize);
            model.RouteValues.Keys.Should().Contain(RouteValueKeySearchTerm);
            model.RouteValues.Values.Should().Contain(string.Empty);
        }

        [Fact]
        public async Task All_ValidPageAndNoTownsShouldReturnCountZero()
        {
            //Arrange
            var controller = new AdminUsersController(this.userService.Object,this.fixture.UserManagerMockInstance.Object);

            this.userService.Setup(u => u.AllAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new List<AdminUserListingServiceModel>());

            //Act
            var result = await controller.All(string.Empty, MinPageSize);

            //Assert
            result.Should().BeOfType<ViewResult>();
            var model = result.As<ViewResult>().Model.As<AllUsers>();
            model.Users.Should().HaveCount(0);
            model.Pagination.SearchTerm.Should().Be(string.Empty);
            model.Pagination.CurrentPage.Should().Be(MinPageSize);
            model.Pagination.NextPage.Should().Be(0);
            model.Pagination.PreviousPage.Should().Be(MinPageSize);
            model.Pagination.TotalElements.Should().Be(0);
        }

        [Fact]
        public async Task All_PageGreaterThanTotalPagesShouldRedirectToAllUsers()
        {
            const int InvalidPageSize = 10;

            //Arrange
            var controller = new AdminUsersController(this.userService.Object, this.fixture.UserManagerMockInstance.Object);

            this.userService.Setup(u => u.AllAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(this.GetUsers());

            this.userService.Setup(u => u.TotalUsers(It.IsAny<string>()))
                .Returns(TotalUsers);

            //Act
            var result = await controller.All(string.Empty, InvalidPageSize);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var model = result.As<RedirectToActionResult>();
            model.ActionName.Should().Be(WebConstants.Action.AdminAllUsers);
            model.RouteValues.Keys.Should().Contain(RouteValueKeyPage);
            model.RouteValues.Values.Should().Contain(MaxPageSize);
            model.RouteValues.Keys.Should().Contain(RouteValueKeySearchTerm);
            model.RouteValues.Values.Should().Contain(string.Empty);
        }

        [Fact]
        public async Task All_ShouldReturnCorrectDataWithValidInput()
        {
            //Arrange
            var controller = new AdminUsersController(this.userService.Object, this.fixture.UserManagerMockInstance.Object);

            this.userService.Setup(u => u.AllAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(this.GetUsers());

            this.userService.Setup(u => u.TotalUsers(It.IsAny<string>()))
                .Returns(TotalUsers);

            //Act
            var result = await controller.All(string.Empty, MinPageSize);

            //Assert
            result.Should().BeOfType<ViewResult>();
            var model = result.As<ViewResult>().Model.As<AllUsers>();
            model.Users.Should().HaveCount(TotalUsers);
            model.Pagination.SearchTerm.Should().Be(string.Empty);
            model.Pagination.CurrentPage.Should().Be(MinPageSize);
            model.Pagination.NextPage.Should().Be(MaxPageSize);
            model.Pagination.PreviousPage.Should().Be(MinPageSize);
            model.Pagination.TotalElements.Should().Be(TotalUsers);
        }

        [Fact]
        public async Task All_ShouldReturnCorrectDataWithValidInputAndParticularSearchTerm()
        {
            const string SearchTerm = "Test";

            //Arrange
            var controller = new AdminUsersController(this.userService.Object, this.fixture.UserManagerMockInstance.Object);

            this.userService.Setup(u => u.AllAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(this.GetUsers());

            this.userService.Setup(u => u.TotalUsers(It.IsAny<string>()))
                .Returns(TotalUsers);

            //Act
            var result = await controller.All(SearchTerm, MinPageSize);

            //Assert
            result.Should().BeOfType<ViewResult>();
            var model = result.As<ViewResult>().Model.As<AllUsers>();
            model.Users.Should().HaveCount(TotalUsers);
            model.Pagination.SearchTerm.Should().Be(SearchTerm);
            model.Pagination.CurrentPage.Should().Be(MinPageSize);
            model.Pagination.NextPage.Should().Be(MaxPageSize);
            model.Pagination.PreviousPage.Should().Be(MinPageSize);
            model.Pagination.TotalElements.Should().Be(TotalUsers);
        }

        [Fact]
        public async Task ChangeRoles_WithNonExistingUserShouldRedirectToAllUsers()
        {
            //Arrange
            User user = null;
            var controller = new AdminUsersController(null,this.fixture.UserManagerMockInstance.Object);

            this.fixture.UserManagerMockInstance.Setup(u => u.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = await controller.ChangeRolesAsync(UserId,Role.Administrator,false);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var model = result.As<RedirectToActionResult>();
            model.ActionName.Should().Be(WebConstants.Action.AdminAllUsers);
            this.customMessage.Should().Be(string.Format(WebConstants.Message.NonExistingEntity, nameof(WebConstants.Entity.User), UserId));
        }

        [Fact]
        public async Task ChangeRoles_RemoveUserFromRoleWhenNotInThisRoleShouldReirectToAllUsers()
        {
            //Arrange
            var controller = new AdminUsersController(null, this.fixture.UserManagerMockInstance.Object);

            this.fixture.UserManagerMockInstance.Setup(u => u.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(this.GetUser());

            this.fixture.UserManagerMockInstance.Setup(u => u.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = await controller.ChangeRolesAsync(UserId, Role.Administrator, true);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var model = result.As<RedirectToActionResult>();
            model.ActionName.Should().Be(WebConstants.Action.AdminAllUsers);
            this.customMessage.Should().Be(string.Format(WebConstants.Message.UserNotInRole, UserUsername, Role.Administrator.ToString()));
        }

        [Fact]
        public async Task ChangeRoles_RemoveUserFromRoleWhenInThisRoleShouldReirectToAllUsers()
        {
            //Arrange
            var controller = new AdminUsersController(null, this.fixture.UserManagerMockInstance.Object);

            this.fixture.UserManagerMockInstance.Setup(u => u.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(this.GetUser());

            this.fixture.UserManagerMockInstance.Setup(u => u.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            this.fixture.UserManagerMockInstance.Setup(u => u.RemoveFromRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(new IdentityResult());

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = await controller.ChangeRolesAsync(UserId, Role.Administrator, true);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var model = result.As<RedirectToActionResult>();
            model.ActionName.Should().Be(WebConstants.Action.AdminAllUsers);
            this.customMessage.Should().Be(string.Format(WebConstants.Message.UserRemovedFromRole, UserUsername, Role.Administrator.ToString()));
        }

        [Fact]
        public async Task ChangeRoles_AddUserToRoleWhenHeIsAlreadyInThatRoleShouldReirectToAllUsers()
        {
            //Arrange
            var controller = new AdminUsersController(null, this.fixture.UserManagerMockInstance.Object);

            this.fixture.UserManagerMockInstance.Setup(u => u.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(this.GetUser());

            this.fixture.UserManagerMockInstance.Setup(u => u.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = await controller.ChangeRolesAsync(UserId, Role.Administrator, false);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var model = result.As<RedirectToActionResult>();
            model.ActionName.Should().Be(WebConstants.Action.AdminAllUsers);
            this.customMessage.Should().Be(string.Format(WebConstants.Message.UserAlreadyInRole, UserUsername, Role.Administrator.ToString()));
        }

        [Fact]
        public async Task ChangeRoles_AddUserToRoleWhenNotInThisRoleShouldReirectToAllUsers()
        {
            //Arrange
            var controller = new AdminUsersController(null, this.fixture.UserManagerMockInstance.Object);

            this.fixture.UserManagerMockInstance.Setup(u => u.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(this.GetUser());

            this.fixture.UserManagerMockInstance.Setup(u => u.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            this.fixture.UserManagerMockInstance.Setup(u => u.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(new IdentityResult());

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = await controller.ChangeRolesAsync(UserId, Role.Administrator, false);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var model = result.As<RedirectToActionResult>();
            model.ActionName.Should().Be(WebConstants.Action.AdminAllUsers);
            this.customMessage.Should().Be(string.Format(WebConstants.Message.UserAddedToRole, UserUsername, Role.Administrator.ToString()));
        }

        private IEnumerable<AdminUserListingServiceModel> GetUsers()
        {
            var list = new List<AdminUserListingServiceModel>();

            for (int i = 0; i < TotalUsers; i++)
            {
                list.Add(new AdminUserListingServiceModel()
                {
                    Id = i.ToString(),
                    Name = $"User {i}",
                    Email = $"user{i}@user.com",
                    Username = $"username{i}"
                });
            }

            return list;
        }

        private User GetUser() =>
            new User()
            {
                UserName = UserUsername
            };
    }
}
