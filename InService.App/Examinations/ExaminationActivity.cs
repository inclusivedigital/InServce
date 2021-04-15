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
using InService.App.Exercises;
using InService.App.UI;
using InService.Lib;
using InService.Lib.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InService.App.Examinations
{
    [Activity(Label = "Examination")]
    public class ExaminationActivity : AppCompatActivity
    {
        int CourseID = 0;
        Course CurCourse;
        RadioGroup group;
        Examination CurExamination;
        ImageButton NextBtn;
        Question Question;
        List<Question> Questions;
        RadioButton[] RadioButtons;
        WebView WebView;
        string HTML;
        int AnswerID = 0;
        UserExamination UserExamination;
        int Counter = 1;
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
            CourseID = Intent.GetIntExtra(nameof(CourseID), 0);
            CurCourse = await Data.Course.DB.RowsAsync.FirstOrDefaultAsync(c => c.ID == CourseID);
            await CurCourse.LoadExaminations();
            Questions = new List<Question>();
            foreach (var exam in CurCourse.Examinations)
            {
                await exam.LoadQuestions();
                Questions.AddRange(exam.Questions);
            }
            var guid = Guid.NewGuid();
            CurExamination = new Examination
            {
                LocalID = guid,
                CourseID = CourseID,
                CreationDate = DateTime.UtcNow,
                CreatorID = SessionManager.User.ID,
                Topic = CurCourse.Name,
                Year = DateTime.Now.Year,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow,
                TypeID = (int)ExaminationType.EXAMINATION,
                Duration = 500,
                PaperFormatID = (int)QuestionPaperFormat.ONE_QESTION_PER_PAGE,
                FlagsID = (int)ExaminationFlags.PREMIUM_EXAMINATION,
                TargetAudienceID = (int)ExaminationAudience.EXTENSION_OFFICERS,
                NumberOfQuestions = CurCourse.FinalExamQuestions ?? 25

            };

            UserExamination = new UserExamination { CreationDate = DateTime.Now, UserID = SessionManager.User.ID, StartTime = DateTime.Now };
            UserExamination.ID = guid;
            UserExamination.LocalExamID = CurExamination.LocalID;
            Questions = Questions.Shuffled().ToList().Take(CurCourse.FinalExamQuestions ?? 25).ToList().Shuffled().ToList();
            Count = Questions.Count;
            pb.Max = Count;
            if (Questions.Count > 0)
            {
                Question = Questions.FirstOrDefault();
            }
            Examination.DB.Insert(CurExamination);
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
            pb.Progress = Counter;// SetProgress(Counter, true);
        }

        protected override void OnResume()
        {
            base.OnResume();
            NextBtn.Click += async (o, e) =>
            {
                Counter++;
                tv.Text = $"Question {Counter} of {Count}";
                TextQuestionNumber.Text = $"Question number {Counter}";
                pb.Progress = Counter;//.SetProgress(Counter, true);
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
                    UserExamination.EndTime = DateTime.Now;
                    UserExamination.DB.Update(UserExamination);
                    var intent = new Intent(this, typeof(ResultsActivity));
                    intent.PutExtra("ExaminationID", UserExamination.ID.ToString());
                    StartActivity(intent);
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
            Toast.MakeText(this, "Finish your exercise", ToastLength.Long).Show();
            // base.OnBackPressed();
        }

    }
}