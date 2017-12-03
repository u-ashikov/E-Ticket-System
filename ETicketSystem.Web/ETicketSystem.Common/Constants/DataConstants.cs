﻿namespace ETicketSystem.Common.Constants
{
	public static class DataConstants
    {
		public class Town
		{
			public const int TownMaxNameLength = 50;
		}

		public class Station
		{
			public const int NameMaxLength = 50;
		}

		public class Company
		{
			public const int NameMinLength = 3;

			public const int NameMaxLength = 100;

			public const int LogoMaxLength =  500 * 1024;

			public const int DescriptionMinLength = 200;

			public const int DescriptionMaxLength = 3000;

			public const int UniqueReferenceNumberMinLength = 9;

			public const int UniqueReferenceNumberMaxLength = 13;

			public const int ChiefNameMaxLength = 20;

			public const int EmailMinLength = 5;

			public const int EmailMaxLength = 254;

			public const int PhoneLength = 10;

			public const int AddressMinLength = 10;

			public const int AddressMaxLength = 95;
		}

		public class Route
		{
			public const string TimeMinValue = "00:00";

			public const string TimeMaxValue = "23:59";

			public const int PriceMinValue = 0;
		}

		public class Review
		{
			public const int DescriptionMinLength = 20;

			public const int DescriptionMaxLength = 150;
		}

		public class Ticket
		{
			public const int SeatMinValue = 1;

			public const int SeatMaxValue = 45;
		}
		
		public class User
		{
			public const int UsernameMinLength = 4;

			public const int UsernameMaxLength = 20;

			public const int NameMaxLength = 35;
		}
    }
}
