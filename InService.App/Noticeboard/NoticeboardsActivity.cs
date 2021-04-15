using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using InService.App.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InService.App.Noticeboard
{
    [Activity(Label = "Noticeboard")]
    public class NoticeboardsActivity : AppCompatActivity
    {
        RecyclerView RecyclerView;
        NoticeAdapter Adapter;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SupportActionBar.Elevation = 0;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SetContentView(Resource.Layout.generic_list);
            RecyclerView = FindViewById<RecyclerView>(Resource.Id.gridView_items);
            RecyclerView.SetLayoutManager(new LinearLayoutManager(this));
        }

        protected override async void OnResume()
        {
            base.OnResume();
            if (Adapter == null)
            {
                var exams = await Notice.DB.RowsAsync.OrderByDescending(c => c.CreationDate).ToListAsync();
                foreach (var item in exams)
                {
                }
                if (exams.Any())
                {
                    Adapter = new NoticeAdapter(exams);
                    Adapter.ItemClick += (o, e) =>
                    {
                        var exam = Adapter.Items[e.Position];
                        var intent = new Intent(this, typeof(NoticeActivity));
                        intent.PutExtra("NoticeID", exam.ID.ToString());
                        StartActivity(intent);
                        OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
                    };
                    RecyclerView.SetAdapter(Adapter);
                    Adapter.NotifyDataSetChanged();
                }
            }
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    SetResult(Result.Canceled);
                    StartActivity(typeof(MainActivity));
                    Finish();
                    OverridePendingTransition(Resource.Animation.side_out_right, Resource.Animation.side_in_left);
                    break;
                default:
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (resultCode != Result.Ok) return;
            SetResult(Result.Ok, data);
            StartActivity(typeof(MainActivity));
            Finish();
            OverridePendingTransition(Resource.Animation.side_out_right, Resource.Animation.side_in_left);
            base.OnActivityResult(requestCode, resultCode, data);
        }

        public override void OnBackPressed()
        {
            SetResult(Result.Canceled);
            StartActivity(typeof(MainActivity));
            Finish();
            OverridePendingTransition(Resource.Animation.side_out_right, Resource.Animation.side_in_left);
            base.OnBackPressed();
        }
    }
}