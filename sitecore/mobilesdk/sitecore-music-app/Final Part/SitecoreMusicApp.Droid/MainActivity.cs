using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Widget;
using Com.Bumptech.Glide;
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

        private AppBarLayout _appBarLayout;
        private CollapsingToolbarLayout _collapsingToolbar;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            _albums = new List<Album>();
            SetupRecyclerView();

            SetupCollapsingToolbar();
            LoadBackdropCover();

        }

        private void SetupCollapsingToolbar()
        {
            _collapsingToolbar = FindViewById<CollapsingToolbarLayout>(Resource.Id.collapsingToolbar);
            _collapsingToolbar.SetTitle(" ");

            this._appBarLayout = FindViewById<AppBarLayout>(Resource.Id.appBar);
            this._appBarLayout.SetExpanded(true);
            this._appBarLayout.OffsetChanged += AppBarLayout_OffsetChanged;
        }

        private void LoadBackdropCover()
        {
            try
            {
                Glide.With(this).Load(Resource.Drawable.Backdrop)
                    .Into((ImageView)FindViewById(Resource.Id.imageBackdrop));
            }
            catch (System.Exception e)
            {
                Log.Error("SitecoreMusicApp", e.Message);
            }
        }

        private void AppBarLayout_OffsetChanged(object sender, AppBarLayout.OffsetChangedEventArgs e)
        {

            int scrollRange = -1;
            if (scrollRange == -1) scrollRange = _appBarLayout.TotalScrollRange;

            if (scrollRange + e.VerticalOffset == 0)
            {
                this._collapsingToolbar.SetTitle(GetString(Resource.String.ApplicationName));
            }
            else
            {
                this._collapsingToolbar.SetTitle(" ");
            }
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

