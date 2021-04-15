using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using InService.App.Data;
using System.Collections.Generic;
using System.Linq;
using InService.App.UI;

namespace InService.App.Modules
{
    class ModuleAdapter : RecyclerView.Adapter
    {
        public event EventHandler<ModuleAdapterClickEventArgs> ItemClick;
        public event EventHandler<ModuleAdapterClickEventArgs> ItemLongClick;
        public List<Module> Items { get; private set; }
        List<Module> itemsUnfiltered;

        public string Filter
        {
            set
            {
                if (string.IsNullOrWhiteSpace(value)) Items = itemsUnfiltered.ToList();
                else Items = itemsUnfiltered.Where(c => c.Name.ToLower().Contains(value.ToLower()) == true).ToList();
                NotifyDataSetChanged();
            }
        }

        public ModuleAdapter(List<Module> data)
        {
            Items = data;
            itemsUnfiltered = new List<Module>(data);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.list_item_module, parent, false);
            var vh = new ModuleAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        public  override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = Items[position];
            var holder = viewHolder as ModuleAdapterViewHolder;
            holder.TextName.Text = item.Name;
            holder.TextModulecode.Text = item.Code;
            holder.TextExaminations.Text = $"{item.Examinations.Count} examination{(item.Examinations.Count == 1 ? "" : "s")}";
            if (item.IconID != null)
            {
                holder.Image.SetImageBitmap(item.Attachment.Data.BaseStringToBitmap());
                //if (item.Thumbnail == null) await item.LoadThumbnail(viewHolder.ItemView.Context);
                //if (item.Thumbnail != null) holder.Image.SetImageBitmap(item.Thumbnail);
            }
        }
        public override int ItemCount => Items.Count;

        void OnClick(ModuleAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(ModuleAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);
    }

    public class ModuleAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView TextName { get; private set; }
        public TextView TextExaminations { get; private set; }
        public TextView TextModulecode { get; private set; }
        public ImageView Image { get; private set; }

        public ModuleAdapterViewHolder(View itemView, Action<ModuleAdapterClickEventArgs> clickListener,
                            Action<ModuleAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            TextName = itemView.FindViewById<TextView>(Resource.Id.text_item_name);
            TextExaminations = itemView.FindViewById<TextView>(Resource.Id.text_item_examinations);
            TextModulecode = itemView.FindViewById<TextView>(Resource.Id.text_item_modulecode);
            Image = itemView.FindViewById<ImageView>(Resource.Id.img_icon);
            itemView.Click += (sender, e) => clickListener(new ModuleAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new ModuleAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class ModuleAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}