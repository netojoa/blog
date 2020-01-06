using Custom.Foundation.Xdb.Events;
using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Client.WebApi;
using Sitecore.XConnect.Collection.Model;
using Sitecore.XConnect.Schema;
using Sitecore.Xdb.Common.Web;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace XdbExternalClient
{
    internal class Program
    {
        private static FakeContact _fakeContact;

        static Program()
        {
            _fakeContact = FakeContact.Default();
        }

        private static void Main(string[] args)
        {
            try
            {
                MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
                Console.WriteLine("Success");
                Console.Read();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error. \n Message {0}.\n Inner exception {1}\n. Details {2}.", ex.Message, ex.InnerException, ex.StackTrace);
                Console.Read();
            }
        }

        private static async Task MainAsync(string[] args)
        {
            using (var client = await GetXConnectClient())
            {
                var contact = await CreateOrUpdateContact(client);
                client.DeleteContact(contact);
                //await GetInteractionFacets(client);
                //CreateOrUpdateFacets(client, contact);
                //SetInteractionFacets(client, contact);
                await client.SubmitAsync();
            }
        }

        private static async Task GetInteractionFacets(XConnectClient client)
        {
            var references = new List<Sitecore.XConnect.IEntityReference<Sitecore.XConnect.Contact>>();
            references.Add(new IdentifiedContactReference(_fakeContact.Source, _fakeContact.EmailAddress));

            var contact = await client.GetAsync<Sitecore.XConnect.Contact>(references, new Sitecore.XConnect.ContactExpandOptions(PersonalInformation.DefaultFacetKey)
            {
            });

            Console.Read();
        }

        private static async Task<XConnectClient> GetXConnectClient()
        {
            // Valid certificate thumbprints must be passed in
            var options =
                CertificateHttpClientHandlerModifierOptions.Parse($"StoreName=My;StoreLocation=LocalMachine;FindType=FindByThumbprint;FindValue=55AAF36E6508E0670F2B11F67371993C9EB8428D");

            // Optional timeout modifier
            var certificateModifier = new CertificateHttpClientHandlerModifier(options);

            var clientModifiers = new List<IHttpClientModifier>();
            var timeoutClientModifier = new TimeoutHttpClientModifier(new TimeSpan(0, 0, 60));
            clientModifiers.Add(timeoutClientModifier);

            // This overload takes three client end points - collection, search, and configuration
            var collectionClient = new CollectionWebApiClient(new Uri("https://xp930.xconnect/odata"), clientModifiers, new[] { certificateModifier });
            var searchClient = new SearchWebApiClient(new Uri("https://xp930.xconnect/odata"), clientModifiers, new[] { certificateModifier });
            var configurationClient = new ConfigurationWebApiClient(new Uri("https://xp930.xconnect/configuration"), clientModifiers, new[] { certificateModifier });

            XdbModel[] models = { CollectionModel.Model, Custom.Foundation.Xdb.Models.FormsModel.Model };

            var cfg = new XConnectClientConfiguration(
                new XdbRuntimeModel(models), collectionClient, searchClient, configurationClient);

            await cfg.InitializeAsync();

            return new XConnectClient(cfg);
        }

        private async static Task<Contact> CreateOrUpdateContact(XConnectClient client)
        {
            var identifiedContactReference = new IdentifiedContactReference(_fakeContact.Source, _fakeContact.EmailAddress);

            var options = new ContactExpandOptions(PersonalInformation.DefaultFacetKey, EmailAddressList.DefaultFacetKey, AddressList.DefaultFacetKey, PhoneNumberList.DefaultFacetKey, Avatar.DefaultFacetKey);

            Task<Contact> contactTask = client.GetAsync<Contact>(identifiedContactReference, options);
            Contact contact = await contactTask;

            if (contact == null)
            {
                contact = new Sitecore.XConnect.Contact(new Sitecore.XConnect.ContactIdentifier(_fakeContact.Source, _fakeContact.EmailAddress, Sitecore.XConnect.ContactIdentifierType.Known));
                client.AddContact(contact);
            }

            return contact;
        }

        private static void CreateOrUpdateFacets(XConnectClient client, Contact contact)
        {
            UpdatePersonalInformationFacet(client, contact);
            UpdateEmailFacet(client, contact);
            UpdateAddressFacet(client, contact);
            UpdatePhoneNumberFacet(client, contact);

            // BUG: http://blog.peplau.com.br/en_US/xconnect-avatar-facet-breaking-experience-profile/
            //UpdateAvatarFacet(client, contact);
        }

        private static void UpdatePersonalInformationFacet(XConnectClient client, Contact contact)
        {
            var facet = contact.GetFacet<PersonalInformation>(PersonalInformation.DefaultFacetKey);

            if (facet != null)
            {
                facet.FirstName = _fakeContact.FirstName;
                facet.LastName = _fakeContact.LastName;
                facet.MiddleName = _fakeContact.MiddleName;
                facet.Birthdate = _fakeContact.Birthdate;
                facet.Gender = _fakeContact.Gender;
                facet.JobTitle = _fakeContact.JobTitle;
                client.SetFacet(contact, PersonalInformation.DefaultFacetKey, facet);
            }
            else
            {
                PersonalInformation personalInformation = new PersonalInformation
                {
                    FirstName = _fakeContact.FirstName,
                    LastName = _fakeContact.LastName,
                    MiddleName = _fakeContact.MiddleName,
                    Birthdate = _fakeContact.Birthdate,
                    Gender = _fakeContact.Gender,
                    JobTitle = _fakeContact.JobTitle
                };
                client.SetFacet<PersonalInformation>(contact, PersonalInformation.DefaultFacetKey, personalInformation);
            }
        }

        private static void UpdateEmailFacet(XConnectClient client, Contact contact)
        {
            var facet = contact.GetFacet<EmailAddressList>(EmailAddressList.DefaultFacetKey);

            if (facet != null)
            {
                facet.PreferredEmail = new EmailAddress(_fakeContact.EmailAddress, true);
                facet.PreferredKey = _fakeContact.PreferredEmailAddressKey;
                client.SetFacet(contact, EmailAddressList.DefaultFacetKey, facet);
            }
            else
            {
                EmailAddressList emails = new EmailAddressList(new EmailAddress(_fakeContact.EmailAddress, true), _fakeContact.PreferredEmailAddressKey);
                client.SetFacet<EmailAddressList>(contact, EmailAddressList.DefaultFacetKey, emails);
            }
        }

        private static void UpdateAddressFacet(XConnectClient client, Contact contact)
        {
            var facet = contact.GetFacet<AddressList>(AddressList.DefaultFacetKey);

            if (facet?.PreferredAddress != null)
            {
                facet.PreferredAddress.AddressLine1 = _fakeContact.AddressLine1;
                facet.PreferredAddress.City = _fakeContact.City;
                facet.PreferredAddress.PostalCode = _fakeContact.PostalCode;
                facet.PreferredAddress.CountryCode = _fakeContact.CountryCode;
                facet.PreferredAddress.StateOrProvince = _fakeContact.StateOrProvince;
                client.SetFacet(contact, AddressList.DefaultFacetKey, facet);
            }
            else
            {
                AddressList addresses = new AddressList(new Address()
                {
                    AddressLine1 = _fakeContact.AddressLine1,
                    City = _fakeContact.City,
                    PostalCode = _fakeContact.PostalCode,
                    CountryCode = _fakeContact.CountryCode,
                    StateOrProvince = _fakeContact.StateOrProvince
                }, "Home");
                client.SetFacet(contact, AddressList.DefaultFacetKey, addresses);
            }
        }

        private static void UpdatePhoneNumberFacet(XConnectClient client, Contact contact)
        {
            var facet = contact.GetFacet<PhoneNumberList>(PhoneNumberList.DefaultFacetKey);

            if (facet?.PreferredPhoneNumber != null)
            {
                facet.PreferredPhoneNumber.CountryCode = _fakeContact.PhoneCountryCode;
                facet.PreferredPhoneNumber.Number = _fakeContact.PhoneNumber;
                facet.PreferredPhoneNumber.AreaCode = _fakeContact.PhoneAreaCode;
                client.SetFacet(contact, PhoneNumberList.DefaultFacetKey, facet);
            }
            else
            {
                PhoneNumberList phoneNumbers = new PhoneNumberList(new PhoneNumber(_fakeContact.PhoneCountryCode, _fakeContact.PhoneNumber), "Home");
                client.SetFacet(contact, PhoneNumberList.DefaultFacetKey, phoneNumbers);
            }
        }

        private static void UpdateAvatarFacet(XConnectClient client, Contact contact)
        {
            var facet = contact.GetFacet<Avatar>(Avatar.DefaultFacetKey);

            Image image = Image.FromFile(_fakeContact.AvatarImage);

            if (image == null)
                return;

            if (facet != null)
            {
                facet.MimeType = GetImageMimeType(image);
                facet.Picture = GetAvatarImage(image);
                client.SetFacet(contact, Avatar.DefaultFacetKey, facet);
            }
            else
            {
                Avatar addresses = new Avatar(GetImageMimeType(image), GetAvatarImage(image));
                client.SetFacet(contact, Avatar.DefaultFacetKey, addresses);
            }
        }

        private static byte[] GetAvatarImage(Image image)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                return memoryStream.ToArray();
            }
        }

        private static string GetImageMimeType(Image image)
        {
            ImageFormat format = image.RawFormat;
            ImageCodecInfo codecInfo = ImageCodecInfo.GetImageDecoders().First(c => c.FormatID == format.Guid);
            return codecInfo.MimeType;
        }

        private static void SetInteractionFacets(XConnectClient client, Contact contact)
        {
            if (contact != null)
            {
                Guid channelId = Guid.Parse("a5d2d58e-8716-4aeb-bab5-6905aa148f9d");
                var userAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 10_3 like Mac OS X) AppleWebKit/602.1.50 (KHTML, like Gecko) CriOS/56.0.2924.75 Mobile/14E5239e Safari/602.1";
                Interaction interaction = new Interaction(contact, InteractionInitiator.Brand, channelId, userAgent);

                //TriggerGoal(interaction);
                //RegisterSearch(interaction);
                //VisitPage(interaction);
                RegisterFormSubmissionFacet(client, interaction);

                // BUG: URI issues
                //SetWebVisit(client, interaction, VisitPage);

                client.AddInteraction(interaction);
            }
        }

        private static void SetWebVisit(XConnectClient client, Interaction interaction, Action<Interaction> visitPageAction)
        {
            visitPageAction(interaction);

            var facet = new WebVisit();
            // Populate data about the web visit
            facet.Browser = new BrowserData() { BrowserMajorName = "Chrome", BrowserMinorName = "Desktop", BrowserVersion = "22.0" };
            facet.Language = "en";
            facet.OperatingSystem = new OperatingSystemData() { Name = "Windows", MajorVersion = "10", MinorVersion = "4" };
            facet.Referrer = "www.google.com";
            facet.Screen = new ScreenData() { ScreenHeight = 1080, ScreenWidth = 685 };
            facet.SearchKeywords = "banana smoothie chocolate";
            facet.SiteName = "website";
            client.SetWebVisit(interaction, facet);
        }

        private static void RegisterSearch(Interaction interaction)
        {
            // Search event
            SearchEvent searchEvent = new SearchEvent(DateTime.Now)
            {
                Keywords = "banana smoothie recipes",
            };
            interaction.Events.Add(searchEvent);
        }

        private static void VisitPage(Interaction interaction)
        {
            /// Visit page
            var homeItemId = Guid.Parse("110d559f-dea5-42ea-9c1c-8a5df7e70ef9");
            PageViewEvent pageView = new PageViewEvent(DateTime.UtcNow, homeItemId, 1, "en")
            {
                Duration = new TimeSpan(0, 0, 30),
                Url = "/"
            };
            interaction.Events.Add(pageView);
        }

        private static void TriggerGoal(Interaction interaction)
        {
            // Trigger Goal
            var goal = new Goal(Guid.Parse("{968897F1-328A-489D-88E8-BE78F4370958}"), DateTime.UtcNow);
            goal.EngagementValue = 20;
            interaction.Events.Add(goal);
        }

        private static void RegisterFormSubmissionFacet(XConnectClient client, Interaction interaction)
        {
            FormSubmission formSubmission = new FormSubmission(DateTime.UtcNow)
            {
                FormId = Guid.NewGuid()
            };

            formSubmission.CustomValues.Add(Guid.NewGuid().ToString(), "Books");
            formSubmission.CustomValues.Add(Guid.NewGuid().ToString(), "1");
            formSubmission.CustomValues.Add(Guid.NewGuid().ToString(), "5");

            interaction.Events.Add(formSubmission);

        }
    }
}