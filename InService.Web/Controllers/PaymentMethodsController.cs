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
    public class PaymentMethodsController : SysController
    {
        public ActionResult Index(int? p, string q)
        {
            var query = DB.PaymentMethods.AsQueryable();
            if (!String.IsNullOrEmpty(q)) query = query.Where(c => c.Name.Contains(q));
            ViewBag.Title = "Payment methods";
            return View(query.OrderByDescending(c => c.CreationDate).ToPagedList(p ?? 1, DefaultPageSize));
        }

        public ActionResult Details(int? id)
        {
            var paymentMethod = DB.PaymentMethods.Find(id);
            ViewBag.Title = $"Details: {paymentMethod.Name}";
            return View(paymentMethod);
        }

        public ActionResult Add()
        {
            ViewBag.Title = "Add a payment method";
            return View();
        }

        [ValidateAntiForgeryToken, HttpPost]
        public ActionResult Add(PaymentMethod paymentMethod)
        {
            paymentMethod.CreatorID = CurrentUser.ID;
            paymentMethod.CreationDate = DateTime.Now;
            DB.PaymentMethods.Add(paymentMethod);
            DB.SaveChanges();
            return RedirectToAction(nameof(Details), new { paymentMethod.ID });
        }

        public ActionResult Edit(int? id)
        {
            var paymentMethod = DB.PaymentMethods.Find(id);
            ViewBag.Title = $"Edit: {paymentMethod.Name}";
            return View(paymentMethod);
        }

        [ValidateAntiForgeryToken, HttpPost, ActionName(nameof(Edit))]
        public ActionResult Update(int id)
        {
            var paymentMethod = DB.PaymentMethods.Find(id);
            if (TryUpdateModel(paymentMethod, new string[] { nameof(PaymentMethod.Name), nameof(PaymentMethod.RequireReference) }))
            {
                DB.SaveChanges();
                return RedirectToAction(nameof(Details), new { paymentMethod.ID });
            }
            ViewBag.Title = $"Edit: {paymentMethod.Name}";
            return View(paymentMethod);
        }
    }
}
