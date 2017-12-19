namespace ETicketSystem.Web.Controllers
{
	using Common.Constants;
	using Common.Enums;
	using Data.Models;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using Services.Contracts;

	[Authorize]
	public class TicketsController : BaseController
    {
		private readonly ITicketService tickets;

		private readonly UserManager<User> userManager;

		public TicketsController(ITicketService tickets, UserManager<User> userManager)
		{
			this.tickets = tickets;
			this.userManager = userManager;
		}

		public IActionResult Cancel(int id)
		{
			var userId = this.userManager.GetUserId(User);

			if (!this.tickets.TicketExists(id) 
				|| !this.tickets.IsTicketOwner(id, userId)
				|| this.tickets.IsCancelled(id))
			{
				this.GenerateAlertMessage(string.Format(WebConstants.Message.NonExistingEntity,WebConstants.Entity.Ticket,id), Alert.Danger);
				return RedirectToAction(WebConstants.Action.MyTickets,WebConstants.Controller.Users,new { id = userId});
			}

			bool success = this.tickets.CancelTicket(id, userId);

			if (!success)
			{
				this.GenerateAlertMessage(WebConstants.Message.TicketCancelationDenied, Alert.Warning);
				return RedirectToAction(WebConstants.Action.MyTickets, WebConstants.Controller.Users, new { id = userId });
			}

			this.GenerateAlertMessage(WebConstants.Message.TicketCancelationSuccess,Alert.Success);

			return RedirectToAction(WebConstants.Action.MyTickets, WebConstants.Controller.Users, new { id = userId });
		}

		public IActionResult Download(int id)
		{
			var userId = this.userManager.GetUserId(User);
			var ticket = this.tickets.GetPdfTicket(id, userId);

			if (ticket == null)
			{
				this.GenerateAlertMessage(WebConstants.Message.InvalidTicket, Alert.Danger);
				return RedirectToAction(WebConstants.Action.MyTickets,WebConstants.Controller.Users, new { id = this.userManager.GetUserId(User) });
			}

			var ticketInfo = this.tickets.GetTicketDownloadInfo(id, userId);

			return File(ticket, WebConstants.ContentType.Pdf, string.Format(WebConstants.Pdf.TicketName,ticketInfo.StartTown, ticketInfo.EndTown, string.Concat(ticketInfo.DepartureTime)));
		}
	}
}
