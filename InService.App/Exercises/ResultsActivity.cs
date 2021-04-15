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

namespace InService.App.Exercises
{
    [Activity(Label = "Results")]
    public class ResultsActivity : AppCompatActivity
    {
        TextView TextExercise, TextTotalScore, TextOutOf;
        Guid ExaminationID;
        UserExamination UserExamination;
        RecyclerView RecyclerView;
        ResultAdapter Adapter;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SupportActionBar.Elevation = 0;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SetContentView(Resource.Layout.exercise_result);
            TextExercise = FindViewById<TextView>(Resource.Id.text_exercise_name);
            TextTotalScore = FindViewById<TextView>(Resource.Id.text_total_score);
            TextOutOf = FindViewById<TextView>(Resource.Id.text_out_of);
            ExaminationID = Guid.Parse(Intent.GetStringExtra(nameof(ExaminationID)));
            RecyclerView = FindViewById<RecyclerView>(Resource.Id.gridView_items);
            RecyclerView.SetLayoutManager(new LinearLayoutManager(this));
            UserExamination = UserExamination.DB.Rows.FirstOrDefault(c => c.ID == ExaminationID);
        }

        protected override async void OnResume()
        {
            base.OnResume();
            if (UserExamination != null)
            {
                await UserExamination.LoadExamination();
                await UserExamination.LoadUserExaminationDetails();
                var details = UserExamination.UserExaminationDetails;
                foreach (var item in details)
                {
                    await item.LoadQuestion();
                    var q = item.Question;
                    await q.LoadAnswers();
                    await item.LoadAnswer();
                }
                var correctanswers = details.Where(c => c.AnswerID != null && c.Answer.IsCorrect);
                var wronganswers = details.Where(c => c.AnswerID != null && !c.Answer.IsCorrect);
                var examination = UserExamination.Examination;
                await examination.LoadModule();
                var module = examination.Module;
                TextExercise.Text = UserExamination.Examination.Topic ?? module.Name;
                TextTotalScore.Text = $"{correctanswers.Count()} mark{(correctanswers.Count() == 1 ? "" : "s")}";
                TextOutOf.Text = $"{(details.Count)}";
                Adapter = new ResultAdapter(details);
                RecyclerView.SetAdapter(Adapter);
                Adapter.NotifyDataSetChanged();
            }
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    SetResult(Result.Canceled);
                    StartActivity(typeof(MyExercisesActivity));
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
            StartActivity(typeof(MyExercisesActivity));
            Finish();
            OverridePendingTransition(Resource.Animation.side_out_right, Resource.Animation.side_in_left);
            base.OnActivityResult(requestCode, resultCode, data);
        }

        public override void OnBackPressed()
        {
            SetResult(Result.Canceled);
            StartActivity(typeof(MyExercisesActivity));
            Finish();
            OverridePendingTransition(Resource.Animation.side_out_right, Resource.Animation.side_in_left);
            base.OnBackPressed();
        }
    }
}