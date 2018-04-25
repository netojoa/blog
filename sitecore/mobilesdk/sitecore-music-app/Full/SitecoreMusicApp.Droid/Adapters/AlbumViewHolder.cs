using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace SitecoreMusicApp.Droid.Adapters
{
    public class AlbumViewHolder : RecyclerView.ViewHolder
    {
        public TextView Title { get; set; }
        public TextView Count { get; set; }
        public ImageView Thumbnail { get; set; }
        public ImageView Overflow { get; set; }

        public AlbumViewHolder(View view) : base(view)
        {
            this.Title = view.FindViewById<TextView>(Resource.Id.title);
            this.Count = view.FindViewById<TextView>(Resource.Id.count);
            this.Thumbnail = view.FindViewById<ImageView>(Resource.Id.thumbnail);
            this.Overflow = view.FindViewById<ImageView>(Resource.Id.overflow);
        }
    }
}