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
using InService.App.UI;

namespace InService.App.Examinations
{
    [Activity(Label = "Examination")]
    public class ExaminationsActivity : Activity
    {
        ExaminationAdapter Adapter;
        RecyclerView ExaminationView;
        ImageButton FAB;
        readonly ExaminationsDataSync DataSync = new ExaminationsDataSync();
        SwipeRefreshLayout SwipeRefreshLayout;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.generic_list);
            ActionBar.SetDisplayHomeAsUpEnabled(true);

            ExaminationView = FindViewById<RecyclerView>(Resource.Id.gridView_items);
            ExaminationView.SetLayoutManager(new LinearLayoutManager(this));
            ExaminationView.SetAdapter(Adapter);

            FAB = FindViewById<ImageButton>(Resource.Id.btn_floating_action);
            FAB.Visibility = ViewStates.Gone;
            SwipeRefreshLayout = FindViewById<SwipeRefreshLayout>(Resource.Id.swipe_refresh_layout);
            SwipeRefreshLayout.Refresh += (o, e) => SyncExamination();
            DataSync.DataSyncCompleted += (o, e) =>
            {
                SwipeRefreshLayout.Refreshing = false;
                LoadExamination(true);
            };
            DataSync.DataSyncFailed += (o, e) =>
            {
                this.ShowToast(e.Message);
                SwipeRefreshLayout.Refreshing = false;
            };
        }

        protected override void OnResume()
        {
            base.OnResume();
            LoadExamination();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.banking, menu);
            return true;
        }

        bool IsLoading;
        async void LoadExamination(bool reload = false)
        {
            if (IsLoading) return;
            if (reload || Adapter == null)
            {
                IsLoading = true;
                var examinations = await Examination.DB.RowsAsync.OrderByDescending(c => c.CreationDate).ToListAsync();
                foreach (var item in examinations)
                {
                    await item.LoadCourse();
                    await item.LoadModule();
                }

                Adapter = new ExaminationAdapter(examinations);
                Adapter.ItemClick += (o, e) =>
                {
                    var intent = new Intent(this, typeof(ExaminationDetailsActivity));
                    var examination = Adapter.Items[e.Position];
                    intent.PutExtra("ExaminationID", examination.ID);
                    StartActivityForResult(intent, 0);
                    OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
                };
                IsLoading = false;
                if (!reload && (examinations.Count == 0 || examinations.Any(d => d.ID == 0)) && Xamarin.Essentials.Connectivity.NetworkAccess.HasInternetAccess()) SyncExamination();
            }
            ExaminationView.SetAdapter(Adapter);
            Adapter.NotifyDataSetChanged();
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
                    return true;
                case Resource.Id.action_refresh:
                    SyncExamination();
                    break;
                default:
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (resultCode == Result.Ok) LoadExamination(true);
            base.OnActivityResult(requestCode, resultCode, data);
        }

        async void SyncExamination()
        {
            SwipeRefreshLayout.Refreshing = true;
            await DataSync.Sync();
        }
    }
}