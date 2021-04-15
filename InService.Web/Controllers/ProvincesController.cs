
using InService.Data;
using InService.Lib.Auth;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InService.Web.Controllers
{
    //  [Authorize(Roles = nameof(UserRole.ADMINISTRATOR))]
    public class ProvincesController : SysController
    {
        public ActionResult Index(int? p, int? CountryID)
        {
            var query = DB.Provinces.AsQueryable();
            if (CountryID.HasValue) query = query.Where(c => c.CountryID == CountryID);
            ViewBag.Title = "Provinces";
            return View(query.OrderBy(c => c.Name).ToPagedList(p ?? 1, DefaultPageSize));
        }
        public ActionResult Details(int id)
        {
            var province = DB.Provinces.Find(id);
            ViewBag.Title = $"Province details :{province.Name}";
            return View(province);
        }

        public ActionResult Add(int CountryID)
        {
            var country = DB.Countries.Where(c => c.ID == CountryID).FirstOrDefault();
            ViewBag.Title = "Add a province";
            ViewBag.country = country;
            return View();
        }

        [ValidateAntiForgeryToken, HttpPost]
        public ActionResult Add(Province province, int CountryID)
        {
            var country = DB.Countries.FirstOrDefault(c => c.ID == CountryID);
            province.CountryID = CountryID;
            province.Code = GenerateCode();
            if (ModelState.IsValid)
            {
                DB.Provinces.Add(province);
                DB.SaveChanges();
                return RedirectToAction(nameof(Details), new { province.ID });
            }
            ViewBag.country = country;
            ViewBag.Title = "Add a province";
            return View(province);
        }

        public ActionResult Edit(int id)
        {
            var province = DB.Provinces.Find(id);
            ViewBag.Title = $"Edit province :{province.Name}";
            return View(province);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Edit))]
        public ActionResult Update(int id)
        {
            var province = DB.Provinces.Find(id);
            if (TryUpdateModel(province, new string[] { nameof(Province.Name) }))
            {
                if (province.Code == 0) province.Code = GenerateCode();
                DB.SaveChanges();
                return RedirectToAction(nameof(Details), new { province.ID });
            }
            ViewBag.Title = $"Edit province :{province.Name}";
            return View(province);
        }

        [AllowAnonymous]
        public JsonResult Search(string q)
        {
            var query = DB.Provinces.Where(c => c.Name.Contains(q));
            var provinces = new List<dynamic>();
            foreach (var item in query.OrderBy(c => c.Name))
            {
                provinces.Add(new { id = item.ID, text = item.Name });
            }
            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = provinces,
            };
        }

        public int GenerateCode()
        {
            return DB.Provinces.Count() + 1;
        }
    }
}

