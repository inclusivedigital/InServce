
using InService.Lib.Auth;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InService.Data;
using InService.Lib;
using PagedList;

namespace InService.Web.Controllers
{
    //   [Authorize(Roles = nameof(UserRole.ADMINISTRATOR))]
    public class DistrictsController : SysController
    {
        public ActionResult Index(int? p)
        {
            var query = DB.Districts.AsQueryable();
            return View(query.OrderBy(c => c.Name).ToPagedList(p ?? 1, DefaultPageSize));
        }
        public ActionResult Details(int id)
        {
            var d = DB.Districts.Find(id);
            ViewBag.Title = $"District : {d.Name}";
            return View(d);
        }

        public ActionResult Add(int id)
        {
            var province = DB.Provinces.Find(id);
            ViewBag.province = province;
            ViewBag.Title = "Add a district";
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Add(District district, int ProvinceID)
        {
            var province = DB.Provinces.Find(ProvinceID);
            district.CreationDate = DateTime.Now;
            district.CreatorID = User.Identity.GetUserId<int>();
            district.ProvinceID = province.ID;
            district.Code = GenerateCode(province);
            if (ModelState.IsValid)
            {
                DB.Districts.Add(district);
                DB.SaveChanges();
                return RedirectToAction(nameof(Details), new { district.ID });
            }

            ViewBag.province = province;
            ViewBag.Title = "Add a district";
            return View(district);
        }

        public ActionResult Edit(int id)
        {
            var d = DB.Districts.Find(id);
            ViewBag.Title = $"Edit district: {d.Name}";
            return View(d);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Edit))]
        public ActionResult Update(int id)
        {
            var d = DB.Districts.Find(id);
            if (TryUpdateModel(d, new string[] { nameof(District.Name) }))
            {
                if (d.Code == 0) d.Code = GenerateCode(d.Province);
                DB.SaveChanges();
                return RedirectToAction(nameof(Details), new { d.ID });
            }
            ViewBag.Title = $"Edit district: {d.Name}";
            return View(d);
        }

        [AllowAnonymous]
        public JsonResult Search(string q)
        {
            var query = DB.Districts.ToList().Where(c => c.Name.ToLower().Contains(q.ToLower()));
            var res = new List<dynamic>();
            foreach (var item in query)
            {
                var list = new List<Select2Model>
                {
                    new Select2Model{Caption="Name",Value=item.Name},
                    new Select2Model{Caption="Province",Value=item.Province.Name ?? ""},
                    new Select2Model{Caption="Country",Value=item.Province.Country.Name ?? ""},
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

        public int GenerateCode(Province province)
        {
            return DB.Districts.Count() + 1;
        }
    }
}