namespace ETicketSystem.Test.Mocks
{
	using ETicketSystem.Services.Contracts;
	using Moq;

	public class RouteServiceMock
    {
		public static Mock<IRouteService> New =>
			new Mock<IRouteService>();
    }
}
