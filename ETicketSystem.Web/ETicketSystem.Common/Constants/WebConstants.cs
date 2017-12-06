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
		}

		public class RegexPattern
		{
			public const string Email = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";

			public const string CompanyUniqueReferenceNumber = @"^\d{9,13}$";

			public const string Phone = @"^0\d{9}$";
		}

		public class Error
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
		}

		public class Area
		{
			public const string Admin = "Admin";

			public const string Company = "Company";
		}

		public class Route
		{
			public const string Admin = "Admin";

			public const string AllCompanies = "Companies/All";

			public const string ApproveCompany = "Companies/Approve/{companyId}";
		}

		public class Action
		{
			public const string VerifyCompanyName = "VerifyCompanyName";

			public const string VerifyUrn = "VerifyUrn";

			public const string VerifyPhoneNumber = "VerifyPhoneNumber";
		}

		public class Controller
		{
			public const string Account = "Account";
		}

		public class FilePath
		{
			public const string Towns = "../ETicketSystem.Data/SeedData/towns.csv";

			public const string Stations = "../ETicketSystem.Data/SeedData/stations.csv";
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
    }
}
