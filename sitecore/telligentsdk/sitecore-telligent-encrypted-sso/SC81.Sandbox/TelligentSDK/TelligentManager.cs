using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Telligent.Evolution.Extensibility.Rest.Version1;
using Telligent.Rest.SDK;

namespace SC81.Sandbox.TelligentSDK
{
    public class TelligentManagerSingleton
    {

        private const string DefaultUsername = "admin";
        private const string CommunityUrl = "http://telligentcommunity.local.nlc";
        private const string ClientId = "6dca350b-7ec8-43b0-8782-d3612dfe6d75";
        private const string ClientSecret = "25e5781eb82c4133a46df0e8865237f5284248c3f67e4800ac7455f0e9bcd9e5";


        private TelligentManagerSingleton()
        {

        }

        private static TelligentManagerSingleton singleton;

        public static TelligentManagerSingleton GetInstance()
        {
            if (singleton == null)
                singleton = new TelligentManagerSingleton();
            return singleton;
        }

        public ClientCredentialsRestHost GetHost()
        {
            return new ClientCredentialsRestHost(DefaultUsername, CommunityUrl, ClientId, ClientSecret);
        }

        public dynamic CallEndpoint(ClientCredentialsRestHost host, string endpoint, RestOptions options, out bool threwException)
        {
            dynamic response = null;
            threwException = false;
            try
            {
                if (options is RestDeleteOptions)
                    response = host.DeleteToDynamic(2, endpoint, true, (RestDeleteOptions)options);
                else if (options is RestPutOptions)
                    response = host.PutToDynamic(2, endpoint, true, (RestPutOptions)options);
                else if (options is RestPostOptions)
                    response = host.PostToDynamic(2, endpoint, true, (RestPostOptions)options);
                else
                    response = host.GetToDynamic(2, endpoint, true, (RestGetOptions)options);
            }
            catch (Exception ex)
            {
                if (options is RestDeleteOptions)
                    return response;

                threwException = true;

                var sb = new StringBuilder();
                sb.AppendLine(string.Format("TelligentManager.CallEndpoint - Endpoint: {0}", endpoint));
                sb.AppendLine("Options: ");
                sb.AppendLine(Newtonsoft.Json.JsonConvert.SerializeObject(options));
                sb.AppendLine("Exception Messages: ");
                sb.AppendLine(ex.Message);
                sb.AppendLine("Stack Trace: ");
                sb.AppendLine(ex.StackTrace);

                Sitecore.Diagnostics.Log.Error(sb.ToString(), this);

            }

            return response;
        }


    }
}