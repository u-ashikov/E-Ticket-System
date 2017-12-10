namespace ETicketSystem.Web.Infrastructure.Extensions
{
	using System.Text.RegularExpressions;

	public static class StringExtensions
    {
		public static string ToFormatedPrice(this decimal price) =>
			price.ToString("0.00");

		public static string ToFriendlyUrl(this string text)
			=> Regex.Replace(text, @"[^A-Za-z0-9_\.~]+", "-").ToLower();
	}
}
