namespace ETicketSystem.Services.Admin.Models
{
	using AutoMapper;
	using Common.Automapper;
	using Data.Models;

	public class AdminCompanyListingServiceModel : IMapFrom<Company>, IHaveCustomMapping
    {
		public string Id { get; set; }

		public string Name { get; set; }

		public string UniqueReferenceNumber { get; set; }

		public string Address { get; set; }

		public string Chief { get; set; }

		public string Email { get; set; }

		public string PhoneNumber { get; set; }

		public bool IsApproved { get; set; }

		public bool IsBlocked { get; set; }

		public void ConfigureMapping(Profile mapper)
		{
			mapper.CreateMap<Company, AdminCompanyListingServiceModel>()
				.ForMember(dest => dest.Address, cfg => cfg.MapFrom(src => $"{src.Town.Name}, {src.Address}"))
				.ForMember(dest => dest.Chief, cfg => cfg.MapFrom(src => $"{src.ChiefFirstName} {src.ChiefLastName}"));
		}
	}
}
