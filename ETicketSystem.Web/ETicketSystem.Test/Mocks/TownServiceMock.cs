namespace ETicketSystem.Test.Mocks
{
	using ETicketSystem.Services.Contracts;
	using Moq;

	public class TownServiceMock
    {
		public static Mock<ITownService> New =>
			new Mock<ITownService>();
    }
}
