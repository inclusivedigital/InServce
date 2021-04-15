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
    public class CropsController : SysController
    {
        public ActionResult Index(int? p, int? CategoryID)
        {
            var query = DB.Crops.AsQueryable();
            if (CategoryID.HasValue) query = query.Where(c => c.CategoryID == CategoryID);
            ViewBag.Title = "crops";
            return View(new PagedList.PagedList<Crop>(query.OrderBy(c => c.Name), p ?? 1, DefaultPageSize));
        }

        [AllowAnonymous]
        public ActionResult Details(int id)
        {
            var crop = DB.Crops.Find(id);
            ViewBag.Title = $"{crop.Name}";
            return View(crop);
        }

        public ActionResult Remove(int id)
        {
            var crop = DB.Crops.Find(id);
            DB.Crops.Remove(crop);
            DB.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public ActionResult Add(int? CategoryID, int? CourseID)
        {
            ViewBag.Title = "Add a crop";
            ViewBag.courses = new SelectList(DB.Courses.OrderBy(c => c.Name), nameof(Course.ID), nameof(Course.Name), CourseID);
            ViewBag.categories = new SelectList(DB.CropCategories.OrderBy(c => c.Name), nameof(CropCategory.ID), nameof(CropCategory.Name), CategoryID);
            return View();
        }

        [ValidateAntiForgeryToken, HttpPost]
        public ActionResult Add(Crop crop, HttpPostedFileBase file)
        {
            crop.CreatorID = User.Identity.GetUserId<int>();
            crop.CreationDate = DateTime.UtcNow;
            if (DB.Crops.Any(c => c.Name.Contains(crop.Name))) ModelState.AddModelError(nameof(crop.Name), "The crop already exists");
            if (ModelState.IsValid)
            {
                DB.Crops.Add(crop);
                DB.SaveChanges();
                return RedirectToAction(nameof(Details), new { crop.ID });
            }
            ViewBag.categories = new SelectList(DB.CropCategories.OrderBy(c => c.Name), nameof(CropCategory.ID), nameof(CropCategory.Name), crop.CategoryID);
            ViewBag.courses = new SelectList(DB.Courses.OrderBy(c => c.Name), nameof(Course.ID), nameof(Course.Name), crop.CourseID);
            ViewBag.Title = "Add a value Chain";
            return View(crop);
        }

        public ActionResult Edit(int id)
        {
            var crop = DB.Crops.Find(id);
            ViewBag.Title = $"Edit {crop.Name}";
            ViewBag.categories = new SelectList(DB.CropCategories.OrderBy(c => c.Name), nameof(CropCategory.ID), nameof(CropCategory.Name), crop.CategoryID);
            ViewBag.courses = new SelectList(DB.Courses.OrderBy(c => c.Name), nameof(Course.ID), nameof(Course.Name), crop.CourseID);
            return View(crop);
        }

        [ValidateAntiForgeryToken, HttpPost, ActionName(nameof(Edit))]
        public ActionResult Update(int id)
        {
            var crop = DB.Crops.Find(id);
            if (TryUpdateModel(crop))
            {
                if (ModelState.IsValid)
                {
                    DB.SaveChanges();
                    return RedirectToAction(nameof(Details), new { crop.ID });
                }
            }
            ViewBag.Title = $"Edit {crop.Name}";
            ViewBag.categories = new SelectList(DB.CropCategories.OrderBy(c => c.Name), nameof(CropCategory.ID), nameof(CropCategory.Name), crop.CategoryID);
            ViewBag.courses = new SelectList(DB.Courses.OrderBy(c => c.Name), nameof(Course.ID), nameof(Course.Name), crop.CourseID);
            return View(crop);
        }

        [AllowAnonymous]
        public JsonResult Search(string q)
        {
            var query = DB.Crops.ToList().Where(c => c.Name.ToLower().Contains(q.ToLower()));
            var papers = new List<dynamic>();
            foreach (var item in query.Take(5))
            {
                var list = new List<Select2Model>
                {
                    new Select2Model{Caption="Crop",Value=item.Name},
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
            var crop = DB.Crops.Find(id);
            ViewBag.crop = crop;
            ViewBag.Title = "Add icon";
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AddIcon(int id, HttpPostedFileBase file)
        {
            var crop = DB.Crops.Find(id);
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
            crop.IconID = attachment.ID;
            DB.Attachments.Add(attachment);
            DB.SaveChanges();
            return RedirectToAction(nameof(Details), new { crop.ID });
        }

        [AllowAnonymous]
        public FileResult Icon(int id)
        {
            var crop = DB.Crops.Find(id);
            if (crop.IconID.HasValue)
            {
                var attachment = crop.Attachment;
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