namespace ETicketSystem.Services.Company.Models
{
	using Common.Automapper;
	using Data.Models;

	public class CompanyProfileBaseServiceModel : IMapFrom<Company>
    {
		public string Id { get; set; }

		public string Name { get; set; }

		public byte[] Logo { get; set; }
    }
}
