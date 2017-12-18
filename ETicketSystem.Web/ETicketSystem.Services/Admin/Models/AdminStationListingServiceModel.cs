namespace ETicketSystem.Services.Admin.Models
{
	using AutoMapper;
	using Common.Automapper;
	using Data.Models;

	public class AdminStationListingServiceModel : IMapFrom<Station>, IHaveCustomMapping
    {
		public int Id { get; set; }

		public string Town { get; set; }

		public string Name { get; set; }

		public string Phone { get; set; }

		public void ConfigureMapping(Profile mapper)
		{
			mapper.CreateMap<Station, AdminStationListingServiceModel>()
				.ForMember(dest => dest.Town, cfg => cfg.MapFrom(src => src.Town.Name));
		}
	}
}
