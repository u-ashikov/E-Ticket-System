namespace ETicketSystem.Web
{
	using AutoMapper;
	using Common.Automapper;
	using Common.Constants;
	using Data;
	using Data.Models;
	using Infrastructure.Extensions;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
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

			services.AddRouting(options => options.LowercaseUrls = true);

            services.AddIdentity<User, IdentityRole>()
				.AddEntityFrameworkStores<ETicketSystemDbContext>()
				.AddDefaultTokenProviders();

			services.AddDomainServices();

			services.AddAutoMapper(opt => opt.AddProfile(new AutoMapperProfile()));

			services.AddMvc(options =>
							options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));
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
                app.UseExceptionHandler(WebConstants.Routing.HomeError);
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
