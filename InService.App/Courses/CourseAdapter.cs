using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using InService.App.Data;
using System.Collections.Generic;
using System.Linq;
using InService.App.UI;

namespace InService.App.Courses
{
    class CourseAdapter : RecyclerView.Adapter
    {
        public event EventHandler<CourseAdapterClickEventArgs> ItemClick;
        public event EventHandler<CourseAdapterClickEventArgs> ItemLongClick;
        public List<Course> Items { get; private set; }
        List<Course> itemsUnfiltered;

        public string Filter
        {
            set
            {
                if (string.IsNullOrWhiteSpace(value)) Items = itemsUnfiltered.ToList();
                else Items = itemsUnfiltered.Where(c => c.Name.ToLower().Contains(value.ToLower()) == true).ToList();
                NotifyDataSetChanged();
            }
        }

        public CourseAdapter(List<Course> data)
        {
            Items = data;
            itemsUnfiltered = new List<Course>(data);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.list_item_course, parent, false);
            var vh = new CourseAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        public async override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = Items[position];
            var holder = viewHolder as CourseAdapterViewHolder;
            holder.TextName.Text = item.Name;
            holder.TextCoursecode.Text = item.Code;
            holder.TextModules.Text = item.Modules.Count.ToString();
            if (item.IconID != null)
            {
                holder.Image.SetImageBitmap(item.Attachment.Data.BaseStringToBitmap());
                //if (item.Thumbnail == null) await item.LoadThumbnail(viewHolder.ItemView.Context);
                //if (item.Thumbnail != null) holder.Image.SetImageBitmap(item.Thumbnail);
            }
        }
        public override int ItemCount => Items.Count;

        void OnClick(CourseAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(CourseAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);
    }

    public class CourseAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView TextName { get; private set; }
        public TextView TextModules { get; private set; }
        public TextView TextCoursecode { get; private set; }
        public ImageView Image { get; private set; }

        public CourseAdapterViewHolder(View itemView, Action<CourseAdapterClickEventArgs> clickListener,
                            Action<CourseAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            TextName = itemView.FindViewById<TextView>(Resource.Id.text_item_name);
            TextModules = itemView.FindViewById<TextView>(Resource.Id.text_item_modules);
            TextCoursecode = itemView.FindViewById<TextView>(Resource.Id.text_item_coursecode);
            Image = itemView.FindViewById<ImageView>(Resource.Id.img_icon);
            itemView.Click += (sender, e) => clickListener(new CourseAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new CourseAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class CourseAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}