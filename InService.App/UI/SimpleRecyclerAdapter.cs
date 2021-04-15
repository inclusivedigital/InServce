using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using System.Collections.Generic;
using System.Linq;
using InService.App.Data;
using InService.App;

namespace Inservice.App.UI
{
    class SimpleRecyclerAdapter<T> : RecyclerView.Adapter
    {
        public event EventHandler<SimpleRecyclerAdapterClickEventArgs> ItemClick;
        public event EventHandler<SimpleRecyclerAdapterClickEventArgs> ItemLongClick;
        public List<T> Items { get; private set; }
        List<T> itemsUnfiltered;

        public string Filter
        {
            set
            {
                if (string.IsNullOrWhiteSpace(value)) Items = itemsUnfiltered.ToList();
                else Items = itemsUnfiltered.Where(c => c.ToString().ToLower().Contains(value.ToLower())).ToList();
                NotifyDataSetChanged();
            }
        }

        public SimpleRecyclerAdapter(List<T> data)
        {
            Items = data;
            itemsUnfiltered = new List<T>(data);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = null;
            if (typeof(IKeyValuePair).IsAssignableFrom(typeof(T)))
                itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.list_item_key_value, parent, false);
            else itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.list_item_simple, parent, false);

            var vh = new SimpleRecyclerAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = Items[position];
            var holder = viewHolder as SimpleRecyclerAdapterViewHolder;
            if (item is IKeyValuePair)
            {
                holder.TextKey.Text = ((IKeyValuePair)item).Key?.Trim();
                holder.TextValue.Text = ((IKeyValuePair)item).Value?.Trim();
            }
            else
            {
                holder.TextView.Text = item.ToString();
            }
        }

        public override int ItemCount => Items.Count;

        void OnClick(SimpleRecyclerAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(SimpleRecyclerAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class SimpleRecyclerAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView TextView { get; private set; }
        public TextView TextKey { get; private set; }
        public TextView TextValue { get; private set; }

        public SimpleRecyclerAdapterViewHolder(View itemView, Action<SimpleRecyclerAdapterClickEventArgs> clickListener,
                            Action<SimpleRecyclerAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            TextView = itemView.FindViewById<TextView>(Resource.Id.list_item_text);
            TextKey = itemView.FindViewById<TextView>(Resource.Id.list_item_key);
            TextValue = itemView.FindViewById<TextView>(Resource.Id.list_item_value);
            itemView.Click += (sender, e) => clickListener(new SimpleRecyclerAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new SimpleRecyclerAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class SimpleRecyclerAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}