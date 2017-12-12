namespace ETicketSystem.Services.Models.Route
{
	using AutoMapper;
	using Common.Automapper;
	using Data.Models;

	public class RouteBaseServiceModel : IMapFrom<Route>, IHaveCustomMapping
    {
		public int Id { get; set; }

		public int StartTown { get; set; }

		public string StartTownName { get; set; }

		public int EndTown { get; set; }

		public string EndTownName { get; set; }

		public decimal Price { get; set; }

		public void ConfigureMapping(Profile mapper)
		{
			mapper.CreateMap<Route, RouteBaseServiceModel>()
				.ForMember(dest => dest.StartTown, cfg => cfg.MapFrom(src => src.StartStation.TownId))
				.ForMember(dest => dest.StartTownName, cfg => cfg.MapFrom(src => src.StartStation.Town.Name))
				.ForMember(dest => dest.EndTown, cfg => cfg.MapFrom(src => src.EndStation.TownId))
				.ForMember(dest => dest.EndTownName, cfg => cfg.MapFrom(src => src.EndStation.Town.Name));
		}
	}
}
