using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using InService.App.Data;
using InService.App.Modules;
using InService.App.UI;

namespace InService.App.Courses
{
    [Activity(Label = "Courses")]
    public class AllCoursesActivity : AppCompatActivity
    {
        bool IsSyncing = false;
        MainMenuIconAdapter pAdapter;
        RecyclerView Recycler;
        bool SelectMode;
        LinearLayout TitlesLayout;
        TextView TextOne, TextTwo;
        Branch CurBranch;

        public const int AddCourseReqCode = 1001;
        public const int SelectCourseReqCode = 1002;
        int BranchID = 0;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.Elevation = 0;
            SetContentView(Resource.Layout.generic_list);
            Recycler = FindViewById<RecyclerView>(Resource.Id.gridView_items);
            TitlesLayout = FindViewById<LinearLayout>(Resource.Id.layout_title);
            TextOne = FindViewById<TextView>(Resource.Id.text_title1);
            TextTwo = FindViewById<TextView>(Resource.Id.text_title2);
            TitlesLayout.Visibility = ViewStates.Visible;
            TextOne.Visibility = ViewStates.Visible;
            TextTwo.Visibility = ViewStates.Visible;
            BranchID = Intent.GetIntExtra(nameof(BranchID), 0);
            CurBranch = await Branch.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == BranchID);
            if (CurBranch != null)
            {
                TextOne.Text = $"{CurBranch.Name}";
                TextTwo.Text = "Courses";
            }
            else
            {
                TextOne.Text = "AGRITEX";
                TextTwo.Text = "All courses";
            }
            //Recycler.SetLayoutManager(new LinearLayoutManager(this));
            Recycler.SetLayoutManager(new GridLayoutManager(this, 2));
            SelectMode = Intent.GetBooleanExtra(nameof(SelectMode), false);
            if (SelectMode) Title = "Select course..";
            LoadCourses();
        }
        protected override void OnResume()
        {
           // LoadCourses();
            base.OnResume();
        }
        //public override bool OnCreateOptionsMenu(IMenu menu)
        //{
        //    MenuInflater.Inflate(Resource.Menu.search, menu);
        //    ((Android.Support.V7.Widget.SearchView)menu.FindItem(Resource.Id.action_search).ActionView).QueryTextChange += (o, e) =>
        //    {
        //        pAdapter.Filter = e.NewText;
        //        e.Handled = true;
        //    };
        //    return true;
        //}

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    SetResult(Result.Canceled);
                    Finish();
                    OverridePendingTransition(Resource.Animation.side_out_right, Resource.Animation.side_in_left);
                    break;
                default:
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        async void LoadCourses()
        {
            List<Course> courses = null;
            if (CurBranch != null)
            {
                courses = await Course.DB.RowsAsync.Where(c => c.BranchID == BranchID).OrderBy(c => c.Name).ToListAsync();
            }
            else
            {
                courses = await Course.DB.RowsAsync.OrderBy(c => c.Name).ToListAsync();
            }
            var adapterItems = new List<MainMenuItemIcon>();
            foreach (var item in courses)
            {
                await item.LoadModules();
                await item.LoadAttachment();
                await item.LoadBranch();
                var iconItem = new MainMenuItemIcon { ID = item.ID, Title = item.Name, IsEnabled = true };
                var attachment = item.Attachment;
                if (attachment != null) iconItem.Base64String = attachment.Data;
                adapterItems.Add(iconItem);
            }

            pAdapter = new MainMenuIconAdapter(adapterItems.ToArray());
            pAdapter.ItemClick += (o, e) =>
            {
                var course = adapterItems[e.Position];
                if (SelectMode)
                {
                    var resIntent = new Intent();
                    resIntent.PutExtra("CourseID", course.ID);
                    SetResult(Result.Ok, resIntent);
                    Finish();
                    OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
                }
                else
                {
                    var intent = new Intent(this, typeof(ModulesActivity));
                    intent.PutExtra("CourseID", course.ID);
                    StartActivity(intent);
                    OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
                }
            };
            Recycler.SetAdapter(pAdapter);
            pAdapter.NotifyDataSetChanged();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (resultCode != Result.Ok) return;

            if (requestCode == AddCourseReqCode) LoadCourses();
            else if (requestCode == SelectCourseReqCode)
            {
                SetResult(Result.Ok, data);
                Finish();
                OverridePendingTransition(Resource.Animation.side_out_right, Resource.Animation.side_in_left);
            }
            base.OnActivityResult(requestCode, resultCode, data);
        }

        public override void OnBackPressed()
        {
            SetResult(Result.Canceled);
            Finish();
            OverridePendingTransition(Resource.Animation.side_out_right, Resource.Animation.side_in_left);
            base.OnBackPressed();
        }
    }
}