using Sitecore.Analytics;
using Sitecore.Analytics.Model;
using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Client.Configuration;
using Sitecore.XConnect.Collection.Model;
using System;
using System.Linq;

namespace Custom.Foundation.Forms.Services
{
    public class XdbContactService : IXdbContactService
    {
        public XdbContactService()
        {
            StartTracking();
        }

        private void StartTracking()
        {
            if (Tracker.Current == null && Tracker.Enabled)
            {
                Tracker.StartTracking();
            }
        }

        public void RegisterInteractionEvent(Event interactionEvent)
        {
            var trackerIdentifier = GetContactReference();

            using (XConnectClient client = SitecoreXConnectClientConfiguration.GetClient())
            {
                var contact = client.Get<Contact>(trackerIdentifier, new ExpandOptions());

                if (contact != null)
                {

                    var interaction = new Sitecore.XConnect.Interaction(contact, InteractionInitiator.Brand, Guid.NewGuid(), "Experience Forms");

                    interaction.Events.Add(interactionEvent);

                    client.AddInteraction(interaction);

                    client.Submit();

                    ReloadContactDataIntoSession();
                }
            }
        }

        protected IdentifiedContactReference GetContactReference()
        {
            var id = GetContactId();
            var isAnonymous = Tracker.Current.Contact.IsNew || Tracker.Current.Contact.Identifiers.Count == 0;
            return isAnonymous
                ? new IdentifiedContactReference(Sitecore.Analytics.XConnect.DataAccess.Constants.IdentifierSource, Tracker.Current.Contact.ContactId.ToString("N"))
                : new IdentifiedContactReference(id.Source, id.Identifier);
        }

        protected Sitecore.Analytics.Model.Entities.ContactIdentifier GetContactId()
        {
            if (Tracker.Current?.Contact == null)
            {
                return null;
            }

            if (Tracker.Current.Contact.IsNew)
            {
                this.SaveContact();
            }

            return Tracker.Current.Contact.Identifiers.FirstOrDefault();
        }

        protected void SaveContact()
        {
            if (CreateContactManager() is Sitecore.Analytics.Tracking.ContactManager manager)
            {
                Tracker.Current.Contact.ContactSaveMode = ContactSaveMode.AlwaysSave;
                manager.SaveContactToCollectionDb(Tracker.Current.Contact);
            }
        }

        private static object CreateContactManager()
        {
            return Sitecore.Configuration.Factory.CreateObject("tracking/contactManager", true);
        }

        protected void ReloadContactDataIntoSession()
        {
            if (Tracker.Current?.Contact == null)
                return;

            if (CreateContactManager() is Sitecore.Analytics.Tracking.ContactManager manager)
            {
                manager.RemoveFromSession(Tracker.Current.Contact.ContactId);
                Tracker.Current.Session.Contact = manager.LoadContact(Tracker.Current.Contact.ContactId);
            }
        }

        public void IdentifyCurrentAndUpdateEmailFacet(string source, string identifier)
        {
            if (Tracker.Current?.Session != null)
            {
                Tracker.Current.Session.IdentifyAs(source, identifier);
            }

            if (IsValidEmail(identifier))
                UpdateOrCreateContactWithEmail(source, identifier);
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public void UpdateOrCreateContactWithEmail(string source, string identifier)
        {
            using (var client = SitecoreXConnectClientConfiguration.GetClient())
            {
                var reference = new IdentifiedContactReference(source, identifier);
                var contact = client.Get(reference, new ContactExpandOptions(CollectionModel.FacetKeys.EmailAddressList));
                if (contact == null)
                {
                    contact = new Contact(new ContactIdentifier(reference.Source, reference.Identifier, ContactIdentifierType.Known));
                    SetEmailFacet(contact, identifier, client);
                    client.AddContact(contact);
                    client.Submit();
                }
                else if (contact.Emails()?.PreferredEmail.SmtpAddress != identifier)
                {
                    SetEmailFacet(contact, identifier, client);
                    client.Submit();
                }
            }
        }

        private static void SetEmailFacet(Contact contact, string email, IXdbContext client)
        {
            if (string.IsNullOrEmpty(email))
            {
                return;
            }
            var emailFacet = contact.Emails();
            if (emailFacet == null)
            {
                emailFacet = new EmailAddressList(new EmailAddress(email, false), "Preferred");
            }
            else
            {
                if (emailFacet.PreferredEmail?.SmtpAddress == email)
                {
                    return;
                }
                emailFacet.PreferredEmail = new EmailAddress(email, false);
            }
            client.SetEmails(contact, emailFacet);
        }
    }
}