namespace ETicketSystem.Services.Company.Models
{
	using AutoMapper;
	using Data.Models;
	using ETicketSystem.Common.Automapper;
	using System;

	public class CompanyRouteBaseSerivceModel : IMapFrom<Route>, IHaveCustomMapping
    {
		public string StartStationTownName { get; set; }

		public string EndStationTownName { get; set; }

		public TimeSpan DepartureTime { get; set; }

		public string Status { get; set; }

		public void ConfigureMapping(Profile mapper)
		{
			mapper.CreateMap<Route, CompanyRouteBaseSerivceModel>()
				.ForMember(dest => dest.StartStationTownName, cfg => cfg.MapFrom(src => src.StartStation.Town.Name))
				.ForMember(dest => dest.EndStationTownName, cfg => cfg.MapFrom(src => src.EndStation.Town.Name))
				.ForMember(dest => dest.Status, cfg => cfg.MapFrom(src => src.IsActive ? "acitvated" : "deactivated"));
		}
	}
}
