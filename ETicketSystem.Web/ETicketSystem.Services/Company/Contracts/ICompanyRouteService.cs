namespace ETicketSystem.Services.Company.Contracts
{
	using Data.Enums;
	using Models;
	using System;

	public interface ICompanyRouteService
    {
		bool Add(int startStation, int endStation, TimeSpan departureTime, TimeSpan duration, BusType busType, decimal price, string companyId);

		bool Edit(int routeId,int startStation, int endStatio, TimeSpan departureTime, TimeSpan duration, BusType busType, decimal price, string companyId);

		bool ChangeStatus(int routeId, string companyId);

		CompanyRoutesServiceModel All(int startTown, int endTown, DateTime? date,string companyId, int page, int pageSize = 10);

		CompanyRouteEditServiceModel GetRouteToEdit(string companyId, int routeId);

		CompanyRouteBaseSerivceModel GetRouteBaseInfo(int routeId, string companyId);

		bool RouteAlreadyExist(int routeId,int startStation, int endStation, TimeSpan departureTime, string companyId);

		int TotalRoutes(int startTown, int endTown, DateTime? date,string companyId);

		bool HasReservedTickets(int routeId, string companyId);

		bool IsRouteOwner(int id, string companyId);
    }
}
