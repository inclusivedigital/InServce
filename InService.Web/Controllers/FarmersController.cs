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
using InService.Web.Models;
using InService.Lib;
using System.IO;
using Newtonsoft.Json;

namespace InService.Web.Controllers
{
    public class FarmersController : SysController
    {
        public ActionResult Index(int? p, string q, int? CountryID)
        {
            var query = DB.Farmers.AsQueryable();
            if (!String.IsNullOrEmpty(q)) query = query.Where(c => c.Name.Contains(q) || c.Mobile.Contains(q) || c.IDNumber.Contains(q) || c.Email.Contains(q) || c.GrowersNumber.Contains(q));
            if (CountryID.HasValue) query = query.Where(c => c.CountryID == CountryID);
            ViewBag.Title = "Farmers";
            return View(query.OrderBy(c => c.Name).ToPagedList(p ?? 1, DefaultPageSize));
        }

        public ActionResult Details(int id)
        {
            var farmer = DB.Farmers.Find(id);
            ViewBag.Title = "Farmer details";
            return View(farmer);
        }

        // GET: Farmers/Create
        public ActionResult Add()
        {
            ViewBag.Title = "Add a farmer";
            return View();
        }

        [ValidateAntiForgeryToken, HttpPost]
        public ActionResult Add(Farmer farmer, int? WardID)
        {
            farmer.CreationDate = DateTime.UtcNow;
            farmer.Account = AccountGenerator(6).ToString();
            if (WardID.HasValue)
            {
                farmer.WardID = WardID;
                var ward = DB.Wards.Find(WardID);
                farmer.DistrictID = ward.DistrictID;
                farmer.ProvinceID = ward.District.ProvinceID;
            }
            if (ModelState.IsValid)
            {
                DB.Farmers.Add(farmer);
                farmer.Users.Add(new User
                {
                    CreationDate = DateTime.UtcNow,
                    LoginID = farmer.Mobile,
                    Email = farmer.Email ?? farmer.Mobile,
                    Name = $"{farmer.Name ?? ""} {farmer.Surname ?? ""}",
                    StatusID = (int)UserStatus.ACTIVE,
                    RoleID = (int)UserRole.FARMER,
                    Mobile = farmer.Mobile,
                });
                DB.SaveChanges();
                return RedirectToAction(nameof(Details), new { farmer.ID });
            }
            ViewBag.Title = "Add a farmer";
            return View(farmer);
        }


        // GET: Farmers/Edit/5
        public ActionResult Edit(int id)
        {
            var farmer = DB.Farmers.Find(id);
            ViewBag.Title = "Edit farmer details";
            return View(farmer);
        }

        [ValidateAntiForgeryToken, HttpPost, ActionName(nameof(Edit))]
        public ActionResult Update(int id, int? WardID)
        {
            var farmer = DB.Farmers.Find(id);
            //  if (String.IsNullOrEmpty(farmer.AccountNumber)) farmer.AccountNumber = GenerateCode().ToString();
            if (TryUpdateModel(farmer))
            {
                if (WardID.HasValue)
                {
                    farmer.WardID = WardID;
                    var ward = DB.Wards.Find(WardID);
                    farmer.DistrictID = ward.DistrictID;
                    farmer.ProvinceID = ward.District.ProvinceID;
                }
                DB.SaveChanges();
                return RedirectToAction(nameof(Details), new { farmer.ID });
            }
            ViewBag.Title = "Edit farmer details";
            return View(farmer);
        }

        public JsonResult Search(string q)
        {
            var query = DB.Farmers.ToList().Where(c => c.Name.ToLower().Contains(q.ToLower()));
            var res = new List<dynamic>();
            foreach (var item in query)
            {
                var list = new List<Select2Model>
                {
                    new Select2Model{Caption="Name",Value=item.Fullname},
                    new Select2Model{Caption="Mobile",Value=item.Mobile ?? ""},
                    new Select2Model{Caption="Email",Value=item.Email ?? ""},
                    new Select2Model{Caption="Growers #",Value=item.GrowersNumber ?? ""},
                };
                var kvpairs = new List<dynamic>();
                for (int i = 0; i < list.Count && i <= 3; i++)
                {
                    var detail = list[i];
                    kvpairs.Add(new { detail.Caption, detail.Value });
                }
                res.Add(new { id = item.ID, text = $"{item.Fullname}", extra = kvpairs, imgURL = Url.Action(nameof(Photo), "Farmers", new { item.ID }) });
            }
            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = res,
            };
        }

        [AllowAnonymous]
        public FileResult Photo(int ID)
        {
            var farmer = DB.Farmers.Find(ID);
            // string mainDir = Server.MapPath(farmer.Photo);
            string mainDir = farmer.Photo;
            if (Directory.Exists(mainDir))
            {
                var file = Directory.EnumerateFiles(mainDir).FirstOrDefault(c => c.Contains(farmer.ID.ToString()));
                if (file != null) return File(file, $"image/{new FileInfo(file).Extension.Replace(".", "")}");
            }
            string name = farmer.Gender == Gender.MALE ? "boy" : "girl";
            return File($"~/Content/images/{name}.png", "image/png");
        }

        public ActionResult Insights(int? TypeID)
        {
            ViewBag.Title = $"Farmer insights";
            var plot = new List<Plot>();
            var query = DB.Farmers.AsQueryable();
            if (TypeID.HasValue) query = query.Where(c => c.TypeID == TypeID);
            foreach (var item in query.GroupBy(c => c.Type))
            {
                var t = item.Count();
                plot.Add(new Plot(t, item.Key.ToEnumString()));
            }
            ViewBag.points = JsonConvert.SerializeObject(plot);
            return View(new PagedList.PagedList<Farmer>(query.OrderByDescending(c => c.CreationDate), 1, 100));
        }
    }
}




