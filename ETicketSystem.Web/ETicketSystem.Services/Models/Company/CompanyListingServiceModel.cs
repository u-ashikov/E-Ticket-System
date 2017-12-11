namespace ETicketSystem.Services.Models.Company
{
	using Common.Automapper;
	using Data.Models;

	public class CompanyListingServiceModel : IMapFrom<Company>
    {
		public string Id { get; set; }

		public string Name { get; set; }

		public byte[] Logo { get; set; }

		public string Description { get; set; }
	}
}
