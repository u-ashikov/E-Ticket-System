namespace ETicketSystem.Web.Models.Routes
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	public class BookSeatViewModel
    {
		public int Value { get; set; }

		public string Text { get; set; }

		public bool Checked { get; set; }

		public bool Disabled { get; set; }
	}
}
