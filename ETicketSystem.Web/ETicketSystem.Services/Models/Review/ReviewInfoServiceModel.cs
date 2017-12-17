namespace ETicketSystem.Services.Models.Review
{
	using AutoMapper;
	using Common.Automapper;
	using Data.Models;
	using System;

	public class ReviewInfoServiceModel : IMapFrom<Review>, IHaveCustomMapping
    {
		public int Id { get; set; }

		public string CompanyId { get; set; }

		public string Description { get; set; }

		public string UserId { get; set; }

		public string User { get; set; }

		public DateTime PublishDate { get; set; }

		public void ConfigureMapping(Profile mapper)
		{
			mapper.CreateMap<Review, ReviewInfoServiceModel>()
				.ForMember(dest => dest.User, cfg => cfg.MapFrom(src => src.User.UserName));
		}
	}
}
