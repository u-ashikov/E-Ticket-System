namespace ETicketSystem.Test.Mocks.Company
{
    using Moq;
    using Services.Contracts;

    public static class StationServiceMock
    {
        public static Mock<IStationService> New =>
            new Mock<IStationService>();
    }
}
