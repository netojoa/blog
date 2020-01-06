using Sitecore.XConnect;

namespace Custom.Foundation.Forms.Services
{
    public interface IXdbContactService
    {
        void RegisterInteractionEvent(Event interactionEvent);

        void IdentifyCurrentAndUpdateEmailFacet(string source, string identifier);
    }
}