using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using InService.App.Data;

namespace InService.App.Examinations
{
    [Activity(Label = "Examination details")]
    public class ExaminationDetailsActivity : Activity
    {
        TextView TextTopic, TextCourseName, TextModuleName, TextDuration, TextEndDate;
        View ViewDuration;
        int CurExaminationID = 0;
        Examination CurExamination;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.examination_details);
            ActionBar.SetDisplayHomeAsUpEnabled(true);

            TextTopic = FindViewById<TextView>(Resource.Id.text_topic);
            TextCourseName = FindViewById<TextView>(Resource.Id.text_coursename);
            TextModuleName = FindViewById<TextView>(Resource.Id.text_module);
            TextDuration = FindViewById<TextView>(Resource.Id.text_date);
            TextEndDate = FindViewById<TextView>(Resource.Id.text_date);
            ViewDuration = FindViewById(Resource.Id.view_duration);
            CurExaminationID = Intent.GetIntExtra("ExaminationID", 0);
        }

        protected override void OnResume()
        {
            base.OnResume();
            LoadExamination();
        }

        async void LoadExamination()
        {
            if (CurExamination == null)
            {
                CurExamination = await Examination.DB.RowsAsync.Where(d => d.ID == CurExaminationID).FirstOrDefaultAsync();
            }
            await CurExamination.LoadCourse(); await CurExamination.LoadModule();
            TextTopic.Text = CurExamination.Topic;
            TextCourseName.Text = $"{CurExamination.Course.Name} {CurExamination.Course.Code}";
            TextDuration.Text = CurExamination.DurationString;
            TextModuleName.Text = CurExamination.Module.Name;
            TextEndDate.Text = CurExamination.EndDate.ToString("dd MMM yyy");
            if (string.IsNullOrWhiteSpace(CurExamination.DurationString)) ViewDuration.Visibility = ViewStates.Gone;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
                    return true;
                default:
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}