namespace ETicketSystem.Test.Web.Controllers
{
	using Common.Constants;
	using ETicketSystem.Web.Controllers;
	using Fixtures;
	using FluentAssertions;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.ViewFeatures;
	using Mock;
	using Moq;
	using Services.Models.Ticket;
	using System.Linq;
	using Xunit;

	public class TicketsControllerTest : IClassFixture<UserManagerGetUserIdFixture>
	{
		const string UserId = "SomeuserId";

		const string RouteValueKeyId = "id";

		private readonly UserManagerGetUserIdFixture fixture;

		public TicketsControllerTest(UserManagerGetUserIdFixture fixture)
		{
			this.fixture = fixture;
		}

		[Fact]
		public void ControllerShouldBeOnlyForAuthorizedUsers()
		{
			//Arrange
			var controller = typeof(TicketsController);

			//Act
			var attributes = controller.GetCustomAttributes(true);

			//Assert
			attributes
				.Should()
				.Match(attr => attr.Any(a => a.GetType() == typeof(AuthorizeAttribute)));
		}

		[Fact]
		public void Cancel_WithNotExistingTicketShouldReturnRedirectToUserTicketsPage()
		{
			//Arrange
			var ticketService = TicketServiceMock.New;

			ticketService
				.Setup(t => t.TicketExists(It.IsAny<int>()))
				.Returns(false);

			Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();

			string errorMessage = null;

			tempData.SetupSet(t => t[WebConstants.TempDataKey.Message] = It.IsAny<string>())
				.Callback((string key, object message) => errorMessage = message as string);

			var controller = new TicketsController(ticketService.Object, fixture.UserManagerMockInstance.Object)
			{
				ControllerContext = new ControllerContext
				{
					HttpContext = fixture.HttpContextMockInstance.Object
				}
			};
			controller.TempData = tempData.Object;

			//Act
			IActionResult result = controller.Cancel(1);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			errorMessage.Should().Be(string.Format(WebConstants.Message.NonExistingEntity,WebConstants.Entity.Ticket,1));
			this.AssertRedirect(UserId, result as RedirectToActionResult);
		}

		[Fact]
		public void Cancel_WithNotTicketOwnerShouldReturnRedirectToUserTicketsPage()
		{
			//Arrange
			var ticketService = TicketServiceMock.New;
			ticketService.Setup(t => t.IsTicketOwner(It.IsAny<int>(), It.IsAny<string>()))
				.Returns(false);

			Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();

			string errorMessage = null;

			tempData.SetupSet(t => t[WebConstants.TempDataKey.Message] = It.IsAny<string>())
				.Callback((string key, object message) => errorMessage = message as string);

			var controller = new TicketsController(ticketService.Object, fixture.UserManagerMockInstance.Object)
			{
				ControllerContext = new ControllerContext
				{
					HttpContext = fixture.HttpContextMockInstance.Object
				}
			};
			controller.TempData = tempData.Object;

			//Act
			IActionResult result = controller.Cancel(1);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			errorMessage.Should().Be(string.Format(WebConstants.Message.NonExistingEntity, WebConstants.Entity.Ticket, 1));
			this.AssertRedirect(UserId, result as RedirectToActionResult);
		}

		[Fact]
		public void Cancel_WithCancelledTicketShouldReturnRedirectToUserTicketsPage()
		{
			//Arrange
			var ticketService = TicketServiceMock.New;
			ticketService.Setup(t => t.IsCancelled(It.IsAny<int>()))
				.Returns(true);

			Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();

			string errorMessage = null;

			tempData.SetupSet(t => t[WebConstants.TempDataKey.Message] = It.IsAny<string>())
				.Callback((string key, object message) => errorMessage = message as string);

			var controller = new TicketsController(ticketService.Object, fixture.UserManagerMockInstance.Object)
			{
				ControllerContext = new ControllerContext
				{
					HttpContext = fixture.HttpContextMockInstance.Object
				}
			};
			controller.TempData = tempData.Object;

			//Act
			IActionResult result = controller.Cancel(1);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			errorMessage.Should().Be(string.Format(WebConstants.Message.NonExistingEntity, WebConstants.Entity.Ticket, 1));
			this.AssertRedirect(UserId, result as RedirectToActionResult);
		}

