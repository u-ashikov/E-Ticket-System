namespace ETicketSystem.Test.Web.Controllers
{
	using Common.Constants;
	using Data.Enums;
	using ETicketSystem.Web.Controllers;
	using ETicketSystem.Web.Infrastructure.Extensions;
	using ETicketSystem.Web.Models.Routes;
	using FluentAssertions;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.ViewFeatures;
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

	public class RoutesControllerTest
    {
		private const int StartTownId = 1;

		private const int EndTownId = 2;

		private const string CompanyId = "CompanyId";

		private const int RouteId = 1;

		private const string StartStation = "Albena";

		private const string EndStation = "Balchik";

		public const string RouteValueStartTownKey = "startTown";
		public const string RouteValueEndTownKey = "endTown";
		public const string RouteValueDateKey = "date";
		public const string RouteValueCompanyIdKey = "companyId";

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
		public void Search_ShouldRedirectToHomeWithNonExistingStartTown()
		{
			//Arrange
			var townService = TownServiceMock.New;
			townService.Setup(t => t.TownExistsById(It.IsAny<int>()))
				.Returns(false);

			var controller = new RoutesController(townService.Object, null, null, null, null);

			var tempData = new Mock<ITempDataDictionary>();

			string errorMessage = null;

			tempData.SetupSet(t => t[WebConstants.TempDataKey.Message] = It.IsAny<string>())
				.Callback((string key, object msg) => errorMessage = msg as string);

			controller.TempData = tempData.Object;

			//Act
			var result = controller.Search(StartTownId, EndTownId, DateTime.UtcNow.ToLocalTime().Date,CompanyId);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(HomeController.Index));
			result.As<RedirectToActionResult>().ControllerName.Should().Be(WebConstants.Controller.Home);
			errorMessage.Should().Be(WebConstants.Message.InvalidTown);
		}

		[Fact]
		public void Search_ShouldRedirectToHomeWithPastDate()
		{
			//Arrange
			var townService = TownServiceMock.New;
			townService.Setup(t => t.TownExistsById(It.IsAny<int>()))
				.Returns(true);

			var controller = new RoutesController(townService.Object, null, null, null, null);

			var tempData = new Mock<ITempDataDictionary>();

			string errorMessage = null;

			tempData.SetupSet(t => t[WebConstants.TempDataKey.Message] = It.IsAny<string>())
				.Callback((string key, object msg) => errorMessage = msg as string);

			controller.TempData = tempData.Object;

			//Act
			var result = controller.Search(StartTownId, EndTownId, new DateTime(2017, 11, 19), CompanyId);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(HomeController.Index));
			result.As<RedirectToActionResult>().ControllerName.Should().Be(WebConstants.Controller.Home);
			errorMessage.Should().Be(WebConstants.Message.InvalidDate);
		}

		[Fact]
		public void Search_ShouldReturnCorrectResultsWithValidData()
		{
			//Arrange
			const int RouteTicketsCount = 10;
			var date = DateTime.UtcNow.ToLocalTime().Date;

			var townService = TownServiceMock.New;
			var routeService = RouteServiceMock.New;
			var companyService = CompanyServiceMock.New;
			var ticketService = TicketServiceMock.New;

			companyService.Setup(c => c.GetCompaniesSelectListItems())
				.Returns(this.GetCompaniesSelectListItems());

			townService.Setup(t => t.GetTownsListItems())
				.Returns(this.GetTownsSelectListItems());

			townService.Setup(t => t.TownExistsById(It.IsAny<int>()))
				.Returns(true);

			ticketService.Setup(t => t.GetRouteReservedTicketsCount(It.IsAny<int>(), It.IsAny<DateTime>()))
				.Returns(RouteTicketsCount);

			routeService.Setup(r => r.GetSearchedRoutes(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
				.Returns(this.GetRoutes());

			routeService.Setup(r => r.GetSearchedRoutesCount(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<string>()))
				.Returns(2);

			var controller = new RoutesController(townService.Object, routeService.Object, ticketService.Object, companyService.Object, null);

			//Act
			var result = controller.Search(StartTownId, EndTownId, date, CompanyId);

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model;
			model.Should().BeOfType<SearchedRoutes>();
			model.As<SearchedRoutes>().CompanyId.Should().Be(CompanyId);
			model.As<SearchedRoutes>().Date.Should().Be(date);
			model.As<SearchedRoutes>().StartTown.Should().Be(StartTownId);
			model.As<SearchedRoutes>().EndTown.Should().Be(EndTownId);
			model.As<SearchedRoutes>().Routes.Should().HaveCount(2);
			model.As<SearchedRoutes>().Towns.Should().HaveCount(6);
			model.As<SearchedRoutes>().Companies.Should().HaveCount(6);
			model.As<SearchedRoutes>().Pagination.TotalElements.Should().Be(2);
			model.As<SearchedRoutes>().Pagination.TotalPages.Should().Be(1);
		}

		[Fact]
		public void Get_BookTicket_ShouldRedirectToHomeWithInvalidRouteId()
		{
			//Arrange
			var departureTime = new TimeSpan(0, 0, 0);
			var date = DateTime.UtcNow.ToLocalTime().Date;
			var routeService = RouteServiceMock.New;
			routeService.Setup(r => r.RouteExists(It.IsAny<int>(),It.IsAny<TimeSpan>()))
				.Returns(false);

			var controller = new RoutesController(null, routeService.Object, null, null, null);

			var tempData = new Mock<ITempDataDictionary>();

			string errorMessage = null;

			tempData.SetupSet(t => t[WebConstants.TempDataKey.Message] = It.IsAny<string>())
				.Callback((string key, object msg) => errorMessage = msg as string);

			controller.TempData = tempData.Object;

			//Act
			var result = controller.BookTicket(StartTownId, departureTime, date, CompanyId);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(HomeController.Index));
			result.As<RedirectToActionResult>().ControllerName.Should().Be(WebConstants.Controller.Home);
			errorMessage.Should().Be(WebConstants.Message.InvalidRoute);
		}

		[Fact]
		public void Get_BookTicket_ShouldRedirectToHomeWithInvalidDate()
		{
			//Arrange
			var departureTime = new TimeSpan(0, 0, 0);
			var date = new DateTime(2017,11,11);
			var routeService = RouteServiceMock.New;
			routeService.Setup(r => r.RouteExists(It.IsAny<int>(), It.IsAny<TimeSpan>()))
				.Returns(true);

			var controller = new RoutesController(null, routeService.Object, null, null, null);

			var tempData = new Mock<ITempDataDictionary>();

			string errorMessage = null;

			tempData.SetupSet(t => t[WebConstants.TempDataKey.Message] = It.IsAny<string>())
				.Callback((string key, object msg) => errorMessage = msg as string);

			controller.TempData = tempData.Object;

			//Act
			var result = controller.BookTicket(StartTownId, departureTime, date, CompanyId);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(HomeController.Index));
			result.As<RedirectToActionResult>().ControllerName.Should().Be(WebConstants.Controller.Home);
			errorMessage.Should().Be(WebConstants.Message.InvalidRoute);
		}

		[Fact]
		public void Get_BookTicket_ShouldReturnBusSchemaWithValidData()
		{
			//Arrange
			var departureTime = new TimeSpan(23, 10, 10);
			var date = DateTime.UtcNow.ToLocalTime().Date;
			var departureDateTime = new DateTime(date.Year, date.Month, date.Day, departureTime.Hours, departureTime.Minutes, departureTime.Seconds);

			var routeService = RouteServiceMock.New;
			var companyService = CompanyServiceMock.New;

			routeService.Setup(r => r.RouteExists(It.IsAny<int>(), It.IsAny<TimeSpan>()))
				.Returns(true);

			routeService.Setup(r => r.GetRouteTicketBookingInfo(It.IsAny<int>(), It.IsAny<DateTime>()))
				.Returns(this.GetRouteInfo());

			companyService.Setup(c => c.Exists(It.IsAny<string>()))
				.Returns(true);

			var controller = new RoutesController(null, routeService.Object, null, companyService.Object, null);

			//Act
			var result = controller.BookTicket(RouteId, departureTime, date, CompanyId);

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model;
			model.Should().BeOfType<BookTicketFormModel>();
			model.As<BookTicketFormModel>().BusSeats.Should().Be(20);
			model.As<BookTicketFormModel>().RouteId.Should().Be(RouteId);
			model.As<BookTicketFormModel>().StartStation.Should().Be("Albena");
			model.As<BookTicketFormModel>().EndStation.Should().Be("Balchik");
			model.As<BookTicketFormModel>().StartTownId.Should().Be(1);
			model.As<BookTicketFormModel>().EndTownId.Should().Be(2);
			model.As<BookTicketFormModel>().CompanyName.Should().Be("Azimut");
			model.As<BookTicketFormModel>().DepartureDateTime.Should().Be(departureDateTime);
			model.As<BookTicketFormModel>().Duration.Should().Be(new TimeSpan(23,10,10));
			model.As<BookTicketFormModel>().CompanyId.Should().Be(CompanyId);
		}

		[Fact]
		public void Post_BookTicket_ShouldReturnViewForInvalidModel()
		{
			//Arrange
			var departureTime = new TimeSpan(23, 10, 10);
			var date = DateTime.UtcNow.ToLocalTime().Date;
			var departureDateTime = new DateTime(date.Year, date.Month, date.Day, departureTime.Hours, departureTime.Minutes, departureTime.Seconds);

			var routeService = RouteServiceMock.New;

			routeService.Setup(r => r.GetRouteTicketBookingInfo(It.IsAny<int>(), It.IsAny<DateTime>()))
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
			var routeService = RouteServiceMock.New;

			routeService.Setup(r => r.RouteExists(It.IsAny<int>(), It.IsAny<TimeSpan>()))
				.Returns(false);

			var controller = new RoutesController(null, routeService.Object, null, null, null);

			string errorMessage = null;

			var tempData = new Mock<ITempDataDictionary>();
			tempData
				.SetupSet(t => t[WebConstants.TempDataKey.Message] = It.IsAny<string>())
				.Callback((string key, object msg) => errorMessage = msg as string);

			controller.TempData = tempData.Object;

			//Act
			var result = controller.BookTicket(new BookTicketFormModel());

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(HomeController.Index), WebConstants.Controller.Home);
			errorMessage.Should().Be(WebConstants.Message.InvalidRoute);
		}

		[Fact]
		public void Post_BookTicket_ShouldRedirectToHomeWithPastData()
		{
			//Arrange
			var departureTime = new TimeSpan(10, 11, 12);
			var date = new DateTime(2017, 11, 11);
			var departureDateTime = new DateTime(date.Year, date.Month, date.Day, departureTime.Hours, departureTime.Minutes, departureTime.Seconds);

			var routeService = RouteServiceMock.New;

			routeService.Setup(r => r.RouteExists(It.IsAny<int>(), It.IsAny<TimeSpan>()))
				.Returns(true);

			var controller = new RoutesController(null, routeService.Object, null, null, null);

			string errorMessage = null;

			var tempData = new Mock<ITempDataDictionary>();
			tempData
				.SetupSet(t => t[WebConstants.TempDataKey.Message] = It.IsAny<string>())
				.Callback((string key, object msg) => errorMessage = msg as string);

			controller.TempData = tempData.Object;

			//Act
			var result = controller.BookTicket(new BookTicketFormModel() { DepartureDateTime = departureDateTime});

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(HomeController.Index), WebConstants.Controller.Home);
			errorMessage.Should().Be(WebConstants.Message.InvalidRoute);
		}

		[Fact]
		public void Post_BookTicket_ShouldRedirectToSearchWhenBusIsAlreadyReserved()
		{
			//Arrange
			var routeService = RouteServiceMock.New;
			var ticketService = TicketServiceMock.New;

			routeService.Setup(r => r.RouteExists(It.IsAny<int>(), It.IsAny<TimeSpan>()))
				.Returns(true);

			ticketService.Setup(t => t.GetAlreadyReservedTickets(It.IsAny<int>(), It.IsAny<DateTime>()))
				.Returns(new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 });

			var controller = new RoutesController(null, routeService.Object, ticketService.Object, null, null);

			string errorMessage = null;

			var tempData = new Mock<ITempDataDictionary>();
			tempData
				.SetupSet(t => t[WebConstants.TempDataKey.Message] = It.IsAny<string>())
				.Callback((string key, object msg) => errorMessage = msg as string);

			controller.TempData = tempData.Object;

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
			errorMessage.Should().Be(string.Format(WebConstants.Message.RouteSoldOut, form.StartStation, form.EndStation, form.DepartureDateTime.Date.ToYearMonthDayFormat(), form.DepartureDateTime.TimeOfDay));
		}

		private BookTicketFormModel GenerateBookTicketForm()
		{
			var departureTime = new TimeSpan(23, 10, 10);
			var date = DateTime.UtcNow.ToLocalTime().Date;
			var departureDateTime = new DateTime(date.Year, date.Month, date.Day, departureTime.Hours, departureTime.Minutes, departureTime.Seconds);

			return new BookTicketFormModel() { DepartureDateTime = departureDateTime, StartStation = StartStation, EndStation = EndStation, StartTownId = StartTownId, EndTownId = EndTownId, CompanyId = CompanyId, BusSeats = 20 };
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
				ReservedTickets = new List<int>() { 1,2,3}
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
					}
			};
		}
	}
}
