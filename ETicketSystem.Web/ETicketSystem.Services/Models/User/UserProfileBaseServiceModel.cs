namespace ETicketSystem.Services.Models.User
{
	using Common.Automapper;
	using Data.Models;
	using System.Collections.Generic;

	public class UserProfileBaseServiceModel : IMapFrom<User>
    {
		public string Id { get; set; }

		public string Username { get; set; }

		public string Email { get; set; }

		public IEnumerable<string> Roles { get; set; }
	}
}
