using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using System.Collections.Generic;
using InService.App.Data;

namespace InService.App.Exercises
{
    class MyExerciseAdapter : RecyclerView.Adapter
    {
        public event EventHandler<MyExerciseAdapterClickEventArgs> ItemClick;
        public event EventHandler<MyExerciseAdapterClickEventArgs> ItemLongClick;
       public List<UserExamination> Items;

        public MyExerciseAdapter(List<UserExamination> data)
        {
            Items = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            var itemView = LayoutInflater.From(parent.Context).
                   Inflate(Resource.Layout.list_item_myexercise, parent, false);

            var vh = new MyExerciseAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = Items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as MyExerciseAdapterViewHolder;
            holder.TextModuleName.Text = item.Examination?.Module?.Name??item.Examination.Course.Name;
            holder.TextModuleCode.Text = item.Examination?.Module?.Code??item.Examination.Course.Name;
            holder.TextDate.Text = $"{item.StartTime:dd MMM yyy HH:mm}";
        }

        public override int ItemCount => Items.Count;

        void OnClick(MyExerciseAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(MyExerciseAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class MyExerciseAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView TextModuleName { get; set; }
        public TextView TextModuleCode { get; set; }
        public TextView TextDate { get; set; }


        public MyExerciseAdapterViewHolder(View itemView, Action<MyExerciseAdapterClickEventArgs> clickListener,
                            Action<MyExerciseAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            TextModuleName = itemView.FindViewById<TextView>(Resource.Id.text_item_name);
            TextModuleCode = itemView.FindViewById<TextView>(Resource.Id.text_item_modulecode);
            TextDate = itemView.FindViewById<TextView>(Resource.Id.text_item_date);
            itemView.Click += (sender, e) => clickListener(new MyExerciseAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new MyExerciseAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }

    public class MyExerciseAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}