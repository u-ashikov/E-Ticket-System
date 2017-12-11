namespace ETicketSystem.Web.Infrastructure.Extensions
{
	using Common.Constants;
	using Common.Enums;
	using Data;
	using Data.Enums;
	using Data.Models;
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
					foreach (var roleName in Enum.GetNames(typeof(Role)))
					{
						var roleExists = await roleManager.RoleExistsAsync(roleName);

						if (!roleExists)
						{
							await roleManager.CreateAsync(new IdentityRole()
							{
								Name = roleName
							});
						}
					}

					var adminRole = Role.Administrator.ToString();
					var adminUser = await userManager.FindByEmailAsync(AdminConstants.Email);

					if (adminUser == null)
					{
						adminUser = new RegularUser()
						{
							Email = AdminConstants.Email,
							FirstName = AdminConstants.FirstName,
							LastName = AdminConstants.LastName,
							Gender = Gender.Male,
							UserName = AdminConstants.Username
						};

						await userManager.CreateAsync(adminUser, AdminConstants.Password);

						await userManager.AddToRoleAsync(adminUser, adminRole);
					}
				})
				.GetAwaiter()
				.GetResult();
			}

			return app;
		}

		//public static IApplicationBuilder Seed(this IApplicationBuilder app)
		//{
		//	using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
		//	{
		//		var db = serviceScope.ServiceProvider.GetRequiredService<ETicketSystemDbContext>();

		//		var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
		//		var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

		//		if (File.Exists(WebConstants.FilePath.Towns))
		//		{
		//			var towns = File.ReadAllText(WebConstants.FilePath.Towns).Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

		//			for (int i = 1; i < towns.Length; i++)
		//			{
		//				var town = new Town()
		//				{
		//					Name = towns[i]
		//				};

		//				Task.Run(async () =>
		//				{
		//					var townExists = await db.Towns.AnyAsync(t => t.Name.ToLower() == towns[i].ToLower());

		//					if (!townExists)
		//					{
		//						await db.Towns.AddAsync(town);
		//						await db.SaveChangesAsync();
		//					}
		//				})
		//				.Wait();
		//			}
		//		}

		//		if (File.Exists(WebConstants.FilePath.Stations))
		//		{
		//			var stations = File.ReadAllText(WebConstants.FilePath.Stations).Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

		//			for (int i = 1; i < stations.Length; i++)
		//			{
		//				var stationInfo = stations[i].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

		//				var stationName = stationInfo[0];
		//				var townId = int.Parse(stationInfo[1]);
		//				var phone = stationInfo[2];

		//				var station = new Station()
		//				{
		//					Name = stationName,
		//					TownId = townId,
		//					Phone = phone
		//				};

		//				Task.Run(async () =>
		//				{
		//					var townExists = await db.Towns.AnyAsync(t => t.Id == townId);

		//					if (townExists)
		//					{
		//						var town = db.Towns
		//							.Include(t => t.Stations)
		//							.FirstOrDefault(t => t.Id == townId);

		//						var stationExists = town.Stations.Any(s => s.Name.ToLower() == stationName.ToLower() && s.Phone == phone);

		//						if (!stationExists)
		//						{
		//							await db.Stations.AddAsync(station);
		//							await db.SaveChangesAsync();
		//						}
		//					}
		//				})
		//				.Wait();
		//			}
		//		}

		//		if (File.Exists(WebConstants.FilePath.Companies))
		//		{
		//			var companies = File.ReadAllText(WebConstants.FilePath.Companies).Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

		//			var random = new Random();
		//			var townsCount = db.Towns.Count();
		//			var firstTownId = db.Towns.First().Id;
		//			var statiosnCount = db.Stations.Count();
		//			var firstStationId = db.Stations.First().Id;

		//			for (int i = 1; i < companies.Length; i++)
		//			{
		//				var companyInfo = companies[i].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

		//				var password = companyInfo[2];

		//				var company = new Company()
		//				{
		//					UserName = companyInfo[0],
		//					Email = companyInfo[1],
		//					Name = companyInfo[3],
		//					Logo = File.ReadAllBytes(WebConstants.FilePath.CompaniesImages + companyInfo[4]),
		//					Description = companyInfo[5],
		//					UniqueReferenceNumber = companyInfo[6],
		//					ChiefFirstName = companyInfo[7],
		//					ChiefLastName = companyInfo[8],
		//					Address = companyInfo[9],
		//					PhoneNumber = companyInfo[10],
		//					TownId = random.Next(firstTownId, townsCount)
		//				};

		//				Task.Run(async () =>
		//				{
		//					var companyExists = await db.Companies.AnyAsync(c => c.Name.ToLower() == company.Name.ToLower() && c.UserName == company.UserName);

		//					if (!companyExists)
		//					{
		//						await userManager.CreateAsync(company, password);
		//						await userManager.AddToRoleAsync(company, Role.Company.ToString());

		//						var currentCompany = db.Companies
		//												.Where(c => c.UserName == company.UserName)
		//												.Include(c => c.Routes)
		//												.FirstOrDefault();

		//						for (int r = 1; r <= statiosnCount * 4; r++)
		//						{
		//							var startStationId = random.Next(firstStationId, firstStationId + statiosnCount);
		//							var endStationId = random.Next(firstStationId, firstStationId + statiosnCount);
		//							var departureTime = new TimeSpan(random.Next(0, 23), random.Next(0, 59), 0);
		//							var duration = new TimeSpan(random.Next(0, 23), random.Next(0, 59), 0);

		//							if (startStationId == endStationId)
		//							{
		//								continue;
		//							}

		//							var route = new Route()
		//							{
		//								BusType = r % 2 == 0 ? BusType.Mini : BusType.Standart,
		//								StartStationId = startStationId,
		//								EndStationId = endStationId,
		//								Price = random.Next(DataConstants.Route.PriceMinValue, DataConstants.Route.PriceMaxValue),
		//								DepartureTime = departureTime,
		//								Duration = duration,
		//								IsActive = true
		//							};

		//							if (!currentCompany.Routes.Any(cr => cr.StartStationId == startStationId && cr.EndStationId == endStationId))
		//							{
		//								currentCompany.Routes.Add(route);
		//								db.SaveChanges();
		//							}
		//						}
		//					}
		//				})
		//				.Wait();
		//			}
		//		}
		//	}
		//	return app;
		//}
    }
}
