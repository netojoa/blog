using Sitecore;
using Sitecore.Security.Authentication;
using System;
using System.Web;

namespace SC81.Sandbox.TelligentSDK.SSO
{
    public static class TelligentSSOManager
    {
        public const bool EncryptEnabled = true;
        public const string ExtranetDomain = "extranet";
        public const string CookieDomain = "local.nlc";
        public const string AuthenticatedUserCookieName = "SitecoreSandboxUser";
        public const string CookieKeyNameForUsername = "username";
        public const string CookieKeyNameForEmail = "emailAddress";
        public const string CookieKeyNameForDisplayName = "commonname";

        public const string EncryptionKey = "FFhrYY4xw9Y/xRKE7eS4jV/2YaPbpt7ryvjJ1E8SwV0=";
        public const string HmacKey = "NNeWjU+i4/V9lkVhIRoWY3CfxBy7nmU3okSD/9fBqnScP8DbdY7elgow0xi3LDyQWMd795gnL+2v+ZHpYUJlMg==";

        public static bool IsAuthenticated
        {
            get
            {
                return Sitecore.Context.IsLoggedIn &&
                       Sitecore.Context.User.GetDomainName() == ExtranetDomain;
            }
        }

        public static void RefreshCookieState()
        {
            if (!IsAuthenticated)
            {
                Logout();
            }
            else
            {
                Login(Context.User.Name);
            }
        }

        public static void Login(string userName)
        {

            var scUser = Sitecore.Security.Accounts.User.FromName(userName, true);
            if (scUser == null) return;

            var customProfile = scUser.Profile;
            var userId = scUser.LocalName;
            if (string.IsNullOrEmpty(userId))
                userId = customProfile.Email;
            if (string.IsNullOrEmpty(userId))
                userId = customProfile.FullName.Replace(" ", "_");

            var email = customProfile.Email;
            var displayName = customProfile.FullName;

            var cookieName = AuthenticatedUserCookieName;
            if (HttpContext.Current.Request.Cookies[cookieName] != null)
                return;

            var cookieValue = EncryptEnabled ?
                (new AesHmac(EncryptionKey, HmacKey).SecureCookieForSSO(CookieKeyNameForUsername, userId, CookieKeyNameForEmail, email, CookieKeyNameForDisplayName, displayName)) :
                (new NoEncrypt().SecureCookieForSSO(CookieKeyNameForUsername, userId, CookieKeyNameForEmail, email, CookieKeyNameForDisplayName, displayName));

            var cookie = new HttpCookie(AuthenticatedUserCookieName, cookieValue)
            {
                HttpOnly = true,
                Domain = CookieDomain,
                Expires = DateTime.Now.AddMonths(6)
            };


            HttpContext.Current.Response.SetCookie(cookie);

        }

        public static void Logout()
        {

            var cookieName = AuthenticatedUserCookieName;

            if (HttpContext.Current.Request.Cookies[cookieName] == null)
                return;

            var cookieToSave = new HttpCookie(cookieName)
            {
                Expires = DateTime.Today.AddDays(-10),
                Domain = CookieDomain
            };

            HttpContext.Current.Response.Cookies.Add(cookieToSave);

            AuthenticationManager.Logout();

        }

    }
}