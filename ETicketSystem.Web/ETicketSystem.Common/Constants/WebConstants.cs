namespace ETicketSystem.Common.Constants
{
	public class WebConstants
    {
		public class Admin
		{
			public const string Role = "Administrator";

			public const string Username = "admin";

			public const string Email = "admin@eticket.com";

			public const string Password = "admin12";

			public const string FirstName = "Gergan";

			public const string LastName = "Gerganov";
		}

		public class FieldDisplay
		{
			public const string ConfirmPassword = "Confirm Password";

			public const string FirstName = "First name";

			public const string LastName = "Last name";

			public const string RememberMe = "Remember me?";

			public const string UniqueReferenceNumber = "Unique Reference Number";

			public const string ChiefFirstName = "Chief first name";

			public const string ChiefLastName = "Chief last name";
		}

		public class RegexPattern
		{
			public const string Email = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
		}

		public class Error
		{
			public const string InvalidEmail = "Invalid email address!";

			public const string PasswordLength = "The {0} must be at least {2} and at max {1} characters long.";

			public const string UsernameLength = "The {0} must be at least {2} and at max {1} characters long.";

			public const string PasswordsMissmatch = "The password and confirmation password do not match.";
		}

		public class Area
		{
			public const string Admin = "Admin";
		}

		public class FilePath
		{
			public const string Towns = "../ETicketSystem.Data/SeedData/towns.csv";
		}
    }
}
