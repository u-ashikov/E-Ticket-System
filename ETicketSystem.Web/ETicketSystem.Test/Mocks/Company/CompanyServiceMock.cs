namespace ETicketSystem.Test.Mocks.Company
{
    using Moq;
    using Services.Contracts;

    public static class CompanyServiceMock
    {
        public static Mock<ICompanyService> New =>
            new Mock<ICompanyService>();
    }
}
