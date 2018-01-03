namespace ETicketSystem.Test.Infrastructure
{
	using ETicketSystem.Common.Constants;
	using FluentAssertions;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.ViewFeatures;
	using Mocks;
	using Moq;

	public abstract class BaseControllerTest
    {
		protected readonly Mock<ITempDataDictionary> tempData = TempDataMock.New;

		protected string customMessage = string.Empty;

		protected void PrepareTempData()
		{
			this.tempData.SetupSet(t => t[WebConstants.TempDataKey.Message] = It.IsAny<string>())
							.Callback((string key, object message) => this.customMessage = message as string);
		}

		protected void AssertRedirectToHome(IActionResult result)
		{
			result.Should().BeOfType<RedirectToActionResult>();
			var model = result.As<RedirectToActionResult>();
			model.ActionName.Should().Be(WebConstants.Action.Index);
			model.ControllerName.Should().Be(WebConstants.Controller.Home);
		}
	}
}
