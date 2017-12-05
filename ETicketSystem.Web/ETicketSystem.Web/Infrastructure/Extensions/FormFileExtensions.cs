namespace ETicketSystem.Web.Infrastructure.Extensions
{
	using Microsoft.AspNetCore.Http;
	using System.IO;
	using System.Threading.Tasks;

	public static class FormFileExtensions
    {
		public static byte[] GetFormFileBytes(this IFormFile file)
		{
			if (file == null)
			{
				return null;
			}

			var fileBytes = new byte[file.Length];

			Task.Run(async () =>
			{
				using (var ms = new MemoryStream())
				{
					await file.CopyToAsync(ms);
					fileBytes = ms.ToArray();
				}
			})
			.Wait();

			return fileBytes;
		}
    }
}
