namespace ETicketSystem.Services.Admin.Models
{
	using AutoMapper;
	using Common.Automapper;
	using Data.Models;

	public class AdminTownStationsServiceModel : IMapFrom<Station>, IHaveCustomMapping
    {
		public int Id { get; set; }

		public string Name { get; set; }

		public int ArrivingRoutes { get; set; }

		public int DepartingRoutes { get; set; }

		public void ConfigureMapping(Profile mapper)
		{
			mapper.CreateMap<Station, AdminTownStationsServiceModel>()
				.ForMember(dest => dest.ArrivingRoutes, cfg => cfg.MapFrom(src => src.ArrivalRoutes.Count))
				.ForMember(dest => dest.DepartingRoutes, cfg => cfg.MapFrom(src => src.DepartureRoutes.Count));
		}
	}
}
