using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using InService.App.Auth;
using InService.App.Data;
using InService.App.UI;
using InService.Lib;
using InService.Lib.Data;
using Newtonsoft.Json;

namespace InService.App.Exercises
{
    [Activity(Label = "Exercise")]
    public class StartExerciseActivity : AppCompatActivity
    {
        int ExaminationID = 0;
        RadioGroup group;
        Examination CurExamination;
        ImageButton NextBtn;
        // TextView TextQuestion;
        Question Question;
        List<Question> Questions;
        RadioButton[] RadioButtons;
        WebView WebView;
        string HTML;
        int AnswerID = 0;
        UserExamination UserExamination;
        int Counter = 1;
        //UpdatePB uptask;
        ProgressBar pb;
        TextView tv;
        int Count = 0;
        TextView TimerText, TextQuestionNumber;
        StartTimer StartTimer;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SupportActionBar.Elevation = 0;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SetContentView(Resource.Layout.exercise_question);
            WebView = FindViewById<WebView>(Resource.Id.webView1);
            group = FindViewById<RadioGroup>(Resource.Id.radio_group);
            NextBtn = FindViewById<ImageButton>(Resource.Id.btn_floating_action);
            pb = FindViewById<ProgressBar>(Resource.Id.pb);
            TimerText = FindViewById<TextView>(Resource.Id.text_timer);
            TextQuestionNumber = FindViewById<TextView>(Resource.Id.question_number);

            tv = FindViewById<TextView>(Resource.Id.tv);
            // TextQuestion = FindViewById<TextView>(Resource.Id.question);
            ExaminationID = Intent.GetIntExtra(nameof(ExaminationID), 0);
            CurExamination = await Data.Examination.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == ExaminationID);
            await CurExamination.LoadCourse();
            await CurExamination.LoadModule();
            //await CurExamination.LoadQuestions();
            var guid = Guid.NewGuid();
            UserExamination = new UserExamination { CreationDate = DateTime.Now, UserID = SessionManager.User.ID, StartTime = DateTime.Now, CourseID = CurExamination.CourseID.Value,ModuleID= CurExamination.ModuleID.Value};
            UserExamination.ID = guid;
            UserExamination.ExaminationID = CurExamination.ID;
            Questions = Question.DB.Rows.Where(c => c.ExaminationID == ExaminationID).ToList().Shuffled().ToList();// (List<Question>)CurExamination.Questions.Shuffled();
            Count = Questions.Count;
            pb.Max = Count;
            // uptask = new UpdatePB(this, pb, tv, Count);
            // uptask.Execute(100);
            if (Questions.Count > 0)
            {
                Question = Questions.FirstOrDefault();
                SessionManager.CanViewResults = false;
            }
            UserExamination.DB.Insert(UserExamination);
            var answers = Answer.DB.Rows.Where(c => c.QuestionID == Question.ID).ToList().Shuffled().ToList();
            StartExercise(Question, answers, group);
            StartTimer = new StartTimer(this, TimerText)
            {
                StartDate = DateTime.Now,
            };
            StartTimer.Start();
            tv.Text = $"Question {Counter} of {Count}";
            TextQuestionNumber.Text = $"Question number {Counter}";
            pb.Progress = Counter; // SetProgress(Counter, true);
        }

        protected override void OnResume()
        {
            base.OnResume();
            NextBtn.Click += async (o, e) =>
            {
                Counter++;
                tv.Text = $"Question {Counter} of {Count}";
                TextQuestionNumber.Text = $"Question number {Counter}";
                pb.Progress = Counter;// SetProgress(Counter, true);
                var detail = new UserExaminationDetail
                {
                    ID = Guid.NewGuid(),
                    AnswerID = AnswerID,
                    ExaminationID = UserExamination.ID,
                    QuestionID = Question.ID,
                };
                var attach = new IUserExaminationDetail { AnswerID = detail.AnswerID, QuestionID = detail.QuestionID };
                var list = UserExamination.Details;
                list.Add(attach);
                UserExamination.DetailsJson = JsonConvert.SerializeObject(list);
                UserExamination.DB.Update(UserExamination);
                UserExaminationDetail.DB.Insert(detail);
                if (Counter > Questions.Count)
                {
                    SessionManager.CanViewResults = true;
                    UserExamination.EndTime = DateTime.Now;
                    UserExamination.DB.Update(UserExamination);
                    var intent = new Intent(this, typeof(ResultsActivity));
                    intent.PutExtra("ExaminationID", UserExamination.ID.ToString());
                    StartActivity(intent);
                    Finish();
                    OverridePendingTransition(Resource.Animation.side_in_right, Resource.Animation.side_out_left);
                }
                else
                {
                    var newindex = Questions.ElementAt(Counter - 1);
                    if (newindex != null)
                    {
                        Question = newindex;
                        await Question.LoadAnswers();
                        var answers = Question.Answers.Shuffled().ToList();
                        StartExercise(Question, answers, group);
                    }
                }

            };
        }
        void StartExercise(Question question, List<Answer> answers, RadioGroup group)
        {
            HTML = question.Name;
            WebView.LoadData(HTML, "text/html", Encoding.UTF8.EncodingName);
            RadioButtons = new RadioButton[answers.Count];
            if (group.ChildCount > 0) group.RemoveAllViews();
            foreach (var answer in answers)
            {
                int index = answers.FindIndex(a => a.ID == answer.ID);
                RadioButtons[index] = new RadioButton(this) { Text = $"{answer.Name}", Tag = $"{answer.ID}" };
                RadioButtons[index].Gravity = GravityFlags.Center;
                group.AddView(RadioButtons[index]);
                RadioButtons[index].Click += (o, e) =>
                {
                    AnswerID = answer.ID;
                };
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Toast.MakeText(this, "Finish your exercise", ToastLength.Long).Show();
            return base.OnOptionsItemSelected(item);
        }

        public override void OnBackPressed()
        {
            if (!SessionManager.CanViewResults) Toast.MakeText(this, "Finish your exercise", ToastLength.Long).Show();
            else
            {

            }
            // base.OnBackPressed();
        }

    }
}