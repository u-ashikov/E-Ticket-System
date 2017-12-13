namespace ETicketSystem.Services.Implementations
{
	using Contracts;
	using iTextSharp.text;
	using iTextSharp.text.html.simpleparser;
	using iTextSharp.text.pdf;
	using System.IO;

	public class PdfGenerator : IPdfGenerator
	{
		public byte[] GeneratePdfFromHtml(string html)
		{
			var pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 10f);
			var htmlParser = new HtmlWorker(pdfDoc);

			using (var memoryStream = new MemoryStream())
			{
				var writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
				pdfDoc.Open();

				using (var stringReader = new StringReader(html))
				{
					htmlParser.Parse(stringReader);
				}

				pdfDoc.Close();

				return memoryStream.ToArray();
			}
		}
	}
}
