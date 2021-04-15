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
using PagedList;
using PagedList.Mvc;
using InService.Lib.Auth;

namespace InService.Web.Controllers
{
    public class CurrenciesController : SysController
    {
        public ActionResult Index(int? p, string q)
        {
            var query = DB.Currencies.AsQueryable();
            if (!String.IsNullOrEmpty(q)) query = query.Where(c => c.Name.Contains(q));
            ViewBag.Title = "Currencies";
            return View(query.OrderByDescending(c => c.CreationDate).ToPagedList(p ?? 1, DefaultPageSize));
        }

        public ActionResult Details(int id)
        {
            Currency currency = DB.Currencies.Find(id);
            ViewBag.Title = "Currency details";
            return View(currency);
        }

        public ActionResult Add()
        {
            ViewBag.Title = "Add currency";
            return View();
        }

        [ValidateAntiForgeryToken, HttpPost]
        public ActionResult Add(Currency currency)
        {
            currency.CreatorID = CurrentUser.ID;
            currency.CreationDate = DateTime.UtcNow;
            DB.Currencies.Add(currency);
            DB.SaveChanges();
            return RedirectToAction(nameof(Details), new { currency.ID });
        }

        public ActionResult Edit(int id)
        {
            Currency currency = DB.Currencies.Find(id);
            ViewBag.Title = "Edit currency";
            return View(currency);
        }

        [ValidateAntiForgeryToken, HttpPost, ActionName(nameof(Edit))]
        public ActionResult Update(int id)
        {
            var currency = DB.Currencies.Find(id);
            if (TryUpdateModel(currency, new string[] { nameof(Currency.Name), nameof(Currency.Symbol) }))
            {
                DB.SaveChanges();
                return RedirectToAction(nameof(Details), new { currency.ID });
            }
            ViewBag.Title = "Edit currency";
            return View(currency);
        }

        [AllowAnonymous]
        public JsonResult Search(string q)
        {
            var query = DB.Currencies.Where(c => c.Name.ToUpper().Contains(q.ToUpper()) || c.Symbol.ToUpper().Contains(q.ToUpper()));
            var curr = new List<dynamic>();
            foreach (var item in query.OrderBy(c => c.Name))
            {
                curr.Add(new { id = item.ID, text = item.Name });
            }
            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = curr,
            };
        }

        public ActionResult Delete(int id)
        {
            var currency = DB.Currencies.Find(id);
            if (!currency.Payments.Any()) DB.Currencies.Remove(currency); DB.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
