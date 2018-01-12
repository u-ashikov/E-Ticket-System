namespace ETicketSystem.Test.Mocks.Company
{
    using Moq;
    using Services.Contracts;

    public static class UserServiceMock
    {
        public static Mock<IUserService> New =>
            new Mock<IUserService>();
    }
}
