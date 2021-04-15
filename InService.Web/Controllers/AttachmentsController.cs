using InService.Data;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InService.Web.Controllers
{
    public class AttachmentsController : SysController
    {
        public ActionResult Index(int? p)
        {
            var query = DB.Attachments.AsQueryable();
            ViewBag.Title = "Attachments";
            return View(new PagedList.PagedList<Attachment>(query.OrderByDescending(c => c.UploadDate), p ?? 1, DefaultPageSize));
        }

        [AllowAnonymous]
        public ActionResult Add()
        {
            ViewBag.Title = "Add attachment";
            return View();
        }

        public ActionResult Details(Guid id)
        {
            var attachment = DB.Attachments.Find(id);
            ViewBag.Title = "Attachment details";
            return View(attachment);
        }

        [HttpPost, ValidateAntiForgeryToken, AllowAnonymous]
        public ActionResult Add(Attachment attachment, HttpPostedFileBase file)
        {
            attachment.UploadDate = DateTime.Now;
            attachment.CreatorID = CurrentUserID;
            attachment.ID = Guid.NewGuid();
            if (file != null)
            {
                // string mainDir = Server.MapPath(attachment.Path);
                string mainDir = attachment.Path;
                if (!Directory.Exists(mainDir))
                    Directory.CreateDirectory(mainDir);
                attachment.Size = file.ContentLength;
                attachment.Extension = Path.GetExtension(file.FileName);
                DB.SaveChanges();
                file.SaveAs(Path.Combine(mainDir, $"{attachment.ID}{Path.GetExtension(file.FileName)}"));
            }
            return RedirectToAction(nameof(Details), new { attachment.ID });
        }

        [AllowAnonymous]
        public FileResult Download(Guid ID)
        {
            var document = DB.Attachments.Find(ID);
            string path = null, filename = null;
            byte[] fileBytes = null;
            // string mainDir = Server.MapPath(document.Path);
            string mainDir = document.Path;
            path = Path.Combine(mainDir, $"{document.ID}{document.Extension}");
            fileBytes = System.IO.File.ReadAllBytes(path);
            filename = $"{document.ID}{document.Extension}";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
        }

        [AllowAnonymous]
        public FileResult Source(Guid ID)
        {
            var document = DB.Attachments.Find(ID);
            // string mainDir = Server.MapPath(document.Path);
            string mainDir = document.Path;
            if (Directory.Exists(mainDir))
            {
                var file = Directory.EnumerateFiles(mainDir).FirstOrDefault(c => c.Contains(document.ID.ToString()));
                if (file != null) return File(file, $"{System.Net.Mime.MediaTypeNames.Application.Octet}/{new FileInfo(file).Extension.Replace(".", "")}");
            }
            return File("~/Content/images/placeholder.png", "image/png");
        }

        public ActionResult Edit(Guid id)
        {
            var attachment = DB.Attachments.Find(id);
            ViewBag.Title = "Edit attachment";
            return View(attachment);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Edit))]
        public ActionResult Update(Guid id, HttpPostedFileBase file)
        {
            var attachment = DB.Attachments.Find(id);
            if (TryUpdateModel(attachment))
            {

                DB.SaveChanges();
                if (file != null)
                {
                    // string mainDir = Server.MapPath(attachment.Path);
                    string mainDir = attachment.Path;
                    if (!Directory.Exists(mainDir))
                        Directory.CreateDirectory(mainDir);
                    string path = System.IO.Path.Combine(mainDir, System.IO.Path.GetFileName(file.FileName));
                    attachment.Size = file.ContentLength;
                    attachment.Extension = Path.GetExtension(path).Replace('.', ' ').Trim();
                    file.SaveAs(Path.Combine(mainDir, $"{attachment.ID}{Path.GetExtension(file.FileName)}"));
                }
                return RedirectToAction(nameof(Details), new { attachment.ID });
            }
            ViewBag.Title = "Edit attachment";
            return View(attachment);
        }


    }
}