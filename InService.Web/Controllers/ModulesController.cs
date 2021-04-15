using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using InService.Data;
using InService.Web.Models;
using Microsoft.AspNet.Identity;
using PagedList;

namespace InService.Web.Controllers
{
    [Authorize]
    public class ModulesController : SysController
    {
        public ActionResult Index(int? p, string q)
        {
            var query = DB.Modules.AsQueryable();
            if (!String.IsNullOrEmpty(q)) query = query.Where(l => l.Name.Contains(q));
            ViewBag.Title = "Modules";
            ViewBag.q = q;
            return View(query.OrderByDescending(c => c.CreationDate).ToPagedList(p ?? 1, DefaultPageSize));
        }

        public ActionResult Details(int id)
        {
            Module module = DB.Modules.Find(id);
            ViewBag.Title = $"{module.Name} details";
            return View(module);
        }

        public ActionResult Add(int? CourseID)
        {
            ViewBag.Title = "Add module";
            ViewBag.courses = new SelectList(DB.Courses.OrderBy(c => c.Name), nameof(Course.ID), nameof(Course.Name), CourseID);
            return View();
        }

        [ValidateAntiForgeryToken, HttpPost]
        public ActionResult Add(Module module, int? CourseID)
        {
            module.CreatorID = CurrentUserID;
            module.CreationDate = DateTime.Now;
            if (string.IsNullOrWhiteSpace(module.Code)) module.Code = $"{(module.Name.Length <= 2 ? module.Name : module.Name.Substring(0, 2))}{AccountGenerator(4)}";
            if (ModelState.IsValid)
            {
                DB.Modules.Add(module);
                DB.SaveChanges();
                return RedirectToAction(nameof(Details), new { module.ID });
            }
            ViewBag.Title = "Add module";
            ViewBag.courses = new SelectList(DB.Courses.OrderBy(c => c.Name), nameof(Course.ID), nameof(Course.Name), CourseID);
            return View(module);
        }

        public ActionResult Edit(int id)
        {
            Module module = DB.Modules.Find(id);
            ViewBag.Title = $"Edit {module.Name}";
            ViewBag.courses = new SelectList(DB.Courses.OrderBy(c => c.Name), nameof(Course.ID), nameof(Course.Name), module.CourseID);
            return View(module);
        }

        [ValidateAntiForgeryToken, HttpPost, ActionName(nameof(Edit))]
        public ActionResult Update(int id)
        {
            var module = DB.Modules.Find(id);
            if (ModelState.IsValid)
            {
                if (TryUpdateModel(module))
                {
                    DB.SaveChanges();
                    return RedirectToAction(nameof(Details), new { module.ID });
                }
            }
            ViewBag.Title = $"Edit {module.Name}";
            ViewBag.courses = new SelectList(DB.Courses.OrderBy(c => c.Name), nameof(Course.ID), nameof(Course.Name), module.CourseID);
            return View(module);
        }

        public ActionResult Delete(int? id)
        {
            var module = DB.Modules.Find(id);
            ViewBag.Title = $"Delete {module.Name}";
            return View(module);
        }

        [HttpPost, ActionName(nameof(Delete))]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Module module = DB.Modules.Find(id);
            DB.Modules.Remove(module);
            DB.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public JsonResult Search(string q)
        {
            var query = DB.Modules.Where(c => c.Name.Contains(q));
            var modules = new List<dynamic>();
            foreach (var item in query.OrderBy(c => c.Name))
            {
                modules.Add(new { id = item.ID, text = $"{item.Name} {item.Code} Course: {item.Course.Name}" });
            }
            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = modules,
            };
        }

        public ActionResult Sorter(int CourseID, List<int> OID)
        {
            for (int i = 0; i < OID.Count; i++)
            {
                var field = DB.Modules.Find(OID[i]);
                field.Number = i + 1;
            }
            DB.SaveChanges();
            return RedirectToAction("Details", "Courses", new { ID = CourseID });
        }

        public ActionResult AddAttachment(int id)
        {
            var module = DB.Modules.Find(id);
            ViewBag.module = module;
            ViewBag.Title = "Add module attachments";
            return View();
        }

        [HttpPost]
        public ActionResult AddAttachment(int ID, List<HttpPostedFileBase> DocumentPhotos)
        {
            var module = DB.Modules.Find(ID);
            if (TryUpdateModel(module))
            {
                if (DocumentPhotos != null)
                {
                    var attachments = module.Attachments;
                    foreach (var item in DocumentPhotos)
                    {
                        var attachment = new Attachment
                        {
                            UploadDate = DateTime.UtcNow,
                            CreatorID = CurrentUserID,
                            Name = item.FileName,
                            Extension = Path.GetExtension(item.FileName),
                            Size = item.ContentLength,
                            Description = item.FileName,
                            ID = Guid.NewGuid(),
                        };
                        attachments.Add(attachment);
                        module.AttachmentsJson = new HtmlString(attachments.ToJSONString()).ToHtmlString().ToString();
                        DB.Attachments.Add(attachment);

                        string mainDir = attachment.Path;
                        if (!Directory.Exists(mainDir))
                            Directory.CreateDirectory(mainDir);
                        item.SaveAs(Path.Combine(mainDir, $"{attachment.ID}{Path.GetExtension(item.FileName)}"));
                    }
                    DB.SaveChanges();
                }
                return RedirectToAction(nameof(Details), new { ID });
            }
            ViewBag.module = module;
            ViewBag.Title = "Add module attachments";
            return View();
        }

        public ActionResult DeleteAttachment(List<int> AID, int id)
        {
            var module = DB.Modules.Find(id);
            foreach (var i in AID)
            {
                var attachment = DB.Attachments.Find(i);
                // string mainDir = Server.MapPath(attachment.Path);
                string mainDir = attachment.Path;
                if (!Directory.Exists(mainDir))
                    Directory.CreateDirectory(mainDir);
                DirectoryInfo dir = new DirectoryInfo(mainDir);
                foreach (FileInfo fi in dir.GetFiles())
                {
                    if (fi.Name.StartsWith($"{i}"))
                    {
                        fi.IsReadOnly = false;
                        fi.Delete();
                    }
                }
                var newattact = module.Attachments.Remove(attachment);
                DB.Attachments.Remove(attachment);
                module.AttachmentsJson = new HtmlString(newattact.ToJSONString()).ToHtmlString().ToString();
            }
            DB.SaveChanges();
            return RedirectToAction(nameof(Details), new { module.ID });
        }

        public ActionResult AddIcon(int id)
        {
            var module = DB.Modules.Find(id);
            ViewBag.module = module;
            ViewBag.Title = "Add icon";
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AddIcon(int id, HttpPostedFileBase file)
        {
            var module = DB.Modules.Find(id);
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
            module.IconID = attachment.ID;
            DB.Attachments.Add(attachment);
            DB.SaveChanges();
            return RedirectToAction(nameof(Details), new { module.ID });
        }

        [AllowAnonymous]
        public FileResult Icon(int id)
        {
            var module = DB.Crops.Find(id);
            if (module.IconID.HasValue)
            {
                var attachment = module.Attachment;
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
