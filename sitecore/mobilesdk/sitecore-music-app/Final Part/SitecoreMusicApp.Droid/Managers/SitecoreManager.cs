
using Android.Util;
using Sitecore.MobileSDK.API;
using Sitecore.MobileSDK.API.Exceptions;
using Sitecore.MobileSDK.API.Items;
using Sitecore.MobileSDK.API.Request;
using Sitecore.MobileSDK.API.Request.Parameters;
using Sitecore.MobileSDK.API.Session;
using Sitecore.MobileSDK.PasswordProvider;
using Sitecore.MobileSDK.PasswordProvider.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SitecoreMusicApp.Droid.Managers
{
    public class SitecoreManager
    {

        public async Task<ScItemsResponse> GetItemByPath(string itemPath, PayloadType itemLoadType, List<ScopeType> itemScopeTypes, string itemLanguage = "en")
        {
            try
            {
                using (ISitecoreWebApiSession session = GetSession())
                {
                    IReadItemsByPathRequest request = ItemWebApiRequestBuilder.ReadItemsRequestWithPath(itemPath)
                        .Payload(itemLoadType)
                        .AddScope(itemScopeTypes)
                        .Language(itemLanguage)
                        .Build();
                    return await session.ReadItemAsync(request);

                }
            }
            catch (SitecoreMobileSdkException ex)
            {
                Log.Error("Error in GetItemByPath,  id {0} . Error: {1}", itemPath, ex.Message);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("Error in GetItemByPath,  id {0} . Error: {1}", itemPath, ex.Message);
                throw ex;
            }
        }

        public async Task<ScItemsResponse> GetItemById(string itemId, PayloadType itemLoadType, List<ScopeType> itemScopeTypes, string itemLanguage = "en")
        {

            try
            {

                using (ISitecoreWebApiSession session = GetSession())
                {
                    IReadItemsByIdRequest request = ItemWebApiRequestBuilder.ReadItemsRequestWithId(itemId)
                        .Payload(itemLoadType)
                        .AddScope(itemScopeTypes)
                        .Language(itemLanguage)
                        .Build();

                    return await session.ReadItemAsync(request);

                }

            }
            catch (SitecoreMobileSdkException ex)
            {
                Log.Error("Error in GetItemById,  id {0} . Error: {1}", itemId, ex.Message);
                throw ex;
            }
            catch (Exception ex)
            {
                Log.Error("Error in GetItemById,  id {0} . Error: {1}", itemId, ex.Message);
                throw ex;
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