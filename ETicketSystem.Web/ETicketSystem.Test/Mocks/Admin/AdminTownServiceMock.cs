namespace ETicketSystem.Test.Mocks.Admin
{
    using Moq;
    using Services.Admin.Contracts;

    public static class AdminTownServiceMock
    {
        public static Mock<IAdminTownService> New =>
            new Mock<IAdminTownService>();
    }
}
