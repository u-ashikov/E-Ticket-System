namespace ETicketSystem.Test.Mocks
{
	using Microsoft.AspNetCore.Mvc.ViewFeatures;
	using Moq;

	public static class TempDataMock
    {
		public static Mock<ITempDataDictionary> New =>
			new Mock<ITempDataDictionary>();
    }
}
