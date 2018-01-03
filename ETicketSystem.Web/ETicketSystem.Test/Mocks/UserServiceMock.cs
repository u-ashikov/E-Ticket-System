namespace ETicketSystem.Test.Mocks
{
	using ETicketSystem.Services.Contracts;
	using Moq;

	public static class UserServiceMock
    {
		public static Mock<IUserService> New =>
			new Mock<IUserService>();
    }
}
