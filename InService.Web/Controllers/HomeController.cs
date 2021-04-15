using InService.Data;
using InService.Lib.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InService.Web.Controllers
{
    [AllowAnonymous]
    public class HomeController : SysController
    {
        [Authorize]
        public ActionResult Index()
        {
            ViewBag.farmer = DB.Farmers;
            ViewBag.courses = DB.Courses;
            ViewBag.examinations = DB.Examinations;
            ViewBag.officers = DB.ExtensionOfficers;
            if (User.IsInRole(nameof(UserRole.FARMER))) return RedirectToAction("Details", "Farmers", new { CurrentUser.Farmer.ID });
            return View();
        }

        [AllowAnonymous]
        public ActionResult Article(int? ModuleID, int? DefaultID)
        {
            Article article = null;
            if (ModuleID.HasValue)
            {
                var module = DB.Modules.Find(ModuleID);
                article = DB.Articles.ToList().Where(c => c.Flags.HasFlag(Lib.ArticleFlags.PUBLISHED)).FirstOrDefault(c => c.ModuleID == module.ID);
                ViewBag.article = article;
                ViewBag.Title = $"{module.Name}";
            }
            else if (DefaultID.HasValue)
            {
                article = DB.Articles.ToList().Where(c => c.Flags.HasFlag(Lib.ArticleFlags.PUBLISHED) && c.IsDefault == true).FirstOrDefault();
                ViewBag.article = article;
                if (article != null) ViewBag.Title = $"{article.Module?.Name}";
                ViewBag.Title = "Article";
            }
            else
            {
                article = DB.Articles.ToList().Where(c => c.Flags.HasFlag(Lib.ArticleFlags.PUBLISHED) && c.IsDefault == true).FirstOrDefault();
                ViewBag.article = article;
                if (article != null) ViewBag.Title = $"{article.Module?.Name}";
                ViewBag.Title = "Article";
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult Course(int? CourseID)
        {
            var course = DB.Courses.Find(CourseID);
            ViewBag.course = course;
            ViewBag.Title = $"{course.Name}";
            return View(course);
        }

        public ActionResult Branch(int? BranchID)
        {
            var branch = DB.Branches.Find(BranchID);
            ViewBag.branch = branch;
            ViewBag.Title = $"{branch.Name}";
            return View(branch);
        }

        public ActionResult Courses()
        {
            var courses = DB.Courses.AsQueryable();
            ViewBag.Title = "Courses";
            return View(courses.OrderBy(c => c.Name));
        }

        public ActionResult ValueChains()
        {
            var values = DB.ValueChains.AsQueryable();
            ViewBag.Title = "Value chains";
            return View(values.OrderBy(c => c.Name));
        }

    }
}