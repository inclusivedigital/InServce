using InService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace InService.Web.Controllers
{
    [Authorize]
    public class QuestionsController : SysController
    {
        [AllowAnonymous]
        public ActionResult Details(int id)
        {
            var question = DB.Questions.Find(id);
            ViewBag.Title = "Question details";
            return View(question);
        }

        public ActionResult Add(int id)
        {
            var ex = DB.Examinations.Find(id);
            ViewBag.number = ex.Questions.Count() + 1;
            ViewBag.ex = ex;
            ViewBag.Title = "Add question";
            return View();
        }

        [ValidateAntiForgeryToken, HttpPost, ValidateInput(false)]
        public ActionResult Add(Question question, List<string> Anss, List<int> Bits, int id)
        {
            var ex = DB.Examinations.Find(id);
            ViewBag.number = ex.Questions.Count() + 1;
            ViewBag.ex = ex;
            question.CreationDate = DateTime.UtcNow;
            question.CreatorID = CurrentUserID;
            if (ModelState.IsValid)
            {
                if (Anss != null)
                {
                    for (int i = 0; i < Anss.Count; i++)
                    {
                        question.Answers.Add(new Answer
                        {
                            Name = Anss[i],
                            FlagsID = Bits[i],
                            CreationDate = DateTime.UtcNow,
                            CreatorID = CurrentUserID

                        });
                    }
                }
                if (question.Name.Contains("<table"))
                {
                    question.Name = question.Name.Replace("<table", $"<table class=\"table table-stripped table-hover table-sm\"");
                }
                ex.Questions.Add(question);
                DB.SaveChanges();
                return RedirectToAction(nameof(Details), new { question.ID });
            }

            var list = new List<dynamic>();
            if (Anss != null)
            {
                foreach (var i in Anss)
                {
                    list.Add(new
                    {
                        Answer = i
                    });
                }
                ViewBag.list = new HtmlString(new JavaScriptSerializer().Serialize(list));
            }
            ViewBag.Title = "Add new question";
            return View(question);
        }

        public ActionResult Edit(int id)
        {
            var question = DB.Questions.Find(id);
            ViewBag.Title = "Edit question";
            var list = new List<dynamic>();
            if (question.Answers.Any())
            {
                foreach (var i in question.Answers)
                {
                    list.Add(new
                    {
                        Answer = i.Name
                    });
                }
                ViewBag.list = new HtmlString(new JavaScriptSerializer().Serialize(list));
            }
            return View(question);
        }

        [ValidateAntiForgeryToken, HttpPost, ActionName(nameof(Edit)), ValidateInput(false)]
        public ActionResult Update(int id, List<string> Anss, List<int> Bits)
        {
            var question = DB.Questions.Find(id);
            if (Anss != null)
            {
                if (question.Answers.Any()) DB.Answers.RemoveRange(question.Answers);
            }
            if (ModelState.IsValid)
            {
                if (TryUpdateModel(question))
                {
                    if (Anss != null)
                    {
                        for (int i = 0; i < Anss.Count; i++)
                        {
                            question.Answers.Add(new Answer
                            {
                                Name = Anss[i],
                                FlagsID = Bits[i],
                                CreationDate = DateTime.UtcNow,
                                CreatorID = CurrentUserID
                            });
                        }
                    }
                    DB.SaveChanges();
                    return RedirectToAction(nameof(Details), new { question.ID });
                }
            }
            var list = new List<dynamic>();
            if (Anss != null)
            {
                foreach (var i in Anss)
                {
                    list.Add(new
                    {
                        Answer = i
                    });
                }
                ViewBag.list = new HtmlString(new JavaScriptSerializer().Serialize(list));
            }
            ViewBag.Title = "Edit question";
            return View(question);
        }
    }
}