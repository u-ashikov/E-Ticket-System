namespace ETicketSystem.Test.Web.Areas.Admin.Controllers
{
    using ETicketSystem.Common.Constants;
    using ETicketSystem.Common.Enums;
    using ETicketSystem.Services.Admin.Models;
    using ETicketSystem.Web.Areas.Admin.Controllers;
    using ETicketSystem.Web.Areas.Admin.Models.AdminTowns;
    using FluentAssertions;
    using Infrastructure;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Mocks.Admin;
    using Moq;
    using Services.Admin.Contracts;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    using static Common.TestConstants;

    public class AdminTownsControllerTest : BaseControllerTest
    {
        private const int TotalTowns = 30;

        private const int MaxPageSize = 2;

        private const int TownId = 21;

        private readonly Mock<IAdminTownService> townService = AdminTownServiceMock.New;

        [Fact]
        public void ControllerShoulBeForAuthorizedUsersOnly()
        {
            //Arrange
            var controller = new AdminTownsController(null);

            //Act
            var attributes = controller.GetType().GetCustomAttributes(true);

            //Assert
            attributes.Any(a => a.GetType() == typeof(AuthorizeAttribute));
        }

        [Fact]
        public void ControllerShoulBeForAdminsOnly()
        {
            //Arrange
            var controller = new AdminTownsController(null);

            //Act
            var attributes = controller.GetType().GetCustomAttributes(true);

            //Assert
            var authorizeAttribute = attributes.FirstOrDefault(a => a.GetType() == typeof(AuthorizeAttribute));
            authorizeAttribute.As<AuthorizeAttribute>().Roles.Should().Be(Role.Administrator.ToString());
        }

        [Fact]
        public void ControllerShoulBeInAdminArea()
        {
            //Arrange
            var controller = new AdminTownsController(null);

            //Act
            var attributes = controller.GetType().GetCustomAttributes(true);

            //Assert
            attributes.Any(a => a.GetType() == typeof(AreaAttribute));
            var areaAttribute = attributes.FirstOrDefault(a => a.GetType() == typeof(AreaAttribute));
            areaAttribute.As<AreaAttribute>().RouteValue.Should().Be(WebConstants.Area.Admin);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void All_WithPageLessOrEqualToZeroShouldRedirectToAllTowns(int page)
        {
            //Arrange
            var controller = new AdminTownsController(this.townService.Object);

            //Act
            var result = controller.All(string.Empty, page);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var model = result.As<RedirectToActionResult>();
            model.ActionName.Should().Be(WebConstants.Action.AdminAllTowns);
            model.RouteValues.Keys.Should().Contain(RouteValueKeyPage);
            model.RouteValues.Values.Should().Contain(MinPageSize);
            model.RouteValues.Keys.Should().Contain(RouteValueKeySearchTerm);
            model.RouteValues.Values.Should().Contain(string.Empty);
        }

        [Fact]
        public void All_ValidPageAndNoTownsShouldReturnCountZero()
        {
            //Arrange
            var controller = new AdminTownsController(this.townService.Object);

            this.townService.Setup(t => t.All(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(new List<AdminTownListingServiceModel>());

            this.townService.Setup(t => t.TotalTowns(It.IsAny<string>()))
                .Returns(0);

            //Act
            var result = controller.All(string.Empty, MinPageSize);

            //Assert
            result.Should().BeOfType<ViewResult>();
            var model = result.As<ViewResult>().Model.As<AllTowns>();
            model.Towns.Should().HaveCount(0);
            model.Pagination.SearchTerm.Should().Be(string.Empty);
            model.Pagination.CurrentPage.Should().Be(MinPageSize);
            model.Pagination.NextPage.Should().Be(0);
            model.Pagination.PreviousPage.Should().Be(MinPageSize);
            model.Pagination.TotalElements.Should().Be(0);
        }

        [Fact]
        public void All_PageGreaterThanTotalPagesShouldRedirectToAllTowns()
        {
            const int InvalidPageSize = 10;

            //Arrange
            var controller = new AdminTownsController(this.townService.Object);

            this.townService.Setup(t => t.All(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(this.GetTowns());

            this.townService.Setup(t => t.TotalTowns(It.IsAny<string>()))
                .Returns(TotalTowns);

            //Act
            var result = controller.All(string.Empty, InvalidPageSize);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var model = result.As<RedirectToActionResult>();
            model.ActionName.Should().Be(WebConstants.Action.AdminAllTowns);
            model.RouteValues.Keys.Should().Contain(RouteValueKeyPage);
            model.RouteValues.Values.Should().Contain(MaxPageSize);
            model.RouteValues.Keys.Should().Contain(RouteValueKeySearchTerm);
            model.RouteValues.Values.Should().Contain(string.Empty);
        }

        [Fact]
        public void All_ShouldReturnCorrectDataWithValidInput()
        {
            //Arrange
            var controller = new AdminTownsController(this.townService.Object);

            this.townService.Setup(t => t.All(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(this.GetTowns());

            this.townService.Setup(t => t.TotalTowns(It.IsAny<string>()))
                .Returns(TotalTowns);

            //Act
            var result = controller.All(string.Empty, MinPageSize);

            //Assert
            result.Should().BeOfType<ViewResult>();
            var model = result.As<ViewResult>().Model.As<AllTowns>();
            model.Towns.Should().HaveCount(TotalTowns);
            model.Pagination.SearchTerm.Should().Be(string.Empty);
            model.Pagination.CurrentPage.Should().Be(MinPageSize);
            model.Pagination.NextPage.Should().Be(MaxPageSize);
            model.Pagination.PreviousPage.Should().Be(MinPageSize);
            model.Pagination.TotalElements.Should().Be(TotalTowns);
        }

        [Fact]
        public void All_ShouldReturnCorrectDataWithValidInputAndParticularSearchTerm()
        {
            const string SearchTerm = "Test";

            //Arrange
            var controller = new AdminTownsController(this.townService.Object);

            this.townService.Setup(t => t.All(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                .Returns(this.GetTowns());

            this.townService.Setup(t => t.TotalTowns(It.IsAny<string>()))
                .Returns(TotalTowns);

            //Act
            var result = controller.All(SearchTerm, MinPageSize);

            //Assert
            result.Should().BeOfType<ViewResult>();
            var model = result.As<ViewResult>().Model.As<AllTowns>();
            model.Towns.Should().HaveCount(TotalTowns);
            model.Pagination.SearchTerm.Should().Be(SearchTerm);
            model.Pagination.CurrentPage.Should().Be(MinPageSize);
            model.Pagination.NextPage.Should().Be(MaxPageSize);
            model.Pagination.PreviousPage.Should().Be(MinPageSize);
            model.Pagination.TotalElements.Should().Be(TotalTowns);
        }

        [Fact]
        public void TownStations_ShouldRedirectToAllTownsForNonExistingTown()
        {
            //Arrange
            var controller = new AdminTownsController(this.townService.Object);

            this.townService.Setup(t => t.TownExists(It.IsAny<int>()))
                .Returns(false);

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = controller.TownStations(TownId);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var model = result.As<RedirectToActionResult>();
            model.ActionName.Should().Be(WebConstants.Action.AdminAllTowns);
            this.customMessage.Should().Be(string.Format(WebConstants.Message.NonExistingEntity, nameof(WebConstants.Entity.Town), TownId));
        }

        [Fact]
        public void TownStations_WithExistingTownAndNoStationsShouldReturnCountZero()
        {
            //Arrange
            var controller = new AdminTownsController(this.townService.Object);

            this.townService.Setup(t => t.TownExists(It.IsAny<int>()))
                .Returns(true);

            this.townService.Setup(t => t.TownStations(It.IsAny<int>()))
                .Returns(new List<AdminTownStationsServiceModel>());

            //Act
            var result = controller.TownStations(TownId);

            //Assert
            result.Should().BeOfType<JsonResult>();
            var model = result.As<JsonResult>().Value;
            model.As<IEnumerable<AdminTownStationsServiceModel>>().Should().HaveCount(0);
        }

        [Fact]
        public void TownStations_WithValidDataShouldReturnCorrectResults()
        {
            const int TownStationsCount = 5;

            //Arrange
            var controller = new AdminTownsController(this.townService.Object);

            this.townService.Setup(t => t.TownExists(It.IsAny<int>()))
                .Returns(true);

            this.townService.Setup(t => t.TownStations(It.IsAny<int>()))
                .Returns(this.GetTownStations());

            //Act
            var result = controller.TownStations(TownId);

            //Assert
            result.Should().BeOfType<JsonResult>();
            var model = result.As<JsonResult>().Value;
            model.As<IEnumerable<AdminTownStationsServiceModel>>().Should().HaveCount(TownStationsCount);
        }

        [Fact]
        public void Get_AddTownShouldReturnAddTownForm()
        {
            //Arrange
            var controller = new AdminTownsController(this.townService.Object);

            //Act
            var result = controller.Add();

            //Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public void Post_AddTownShouldReturnAddTownFormForInvalidModelState()
        {
            //Arrange
            var controller = new AdminTownsController(this.townService.Object);
            controller.ModelState.AddModelError(string.Empty, "Error");

            //Act
            var result = controller.Add(this.GetAddTownForm());

            //Assert
            result.Should().BeOfType<ViewResult>();
            var model = result.As<ViewResult>().Model;
            model.Should().BeOfType<AddTownFormModel>();
            var form = model.As<AddTownFormModel>();
            form.Name.Should().Be(TownName);
        }

        [Fact]
        public void Post_AddTownShouldReturnAddTownFormForAlreadyExistingTown()
        {
            //Arrange
            var controller = new AdminTownsController(this.townService.Object);

            this.townService.Setup(t => t.TownExistsByName(It.IsAny<string>()))
                .Returns(true);

            //Act
            var result = controller.Add(this.GetAddTownForm());

            //Assert
            result.Should().BeOfType<ViewResult>();
            var errors = controller.ModelState.Root.Errors;
            errors.Any(e=>e.ErrorMessage == string.Format(WebConstants.Message.EntityAlreadyExist, nameof(WebConstants.Entity.Town)));
            var model = result.As<ViewResult>().Model;
            model.Should().BeOfType<AddTownFormModel>();
            var form = model.As<AddTownFormModel>();
            form.Name.Should().Be(TownName);
        }

        [Fact]
        public void Post_AddTownShouldRedirectToAllTownsWithCorrectData()
        {
            //Arrange
            var controller = new AdminTownsController(this.townService.Object);

            this.townService.Setup(t => t.TownExistsByName(It.IsAny<string>()))
                .Returns(false);

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = controller.Add(this.GetAddTownForm());

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();       
            this.customMessage.Should().Be(string.Format(WebConstants.Message.TownAdded, TownName));
        }

        private IEnumerable<AdminTownListingServiceModel> GetTowns()
        {
            var list = new List<AdminTownListingServiceModel>();

            for (int i = 0; i < TotalTowns; i++)
            {
                list.Add(new AdminTownListingServiceModel()
                {
                    Id = i,
                    Name = $"Town {i}"
                });
            }

            return list;
        }

        private IEnumerable<AdminTownStationsServiceModel> GetTownStations()
        {
            var list = new List<AdminTownStationsServiceModel>();

            for (int i = 0; i < 5; i++)
            {
                list.Add(new AdminTownStationsServiceModel()
                {
                    Id = i,
                    Name = $"Station {i}",
                    ArrivingRoutes = i * 2,
                    DepartingRoutes = i * 3
                });
            }

            return list;
        }

        private AddTownFormModel GetAddTownForm() => 
            new AddTownFormModel()
            {
                Name = TownName
            };
    }
}
