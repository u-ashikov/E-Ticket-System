namespace ETicketSystem.Services.Company.Models
{
	using AutoMapper;
	using Data.Models;
	using ETicketSystem.Common.Automapper;
	using System.Collections.Generic;

	public class CompanyRoutesServiceModel : IMapFrom<Company>, IHaveCustomMapping
    {
		public IEnumerable<CompanyRouteListingServiceModel> Routes { get; set; }

		public void ConfigureMapping(Profile mapper)
		{
			mapper.CreateMap<Company, CompanyRoutesServiceModel>()
				.ForMember(dest => dest.Routes, cfg => cfg.MapFrom(src => src.Routes));
		}
	}
}
