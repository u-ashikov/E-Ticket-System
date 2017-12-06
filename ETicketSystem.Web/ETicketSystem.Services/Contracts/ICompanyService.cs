namespace ETicketSystem.Services.Contracts
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	public interface ICompanyService
    {
		bool IsCompanyNameRegistered(string name);

		bool IsUniqueReferenceNumberRegistered(string uniqueReferenceNumber);

		bool IsCompanyPhoneNumberRegistered(string phoneNumber);
    }
}
