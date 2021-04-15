using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using InService.App.Data;
using InService.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InService.App.Examinations
{
    [Activity(Label = "Instructions")]
    public class InstructionActivity : AppCompatActivity
    {
        Button GO;
        WebView WebView;
        string HTML = "No instructions found";
        int ExamTypeID = 0;
        int CourseID = 0;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SupportActionBar.Elevation = 0;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SetContentView(Resource.Layout.exam_instruction);
            ExamTypeID = Intent.GetIntExtra(nameof(ExamTypeID), 0);
            GO = FindViewById<Button>(Resource.Id.btn_floating_action);
            WebView = FindViewById<WebView>(Resource.Id.webView1);

            GO.Click += (o, e) =>
            {
                if (ExamTypeID == (int)ExaminationType.EXAMINATION)
                {
                    CourseID = Intent.GetIntExtra(nameof(CourseID), 0);
                    var intent = new Intent(this, typeof(ExaminationActivity));
                    intent.PutExtra(nameof(CourseID), CourseID);
                    StartActivity(intent);
                    OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
                    Finish();
                }
                else
                {

                }
            };
        }

        protected override void OnResume()
        {
            base.OnResume();
            LoadInstructions();
        }

        async void LoadInstructions()
        {
            var instruction = await Instruction.DB.RowsAsync.FirstOrDefaultAsync(c => c.ExamTypeID == ExamTypeID);
            if (instruction != null) HTML = instruction.Description;
            WebView.LoadData(HTML, "text/html", Encoding.UTF8.EncodingName);
        }
    }
}