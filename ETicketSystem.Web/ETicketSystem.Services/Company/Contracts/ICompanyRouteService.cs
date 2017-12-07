namespace ETicketSystem.Services.Company.Contracts
{
	using ETicketSystem.Data.Enums;
	using System;

	public interface ICompanyRouteService
    {
		bool Add(int startStation, int endStation, TimeSpan departureTime, TimeSpan duration, BusType busType, decimal price, string companyId);
    }
}
