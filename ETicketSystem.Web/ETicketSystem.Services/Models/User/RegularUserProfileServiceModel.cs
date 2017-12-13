namespace ETicketSystem.Services.Models.User
{
	using AutoMapper;
	using Common.Automapper;
	using Data.Models;

	public class RegularUserProfileServiceModel : UserProfileBaseServiceModel,IMapFrom<RegularUser>,IHaveCustomMapping
    {
		public string FirstName { get; set; }

		public string LastName { get; set; }

		public void ConfigureMapping(Profile mapper)
		{
			mapper.CreateMap<RegularUser, RegularUserProfileServiceModel>()
				.ForMember(dest => dest.FirstName, cfg => cfg.MapFrom(src => src.FirstName))
				.ForMember(dest => dest.LastName, cfg => cfg.MapFrom(src => src.LastName));
		}
	}
}
