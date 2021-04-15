using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using InService.Lib.Auth;
using Microsoft.AspNet.Identity;
using PagedList;
using PagedList.Mvc;
using InService.Data;

namespace InService.Web.Controllers
{
    [Authorize(Roles = nameof(UserRole.ADMINISTRATOR))]
    public class ContinentsController : SysController
    {
        public ActionResult Index(int? p, string q)
        {
            var query = DB.Continents.AsQueryable();
            query = query.Where(c => c.Name.Contains(q) || q == null);
            ViewBag.Title = "Continents";
            return View(query.OrderBy(c => c.Name).ToPagedList(p ?? 1, DefaultPageSize));
        }

        public ActionResult Details(int id)
        {
            var continent = DB.Continents.Find(id);
            ViewBag.Title = $"Continent: {continent.Name}";
            return View(continent);
        }

        public ActionResult Add()
        {
            ViewBag.Title = "Add a continent";
            return View();
        }

        [ValidateAntiForgeryToken, HttpPost]
        public ActionResult Add(Continent continent)
        {
            if (DB.Continents.Any(c => c.Name == continent.Name)) ModelState.AddModelError(nameof(continent.Name), "The continent has been added already.");
            continent.CreationDate = DateTime.UtcNow;
            continent.CreatorID = User.Identity.GetUserId<int>();
            if (ModelState.IsValid)
            {
                DB.Continents.Add(continent);
                DB.SaveChanges();
                return RedirectToAction(nameof(Details), new { continent.ID });
            }
            ViewBag.Title = "Add a continent";
            return View(continent);
        }

        public ActionResult Edit(int id)
        {
            var continent = DB.Continents.Find(id);
            ViewBag.Title = $"Edit continent: {continent.Name}";
            return View(continent);
        }

        [ValidateAntiForgeryToken, HttpPost, ActionName(nameof(Edit))]
        public ActionResult Update(int id)
        {
            var continent = DB.Continents.Find(id);
            if (TryUpdateModel(continent, new string[] { nameof(Continent.Name) }))
            {
                DB.SaveChanges();
                return RedirectToAction(nameof(Details), new { continent.ID });
            }
            ViewBag.Title = $"Edit continent: {continent.Name}";
            return View(continent);
        }

        [AllowAnonymous]
        public JsonResult Search(string q)
        {
            var query = DB.Continents.Where(c => c.Name.Contains(q));
            var continents = new List<dynamic>();
            foreach (var item in query.OrderBy(c => c.Name))
            {
                continents.Add(new { id = item.ID, text = item.Name });
            }
            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = continents,
            };
        }
    }
}