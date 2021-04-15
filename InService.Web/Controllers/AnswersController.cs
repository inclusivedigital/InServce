using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InService.Web.Controllers
{
    public class AnswersController : SysController
    {
        // GET: Answers
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit(int id)
        {
            var answer = DB.Answers.Find(id);
            ViewBag.Title = "Edit answer";
            return View(answer);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Edit))]
        public ActionResult Update(int id)
        {
            var answer = DB.Answers.Find(id);
            if (TryUpdateModel(answer))
            {
                DB.SaveChanges();
                return RedirectToAction("Details", "Questions", new { answer.Question.ID });
            }
            ViewBag.Title = "Edit answer";
            return View(answer);
        }
    }
}