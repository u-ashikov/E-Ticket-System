namespace ETicketSystem.Services.Models.User
{
	using AutoMapper;
	using Common.Automapper;
	using Data.Models;

	public class RegularUserProfileServiceModel : UserProfileBaseServiceModel,IMapFrom<RegularUser>, IHaveCustomMapping
    {
		public string Name { get; set; }

		public void ConfigureMapping(Profile mapper)
		{
			mapper.CreateMap<RegularUser, RegularUserProfileServiceModel>()
				.ForMember(dest => dest.Name, cfg => cfg.MapFrom(src => $"{src.FirstName} {src.LastName}"));
		}
	}
}
