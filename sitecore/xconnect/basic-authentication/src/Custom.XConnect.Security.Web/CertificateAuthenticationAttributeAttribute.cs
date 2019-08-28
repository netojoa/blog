using Sitecore.XConnect.Security.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace Custom.XConnect.Security.Web
{
    public class CertificateAuthenticationAttributeAttribute : ActionFilterAttribute, IAuthenticationFilter, IFilter
    {
        protected string[] ValidCertificateThumbprints = new string[0];

        public override bool AllowMultiple => false;

        public string[] ValidCertificates
        {
            get => this.ValidCertificateThumbprints;
            set
            {
                if (value == null)
                {
                    this.ValidCertificateThumbprints = new string[0];
                    return;
                }
                this.ValidCertificateThumbprints = value.Select<string, string>(new Func<string, string>(CertificateAuthenticationAttributeAttribute.NormalizeThumbprintString)).ToArray<string>();
            }
        }

        public CertificateAuthenticationAttributeAttribute(params string[] thumbprints)
        {
            this.ValidCertificates = thumbprints;
        }

        public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            if (AuthenticationHelper.IsBasicAuthentication(context))
            {
                return Task.FromResult<int>(0);
            }

            return AuthenticateAgainstClientCertificateAsync(context);
        }

        public Task AuthenticateAgainstClientCertificateAsync(HttpAuthenticationContext context)
        {
            HttpRequestMessage request = context.Request;
            X509Certificate2 clientCertificate = request.GetClientCertificate();

            if (clientCertificate == null)
            {
                context.ErrorResult = new AuthenticationFailureResult("Invalid certificate", request);
                return Task.FromResult<int>(0);
            }

            X509Certificate2 x509Certificate2 = new X509Certificate2(clientCertificate);

            string str = CertificateAuthenticationAttributeAttribute.NormalizeThumbprintString(x509Certificate2.Thumbprint);
            if (this.ValidCertificates != null && !this.ValidCertificates.Contains<string>(str))
            {
                context.ErrorResult = new AuthenticationFailureResult("Invalid certificate", request);
                return Task.FromResult<int>(0);
            }

            if (context.ErrorResult == null)
            {
                GenericPrincipal genericPrincipal = new GenericPrincipal(new GenericIdentity(x509Certificate2.Subject, "ClientCertificate"), null);
                context.Principal = genericPrincipal;
            }

            return Task.FromResult<int>(0);
        }

        public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {
            return Task.FromResult<int>(0);
        }

        protected static string NormalizeThumbprintString(string input)
        {
            List<char> chrs = new List<char>();
            string upper = input.ToUpper();
            for (int i = 0; i < upper.Length; i++)
            {
                char chr = upper[i];
                if (chr >= 'A' && chr <= 'Z' || chr >= '0' && chr <= '9')
                {
                    chrs.Add(chr);
                }
            }
            return string.Join<char>("", chrs);
        }
    }
}