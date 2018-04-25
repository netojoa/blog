using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using SitecoreMusicApp.Droid.Models;
using System.Collections.Generic;

namespace SitecoreMusicApp.Droid.Adapters
{
    public class AlbumsAdapter : RecyclerView.Adapter
    {
        private string TAG = "AlbumsAdapter";

        private List<Album> _datasource;
        private Context _context;

        public AlbumsAdapter(Context context, List<Album> datasource)
        {
            _datasource = datasource;
            _context = context;
        }

        public void UpdateDataSource(List<Album> datasource)
        {
            _datasource = datasource;
            NotifyDataSetChanged();
        }

        #region Overrides

        public override int ItemCount
        {
            get
            {
                return _datasource.Count;
            }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            Album album = _datasource[position];
            AlbumViewHolder viewHolder = holder as AlbumViewHolder;
            viewHolder.Title.Text = album.Name;
            viewHolder.Count.Text = string.Format("{0} songs", album.NumberOfSongs);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.AlbumCard, parent, false);
            return new AlbumViewHolder(itemView);
        }

        #endregion
               
    }
}