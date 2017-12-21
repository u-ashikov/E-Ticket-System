namespace ETicketSystem.Services.Models.Ticket
{
	using AutoMapper;
	using Common.Automapper;
	using Data.Models;
	using System;

	public class TicketPdfServiceModel : IMapFrom<Ticket>, IHaveCustomMapping
    {
		public string Company { get; set; }

		public string Route { get; set; }

		public DateTime DepartureTime { get; set; }

		public int Seat { get; set; }

		public void ConfigureMapping(Profile mapper)
		{
			mapper.CreateMap<Ticket, TicketPdfServiceModel>()
				.ForMember(dest => dest.Company, cfg => cfg.MapFrom(src => src.Route.Company.Name))
				.ForMember(dest => dest.Route, cfg => cfg.MapFrom(src => $"{src.Route.StartStation.Town.Name}, {src.Route.StartStation.Name} -> {src.Route.EndStation.Town.Name}, {src.Route.EndStation.Name}"))
				.ForMember(dest => dest.DepartureTime, cfg => cfg.MapFrom(src => src.DepartureTime))
				.ForMember(dest => dest.Seat, cfg => cfg.MapFrom(src => src.SeatNumber));
		}
	}
}
