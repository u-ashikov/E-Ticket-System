namespace ETicketSystem.Services.Company.Implementations
{
	using AutoMapper.QueryableExtensions;
	using Contracts;
	using Data;
	using Data.Enums;
	using Data.Models;
	using ETicketSystem.Services.Company.Models;
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

			if (this.RouteAlreadyExist(startStation,endStation,departureTime,companyId))
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

		public CompanyRoutesServiceModel All(string companyId) =>
			this.db
				.Companies
				.Include(c => c.Routes)
				.Where(c => c.Id == companyId)
				.ProjectTo<CompanyRoutesServiceModel>()
				.FirstOrDefault();

		public CompanyRouteEditServiceModel GetRouteToEdit(string companyId, int routeId) =>
			this.db
				.Routes
				.Where(r => r.Id == routeId && r.CompanyId == companyId)
				.ProjectTo<CompanyRouteEditServiceModel>()
				.FirstOrDefault();

		public bool Edit(int routeId, int startStation, int endStation, TimeSpan departureTime, TimeSpan duration, BusType busType, decimal price, string companyId)
		{
			var route = this.db.Routes.FirstOrDefault(r => r.Id == routeId && r.CompanyId == companyId);

			if (route == null)
			{
				return false;
			}

			route.DepartureTime = departureTime;
			route.Duration = duration;
			route.BusType = busType;
			route.Price = price;

			this.db.SaveChanges();

			return true;
		}

		public bool RouteAlreadyExist(int startStation, int endStation, TimeSpan departureTime, string companyId) =>
			this.db.Routes.Any(r => r.StartStationId == startStation && r.EndStationId == endStation && r.DepartureTime == departureTime && r.CompanyId == companyId);
	}
}
