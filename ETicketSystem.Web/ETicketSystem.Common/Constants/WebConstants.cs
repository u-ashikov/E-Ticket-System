namespace ETicketSystem.Common.Constants
{
	public class WebConstants
	{
		public class FieldDisplay
		{
			public const string ConfirmPassword = "Confirm password";

			public const string FirstName = "First name";

			public const string LastName = "Last name";

			public const string RememberMe = "Remember me?";

			public const string UniqueReferenceNumber = "Unique reference number";

			public const string ChiefFirstName = "Chief first name";

			public const string ChiefLastName = "Chief last name";

			public const string PhoneNumber = "Phone";

			public const string RouteStartStation = "Start station";

			public const string RouteEndStation = "End station";

			public const string RouteDepartureTime = "Departure time";

			public const string RouteBusType = "Bus type";

			public const string StartDestination = "Start destination";

			public const string EndDestination = "End destination";

			public const string Company = "Company";

			public const string NewPassword = "New password";

			public const string Town = "Town";
		}

		public class SelectListDefaultItem
		{
			public const string All = " -- All -- ";

			public const string SelectTown = " -- Select town -- ";

			public const string DefaultItemValue = "0";
		}

		public class RegexPattern
		{
			public const string Email = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";

			public const string CompanyUniqueReferenceNumber = @"^\d{9,13}$";

			public const string Phone = @"^0\d{9}$";

			public const string FriendlyUrl = @"[^A-Za-z0-9_\.~]+";

			public const string Name = @"[a-zA-Z ]+";
		}

		public class Message
		{
			public const string NameContainOnlyLetters = "{0} can contain only letetrs!";

			public const string InvalidEmail = "Invalid email address!";

			public const string PasswordLength = "The {0} must be at least {2} and at max {1} characters long.";

			public const string PasswordsMissmatch = "The password and confirmation password do not match.";

			public const string UsernameLength = "The {0} must be at least {2} and at max {1} characters long.";

			public const string CompanyNameLength = "The {0} must be at least {2} and at max {1} characters long.";

			public const string UniqueReferenceNumberFormat = "The {0} must be between 9 and 13 symbols long, containing only digits.";

			public const string CompanyChiefNameMaxLength = "The {0} must be at max {1} symbols long.";

			public const string CompanyAddressLength = "The {0} must be between {2} and {1} symbols long.";

			public const string CompanyDescriptionLength = "The {0} must be between {2} and {1} symbols long.";

			public const string LogoMaxLength = "The company logo cannot be larger than 500 KB.";

			public const string LogoAvailableFormats = "The company logo can be in the following formats: .jpg, .png or .bmp.";

			public const string PhoneNumberFormat = "The {0} should start with '0' containing exactly 10 digits.";

			public const string RegularUserNameMaxLength = "The {0} must be at max {1} symbols long.";

			public const string CompanyNameAlreadyTaken = "Company with that name is already registered.";

			public const string CompanyUrnAlreadyTaken = "Company with that URN already exists.";

			public const string CompanyPhoneAlreadyTaken = "That phone number is already in use.";

			public const string NonExistingEntity = "{0} with id: {1} does not exist!";
			public const string EntityAlreadyExist = "{0} already exists!";
			public const string EntityCreated = "{0} created successfully!";
			public const string EntityEdited = "{0} edited successfully!";
			public const string EntityDeleted = "{0} deleted successfully!";

			public const string CompanyAlreadyApproved = "Company {0} is already approved!";
			public const string CompanyApproved = "Company {0} approved successfully!";

			public const string CompanyNotApproved = "You cannot add routes yet because you are still not approved by the site administrator!";

			public const string AddRouteCompanyBlocked = "You cannot add routes because you have been blocked by the site administrator!";

			public const string ChangeRouteCompanyBlocked = "You cannot manupulate routes because you have been blocked by the site administrator!";

			public const string ChangeRouteCompanyNotApproved = "You cannot manupulate routes because you are still not approved by the site administrator!";

			public const string InvalidRoute = "Invalid route!";
			public const string InvalidTown = "Invalid town!";
			public const string InvalidDate = "Invalid date!";
			public const string InvalidStation = "Invalid station!";
			public const string InvalidCompany = "Invalid company!";

			public const string StartStationEqualToEndStation = "Start station and end station cannot be same or in same town.";
			public const string RouteDurationZeroLength = "Route duration must be greater than 00:00.";
			public const string CompanyRouteDuplication = "This route already exists!";
			public const string RouteAdded = "Route from {0} to {1} added successfully!";
			public const string RouteEdited = "Successfully edited route from {0} to {1}!";
			public const string RouteStatusChanged = "Route from {0} to {1} departing at {2} {3}!";
			public const string EditRouteWithTickets = "Cannot edit route from {0} to {1} because there are already reserved tickets!";
			public const string DeactivateRouteWithTickets = "Cannot deactivate route from {0} to {1} because there are already reserved tickets!";
			public const string PositivePrice = "Price must be positive number!";

			public const string InvalidTicket = "Invalid ticket!";
			public const string NoneSelectedSeats = "You have to choose at least one seat.";
			public const string SeatsAlreadyTaken = "Some of the seats you choose are already taken: {0}.";
			public const string SuccessfullyTicketReservation = "You have successfully reservated seats: {0}, for route {1} - {2}, departing at {3}.";
			public const string TicketCancelationDenied = "You can cancel tickets only 30 and more minutes before bus departing!";
			public const string TicketCancelationSuccess = "Successfully cancelled ticket!";
			public const string RouteSoldOut = "Route from {0} to {1} departing on {2} at {3} sold out!";

			public const string NotProfileOwner = "You are not owner of that profile!";

			public const string EmptyUsername = "Username cannot be empty!";
			public const string IncorrectOldPassword = "Old password is incorrect.";
			public const string BothPasswordFieldsRequired = "Both password fields are required!";
			public const string ProfileEdited = "Profile edited successfully!";

			public const string BlockCompanyUnavailable = "Company must be approved before been blocked!";

			public const string CompanyStatusChanged = "Company {0} status has been changed to {1}!";

			public const string StationAlreadyExists = "Station {0} in town {1} already exist!";
			public const string StationCreated = "Station {0} in town {1} created!";
			public const string StationNameMaxLength = "{0} cannot be more than {1} symbols long.";
			public const string StationPhoneMaxLength = "{0} cannot be more than {1} symbols long.";

			public const string TownNameMaxLength = "{0} cannot be more than {1} symbols long.";
			public const string TownAdded = "Town {0} created successfully!";

			public const string UserAddedToRole = "User {0} added to role {1} successfully!";
			public const string UserRemovedFromRole = "User {0} removed from role {1} successfully!";
			public const string UserAlreadyInRole = "User {0} already in role {1}!";
			public const string UserNotInRole = "User {0} not in role {1}!";

			public const string NoChangesFound = "No changes found to edit!";

			public const string UnableToAddReview = "You are not able to add reviews for this company because you haven't used its services yet!";

			public const string OwnerAddReview = "You are the owner and you cannot add reviews to yourself!";

			public const string CompanyReviewDescriptionLength = "The field {0} must be between {2} and {1} symbols long!";

			public const string CompanyPhoneCannotBeEmpty = "Company phone cannot be empty!";

			public const string CompanyDescriptionCannotBeEmpty = "Company description cannot be empty!";
		}

		public class ContentType
		{
			public const string Pdf = "application/pdf";
		}

		public class Area
		{
			public const string Admin = "Admin";

			public const string Company = "Company";
		}

		public class Routing
		{
			public const string Admin = "Admin";
			public const string AdminUsersAddToRole = "Admin/Users/AddToRole/{userId}";
			public const string AdminUsersRemoveFromRole = "Admin/Users/RemoveFromRole/{userId}";

			public const string AdminAllTowns = "Towns/All";
			public const string AdminAddTown = "Towns/Add";
			public const string AdminTownStations = "Towns/Stations/{id}";

			public const string AdminAddStation = "Stations/Add";
			public const string AdminAllStations = "Stations/All";
			public const string AdminEditStation = "Stations/Edit/{id}";

			public const string AdminAllCompanies = "Companies/All";
			public const string AdminApproveCompany = "Companies/Approve/{companyId}";
			public const string AdminBlockCompany = "Companies/Block/{companyId}";

			public const string HomeError = "/Home/Error";

			public const string Company = "Company";
			public const string CompanyProfile = "Profile/{id}";
			public const string AddCompanyRoute = "Routes/Add";
			public const string AllCompanyRoutes = "Routes/All";
			public const string EditCompanyRoute = "Routes/Edit/{id}";
			public const string EditCompanyProfile = "Profile/Edit/{id}";
			public const string ChangeCompanyRouteStatus = "Routes/ChangeStatus/{id}";

			public const string RoutesSearch = "Routes/Search";
			public const string BookRouteTicket = "Routes/BookTicket/{id}/{departureTime}/{date}/{companyId?}";

			public const string EditUser = "Users/Profile/Edit/{id}";
			public const string UserTickets = "Users/MyTickets/{id}";

			public const string AllUsers = "Users/All";

			public const string AdminAllTownsUrl = "/admin/towns/all";
		}

		public class Entity
		{
			public const string User = "User";

			public const string Company = "Company";

			public const string Town = "Town";

			public const string Station = "Station";

			public const string Ticket = "Ticket";

			public const string Route = "Route";

			public const string Review = "Review";
		}

		public class Controller
		{
			public const string Account = "Account";

			public const string Home = "Home";

			public const string Routes = "Routes";

			public const string Companies = "Companies";

			public const string Users = "Users";

			public const string AdminCompanies = "AdminCompanies";
			public const string AdminTowns = "AdminTowns";
			public const string AdminStations = "AdminStations";
			public const string AdminUsers = "AdminUsers";
		}

		public class Action
		{
			public const string VerifyCompanyName = "VerifyCompanyName";

			public const string VerifyUrn = "VerifyUrn";

			public const string VerifyPhoneNumber = "VerifyPhoneNumber";

			public const string VerifyTownName = "VerifyTownName";

			public const string AdminAllTowns = "All";

			public const string AdminAllCompanies = "All";

			public const string AdminAllStations = "All";

			public const string AdminAllUsers = "All";

            public const string CompanyAllRoutes = "All";

            public const string MyTickets = "MyTickets";

			public const string Details = "Details";

			public const string Index = "Index";

			public const string Search = "Search";

			public const string Edit = "Edit";

			public const string Delete = "Delete";
		}

		public class FilePath
		{
			public const string Towns = "../ETicketSystem.Data/SeedData/towns.csv";

			public const string Stations = "../ETicketSystem.Data/SeedData/stations.csv";

			public const string Companies = "../ETicketSystem.Data/SeedData/companies.csv";

			public const string Users = "../ETicketSystem.Data/SeedData/users.csv";

			public const string CompaniesImages = "../ETicketSystem.Data/SeedData/Images/Companies/";
		}

		public class PictureFormat
		{
			public const string Jpg = ".jpg";

			public const string Png = ".png";

			public const string Bmp = ".bmp";
		}

		public class DbConnection
		{
			public const string DefaultConnection = "DefaultConnection";
		}

		public class TempDataKey
		{
			public const string Message = "Message";

			public const string AlertType = "AlertType";
		}

		public class Role
		{
			public const string CompanyRole = "Company";

			public const string ModeratorRole = "Moderator";
		}

		public class Company
		{
			public const int ShortDescriptionLength = 100;

			public const int TopRoutesCount = 10;
		}

		public class Pagination
		{
			public const int CompaniesPageSize = 12;

			public const int SearchedRoutesPageSize = 5;

			public const int UserTicketsPageSize = 20;

			public const int AdminCompaniesListing = 10;

			public const int AdminTownsListing = 20;

			public const int AdminStationsListing = 20;

			public const int AdminUsersListing = 20;

			public const int CompanyRoutesListing = 20;

			public const int CompanyReviewsListing = 7;
		}

		public class Ticket
		{
			public const int CancelationMinutesDifference = 30;
		}

		public class Pdf
		{
			public const string Ticket = "<h3>Company: {0}</h3><h3>Route: {1}</h3><h3>Seat: {2}</h3><h3>Departure time: {3}</h3>";

			public const string TicketName = "E-Ticket_{0}_{1}_{2}.pdf";
		}
	}
}