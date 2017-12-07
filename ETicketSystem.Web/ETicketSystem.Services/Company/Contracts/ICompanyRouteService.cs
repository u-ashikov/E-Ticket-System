namespace ETicketSystem.Services.Company.Contracts
{
	using ETicketSystem.Data.Enums;
	using ETicketSystem.Services.Company.Models;
	using System;

	public interface ICompanyRouteService
    {
		bool Add(int startStation, int endStation, TimeSpan departureTime, TimeSpan duration, BusType busType, decimal price, string companyId);

		bool Edit(int routeId,int startStation, int endStatio, TimeSpan departureTime, TimeSpan duration, BusType busType, decimal price, string companyId);

		bool Deactivate(int routeId, string companyId);

		CompanyRoutesServiceModel All(string companyId);

		CompanyRouteEditServiceModel GetRouteToEdit(string companyId, int routeId);

		CompanyRouteBaseSerivceModel GetRouteBaseInfo(int routeId, string companyId);

		bool RouteAlreadyExist(int startStation, int endStation, TimeSpan departureTime, string companyId);
    }
}
