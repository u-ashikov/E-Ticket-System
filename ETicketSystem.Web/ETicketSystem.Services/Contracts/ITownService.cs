namespace ETicketSystem.Services.Contracts
{
	using ETicketSystem.Services.Models.Town;
	using System.Collections.Generic;

	public interface ITownService
    {
		IEnumerable<TownBaseServiceModel> GetTownsListItems();
    }
}
