namespace ETicketSystem.Test.Mocks.Admin
{
	using ETicketSystem.Services.Admin.Contracts;
	using Moq;

	public static class AdminCompanyServiceMock
    {
		public static Mock<IAdminCompanyService> New =>
			new Mock<IAdminCompanyService>();
    }
}
