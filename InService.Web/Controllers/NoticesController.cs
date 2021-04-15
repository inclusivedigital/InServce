using InService.Data;
using InService.Web.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InService.Web.Controllers
{
    public class NoticesController : SysController
    {
        // GET: Notice
        public ActionResult Index(int? p, bool? withImages)
        {
            var query = DB.Notices.AsQueryable();
            if (withImages.HasValue) query = query.Where(c => c.Description.Contains("<img") || c.Description.Contains("img"));
            ViewBag.Title = "Notices";
            return View(query.OrderByDescending(c => c.CreationDate).ToPagedList(p ?? 1, 1000));
        }

        public ActionResult Details(Guid id)
        {
            var content = DB.Notices.Find(id);
            ViewBag.Title = "Notice";
            return View(content);
        }
        public ActionResult Add()
        {
            ViewBag.Title = "Add content";
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Add(Notice article, HttpPostedFileBase file)
        {
            article.CreationDate = DateTime.UtcNow;
            article.CreatorID = CurrentUserID;
            var attachments = article.Attachments;
            article.ID = Guid.NewGuid();
            foreach (var item in DB.Attachments)
            {
                if (article.Description.Contains($"{item.ID}")) attachments.Add(item.IAttachmentJson);
            }
            article.AttachmentsJson = new HtmlString(attachments.ToJSONString()).ToHtmlString();
            if (file != null && file.ContentLength > 0)
            {
                var attachment = new Attachment
                {
                    CreatorID = CurrentUserID,
                    Description = "Article data",
                    Extension = Path.GetExtension(file.FileName),
                    ID = Guid.NewGuid(),
                    Name = file.FileName,
                    Size = file.ContentLength,
                    UploadDate = DateTime.UtcNow,
                    IsIcon = false,
                    IsArticle = true,
                };
                string mainDir = attachment.Path;
                if (!Directory.Exists(mainDir))
                    Directory.CreateDirectory(mainDir);
                file.SaveAs(Path.Combine(mainDir, $"{attachment.ID}{attachment.Extension}"));
                article.AttachmentID = attachment.ID;
                DB.Attachments.Add(attachment);
            }
            DB.Notices.Add(article);
            DB.SaveChanges();
            return RedirectToAction(nameof(Details), new { article.ID });
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UploadFile(HttpPostedFileBase aUploadedFile)
        {
            string sImageName = string.Empty;
            if (aUploadedFile.ContentLength > 0)
            {
                var attachment = new Attachment
                {
                    ID = Guid.NewGuid(),
                    CreatorID = CurrentUserID,
                    Description = aUploadedFile.FileName,
                    Name = aUploadedFile.FileName,
                    Extension = Path.GetExtension(aUploadedFile.FileName),
                    Size = aUploadedFile.ContentLength,
                    UploadDate = DateTime.UtcNow,
                };
                string mainDir = attachment.Path;
                if (!Directory.Exists(mainDir))
                    Directory.CreateDirectory(mainDir);

                sImageName = attachment.ID.ToString();
                var vImageSavePath = Path.Combine(mainDir, $"{attachment.ID}{attachment.Extension}");
                ViewBag.Msg = vImageSavePath;
                var path = vImageSavePath;
                aUploadedFile.SaveAs(path);
                DB.Attachments.Add(attachment);
                DB.SaveChanges();
                TempData["message"] = string.Format("Image was Added Successfully");
            }
            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = sImageName,
            };
        }

        [AllowAnonymous]
        public FileResult Image(string data)
        {
            if (!string.IsNullOrWhiteSpace(data))
            {
                var id = Guid.Parse(data);
                var attachment = DB.Attachments.Find(id);
                string mainDir = attachment.Path;
                if (Directory.Exists(mainDir))
                {
                    var file = Directory.EnumerateFiles(mainDir).FirstOrDefault(c => c.Contains($"{attachment.ID}"));
                    if (file != null) return File(file, $"image/{new FileInfo(file).Extension.Replace(".", "")}");
                }
            }
            return File("~/Notice/placeholder.png", "image/png");
        }

        public ActionResult ChangeStatus(int Id, int StatusID)
        {
            var article = DB.Notices.Find(Id);
            article.StatusID = StatusID;
            DB.SaveChanges();
            return RedirectToAction(nameof(Details), new { article.ID });
        }

        public ActionResult Edit(Guid id)
        {
            var content = DB.Notices.Find(id);
            ViewBag.Title = "Notice";
            return View(content);
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false), ActionName(nameof(Edit))]
        public ActionResult Update(Guid id)
        {
            var article = DB.Notices.Find(id);
            var attachments = article.Attachments;
            if (TryUpdateModel(article))
            {
                foreach (var item in DB.Attachments)
                {
                    if (article.Description.Contains($"{item.ID}")) attachments.Add(item.IAttachmentJson);
                }
                article.AttachmentsJson = new HtmlString(attachments.ToJSONString()).ToHtmlString();
                DB.SaveChanges();
            }
            return RedirectToAction(nameof(Details), new { article.ID });
        }


        public ActionResult AddAttachmentJson()
        {
            foreach (var article in DB.Notices)
            {
                var attachments = article.Attachments;
                foreach (var item in DB.Attachments)
                {
                    if (article.Description.Contains($"{item.ID}")) attachments.Add(item.IAttachmentJson);
                }
                article.AttachmentsJson = new HtmlString(attachments.ToJSONString()).ToHtmlString();
            }
            DB.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public ActionResult AddAttachment(Guid id)
        {
            var article = DB.Notices.Find(id);
            ViewBag.article = article;
            ViewBag.Title = $"Add attachment: {article.Heading}";
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AddAttachment(Guid id, HttpPostedFileBase file)
        {
            var article = DB.Notices.Find(id);
            var attachment = new Attachment
            {
                CreatorID = CurrentUserID,
                Description = "Article data",
                Extension = Path.GetExtension(file.FileName),
                ID = Guid.NewGuid(),
                Name = file.FileName,
                Size = file.ContentLength,
                UploadDate = DateTime.UtcNow,
                IsIcon = false,
                IsArticle = true,
            };
            string mainDir = attachment.Path;
            if (!Directory.Exists(mainDir))
                Directory.CreateDirectory(mainDir);
            file.SaveAs(Path.Combine(mainDir, $"{attachment.ID}{attachment.Extension}"));
            article.AttachmentID = attachment.ID;
            DB.Attachments.Add(attachment);
            DB.SaveChanges();
            return RedirectToAction(nameof(Details), new { article.ID });
        }
    }
}