		[Fact]
		public void Cancel_WithTicketDepartureTimeLessThanThirtyMinutesShouldReturnRedirectToUserTicketsPage()
		{
			//Arrange
			var ticketService = TicketServiceMock.New;

			ticketService.Setup(t => t.IsCancelled(It.IsAny<int>()))
				.Returns(false);

			ticketService.Setup(t => t.IsTicketOwner(It.IsAny<int>(),It.IsAny<string>()))
				.Returns(true);

			ticketService.Setup(t => t.TicketExists(It.IsAny<int>()))
				.Returns(true);

			ticketService.Setup(t => t.CancelTicket(It.IsAny<int>(),It.IsAny<string>()))
				.Returns(false);

			Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();

			string errorMessage = null;

			tempData.SetupSet(t => t[WebConstants.TempDataKey.Message] = It.IsAny<string>())
				.Callback((string key, object message) => errorMessage = message as string);

			var controller = new TicketsController(ticketService.Object, fixture.UserManagerMockInstance.Object)
			{
				ControllerContext = new ControllerContext
				{
					HttpContext = fixture.HttpContextMockInstance.Object
				}
			};
			controller.TempData = tempData.Object;

			//Act
			IActionResult result = controller.Cancel(1);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			errorMessage.Should().Be(WebConstants.Message.TicketCancelationDenied);
			this.AssertRedirect(UserId, result as RedirectToActionResult);
		}

		[Fact]
		public void Cancel_WithValidDataShouldCancelTicketAndRedirectToUserTicketsPage()
		{
			//Arrange
			var ticketService = TicketServiceMock.New;

			ticketService.Setup(t => t.IsCancelled(It.IsAny<int>()))
				.Returns(false);

			ticketService.Setup(t => t.IsTicketOwner(It.IsAny<int>(), It.IsAny<string>()))
				.Returns(true);

			ticketService.Setup(t => t.TicketExists(It.IsAny<int>()))
				.Returns(true);

			ticketService.Setup(t => t.CancelTicket(It.IsAny<int>(), It.IsAny<string>()))
				.Returns(true);

			Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();

			string errorMessage = null;

			tempData.SetupSet(t => t[WebConstants.TempDataKey.Message] = It.IsAny<string>())
				.Callback((string key, object message) => errorMessage = message as string);

			var controller = new TicketsController(ticketService.Object, fixture.UserManagerMockInstance.Object)
			{
				ControllerContext = new ControllerContext
				{
					HttpContext = fixture.HttpContextMockInstance.Object
				}
			};
			controller.TempData = tempData.Object;

			//Act
			IActionResult result = controller.Cancel(1);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			errorMessage.Should().Be(WebConstants.Message.TicketCancelationSuccess);
			this.AssertRedirect(UserId, result as RedirectToActionResult);
		}

		[Fact]
		public void Download_WithNotExistingTicketShouldReturnNullAndRedirectToUserTicketsPage()
		{
			//Arrange
			byte[] ticket = null; 
			
			var ticketService = TicketServiceMock.New;

			ticketService.Setup(t => t.GetPdfTicket(It.IsAny<int>(), It.IsAny<string>()))
				.Returns(ticket);

			Mock<ITempDataDictionary> tempData = new Mock<ITempDataDictionary>();

			string errorMessage = null;

			tempData.SetupSet(t => t[WebConstants.TempDataKey.Message] = It.IsAny<string>())
				.Callback((string key, object message) => errorMessage = message as string);

			var controller = new TicketsController(ticketService.Object, fixture.UserManagerMockInstance.Object)
			{
				ControllerContext = new ControllerContext
				{
					HttpContext = fixture.HttpContextMockInstance.Object
				}
			};
			controller.TempData = tempData.Object;

			//Act
			IActionResult result = controller.Download(1);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			errorMessage.Should().Be(string.Format(WebConstants.Message.InvalidTicket));
			this.AssertRedirect(UserId, result as RedirectToActionResult);
		}

		[Fact]
		public void Download_WithExistingTicketShouldReturnFile()
		{
			//Arrange
			byte[] ticket = new byte[100];

			var ticketService = TicketServiceMock.New;

			ticketService.Setup(t => t.GetPdfTicket(It.IsAny<int>(), It.IsAny<string>()))
				.Returns(ticket);

			ticketService.Setup(t => t.GetTicketDownloadInfo(It.IsAny<int>(), It.IsAny<string>()))
				.Returns(new TicketDownloadInfoServiceModel()
				{
					DepartureTime = "14:30:00",
					EndTown = "Balchik",
					StartTown = "Albena"
				});

			var controller = new TicketsController(ticketService.Object, fixture.UserManagerMockInstance.Object)
			{
				ControllerContext = new ControllerContext
				{
					HttpContext = fixture.HttpContextMockInstance.Object
				}
			};

			//Act
			IActionResult result = controller.Download(1);

			//Assert
			result.Should().BeOfType<FileContentResult>();
		}

		private void AssertRedirect(string userId,RedirectToActionResult result)
		{
			result.ActionName.Should().Be(WebConstants.Action.MyTickets);
			result.ControllerName.Should().Be(WebConstants.Controller.Users);
			result.RouteValues[RouteValueKeyId].Should().Be(userId);
		}
	}
}
