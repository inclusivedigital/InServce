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
    public class SectionsController : SysController
    {
        public ActionResult Index(int? p, string q, int? DepartmentID)
        {
            var query = DB.Sections.AsQueryable();
            if (!String.IsNullOrEmpty(q)) query = query.Where(l => l.Name.Contains(q));
            if (DepartmentID.HasValue) query = query.Where(c => c.DepartmentID == DepartmentID);
            ViewBag.Title = "Sections";
            ViewBag.q = q;
            return View(query.OrderByDescending(c => c.CreationDate).ToPagedList(p ?? 1, DefaultPageSize));
        }

        public ActionResult Details(int id)
        {
            Section department = DB.Sections.Find(id);
            ViewBag.Title = $"{department.Name} details";
            return View(department);
        }

        public ActionResult Add(int? DepartmentID)
        {
            ViewBag.Title = "Add department";
            ViewBag.DepartmentID = DepartmentID;
            ViewBag.departments = new SelectList(DB.Departments.OrderBy(c => c.Name), nameof(Department.ID), nameof(Department.Name));
            return View();
        }

        [ValidateAntiForgeryToken, HttpPost]
        public ActionResult Add(Section section, int? DepartmentID)
        {
            section.CreatorID = CurrentUserID;
            section.CreationDate = DateTime.Now;
            if (DepartmentID.HasValue) section.DepartmentID = DepartmentID;
            if (ModelState.IsValid)
            {
                DB.Sections.Add(section);
                DB.SaveChanges();
                return RedirectToAction(nameof(Details), new { section.ID });
            }
            ViewBag.Title = "Add department";
            ViewBag.departments = new SelectList(DB.Departments.OrderBy(c => c.Name), nameof(Department.ID), nameof(Department.Name), section.DepartmentID);
            return View(section);
        }

        public ActionResult Edit(int id)
        {
            Section section = DB.Sections.Find(id);
            ViewBag.departments = new SelectList(DB.Departments.OrderBy(c => c.Name), nameof(Department.ID), nameof(Department.Name), section.DepartmentID);
            ViewBag.Title = $"Edit {section.Name}";
            return View(section);
        }

        [ValidateAntiForgeryToken, HttpPost, ActionName(nameof(Edit))]
        public ActionResult Update(int id)
        {
            var department = DB.Sections.Find(id);
            if (ModelState.IsValid)
            {
                if (TryUpdateModel(department))
                {
                    DB.SaveChanges();
                    return RedirectToAction(nameof(Details), new { department.ID });
                }
            }
            ViewBag.departments = new SelectList(DB.Departments.OrderBy(c => c.Name), nameof(Department.ID), nameof(Department.Name), department.DepartmentID);
            ViewBag.Title = $"Edit {department.Name}";
            return View(department);
        }

        public ActionResult Delete(int? id)
        {
            var department = DB.Sections.Find(id);
            ViewBag.Title = $"Delete {department.Name}";
            return View(department);
        }

        [HttpPost, ActionName(nameof(Delete))]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Section department = DB.Sections.Find(id);
            DB.Sections.Remove(department);
            DB.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public JsonResult Search(string q)
        {
            var query = DB.Sections.Where(c => c.Name.Contains(q));
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
