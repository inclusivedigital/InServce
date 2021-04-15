using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Support.V4.Content;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Support.V4.Widget;

namespace InService.App.UI
{
    class MainMenuIconAdapter : RecyclerView.Adapter
    {
        public event EventHandler<MainMenuIconAdapterClickEventArgs> ItemClick;
        public event EventHandler<MainMenuIconAdapterClickEventArgs> ItemLongClick;
        MainMenuItemIcon[] items;

        public MainMenuIconAdapter(MainMenuItemIcon[] data)
        {
            items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.listview_item_main_menu_icon, parent, false);
            var vh = new MainMenuIconAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];
            var holder = viewHolder as MainMenuIconAdapterViewHolder;
            holder.ImageViewIcon.SetImageBitmap(item.Base64String.BaseStringToBitmap());
            holder.TextTitle.Text = item.Title;

        }

        public override int ItemCount => items.Length;

        void OnClick(MainMenuIconAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(MainMenuIconAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class MainMenuIconAdapterViewHolder : RecyclerView.ViewHolder
    {
        public ImageView ImageViewIcon { get; private set; }
        public TextView TextTitle { get; private set; }
        public MainMenuIconAdapterViewHolder(View itemView, Action<MainMenuIconAdapterClickEventArgs> clickListener,
                            Action<MainMenuIconAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            ImageViewIcon = itemView.FindViewById<ImageView>(Resource.Id.imageView_icon);
            TextTitle = itemView.FindViewById<TextView>(Resource.Id.menu_item_title);

            itemView.Click += (sender, e) => clickListener(new MainMenuIconAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new MainMenuIconAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class MainMenuIconAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}