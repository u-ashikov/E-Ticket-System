namespace ETicketSystem.Services.Company.Models
{
	using AutoMapper;
	using Common.Automapper;
	using Data.Enums;
	using Data.Models;
	using System;

	public class CompanyRouteListingServiceModel : IMapFrom<Route>, IHaveCustomMapping
    {
		public int Id { get; set; }

		public int StartTown { get; set; }

		public int EndTown { get; set; }

		public string CompanyId { get; set; }

		public string StartStation { get; set; }

		public string EndStation { get; set; }

		public TimeSpan DepartureTime { get; set; }

		public TimeSpan Duration { get; set; }

		public string BusType { get; set; }

		public decimal Price { get; set; }

		public bool IsActive { get; set; }

		public void ConfigureMapping(Profile mapper)
		{
			mapper.CreateMap<Route, CompanyRouteListingServiceModel>()
				.ForMember(dest => dest.StartStation, cfg => cfg.MapFrom(src => $"{src.StartStation.Town.Name}, {src.StartStation.Name}"))
				.ForMember(dest => dest.EndStation, cfg => cfg.MapFrom(src => $"{src.EndStation.Town.Name}, {src.EndStation.Name}"))
				.ForMember(dest => dest.BusType, cfg => cfg.MapFrom(src => $"{src.BusType.ToString()} - {(int)Enum.Parse(typeof(BusType), src.BusType.ToString())} seats"))
				.ForMember(dest => dest.StartTown, cfg => cfg.MapFrom(src => src.StartStation.TownId))
				.ForMember(dest => dest.EndTown, cfg => cfg.MapFrom(src => src.EndStation.TownId))
				.ForMember(dest => dest.CompanyId, cfg => cfg.MapFrom(src => src.CompanyId));
		}
	}
}
