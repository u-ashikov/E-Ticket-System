namespace ETicketSystem.Services.Models.Route
{
	using AutoMapper;
	using Data.Models;
	using ETicketSystem.Common.Automapper;

	public class RouteSearchListingServiceModel : RouteBaseInfoServiceModel,IMapFrom<Route>, IHaveCustomMapping
    {
		public byte[] CompanyLogo { get; set; }

		public override void ConfigureMapping(Profile mapper)
		{
			base.ConfigureMapping(mapper);

			mapper.CreateMap<Route, RouteSearchListingServiceModel>()
				.ForMember(dest => dest.CompanyLogo, cfg => cfg.MapFrom(src => src.Company.Logo));
		}
	}
}
