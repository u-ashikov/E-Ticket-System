namespace ETicketSystem.Services.Company.Models
{
	using AutoMapper;
	using Data.Models;
	using ETicketSystem.Common.Automapper;
	using ETicketSystem.Data.Enums;
	using System;

	public class CompanyRouteEditServiceModel : IMapFrom<Route>, IHaveCustomMapping
    {
		public int Id { get; set; }

		public int StartStationId { get; set; }

		public int EndStationId { get; set; }

		public DateTime DepartureTime { get; set; }

		public TimeSpan Duration { get; set; }

		public BusType BusType { get; set; }

		public decimal Price { get; set; }

		public void ConfigureMapping(Profile mapper)
		{
			mapper.CreateMap<Route, CompanyRouteEditServiceModel>()
				.ForMember(dest => dest.DepartureTime, cfg => cfg.MapFrom(src => new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, src.DepartureTime.Hours, src.DepartureTime.Minutes, src.DepartureTime.Seconds)));
		}
	}
}
