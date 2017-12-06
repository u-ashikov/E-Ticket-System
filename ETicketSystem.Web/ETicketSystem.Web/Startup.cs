namespace ETicketSystem.Web
{
	using AutoMapper;
	using ETicketSystem.Common.Automapper;
	using ETicketSystem.Common.Constants;
	using ETicketSystem.Data;
	using ETicketSystem.Data.Models;
	using ETicketSystem.Services.Admin.Contracts;
	using ETicketSystem.Services.Admin.Implementations;
	using ETicketSystem.Services.Company.Contracts;
	using ETicketSystem.Services.Company.Implementations;
	using ETicketSystem.Services.Contracts;
	using ETicketSystem.Services.Implementations;
	using Infrastructure.Extensions;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;

	public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ETicketSystemDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString(WebConstants.DbConnection.DefaultConnection)));

			services.Configure<IdentityOptions>(options =>
			{
				options.Password.RequiredLength = DataConstants.User.PasswordMinLength;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireLowercase = false;
				options.Password.RequireUppercase = false;

				options.User.RequireUniqueEmail = true;
			});

            services.AddIdentity<User, IdentityRole>()
				.AddEntityFrameworkStores<ETicketSystemDbContext>()
				.AddDefaultTokenProviders();

            services.AddTransient<IEmailSender, EmailSender>();
			services.AddTransient<IUserService, UserService>();
			services.AddTransient<ITownService, TownService>();
			services.AddTransient<ICompanyService, CompanyService>();
			services.AddTransient<IAdminCompanyService, AdminCompanyService>();
			services.AddTransient<ICompanyRouteService, CompanyRouteService>();

			services.AddAutoMapper(opt => opt.AddProfile(new AutoMapperProfile()));

			services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
			app.UseDatabaseMigration();

			//app.Seed();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
