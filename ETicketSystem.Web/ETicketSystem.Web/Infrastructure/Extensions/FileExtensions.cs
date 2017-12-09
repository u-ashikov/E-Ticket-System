namespace ETicketSystem.Web.Infrastructure.Extensions
{
	using Microsoft.AspNetCore.Http;
	using System;
	using System.IO;
	using System.Threading.Tasks;

	public static class FileExtensions
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

		public static string ConvertBytesToImage(this byte[] image)
		{
			if (image == null)
			{
				return string.Empty;
			}

			var base64 = Convert.ToBase64String(image,0,image.Length);
			
			return String.Format("data:image/png;base64,{0}", base64);
		}
    }
}
