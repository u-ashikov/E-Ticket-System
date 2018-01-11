namespace ETicketSystem.Test.Mocks.Admin
{
    using Moq;
    using Services.Admin.Contracts;

    public static class AdminUserServiceMock
    {
        public static Mock<IAdminUserService> New =>
            new Mock<IAdminUserService>();
    }
}
