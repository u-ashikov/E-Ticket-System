namespace ETicketSystem.Services.Models.Company
{
	using Common.Automapper;
	using Data.Models;

	public class CompanyBaseServiceModel : IMapFrom<Company>
    {
		public string Id { get; set; }

		public string Name { get; set; }
    }
}
