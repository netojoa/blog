using Sitecore.MobileSDK.API;
using Sitecore.MobileSDK.API.Session;
using Sitecore.MobileSDK.PasswordProvider;
using Sitecore.MobileSDK.PasswordProvider.Interface;
using System.Threading.Tasks;

namespace SitecoreBeautifulLogin.Droid.Services
{
    public class SitecoreService
    {

        public async Task<bool> Authenticate(string login, string password)
        {
            using (ISitecoreWebApiSession session = GetSession(login, password))
            {
                return await session.AuthenticateAsync();
            }
        }

        private ISitecoreWebApiSession GetSession(string userName = "", string password = "")
        {

            if (string.IsNullOrEmpty(userName))
            {
                userName = Constants.Sitecore.SitecoreUserName;
                password = Constants.Sitecore.SitecorePassword;
            }

            using (IWebApiCredentials credentials = new SecureStringPasswordProvider(userName, password))
            {
                ISitecoreWebApiSession session = SitecoreWebApiSessionBuilder.AuthenticatedSessionWithHost(Constants.Sitecore.RestBaseUrl)
                    .Credentials(credentials)
                    .Site(Constants.Sitecore.SitecoreShellSite)
                    .DefaultDatabase(Constants.Sitecore.SitecoreDefaultDatabase)
                    .DefaultLanguage(Constants.Sitecore.SitecoreDefaultLanguage)
                    .MediaLibraryRoot(Constants.Sitecore.SitecoreMediaLibraryRoot)
                    .MediaPrefix(Constants.Sitecore.SitecoreMediaPrefix)
                    .DefaultMediaResourceExtension(Constants.Sitecore.SitecoreDefaultMediaResourceExtension)
                    .BuildSession();
                return session;
            }
        }

    }
}