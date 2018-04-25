using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telligent.Evolution.Extensibility.Rest.Version1;
using Telligent.Rest.SDK;

namespace SC81.Sandbox.TelligentSDK.Client
{
    public class BaseTelligentClient
    {

        protected readonly ClientCredentialsRestHost Host;
        private TelligentManagerSingleton telligentManager;


        public BaseTelligentClient()
        {
            telligentManager = TelligentManagerSingleton.GetInstance();
            this.Host = telligentManager.GetHost();
        }

        protected dynamic CallEndpoint(string endpoint, RestOptions options, out bool threwException)
        {
            return telligentManager.CallEndpoint(Host, endpoint, options, out threwException);
        }


    }
}