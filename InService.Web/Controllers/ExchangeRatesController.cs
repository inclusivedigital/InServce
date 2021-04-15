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
using InService.Lib;
using InService.Web.Auth;

namespace InService.Web.Controllers
{
    public class ExchangeRatesController : SysController
    {
        public ActionResult Index(int? p, DateTime? q)
        {
            var query = DB.ExchangeRates.AsQueryable();
            if (q.HasValue)
            {
                var end = Convert.ToDateTime(q.Value.ToString("yyy-MM-dd") + " " + DateTime.Now.ToString("HH:mm:ss"));
                query = query.Where(c => c.CreationDate <= end);
            }
            ViewBag.Title = "Exchange rates";
            ViewBag.q = q;
            return View(new PagedList.PagedList<ExchangeRate>(query.OrderByDescending(c => c.CreationDate), p ?? 1, DefaultPageSize));
        }

        // GET: BankAccounts/Details/5
        public ActionResult Details(int id)
        {
            var erate = DB.ExchangeRates.Find(id);
            ViewBag.Title = $"Rate details";
            return View(erate);
        }

        // GET: BankAccounts/Create
        public ActionResult Add()
        {
            ViewBag.Currency = new SelectList(DB.Currencies.OrderBy(c => c.Name), nameof(Currency.ID), nameof(Currency.Name));
            ViewBag.PaymentMethods = new SelectList(DB.PaymentMethods.OrderBy(c => c.Name), nameof(PaymentMethod.ID), nameof(PaymentMethod.Name));
            ViewBag.Title = "Add an exchange rate";
            return View();
        }

        [ValidateAntiForgeryToken, HttpPost]
        public ActionResult Add(int CurrencyID, decimal Rate, int? SiteID, int? WarehouseID, int? PaymentMethodID)
        {
            DB.ExchangeRates.Add(new ExchangeRate
            {
                CreationDate = DateTime.Now,
                CreatorID = User.Identity.GetUserId<int>(),
                Rate = 1 / Rate,
                CurrencyID = CurrencyID,
                PaymentMethodID = PaymentMethodID,
            });
            DB.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
