using System.Web.Http.Filters;

namespace Custom.XConnect.Security.Web
{
    public static class AuthenticationHelper
    {
        public static bool IsBasicAuthentication(HttpAuthenticationContext context)
        {
            return context.Request?.Headers?.Authorization?.Scheme == "Basic";
        }
    }
}