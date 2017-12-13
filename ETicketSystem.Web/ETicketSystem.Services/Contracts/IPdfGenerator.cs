namespace ETicketSystem.Services.Contracts
{
	public interface IPdfGenerator
    {
		byte[] GeneratePdfFromHtml(string html);
    }
}
