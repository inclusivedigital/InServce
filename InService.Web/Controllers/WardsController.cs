
using InService.Lib.Auth;
using InService.Web.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InService.Data;
using InService.Lib;

namespace InService.Web.Controllers
{
    //  [Authorize(Roles = nameof(UserRole.ADMINISTRATOR))]
    public class WardsController : SysController
    {
        public ActionResult Details(int id)
        {
            var d = DB.Wards.Find(id);
            ViewBag.Title = $"Ward : {d.Name}";
            return View(d);
        }

        public ActionResult Add(int id)
        {
            var d = DB.Districts.Find(id);
            ViewBag.district = d;
            ViewBag.Title = "Add a ward";
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Add(Ward ward, int DistrictID)
        {
            var d = DB.Districts.Find(DistrictID);
            ward.CreationDate = DateTime.Now;
            ward.CreatorID = User.Identity.GetUserId<int>();
            ward.DistrictID = d.ID;
            ward.Code = $"{(DB.Wards.Count() + 1)}";
            if (ModelState.IsValid)
            {
                DB.Wards.Add(ward);
                DB.SaveChanges();
                return RedirectToAction(nameof(Details), new { ward.ID });
            }

            ViewBag.district = d;
            ViewBag.Title = "Add a ward";
            return View(ward);
        }

        public ActionResult Edit(int id)
        {
            var d = DB.Wards.Find(id);
            ViewBag.Title = $"Edit ward: {d.Name}";
            return View(d);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Edit))]
        public ActionResult Update(int id)
        {
            var d = DB.Wards.Find(id);
            if (TryUpdateModel(d))
            {
                DB.SaveChanges();
                return RedirectToAction(nameof(Details), new { d.ID });
            }
            ViewBag.Title = $"Edit ward: {d.Name}";
            return View(d);
        }

        [AllowAnonymous]
        public JsonResult Search(string q)
        {
            var query = DB.Wards.ToList().Where(c => c.Name.ToLower().Contains(q.ToLower()) || c.Number.ToString().ToLower().Contains(q.ToLower()));
            var students = new List<dynamic>();
            foreach (var item in query.Take(5))
            {
                var list = new List<Select2Model>
                {
                    new Select2Model{Caption="Name",Value=item.Name},
                    new Select2Model{Caption="District",Value=item.District.Name},
                    new Select2Model{Caption="Province",Value=item.District.Province.Name},
                    new Select2Model{Caption="Country",Value=item.District.Province.Country.Name},
                };
                var kvpairs = new List<dynamic>();
                for (int i = 0; i < list.Count && i <= 3; i++)
                {
                    var detail = list[i];
                    kvpairs.Add(new { detail.Caption, detail.Value });
                }
                students.Add(new { id = item.ID, text = $"{item.Name}, {item.District.Name}, {item.District.Province.Name} {item.District.Province.Country.Name}", extra = kvpairs });
            }

            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = students,
            };
        }
    }
}