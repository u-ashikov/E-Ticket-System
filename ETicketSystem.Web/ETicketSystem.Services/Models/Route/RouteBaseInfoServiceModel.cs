namespace ETicketSystem.Services.Models.Route
{
	using AutoMapper;
	using Data.Models;
	using ETicketSystem.Common.Automapper;
	using System;

	public class RouteBaseInfoServiceModel : IMapFrom<Route>, IHaveCustomMapping
	{
		public int Id { get; set; }

		public string CompanyName { get; set; }

		public TimeSpan DepartureTime { get; set; }

		public TimeSpan Duration { get; set; }

		public string StartStation { get; set; }

		public string EndStation { get; set; }

		public decimal Price { get; set; }

		public virtual void ConfigureMapping(Profile mapper)
		{
			mapper.CreateMap<Route, RouteSearchListingServiceModel>()
				.ForMember(dest => dest.StartStation, cfg => cfg.MapFrom(src => src.StartStation.Name))
				.ForMember(dest => dest.EndStation, cfg => cfg.MapFrom(src => src.EndStation.Name))
				.ForMember(dest => dest.CompanyName, cfg => cfg.MapFrom(src => src.Company.Name));
		}
	}
}
