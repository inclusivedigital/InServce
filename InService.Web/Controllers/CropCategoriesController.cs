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
    public class CropCategoriesController : SysController
    {
        public ActionResult Index(int? p, int? BranchID)
        {
            var query = DB.CropCategories.AsQueryable();
            if (BranchID.HasValue) query = query.Where(c => c.BranchID == BranchID);
            ViewBag.Title = "crop categories";
            return View(new PagedList.PagedList<CropCategory>(query.OrderBy(c => c.Name), p ?? 1, DefaultPageSize));
        }

        [AllowAnonymous]
        public ActionResult Details(int id)
        {
            var category = DB.CropCategories.Find(id);
            ViewBag.Title = $"{category.Name}";
            return View(category);
        }

        public ActionResult Remove(int id)
        {
            var category = DB.CropCategories.Find(id);
            DB.CropCategories.Remove(category);
            DB.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public ActionResult Add(int? BranchID)
        {
            ViewBag.Title = "Add a crop category";
            ViewBag.branches = new SelectList(DB.Branches.OrderBy(c => c.Name), nameof(Branch.ID), nameof(Branch.Name), BranchID);
            return View();
        }

        [ValidateAntiForgeryToken, HttpPost]
        public ActionResult Add(CropCategory category, HttpPostedFileBase file)
        {
            category.CreatorID = User.Identity.GetUserId<int>();
            category.CreationDate = DateTime.UtcNow;
            if (DB.CropCategories.Any(c => c.Name.Contains(category.Name))) ModelState.AddModelError(nameof(category.Name), "The  category already exists");
            if (ModelState.IsValid)
            {
                DB.CropCategories.Add(category);
                DB.SaveChanges();
                return RedirectToAction(nameof(Details), new { category.ID });
            }
            ViewBag.branches = new SelectList(DB.Branches.OrderBy(c => c.Name), nameof(Branch.ID), nameof(Branch.Name), category.BranchID);
            ViewBag.Title = "Add a crop category";
            return View(category);
        }

        public ActionResult Edit(int id)
        {
            var category = DB.CropCategories.Find(id);
            ViewBag.Title = $"Edit {category.Name}";
            ViewBag.branches = new SelectList(DB.Branches.OrderBy(c => c.Name), nameof(Branch.ID), nameof(Branch.Name), category.BranchID);
            return View(category);
        }

        [ValidateAntiForgeryToken, HttpPost, ActionName(nameof(Edit))]
        public ActionResult Update(int id)
        {
            var category = DB.CropCategories.Find(id);
            if (TryUpdateModel(category, new string[] { nameof(CropCategory.Name) }))
            {
                if (ModelState.IsValid)
                {
                    DB.SaveChanges();
                    return RedirectToAction(nameof(Details), new { category.ID });
                }
            }
            ViewBag.Title = $"Edit {category.Name}";
            ViewBag.branches = new SelectList(DB.Branches.OrderBy(c => c.Name), nameof(Branch.ID), nameof(Branch.Name), category.BranchID);
            return View(category);
        }

        [AllowAnonymous]
        public JsonResult Search(string q)
        {
            var query = DB.CropCategories.ToList().Where(c => c.Name.ToLower().Contains(q.ToLower()));
            var papers = new List<dynamic>();
            foreach (var item in query.Take(5))
            {
                var list = new List<Select2Model>
                {
                    new Select2Model{Caption="Category",Value=item.Name},
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
            var category = DB.CropCategories.Find(id);
            ViewBag.category = category;
            ViewBag.Title = "Add icon";
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AddIcon(int id, HttpPostedFileBase file)
        {
            var category = DB.CropCategories.Find(id);
            var attachment = new Attachment
            {
                CreatorID = CurrentUserID,
                Description = "Icon",
                Extension = Path.GetExtension(file.FileName),
                ID = Guid.NewGuid(),
                Name = file.FileName,
                Size = file.ContentLength,
                UploadDate = DateTime.UtcNow,
                IsIcon=true,
            };
            string mainDir = attachment.Path;
            if (!Directory.Exists(mainDir))
                Directory.CreateDirectory(mainDir);
            file.SaveAs(Path.Combine(mainDir, $"{attachment.ID}{attachment.Extension}"));
            category.IconID = attachment.ID;
            DB.Attachments.Add(attachment);
            DB.SaveChanges();
            return RedirectToAction(nameof(Details), new { category.ID });
        }

        [AllowAnonymous]
        public FileResult Icon(int id)
        {
            var category = DB.CropCategories.Find(id);
            if (category.IconID.HasValue)
            {
                var attachment = category.Attachment;
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