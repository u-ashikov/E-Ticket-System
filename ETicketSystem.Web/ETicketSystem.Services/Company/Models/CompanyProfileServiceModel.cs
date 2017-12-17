namespace ETicketSystem.Services.Company.Models
{
	using AutoMapper;
	using Common.Automapper;
	using Data.Models;

	public class CompanyProfileServiceModel : IMapFrom<Company>, IHaveCustomMapping
    {
		public string Id { get; set; }

		public string Username { get; set; }

		public string Email { get; set; }

		public byte[] Logo { get; set; }

		public string Description { get; set; }

		public string PhoneNumber { get; set; }

		public void ConfigureMapping(Profile mapper)
		{
			mapper.CreateMap<Company, CompanyProfileServiceModel>()
				.ForMember(dest => dest.PhoneNumber, cfg => cfg.MapFrom(src => src.PhoneNumber));
		}
	}
}
