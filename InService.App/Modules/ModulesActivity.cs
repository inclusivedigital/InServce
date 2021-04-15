using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using InService.App.Articles;
using InService.App.Data;
using InService.App.Examinations;
using InService.Lib;

namespace InService.App.Modules
{
    [Activity(Label = "Modules")]
    public class ModulesActivity : AppCompatActivity
    {
        ModuleAdapter pAdapter;
        RecyclerView Recycler;
        bool SelectMode;
        int CourseID = 0;
        LinearLayout TitlesLayout;
        TextView TextOne, TextTwo;
        Button ExamBtn;
        public const int AddCourseReqCode = 1001;
        public const int SelectCourseReqCode = 1002;
        Course CurCourse;
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.Elevation = 0;
            SetContentView(Resource.Layout.generic_list);
            Recycler = FindViewById<RecyclerView>(Resource.Id.gridView_items);
            Recycler.SetLayoutManager(new LinearLayoutManager(this));
            SelectMode = Intent.GetBooleanExtra(nameof(SelectMode), false);
            CourseID = Intent.GetIntExtra(nameof(CourseID), 1);
            CurCourse = await Course.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == CourseID);
            TitlesLayout = FindViewById<LinearLayout>(Resource.Id.layout_title);
            TextOne = FindViewById<TextView>(Resource.Id.text_title1);
            TextTwo = FindViewById<TextView>(Resource.Id.text_title2);
            ExamBtn = FindViewById<Button>(Resource.Id.btn_floating_action2);
            ExamBtn.Visibility = ViewStates.Visible;
            ExamBtn.Click += async (o, e) =>
             {
                 await CurCourse.LoadUserExaminations();
                 await CurCourse.LoadAllExercises();
                 var allexercises = CurCourse.AllExercises;
                 var exercises = CurCourse.UserExaminations;
                 var passes = new List<UserExamination>();
                 foreach (var item in exercises)
                 {
                     await item.LoadUserExaminationDetails();
                     var details = item.UserExaminationDetails;
                     foreach (var item2 in details)
                     {
                         await item2.LoadQuestion();
                         var q = item2.Question;
                         await q.LoadAnswers();
                         await item2.LoadAnswer();
                     }
                     var correctanswers = details.Where(c => c.AnswerID != null && c.Answer.IsCorrect);
                     var wronganswers = details.Where(c => c.AnswerID != null && !c.Answer.IsCorrect);
                     var percentage = 100 * correctanswers.Count() / details.Count;
                     if (percentage >= 70) passes.Add(item);
                 }
                 var overalpercentage = allexercises.Count > 0 ? 100 * passes.Count / allexercises.Count : 0;
                 var requiredExercises = allexercises.Count * 70 / 100;
                 if (overalpercentage >= 70)
                 {
                     var intent = new Intent(this, typeof(InstructionActivity));
                     intent.PutExtra(nameof(CourseID), CourseID);
                     intent.PutExtra("ExamTypeID", (int)ExaminationType.EXAMINATION);
                     StartActivity(intent);
                     OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
                     Finish();
                 }
                 else
                 {
                     Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
                     Android.App.AlertDialog alert = dialog.Create();
                     alert.SetTitle("Information");
                     alert.SetCanceledOnTouchOutside(false);
                     alert.SetMessage($"Number of attempted exercises: {exercises.Count}.\nPassed exercises: {passes.Count}\nOveral Pass rate: {overalpercentage}%\nYou need to pass at least {requiredExercises} short exercises to qualify for the final exam");
                     alert.SetIcon(Resource.Drawable.ic_account_key);
                     alert.SetButton("OK", (c, ev) =>
                     {
                         alert.Dismiss();
                     });
                     alert.Show();
                 }
             };
            TitlesLayout.Visibility = ViewStates.Visible;
            TextOne.Visibility = ViewStates.Visible;
            TextTwo.Visibility = ViewStates.Visible;
            if (SelectMode) Title = "Select module..";
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
        protected override void OnResume()
        {
            LoadModules();
            base.OnResume();
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

        async void LoadModules()
        {
            List<Data.Module> modules = null;
            if (CourseID > 0)
            {
                modules = await Data.Module.DB.RowsAsync.Where(c => c.CourseID == CourseID).OrderBy(c => c.Number).ToListAsync();
                TextOne.Text = $"{CurCourse.Name}";
                TextTwo.Text = "Modules";
                Title = $"Course: {CurCourse.Name}";
            }
            else modules = await Data.Module.DB.RowsAsync.OrderBy(c => c.Number).ToListAsync();
            foreach (var item in modules)
            {
                await item.LoadAttachment();
                await item.LoadCourse();
                await item.LoadExaminations();
                await item.LoadArticles();
            }
            pAdapter = new ModuleAdapter(modules);

            pAdapter.ItemClick += (o, e) =>
            {
                var module = pAdapter.Items[e.Position];
                if (SelectMode)
                {
                    var resIntent = new Intent();
                    resIntent.PutExtra("ModuleID", module.ID);
                    SetResult(Result.Ok, resIntent);
                    Finish();
                    OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
                }
                else
                {
                    if (module.Articles.Count == 1)
                    {
                        var article = module.Articles.FirstOrDefault();
                        if (article != null)
                        {
                            var intent = new Intent(this, typeof(ArticleActivity));
                            intent.PutExtra("ArticleID", article.ID);
                            StartActivity(intent);
                            OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
                        }
                    }
                    else
                    {
                        var intent = new Intent(this, typeof(ArticlesActivity));
                        intent.PutExtra("ModuleID", module.ID);
                        StartActivity(intent);
                        OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
                    }
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

            if (requestCode == AddCourseReqCode) LoadModules();
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