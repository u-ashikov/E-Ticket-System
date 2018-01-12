namespace ETicketSystem.Test.Mocks.Company
{
    using Moq;
    using Services.Company.Contracts;

    public static class CompanyRouteServiceMock
    {
        public static Mock<ICompanyRouteService> New =>
            new Mock<ICompanyRouteService>();
    }
}
