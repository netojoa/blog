using System;
using System.Web;
using System.Linq;
using Sitecore.Security.Authentication;
using Sitecore;

namespace SC81.Sandbox.Authentication
{
    public static class TelligentSSOManager
    {
        public const bool EncryptEnabled = false;
        public const string ExtranetDomain = "extranet";

        public const string CookieDomain = "local.nlc";
        public const string AuthenticatedUserCookieName = "SitecoreSandboxUser";
        public const string CookieKeyNameForUsername = "username";
        public const string CookieKeyNameForEmail = "emailAddress";
        public const string CookieKeyNameForDisplayName = "commonname";     

        public static void Login(string userName)
        {

            var scUser = Sitecore.Security.Accounts.User.FromName(userName, true);
            if (scUser == null) return;

            var customProfile = scUser.Profile;
            var userId = scUser.LocalName;
            var email = customProfile.Email;
            var displayName = customProfile.FullName;

            var cookieName = AuthenticatedUserCookieName;
            if (HttpContext.Current.Request.Cookies[cookieName] != null)
                return;


            var cookieValue = string.Format("{0}={1}&{2}={3}&{4}={5}", CookieKeyNameForUsername, userId,
                                                                   CookieKeyNameForEmail, email,
                                                                   CookieKeyNameForDisplayName, displayName);            

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