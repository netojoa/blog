namespace Custom.Foundation.ExmExtensions.Pipelines.TransformQueryString
{
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.DependencyInjection;
    using Sitecore.Diagnostics;
    using Sitecore.ExM.Framework.Diagnostics;
    using Sitecore.ExM.Framework.Helpers;
    using Sitecore.Modules.EmailCampaign.Core.Crypto;
    using Sitecore.Modules.EmailCampaign.Core.Pipelines.TransformQueryString;
    using System.Collections.Generic;
    using System.Collections.Specialized;

    public class DecryptQueryString : Sitecore.Modules.EmailCampaign.Core.Pipelines.TransformQueryString.DecryptQueryString
    {
        private readonly ILogger _logger;

        private readonly HashSet<string> fieldNames = new HashSet<string>();

        private readonly string _oldCryptographicKey;
        private readonly string _oldAuthenticationKey;

        private readonly bool _abortIfNotEncrypted;
        private readonly QueryStringEncryption _queryStringEncryption;

        public DecryptQueryString(QueryStringEncryption queryStringEncryption, string abortIfNotEncrypted, string oldCryptographicKey, string oldAuthenticationKey) :
            this(queryStringEncryption,
                AssertString.ArgumentToBool(abortIfNotEncrypted, nameof(abortIfNotEncrypted), (string)null),
                oldCryptographicKey,
                oldAuthenticationKey)
        {
        }

        public DecryptQueryString(QueryStringEncryption queryStringEncryption, bool abortIfNotEncrypted, string oldCryptographicKey, string oldAuthenticationKey) : base(queryStringEncryption, abortIfNotEncrypted)
        {
            Assert.ArgumentNotNull((object)queryStringEncryption, nameof(queryStringEncryption));
            this._abortIfNotEncrypted = abortIfNotEncrypted;
            this._queryStringEncryption = queryStringEncryption;
            this._oldCryptographicKey = oldCryptographicKey;
            this._oldAuthenticationKey = oldAuthenticationKey;
            this._logger = ServiceLocator.ServiceProvider.GetService<ILogger>();
        }

        public new void Process(TransformQueryStringPipelineArgs args)
        {
            this._logger.LogDebug($"Attempting to decrypt query string {args.Query.ToString()}");

            Assert.ArgumentNotNull((object)args, nameof(args));

            if (args.IsValidLegacyQuery)
                return;

            var encryptionResult = this._queryStringEncryption.TryDecrypt(args.Query, out var queryResult);

            if (encryptionResult == QueryStringDecryptResult.DecryptionFailed ||
                encryptionResult == QueryStringDecryptResult.QueryNotEncrypted)
            {
                this._logger.LogDebug($"Decrypt query string failed with current encryption keys. Attempting with previous ones. A: {_oldAuthenticationKey}; C: {_oldCryptographicKey}");
                encryptionResult = AttemptDecryptWithPreviousEncryptionKeys(args.Query, out queryResult);
            }

            this._logger.LogDebug($"Final encryption result is {encryptionResult.ToString()}");

            switch (encryptionResult)
            {
                case QueryStringDecryptResult.QueryNotEncrypted:
                    if (!this._abortIfNotEncrypted)
                        break;
                    args.AbortPipeline();
                    break;

                case QueryStringDecryptResult.DecryptionFailed:
                    args.AbortPipeline();
                    break;

                default:
                    args.Query = queryResult;
                    break;
            }
        }

        public void RegisterFieldName(string fieldName)
        {
            Assert.ArgumentNotNullOrEmpty(fieldName, nameof(fieldName));
            this.fieldNames.Add(fieldName);
        }

        private QueryStringDecryptResult AttemptDecryptWithPreviousEncryptionKeys(NameValueCollection argsQuery, out NameValueCollection queryResult)
        {
            queryResult = new NameValueCollection();
            var queryStringEncryption = GetQueryStringEncryptionInstance();
            var encryptionResult = queryStringEncryption.TryDecrypt(argsQuery, out queryResult);

            return encryptionResult;
        }

        private QueryStringEncryption GetQueryStringEncryptionInstance()
        {
            var authenticatedAesStringCipher = new AuthenticatedAesStringCipher(
                cryptographicKeyName: this._oldCryptographicKey,
                authenticationKeyName: this._oldAuthenticationKey,
                logger: ServiceLocator.ServiceProvider.GetService<ILogger>());

            var instance = new QueryStringEncryption(
                Sitecore.Configuration.Settings.GetSetting("QueryStringKey.ExmEncryptedQuery", "ec_eq"),
                authenticatedAesStringCipher);

            foreach (var fieldName in fieldNames)
            {
                instance.RegisterFieldName(fieldName);
            }

            return instance;
        }
    }
}