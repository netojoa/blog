using Android.App;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Widget;
using Com.Bumptech.Glide;
using Java.Lang;
using MusicApp.Droid.Extensions.ItemDecorators;
using SitecoreMusicApp.Droid.Adapters;
using SitecoreMusicApp.Droid.Managers;
using SitecoreMusicApp.Droid.Models;
using System;
using System.Collections.Generic;
using static Android.Support.V7.Widget.RecyclerView;

namespace SitecoreMusicApp.Droid
{
    [Activity(Label = "@string/ApplicationName", MainLauncher = true, Theme = "@style/AppTheme.NoActionBar")]
    public class MainActivity : AppCompatActivity
    {
        private RecyclerView _recyclerView;
        private AppBarLayout _appBarLayout;
        private CollapsingToolbarLayout _collapsingToolbar;
        private AlbumsAdapter _adapter;
        private List<Album> _albums;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            SetupCollapsingToolbar();
            SetupRecyclerView();
            LoadBackdropCover();

        }


        private void SetupRecyclerView()
        {
            _recyclerView = FindViewById<RecyclerView>(Resource.Id.recyclerView);
            _albums = new List<Album>();
            _adapter = new AlbumsAdapter(this, _albums);
            LayoutManager layoutManager = new GridLayoutManager(this, 2);
            _recyclerView.SetLayoutManager(layoutManager);
            _recyclerView.AddItemDecoration(new GridSpacingItemDecoration(2, dpToPx(10), true));
            _recyclerView.SetItemAnimator(new DefaultItemAnimator());
            _recyclerView.SetAdapter(_adapter);
            PrepareAlbums();
        }

        private async void PrepareAlbums()
        {
            MusicStoreManager musicStoreManager = new MusicStoreManager();
            _albums = await musicStoreManager.GetAlbums();
            _adapter.UpdateDataSource(_albums);
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

        private int dpToPx(int dp)
        {
            Android.Util.DisplayMetrics displayMetrics = this.Resources.DisplayMetrics;
            int px = Convert.ToInt32(System.Math.Round(dp * (displayMetrics.Xdpi / (float)Android.Util.DisplayMetricsDensity.Default)));
            return px;
        }

        private void AppBarLayout_OffsetChanged(object sender, AppBarLayout.OffsetChangedEventArgs e)
        {

            bool isShow = false;

            int scrollRange = -1;
            if (scrollRange == -1) scrollRange = _appBarLayout.TotalScrollRange;

            if (scrollRange + e.VerticalOffset == 0)
            {
                this._collapsingToolbar.SetTitle(GetString(Resource.String.ApplicationName));
                isShow = true;
            }
            else
            {
                this._collapsingToolbar.SetTitle(" ");
                isShow = false;
            }
        }

    }
}

