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
    public class ExaminationPricesController : SysController
    {
        public ActionResult Index(int? p, int? id, int? PID, int? CID, DateTime? sd, DateTime? ed)
        {
            var query = DB.ExaminationPrices.AsQueryable();
            if (id.HasValue)
            {
                var Examination = DB.Examinations.Find(id);
                ViewBag.Examination = Examination;
                query = query.Where(c => c.ExaminationID == Examination.ID);
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
            return View(new PagedList.PagedList<ExaminationPrice>(query.OrderByDescending(c => c.CreationDate), p ?? 1, DefaultPageSize));
        }

        public ActionResult Details(int id)
        {
            ExaminationPrice priceList = DB.ExaminationPrices.Find(id);
            ViewBag.Title = "Price list";
            return View(priceList);
        }

        //Get
        public ActionResult Edit(int id)
        {
            ExaminationPrice priceList = DB.ExaminationPrices.Find(id);
            ViewBag.PaymentMethodID = new SelectList(DB.PaymentMethods, nameof(PaymentMethod.ID), nameof(PaymentMethod.Name), priceList.PaymentMethodID);
            ViewBag.ExaminationID = new SelectList(DB.Examinations, nameof(Examination.ID), nameof(Examination.Topic), priceList.ExaminationID);
            ViewBag.CurrencyID = new SelectList(DB.Currencies, nameof(Currency.ID), nameof(Currency.Name), priceList.CurrencyID);
            ViewBag.Title = "Edit Price list";
            return View(priceList);
        }

        [ValidateAntiForgeryToken, HttpPost]
        public ActionResult Edit(ExaminationPrice priceList)
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
            ViewBag.ExaminationID = new SelectList(DB.Examinations, nameof(Examination.ID), nameof(Examination.Topic), priceList.ExaminationID);
            ViewBag.CurrencyID = new SelectList(DB.Currencies, nameof(Currency.ID), nameof(Currency.Name), priceList.CurrencyID);
            ViewBag.Title = "Edit Price list";
            return View(priceList);
        }


        public ActionResult Add(int id)
        {
            var Examination = DB.Examinations.Find(id);
            ViewBag.Examination = Examination;
            if (!DB.PaymentMethods.Any()) return RedirectToAction("Add", "PaymentMethods");
            ViewBag.PaymentMethods = new SelectList(DB.PaymentMethods.OrderBy(c => c.Name), nameof(PaymentMethod.ID), nameof(PaymentMethod.Name));
            if (!DB.Currencies.Any()) return RedirectToAction("Add", "Currencies");
            ViewBag.Currencies = new SelectList(DB.Currencies.OrderBy(c => c.Name), nameof(Currency.ID), nameof(Currency.Name));
            ViewBag.Title = $"Add price: {Examination.Topic}";
            return View();
        }

        [ValidateAntiForgeryToken, HttpPost]
        public ActionResult Add(ExaminationPrice priceList)
        {
            int CreatorID = User.Identity.GetUserId<int>();
            var Examination = DB.Examinations.Find(priceList.ExaminationID);
            priceList.CreationDate = DateTime.UtcNow;
            priceList.RevisionDate = DateTime.UtcNow;
            priceList.CreatorID = CreatorID;
            Examination.ExaminationPrices.Add(priceList);
            DB.SaveChanges();
            return RedirectToAction("Details", "Examinations", new { Examination.ID });
        }
    }
}

