namespace ETicketSystem.Services.Admin.Contracts
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	public interface IAdminStationService
    {
		bool Add(string name, int townId, string phone);
    }
}
