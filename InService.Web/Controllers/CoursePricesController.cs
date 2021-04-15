using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using InService.Data;
using InService.Lib.Auth;
using InService.Web.Auth;

namespace InService.Web.Controllers
{
    public class CoursePricesController : SysController
    {
        public ActionResult Index(int? p, int? id, int? PID, int? CID, DateTime? sd, DateTime? ed)
        {
            var query = DB.CoursePrices.AsQueryable();
            if (id.HasValue)
            {
                var Course = DB.Courses.Find(id);
                ViewBag.Course = Course;
                query = query.Where(c => c.CourseID == Course.ID);
            }
            if (PID.HasValue)
            {
                var paymentmethod = DB.PaymentMethods.Find(PID);
                ViewBag.PaymentMethod = paymentmethod;
                query = query.Where(c => c.PaymentMethodID == paymentmethod.ID);
            }
            if (CID.HasValue)
            {
                var currency = DB.Currencies.Find(CID);
                ViewBag.Currency = currency;
                query = query.Where(c => c.CurrencyID == currency.ID);
            }
            if (sd.HasValue) query = query.Where(c => c.CreationDate >= sd);
            if (ed.HasValue)
            {
                var end = Convert.ToDateTime(ed.Value.ToString("yyy-MM-dd") + " " + DateTime.Now.ToString("HH:mm:ss"));
                query = query.Where(c => c.CreationDate <= end);
            }

            ViewBag.paymentmethds = DB.PaymentMethods.OrderBy(c => c.Name);
            ViewBag.Currencies = DB.Currencies.OrderBy(c => c.Name);

            ViewBag.id = id;
            ViewBag.PID = PID;
            ViewBag.CID = CID;
            ViewBag.sd = sd?.ToString("yyy-MM-dd");
            ViewBag.ed = ed?.ToString("yyy-MM-dd");
            ViewBag.Title = "Prices";
            return View(new PagedList.PagedList<CoursePrice>(query.OrderByDescending(c => c.CreationDate), p ?? 1, DefaultPageSize));
        }

        public ActionResult Details(int id)
        {
            CoursePrice priceList = DB.CoursePrices.Find(id);
            ViewBag.Title = "Price list";
            return View(priceList);
        }

        //Get
        public ActionResult Edit(int id)
        {
            CoursePrice priceList = DB.CoursePrices.Find(id);
            ViewBag.PaymentMethodID = new SelectList(DB.PaymentMethods, nameof(PaymentMethod.ID), nameof(PaymentMethod.Name), priceList.PaymentMethodID);
            ViewBag.CourseID = new SelectList(DB.Courses, nameof(Course.ID), nameof(Course.Name), priceList.CourseID);
            ViewBag.CurrencyID = new SelectList(DB.Currencies, nameof(Currency.ID), nameof(Currency.Name), priceList.CurrencyID);
            ViewBag.Title = "Edit Price list";
            return View(priceList);
        }

        [ValidateAntiForgeryToken, HttpPost]
        public ActionResult Edit(CoursePrice priceList)
        {
            int CreatorID = User.Identity.GetUserId<int>();
            priceList.CreatorID = CreatorID;
            priceList.CreationDate = DateTime.UtcNow;
            priceList.RevisionDate = DateTime.UtcNow;
            if (ModelState.IsValid)
            {
                DB.Entry(priceList).State = EntityState.Modified;
                DB.SaveChanges();
                return RedirectToAction(nameof(Details), new { priceList.ID });
            }
            ViewBag.PaymentMethodID = new SelectList(DB.PaymentMethods, nameof(PaymentMethod.ID), nameof(PaymentMethod.Name), priceList.PaymentMethodID);
            ViewBag.CourseID = new SelectList(DB.Courses, nameof(Course.ID), nameof(Course.Name), priceList.CourseID);
            ViewBag.CurrencyID = new SelectList(DB.Currencies, nameof(Currency.ID), nameof(Currency.Name), priceList.CurrencyID);
            ViewBag.Title = "Edit Price list";
            return View(priceList);
        }


        public ActionResult Add(int id)
        {
            var Course = DB.Courses.Find(id);
            ViewBag.Course = Course;
            if (!DB.PaymentMethods.Any()) return RedirectToAction("Add", "PaymentMethods");
            ViewBag.PaymentMethods = new SelectList(DB.PaymentMethods.OrderBy(c => c.Name), nameof(PaymentMethod.ID), nameof(PaymentMethod.Name));
            if (!DB.Currencies.Any()) return RedirectToAction("Add", "Currencies");
            ViewBag.Currencies = new SelectList(DB.Currencies.OrderBy(c => c.Name), nameof(Currency.ID), nameof(Currency.Name));
            ViewBag.Title = $"Add price: {Course.Name}";
            return View();
        }

        [ValidateAntiForgeryToken, HttpPost]
        public ActionResult Add(CoursePrice priceList)
        {
            int CreatorID = User.Identity.GetUserId<int>();
            var Course = DB.Courses.Find(priceList.CourseID);
            priceList.CreationDate = DateTime.UtcNow;
            priceList.RevisionDate = DateTime.UtcNow;
            priceList.CreatorID = CreatorID;
            Course.CoursePrices.Add(priceList);
            DB.SaveChanges();
            return RedirectToAction("Details", "Courses", new { Course.ID });
        }
    }
}

