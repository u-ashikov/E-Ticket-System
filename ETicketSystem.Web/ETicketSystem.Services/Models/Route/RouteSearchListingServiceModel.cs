namespace ETicketSystem.Services.Models.Route
{
	using AutoMapper;
	using Data.Models;
	using Common.Automapper;
	using System;
	using System.Linq;

	public class RouteSearchListingServiceModel : RouteBaseInfoServiceModel,IMapFrom<Route>, IHaveCustomMapping
    {
		public byte[] CompanyLogo { get; set; }

		public int TotalBusSeats { get; set; }

		public int ReservedTickets { get; set; }

		public override void ConfigureMapping(Profile mapper)
		{
			mapper.CreateMap<Route, RouteSearchListingServiceModel>()
				.ForMember(dest => dest.StartStation, cfg => cfg.MapFrom(src => src.StartStation.Name))
				.ForMember(dest => dest.EndStation, cfg => cfg.MapFrom(src => src.EndStation.Name))
				.ForMember(dest => dest.CompanyName, cfg => cfg.MapFrom(src => src.Company.Name))
				.ForMember(dest => dest.TotalBusSeats, cfg => cfg.MapFrom(src => (int)src.BusType));
		}
	}
}
