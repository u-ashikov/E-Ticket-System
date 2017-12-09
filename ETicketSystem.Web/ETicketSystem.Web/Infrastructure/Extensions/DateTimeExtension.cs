﻿namespace ETicketSystem.Web.Infrastructure.Extensions
{
	using System;

	public static class DateTimeExtension
    {
		public static string ToHoursAndMinutes(this TimeSpan time)
		{
			return time.ToString(@"hh\:mm");
		}
    }
}
