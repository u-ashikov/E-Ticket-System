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

		public class RegexPattern
		{
			public const string Email = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";

			public const string CompanyUniqueReferenceNumber = @"^\d{9,13}$";

			public const string Phone = @"^0\d{9}$";

			public const string FriendlyUrl = @"[^A-Za-z0-9_\.~]+";
		}

		public class Message
		{
			public const string InvalidEmail = "Invalid email address!";

			public const string PasswordLength = "The {0} must be at least {2} and at max {1} characters long.";

			public const string UsernameLength = "The {0} must be at least {2} and at max {1} characters long.";

			public const string CompanyNameLength = "The {0} must be at least {2} and at max {1} characters long.";

			public const string UniqueReferenceNumberFormat = "The {0} must be between 9 and 13 symbols long, containing only digits.";

			public const string CompanyChiefNameMaxLength = "The {0} must be at max {1} symbols long.";

			public const string PasswordsMissmatch = "The password and confirmation password do not match.";

			public const string CompanyAddressLength = "The {0} must be between {2} and {1} symbols long.";

			public const string CompanyDescriptionLength = "The {0} must be between {2} and {1} symbols long.";

			public const string LogoMaxLength = "The company logo cannot be larger than 500 KB.";

			public const string LogoAvailableFormats = "The company logo can be in the following formats: .jpg, .png or .bmp.";

			public const string PhoneNumberFormat = "The {0} should start with '0' containing exactly 10 digits.";

			public const string RegularUserNameMaxLength = "The {0} must be at max {1} symbols long.";

			public const string CompanyNameAlreadyTaken = "Company with that name is already registered.";

			public const string CompanyUrnAlreadyTaken = "Company with that URN already exists.";

			public const string CompanyPhoneAlreadyTaken = "That phone number is already in use.";

			public const string NonExistingCompany = "Company with id: {0} does not exist!";

			public const string CompanyAlreadyApproved = "Company {0} is already approved!";

			public const string CompanyApproved = "Company {0} approved successfully!";

			public const string StartStationEqualToEndStation = "Start station and end station cannot be same.";

			public const string RouteDurationZeroLength = "Route duration must be greater than 00:00.";

			public const string CompanyRouteDuplication = "This route already exists!";

			public const string RouteAdded = "Route from {0} to {1} added successfully!";

			public const string NotApproved = "You cannot add routes yet because you are still not approved by the site administrator!";

			public const string Blocked = "You cannot add routes because you have been blocked by the site administrator!";

			public const string InvalidRoute = "Invalid route!";

			public const string InvalidTown = "Invalid town!";

			public const string InvalidDate = "Invalid date!";

			public const string SuccessfullyEditedRoute = "Successfully edited route from {0} to {1}!";

			public const string RouteStatusChanged = "Route from {0} to {1} departing at {2} {3}!";

			public const string NoneSelectedSeats = "You have to choose at least one seat.";

			public const string SuccessfullyTicketReservation = "You have successfully reservated seats: {0}, for route {1} - {2}, departing at {3}.";

			public const string NotProfileOwner = "You are not owner of that profile!";

			public const string NonExistingUser = "User with id: {0} does not exist!";

			public const string NonExistingTown = "Town with id: {0} does not exist!";

			public const string NonExistingStation = "Station with id: {0} does not exist!";

			public const string EmptyUsername = "Username cannot be empty!";

			public const string IncorrectOldPassword = "Old password is incorrect.";

			public const string BothPasswordFieldsRequired = "Both password fields are required!";

			public const string ProfileEdited = "Profile edited successfully!";

			public const string InvalidTicket = "Invalid ticket!";

			public const string BlockCompanyUnavailable = "Company must be approved before been blocked!";

			public const string CompanyStatusChanged = "Company {0} status has been changed to {1}!";

			public const string StationAlreadyExists = "Station {0} in town {1} already exist!";

			public const string StationCreated = "Station {0} in town {1} created!";

			public const string StationNameMaxLength = "{0} cannot be more than {1} symbols long.";

			public const string StationPhoneMaxLength = "{0} cannot be more than {1} symbols long.";

			public const string StationEdited = "Station edited successfully!";

			public const string NoChangesFound = "No changes found to edit!";

			public const string TownNameMaxLength = "{0} cannot be more than {1} symbols long.";

			public const string TownAlreadyExist = "Town already exist!";

			public const string TownAdded = "Town {0} created successfully!";

			public const string UserAddedToRole = "User {0} added to role {1} successfully!";

			public const string UserRemovedFromRole = "User {0} removed from role {1} successfully!";

			public const string UserAlreadyInRole = "User {0} already in role {1}!";

			public const string UserNotInRole = "User {0} not in role {1}!";
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

		public class Route
		{
			public const string Admin = "Admin";

			public const string AdminUsersAddToRole = "Admin/Users/AddToRole/{userId}";

			public const string AdminUsersRemoveFromRole = "Admin/Users/RemoveFromRole/{userId}";

			public const string HomeError = "/Home/Error";

			public const string AllCompanies = "Companies/All";

			public const string AllTowns = "Towns/All";

			public const string AddTown = "Towns/Add";

			public const string TownStations = "Towns/Stations/{id}";

			public const string ApproveCompany = "Companies/Approve/{companyId}";

			public const string Company = "Company";

			public const string AddCompanyRoute = "Routes/Add";

			public const string AllCompanyRoutes = "Routes/All";

			public const string EditCompanyRoute = "Routes/Edit/{id}";

			public const string ChangeCompanyRouteStatus = "Routes/ChangeStatus/{id}";

			public const string RoutesSearch = "Routes/Search";

			public const string BookRouteTicket = "Routes/BookTicket/{id}/{departureTime}/{date}/{companyId?}";

			public const string EditUser = "Users/Profile/Edit/{id}";

			public const string UserTickets = "Users/MyTickets/{id}";

			public const string AddStation = "Stations/Add";

			public const string AllStations = "Stations/All";

			public const string AllUsers = "Users/All";

			public const string EditStation = "Stations/Edit/{id}";

			public const string AdminAllTownsUrl = "/admin/towns/all";
		}

		public class Action
		{
			public const string VerifyCompanyName = "VerifyCompanyName";

			public const string VerifyUrn = "VerifyUrn";

			public const string VerifyPhoneNumber = "VerifyPhoneNumber";

			public const string VerifyTownName = "VerifyTownName";

			public const string AdminAllTowns = "All";
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
		}

		public class SearchForm
		{
			public const string SearchDefaultStartTown = "Sofia";

			public const string SearchDefaultEndTown = "Varna";
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

			public const int UserTicketsPageSize = 10;

			public const int AdminCompaniesListing = 10;

			public const int AdminTownsListing = 20;

			public const int AdminStationsListing = 20;

			public const int AdminUsersListing = 20;
		}

		public class Pdf
		{
			public const string Ticket = "<h3>Company: {0}</h3><h3>Route: {1}</h3><h3>Seat: {2}</h3><h3>Departure time: {3}</h3>";

			public const string TicketName = "E-Ticket.pdf";
		}
	}
}