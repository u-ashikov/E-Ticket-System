namespace ETicketSystem.Services.Contracts
{
	using ETicketSystem.Data;
	using System;
	using System.Collections.Generic;
	using System.Text;

	public interface IReviewService
    {
		bool Add(string companyId, string userId, string description);
    }
}
