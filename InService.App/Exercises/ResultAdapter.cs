using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using System.Collections.Generic;
using InService.App.Data;
using Android.Webkit;
using System.Text;
using System.Linq;
using Android.Support.V4.Widget;

namespace InService.App.Exercises
{
    class ResultAdapter : RecyclerView.Adapter
    {
        public event EventHandler<ResultAdapterClickEventArgs> ItemClick;
        public event EventHandler<ResultAdapterClickEventArgs> ItemLongClick;
        List<UserExaminationDetail> items;

        public ResultAdapter(List<UserExaminationDetail> data)
        {
            items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            var itemView = LayoutInflater.From(parent.Context).
                   Inflate(Resource.Layout.quick_view, parent, false);

            var vh = new ResultAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];
            var holder = viewHolder as ResultAdapterViewHolder;
            holder.TextMyAnswer.Text = item.Answer.Name;
            var HTML = item.Question.Name;
            holder.webView.LoadData(HTML, "text/html", Encoding.UTF8.EncodingName);
            holder.CorrectBtn.Text = item.Question.Answers.FirstOrDefault(c => c.IsCorrect)?.Name;
            holder.CorrectBtn.Visibility = item.Answer.IsCorrect ? ViewStates.Gone : ViewStates.Visible;
            holder.TextNumber.Text = $"Number {position+1}";
            if (item.Answer.IsCorrect)
            {
                ImageViewCompat.SetImageTintList(holder.ImageStatus, Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.MediumSeaGreen));
                holder.ImageStatus.SetImageResource(Resource.Drawable.ic_check_circle_outline);
            }
            else
            {
                ImageViewCompat.SetImageTintList(holder.ImageStatus, Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Red));
                holder.ImageStatus.SetImageResource(Resource.Drawable.ic_close_circle_outline);
            }
        }

        public override int ItemCount => items.Count;

        void OnClick(ResultAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(ResultAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class ResultAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView TextMyAnswer { get; set; }
        public TextView TextNumber { get; set; }
        public WebView webView { get; set; }
        public Button CorrectBtn { get; set; }
        public ImageView ImageStatus { get; private set; }
       

        public ResultAdapterViewHolder(View itemView, Action<ResultAdapterClickEventArgs> clickListener,
                            Action<ResultAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            TextNumber = itemView.FindViewById<TextView>(Resource.Id.question_count_number);
            TextMyAnswer = itemView.FindViewById<TextView>(Resource.Id.text_myanswer);
            webView = itemView.FindViewById<WebView>(Resource.Id.webView1);
            CorrectBtn = itemView.FindViewById<Button>(Resource.Id.text_correctanswer);
            ImageStatus = itemView.FindViewById<ImageView>(Resource.Id.imageView2);
            itemView.Click += (sender, e) => clickListener(new ResultAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new ResultAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class ResultAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}