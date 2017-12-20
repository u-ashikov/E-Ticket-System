namespace ETicketSystem.Test.Mock
{
	using ETicketSystem.Data.Models;
	using Microsoft.AspNetCore.Identity;
	using Moq;

	public class UserManagerMock
	{
		public static Mock<UserManager<User>> New
			=> new Mock<UserManager<User>>(
				Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
	}
}
