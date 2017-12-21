namespace ETicketSystem.Data
{
	using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore;
	using Models;

	public class ETicketSystemDbContext : IdentityDbContext<User>
    {
		public DbSet<RegularUser> RegularUsers { get; set; }

		public DbSet<Company> Companies { get; set; }

		public DbSet<Town> Towns { get; set; }

		public DbSet<Station> Stations { get; set; }

		public DbSet<Route> Routes { get; set; }

		public DbSet<Review> Reviews { get; set; }

		public DbSet<Ticket> Tickets { get; set; }

        public ETicketSystemDbContext(DbContextOptions<ETicketSystemDbContext> options)
            : base(options) {}

        protected override void OnModelCreating(ModelBuilder builder)
        {
			builder.Entity<Town>()
				.HasMany(t => t.Stations)
				.WithOne(s => s.Town)
				.HasForeignKey(s => s.TownId);

			builder.Entity<Town>()
				.HasMany(t => t.Companies)
				.WithOne(c=>c.Town)
				.HasForeignKey(c => c.TownId);

			builder.Entity<Company>()
				.HasBaseType<User>();

			builder.Entity<Company>()
				.HasIndex(c => c.UniqueReferenceNumber)
				.IsUnique();

			builder.Entity<Company>()
				.HasIndex(c => c.Name)
				.IsUnique();

			builder.Entity<Company>()
				.HasMany(c => c.Routes)
				.WithOne(r => r.Company)
				.HasForeignKey(r => r.CompanyId);

			builder.Entity<Route>()
				.HasOne(r => r.StartStation)
				.WithMany(s => s.DepartureRoutes)
				.HasForeignKey(r => r.StartStationId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<Route>()
				.HasOne(r => r.EndStation)
				.WithMany(s => s.ArrivalRoutes)
				.HasForeignKey(r => r.EndStationId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<Review>()
				.HasOne(r => r.User)
				.WithMany(u => u.Reviews)
				.HasForeignKey(r => r.UserId);

			builder.Entity<Review>()
				.HasOne(r => r.Company)
				.WithMany(c => c.Reviews)
				.HasForeignKey(r => r.CompanyId);

			builder.Entity<Ticket>()
				.HasOne(t => t.User)
				.WithMany(u => u.Tickets)
				.HasForeignKey(t => t.UserId);

			builder.Entity<Ticket>()
				.HasOne(t => t.Route)
				.WithMany(r => r.Tickets)
				.HasForeignKey(t => t.RouteId);

			builder.Entity<RegularUser>()
				.HasBaseType<User>();

            base.OnModelCreating(builder);
        }
    }
}
