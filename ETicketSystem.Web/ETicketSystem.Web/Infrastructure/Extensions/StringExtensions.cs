namespace ETicketSystem.Web.Infrastructure.Extensions
{
	public static class StringExtensions
    {
		public static string ToFormatedPrice(this decimal price)
		{
			return price.ToString("0.00");
		}
    }
}
