namespace ETicketSystem.Test.Web.Controllers
{
	using ETicketSystem.Services.Contracts;
	using ETicketSystem.Web.Controllers;
	using ETicketSystem.Web.Models.Routes;
	using FluentAssertions;
	using Microsoft.AspNetCore.Mvc;
	using Mocks;
	using Moq;
	using Xunit;

	public class HomeControllerTest
    {
		private readonly Mock<ITownService> townService = TownServiceMock.New;

		[Fact]
		public void Index_ShouldReturnSearchFormWithTowns()
		{
			//Arrange
			var controller = new HomeController(this.townService.Object);

			//Act
			var result = controller.Index();

			//Assert
			result.Should().BeOfType<ViewResult>();
			var model = result.As<ViewResult>().Model;
			model.Should().BeOfType<SearchRouteFormModel>();
		}
    }
}
