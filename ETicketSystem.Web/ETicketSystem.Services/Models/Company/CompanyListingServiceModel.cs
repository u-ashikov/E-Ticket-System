namespace ETicketSystem.Services.Models.Company
{
	using AutoMapper;
	using Data.Models;
	using ETicketSystem.Common.Automapper;

	public class CompanyListingServiceModel : IMapFrom<Company>, IHaveCustomMapping
    {
		public string Id { get; set; }

		public string Name { get; set; }

		public string UniqueReferenceNumber { get; set; }

		public string Address { get; set; }

		public string Chief { get; set; }

		public string Email { get; set; }

		public string PhoneNumber { get; set; }

		public bool IsApproved { get; set; }

		public void ConfigureMapping(Profile mapper)
		{
			mapper.CreateMap<Company, CompanyListingServiceModel>()
				.ForMember(dest => dest.Address, cfg => cfg.MapFrom(src => $"{src.Town.Name}, {src.Address}"))
				.ForMember(dest => dest.Chief, cfg => cfg.MapFrom(src => $"{src.ChiefFirstName} {src.ChiefLastName}"));
		}
	}
}
