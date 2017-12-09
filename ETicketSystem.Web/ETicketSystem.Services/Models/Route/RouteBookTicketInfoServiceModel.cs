namespace ETicketSystem.Services.Models.Route
{
	using AutoMapper;
	using Data.Models;
	using ETicketSystem.Common.Automapper;
	using ETicketSystem.Data.Enums;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class RouteBookTicketInfoServiceModel : RouteBaseInfoServiceModel, IMapFrom<Route>, IHaveCustomMapping
    {
		public BusType BusType { get; set; }

		public IList<int> ReservedTickets { get; set; }

		public override void ConfigureMapping(Profile mapper)
		{
			base.ConfigureMapping(mapper);

			DateTime ticketDate = default(DateTime);

			mapper.CreateMap<Route, RouteBookTicketInfoServiceModel>()
				.ForMember(dest => dest.ReservedTickets, cfg => cfg.MapFrom(src => src.Tickets.Where(t=>t.DepartureTime == ticketDate).Select(t => t.SeatNumber)));
		}
	}
}
