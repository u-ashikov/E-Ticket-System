namespace ETicketSystem.Data
{
	using ETicketSystem.Data.Models;
	using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore;

	public class ETicketSystemDbContext : IdentityDbContext<User>
    {
        public ETicketSystemDbContext(DbContextOptions<ETicketSystemDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
