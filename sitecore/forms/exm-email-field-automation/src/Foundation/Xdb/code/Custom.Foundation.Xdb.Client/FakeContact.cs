using System;

namespace XdbExternalClient
{
    public class FakeContact
    {
        public string EmailAddress { get; set; }
        public string PreferredEmailAddressKey { get; set; }
        public string Source { get; set; }
        public string JobTitle { get; set; }
        public string Gender { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public DateTime? Birthdate { get; set; }
        public string AddressLine1 { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }
        public string StateOrProvince { get; set; }
        public string PhoneCountryCode { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneAreaCode { get; set; }
        public string AvatarImage { get; set; }

        public static FakeContact Default()
        {
            return new FakeContact
            {
                Source = "ExperienceForms",
                EmailAddress = "john.doe@sitecore.com",
                PreferredEmailAddressKey = "Work",
                FirstName = "John",
                LastName = "Doe",
                MiddleName = "Stuart",
                Gender = "Male",
                Birthdate = new DateTime(1987, 12, 26),
                JobTitle = "Senior Legacy Systems Supervisor",
                AddressLine1 = "Cool Place, 12",
                City = "Amsterdam",
                PostalCode = "1215 HH",
                CountryCode = "NL",
                StateOrProvince = "Nord Holland",
                PhoneAreaCode = "6",
                PhoneCountryCode = "31",
                PhoneNumber = "98 98 98 98",
                AvatarImage = "C:\\images\\batman.png",
            };
        }
    }
}