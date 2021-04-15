using InService.Data;
using InService.Lib.Auth;
using InService.Web.Auth;
using InService.Web.Models;
using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InService.Lib;

namespace InService.Web.Controllers
{
    [Authorize]
    public class LivestocksController : SysController
    {
        public ActionResult Index(int? p, int? CategoryID)
        {
            var query = DB.Livestocks.AsQueryable();
            if (CategoryID.HasValue) query = query.Where(c => c.CategoryID == CategoryID);
            ViewBag.Title = "livestocks";
            return View(new PagedList.PagedList<Livestock>(query.OrderBy(c => c.Name), p ?? 1, DefaultPageSize));
        }

        [AllowAnonymous]
        public ActionResult Details(int id)
        {
            var livestock = DB.Livestocks.Find(id);
            ViewBag.Title = $"{livestock.Name}";
            return View(livestock);
        }

        public ActionResult Remove(int id)
        {
            var livestock = DB.Livestocks.Find(id);
            DB.Livestocks.Remove(livestock);
            DB.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public ActionResult Add(int? CategoryID, int? ParentID, int? CourseID)
        {
            ViewBag.Title = "Add a livestock";
            ViewBag.parents = new SelectList(DB.Livestocks.OrderBy(c => c.Name), nameof(Livestock.ID), nameof(Livestock.Name), ParentID);
            ViewBag.categories = new SelectList(DB.LivestockCategories.OrderBy(c => c.Name), nameof(LivestockCategory.ID), nameof(LivestockCategory.Name), CategoryID);
            ViewBag.courses = new SelectList(DB.Courses.OrderBy(c => c.Name), nameof(Course.ID), nameof(Course.Name), CourseID);
            return View();
        }

        [ValidateAntiForgeryToken, HttpPost]
        public ActionResult Add(Livestock livestock, HttpPostedFileBase file)
        {
            livestock.CreatorID = User.Identity.GetUserId<int>();
            livestock.CreationDate = DateTime.UtcNow;
            if (DB.Livestocks.Any(c => c.Name.Contains(livestock.Name))) ModelState.AddModelError(nameof(livestock.Name), "The livestock already exists");
            if (ModelState.IsValid)
            {
                DB.Livestocks.Add(livestock);
                DB.SaveChanges();
                return RedirectToAction(nameof(Details), new { livestock.ID });
            }
            ViewBag.categories = new SelectList(DB.LivestockCategories.OrderBy(c => c.Name), nameof(LivestockCategory.ID), nameof(LivestockCategory.Name), livestock.CategoryID);
            ViewBag.parents = new SelectList(DB.Livestocks.OrderBy(c => c.Name), nameof(Livestock.ID), nameof(Livestock.Name), livestock.ParentID);
            ViewBag.courses = new SelectList(DB.Courses.OrderBy(c => c.Name), nameof(Course.ID), nameof(Course.Name), livestock.CourseID);
            ViewBag.Title = "Add a livestock";
            return View(livestock);
        }

        public ActionResult Edit(int id)
        {
            var livestock = DB.Livestocks.Find(id);
            ViewBag.Title = $"Edit {livestock.Name}";
            ViewBag.categories = new SelectList(DB.LivestockCategories.OrderBy(c => c.Name), nameof(LivestockCategory.ID), nameof(LivestockCategory.Name), livestock.CategoryID);
            ViewBag.parents = new SelectList(DB.Livestocks.Where(c => c.ID != livestock.ID).OrderBy(c => c.Name), nameof(Livestock.ID), nameof(Livestock.Name), livestock.ParentID);
            ViewBag.courses = new SelectList(DB.Courses.OrderBy(c => c.Name), nameof(Course.ID), nameof(Course.Name), livestock.CourseID);
            return View(livestock);
        }

        [ValidateAntiForgeryToken, HttpPost, ActionName(nameof(Edit))]
        public ActionResult Update(int id)
        {
            var livestock = DB.Livestocks.Find(id);
            if (TryUpdateModel(livestock))
            {
                if (ModelState.IsValid)
                {
                    DB.SaveChanges();
                    return RedirectToAction(nameof(Details), new { livestock.ID });
                }
            }
            ViewBag.Title = $"Edit {livestock.Name}";
            ViewBag.categories = new SelectList(DB.LivestockCategories.OrderBy(c => c.Name), nameof(LivestockCategory.ID), nameof(LivestockCategory.Name), livestock.CategoryID);
            ViewBag.parents = new SelectList(DB.Livestocks.Where(c => c.ID != livestock.ID).OrderBy(c => c.Name), nameof(Livestock.ID), nameof(Livestock.Name), livestock.ParentID);
            ViewBag.courses = new SelectList(DB.Courses.OrderBy(c => c.Name), nameof(Course.ID), nameof(Course.Name), livestock.CourseID);
            return View(livestock);
        }

        [AllowAnonymous]
        public JsonResult Search(string q)
        {
            var query = DB.Livestocks.ToList().Where(c => c.Name.ToLower().Contains(q.ToLower()));
            var papers = new List<dynamic>();
            foreach (var item in query.Take(5))
            {
                var list = new List<Select2Model>
                {
                    new Select2Model{Caption="Livestock",Value=item.Name},
                };
                var kvpairs = new List<dynamic>();
                for (int i = 0; i < list.Count && i <= 3; i++)
                {
                    var detail = list[i];
                    kvpairs.Add(new { detail.Caption, detail.Value });
                }
                papers.Add(new { id = item.ID, text = $"{item.Name}", extra = kvpairs });
            }
            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = papers,
            };
        }

        public ActionResult AddIcon(int id)
        {
            var stock = DB.Livestocks.Find(id);
            ViewBag.stock = stock;
            ViewBag.Title = "Add icon";
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AddIcon(int id, HttpPostedFileBase file)
        {
            var stock = DB.Livestocks.Find(id);
            var attachment = new Attachment
            {
                CreatorID = CurrentUserID,
                Description = "Icon",
                Extension = Path.GetExtension(file.FileName),
                ID = Guid.NewGuid(),
                Name = file.FileName,
                Size = file.ContentLength,
                UploadDate = DateTime.UtcNow,
                IsIcon = true,
            };
            string mainDir = attachment.Path;
            if (!Directory.Exists(mainDir))
                Directory.CreateDirectory(mainDir);
            file.SaveAs(Path.Combine(mainDir, $"{attachment.ID}{attachment.Extension}"));
            stock.IconID = attachment.ID;
            DB.Attachments.Add(attachment);
            DB.SaveChanges();
            return RedirectToAction(nameof(Details), new { stock.ID });
        }

        [AllowAnonymous]
        public FileResult Icon(int id)
        {
            var stock = DB.Livestocks.Find(id);
            if (stock.IconID.HasValue)
            {
                var attachment = stock.Attachment;
                string mainDir = attachment.Path;
                if (Directory.Exists(mainDir))
                {
                    var file = Directory.EnumerateFiles(mainDir).FirstOrDefault(c => c.Contains($"{attachment.ID}{attachment.Extension}"));
                    if (file != null) return File(file, $"image/{new FileInfo(file).Extension.Replace(".", "")}");
                }
            }
            return File("~/Content/placeholder.png", "image/png");
        }
    }
}