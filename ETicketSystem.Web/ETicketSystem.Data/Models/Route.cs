namespace ETicketSystem.Data.Models
{
	using Enums;
	using ETicketSystem.Common.Constants;
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.DataAnnotations;

	public class Route
    {
		public int Id { get; set; }

		public int StartStationId { get; set; }

		public Station StartStation { get; set; }

		public int EndStationId { get; set; }

		public Station EndStation { get; set; }

		[Range(typeof(TimeSpan),DataConstants.Route.TimeMinValue,DataConstants.Route.TimeMaxValue)]
		public TimeSpan DepartureTime { get; set; }

		[Range(typeof(TimeSpan), DataConstants.Route.TimeMinValue, DataConstants.Route.TimeMaxValue)]
		public TimeSpan ArrivalTime { get; set; }

		[Required]
		public BusType BusType { get; set; }

		[Range(DataConstants.Route.PriceMinValue,double.MaxValue)]
		public decimal Price { get; set; }

		public string CompanyId { get; set; }

		public Company Company { get; set; }

		public List<Ticket> Tickets { get; set; }
	}
}
