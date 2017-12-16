namespace ETicketSystem.Data.Models
{
	using Common.Constants;
	using System;
	using System.ComponentModel.DataAnnotations;

	public class Ticket
    {
		public int Id { get; set; }

		public int RouteId { get; set; }

		public Route Route { get; set; }

		[Range(DataConstants.Ticket.SeatMinValue,DataConstants.Ticket.SeatMaxValue)]
		public int SeatNumber { get; set; }

		public string UserId { get; set; }

		public RegularUser User { get; set; }

		public DateTime DepartureTime { get; set; }

		public bool IsCancelled { get; set; }
	}
}
