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
	using System;
	using System.IO;
	using System.Linq;
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
						adminUser = new RegularUser()
						{
							Email = WebConstants.Admin.Email,
							FirstName = WebConstants.Admin.FirstName,
							LastName = WebConstants.Admin.LastName,
							Gender = Gender.Male,
							UserName = WebConstants.Admin.Username
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

		public static IApplicationBuilder Seed(this IApplicationBuilder app)
		{
			using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
			{
				var db = serviceScope.ServiceProvider.GetRequiredService<ETicketSystemDbContext>();

				if (File.Exists(WebConstants.FilePath.Towns))
				{
					var towns = File.ReadAllText(WebConstants.FilePath.Towns).Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

					for (int i = 1; i < towns.Length; i++)
					{
						var town = new Town()
						{
							Name = towns[i]
						};

						Task.Run(async () =>
						{
							var townExists = await db.Towns.AnyAsync(t => t.Name.ToLower() == towns[i].ToLower());

							if (!townExists)
							{
								await db.Towns.AddAsync(town);
								await db.SaveChangesAsync();
							}
						})
						.Wait();
					}
				}

				if (File.Exists(WebConstants.FilePath.Stations))
				{
					var stations = File.ReadAllText(WebConstants.FilePath.Stations).Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

					for (int i = 1; i < stations.Length; i++)
					{
						var stationInfo = stations[i].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

						var stationName = stationInfo[0];
						var townId = int.Parse(stationInfo[1]);
						var phone = stationInfo[2];

						var station = new Station()
						{
							Name = stationName,
							TownId = townId,
							Phone = phone
						};

						Task.Run(async () =>
						{
							var townExists = await db.Towns.AnyAsync(t => t.Id == townId);

							if (townExists)
							{
								var town = db.Towns
									.Include(t => t.Stations)
									.FirstOrDefault(t => t.Id == townId);

								var stationExists = town.Stations.Any(s => s.Name.ToLower() == stationName.ToLower() && s.Phone == phone);

								if (!stationExists)
								{
									await db.Stations.AddAsync(station);
									await db.SaveChangesAsync();
								}
							}
						})
						.Wait();
					}
				}
			}

			return app;
		}
    }
}
