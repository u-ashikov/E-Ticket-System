namespace ETicketSystem.Services.Implementations
{
	using ETicketSystem.Data;
	using ETicketSystem.Services.Contracts;

	public class UserService : IUserService
    {
		private readonly ETicketSystemDbContext db;

		public UserService(ETicketSystemDbContext db)
		{
			this.db = db;
		}
    }
}
