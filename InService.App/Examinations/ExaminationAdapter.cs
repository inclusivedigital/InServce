using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using InService.App.Data;

namespace InService.App.Examinations
{
    class ExaminationAdapter : RecyclerView.Adapter
    {
        public event EventHandler<ExaminationAdapterClickEventArgs> ItemClick;
        public event EventHandler<ExaminationAdapterClickEventArgs> ItemLongClick;
        public List<Examination> Items { get; private set; }
        List<Examination> itemsUnfiltered;

        public string Filter
        {
            set
            {
                if (string.IsNullOrWhiteSpace(value)) Items = itemsUnfiltered.ToList();
                else Items = itemsUnfiltered.Where(c => c.Topic.ToLower().Contains(value.ToLower()) == true).ToList();
                NotifyDataSetChanged();
            }
        }

        public ExaminationAdapter(List<Examination> data)
        {
            Items = data;
            itemsUnfiltered = new List<Examination>(data);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.list_item_examination, parent, false);
            var vh = new ExaminationAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        public async override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = Items[position];
            var holder = viewHolder as ExaminationAdapterViewHolder;
            holder.TextTopic.Text = item.Topic;
            holder.TextCourseName.Text = item.Course.Name;
            holder.TextCourseCode.Text = item.Course.Code;
            holder.TextExpiry.Text = item.EndDate.ToString("dd MMM yyy");
            if (item.IsClosed)
            {
                ImageViewCompat.SetImageTintList(holder.Image, Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.DarkGray));
                holder.Image.SetImageResource(Resource.Drawable.ic_check_circle_outline);
            }
            else if (item.IsInProgress)
            {
                ImageViewCompat.SetImageTintList(holder.Image, Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.MediumSeaGreen));
                holder.Image.SetImageResource(Resource.Drawable.ic_check_circle_outline);
            }
            else
            {
                ImageViewCompat.SetImageTintList(holder.Image, Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.OrangeRed));
                holder.Image.SetImageResource(Resource.Drawable.ic_close_circle_outline);
            }
        }

        public override int ItemCount => Items.Count;

        void OnClick(ExaminationAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(ExaminationAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class ExaminationAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView TextTopic { get; private set; }
        public TextView TextCourseCode { get; private set; }
        public TextView TextCourseName { get; private set; }
        public TextView TextExpiry { get; private set; }
        public ImageView Image { get; private set; }

        public ExaminationAdapterViewHolder(View itemView, Action<ExaminationAdapterClickEventArgs> clickListener,
                            Action<ExaminationAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            TextTopic = itemView.FindViewById<TextView>(Resource.Id.text_topic);
            TextCourseCode = itemView.FindViewById<TextView>(Resource.Id.text_coursecode);
            TextCourseName = itemView.FindViewById<TextView>(Resource.Id.text_coursename);
            TextExpiry = itemView.FindViewById<TextView>(Resource.Id.text_date);
            Image = itemView.FindViewById<ImageView>(Resource.Id.img_status);
            itemView.Click += (sender, e) => clickListener(new ExaminationAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new ExaminationAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class ExaminationAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}