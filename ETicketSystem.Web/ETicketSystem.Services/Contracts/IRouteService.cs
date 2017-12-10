namespace ETicketSystem.Services.Contracts
{
	using ETicketSystem.Services.Models.Route;
	using System;
	using System.Collections.Generic;

	public interface IRouteService
    {
		IEnumerable<RouteSearchListingServiceModel> GetSearchedRoutes(int startTown, int endTown, DateTime date);

		RouteBookTicketInfoServiceModel GetRouteTicketBookingInfo(int id, DateTime date);

		bool RouteExists(int id, TimeSpan departureTime);
	}
}
