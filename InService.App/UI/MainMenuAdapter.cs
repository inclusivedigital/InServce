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
    class MainMenuAdapter : RecyclerView.Adapter
    {
        public event EventHandler<MainMenuAdapterClickEventArgs> ItemClick;
        public event EventHandler<MainMenuAdapterClickEventArgs> ItemLongClick;
        MainMenuItem[] items;

        public MainMenuAdapter(MainMenuItem[] data)
        {
            items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.listview_item_main_menu, parent, false);
            var vh = new MainMenuAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as MainMenuAdapterViewHolder;
            holder.TextViewLabel.Text = item.Label;
            holder.ImageViewIcon.SetImageResource(item.IconResID.GetValueOrDefault());
            holder.TextViewCount.Text = item.Count?.ToString();
            holder.TextViewCount.Visibility = string.IsNullOrWhiteSpace(item.Count) ? ViewStates.Invisible : ViewStates.Visible;
            if (item.IconTint.HasValue)
            {
                ImageViewCompat.SetImageTintList(holder.ImageViewIcon, Android.Content.Res.ColorStateList.ValueOf(new Color(item.IconTint.Value)));
                holder.TextViewCount.SetTextColor(new Color(item.IconTint.Value));
            }
            else
            {
                holder.ImageViewIcon.ImageTintList = null;
            }
            //if (item.BackgroundResID.HasValue) holder.ItemView.SetBackgroundResource(item.BackgroundResID.Value);
        }

        public override int ItemCount => items.Length;

        void OnClick(MainMenuAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(MainMenuAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class MainMenuAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView TextViewLabel { get; private set; }
        public TextView TextViewCount { get; private set; }
        public ImageView ImageViewIcon { get; private set; }

        public MainMenuAdapterViewHolder(View itemView, Action<MainMenuAdapterClickEventArgs> clickListener,
                            Action<MainMenuAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            TextViewCount = itemView.FindViewById<TextView>(Resource.Id.textView_count);
            TextViewLabel = itemView.FindViewById<TextView>(Resource.Id.textView_label);
            ImageViewIcon = itemView.FindViewById<ImageView>(Resource.Id.imageView_icon);

            itemView.Click += (sender, e) => clickListener(new MainMenuAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new MainMenuAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class MainMenuAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}