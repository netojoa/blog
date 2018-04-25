using System;

namespace SC81.Sandbox.TelligentSDK.SSO
{
    public class NoEncrypt : IDisposable
    {
        public void Dispose()
        {
        }

        public string SecureCookieForSSO(string cookieKeyNameForUsername, string userName,
                                         string cookieKeyNameForEmail, string email,
                                         string cookieKeyNameForDisplayName, string displayName)
        {
            var ssoData = string.Format("{0}={1}&{2}={3}&{4}={5}", cookieKeyNameForUsername, userName,
                                                                   cookieKeyNameForEmail, email,
                                                                   cookieKeyNameForDisplayName, displayName);
            return ssoData;
        }
    }
}