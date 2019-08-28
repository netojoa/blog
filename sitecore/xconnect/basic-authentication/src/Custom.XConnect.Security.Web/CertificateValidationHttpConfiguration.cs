using Microsoft.Extensions.Logging;
using Sitecore.XConnect.DependencyInjection.Web.Abstractions;
using System;
using System.Configuration;
using System.Web.Http;

namespace Custom.XConnect.Security.Web
{
    public class CertificateValidationHttpConfiguration : IHttpConfiguration
    {

        private readonly ILogger<IHttpConfiguration> _logger;

        public CertificateValidationHttpConfiguration(ILogger<IHttpConfiguration> logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger", "CertficateValidationHttpConfiguration instance requires ILogger<T> argument logger is not null");
            }
            this._logger = logger;
        }

        public void ConfigureServices(HttpConfiguration config)
        {
            ConfigureCertificateValidationFilter(config);
            ConfigureBasicAuthValidationFilter(config);
        }

        protected void ConfigureBasicAuthValidationFilter(HttpConfiguration config)
        {
            string usernameAndPasswordSetting = ConfigurationManager.AppSettings["usernameAndPassword"];
            if (string.IsNullOrWhiteSpace(usernameAndPasswordSetting))
            {
                this._logger.LogWarning(string.Format("Basic Auth Filter DISABLED, app setting key [{0}] value is null, empty or whitespace", "usernameAndPassword"), Array.Empty<object>());
                return;
            }
            config.Filters.Add(new BasicAuthenticationAttribute(usernameAndPasswordSetting));
            this._logger.LogInformation(string.Format("Basic Auth Filter Enabled"), Array.Empty<object>());
        }

        protected void ConfigureCertificateValidationFilter(HttpConfiguration config)
        {
            string validateCertificateThumbprintSetting = ConfigurationManager.AppSettings["validateCertificateThumbprint"];
            if (string.IsNullOrWhiteSpace(validateCertificateThumbprintSetting))
            {
                this._logger.LogWarning(string.Format("Certificate Validation DISABLED, Thumbprint app setting key [{0}] value is null, empty or whitespace", "validateCertificateThumbprint"), Array.Empty<object>());
                return;
            }
            config.Filters.Add(new CertificateAuthenticationAttributeAttribute(new string[] { validateCertificateThumbprintSetting }));
            this._logger.LogInformation(string.Format("Certificate Validation Filter Enabled, Thumbprint: {0}", validateCertificateThumbprintSetting), Array.Empty<object>());
        }
    }
}