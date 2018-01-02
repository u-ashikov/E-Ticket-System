namespace ETicketSystem.Test.Web.Controllers
{
	using Data.Enums;
	using ETicketSystem.Common.Constants;
	using ETicketSystem.Data.Models;
	using ETicketSystem.Services.Contracts;
	using ETicketSystem.Web.Controllers;
	using ETicketSystem.Web.Infrastructure.Extensions;
	using ETicketSystem.Web.Models.Routes;
	using FluentAssertions;
	using Infrastructure;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using Mock;
	using Mocks;
	using Moq;
	using Services.Models.Company;
	using Services.Models.Route;
	using Services.Models.Town;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Xunit;

	using static Common.TestConstants;

	public class RoutesControllerTest : BaseControllerTest
    {
		private const int StartTownId = 1;

		private const int EndTownId = 2;

		private const string CompanyId = "CompanyId";

		private const int RouteId = 1;

		private const string StartStation = "Albena";

		private const string EndStation = "Balchik";

		private readonly Mock<ITownService> townService = TownServiceMock.New;

		private readonly Mock<ITicketService> ticketService = TicketServiceMock.New;

		private readonly Mock<ICompanyService> companyService = CompanyServiceMock.New;

		private readonly Mock<IRouteService> routeService = RouteServiceMock.New;

		private readonly Mock<UserManager<User>> userManager = UserManagerMock.New;

		[Fact]
		public void RoutesControllerShouldBeForAuthorizedUsersOnly()
		{
			//Arrange
			var controller = typeof(RoutesController);

			//Act
			var attributes = controller.GetCustomAttributes(true);

			//Assert
			attributes.Should().Match(attr => attr.Any(a=>a.GetType() == typeof(AuthorizeAttribute)));
		}

		[Fact]
		public void Search_ShouldBeAccessibleByAnnonymousUsers()
		{
			//Arrange
			var controllerMethod = typeof(RoutesController).GetMethod(nameof(RoutesController.Search));

			//Act
			var attributes = controllerMethod.GetCustomAttributes(true);

			//Assert
			attributes.Should().Match(attr => attr.Any(a => a.GetType() == typeof(AllowAnonymousAttribute)));
		}

		[Fact]
		public void Search_ShouldRedirectToHomeWithNonExistingTown()
		{
			//Arrange
			this.townService.Setup(t => t.TownExistsById(It.IsAny<int>()))
				.Returns(false);

			var controller = new RoutesController(townService.Object, null, null, null, null);
			this.SetControllerTempData(controller);

			this.PrepareTempData();

			//Act
			var result = controller.Search(StartTownId, EndTownId, DateTime.UtcNow.ToLocalTime().Date, CompanyId);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(HomeController.Index));
			result.As<RedirectToActionResult>().ControllerName.Should().Be(WebConstants.Controller.Home);
			this.customMessage.Should().Be(WebConstants.Message.InvalidTown);
		}

		[Fact]
		public void Search_ShouldRedirectToHomeWithPastDate()
		{
			//Arrange
			townService.Setup(t => t.TownExistsById(It.IsAny<int>()))
				.Returns(true);

			var controller = new RoutesController(townService.Object, null, null, null, null);
			this.SetControllerTempData(controller);

			this.PrepareTempData();

			//Act
			var result = controller.Search(StartTownId, EndTownId, DateTime.UtcNow.AddMonths(-1), CompanyId);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(HomeController.Index));
			result.As<RedirectToActionResult>().ControllerName.Should().Be(WebConstants.Controller.Home);
			this.customMessage.Should().Be(WebConstants.Message.InvalidDate);
		}

		[Theory]
		[InlineData(-1)]
		[InlineData(0)]
		public void Search_ShouldRedirectToSearchWithPageLessThanOrEqualToZero(int page)
		{
			//Arrange
			const int RouteTicketsCount = 10;
			var date = DateTime.UtcNow.ToLocalTime().Date;

			this.companyService.Setup(c => c.GetCompaniesSelectListItems())
				.Returns(this.GetCompaniesSelectListItems());

			this.townService.Setup(t => t.GetTownsListItems())
				.Returns(this.GetTownsSelectListItems());

			this.townService.Setup(t => t.TownExistsById(It.IsAny<int>()))
				.Returns(true);

			this.ticketService.Setup(t => t.GetRouteReservedTicketsCount(It.IsAny<int>(), It.IsAny<DateTime>()))
				.Returns(RouteTicketsCount);

			this.routeService.Setup(r => r.GetSearchedRoutes(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns(this.GetRoutes());

			this.routeService.Setup(r => r.GetSearchedRoutesCount(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<string>()))
				.Returns(2);

			var controller = new RoutesController(this.townService.Object, this.routeService.Object, this.ticketService.Object, this.companyService.Object, null);

			//Act
			var result = controller.Search(StartTownId, EndTownId, date, string.Empty,page);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			var model = result.As<RedirectToActionResult>();
			model.ActionName = WebConstants.Action.Search;
			model.ControllerName = WebConstants.Controller.Routes;
			model.RouteValues.Keys.Should().Contain(RouteValueStartTownKey);
			model.RouteValues.Values.Should().Contain(StartTownId);
			model.RouteValues.Keys.Should().Contain(RouteValueEndTownKey);
			model.RouteValues.Values.Should().Contain(EndTownId);
			model.RouteValues.Keys.Should().Contain(RouteValueDateKey);
			model.RouteValues.Values.Should().Contain(date.ToShortDateString());
			model.RouteValues.Keys.Should().Contain(RouteValueCompanyIdKey);
			model.RouteValues.Values.Should().Contain(string.Empty);
			model.RouteValues.Keys.Should().Contain(RouteValueKeyPage);
			model.RouteValues.Values.Should().Contain(1);
		}

		[Theory]
		[InlineData(10)]
		public void Search_ShouldRedirectToSearchWithPageGreaterThanTotalPages(int page)
		{
			//Arrange
			const int RouteTicketsCount = 10;
			var date = DateTime.UtcNow.ToLocalTime().Date;

			this.companyService.Setup(c => c.GetCompaniesSelectListItems())
				.Returns(this.GetCompaniesSelectListItems());

			this.townService.Setup(t => t.GetTownsListItems())
				.Returns(this.GetTownsSelectListItems());

			this.townService.Setup(t => t.TownExistsById(It.IsAny<int>()))
				.Returns(true);

			this.ticketService.Setup(t => t.GetRouteReservedTicketsCount(It.IsAny<int>(), It.IsAny<DateTime>()))
				.Returns(RouteTicketsCount);

			this.routeService.Setup(r => r.GetSearchedRoutes(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns(this.GetRoutes());

			this.routeService.Setup(r => r.GetSearchedRoutesCount(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<string>()))
				.Returns(2);

			var controller = new RoutesController(this.townService.Object, this.routeService.Object, this.ticketService.Object, this.companyService.Object, null);

			//Act
			var result = controller.Search(StartTownId, EndTownId, date, string.Empty, page);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			var model = result.As<RedirectToActionResult>();
			model.ActionName = WebConstants.Action.Search;
			model.ControllerName = WebConstants.Controller.Routes;
			model.RouteValues.Keys.Should().Contain(RouteValueStartTownKey);
			model.RouteValues.Values.Should().Contain(StartTownId);
			model.RouteValues.Keys.Should().Contain(RouteValueEndTownKey);
			model.RouteValues.Values.Should().Contain(EndTownId);
			model.RouteValues.Keys.Should().Contain(RouteValueDateKey);
			model.RouteValues.Values.Should().Contain(date.ToShortDateString());
			model.RouteValues.Keys.Should().Contain(RouteValueCompanyIdKey);
			model.RouteValues.Values.Should().Contain(string.Empty);
			model.RouteValues.Keys.Should().Contain(RouteValueKeyPage);
			model.RouteValues.Values.Should().Contain(2);
		}

		[Fact]
		public void Search_ShouldReturnCorrectResultsWithValidData()
		{
			//Arrange
			const int RouteTicketsCount = 10;
			var date = DateTime.UtcNow.ToLocalTime().Date;

			this.companyService.Setup(c => c.GetCompaniesSelectListItems())
				.Returns(this.GetCompaniesSelectListItems());

			this.townService.Setup(t => t.GetTownsListItems())
				.Returns(this.GetTownsSelectListItems());

			this.townService.Setup(t => t.TownExistsById(It.IsAny<int>()))
				.Returns(true);

			this.ticketService.Setup(t => t.GetRouteReservedTicketsCount(It.IsAny<int>(), It.IsAny<DateTime>()))
				.Returns(RouteTicketsCount);

			this.routeService.Setup(r => r.GetSearchedRoutes(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns(this.GetRoutes());

			this.routeService.Setup(r => r.GetSearchedRoutesCount(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<string>()))
				.Returns(6);

			var controller = new RoutesController(this.townService.Object, this.routeService.Object, this.ticketService.Object, this.companyService.Object, null);

			//Act
			var result = controller.Search(StartTownId, EndTownId, date, string.Empty);

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model;
			var searchedRoutes = model.As<SearchedRoutes>();
			model.Should().BeOfType<SearchedRoutes>();
			searchedRoutes.CompanyId.Should().Be(string.Empty);
			searchedRoutes.Date.Should().Be(date);
			searchedRoutes.StartTown.Should().Be(StartTownId);
			searchedRoutes.EndTown.Should().Be(EndTownId);
			searchedRoutes.Routes.Should().HaveCount(6);
			searchedRoutes.Towns.Should().HaveCount(6);
			searchedRoutes.Companies.Should().HaveCount(6);
			searchedRoutes.Pagination.TotalElements.Should().Be(6);
			searchedRoutes.Pagination.TotalPages.Should().Be(2);
		}

		[Theory]
		[InlineData("Test")]
		[InlineData("")]
		[InlineData(null)]
		public void Search_ShouldReturnNoResultsForNonExistingCompany(string companyId)
		{
			//Arrange
			var date = DateTime.UtcNow.ToLocalTime().Date;

			this.companyService.Setup(c => c.GetCompaniesSelectListItems())
				.Returns(this.GetCompaniesSelectListItems());

			this.townService.Setup(t => t.GetTownsListItems())
				.Returns(this.GetTownsSelectListItems());

			this.townService.Setup(t => t.TownExistsById(It.IsAny<int>()))
				.Returns(true);

			this.ticketService.Setup(t => t.GetRouteReservedTicketsCount(It.IsAny<int>(), It.IsAny<DateTime>()))
				.Returns(0);

			this.routeService.Setup(r => r.GetSearchedRoutes(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns(new List<RouteSearchListingServiceModel>());

			this.routeService.Setup(r => r.GetSearchedRoutesCount(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<string>()))
				.Returns(0);

			var controller = new RoutesController(this.townService.Object, this.routeService.Object, this.ticketService.Object, this.companyService.Object, null);

			//Act
			var result = controller.Search(StartTownId, EndTownId, date, companyId);

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model;
			var searchedRoutes = model.As<SearchedRoutes>();
			model.Should().BeOfType<SearchedRoutes>();
			searchedRoutes.CompanyId.Should().Be(companyId);
			searchedRoutes.Date.Should().Be(date);
			searchedRoutes.StartTown.Should().Be(StartTownId);
			searchedRoutes.EndTown.Should().Be(EndTownId);
			searchedRoutes.Routes.Should().HaveCount(0);
			searchedRoutes.Towns.Should().HaveCount(6);
			searchedRoutes.Companies.Should().HaveCount(6);
			searchedRoutes.Pagination.TotalElements.Should().Be(0);
			searchedRoutes.Pagination.TotalPages.Should().Be(0);
		}

		[Fact]
		public void Get_BookTicket_ShouldRedirectToHomeWithInvalidRouteId()
		{
			//Arrange
			var departureTime = new TimeSpan(0, 0, 0);
			var date = DateTime.UtcNow.ToLocalTime().Date;

			this.routeService.Setup(r => r.RouteExists(It.IsAny<int>(),It.IsAny<TimeSpan>()))
				.Returns(false);

			var controller = new RoutesController(null, routeService.Object, null, null, null);

			this.PrepareTempData();

			this.SetControllerTempData(controller);

			//Act
			var result = controller.BookTicket(StartTownId, departureTime, date, CompanyId);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(HomeController.Index));
			result.As<RedirectToActionResult>().ControllerName.Should().Be(WebConstants.Controller.Home);
			this.customMessage.Should().Be(WebConstants.Message.InvalidRoute);
		}

		[Fact]
		public void Get_BookTicket_ShouldRedirectToHomeWithInvalidDate()
		{
			//Arrange
			var departureTime = new TimeSpan(0, 0, 0);
			var date = new DateTime(2017,11,11);

			this.routeService.Setup(r => r.RouteExists(It.IsAny<int>(), It.IsAny<TimeSpan>()))
				.Returns(true);

			var controller = new RoutesController(null, routeService.Object, null, null, null);

			this.PrepareTempData();

			this.SetControllerTempData(controller);

			//Act
			var result = controller.BookTicket(StartTownId, departureTime, date, CompanyId);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(HomeController.Index));
			result.As<RedirectToActionResult>().ControllerName.Should().Be(WebConstants.Controller.Home);
			this.customMessage.Should().Be(WebConstants.Message.InvalidRoute);
		}

		[Fact]
		public void Get_BookTicket_ShouldReturnBusSchemaWithValidData()
		{
			//Arrange
			var departureTime = new TimeSpan(23, 10, 10);
			var date = DateTime.UtcNow.ToLocalTime().Date;
			var departureDateTime = new DateTime(date.Year, date.Month, date.Day, departureTime.Hours, departureTime.Minutes, departureTime.Seconds);

			this.routeService.Setup(r => r.RouteExists(It.IsAny<int>(), It.IsAny<TimeSpan>()))
				.Returns(true);

			this.routeService.Setup(r => r.GetRouteTicketBookingInfo(It.IsAny<int>(), It.IsAny<DateTime>()))
				.Returns(this.GetRouteInfo());

			this.companyService.Setup(c => c.Exists(It.IsAny<string>()))
				.Returns(true);

			var controller = new RoutesController(null, routeService.Object, null, companyService.Object, null);

			//Act
			var result = controller.BookTicket(RouteId, departureTime, date, CompanyId);

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model;
			model.Should().BeOfType<BookTicketFormModel>();
			var form = model.As<BookTicketFormModel>();

			form.BusSeats.Should().Be(20);
			form.RouteId.Should().Be(RouteId);
			form.StartStation.Should().Be("Albena");
			form.EndStation.Should().Be("Balchik");
			form.StartTownId.Should().Be(1);
			form.EndTownId.Should().Be(2);
			form.CompanyName.Should().Be("Azimut");
			form.DepartureDateTime.Should().Be(departureDateTime);
			form.Duration.Should().Be(new TimeSpan(23,10,10));
			form.CompanyId.Should().Be(CompanyId);
		}

		[Fact]
		public void Post_BookTicket_ShouldReturnViewForInvalidModel()
		{
			//Arrange
			var departureTime = new TimeSpan(23, 10, 10);
			var date = DateTime.UtcNow.ToLocalTime().Date;
			var departureDateTime = new DateTime(date.Year, date.Month, date.Day, departureTime.Hours, departureTime.Minutes, departureTime.Seconds);

			this.routeService.Setup(r => r.GetRouteTicketBookingInfo(It.IsAny<int>(), It.IsAny<DateTime>()))
				.Returns(this.GetRouteInfo());

			var controller = new RoutesController(null, routeService.Object, null, null, null);
			controller.ModelState.AddModelError(string.Empty, "Error");

			//Act
			var result = controller.BookTicket(new BookTicketFormModel());

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model;
			model.Should().BeOfType<BookTicketFormModel>();
		}

		[Fact]
		public void Post_BookTicket_ShouldRedirectToHomeWithNonExistingRoute()
		{
			//Arrange
			this.routeService.Setup(r => r.RouteExists(It.IsAny<int>(), It.IsAny<TimeSpan>()))
				.Returns(false);

			var controller = new RoutesController(null, routeService.Object, null, null, null);

			this.PrepareTempData();
			this.SetControllerTempData(controller);

			//Act
			var result = controller.BookTicket(new BookTicketFormModel());

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(HomeController.Index), WebConstants.Controller.Home);
			this.customMessage.Should().Be(WebConstants.Message.InvalidRoute);
		}

		[Fact]
		public void Post_BookTicket_ShouldRedirectToHomeWithPastData()
		{
			//Arrange
			var departureTime = new TimeSpan(10, 11, 12);
			var date = new DateTime(2017, 11, 11);
			var departureDateTime = new DateTime(date.Year, date.Month, date.Day, departureTime.Hours, departureTime.Minutes, departureTime.Seconds);

			routeService.Setup(r => r.RouteExists(It.IsAny<int>(), It.IsAny<TimeSpan>()))
				.Returns(true);

			var controller = new RoutesController(null, routeService.Object, null, null, null);

			this.PrepareTempData();
			this.SetControllerTempData(controller);

			//Act
			var result = controller.BookTicket(new BookTicketFormModel() { DepartureDateTime = departureDateTime});

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(HomeController.Index), WebConstants.Controller.Home);
			this.customMessage.Should().Be(WebConstants.Message.InvalidRoute);
		}

		[Fact]
		public void Post_BookTicket_ShouldRedirectToSearchWhenBusIsAlreadyReserved()
		{
			//Arrange
			this.routeService.Setup(r => r.RouteExists(It.IsAny<int>(), It.IsAny<TimeSpan>()))
				.Returns(true);

			this.ticketService.Setup(t => t.GetAlreadyReservedTickets(It.IsAny<int>(), It.IsAny<DateTime>()))
				.Returns(new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 });

			this.companyService.Setup(c => c.Exists(It.IsAny<string>()))
				.Returns(true);

			var controller = new RoutesController(null, routeService.Object, ticketService.Object, companyService.Object, null);

			this.PrepareTempData();
			this.SetControllerTempData(controller);

			//Act
			var form = this.GenerateBookTicketForm();
			var result = controller.BookTicket(form);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(RoutesController.Search), WebConstants.Controller.Routes);
			result.As<RedirectToActionResult>().RouteValues[RouteValueStartTownKey].Should().Be(form.StartTownId);
			result.As<RedirectToActionResult>().RouteValues[RouteValueEndTownKey].Should().Be(form.EndTownId);
			result.As<RedirectToActionResult>().RouteValues[RouteValueDateKey].Should().Be(form.DepartureDateTime.Date);
			result.As<RedirectToActionResult>().RouteValues[RouteValueCompanyIdKey].Should().Be(form.CompanyId);
			this.customMessage.Should().Be(string.Format(WebConstants.Message.RouteSoldOut, form.StartStation, form.EndStation, form.DepartureDateTime.Date.ToYearMonthDayFormat(), form.DepartureDateTime.TimeOfDay));
		}

		[Fact]
		public void Post_BookTicket_ShouldReturnBusSchemaWhenSomeOfTheSelectedSeatsAreAlreadyReserved()
		{
			//Arrange
			this.routeService.Setup(r => r.RouteExists(It.IsAny<int>(), It.IsAny<TimeSpan>()))
				.Returns(true);

			this.routeService.Setup(r => r.GetRouteTicketBookingInfo(It.IsAny<int>(), It.IsAny<DateTime>()))
				.Returns(new RouteBookTicketInfoServiceModel() { BusType = BusType.Mini, ReservedTickets =  new List<int>()});

			this.ticketService.Setup(t => t.GetAlreadyReservedTickets(It.IsAny<int>(), It.IsAny<DateTime>()))
				.Returns(new List<int>() { 1, 2, 3, 4 });

			this.companyService.Setup(c => c.Exists(It.IsAny<string>()))
				.Returns(true);

			var controller = new RoutesController(null, this.routeService.Object, this.ticketService.Object, this.companyService.Object, this.userManager.Object);

			controller.ModelState.AddModelError(string.Empty, "Error");

			this.PrepareTempData();
			this.SetControllerTempData(controller);

			var matchingSeats = new List<int>() { 1, 2 };

			//Act
			var form = this.GenerateBookTicketForm();
			var result = controller.BookTicket(form);

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model;
			model.Should().BeOfType<BookTicketFormModel>();
		}

		[Fact]
		public void Post_BookTicket_ShouldRedirectToHomeWithInvalidData()
		{
			//Arrange
			this.routeService.Setup(r => r.RouteExists(It.IsAny<int>(), It.IsAny<TimeSpan>()))
				.Returns(true);

			this.routeService.Setup(r => r.GetRouteTicketBookingInfo(It.IsAny<int>(), It.IsAny<DateTime>()))
				.Returns(new RouteBookTicketInfoServiceModel() { BusType = BusType.Mini, ReservedTickets = new List<int>() });

			this.ticketService.Setup(t => t.GetAlreadyReservedTickets(It.IsAny<int>(), It.IsAny<DateTime>()))
				.Returns(new List<int>() { 1, 2, 3, 4 });

			this.ticketService.Setup(t => t.Add(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<IEnumerable<int>>(), It.IsAny<string>()))
				.Returns(false);

			this.companyService.Setup(c => c.Exists(It.IsAny<string>()))
				.Returns(true);

			var controller = new RoutesController(null, this.routeService.Object, this.ticketService.Object, this.companyService.Object, this.userManager.Object);

			this.PrepareTempData();
			this.SetControllerTempData(controller);

			//Act
			var form = this.GenerateBookTicketForm();
			var result = controller.BookTicket(form);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			var model = result.As<RedirectToActionResult>();
			model.ActionName.Should().Be(WebConstants.Action.Index);
			model.ControllerName.Should().Be(WebConstants.Controller.Home);
			this.customMessage.Should().Be(WebConstants.Message.InvalidRoute);
		}

		[Fact]
		public void Post_BookTicket_ShouldRedirectToHomeAfterSuccessfullReservation()
		{
			//Arrange
			this.routeService.Setup(r => r.RouteExists(It.IsAny<int>(), It.IsAny<TimeSpan>()))
				.Returns(true);

			this.routeService.Setup(r => r.GetRouteTicketBookingInfo(It.IsAny<int>(), It.IsAny<DateTime>()))
				.Returns(new RouteBookTicketInfoServiceModel() { BusType = BusType.Mini, ReservedTickets = new List<int>() });

			this.ticketService.Setup(t => t.GetAlreadyReservedTickets(It.IsAny<int>(), It.IsAny<DateTime>()))
				.Returns(new List<int>() { 1, 2, 3, 4 });

			this.ticketService.Setup(t => t.Add(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<IEnumerable<int>>(), It.IsAny<string>()))
				.Returns(true);

			this.companyService.Setup(c => c.Exists(It.IsAny<string>()))
				.Returns(true);

			var controller = new RoutesController(null, this.routeService.Object, this.ticketService.Object, this.companyService.Object, this.userManager.Object);

			this.PrepareTempData();
			this.SetControllerTempData(controller);

			//Act
			var form = this.GenerateBookTicketForm();
			form.Seats.FirstOrDefault(s => s.Value == 10).Checked = true;
			form.Seats.FirstOrDefault(s => s.Value == 11).Checked = true;

			var reservedTickets = new List<int>() { 10, 11 };

			var result = controller.BookTicket(form);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			var model = result.As<RedirectToActionResult>();
			model.ActionName.Should().Be(WebConstants.Action.Index);
			model.ControllerName.Should().Be(WebConstants.Controller.Home);
			this.customMessage.Should().Be(string.Format(WebConstants.Message.SuccessfullyTicketReservation, string.Join(", ", reservedTickets), form.StartStation, form.EndStation, form.DepartureDateTime));
		}

		private BookTicketFormModel GenerateBookTicketForm()
		{
			var departureTime = new TimeSpan(23, 10, 10);
			var date = DateTime.UtcNow.ToLocalTime().Date;
			var departureDateTime = new DateTime(date.Year, date.Month, date.Day, departureTime.Hours, departureTime.Minutes, departureTime.Seconds);

			var seats = new List<BusSeat>();

			for (int i = 1; i <= 20; i++)
			{
				seats.Add(new BusSeat() { Value = i, Checked = false, Disabled = false });
			}

			return new BookTicketFormModel() { DepartureDateTime = departureDateTime, StartStation = StartStation, EndStation = EndStation, StartTownId = StartTownId, EndTownId = EndTownId, CompanyId = CompanyId, BusSeats = 20, Seats = seats };
		}

		private RouteBookTicketInfoServiceModel GetRouteInfo()
		{
			return new RouteBookTicketInfoServiceModel()
			{
				BusType = BusType.Mini,
				CompanyName = "Azimut",
				Duration = new TimeSpan(23, 10, 10),
				StartTownId = 1,
				EndTownId = 2,
				StartStation = "Albena",
				EndStation = "Balchik",
				ReservedTickets = new List<int>() { 1,2,3 }
			};
		}

		private IEnumerable<CompanyBaseServiceModel> GetCompaniesSelectListItems()
		{
			var list = new List<CompanyBaseServiceModel>();

			for (int i = 0; i < 5; i++)
			{
				list.Add(new CompanyBaseServiceModel()
				{
					Id = i.ToString(),
					Name = $"Company {i}"
				});
			}

			return list;
		}

		private IEnumerable<TownBaseServiceModel> GetTownsSelectListItems()
		{
			var list = new List<TownBaseServiceModel>();

			for (int i = 0; i < 5; i++)
			{
				list.Add(new TownBaseServiceModel()
				{
					Id = i+1,
					Name = $"Town {i}"
				});
			}

			return list;
		}

		private IList<RouteSearchListingServiceModel> GetRoutes()
		{
			return new List<RouteSearchListingServiceModel>()
			{
				new RouteSearchListingServiceModel()
					{
						DepartureTime = new TimeSpan(23,10,10),
						CompanyName = "Aguila"
					},
				new RouteSearchListingServiceModel()
					{
						DepartureTime = new TimeSpan(23,20,20),
						CompanyName = "Azimut"
					},
				new RouteSearchListingServiceModel()
					{
						DepartureTime = new TimeSpan(23,30,20),
						CompanyName = "Aguila"
					},
				new RouteSearchListingServiceModel()
					{
						DepartureTime = new TimeSpan(23,34,20),
						CompanyName = "Aguila"
					},
				new RouteSearchListingServiceModel()
					{
						DepartureTime = new TimeSpan(23,44,20),
						CompanyName = "Azimut"
					},
				new RouteSearchListingServiceModel()
					{
						DepartureTime = new TimeSpan(23,46,20),
						CompanyName = "Aguila"
					}
			};
		}

		private void SetControllerTempData(RoutesController controller)
		{
			controller.TempData = this.tempData.Object;
		}
	}
}
