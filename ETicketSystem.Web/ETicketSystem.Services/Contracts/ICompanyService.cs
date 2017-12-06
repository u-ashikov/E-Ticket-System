namespace ETicketSystem.Services.Contracts
{
	public interface ICompanyService
    {
		bool IsCompanyNameRegistered(string name);

		bool IsUniqueReferenceNumberRegistered(string uniqueReferenceNumber);

		bool IsCompanyPhoneNumberRegistered(string phoneNumber);
    }
}
