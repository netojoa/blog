using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using SitecoreMusicApp.Droid.Managers;
using System.Collections.Generic;
using System.Linq;

namespace SitecoreMusicApp.Droid
{
    [Activity(Theme = "@style/AppTheme.NoActionBar", Label = "@string/ApplicationName", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            DisplayMusicRepository();

        }

        public async void DisplayMusicRepository()
        {
            SitecoreManager sitecoreManager = new SitecoreManager();

            var sitecoreItems = await sitecoreManager.GetItemByPath("/sitecore/content/Music Store/Repository",
                                Sitecore.MobileSDK.API.Request.Parameters.PayloadType.Content,
                                new System.Collections.Generic.List<Sitecore.MobileSDK.API.Request.Parameters.ScopeType> { Sitecore.MobileSDK.API.Request.Parameters.ScopeType.Children });

            List<string> listItems = new List<string>(); ;

            if (sitecoreItems != null)
                foreach (var item in sitecoreItems)
                {
                    listItems.Add(string.Format("Album {0}, Author {1}, Year {2}", 
                                                item["Album Name"].RawValue, 
                                                item["Author"].RawValue, 
                                                item["Year"].RawValue));
                }

            ListView musicList = FindViewById<ListView>(Resource.Id.musicListView);
            ArrayAdapter adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, listItems.ToArray<string>());
            musicList.Adapter = adapter;

        }

    }
}

