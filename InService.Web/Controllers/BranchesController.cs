using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InService.Data;
using Microsoft.AspNet.Identity;
using PagedList;

namespace InService.Web.Controllers
{
    [Authorize]
    public class BranchesController : SysController
    {
        public ActionResult Index(int? p, string q)
        {
            var query = DB.Branches.AsQueryable();
            if (!String.IsNullOrEmpty(q)) query = query.Where(l => l.Name.Contains(q));
            ViewBag.Title = "Branches";
            ViewBag.q = q;
            return View(query.OrderByDescending(c => c.CreationDate).ToPagedList(p ?? 1, DefaultPageSize));
        }

        public ActionResult Details(int id)
        {
            var branch = DB.Branches.Find(id);
            ViewBag.Title = $"{branch.Name} details";
            return View(branch);
        }

        public ActionResult Add(int? SectionID)
        {
            ViewBag.Title = "Add branch";
            ViewBag.SectionID = SectionID;
            ViewBag.sections = new SelectList(DB.Sections.OrderBy(c => c.Name), nameof(Section.ID), nameof(Section.Name), SectionID);
            return View();
        }

        [ValidateAntiForgeryToken, HttpPost]
        public ActionResult Add(Branch branch, int? SectionID)
        {
            branch.CreatorID = CurrentUserID;
            branch.CreationDate = DateTime.Now;
            branch.SectionID = SectionID;
            if (ModelState.IsValid)
            {
                DB.Branches.Add(branch);
                DB.SaveChanges();
                return RedirectToAction(nameof(Details), new { branch.ID });
            }
            ViewBag.Title = "Add branch";
            ViewBag.sections = new SelectList(DB.Sections.OrderBy(c => c.Name), nameof(Section.ID), nameof(Section.Name), SectionID);
            return View(branch);
        }

        public ActionResult Edit(int id)
        {
            var branch = DB.Branches.Find(id);
            ViewBag.sections = new SelectList(DB.Sections.OrderBy(c => c.Name), nameof(Section.ID), nameof(Section.Name), branch.SectionID);
            ViewBag.Title = $"Edit {branch.Name}";
            return View(branch);
        }

        [ValidateAntiForgeryToken, HttpPost, ActionName(nameof(Edit))]
        public ActionResult Update(int id)
        {
            var branch = DB.Branches.Find(id);
            if (ModelState.IsValid)
            {
                if (TryUpdateModel(branch))
                {
                    DB.SaveChanges();
                    return RedirectToAction(nameof(Details), new { branch.ID });
                }
            }
            ViewBag.sections = new SelectList(DB.Sections.OrderBy(c => c.Name), nameof(Section.ID), nameof(Section.Name), branch.SectionID);
            ViewBag.Title = $"Edit {branch.Name}";
            return View(branch);
        }

        public ActionResult Delete(int? id)
        {
            var branch = DB.Branches.Find(id);
            ViewBag.Title = $"Delete {branch.Name}";
            return View(branch);
        }

        [HttpPost, ActionName(nameof(Delete))]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var branch = DB.Branches.Find(id);
            DB.Branches.Remove(branch);
            DB.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public JsonResult Search(string q)
        {
            var query = DB.Branches.Where(c => c.Name.Contains(q));
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

        public ActionResult AddIcon(int id)
        {
            var branch = DB.Branches.Find(id);
            ViewBag.branch = branch;
            ViewBag.Title = "Add icon";
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AddIcon(int id, HttpPostedFileBase file)
        {
            var branch = DB.Branches.Find(id);
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
            branch.IconID = attachment.ID;
            DB.Attachments.Add(attachment);
            DB.SaveChanges();
            return RedirectToAction(nameof(Details), new { branch.ID });
        }

        [AllowAnonymous]
        public FileResult Icon(int id)
        {
            var branch = DB.Branches.Find(id);
            if (branch.IconID.HasValue)
            {
                var attachment = branch.Attachment;
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
