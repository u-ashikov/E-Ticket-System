namespace ETicketSystem.Test.Web.Controllers.Fixtures
{
	using Data.Models;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Identity;
	using Mock;
	using Moq;
	using System.Security.Claims;

	public class UserManagerGetUserIdFixture
	{
		private const string UserId = "SomeUserId";

		public Mock<UserManager<User>> UserManagerMockInstance { get; private set; }
		public Mock<ClaimsPrincipal> ClaimsPrincipalMockInstance { get; private set; }
		public Mock<HttpContext> HttpContextMockInstance { get; private set; }

		public UserManagerGetUserIdFixture()
		{
			this.UserManagerMockInstance = UserManagerMock.New;
			this.ClaimsPrincipalMockInstance = ClaimsPrincipalMock.New;
			this.HttpContextMockInstance = HttpContextMock.New;

			this.SetUpMocks();
		}

		private void SetUpMocks()
		{
			this.UserManagerMockInstance
				.Setup(u => u.GetUserId(It.IsAny<ClaimsPrincipal>()))
				.Returns(UserId);

			this.HttpContextMockInstance
				.Setup(m => m.User)
				.Returns(this.ClaimsPrincipalMockInstance.Object);
		}
    }
}
