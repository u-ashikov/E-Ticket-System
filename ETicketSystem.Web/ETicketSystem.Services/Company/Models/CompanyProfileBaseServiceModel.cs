namespace ETicketSystem.Services.Company.Models
{
	using AutoMapper;
	using Common.Automapper;
	using Data.Models;
	using System.Linq;

	public class CompanyProfileBaseServiceModel : IMapFrom<Company>, IHaveCustomMapping
    {
		public string Id { get; set; }

		public string Name { get; set; }

		public byte[] Logo { get; set; }

		public string Chief { get; set; }

		public string Email { get; set; }

		public string Phone { get; set; }

		public int TotalRoutes { get; set; }

		public int TotalTicketsSold { get; set; }

		public void ConfigureMapping(Profile mapper)
		{
			mapper.CreateMap<Company, CompanyProfileBaseServiceModel>()
				.ForMember(dest => dest.Phone, cfg => cfg.MapFrom(src => src.PhoneNumber))
				.ForMember(dest => dest.Chief, cfg => cfg.MapFrom(src => $"{src.ChiefFirstName} {src.ChiefLastName}"))
				.ForMember(dest => dest.TotalRoutes, cfg => cfg.MapFrom(src => src.Routes.Count))
				.ForMember(dest => dest.TotalTicketsSold, cfg => cfg.MapFrom(src => src.Routes.Sum(r => r.Tickets.Count)));
		}
	}
}
