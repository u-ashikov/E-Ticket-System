namespace ETicketSystem.Test.Web.Controllers
{
	using ETicketSystem.Common.Constants;
	using ETicketSystem.Services.Contracts;
	using ETicketSystem.Web.Controllers;
	using Fixtures;
	using FluentAssertions;
	using Infrastructure;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Mvc;
	using Mock;
	using Moq;
	using Services.Models.Ticket;
	using System.Linq;
	using Xunit;
	using static Common.TestConstants;

	public class TicketsControllerTest : BaseControllerTest,IClassFixture<UserManagerGetUserIdFixture>
	{
		private readonly UserManagerGetUserIdFixture fixture;

		private readonly Mock<ITicketService> ticketService = TicketServiceMock.New;

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
			this.ticketService
				.Setup(t => t.TicketExists(It.IsAny<int>()))
				.Returns(false);

			this.PrepareTempData();

			TicketsController controller = PrepareController();

			//Act
			IActionResult result = controller.Cancel(1);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			this.customMessage.Should().Be(string.Format(WebConstants.Message.NonExistingEntity, WebConstants.Entity.Ticket, 1));
			this.AssertRedirect(UserId, result as RedirectToActionResult);
		}

		[Fact]
		public void Cancel_WithNotTicketOwnerShouldReturnRedirectToUserTicketsPage()
		{
			//Arrange
			this.ticketService
				.Setup(t => t.IsTicketOwner(It.IsAny<int>(), It.IsAny<string>()))
				.Returns(false);

			this.PrepareTempData();

			TicketsController controller = this.PrepareController();

			//Act
			IActionResult result = controller.Cancel(1);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			this.customMessage.Should().Be(string.Format(WebConstants.Message.NonExistingEntity, WebConstants.Entity.Ticket, 1));
			this.AssertRedirect(UserId, result as RedirectToActionResult);
		}

		[Fact]
		public void Cancel_WithCancelledTicketShouldReturnRedirectToUserTicketsPage()
		{
			//Arrange
			this.ticketService
				.Setup(t => t.IsCancelled(It.IsAny<int>()))
				.Returns(true);

			this.PrepareTempData();

			var controller = this.PrepareController();

			//Act
			IActionResult result = controller.Cancel(1);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			this.customMessage.Should().Be(string.Format(WebConstants.Message.NonExistingEntity, WebConstants.Entity.Ticket, 1));
			this.AssertRedirect(UserId, result as RedirectToActionResult);
		}

		[Fact]
		public void Cancel_WithTicketDepartureTimeLessThanThirtyMinutesShouldReturnRedirectToUserTicketsPage()
		{
			//Arrange
			this.ticketService
				.Setup(t => t.IsCancelled(It.IsAny<int>()))
				.Returns(false);

			this.ticketService.Setup(t => t.IsTicketOwner(It.IsAny<int>(),It.IsAny<string>()))
				.Returns(true);

			this.ticketService.Setup(t => t.TicketExists(It.IsAny<int>()))
				.Returns(true);

			this.ticketService.Setup(t => t.CancelTicket(It.IsAny<int>(),It.IsAny<string>()))
				.Returns(false);

			this.PrepareTempData();

			var controller = this.PrepareController();

			//Act
			IActionResult result = controller.Cancel(1);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			this.customMessage.Should().Be(WebConstants.Message.TicketCancelationDenied);
			this.AssertRedirect(UserId, result as RedirectToActionResult);
		}

		[Fact]
		public void Cancel_WithValidDataShouldCancelTicketAndRedirectToUserTicketsPage()
		{
			//Arrange
			this.ticketService.Setup(t => t.IsCancelled(It.IsAny<int>()))
				.Returns(false);

			this.ticketService.Setup(t => t.IsTicketOwner(It.IsAny<int>(), It.IsAny<string>()))
				.Returns(true);

			this.ticketService.Setup(t => t.TicketExists(It.IsAny<int>()))
				.Returns(true);

			this.ticketService.Setup(t => t.CancelTicket(It.IsAny<int>(), It.IsAny<string>()))
				.Returns(true);

			this.PrepareTempData();

			var controller = this.PrepareController();

			//Act
			IActionResult result = controller.Cancel(1);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			this.customMessage.Should().Be(WebConstants.Message.TicketCancelationSuccess);
			this.AssertRedirect(UserId, result as RedirectToActionResult);
		}

		[Fact]
		public void Download_WithNotExistingTicketShouldReturnNullAndRedirectToUserTicketsPage()
		{
			//Arrange
			byte[] ticket = null; 

			this.ticketService.Setup(t => t.GetPdfTicket(It.IsAny<int>(), It.IsAny<string>()))
				.Returns(ticket);

			this.PrepareTempData();

			var controller = this.PrepareController();

			//Act
			IActionResult result = controller.Download(1);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			this.customMessage.Should().Be(string.Format(WebConstants.Message.InvalidTicket));
			this.AssertRedirect(UserId, result as RedirectToActionResult);
		}

		[Fact]
		public void Download_WithNotTicketOwnerShouldRedirectToUserTicketsPage()
		{
			//Arrange
			byte[] ticket = null;

			this.ticketService.Setup(t => t.GetPdfTicket(It.IsAny<int>(), It.IsAny<string>()))
				.Returns(ticket);

			this.PrepareTempData();

			var controller = this.PrepareController();

			//Act
			var result = controller.Download(1);

			//Assert
			result.Should().BeOfType<RedirectToActionResult>();
			this.customMessage.Should().Be(string.Format(WebConstants.Message.InvalidTicket));
			this.AssertRedirect(UserId, result as RedirectToActionResult);
		}

		[Fact]
		public void Download_WithExistingTicketShouldReturnFile()
		{
			//Arrange
			byte[] ticket = new byte[100];

			this.ticketService
				.Setup(t => t.GetPdfTicket(It.IsAny<int>(), It.IsAny<string>()))
				.Returns(ticket);

			this.ticketService
				.Setup(t => t.GetTicketDownloadInfo(It.IsAny<int>(), It.IsAny<string>()))
				.Returns(new TicketDownloadInfoServiceModel()
				{
					DepartureTime = "14:30:00",
					EndTown = "Balchik",
					StartTown = "Albena"
				});

			var controller = this.PrepareController();

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

		private TicketsController PrepareController()
		{
			var controller = new TicketsController(this.ticketService.Object, fixture.UserManagerMockInstance.Object)
			{
				ControllerContext = new ControllerContext
				{
					HttpContext = fixture.HttpContextMockInstance.Object
				}
			};

			controller.TempData = this.tempData.Object;

			return controller;
		}
	}
}
