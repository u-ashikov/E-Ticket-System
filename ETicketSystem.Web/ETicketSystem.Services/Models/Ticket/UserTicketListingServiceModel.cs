namespace ETicketSystem.Services.Models.Ticket
{
	using AutoMapper;
	using Common.Automapper;
	using Data.Models;
	using System;

	public class UserTicketListingServiceModel : IMapFrom<Ticket>, IHaveCustomMapping
    {
		public int Id { get; set; }

		public string CompanyId { get; set; }

		public string CompanyName { get; set; }

		public string Route { get; set; }

		public DateTime DepartureTime { get; set; }

		public TimeSpan Duration { get; set; }

		public int SeatNumber { get; set; }

		public decimal Price { get; set; }

		public void ConfigureMapping(Profile mapper)
		{
			mapper.CreateMap<Ticket, UserTicketListingServiceModel>()
				.ForMember(dest => dest.CompanyId, cfg => cfg.MapFrom(src => src.Route.CompanyId))
				.ForMember(dest => dest.CompanyName, cfg => cfg.MapFrom(src => src.Route.Company.Name))
				.ForMember(dest => dest.Route, cfg => cfg.MapFrom(src => $"{src.Route.StartStation.Town.Name} > {src.Route.EndStation.Town.Name}"))
				.ForMember(dest => dest.Price, cfg => cfg.MapFrom(src => src.Route.Price))
				.ForMember(dest => dest.Duration, cfg => cfg.MapFrom(src => src.Route.Duration));
		}
	}
}
