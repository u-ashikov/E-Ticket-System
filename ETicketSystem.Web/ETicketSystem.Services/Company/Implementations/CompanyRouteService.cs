namespace ETicketSystem.Services.Company.Implementations
{
	using ETicketSystem.Data;
	using ETicketSystem.Services.Company.Contracts;

	public class CompanyRouteService : ICompanyRouteService
    {
		private readonly ETicketSystemDbContext db;

		public CompanyRouteService(ETicketSystemDbContext db)
		{
			this.db = db;
		}
    }
}
