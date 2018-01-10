namespace ETicketSystem.Test.Mocks.Admin
{
	using ETicketSystem.Services.Admin.Contracts;
	using Moq;

	public static class AdminStationServiceMock
    {
		public static Mock<IAdminStationService> New =>
			new Mock<IAdminStationService>();
    }
}
