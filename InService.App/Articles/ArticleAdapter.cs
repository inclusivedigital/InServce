using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using InService.App.Data;
using System.Collections.Generic;
using System.Linq;

namespace InService.App.Articles
{
    class ArticleAdapter : RecyclerView.Adapter
    {
        public event EventHandler<ArticleAdapterClickEventArgs> ItemClick;
        public event EventHandler<ArticleAdapterClickEventArgs> ItemLongClick;
        public List<Article> Items { get; private set; }
        List<Article> itemsUnfiltered;

        public string Filter
        {
            set
            {
                if (string.IsNullOrWhiteSpace(value)) Items = itemsUnfiltered.ToList();
                else Items = itemsUnfiltered.Where(c => c.Name.ToLower().Contains(value.ToLower()) == true).ToList();
                NotifyDataSetChanged();
            }
        }

        public ArticleAdapter(List<Article> data)
        {
            Items = data;
            itemsUnfiltered = new List<Article>(data);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.list_item_article, parent, false);
            var vh = new ArticleAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = Items[position];
            var holder = viewHolder as ArticleAdapterViewHolder;
            holder.TextName.Text = item.Name ?? item.Module.Name;
        }
        public override int ItemCount => Items.Count;

        void OnClick(ArticleAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(ArticleAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);
    }

    public class ArticleAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView TextName { get; private set; }

        public ArticleAdapterViewHolder(View itemView, Action<ArticleAdapterClickEventArgs> clickListener,
                            Action<ArticleAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            TextName = itemView.FindViewById<TextView>(Resource.Id.text_item_name);
            itemView.Click += (sender, e) => clickListener(new ArticleAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new ArticleAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class ArticleAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}