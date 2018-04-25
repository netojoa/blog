using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Widget;
using SitecoreMusicApp.Droid.Adapters;
using SitecoreMusicApp.Droid.Managers;
using SitecoreMusicApp.Droid.Models;
using System.Collections.Generic;
using System.Linq;
using static Android.Support.V7.Widget.RecyclerView;

namespace SitecoreMusicApp.Droid
{
    [Activity(Theme = "@style/AppTheme.NoActionBar", Label = "@string/ApplicationName", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {

        private RecyclerView _recyclerView;
        private AlbumsAdapter _adapter;
        private List<Album> _albums;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            _albums = new List<Album>();
            SetupRecyclerView();

        }

        private void SetupRecyclerView()
        {

            _recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            _adapter = new AlbumsAdapter(this, _albums);

            LayoutManager layoutManager = new GridLayoutManager(this, 2);
            _recyclerView.SetLayoutManager(layoutManager);
            _recyclerView.SetAdapter(_adapter);

            PrepareAlbums();

        }

        private async void PrepareAlbums()
        {
            MusicStoreManager musicStoreManager = new MusicStoreManager();
            _albums = await musicStoreManager.GetAlbums();
            _adapter.UpdateDataSource(_albums);
        }

    }
}

