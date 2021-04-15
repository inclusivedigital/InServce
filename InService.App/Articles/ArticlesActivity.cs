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

namespace InService.App.Articles
{
    [Activity(Label = "Articles")]
    public class ArticlesActivity : AppCompatActivity
    {
        bool IsSyncing = false;
        ArticleAdapter pAdapter;
        RecyclerView Recycler;
        bool SelectMode;
        int ModuleID = 0;

        public const int AddCourseReqCode = 1001;
        public const int SelectCourseReqCode = 1002;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.Elevation = 0;
            SetContentView(Resource.Layout.generic_list);
            Recycler = FindViewById<RecyclerView>(Resource.Id.gridView_items);
            Recycler.SetLayoutManager(new LinearLayoutManager(this));
            SelectMode = Intent.GetBooleanExtra(nameof(SelectMode), false);
            ModuleID = Intent.GetIntExtra(nameof(ModuleID), 0);
            if (SelectMode) Title = "Select course..";
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.search, menu);
            ((Android.Support.V7.Widget.SearchView)menu.FindItem(Resource.Id.action_search).ActionView).QueryTextChange += (o, e) =>
            {
                pAdapter.Filter = e.NewText;
                e.Handled = true;
            };
            return true;
        }

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
        protected override void OnResume()
        {
            LoadArticles();
            base.OnResume();
        }

        async void LoadArticles()
        {
            List<Article> articles = null;
            if (ModuleID > 0)
            {
                var module = await Module.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == ModuleID);
                Title = $"{module.Name}";
                articles = await Article.DB.RowsAsync.Where(c => c.ModuleID == ModuleID).OrderBy(c => c.CreationDate).ToListAsync();
            }
            else
            {
                articles = await Article.DB.RowsAsync.OrderByDescending(c => c.CreationDate).ToListAsync();
            }
            foreach (var item in articles)
            {
                await item.LoadModule();
                await item.LoadCourse();
                await item.LoadValueChain();
            }

            pAdapter = new ArticleAdapter(articles);

            pAdapter.ItemClick += (o, e) =>
            {
                var course = pAdapter.Items[e.Position];
                if (SelectMode)
                {
                    var resIntent = new Intent();
                    resIntent.PutExtra("ArticleID", course.ID);
                    SetResult(Result.Ok, resIntent);
                    Finish();
                    OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
                }
                else
                {
                    var intent = new Intent(this, typeof(ArticleActivity));
                    intent.PutExtra("ArticleID", course.ID);
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

            if (requestCode == AddCourseReqCode) LoadArticles();
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