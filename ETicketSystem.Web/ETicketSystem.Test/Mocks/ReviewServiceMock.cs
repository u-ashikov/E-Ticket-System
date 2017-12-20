namespace ETicketSystem.Test.Mocks
{
	using ETicketSystem.Services.Contracts;
	using Moq;

	public class ReviewServiceMock
    {
		public static Mock<IReviewService> New =>
			new Mock<IReviewService>();
    }
}
