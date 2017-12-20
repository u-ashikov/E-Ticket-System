namespace ETicketSystem.Test.Mocks
{
	using Moq;
	using System.Security.Claims;

	public class ClaimsPrincipalMock
    {
		public static Mock<ClaimsPrincipal> New
			=> new Mock<ClaimsPrincipal>();
	}
}
