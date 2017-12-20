namespace ETicketSystem.Test.Mock
{
	using ETicketSystem.Services.Contracts;
	using Moq;

	public class TicketServiceMock
    {
		public static Mock<ITicketService> New
			=> new Mock<ITicketService>();
	}
}
