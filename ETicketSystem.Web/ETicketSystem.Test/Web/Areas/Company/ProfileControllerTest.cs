namespace ETicketSystem.Test.Web.Areas.Company
{
    using ETicketSystem.Services.Company.Models;
    using ETicketSystem.Services.Contracts;
    using ETicketSystem.Web.Areas.Company.Controllers;
    using ETicketSystem.Web.Areas.Company.Models.Profile;
    using FluentAssertions;
    using Infrastructure;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Mocks;
    using Moq;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Web.Controllers.Fixtures;
    using Xunit;

    using static Common.TestConstants;
    using static ETicketSystem.Common.Constants.WebConstants;

    public class ProfileControllerTest : BaseControllerTest, IClassFixture<UserManagerGetUserIdFixture>
    {
        private const string EmailRequired = "Email field is required";

        private const string WrongCompanyId = "CompanyId";

        private const int ErrorsCount = 1;

        private readonly UserManagerGetUserIdFixture fixture;

        private readonly Mock<IUserService> userService = UserServiceMock.New;

        public ProfileControllerTest(UserManagerGetUserIdFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void ControllerShouldBeForAuthorizedUsersOnly()
        {
            //Arrange
            var controller = new ProfileController(null,null);

            //Assert
            var attributes = controller.GetType().GetCustomAttributes(true);

            //Act
            attributes.Any(a => a.GetType() == typeof(AuthorizeAttribute));
        }

        [Fact]
        public void ControllerShouldBeForCompanyUsersOnly()
        {
            //Arrange
            var controller = new ProfileController(null, null);

            //Assert
            var attributes = controller.GetType().GetCustomAttributes(true);
            var authorizeAttribute = attributes.FirstOrDefault(a => a.GetType() == typeof(AuthorizeAttribute));

            //Act
            authorizeAttribute.As<AuthorizeAttribute>().Roles.Should().Be(Role.CompanyRole.ToString());
        }

        [Fact]
        public void ControllerShouldBeInCompanyArea()
        {
            //Arrange
            var controller = new ProfileController(null, null);

            //Assert
            var attributes = controller.GetType().GetCustomAttributes(true);
            var areaAttribute = attributes.FirstOrDefault(a => a.GetType() == typeof(AreaAttribute));

            //Act
            areaAttribute.As<AreaAttribute>().RouteValue.Should().Be(Area.Company);
        }

        [Fact]
        public void Index_ShouldRedirectToHomeWhenCompanyIsNotProfileOwner()
        {
            //Arrange
            var controller = new ProfileController(null,this.fixture.UserManagerMockInstance.Object);

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = controller.Index(WrongCompanyId);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();
            this.AssertRedirectToHome(result);
            this.customMessage.Should().Be(Message.NotProfileOwner);
        }

        [Fact]
        public void Index_WithValidDataShouldReturnCompanyProfileInfo()
        {
            //Arrange
            var controller = new ProfileController(this.userService.Object, this.fixture.UserManagerMockInstance.Object);

            this.userService.Setup(u => u.GetCompanyProfileDetails(It.IsAny<string>()))
                .Returns(this.GetCompanyProfile());

            //Act
            var result = controller.Index(UserId);

            //Assert
            result.Should().BeOfType<ViewResult>();
            var model = result.As<ViewResult>().Model;
            model.Should().BeOfType<CompanyProfileBaseServiceModel>();
            var profile = model.As<CompanyProfileBaseServiceModel>();
            profile.Chief.Should().Be(CompanyChief);
            profile.Email.Should().Be(CompanyEmail);
            profile.Name.Should().Be(CompanyName);
            profile.Id.Should().Be(UserId);
            profile.TotalRoutes.Should().Be(TotalRoutes);
            profile.TotalTicketsSold.Should().Be(TicketsSold);
            profile.Phone.Should().Be(CompanyPhone);
        }

        [Fact]
        public void Get_Edit_ShouldRedirectToHomeWithNonExistingCompany()
        {
            //Arrange
            CompanyProfileServiceModel company = null;
            var controller = new ProfileController(this.userService.Object, null);

            this.userService.Setup(u => u.GetCompanyUserProfileToEdit(It.IsAny<string>()))
                .Returns(company);

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = controller.Edit(UserId);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();
            this.AssertRedirectToHome(result);
            this.customMessage.Should().Be(string.Format(Message.NonExistingEntity, Entity.Company, UserId));
        }

        [Fact]
        public void Get_Edit_ShouldRedirectToHomeWhenUserIsNotCompanyProfileOwner()
        {
            //Arrange
            var controller = new ProfileController(this.userService.Object, this.fixture.UserManagerMockInstance.Object);

            this.userService.Setup(u => u.GetCompanyUserProfileToEdit(It.IsAny<string>()))
                .Returns(this.GetCompanyProfileToEdit());

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = controller.Edit(WrongCompanyId);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();
            this.AssertRedirectToHome(result);
            this.customMessage.Should().Be(string.Format(Message.NotProfileOwner, UserId));
        }

        [Fact]
        public void Get_Edit_WithCorrectDataShouldReturnProfileToEdit()
        {
            //Arrange
            var controller = new ProfileController(this.userService.Object, this.fixture.UserManagerMockInstance.Object);

            this.userService.Setup(u => u.GetCompanyUserProfileToEdit(It.IsAny<string>()))
                .Returns(this.GetCompanyProfileToEdit());

            //Act
            var result = controller.Edit(UserId);

            //Assert
            result.Should().BeOfType<ViewResult>();
            var model = result.As<ViewResult>().Model;
            model.Should().BeOfType<EditCompanyProfileFormModel>();
            var profile = model.As<EditCompanyProfileFormModel>();
            profile.Email.Should().Be(CompanyEmail);
            profile.CurrentLogo.Should().BeEquivalentTo(CompanyLogo);
            profile.PhoneNumber.Should().Be(CompanyPhone);
            profile.Username.Should().Be(UserUsername);
        }

        [Fact]
        public async Task Post_Edit_WithInvalidModelStateShouldReturnEditForm()
        {
            //Arrange
            var controller = new ProfileController(this.userService.Object, this.fixture.UserManagerMockInstance.Object);

            this.userService.Setup(u => u.GetCompanyLogo(It.IsAny<string>()))
                .Returns(CompanyLogo);

            controller.ModelState.AddModelError(string.Empty, "Error");

            //Act
            var result = await controller.Edit(UserId, this.GetCompanyEditProfileForm());

            //Assert
            result.Should().BeOfType<ViewResult>();
            var model = result.As<ViewResult>().Model;
            model.Should().BeOfType<EditCompanyProfileFormModel>();
            var profile = model.As<EditCompanyProfileFormModel>();
            profile.Email.Should().Be(CompanyEmail);
            profile.CurrentLogo.Should().BeEquivalentTo(CompanyLogo);
            profile.PhoneNumber.Should().Be(CompanyPhone);
            profile.Username.Should().Be(UserUsername);
        }

        [Fact]
        public async Task Post_Edit_WithNonExistingCompanyShouldReturnBadRequest()
        {
            //Arrange
            var controller = new ProfileController(this.userService.Object, this.fixture.UserManagerMockInstance.Object);

            this.userService.Setup(u => u.UserExists(It.IsAny<string>()))
                .Returns(false);

            //Act
            var result = await controller.Edit(UserId, this.GetCompanyEditProfileForm());

            //Assert
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async Task Post_Edit_WithNotProfileOwnerCompanyShouldReturnBadRequest()
        {
            //Arrange
            var controller = new ProfileController(this.userService.Object, this.fixture.UserManagerMockInstance.Object);

            this.userService.Setup(u => u.UserExists(It.IsAny<string>()))
                .Returns(true);

            //Act
            var result = await controller.Edit(WrongCompanyId, this.GetCompanyEditProfileForm());

            //Assert
            result.Should().BeOfType<BadRequestResult>();
        }

        [Theory]
        [InlineData(Message.EmptyUsername)]
        [InlineData(EmailRequired)]
        [InlineData(Message.BothPasswordFieldsRequired)]
        [InlineData(Message.IncorrectOldPassword)]
        public async Task Post_Edit_WithEmptyUsernameShouldReturnViewWithEditForm(string error)
        {
            //Arrange
            var controller = new ProfileController(this.userService.Object, this.fixture.UserManagerMockInstance.Object);

            this.userService.Setup(u => u.UserExists(It.IsAny<string>()))
                .Returns(true);

            this.userService.Setup(u => u.EditCompanyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<string>()))
                .ReturnsAsync(this.GetIdentityError(error));

            this.userService.Setup(u => u.GetCompanyLogo(It.IsAny<string>()))
                .Returns(CompanyLogo);

            //Act
            var result = await controller.Edit(UserId, this.GetCompanyEditProfileForm());

            //Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result.As<ViewResult>();
            viewResult.ViewData.ModelState.ErrorCount.Should().Be(ErrorsCount);
            viewResult.ViewData.ModelState.Root.Errors.Any(e => e.ErrorMessage == error);
            this.AssertEditCompanyForm(result);
        }

        [Fact]
        public async Task Post_Edit_WithValidDataShouldRedirectToCompanyProfile()
        {
            //Arrange
            var controller = new ProfileController(this.userService.Object, this.fixture.UserManagerMockInstance.Object);

            this.userService.Setup(u => u.UserExists(It.IsAny<string>()))
                .Returns(true);

            this.userService.Setup(u => u.EditCompanyAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<string>()))
                .ReturnsAsync(new List<IdentityError>());

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = await controller.Edit(UserId, this.GetCompanyEditProfileForm());

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var redirectResult = result.As<RedirectToActionResult>();
            redirectResult.ActionName.Should().Be(Action.Index);
            redirectResult.RouteValues.Keys.Should().Contain(RouteValueKeyId);
            redirectResult.RouteValues.Values.Should().Contain(UserId);
            this.customMessage.Should().Be(Message.ProfileEdited);
        }

        private CompanyProfileBaseServiceModel GetCompanyProfile() =>
            new CompanyProfileBaseServiceModel()
            {
                Chief = CompanyChief,
                Id = UserId,
                Name = CompanyName,
                Email = CompanyEmail,
                TotalRoutes = TotalRoutes,
                TotalTicketsSold = TicketsSold,
                Phone = CompanyPhone
            };

        private EditCompanyProfileFormModel GetCompanyEditProfileForm() =>
            new EditCompanyProfileFormModel()
            {
                Email = CompanyEmail,
                CurrentLogo = CompanyLogo,
                PhoneNumber = CompanyPhone,
                Username = UserUsername
            };

        private CompanyProfileServiceModel GetCompanyProfileToEdit() =>
            new CompanyProfileServiceModel()
            {
                Email = CompanyEmail,
                Logo = CompanyLogo,
                PhoneNumber = CompanyPhone,
                Username = UserUsername,
                Id = UserId
            };

        private IEnumerable<IdentityError> GetIdentityError(string message) =>
            new List<IdentityError>()
            {
                new IdentityError()
                {
                    Code = string.Empty,
                    Description = message
                }
            };

        private void AssertEditCompanyForm(IActionResult result)
        {
            var model = result.As<ViewResult>().Model;
            model.Should().BeOfType<EditCompanyProfileFormModel>();
            var form = model.As<EditCompanyProfileFormModel>();
            form.Email.Should().Be(CompanyEmail);
            form.CurrentLogo.Should().BeEquivalentTo(CompanyLogo);
            form.PhoneNumber.Should().Be(CompanyPhone);
            form.Username.Should().Be(UserUsername);
        }
    }
}
