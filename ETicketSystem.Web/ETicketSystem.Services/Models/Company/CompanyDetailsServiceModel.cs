namespace ETicketSystem.Services.Models.Company
{
	using AutoMapper;
	using Common.Automapper;
	using Common.Constants;
	using Data.Models;
	using Models.Route;
	using System.Collections.Generic;
	using System.Linq;

	public class CompanyDetailsServiceModel : IMapFrom<Company>, IHaveCustomMapping
    {
		public string Name { get; set; }

		public byte[] Logo { get; set; }

		public string Description { get; set; }

		public string Town { get; set; }

		public string Chief { get; set; }

		public string PhoneNumber { get; set; }

		public int TicketsSold { get; set; }

		public IEnumerable<RouteBaseServiceModel> Routes { get; set; }

		public void ConfigureMapping(Profile mapper)
		{
			mapper.CreateMap<Company, CompanyDetailsServiceModel>()
				.ForMember(dest => dest.Town, cfg => cfg.MapFrom(src => src.Town.Name))
				.ForMember(dest => dest.Chief, cfg => cfg.MapFrom(src => $"{src.ChiefFirstName} {src.ChiefLastName}"))
				.ForMember(dest => dest.TicketsSold, cfg => cfg.MapFrom(src => src.Routes.Sum(r => r.Tickets.Count)))
				.ForMember(dest => dest.Routes, cfg => cfg.MapFrom(src => src.Routes.Where(r=>r.Tickets.Count > 0).OrderByDescending(r => r.Tickets.Count).Take(WebConstants.Company.TopRoutesCount)));
		}
	}
}
