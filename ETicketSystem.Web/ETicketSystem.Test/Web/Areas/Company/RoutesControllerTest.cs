namespace ETicketSystem.Test.Web.Areas.Company
{
    using Controllers.Fixtures;
    using ETicketSystem.Common.Constants;
    using ETicketSystem.Common.Enums;
    using ETicketSystem.Data.Enums;
    using ETicketSystem.Services.Company.Contracts;
    using ETicketSystem.Services.Company.Models;
    using ETicketSystem.Services.Contracts;
    using ETicketSystem.Services.Models.Station;
    using ETicketSystem.Services.Models.Town;
    using ETicketSystem.Web.Areas.Company.Controllers;
    using ETicketSystem.Web.Areas.Company.Models.Routes;
    using ETicketSystem.Web.Infrastructure.Extensions;
    using FluentAssertions;
    using Infrastructure;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Mocks;
    using Mocks.Company;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;

    using static Common.TestConstants;

    public class RoutesControllerTest : BaseControllerTest, IClassFixture<UserManagerGetUserIdFixture>
    {
        private const int StartTown = 1;

        private const int EndTown = 20;

        private const int MaxPageSize = 2;

        private const int TotalRoutes = 30;

        private const int TownsCount = 10;

        private static DateTime Date = new DateTime(2018,1,1);

        private static DateTime DepartureDateTime = new DateTime(2018, 1, 1, 1, 1, 1);

        private static TimeSpan DepartureTime = new TimeSpan(11, 30, 00);

        private static TimeSpan Duration = new TimeSpan(1, 30, 0);

        private const int StartStation = 10;

        private const int EndStation = 20;

        private const decimal Price = 19.99m;

        private const string StartTownName = "Sofia";

        private const string EndTownName = "Varna";

        private const string StartStationName = "Central Sofia Station";

        private const string EndStationName = "Central Varna Station";

        private const string RouteStatus = "activated";

        private readonly UserManagerGetUserIdFixture fixture;

        private readonly Mock<ICompanyRouteService> routeService = CompanyRouteServiceMock.New;

        private readonly Mock<ICompanyService> companyService = CompanyServiceMock.New;

        private readonly Mock<IStationService> stationService = StationServiceMock.New;

        private readonly Mock<ITownService> townService = TownServiceMock.New;

        public RoutesControllerTest(UserManagerGetUserIdFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void ControllerShouldBeForAuthorizedUsersOnly()
        {
            //Arrange
            var controller = new RoutesController(null,null,null,null,null);

            //Act
            var attributes = controller.GetType().GetCustomAttributes(true);

            //Assert
            attributes.Any(a => a.GetType() == typeof(AuthorizeAttribute));
        }

        [Fact]
        public void ControllerShouldBeForUsersInRoleCompany()
        {
            //Arrange
            var controller = new RoutesController(null, null, null, null, null);

            //Act
            var attributes = controller.GetType().GetCustomAttributes(true);
            var authorizeAttribute = attributes.FirstOrDefault(a => a.GetType() == typeof(AuthorizeAttribute));

            //Assert
            authorizeAttribute.As<AuthorizeAttribute>().Roles.Should().Be(Role.Company.ToString());
        }

        [Fact]
        public void ControllerShouldBeInCompanyArea()
        {
            //Arrange
            var controller = new RoutesController(null, null, null, null, null);

            //Act
            var attributes = controller.GetType().GetCustomAttributes(true);
            var areaAttribute = attributes.FirstOrDefault(a => a.GetType() == typeof(AreaAttribute));

            //Assert
            areaAttribute.As<AreaAttribute>().RouteValue.Should().Be(WebConstants.Area.Company);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void All_WithPageLessOrEqualToZeroShouldRedirectToAllCompanyRoutes(int page)
        {
            //Arrange
            var controller = new RoutesController(null,null,null,null,null);

            //Act
            var result = controller.All(StartTown,EndTown,Date, page);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var model = result.As<RedirectToActionResult>();
            model.ActionName.Should().Be(WebConstants.Action.CompanyAllRoutes);
            model.RouteValues.Keys.Should().Contain(RouteValueKeyPage);
            model.RouteValues.Values.Should().Contain(MinPageSize);
            model.RouteValues.Keys.Should().Contain(RouteValueStartTownKey);
            model.RouteValues.Values.Should().Contain(StartTown);
            model.RouteValues.Keys.Should().Contain(RouteValueEndTownKey);
            model.RouteValues.Values.Should().Contain(EndTown);
            model.RouteValues.Keys.Should().Contain(RouteValueDateKey);
            model.RouteValues.Values.Should().Contain(Date.ToYearMonthDayFormat());
        }

        [Fact]
        public void All_ValidPageAndNoRoutesShouldReturnCountZero()
        {
            //Arrange
            var controller = new RoutesController(this.routeService.Object, this.townService.Object, this.fixture.UserManagerMockInstance.Object, null, null);

            this.routeService.Setup(t => t.All(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(),It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(new CompanyRoutesServiceModel() { Routes = new List<CompanyRouteListingServiceModel>()});

            this.routeService.Setup(t => t.TotalRoutes(It.IsAny<int>(),It.IsAny<int>(),It.IsAny<DateTime>(),It.IsAny<string>()))
                .Returns(0);

            this.townService.Setup(t => t.GetTownsListItems())
                .Returns(this.GetTowns());

            this.townService.Setup(t => t.GetTownsWithStations())
                .Returns(this.GetTownsStations());

            //Act
            var result = controller.All(StartTown, EndTown, Date, MinPageSize);

            //Assert
            result.Should().BeOfType<ViewResult>();
            var model = result.As<ViewResult>().Model.As<AllRoutes>();
            model.Routes.Should().HaveCount(0);
            model.Pagination.CurrentPage.Should().Be(MinPageSize);
            model.Pagination.NextPage.Should().Be(0);
            model.Pagination.PreviousPage.Should().Be(MinPageSize);
            model.Pagination.TotalElements.Should().Be(0);
        }

        [Fact]
        public void All_PageGreaterThanTotalPagesShouldRedirectToAllCompanyRoutes()
        {
            const int InvalidPageSize = 10;

            //Arrange
            var controller = new RoutesController(this.routeService.Object, this.townService.Object, this.fixture.UserManagerMockInstance.Object, null, null);

            this.routeService.Setup(t => t.All(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(this.GetRoutes());

            this.routeService.Setup(t => t.TotalRoutes(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                .Returns(TotalRoutes);

            this.townService.Setup(t => t.GetTownsListItems())
                .Returns(this.GetTowns());

            this.townService.Setup(t => t.GetTownsWithStations())
                .Returns(this.GetTownsStations());

            //Act
            var result = controller.All(StartTown, EndTown, Date, InvalidPageSize);

            //Assert
            result.Should().BeOfType<RedirectToActionResult>();
            var model = result.As<RedirectToActionResult>();
            model.ActionName.Should().Be(WebConstants.Action.CompanyAllRoutes);
            model.RouteValues.Keys.Should().Contain(RouteValueKeyPage);
            model.RouteValues.Values.Should().Contain(MaxPageSize);
            model.RouteValues.Keys.Should().Contain(RouteValueStartTownKey);
            model.RouteValues.Values.Should().Contain(StartTown);
            model.RouteValues.Keys.Should().Contain(RouteValueEndTownKey);
            model.RouteValues.Values.Should().Contain(EndTown);
            model.RouteValues.Keys.Should().Contain(RouteValueDateKey);
            model.RouteValues.Values.Should().Contain(Date.ToYearMonthDayFormat());
        }

        [Fact]
        public void All_ShouldReturnCorrectDataWithValidInput()
        {
            //Arrange
            var controller = new RoutesController(this.routeService.Object, this.townService.Object, this.fixture.UserManagerMockInstance.Object, null, null);

            this.routeService.Setup(t => t.All(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(this.GetRoutes());

            this.routeService.Setup(t => t.TotalRoutes(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                .Returns(TotalRoutes);

            this.townService.Setup(t => t.GetTownsListItems())
                .Returns(this.GetTowns());

            this.townService.Setup(t => t.GetTownsWithStations())
                .Returns(this.GetTownsStations());

            //Act
            var result = controller.All(StartTown, EndTown, Date, MinPageSize);

            //Assert
            result.Should().BeOfType<ViewResult>();
            var model = result.As<ViewResult>().Model.As<AllRoutes>();
            model.Routes.Should().HaveCount(TotalRoutes);
            model.Pagination.CurrentPage.Should().Be(MinPageSize);
            model.Pagination.NextPage.Should().Be(MaxPageSize);
            model.Pagination.PreviousPage.Should().Be(MinPageSize);
            model.Pagination.TotalElements.Should().Be(TotalRoutes);
        }

        [Fact]
        public void Get_Add_WithNotApprovedCompanyShouldRedirectToAllCompanyRoutes()
        {
            //Arrange
            var controller = new RoutesController(null, null, this.fixture.UserManagerMockInstance.Object, this.companyService.Object, null);

            this.companyService.Setup(c => c.IsApproved(It.IsAny<string>()))
                .Returns(false);

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = controller.Add();

            //Assert
            this.AssertRedirectToCompanyRoutes(result);
            this.customMessage.Should().Be(WebConstants.Message.CompanyNotApproved);
        }

        [Fact]
        public void Get_Add_WithBlockedCompanyShouldRedirectToAllCompanyRoutes()
        {
            //Arrange
            var controller = new RoutesController(null, null, this.fixture.UserManagerMockInstance.Object, this.companyService.Object, null);

            this.companyService.Setup(c => c.IsApproved(It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsBlocked(It.IsAny<string>()))
                .Returns(true);

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = controller.Add();

            //Assert
            this.AssertRedirectToCompanyRoutes(result);
            this.customMessage.Should().Be(WebConstants.Message.AddRouteCompanyBlocked);
        }

        [Fact]
        public void Get_Add_WithValidDataShouldReturnAddRoutesFormView()
        {
            //Arrange
            var controller = new RoutesController(null, this.townService.Object, this.fixture.UserManagerMockInstance.Object, this.companyService.Object, null);

            this.companyService.Setup(c => c.IsApproved(It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsBlocked(It.IsAny<string>()))
                .Returns(false);

            this.townService.Setup(t => t.GetTownsWithStations())
                .Returns(this.GetTownsStations());

            //Act
            var result = controller.Add();

            //Assert
            result.Should().BeOfType<ViewResult>();
            var model = result.As<ViewResult>().Model;
            model.Should().BeOfType<RouteFormModel>();
            var form = model.As<RouteFormModel>();
            form.TownsStations.Should().HaveCount(TownsCount);
        }

        [Fact]
        public void Post_Add_WithNotApprovedCompanyShouldRedirectToAllRoutes()
        {
            //Arrange
            var controller = new RoutesController(null, null, this.fixture.UserManagerMockInstance.Object, this.companyService.Object, null);

            this.companyService.Setup(c => c.IsApproved(It.IsAny<string>()))
                .Returns(false);

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = controller.Add();

            //Assert
            this.AssertRedirectToCompanyRoutes(result);
            this.customMessage.Should().Be(WebConstants.Message.CompanyNotApproved);
        }

        [Fact]
        public void Post_Add_WithBlockedCompanyShouldRedirectToAllRoutes()
        {
            //Arrange
            var controller = new RoutesController(null, null, this.fixture.UserManagerMockInstance.Object, this.companyService.Object, null);

            this.companyService.Setup(c => c.IsApproved(It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsBlocked(It.IsAny<string>()))
                .Returns(true);

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = controller.Add(this.GetRouteFormModel());

            //Assert
            this.AssertRedirectToCompanyRoutes(result);
            this.customMessage.Should().Be(WebConstants.Message.AddRouteCompanyBlocked);
        }

        [Fact]
        public void Post_Add_WithNonExistingStartStationShouldRedirectToAllRoutes()
        {
            //Arrange
            var controller = new RoutesController(null, null, this.fixture.UserManagerMockInstance.Object, this.companyService.Object, this.stationService.Object);

            this.companyService.Setup(c => c.IsApproved(It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsBlocked(It.IsAny<string>()))
                .Returns(false);

            this.stationService.Setup(s => s.StationExist(It.IsAny<int>()))
                .Returns(false);

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = controller.Add(this.GetRouteFormModel());

            //Assert
            this.AssertRedirectToCompanyRoutes(result);
            this.customMessage.Should().Be(WebConstants.Message.InvalidStation);
        }

        [Fact]
        public void Post_Add_WithNonExistingEndStationShouldRedirectToAllRoutes()
        {
            //Arrange
            var controller = new RoutesController(null, null, this.fixture.UserManagerMockInstance.Object, this.companyService.Object, this.stationService.Object);

            this.companyService.Setup(c => c.IsApproved(It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsBlocked(It.IsAny<string>()))
                .Returns(false);

            this.stationService.Setup(s => s.StationExist(It.IsAny<int>()))
                .Returns(false);

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = controller.Add(this.GetRouteFormModel());

            //Assert
            this.AssertRedirectToCompanyRoutes(result);
            this.customMessage.Should().Be(WebConstants.Message.InvalidStation);
        }

        [Fact]
        public void Post_Add_WithEqualStartAndEndStationsShouldReturnRouteAddFormView()
        {
            const int WrongStartStation = 15;

            //Arrange
            var form = this.GetRouteFormModel();
            form.StartStation = WrongStartStation;
            form.EndStation = WrongStartStation;

            var controller = new RoutesController(null, this.townService.Object, this.fixture.UserManagerMockInstance.Object, this.companyService.Object, this.stationService.Object);

            this.companyService.Setup(c => c.IsApproved(It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsBlocked(It.IsAny<string>()))
                .Returns(false);

            this.stationService.Setup(s => s.StationExist(It.IsAny<int>()))
                .Returns(true);

            this.townService.Setup(t => t.GetTownsWithStations())
                .Returns(this.GetTownsStations());

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = controller.Add(form);

            //Assert
            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().ViewData.ModelState.Root.Errors.Should().HaveCount(1);
            result.As<ViewResult>().ViewData.ModelState.Root.Errors.Any(e => e.ErrorMessage == WebConstants.Message.StartStationEqualToEndStation);
            var viewResult = result.As<ViewResult>().Model;
            this.AssertRouteFormModelProperties(WrongStartStation,WrongStartStation, form, viewResult);
        }

        [Fact]
        public void Post_Add_WithInvalidModelStateShouldReturnRouteAddFormView()
        {
            //Arrange
            var controller = new RoutesController(null, this.townService.Object, this.fixture.UserManagerMockInstance.Object, this.companyService.Object, this.stationService.Object);

            this.companyService.Setup(c => c.IsApproved(It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsBlocked(It.IsAny<string>()))
                .Returns(false);

            this.stationService.Setup(s => s.StationExist(It.IsAny<int>()))
                .Returns(true);

            this.townService.Setup(t => t.GetTownsWithStations())
                .Returns(this.GetTownsStations());

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            controller.ModelState.AddModelError(string.Empty, "Error");

            //Act
            var result = controller.Add(this.GetRouteFormModel());

            //Assert
            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().ViewData.ModelState.Root.Errors.Should().HaveCount(1);
            result.As<ViewResult>().ViewData.ModelState.Root.Errors.Any(e => e.ErrorMessage == WebConstants.Message.StartStationEqualToEndStation);
            var viewResult = result.As<ViewResult>().Model;
            viewResult.Should().BeOfType<RouteFormModel>();
            var form = viewResult.As<RouteFormModel>();
            this.AssertRouteFormModelProperties(StartStation, EndStation, form, viewResult);
            form.IsEdit.Should().BeFalse();
        }

        [Fact]
        public void Post_Add_WithInvalidModelStateDuplicatedRouteShouldReturnRouteAddFormView()
        {
            //Arrange
            var controller = new RoutesController(this.routeService.Object, this.townService.Object, this.fixture.UserManagerMockInstance.Object, this.companyService.Object, this.stationService.Object);

            this.companyService.Setup(c => c.IsApproved(It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsBlocked(It.IsAny<string>()))
                .Returns(false);

            this.stationService.Setup(s => s.StationExist(It.IsAny<int>()))
                .Returns(true);

            this.routeService.Setup(r => r.Add(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>(), It.IsAny<BusType>(), It.IsAny<decimal>(), It.IsAny<string>()))
                .Returns(false);

            this.townService.Setup(t => t.GetTownsWithStations())
                .Returns(this.GetTownsStations());

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            controller.ModelState.AddModelError(string.Empty, "Error");

            //Act
            var result = controller.Add(this.GetRouteFormModel());

            //Assert
            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().ViewData.ModelState.Root.Errors.Should().HaveCount(1);
            result.As<ViewResult>().ViewData.ModelState.Root.Errors.Any(e => e.ErrorMessage == WebConstants.Message.CompanyRouteDuplication);
            var viewResult = result.As<ViewResult>().Model;
            viewResult.Should().BeOfType<RouteFormModel>();
            var form = viewResult.As<RouteFormModel>();
            this.AssertRouteFormModelProperties(StartStation, EndStation, form, viewResult);
            form.IsEdit.Should().BeFalse();
        }

        [Fact]
        public void Post_Add_WithValidDataShouldRedirectToAllRoutes()
        {
            //Arrange
            var controller = new RoutesController(this.routeService.Object, this.townService.Object, this.fixture.UserManagerMockInstance.Object, this.companyService.Object, this.stationService.Object);

            this.companyService.Setup(c => c.IsApproved(It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsBlocked(It.IsAny<string>()))
                .Returns(false);

            this.stationService.Setup(s => s.StationExist(It.IsAny<int>()))
                .Returns(true);

            this.routeService.Setup(r => r.Add(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>(), It.IsAny<BusType>(), It.IsAny<decimal>(), It.IsAny<string>()))
                .Returns(true);

            this.townService.Setup(t => t.GetTownsWithStations())
                .Returns(this.GetTownsStations());

            this.townService.Setup(t => t.GetTownNameByStationId(It.IsAny<int>()))
                .Returns(StartTownName);

            this.townService.SetupSequence(t => t.GetTownNameByStationId(It.IsAny<int>()))
                .Returns(StartTownName)
                .Returns(EndTownName);

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = controller.Add(this.GetRouteFormModel());

            //Assert
            this.AssertRedirectToCompanyRoutes(result);
            this.customMessage.Should().Be(string.Format(WebConstants.Message.RouteAdded, StartTownName, EndTownName));
        }

        [Fact]
        public void Get_Edit_WhenCompanyIsNotRouteOwnerShouldRedirectToAllRoutes()
        {
            //Arrange
            var controller = new RoutesController(this.routeService.Object, null, this.fixture.UserManagerMockInstance.Object, null, null);

            this.routeService.Setup(r => r.IsRouteOwner(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(false);

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = controller.Edit(RouteId);

            //Assert
            this.AssertRedirectToCompanyRoutes(result);
            this.customMessage.Should().Be(WebConstants.Message.InvalidRoute);
        }

        [Fact]
        public void Get_Edit_WhenCompanyIsNotApprovedShouldRedirectToAllRoutes()
        {
            //Arrange
            var controller = new RoutesController(this.routeService.Object, null, this.fixture.UserManagerMockInstance.Object, this.companyService.Object, null);

            this.routeService.Setup(r => r.IsRouteOwner(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsApproved(It.IsAny<string>()))
                .Returns(false);

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = controller.Edit(RouteId);

            //Assert
            this.AssertRedirectToCompanyRoutes(result);
            this.customMessage.Should().Be(WebConstants.Message.ChangeRouteCompanyNotApproved);
        }

        [Fact]
        public void Get_Edit_WhenCompanyIsBlockedShouldRedirectToAllRoutes()
        {
            //Arrange
            var controller = new RoutesController(this.routeService.Object, null, this.fixture.UserManagerMockInstance.Object, this.companyService.Object, null);

            this.routeService.Setup(r => r.IsRouteOwner(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsApproved(It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsBlocked(It.IsAny<string>()))
                .Returns(true);

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = controller.Edit(RouteId);

            //Assert
            this.AssertRedirectToCompanyRoutes(result);
            this.customMessage.Should().Be(WebConstants.Message.ChangeRouteCompanyBlocked);
        }

        [Fact]
        public void Get_Edit_WhenRouteDoesNotExistShouldRedirectToAllRoutes()
        {
            //Arrange
            CompanyRouteEditServiceModel route = null;
            var controller = new RoutesController(this.routeService.Object, null, this.fixture.UserManagerMockInstance.Object, this.companyService.Object, null);

            this.routeService.Setup(r => r.IsRouteOwner(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsApproved(It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsBlocked(It.IsAny<string>()))
                .Returns(false);

            this.routeService.Setup(r => r.GetRouteToEdit(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(route);

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = controller.Edit(RouteId);

            //Assert
            this.AssertRedirectToCompanyRoutes(result);
            this.customMessage.Should().Be(WebConstants.Message.InvalidRoute);
        }

        [Fact]
        public void Get_Edit_WhenRouteHasReservedTicketsShouldRedirectToAllRoutes()
        {
            //Arrange
            var controller = new RoutesController(this.routeService.Object, this.townService.Object, this.fixture.UserManagerMockInstance.Object, this.companyService.Object, null);

            this.routeService.Setup(r => r.IsRouteOwner(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsApproved(It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsBlocked(It.IsAny<string>()))
                .Returns(false);

            this.routeService.Setup(r => r.GetRouteToEdit(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(this.GetRouteToEdit());

            this.townService.SetupSequence(t => t.GetTownNameByStationId(It.IsAny<int>()))
                .Returns(StartTownName)
                .Returns(EndTownName);

            this.routeService.Setup(r => r.HasReservedTickets(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(true);

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = controller.Edit(RouteId);

            //Assert
            this.AssertRedirectToCompanyRoutes(result);
            this.customMessage.Should().Be(string.Format(WebConstants.Message.EditRouteWithTickets, StartTownName, EndTownName));
        }

        [Fact]
        public void Get_Edit_WithValidDataShouldReturnEditRouteFormView()
        {
            //Arrange
            var controller = new RoutesController(this.routeService.Object, this.townService.Object, this.fixture.UserManagerMockInstance.Object, this.companyService.Object, null);

            this.routeService.Setup(r => r.IsRouteOwner(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsApproved(It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsBlocked(It.IsAny<string>()))
                .Returns(false);

            this.routeService.Setup(r => r.GetRouteToEdit(It.IsAny<string>(), It.IsAny<int>()))
                .Returns(this.GetRouteToEdit());

            this.townService.SetupSequence(t => t.GetTownNameByStationId(It.IsAny<int>()))
                .Returns(StartTownName)
                .Returns(EndTownName);

            this.routeService.Setup(r => r.HasReservedTickets(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(false);

            //Act
            var result = controller.Edit(RouteId);

            //Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result.As<ViewResult>().Model;
            var form = viewResult.As<RouteFormModel>();
            this.AssertRouteFormModelProperties(StartStation, EndStation, form, viewResult);
            form.IsEdit.Should().BeTrue();
        }

        [Fact]
        public void Post_Edit_WhenCompanyIsNotRouteOwnerShouldRedirectToAllRoutes()
        {
            //Arrange
            var controller = new RoutesController(this.routeService.Object, null, this.fixture.UserManagerMockInstance.Object, null, null);

            this.routeService.Setup(r => r.IsRouteOwner(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(false);

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = controller.Edit(null,RouteId);

            //Assert
            this.AssertRedirectToCompanyRoutes(result);
            this.customMessage.Should().Be(WebConstants.Message.InvalidRoute);
        }

        [Fact]
        public void Post_Edit_WhenCompanyIsNotApprovedShouldRedirectToAllRoutes()
        {
            //Arrange
            var controller = new RoutesController(this.routeService.Object, null, this.fixture.UserManagerMockInstance.Object, this.companyService.Object, null);

            this.routeService.Setup(r => r.IsRouteOwner(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsApproved(It.IsAny<string>()))
                .Returns(false);

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = controller.Edit(null,RouteId);

            //Assert
            this.AssertRedirectToCompanyRoutes(result);
            this.customMessage.Should().Be(WebConstants.Message.ChangeRouteCompanyNotApproved);
        }

        [Fact]
        public void Post_Edit_WhenCompanyIsBlockedShouldRedirectToAllRoutes()
        {
            //Arrange
            var controller = new RoutesController(this.routeService.Object, null, this.fixture.UserManagerMockInstance.Object, this.companyService.Object, null);

            this.routeService.Setup(r => r.IsRouteOwner(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsApproved(It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsBlocked(It.IsAny<string>()))
                .Returns(true);

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = controller.Edit(null,RouteId);

            //Assert
            this.AssertRedirectToCompanyRoutes(result);
            this.customMessage.Should().Be(WebConstants.Message.ChangeRouteCompanyBlocked);
        }

        [Fact]
        public void Post_Edit_WithInvalidModelStateShouldReturnEditRouteFormView()
        {
            //Arrange
            var controller = new RoutesController(this.routeService.Object, this.townService.Object, this.fixture.UserManagerMockInstance.Object, this.companyService.Object, null);

            this.routeService.Setup(r => r.IsRouteOwner(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsApproved(It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsBlocked(It.IsAny<string>()))
                .Returns(false);

            this.townService.Setup(t => t.GetTownsWithStations())
                .Returns(this.GetTownsStations());

            controller.ModelState.AddModelError(string.Empty, "Error");

            //Act
            var result = controller.Edit(this.GetRouteFormModel(), RouteId);

            //Assert
            result.Should().BeOfType<ViewResult>();
            var viewResult = result.As<ViewResult>().Model;
            var form = result.As<ViewResult>().Model.As<RouteFormModel>();
            this.AssertRouteFormModelProperties(StartStation, EndStation, form, viewResult);
        }

        [Fact]
        public void Post_Edit_WhenThereIsAlreadySameRouteShouldReturnEditRouteFormView()
        {
            //Arrange
            var controller = new RoutesController(this.routeService.Object, this.townService.Object, this.fixture.UserManagerMockInstance.Object, this.companyService.Object, null);

            this.routeService.Setup(r => r.IsRouteOwner(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsApproved(It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsBlocked(It.IsAny<string>()))
                .Returns(false);

            this.townService.Setup(t => t.GetTownsWithStations())
                .Returns(this.GetTownsStations());

            this.routeService.Setup(r => r.RouteAlreadyExist(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<TimeSpan>(), It.IsAny<string>()))
                .Returns(true);

            //Act
            var result = controller.Edit(this.GetRouteFormModel(), RouteId);

            //Assert
            result.Should().BeOfType<ViewResult>();
            result.As<ViewResult>().ViewData.ModelState.Root.Errors.Should().HaveCount(1);
            result.As<ViewResult>().ViewData.ModelState.Root.Errors.Any(e=>e.ErrorMessage == WebConstants.Message.CompanyRouteDuplication);
            var viewResult = result.As<ViewResult>().Model;
            var form = viewResult.As<RouteFormModel>();
            this.AssertRouteFormModelProperties(StartStation, EndStation, form, viewResult);
            form.IsEdit.Should().BeTrue();
        }

        [Fact]
        public void Post_Edit_WhenRouteHasAlreadyReservedTicketsShouldRedirectToAllRoutes()
        {
            //Arrange
            var controller = new RoutesController(this.routeService.Object, this.townService.Object, this.fixture.UserManagerMockInstance.Object, this.companyService.Object, null);

            this.routeService.Setup(r => r.IsRouteOwner(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsApproved(It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsBlocked(It.IsAny<string>()))
                .Returns(false);

            this.townService.Setup(t => t.GetTownsWithStations())
                .Returns(this.GetTownsStations());

            this.routeService.Setup(r => r.RouteAlreadyExist(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<TimeSpan>(), It.IsAny<string>()))
                .Returns(false);

            this.townService.SetupSequence(t => t.GetTownNameByStationId(It.IsAny<int>()))
                .Returns(StartTownName)
                .Returns(EndTownName);

            this.routeService.Setup(r => r.HasReservedTickets(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(true);

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = controller.Edit(this.GetRouteFormModel(), RouteId);

            //Assert
            this.AssertRedirectToCompanyRoutes(result);
            this.customMessage.Should().Be(string.Format(WebConstants.Message.EditRouteWithTickets, StartTownName, EndTownName));
        }

        [Fact]
        public void Post_Edit_WithInvalidRouteShouldRedirectToAllRoutes()
        {
            //Arrange
            var controller = new RoutesController(this.routeService.Object, this.townService.Object, this.fixture.UserManagerMockInstance.Object, this.companyService.Object, null);

            this.routeService.Setup(r => r.IsRouteOwner(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsApproved(It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsBlocked(It.IsAny<string>()))
                .Returns(false);

            this.townService.Setup(t => t.GetTownsWithStations())
                .Returns(this.GetTownsStations());

            this.routeService.Setup(r => r.RouteAlreadyExist(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<TimeSpan>(), It.IsAny<string>()))
                .Returns(false);

            this.townService.SetupSequence(t => t.GetTownNameByStationId(It.IsAny<int>()))
                .Returns(StartTownName)
                .Returns(EndTownName);

            this.routeService.Setup(r => r.HasReservedTickets(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(false);

            this.routeService.Setup(r => r.Edit(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>(), It.IsAny<BusType>(), It.IsAny<decimal>(), It.IsAny<string>()))
                .Returns(false);

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = controller.Edit(this.GetRouteFormModel(), RouteId);

            //Assert
            this.AssertRedirectToCompanyRoutes(result);
            this.customMessage.Should().Be(WebConstants.Message.InvalidRoute);
        }

        [Fact]
        public void Post_Edit_WithValidDataShouldRedirectToAllRoutes()
        {
            //Arrange
            var controller = new RoutesController(this.routeService.Object, this.townService.Object, this.fixture.UserManagerMockInstance.Object, this.companyService.Object, null);

            this.routeService.Setup(r => r.IsRouteOwner(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsApproved(It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsBlocked(It.IsAny<string>()))
                .Returns(false);

            this.townService.Setup(t => t.GetTownsWithStations())
                .Returns(this.GetTownsStations());

            this.routeService.Setup(r => r.RouteAlreadyExist(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<TimeSpan>(), It.IsAny<string>()))
                .Returns(false);

            this.townService.SetupSequence(t => t.GetTownNameByStationId(It.IsAny<int>()))
                .Returns(StartTownName)
                .Returns(EndTownName);

            this.routeService.Setup(r => r.HasReservedTickets(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(false);

            this.routeService.Setup(r => r.Edit(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<TimeSpan>(), It.IsAny<TimeSpan>(), It.IsAny<BusType>(), It.IsAny<decimal>(), It.IsAny<string>()))
                .Returns(true);

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = controller.Edit(this.GetRouteFormModel(), RouteId);

            //Assert
            this.AssertRedirectToCompanyRoutes(result);
            this.customMessage.Should().Be(string.Format(WebConstants.Message.RouteEdited, StartTownName, EndTownName));
        }

        [Fact]
        public void Post_ChangeStatus_WhenCompanyIsNotRouteOwnerShouldRedirectToAllRoutes()
        {
            //Arrange
            var controller = new RoutesController(this.routeService.Object, null, this.fixture.UserManagerMockInstance.Object, null, null);

            this.routeService.Setup(r => r.IsRouteOwner(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(false);

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = controller.ChangeStatus(RouteId);

            //Assert
            this.AssertRedirectToCompanyRoutes(result);
            this.customMessage.Should().Be(WebConstants.Message.InvalidRoute);
        }

        [Fact]
        public void Post_ChangeStatus_WhenCompanyIsNotApprovedShouldRedirectToAllRoutes()
        {
            //Arrange
            var controller = new RoutesController(this.routeService.Object, null, this.fixture.UserManagerMockInstance.Object, this.companyService.Object, null);

            this.routeService.Setup(r => r.IsRouteOwner(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsApproved(It.IsAny<string>()))
                .Returns(false);

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = controller.ChangeStatus(RouteId);

            //Assert
            this.AssertRedirectToCompanyRoutes(result);
            this.customMessage.Should().Be(WebConstants.Message.ChangeRouteCompanyNotApproved);
        }

        [Fact]
        public void Post_ChangeStatus_WhenCompanyIsBlockedShouldRedirectToAllRoutes()
        {
            //Arrange
            var controller = new RoutesController(this.routeService.Object, null, this.fixture.UserManagerMockInstance.Object, this.companyService.Object, null);

            this.routeService.Setup(r => r.IsRouteOwner(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsApproved(It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsBlocked(It.IsAny<string>()))
                .Returns(true);

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = controller.ChangeStatus(RouteId);

            //Assert
            this.AssertRedirectToCompanyRoutes(result);
            this.customMessage.Should().Be(WebConstants.Message.ChangeRouteCompanyBlocked);
        }

        [Fact]
        public void Post_ChangeStatus_DeactivateRouteWhenRouteHasAlreadyReservedTicketsShouldRedirectToAllRoutes()
        {
            //Arrange
            var controller = new RoutesController(this.routeService.Object, null, this.fixture.UserManagerMockInstance.Object, this.companyService.Object, null);

            this.routeService.Setup(r => r.IsRouteOwner(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsApproved(It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsBlocked(It.IsAny<string>()))
                .Returns(false);

            this.routeService.Setup(r => r.GetRouteBaseInfo(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(this.GetRouteInfo());

            this.routeService.Setup(r => r.HasReservedTickets(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(true);

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = controller.ChangeStatus(RouteId);

            //Assert
            this.AssertRedirectToCompanyRoutes(result);
            this.customMessage.Should().Be(string.Format(WebConstants.Message.DeactivateRouteWithTickets, StartStationName, EndStationName));
        }

        [Fact]
        public void Post_ChangeStatus_WithInvalidRouteShouldRedirectToAllRoutes()
        {
            //Arrange
            var controller = new RoutesController(this.routeService.Object, null, this.fixture.UserManagerMockInstance.Object, this.companyService.Object, null);

            this.routeService.Setup(r => r.IsRouteOwner(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsApproved(It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsBlocked(It.IsAny<string>()))
                .Returns(false);

            this.routeService.Setup(r => r.GetRouteBaseInfo(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(this.GetRouteInfo());

            this.routeService.Setup(r => r.HasReservedTickets(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(false);

            this.routeService.Setup(r => r.ChangeStatus(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(false);

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = controller.ChangeStatus(RouteId);

            //Assert
            this.AssertRedirectToCompanyRoutes(result);
            this.customMessage.Should().Be(WebConstants.Message.InvalidRoute);
        }

        [Fact]
        public void Post_ChangeStatus_WithValidDataShouldRedirectToAllRoutes()
        {
            //Arrange
            var controller = new RoutesController(this.routeService.Object, null, this.fixture.UserManagerMockInstance.Object, this.companyService.Object, null);

            this.routeService.Setup(r => r.IsRouteOwner(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsApproved(It.IsAny<string>()))
                .Returns(true);

            this.companyService.Setup(c => c.IsBlocked(It.IsAny<string>()))
                .Returns(false);

            this.routeService.Setup(r => r.GetRouteBaseInfo(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(this.GetRouteInfo());

            this.routeService.Setup(r => r.HasReservedTickets(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(false);

            this.routeService.Setup(r => r.ChangeStatus(It.IsAny<int>(), It.IsAny<string>()))
                .Returns(true);

            this.PrepareTempData();

            controller.TempData = this.tempData.Object;

            //Act
            var result = controller.ChangeStatus(RouteId);

            //Assert
            this.AssertRedirectToCompanyRoutes(result);
            this.customMessage.Should().Be(string.Format(WebConstants.Message.RouteStatusChanged, StartStationName, EndStationName, DepartureTime, RouteStatus));
        }

        private IEnumerable<TownBaseServiceModel> GetTowns()
        {
            var list = new List<TownBaseServiceModel>();

            for (int i = 0; i < TownsCount; i++)
            {
                list.Add(new TownBaseServiceModel()
                {
                    Id = i,
                    Name = $"Town {i}"
                });
            }

            return list;
        }

        private IEnumerable<TownStationsServiceModel> GetTownsStations()
        {
            var list = new List<TownStationsServiceModel>();

            for (int i = 0; i < TownsCount; i++)
            {
                list.Add(new TownStationsServiceModel()
                {
                    Id = i,
                    Name = $"Town {i}",
                    Stations = this.GetStations()
                });
            }

            return list;
        }

        private CompanyRoutesServiceModel GetRoutes()
        {
            var routes = new List<CompanyRouteListingServiceModel>();

            for (int i = 0; i < TotalRoutes; i++)
            {
                routes.Add(new CompanyRouteListingServiceModel()
                {
                    BusType = BusType.Mini.ToString(),
                    CompanyId = CompanyId,
                    Id = i,
                    Price = i
                });
            }

            return new CompanyRoutesServiceModel() { Routes = routes };
        }

        private IEnumerable<StationBaseServiceModel> GetStations()
        {
            var list = new List<StationBaseServiceModel>();

            for (int i = 0; i < 1; i++)
            {
                list.Add(new StationBaseServiceModel()
                {
                    Id = i,
                    Name = $"Station {i}"
                });
            }

            return list;
        }

        private RouteFormModel GetRouteFormModel() =>
            new RouteFormModel()
            {
                BusType = BusType.Mini,
                DepartureTime = DepartureDateTime,
                Duration = Duration,
                StartStation = StartStation,
                EndStation = EndStation,
                Price = Price
            };

        private CompanyRouteEditServiceModel GetRouteToEdit() =>
            new CompanyRouteEditServiceModel()
            {
                BusType = BusType.Mini,
                DepartureTime = DepartureDateTime,
                Duration = Duration,
                StartStationId = StartStation,
                EndStationId = EndStation,
                Id = RouteId,
                Price = Price
            };

        private CompanyRouteBaseSerivceModel GetRouteInfo() =>
            new CompanyRouteBaseSerivceModel()
            {
                DepartureTime = DepartureTime,
                StartStationTownName = StartStationName,
                EndStationTownName = EndStationName,
                Status = RouteStatus
            };

        private void AssertRedirectToCompanyRoutes(IActionResult result)
        {
            result.Should().BeOfType<RedirectToActionResult>();
            var redirectResult = result.As<RedirectToActionResult>();
            redirectResult.ActionName.Should().Be(WebConstants.Action.CompanyAllRoutes);
        }

        private void AssertRouteFormModelProperties(int startStationId, int endStationId, RouteFormModel form, object viewResult)
        {
            viewResult.Should().BeOfType<RouteFormModel>();
            form.BusType.Should().Be(BusType.Mini);
            form.DepartureTime.Should().Be(DepartureDateTime);
            form.Duration.Should().Be(Duration);
            form.StartStation.Should().Be(startStationId);
            form.EndStation.Should().Be(endStationId);
            form.Price.Should().Be(Price);
        }
    }
}
