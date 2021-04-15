using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using InService.Data;
using Microsoft.AspNet.Identity;
using PagedList;

namespace InService.Web.Controllers
{
    [Authorize]
    public class DepartmentsController : SysController
    {
        public ActionResult Index(int? p, string q)
        {
            var query = DB.Departments.Where(c => c.ParentID == null).AsQueryable();
            if (!String.IsNullOrEmpty(q)) query = query.Where(l => l.Name.Contains(q));
            ViewBag.Title = "Departments";
            ViewBag.q = q;
            return View(query.OrderByDescending(c => c.CreationDate).ToPagedList(p ?? 1, DefaultPageSize));
        }

        public ActionResult Details(int id)
        {
            Department department = DB.Departments.Find(id);
            ViewBag.Title = $"{department.Name} details";
            return View(department);
        }

        public ActionResult Add(int? ParentID)
        {
            ViewBag.Title = "Add department";
            ViewBag.ParentID = ParentID;
            if (DB.Departments.Any()) ViewBag.departments = new SelectList(DB.Departments.OrderBy(c => c.Name), nameof(Department.ID), nameof(Department.Name));
            return View();
        }

        [ValidateAntiForgeryToken, HttpPost]
        public ActionResult Add(Department department, int? ParentID)
        {
            department.CreatorID = CurrentUserID;
            department.CreationDate = DateTime.Now;
            if (ParentID.HasValue) department.ParentID = ParentID;
            if (ModelState.IsValid)
            {
                DB.Departments.Add(department);
                DB.SaveChanges();
                return RedirectToAction(nameof(Details), new { department.ID });
            }
            ViewBag.Title = "Add department";
            if (DB.Departments.Any()) ViewBag.departments = new SelectList(DB.Departments.OrderBy(c => c.Name), nameof(Department.ID), nameof(Department.Name), department.ParentID);
            return View(department);
        }

        public ActionResult Edit(int id)
        {
            Department department = DB.Departments.Find(id);
            ViewBag.departments = new SelectList(DB.Departments.Where(c => c.ID != department.ID).OrderBy(c => c.Name), nameof(Department.ID), nameof(Department.Name), department.ParentID);
            ViewBag.Title = $"Edit {department.Name}";
            return View(department);
        }

        [ValidateAntiForgeryToken, HttpPost, ActionName(nameof(Edit))]
        public ActionResult Update(int id)
        {
            var department = DB.Departments.Find(id);
            if (ModelState.IsValid)
            {
                if (TryUpdateModel(department))
                {
                    DB.SaveChanges();
                    return RedirectToAction(nameof(Details), new { department.ID });
                }
            }
            ViewBag.departments = new SelectList(DB.Departments.Where(c => c.ID != department.ID).OrderBy(c => c.Name), nameof(Department.ID), nameof(Department.Name), department.ParentID);
            ViewBag.Title = $"Edit {department.Name}";
            return View(department);
        }

        public ActionResult Delete(int? id)
        {
            var department = DB.Departments.Find(id);
            ViewBag.Title = $"Delete {department.Name}";
            return View(department);
        }

        [HttpPost, ActionName(nameof(Delete))]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Department department = DB.Departments.Find(id);
            DB.Departments.Remove(department);
            DB.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public JsonResult Search(string q)
        {
            var query = DB.Departments.Where(c => c.Name.Contains(q));
            var departments = new List<dynamic>();
            foreach (var item in query.OrderBy(c => c.Name))
            {
                departments.Add(new { id = item.ID, text = item.Name });
            }
            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = departments,
            };
        }
    }
}
