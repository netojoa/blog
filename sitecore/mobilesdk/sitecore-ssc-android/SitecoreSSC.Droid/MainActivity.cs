using Android.App;
using Android.OS;
using Sitecore.MobileSDK.API;
using Sitecore.MobileSDK.API.Items;
using Sitecore.MobileSDK.PasswordProvider;

namespace SitecoreSSC.Droid
{
    [Activity(Label = "SitecoreSSC.Droid", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {

            base.OnCreate(bundle);
            SetHttpsHandler();

            SetContentView(Resource.Layout.Main);
            GetSitecoreContent();

        }


        private void SetHttpsHandler()
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) =>
            {
                return true;
            };
        }

        private async void GetSitecoreContent()
        {

            string instanceUrl = "https://xp930.sc";

            using (var credentials = new ScUnsecuredCredentialsProvider("admin", "b", "sitecore"))

            using (var session =
                   SitecoreSSCSessionBuilder.AuthenticatedSessionWithHost(instanceUrl)
                   .Credentials(credentials)
                   .DefaultDatabase("master")
                   .DefaultLanguage("en")
                   .MediaLibraryRoot("/sitecore/media library")
                   .MediaPrefix("~/media/")
                   .DefaultMediaResourceExtension("ashx")
                   .BuildReadonlySession())
            {

                var request = ItemSSCRequestBuilder.ReadItemsRequestWithPath("/sitecore/content/home").Build();
                ScItemsResponse items = await session.ReadItemAsync(request);
                string fieldContent = items[0]["Text"].RawValue;
                string itemName = "Home Item Text";

                var dialogBuilder = new AlertDialog.Builder(this);
                dialogBuilder.SetTitle(itemName);
                dialogBuilder.SetMessage(fieldContent);
                dialogBuilder.Create().Show();

            }

        }

    }
}

