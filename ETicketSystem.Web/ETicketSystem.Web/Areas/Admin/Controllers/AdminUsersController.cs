namespace ETicketSystem.Web.Areas.Admin.Controllers
{
	using ETicketSystem.Services.Contracts;

	public class AdminUsersController : BaseAdminController
    {
		public IUserService users;

		public AdminUsersController(IUserService users)
		{
			this.users = users;
		}
    }
}
