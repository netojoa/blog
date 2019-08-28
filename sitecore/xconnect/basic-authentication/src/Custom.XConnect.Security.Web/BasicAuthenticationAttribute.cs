using Sitecore.XConnect.Security.Web;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace Custom.XConnect.Security.Web
{
    public class BasicAuthenticationAttribute : ActionFilterAttribute, IAuthenticationFilter, IFilter
    {
        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            if (AuthenticationHelper.IsBasicAuthentication(context))
            {
                return AuthenticateAgainstBasicAuthAsync(context);
            }

            return Task.FromResult<int>(0);
        }

        public Task AuthenticateAgainstBasicAuthAsync(HttpAuthenticationContext context)
        {
            HttpRequestMessage request = context.Request;
            if (!IsCredentialValid(request))
            {
                context.ErrorResult = new AuthenticationFailureResult("Invalid username and/or password", context.Request);
                return Task.FromResult<int>(0);
            }

            if (context.ErrorResult == null)
            {
                GenericPrincipal genericPrincipal = new GenericPrincipal(new GenericIdentity("EncryptedUser", "BasicAuth"), null);
                context.Principal = genericPrincipal;
            }

            return Task.FromResult<int>(0);
        }

        private bool IsCredentialValid(HttpRequestMessage request)
        {
            return request?.Headers?.Authorization?.Parameter?.Equals(_encryptedUsernameAndPassword) ?? false;
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult<int>(0);
        }

        public override bool AllowMultiple => false;

        private readonly string _encryptedUsernameAndPassword = string.Empty;

        public BasicAuthenticationAttribute(string encryptedUsernameAndPassword)
        {
            this._encryptedUsernameAndPassword = encryptedUsernameAndPassword;
        }
    }
}