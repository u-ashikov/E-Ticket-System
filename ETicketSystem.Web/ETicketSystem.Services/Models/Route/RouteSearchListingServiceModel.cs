namespace ETicketSystem.Services.Models.Route
{
	using AutoMapper;
	using Data.Models;
	using ETicketSystem.Common.Automapper;

	public class RouteSearchListingServiceModel : RouteBaseInfoServiceModel,IMapFrom<Route>, IHaveCustomMapping
    {
		public byte[] CompanyLogo { get; set; }

		public override void ConfigureMapping(Profile mapper)
		{
			mapper.CreateMap<Route, RouteSearchListingServiceModel>()
				.ForMember(dest => dest.StartStation, cfg => cfg.MapFrom(src => src.StartStation.Name))
				.ForMember(dest => dest.EndStation, cfg => cfg.MapFrom(src => src.EndStation.Name))
				.ForMember(dest => dest.CompanyName, cfg => cfg.MapFrom(src => src.Company.Name));
		}
	}
}
