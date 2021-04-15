using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InService.Data;
using InService.Lib;
using InService.Lib.Auth;
using PagedList;
using PagedList.Mvc;
namespace InService.Web.Controllers
{
    public class CountriesController : SysController
    {
        public ActionResult Index(int? p, string q)
        {
            var query = DB.Countries.AsQueryable();
            query = query.Where(c => c.Name.Contains(q) || q == null);
            ViewBag.Title = "Countries";
            return View(query.OrderBy(c => c.Name).ToPagedList(p ?? 1, DefaultPageSize));
        }

        public ActionResult Details(int id)
        {
            var country = DB.Countries.FirstOrDefault(c => c.ID == id);
            ViewBag.Title = $"Country: {country.Name}";
            return View(country);
        }

        public ActionResult Add(int? id)
        {
            if (id.HasValue)
            {
                var region = DB.Regions.Find(id);
                ViewBag.region = region;
            }

            ViewBag.Title = "Add a country";
            ViewBag.regions = new SelectList(DB.Regions, nameof(Region.ID), nameof(Region.Name));
            return View();
        }

        [ValidateAntiForgeryToken, HttpPost]
        public ActionResult Add(Country country, int? id, string Symbol)
        {
            if (DB.Countries.Any(c => c.ID == country.ID)) ModelState.AddModelError(nameof(country.ID), "The symbol has already been used.");
            if (id.HasValue)
            {
                var region = DB.Regions.Find(id);
                ViewBag.region = region;
                country.RegionID = region.ID;
            }
            if (ModelState.IsValid)
            {
                DB.Countries.Add(country);
                DB.SaveChanges();
                return RedirectToAction(nameof(Details), new { country.ID });
            }
            ViewBag.Title = "Add a country";
            ViewBag.regions = new SelectList(DB.Regions, nameof(Region.ID), nameof(Region.Name));
            return View(country);
        }

        public ActionResult Edit(int id)
        {
            var country = DB.Countries.FirstOrDefault(c => c.ID == id);
            ViewBag.Title = $"Edit country: {country.Name}";
            ViewBag.regions = new SelectList(DB.Regions, nameof(Region.ID), nameof(Region.Name), country.RegionID);
            return View(country);
        }

        [ValidateAntiForgeryToken, HttpPost, ActionName(nameof(Edit))]
        public ActionResult Update(int id)
        {
            var country = DB.Countries.FirstOrDefault(c => c.ID == id);
            if (TryUpdateModel(country))
            {
                DB.SaveChanges();
                return RedirectToAction(nameof(Details), new { country.ID });
            }
            ViewBag.Title = $"Edit country: {country.Name}";
            ViewBag.regions = new SelectList(DB.Regions, nameof(Region.ID), nameof(Region.Name), country.RegionID);
            return View(country);
        }

        [AllowAnonymous]
        public JsonResult Search(string q)
        {
            var query = DB.Countries.ToList().Where(c => c.Name.ToLower().Contains(q.ToLower()));
            var res = new List<dynamic>();
            foreach (var item in query)
            {
                var list = new List<Select2Model>
                {
                    new Select2Model{Caption="Name",Value=item.Name},
                    new Select2Model{Caption="Region",Value=item.Region.Name},
                    new Select2Model{Caption="Continent",Value=item.Region.Continent.Name},
                };
                var kvpairs = new List<dynamic>();
                for (int i = 0; i < list.Count && i <= 3; i++)
                {
                    var detail = list[i];
                    kvpairs.Add(new { detail.Caption, detail.Value });
                }
                res.Add(new { id = item.ID, text = $"{item.Name}", extra = kvpairs });
            }
            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = res,
            };
        }
    }
}