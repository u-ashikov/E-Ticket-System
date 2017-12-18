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

			if (!this.tickets.TicketExists(id) || !this.tickets.IsTicketOwner(id, userId))
			{
				this.GenerateAlertMessage(WebConstants.Message.NonExistingTicket, Alert.Danger);
				return Redirect($"/{WebConstants.Controller.Users}/{WebConstants.Action.MyTickets}/{userId}");
			}

			bool success = this.tickets.CancelTicket(id, userId);

			if (!success)
			{
				this.GenerateAlertMessage(WebConstants.Message.TicketCancelationDenied, Alert.Warning);
				return Redirect($"/{WebConstants.Controller.Users}/{WebConstants.Action.MyTickets}/{userId}");
			}

			this.GenerateAlertMessage(WebConstants.Message.TicketCancelationSuccess,Alert.Success);

			return Redirect($"/{WebConstants.Controller.Users}/{WebConstants.Action.MyTickets}/{userId}");
		}
    }
}
