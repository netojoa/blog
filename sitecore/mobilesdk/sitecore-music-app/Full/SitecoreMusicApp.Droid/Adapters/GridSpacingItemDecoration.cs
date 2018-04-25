using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;

namespace MusicApp.Droid.Extensions.ItemDecorators
{
    public class GridSpacingItemDecoration : RecyclerView.ItemDecoration
    {

        private int _span;
        private int _spacing;
        private bool _includeEdge;

        public GridSpacingItemDecoration(int span, int spacing, bool includeEdge)
        {
            this._span = span;
            this._spacing = spacing;
            this._includeEdge = includeEdge;
        }

        public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
        {
            int position = parent.GetChildAdapterPosition(view);
            int column = position % this._span;

            if (_includeEdge)
            {

                outRect.Left = this._spacing - column * this._spacing / this._span;
                outRect.Right = (column + 1) * this._spacing / this._span;

                if (position < this._span)
                    outRect.Top = this._spacing;

                outRect.Bottom = this._spacing;

            }
            else
            {
                outRect.Left = column * this._spacing / this._span;
                outRect.Right = this._spacing - (column + 1) * this._spacing / this._span;
                if (position > this._span)
                    outRect.Top = this._spacing;
            }

            base.GetItemOffsets(outRect, view, parent, state);
        }

    }
}