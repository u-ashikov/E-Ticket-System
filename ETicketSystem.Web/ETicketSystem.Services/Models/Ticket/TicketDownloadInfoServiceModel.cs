namespace ETicketSystem.Services.Models.Ticket
{
	using AutoMapper;
	using Common.Automapper;
	using Data.Models;

	public class TicketDownloadInfoServiceModel : IMapFrom<Ticket>, IHaveCustomMapping
    {
		public string StartTown { get; set; }

		public string EndTown { get; set; }

		public string DepartureTime { get; set; }

		public void ConfigureMapping(Profile mapper)
		{
			mapper.CreateMap<Ticket, TicketDownloadInfoServiceModel>()
				.ForMember(dest => dest.StartTown, cfg => cfg.MapFrom(src => src.Route.StartStation.Town.Name))
				.ForMember(dest => dest.EndTown, cfg => cfg.MapFrom(src => src.Route.EndStation.Town.Name))
				.ForMember(dest => dest.DepartureTime, cfg => cfg.MapFrom(src => $"{src.DepartureTime.Day}{src.DepartureTime.Month}{src.DepartureTime.Year}"));
		}
	}
}
