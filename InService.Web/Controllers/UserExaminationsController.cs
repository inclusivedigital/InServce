using InService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InService.Lib;
using System.Dynamic;
using System.Web.Script.Serialization;

namespace InService.Web.Controllers
{
    public class UserExaminationsController : SysController
    {
        // GET: UserExaminations
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Attempt(int id)
        {
            var ex = DB.Examinations.Find(id);
            var today = DateTime.Now;
            if (ex.EndDate < today) ViewBag.expired = "expired";
            if (!ex.IsInProgress) return RedirectToAction("Details", "Instructions", new { ex.ID });
            if (DB.UserExaminations.Any(c => c.UserID == CurrentUserID && c.ExaminationID == ex.ID))
            {
                if (ex.EndDate > DateTime.Now)
                {
                    var ste = ex.UserExaminations.FirstOrDefault(c => c.UserID == CurrentUserID);
                    return RedirectToAction(nameof(Results), new { ste.ID });
                }
                ViewBag.done = "done";
            }

            DB.SaveChanges();

            ViewBag.farmer = CurrentUser;
            ViewBag.now = DateTime.Now;
            ViewBag.Title = $"attempt";
            var population = ex.Questions.Count() > ex.NumberOfQuestions ? ex.NumberOfQuestions : ex.Questions.Count();
            ViewBag.population = population;
            var list = new List<dynamic>();
            foreach (var item in ex.Questions.ToList().Shuffled().Take(population))
            {
                dynamic obj = new ExpandoObject();
                var answers = new List<Answer>();
                foreach (var x in item.Answers.ToList().Shuffled()) answers.Add(x);
                list.Add(new
                {
                    q = item.Name,
                    score = item.Score,
                    options = answers.Select(c => c.Name).ToList(),
                    correctIndex = answers.IndexOf(answers.FirstOrDefault(c => c.IsCorrect)),
                    correctResponse = "Correct",
                    incorrectResponse = "Incorrect",
                });
            }
            ViewBag.array = new HtmlString(new JavaScriptSerializer().Serialize(list));
            return View(ex);
        }

        [ValidateAntiForgeryToken, HttpPost, AllowAnonymous]
        public ActionResult Attempt(int? p, List<int> QID, List<string> AID, int eid, DateTime now)
        {
            var farmer = CurrentUser;
            var ex = DB.Examinations.Find(eid);
            ViewBag.farmer = farmer;
            ViewBag.Title = $"attempt";
            ViewBag.now = DateTime.Now;

            var today = DateTime.Now;
            if (ex.EndDate < today)
            {
                ViewBag.expired = "expired";
                return View(ex);
            }

            UserExamination exercise = new UserExamination
            {
                UserID = CurrentUserID,
                ExaminationID = ex.ID,
                CreationDate = DateTime.Now,
                StartTime = now,
                ID = Guid.NewGuid(),
            };
            var list = new List<QuestionAnswer>();
            if (AID != null)
            {
                foreach (var item in AID)
                {
                    var q = int.Parse(item.Split('_')[0]);
                    var a = int.Parse(item.Split('_')[1]);
                    list.Add(new QuestionAnswer { QuestionID = q, AnswerID = a });
                }
            }
            for (int i = 0; i < QID.Count; i++)
            {
                var d = QID[i];
                var question = DB.Questions.Find(d);
                var answers = list.Where(c => c.QuestionID == question.ID);
                if (answers.Any())
                {
                    foreach (var item in answers)
                    {
                        exercise.UserExaminationDetails.Add(new UserExaminationDetail
                        {
                            QuestionID = item.QuestionID,
                            AnswerID = item.AnswerID,
                            ID = Guid.NewGuid(),
                        });
                    }
                }
                //if (AID != null)
                //{
                //    var QA = AID.ElementAtOrDefault(i);
                //    if (!string.IsNullOrEmpty(QA))
                //    {
                //        int q = int.Parse(QA.Split('_')[0]);
                //        int a = int.Parse(QA.Split('_')[1]);
                //        if (question.ID == q)
                //            exercise.UserExaminationDetails.Add(new UserExaminationDetail
                //            {
                //                QuestionID = q,
                //                AnswerID = a,
                //            });
                //        else exercise.UserExaminationDetails.Add(new UserExaminationDetail { QuestionID = QID[i] });
                //    }
                //    else exercise.UserExaminationDetails.Add(new UserExaminationDetail { QuestionID = QID[i] });
                //}
                else exercise.UserExaminationDetails.Add(new UserExaminationDetail { QuestionID = QID[i], ID = Guid.NewGuid() });
            }
            DB.UserExaminations.Add(exercise);
            DB.SaveChanges();
            return RedirectToAction(nameof(Results), new { exercise.ID });
        }

        [AllowAnonymous]
        public ActionResult Results(Guid id)
        {
            var exercise = DB.UserExaminations.Find(id);
            ViewBag.Title = "Results";
            return View(exercise);
        }

        public class QuestionAnswer
        {
            public int QuestionID { get; set; }
            public int AnswerID { get; set; }
        }
    }
}