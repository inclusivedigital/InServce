using InService.Lib;
using InService.Web.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdev.Payments;

namespace InService.Web.Controllers
{
    public class PaymentsController : SysController
    {
        static readonly Random Rnd = new Random();
        public ActionResult Index(int? p)
        {
            var query = DB.Payments.AsQueryable();
            ViewBag.Title = "Payments";
            return View(query.OrderByDescending(c => c.CreationDate).ToPagedList(p ?? 1, DefaultPageSize));
        }
        public ActionResult Details(int id)
        {
            var payment = DB.Payments.Find(id);
            ViewBag.Title = "Payment details";
            return View(payment);
        }

        public ActionResult Success(decimal? amount, string Product)
        {
            ViewBag.Title = "Success";
            var Today = DateTime.Now.Date;
            int count = DB.Payments.Count(c => c.CreationDate > Today);
            int rnd = Rnd.Next(Int16.MaxValue);
            var PollURL = TempData["PollURL"] as string;
            if (!string.IsNullOrWhiteSpace(PollURL))
            {
                var payment = new Data.Payment
                {
                    PollURL = PollURL,
                    Amount = amount ?? 0,
                    CreationDate = DateTime.UtcNow,
                    CreatorID = CurrentUserID,
                    StatusID = (int)PaymentStatus.PENDING_ACKNOWKEDGEMENT,
                    Reference = $"{Today:MMdd}{count:D3}{rnd:X4}",
                    CurrencyID = DB.Currencies.FirstOrDefault(c => c.IsDefault).ID,
                    PaymentMethodID = DB.PaymentMethods.FirstOrDefault(c => c.IsDefault).ID,
                };
                if (CurrentUser.FarmerID.HasValue) payment.FarmerID = CurrentUser.FarmerID;
                DB.Payments.Add(payment);
            }
            DB.SaveChanges();
            return View();
        }

        public ActionResult MakePayment(string Product, decimal? Amount)
        {
            string PollURL = "";
            var paynow = new Paynow("7802", "04ca7900-aff8-429b-8910-b983c237ed16")
            {
                ReturnUrl = $"{Request.BaseUrl()}/Payments/Success?amount={Amount}&Product={Product}",
                ResultUrl = $"{Request.BaseUrl()}"
            };
            var payment = paynow.CreatePayment($"Ref: {Product}");
            payment.Add(Product, Amount ?? 0);
            var response = paynow.Send(payment);
            if (response.Success())
            {
                var link = response.RedirectLink();
                var pollUrl = response.PollUrl();
                PollURL += pollUrl;
                TempData["PollURL"] = PollURL;
                if (!string.IsNullOrEmpty(link)) return new RedirectResult(link);

            }

            ViewBag.response = response;
            ViewBag.Title = "Pay now";
            return View();
        }




    }
}