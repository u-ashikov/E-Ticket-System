namespace ETicketSystem.Services.Admin.Models
{
	using AutoMapper;
	using Common.Automapper;
	using Data.Models;
	using System.Collections.Generic;

	public class AdminUserListingServiceModel : IMapFrom<RegularUser>, IHaveCustomMapping
    {
		public string Id { get; set; }

		public string Username { get; set; }

		public string Name { get; set; }

		public string Email { get; set; }

		public IList<string> Roles { get; set; }

		public void ConfigureMapping(Profile mapper)
		{
			mapper.CreateMap<RegularUser, AdminUserListingServiceModel>()
				.ForMember(dest => dest.Name, cfg => cfg.MapFrom(src => $"{src.FirstName} {src.LastName}"));
		}
	}
}
