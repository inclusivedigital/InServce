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
    public class CoursesController : SysController
    {
        public ActionResult Index(int? p, int? NonValueChainID, int? ValueChainID)
        {
            var query = DB.Courses.AsQueryable();
            if (NonValueChainID.HasValue) query = query.Where(c => c.NonValueChainID == NonValueChainID);
            if (ValueChainID.HasValue) query = query.Where(c => c.ValueChainID == ValueChainID);
            ViewBag.Title = "Courses";
            DB.SaveChanges();
            return View(new PagedList.PagedList<Course>(query.OrderBy(c => c.Name), p ?? 1, DefaultPageSize));
        }

        [AllowAnonymous]
        public ActionResult Details(int id)
        {
            var course = DB.Courses.Find(id);
            ViewBag.Title = $"{course.Name}";
            return View(course);
        }

        public ActionResult Remove(int id)
        {
            var course = DB.Courses.Find(id);
            DB.Courses.Remove(course);
            DB.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public ActionResult Add(int? ValueChainID, int? BranchID, int? NonValueChainID)
        {
            ViewBag.Title = "Add a course";
            if (!DB.Branches.Any()) return RedirectToAction("Add", "Branches");
            ViewBag.branches = new SelectList(DB.Branches.OrderBy(c => c.Name), nameof(Branch.ID), nameof(Branch.Name), BranchID);
            ViewBag.valuechains = new SelectList(DB.ValueChains.OrderBy(c => c.Name), nameof(ValueChain.ID), nameof(ValueChain.Name), ValueChainID);
            ViewBag.nonvaluechains = new SelectList(DB.NonValueChains.OrderBy(c => c.Name), nameof(NonValueChain.ID), nameof(NonValueChain.Name), NonValueChainID);
            return View();
        }

        [ValidateAntiForgeryToken, HttpPost]
        public ActionResult Add(Course course, HttpPostedFileBase file)
        {
            course.CreatorID = User.Identity.GetUserId<int>();
            course.CreationDate = DateTime.UtcNow;
            if (DB.Courses.Any(c => c.Name.Contains(course.Name) && c.Code.Contains(course.Code))) ModelState.AddModelError(nameof(course.Name), "The course and code already exists");
            if (string.IsNullOrWhiteSpace(course.Code)) course.Code = $"{(course.Name.Length <= 2 ? course.Name : course.Name.Substring(0, 2))}{AccountGenerator(4)}";
            if (ModelState.IsValid)
            {
                DB.Courses.Add(course);
                DB.SaveChanges();
                return RedirectToAction(nameof(Details), new { course.ID });
            }
            ViewBag.Title = "Add a course";
            ViewBag.branches = new SelectList(DB.Branches.OrderBy(c => c.Name), nameof(Branch.ID), nameof(Branch.Name), course.BranchID);
            ViewBag.valuechains = new SelectList(DB.ValueChains.OrderBy(c => c.Name), nameof(ValueChain.ID), nameof(ValueChain.Name), course.ValueChainID);
            ViewBag.nonvaluechains = new SelectList(DB.NonValueChains.OrderBy(c => c.Name), nameof(NonValueChain.ID), nameof(NonValueChain.Name), course.NonValueChainID);
            return View(course);
        }

        public ActionResult Edit(int id)
        {
            var course = DB.Courses.Find(id);
            ViewBag.Title = $"Edit {course.Name}";
            ViewBag.branches = new SelectList(DB.Branches.OrderBy(c => c.Name), nameof(Branch.ID), nameof(Branch.Name), course.BranchID);
            ViewBag.valuechains = new SelectList(DB.ValueChains.OrderBy(c => c.Name), nameof(ValueChain.ID), nameof(ValueChain.Name), course.ValueChainID);
            ViewBag.nonvaluechains = new SelectList(DB.NonValueChains.OrderBy(c => c.Name), nameof(NonValueChain.ID), nameof(NonValueChain.Name), course.NonValueChainID);
            return View(course);
        }

        [ValidateAntiForgeryToken, HttpPost, ActionName(nameof(Edit))]
        public ActionResult Update(int id)
        {
            var course = DB.Courses.Find(id);
            if (TryUpdateModel(course))
            {
                if (ModelState.IsValid)
                {
                    DB.SaveChanges();
                    return RedirectToAction(nameof(Details), new { course.ID });
                }
            }
            ViewBag.Title = $"Edit {course.Name}";
            ViewBag.branches = new SelectList(DB.Branches.OrderBy(c => c.Name), nameof(Branch.ID), nameof(Branch.Name), course.BranchID);
            ViewBag.valuechains = new SelectList(DB.ValueChains.OrderBy(c => c.Name), nameof(ValueChain.ID), nameof(ValueChain.Name), course.ValueChainID);
            ViewBag.nonvaluechains = new SelectList(DB.NonValueChains.OrderBy(c => c.Name), nameof(NonValueChain.ID), nameof(NonValueChain.Name), course.NonValueChainID);
            return View(course);
        }

        [AllowAnonymous]
        public JsonResult Search(string q)
        {
            var query = DB.Courses.ToList().Where(c => c.Name.ToLower().Contains(q.ToLower()) || c.Code.ToLower().Contains(q.ToLower()));
            var papers = new List<dynamic>();
            foreach (var item in query.Take(5))
            {
                var list = new List<Select2Model>
                {
                    new Select2Model{Caption="Course",Value=item.Name},
                    new Select2Model{Caption="Code",Value=item.Code},
                };
                var kvpairs = new List<dynamic>();
                for (int i = 0; i < list.Count && i <= 3; i++)
                {
                    var detail = list[i];
                    kvpairs.Add(new { detail.Caption, detail.Value });
                }
                papers.Add(new { id = item.ID, text = $"{item.Name} {item.Code}", extra = kvpairs });
            }
            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = papers,
            };
        }
        public ActionResult Adjust()
        {
            foreach (var course in DB.Courses.GroupBy(c => c.Name))
            {
                var mastercourse = course.First();
                if (course.Count() > 1)
                {
                    foreach (var child in course.ToArray().Skip(1))
                    {
                        if (child.Modules.Any())
                        {
                            foreach (var module in child.Modules)
                            {
                                module.CourseID = mastercourse.ID;
                            }

                        }
                        if (child.Examinations.Any())
                        {
                            foreach (var exam in child.Examinations)
                            {
                                exam.CourseID = mastercourse.ID;
                            }
                        }
                        if (child.ID != mastercourse.ID) DB.Courses.Remove(child);
                    }
                }
            }
            DB.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public ActionResult EditGlossary(int id)
        {
            var course = DB.Courses.Find(id);
            ViewBag.course = course;
            ViewBag.Title = "Edit glossary";
            return View();
        }
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult EditGlossary(int id, string Description)
        {
            var course = DB.Courses.Find(id);
            course.Glossary = Description;
            DB.SaveChanges();
            return RedirectToAction(nameof(Details), new { course.ID });
        }

        public ActionResult AddAttachment(int id)
        {
            var course = DB.Courses.Find(id);
            ViewBag.course = course;
            ViewBag.Title = "Add course attachments";
            return View();
        }

        [HttpPost]
        public ActionResult AddAttachment(int ID, List<HttpPostedFileBase> DocumentPhotos)
        {
            var course = DB.Courses.Find(ID);
            if (TryUpdateModel(course))
            {
                if (DocumentPhotos != null)
                {
                    var attachments = course.Attachments;
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
                        course.AttachmentsJson = new HtmlString(attachments.ToJSONString()).ToHtmlString().ToString();
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
            ViewBag.course = course;
            ViewBag.Title = "Add course attachments";
            return View();
        }

        public ActionResult DeleteAttachment(List<int> AID, int id)
        {
            var course = DB.Courses.Find(id);
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
                var newattact = course.Attachments.Remove(attachment);
                DB.Attachments.Remove(attachment);
                course.AttachmentsJson = new HtmlString(newattact.ToJSONString()).ToHtmlString().ToString();
            }
            DB.SaveChanges();
            return RedirectToAction(nameof(Details), new { course.ID });
        }

        public ActionResult AddIcon(int id)
        {
            var course = DB.Courses.Find(id);
            ViewBag.course = course;
            ViewBag.Title = "Add icon";
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AddIcon(int id, HttpPostedFileBase file)
        {
            var course = DB.Courses.Find(id);
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
            course.IconID = attachment.ID;
            DB.Attachments.Add(attachment);
            DB.SaveChanges();
            return RedirectToAction(nameof(Details), new { course.ID });
        }

        [AllowAnonymous]
        public FileResult Icon(int id)
        {
            var course = DB.Courses.Find(id);
            if (course.IconID.HasValue)
            {
                var attachment = course.Attachment;
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