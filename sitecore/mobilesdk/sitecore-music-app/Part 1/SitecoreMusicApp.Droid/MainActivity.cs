using Android.App;
using Android.OS;
using Android.Support.V7.App;

namespace SitecoreMusicApp.Droid
{
    [Activity(Theme = "@style/AppTheme.NoActionBar", Label = "@string/ApplicationName", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            // SetContentView (Resource.Layout.Main);
        }
    }
}

