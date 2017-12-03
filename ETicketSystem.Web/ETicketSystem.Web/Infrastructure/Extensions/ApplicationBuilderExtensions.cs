namespace ETicketSystem.Web.Infrastructure.Extensions
{
	using ETicketSystem.Common.Constants;
	using ETicketSystem.Data;
	using ETicketSystem.Data.Enums;
	using ETicketSystem.Data.Models;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.Extensions.DependencyInjection;
	using System.Threading.Tasks;

	public static class ApplicationBuilderExtensions
    {
		public static IApplicationBuilder UseDatabaseMigration(this IApplicationBuilder app)
		{
			using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
			{
				serviceScope.ServiceProvider.GetRequiredService<ETicketSystemDbContext>().Database.Migrate();

				var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
				var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

				Task.Run(async () =>
				{
					var roleExists = await roleManager.RoleExistsAsync(WebConstants.Admin.Role);

					if (!roleExists)
					{
						await roleManager.CreateAsync(new IdentityRole()
						{
							Name = WebConstants.Admin.Role
						});
					}

					var adminUser = await userManager.FindByEmailAsync(WebConstants.Admin.Email);

					if (adminUser == null)
					{
						adminUser = new User()
						{
							Email = WebConstants.Admin.Email,
							FirstName = WebConstants.Admin.FirstName,
							LastName = WebConstants.Admin.LastName,
							Gender = Gender.Male,
							UserName = WebConstants.Admin.Email
						};

						await userManager.CreateAsync(adminUser, WebConstants.Admin.Password);

						await userManager.AddToRoleAsync(adminUser, WebConstants.Admin.Role);
					}
				})
				.GetAwaiter()
				.GetResult();
			}

			return app;
		}
    }
}
