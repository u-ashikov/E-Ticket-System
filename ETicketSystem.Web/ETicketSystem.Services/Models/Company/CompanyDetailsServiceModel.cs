namespace ETicketSystem.Services.Models.Company
{
	using Data.Models;
	using Common.Automapper;
	using System.Collections.Generic;

	public class CompanyDetailsServiceModel : IMapFrom<Company>
    {
		public string Name { get; set; }

		public string Description { get; set; }

		public string Town { get; set; }

		public string Chief { get; set; }
    }
}
