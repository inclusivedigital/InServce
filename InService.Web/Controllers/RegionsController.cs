using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using InService.Lib.Auth;
using Microsoft.AspNet.Identity;
using PagedList;
using PagedList.Mvc;
using System.Drawing;
using InService.Data;

namespace InService.Web.Controllers
{
  //  [Authorize(Roles = nameof(UserRole.ADMINISTRATOR))]
    public class RegionsController : SysController
    {
        public ActionResult Index(int? p, string q)
        {
            var query = DB.Regions.AsQueryable();
            query = query.Where(c => c.Name.Contains(q) || q == null);
            ViewBag.Title = "Regions";
            return View(query.OrderBy(c => c.Name).ToPagedList(p ?? 1, DefaultPageSize));
        }

        public ActionResult Details(int id)
        {
            var region = DB.Regions.Find(id);
            ViewBag.Title = $"Region: {region.Name}";
            return View(region);
        }

        public ActionResult Add(int? id)
        {
            if (id.HasValue)
            {
                var continent = DB.Continents.Find(id);
                ViewBag.continent = continent;
            }
            ViewBag.Title = "Add a region";
            ViewBag.continents = new SelectList(DB.Continents.OrderBy(c => c.Name), nameof(Continent.ID), nameof(Continent.Name));
            return View();
        }

        [ValidateAntiForgeryToken, HttpPost]
        public ActionResult Add(Data.Region region, int? id)
        {
            if (DB.Regions.Any(c => c.Name == region.Name)) ModelState.AddModelError(nameof(region.Name), "The region has been added already.");
            region.CreationDate = DateTime.UtcNow;
            region.CreatorID = User.Identity.GetUserId<int>();
            if (id.HasValue)
            {
                var continent = DB.Continents.Find(id);
                ViewBag.continent = continent;
                region.ContinentID = continent.ID;
            }
            if (ModelState.IsValid)
            {
                DB.Regions.Add(region);
                DB.SaveChanges();
                return RedirectToAction(nameof(Details), new { region.ID });
            }
            ViewBag.Title = "Add a region";
            ViewBag.continents = new SelectList(DB.Continents.OrderBy(c => c.Name), nameof(Continent.ID), nameof(Continent.Name));
            return View(region);
        }

        public ActionResult Edit(int id)
        {
            var region = DB.Regions.Find(id);
            ViewBag.continents = new SelectList(DB.Continents.OrderBy(c => c.Name), nameof(Continent.ID), nameof(Continent.Name), region.ContinentID);
            ViewBag.Title = $"Edit region: {region.Name}";
            return View(region);
        }

        [ValidateAntiForgeryToken, HttpPost, ActionName(nameof(Edit))]
        public ActionResult Update(int id)
        {
            var region = DB.Regions.Find(id);
            if (TryUpdateModel(region, new string[] { nameof(Data.Region.Name) }))
            {
                DB.SaveChanges();
                return RedirectToAction(nameof(Details), new { region.ID });
            }
            ViewBag.Title = $"Edit region: {region.Name}";
            ViewBag.continents = new SelectList(DB.Continents.OrderBy(c => c.Name), nameof(Continent.ID), nameof(Continent.Name), region.ContinentID);
            return View(region);
        }

        [AllowAnonymous]
        public JsonResult Search(string q)
        {
            var query = DB.Regions.Where(c => c.Name.Contains(q));
            var regions = new List<dynamic>();
            foreach (var item in query.OrderBy(c => c.Name))
            {
                regions.Add(new { id = item.ID, text = item.Name });
            }
            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = regions,
            };
        }
    }
}