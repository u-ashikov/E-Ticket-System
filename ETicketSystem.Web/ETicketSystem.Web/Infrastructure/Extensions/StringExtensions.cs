namespace ETicketSystem.Web.Infrastructure.Extensions
{
	using Common.Constants;
	using System.Text.RegularExpressions;

	public static class StringExtensions
    {
		public static string ToFormatedPrice(this decimal price) =>
			price.ToString("0.00");

		public static string ToFriendlyUrl(this string text)
			=> Regex.Replace(text, WebConstants.RegexPattern.FriendlyUrl, "-").ToLower();

		public static string ToShortDescription(this string description)
		{
			var minLength = WebConstants.Company.ShortDescriptionLength;

			if (string.IsNullOrEmpty(description) || description.Length < minLength)
			{
				return description;
			}

			return description.Substring(0, minLength) + "...";
		}
	}
}
