﻿namespace ETicketSystem.Test.Mocks
{
	using Microsoft.AspNetCore.Http;
	using Moq;

	public class HttpContextMock
    {
		public static Mock<HttpContext> New
			=> new Mock<HttpContext>();
	}
}
