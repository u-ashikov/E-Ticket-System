namespace ETicketSystem.Test.Mock
{
	using ETicketSystem.Services.Contracts;
	using Moq;

	public class CompanyServiceMock
    {
		public static Mock<ICompanyService> New =>
			new Mock<ICompanyService>();
    }
}
