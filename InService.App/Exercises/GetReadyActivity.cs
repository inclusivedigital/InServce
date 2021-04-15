using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using InService.App.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InService.App.Exercises
{
    [Activity(Label = "Ready")]
    public class GetReadyActivity : AppCompatActivity
    {
        Button BtnContinue, BtnBack;
        TextView TextCourse, TextModule;
        int ExaminationID = 0, ModuleID = 0, ArticleID = 0;
        Examination CurExam;
        Module CurModule;
        Article Curarticle;
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SupportActionBar.Elevation = 0;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SetContentView(Resource.Layout.get_ready);
            TextCourse = FindViewById<TextView>(Resource.Id.txt_course_name);
            TextModule = FindViewById<TextView>(Resource.Id.txt_module_name);
            BtnContinue = FindViewById<Button>(Resource.Id.btn_continue);
            BtnBack = FindViewById<Button>(Resource.Id.btn_back);
            ExaminationID = Intent.GetIntExtra("ExaminationID", 0);
            CurExam = await Examination.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == ExaminationID);
            ModuleID = Intent.GetIntExtra("ModuleID", 0);
            CurModule = await Module.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == ModuleID);
            await CurModule.LoadCourse();
            TextCourse.Text = $"{CurModule.Course.Name}";
            TextModule.Text = $"{CurModule.Name}";
            Curarticle = await Article.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == ArticleID);
            ArticleID = Intent.GetIntExtra("ArticleID", 0);
            BtnBack.Click += (o, e) =>
            {
                var intent = new Intent(this, typeof(Articles.ArticleActivity));
                intent.PutExtra("ArticleID", ArticleID);
                StartActivity(intent);
                Finish();
                OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
            };
            BtnContinue.Click += (o, e) =>
            {
                var intent = new Intent(this, typeof(StartExerciseActivity));
                intent.PutExtra("ExaminationID", CurExam.ID);
                intent.PutExtra("ModuleID", CurModule.ID);
                intent.PutExtra("ArticleID", ArticleID);
                StartActivity(intent);
                Finish();
                OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
            };
            // Create your application here
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    OverridePendingTransition(Resource.Animation.side_out_right, Resource.Animation.side_in_left);
                    break;
                default:
                    break;
            }
            return base.OnOptionsItemSelected(item);
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