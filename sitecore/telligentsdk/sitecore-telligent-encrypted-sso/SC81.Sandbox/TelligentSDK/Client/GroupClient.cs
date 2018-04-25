using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Telligent.Evolution.Extensibility.Rest.Version1;

namespace SC81.Sandbox.TelligentSDK.Client
{
    public class GroupClient : BaseTelligentClient
    {

        public const string GroupsEndpoint = "groups.json";
        
        public struct GroupTypes
        {
            public const string Joinless = "Joinless";
            public const string PublicOpen = "PublicOpen";
            public const string PublicClosed = "PublicClosed";
            public const string PrivateListed = "PrivateListed";
            public const string PrivateUnlisted = "PrivateUnlisted";
            public const string All = "All";
        }


        public void GetUserGroups(string userId)
        {

            var options = new RestGetOptions
            {
                QueryStringParameters = new NameValueCollection { { "UserId", userId } }
            };

            bool hasError;
            var response = CallEndpoint(GroupsEndpoint, options, out hasError);
            if (hasError || response.Errors.Count > 0)
                return;

        }

        public void GetGroupsAvailable()
        {

            var options = new RestGetOptions
            {
                QueryStringParameters = new NameValueCollection
                {
                    {"GroupTypes", string.Format("{0},{1},{2},{3}", GroupTypes.Joinless, 
                                                                    GroupTypes.PrivateListed, 
                                                                    GroupTypes.PublicClosed, 
                                                                    GroupTypes.PublicOpen)
                    }
                }
            };

            bool hasError;
            var response = CallEndpoint(GroupsEndpoint, options, out hasError);
            if (hasError || response.Errors.Count > 0)
                return;

        }

    }
}