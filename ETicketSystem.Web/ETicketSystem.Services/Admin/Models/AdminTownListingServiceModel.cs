namespace ETicketSystem.Services.Admin.Models
{
	using AutoMapper;
	using Common.Automapper;
	using Data.Models;

	public class AdminTownListingServiceModel : IMapFrom<Town>, IHaveCustomMapping
    {
		public int Id { get; set; }

		public string Name { get; set; }

		public int DomesticCompanies { get; set; }

		public int Stations { get; set; }

		public void ConfigureMapping(Profile mapper)
		{
			mapper.CreateMap<Town, AdminTownListingServiceModel>()
				.ForMember(dest => dest.DomesticCompanies, cfg => cfg.MapFrom(src => src.Companies.Count))
				.ForMember(dest => dest.Stations, cfg => cfg.MapFrom(src => src.Stations.Count));
		}
	}
}
