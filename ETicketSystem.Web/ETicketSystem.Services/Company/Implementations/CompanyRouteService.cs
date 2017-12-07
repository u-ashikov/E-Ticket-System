namespace ETicketSystem.Services.Company.Implementations
{
	using Contracts;
	using Data;
	using Data.Enums;
	using Data.Models;
	using Microsoft.EntityFrameworkCore;
	using System;
	using System.Linq;

	public class CompanyRouteService : ICompanyRouteService
    {
		private readonly ETicketSystemDbContext db;

		public CompanyRouteService(ETicketSystemDbContext db)
		{
			this.db = db;
		}

		public bool Add(int startStation, int endStation, TimeSpan departureTime, TimeSpan duration, BusType busType, decimal price, string companyId)
		{
			var company = this.db.Companies
				.Include(c=>c.Routes)
				.FirstOrDefault(c=>c.Id == companyId);

			if (company.Routes.Any(r=> r.StartStationId == startStation && r.EndStationId == endStation && r.DepartureTime == departureTime))
			{
				return false;
			}

			company.Routes.Add(new Route()
			{
				StartStationId = startStation,
				EndStationId = endStation,
				DepartureTime = departureTime,
				Duration = duration,
				BusType = busType,
				Price = price,
				IsActive = true
			});

			this.db.SaveChanges();
			return true;
		}
	}
}
