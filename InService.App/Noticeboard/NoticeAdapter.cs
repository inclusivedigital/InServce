using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using InService.App.Data;
using System.Collections.Generic;
using System.Linq;
using Android.Support.V4.Widget;
using InService.Lib;

namespace InService.App.Noticeboard
{
    class NoticeAdapter : RecyclerView.Adapter
    {
        public event EventHandler<NoticeAdapterClickEventArgs> ItemClick;
        public event EventHandler<NoticeAdapterClickEventArgs> ItemLongClick;
        public List<Notice> Items { get; private set; }
        List<Notice> itemsUnfiltered;

        public string Filter
        {
            set
            {
                if (string.IsNullOrWhiteSpace(value)) Items = itemsUnfiltered.ToList();
                else Items = itemsUnfiltered.Where(c => c.Heading.ToLower().Contains(value.ToLower()) == true).ToList();
                NotifyDataSetChanged();
            }
        }

        public NoticeAdapter(List<Notice> data)
        {
            Items = data;
            itemsUnfiltered = new List<Notice>(data);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.list_item_notice, parent, false);
            var vh = new NoticeAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = Items[position];
            var holder = viewHolder as NoticeAdapterViewHolder;
            holder.TextName.Text = item.Heading;
            holder.TextType.Text = item.Type.ToEnumString();
            if (!item.IsExpired)
            {
                ImageViewCompat.SetImageTintList(holder.Image, Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.MediumSeaGreen));
                holder.Image.SetImageResource(Resource.Drawable.ic_check_circle_outline);
            }
            else
            {
                ImageViewCompat.SetImageTintList(holder.Image, Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Red));
                holder.Image.SetImageResource(Resource.Drawable.ic_close_circle_outline);
            }
        }
        public override int ItemCount => Items.Count;

        void OnClick(NoticeAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(NoticeAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);
    }

    public class NoticeAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView TextName { get; private set; }
        public TextView TextType { get; private set; }
        public ImageView Image { get; private set; }
        public NoticeAdapterViewHolder(View itemView, Action<NoticeAdapterClickEventArgs> clickListener,
                            Action<NoticeAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            TextName = itemView.FindViewById<TextView>(Resource.Id.text_item_name);
            TextType = itemView.FindViewById<TextView>(Resource.Id.text_item_type);
            Image = itemView.FindViewById<ImageView>(Resource.Id.img_icon);
            itemView.Click += (sender, e) => clickListener(new NoticeAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new NoticeAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class NoticeAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}