namespace ETicketSystem.Web.Controllers
{
	using Common.Constants;
	using Common.Enums;
	using Data.Models;
	using Infrastructure.Extensions;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.Rendering;
	using Models.Pagination;
	using Models.Users;
	using Services.Contracts;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	[Authorize]
	public class UsersController : BaseController
    {
		private readonly IUserService users;

		private readonly ITicketService tickets;

		private readonly ICompanyService companies;

		private readonly UserManager<User> userManager;

		public UsersController(IUserService users, ITownService towns, UserManager<User> userManager, ITicketService tickets, ICompanyService companies)
			:base(towns)
		{
			this.users = users;
			this.userManager = userManager;
			this.tickets = tickets;
			this.companies = companies;
		}

		public async Task<IActionResult> Profile(string id)
		{
			if (this.userManager.GetUserId(User) != id)
			{
				this.GenerateAlertMessage(WebConstants.Message.NotProfileOwner, Alert.Danger);

				return RedirectToHome();
			}

			var user = await this.users.GetRegularUserProfileDetailsAsync(id);

			if (user == null)
			{
				this.GenerateAlertMessage(string.Format(WebConstants.Message.NonExistingEntity,WebConstants.Entity.User, id), Alert.Danger);
				return RedirectToHome();
			}

			return View(user);
		}

		[Route(WebConstants.Routing.EditUser)]
		public IActionResult EditUser(string id)
		{
			if (!this.users.UserExists(id))
			{
				this.GenerateAlertMessage(string.Format(WebConstants.Message.NonExistingEntity,WebConstants.Entity.User, id), Alert.Danger);

				return RedirectToHome();
			}

			if (this.userManager.GetUserId(User) != id)
			{
				this.GenerateAlertMessage(string.Format(WebConstants.Message.NotProfileOwner, id), Alert.Danger);

				return RedirectToHome();
			}

			var user = this.users.GetRegularUserProfileToEdit(id);

			return View(new EditUserProfileFormModel()
			{
				Username = user.Username,
				Email = user.Email
			});
		}

		[HttpPost]
		[Route(WebConstants.Routing.EditUser)]
		public async Task<IActionResult> EditUser(string id, EditUserProfileFormModel profile)
		{
			if (!ModelState.IsValid)
			{
				return View(profile);
			}

			if (!this.users.UserExists(id) || this.userManager.GetUserId(User) != id)
			{
				return BadRequest();
			}

			var errors = await this.users.EditRegularUserAsync(id, profile.Username, profile.Email, profile.NewPassword, profile.Password);

			if (errors.Count() != 0)
			{
				foreach (var error in errors)
				{
					ModelState.AddModelError("", error.Description);
				}

				return View(profile);
			}

			this.GenerateAlertMessage(WebConstants.Message.ProfileEdited, Alert.Success);

			return RedirectToAction(nameof(Profile), new { id = id });
		}

		[Route(WebConstants.Routing.UserTickets)]
		public IActionResult MyTickets(string id, int startTown, int endTown, string companyId, DateTime? date, int page = 1)
		{
			if (this.userManager.GetUserId(User) != id)
			{
				this.GenerateAlertMessage(WebConstants.Message.NotProfileOwner, Alert.Danger);

				return RedirectToHome();
			}

			if (page < 1)
			{
				return RedirectToAction(nameof(MyTickets), new { id = id, startTown = startTown, endTown = endTown, companyId = companyId, date = (date.HasValue ? date.Value.ToYearMonthDayFormat() : null), page = 1 });
			}

			var tickets = this.tickets.GetUserTickets(id, startTown, endTown, companyId, date, page, WebConstants.Pagination.UserTicketsPageSize);

			var pagination = new PaginationViewModel()
			{
				Action = nameof(MyTickets),
				Controller = WebConstants.Controller.Users,
				CurrentPage = page,
				PageSize = WebConstants.Pagination.UserTicketsPageSize,
				TotalElements = this.tickets.UserTicketsCount(id, startTown, endTown, companyId, date)
			};

			if (page > pagination.TotalPages && pagination.TotalPages != 0)
			{
				return RedirectToAction(nameof(MyTickets), new { id = id, startTown = startTown, endTown = endTown, companyId = companyId, date = (date.HasValue ? date.Value.ToYearMonthDayFormat() : null), page = pagination.TotalPages });
			}

			var selectListTowns = this.GenerateSelectListTowns();
			selectListTowns.First().Disabled = false;

			return View(new UserTickets()
			{
				Tickets = tickets,
				Pagination = pagination,
				Towns = selectListTowns,
				Companies = this.GenerateSelectListCompanies(),
				CompanyId = companyId,
				Date = date,
				StartTown = startTown,
				EndTown = endTown
			});
		}

		private List<SelectListItem> GenerateSelectListCompanies()
		{
			var list = new List<SelectListItem>();
			var companies = this.companies.GetCompaniesSelectListItems();

			list.Add(new SelectListItem()
			{
				Text = WebConstants.SelectListDefaultItem.All,
				Value = string.Empty,
				Selected = true
			});

			foreach (var c in companies)
			{
				list.Add(new SelectListItem()
				{
					Text = c.Name,
					Value = c.Id.ToString()
				});
			}

			return list;
		}
	}
}
