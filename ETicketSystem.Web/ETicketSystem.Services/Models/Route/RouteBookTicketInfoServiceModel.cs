namespace ETicketSystem.Services.Models.Route
{
	using AutoMapper;
	using Common.Automapper;
	using Data.Enums;
	using Data.Models;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class RouteBookTicketInfoServiceModel : RouteBaseInfoServiceModel, IMapFrom<Route>, IHaveCustomMapping
    {
		public BusType BusType { get; set; }

		public int StartTownId { get; set; }

		public int EndTownId { get; set; }

		public IList<int> ReservedTickets { get; set; }

		public override void ConfigureMapping(Profile mapper)
		{
			DateTime ticketDate = default(DateTime);

			mapper.CreateMap<Route, RouteBookTicketInfoServiceModel>()
				.ForMember(dest => dest.ReservedTickets, cfg => cfg.MapFrom(src => src.Tickets.Where(t => t.DepartureTime == ticketDate && !t.IsCancelled).Select(t => t.SeatNumber)))
				.ForMember(dest => dest.StartStation, cfg => cfg.MapFrom(src => src.StartStation.Name))
				.ForMember(dest => dest.EndStation, cfg => cfg.MapFrom(src => src.EndStation.Name))
				.ForMember(dest => dest.StartTownId, cfg => cfg.MapFrom(src => src.StartStation.TownId))
				.ForMember(dest => dest.EndTownId, cfg => cfg.MapFrom(src => src.EndStation.TownId));

			base.ConfigureMapping(mapper);
		}
	}
}
