using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using Telligent.Evolution.Extensibility.Rest.Version1;

namespace SC81.Sandbox.TelligentSDK.Client
{
    public class UserClient : BaseTelligentClient
    {

        public const string UsersEndpoint = "users.json";

        public string GetUsers()
        {
            
            var options = new RestGetOptions { };
            bool hasError;
            dynamic response = base.CallEndpoint(UsersEndpoint, options, out hasError);
            return Newtonsoft.Json.JsonConvert.SerializeObject(response.Users);

        }

        public string GetUserId(string name)
        {

            var options = new RestGetOptions
            {
                QueryStringParameters = new NameValueCollection
                {
                    {"Usernames", name}
                }
            };

            bool hasError;
            dynamic response = CallEndpoint(UsersEndpoint, options, out hasError);

            var telligentId = string.Empty;
            if (hasError)
                return telligentId;

            if (response.TotalCount > 0)
                telligentId = response.Users[0].Id.ToString();

            return telligentId;
        }


    }
}