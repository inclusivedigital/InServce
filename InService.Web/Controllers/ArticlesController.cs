using InService.Data;
using InService.Lib.Data;
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
    public class ArticlesController : SysController
    {
        // GET: Article
        public ActionResult Index(int? p, bool? withImages)
        {
            var query = DB.Articles.AsQueryable();
            if (withImages.HasValue) query = query.Where(c => c.Description.Contains("<img") || c.Description.Contains("img"));
            ViewBag.Title = "Articles";
            return View(query.OrderByDescending(c => c.CreationDate).ToPagedList(p ?? 1, DefaultPageSize));
        }

        public ActionResult Details(int id)
        {
            var content = DB.Articles.Find(id);
            ViewBag.Title = "Article";
            return View(content);
        }
        public ActionResult Add(int? ModuleID, int? CourseID, int? ValueChainID)
        {
            ViewBag.Title = "Add content";
            if (ModuleID.HasValue) ViewBag.module = DB.Modules.Find(ModuleID);
            if (CourseID.HasValue) ViewBag.course = DB.Courses.Find(CourseID);
            if (ValueChainID.HasValue) ViewBag.valuechains = DB.Courses.Find(ValueChainID);
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Add(Article article, HttpPostedFileBase file)
        {
            article.CreationDate = DateTime.UtcNow;
            article.CreatorID = CurrentUserID;
            var attachments = new List<IAttachmentJson>();
            if (!string.IsNullOrWhiteSpace(article.Description))
            {
                foreach (var item in DB.Attachments)
                {
                    if (article.Description.Contains($"{item.ID}")) attachments.Add(new IAttachmentJson { Description = item.Description, Extension = item.Extension, ID = item.ID, Name = item.Name, Size = item.Size, UploadDate = item.UploadDate });
                }
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
            DB.Articles.Add(article);
            DB.SaveChanges();
            return RedirectToAction(nameof(Details), new { article.ID });
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult UploadFile(HttpPostedFileBase aUploadedFile)
        {
            // var vReturnImagePath = string.Empty;
            string sImageName = string.Empty;
            if (aUploadedFile.ContentLength > 0)
            {
                // var vFileName = Path.GetFileNameWithoutExtension(aUploadedFile.FileName);
                // var vExtension = Path.GetExtension(aUploadedFile.FileName);
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

                //  sImageName = vFileName + DateTime.Now.ToString("YYYYMMDDHHMMSS");
                sImageName = attachment.ID.ToString();
                // var vImageSavePath = Path.Combine(Server.MapPath("~/UpImages/"), sImageName + vExtension);
                //  var mainDir = @"C:\InService\Uploads\";
                // if (!Directory.Exists(mainDir)) Directory.CreateDirectory(mainDir);
                var vImageSavePath = Path.Combine(mainDir, $"{attachment.ID}{attachment.Extension}");
                //sImageName = sImageName + vExtension;  
                //  vReturnImagePath = vImageSavePath;// "~/UpImages/" + sImageName + vExtension;
                ViewBag.Msg = vImageSavePath;
                var path = vImageSavePath;
                // Saving Image in Original Mode  
                aUploadedFile.SaveAs(path);
                //var vImageLength = new FileInfo(path).Length;
                //here to add Image Path to You Database , 
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
                //  string mainDir = @"C:\InService\Uploads\";// Server.MapPath("~/UpImages/");
                string mainDir = attachment.Path;
                if (Directory.Exists(mainDir))
                {
                    var file = Directory.EnumerateFiles(mainDir).FirstOrDefault(c => c.Contains($"{attachment.ID}"));
                    if (file != null) return File(file, $"image/{new FileInfo(file).Extension.Replace(".", "")}");
                }
            }
            return File("~/Article/placeholder.png", "image/png");
        }

        public ActionResult ChangeStatus(int Id, int StatusID)
        {
            var article = DB.Articles.Find(Id);
            article.FlagsID = StatusID;
            DB.SaveChanges();
            return RedirectToAction(nameof(Details), new { article.ID });
        }
        public ActionResult SetDefault(int Id)
        {
            foreach (var article in DB.Articles) article.IsDefault = false;
            var articlew = DB.Articles.Find(Id);
            articlew.IsDefault = true;
            DB.SaveChanges();
            return RedirectToAction(nameof(Details), new { articlew.ID });
        }

        public ActionResult Edit(int id)
        {
            var content = DB.Articles.Find(id);
            ViewBag.Title = "Article";
            return View(content);
        }

        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false), ActionName(nameof(Edit))]
        public ActionResult Update(int id, HttpPostedFileBase file)
        {
            var article = DB.Articles.Find(id);
            var attachments = new List<IAttachmentJson>();
            if (TryUpdateModel(article))
            {
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
                if (!string.IsNullOrWhiteSpace(article.Description))
                {
                    foreach (var item in DB.Attachments)
                    {
                        if (article.Description.Contains($"{item.ID}")) attachments.Add(new IAttachmentJson { Description = item.Description, Extension = item.Extension, ID = item.ID, Name = item.Name, Size = item.Size, UploadDate = item.UploadDate });
                    }
                }
                article.AttachmentsJson = new HtmlString(attachments.ToJSONString()).ToHtmlString();
                //    DB.Articles.Add(article);
                DB.SaveChanges();
            }
            return RedirectToAction(nameof(Details), new { article.ID });
        }

        public ActionResult MigrateFiles()
        {
            var oldpath = @"C:\InService\Uploads\";
            if (!Directory.Exists(oldpath)) Directory.CreateDirectory(oldpath);
            DirectoryInfo dir = new DirectoryInfo(oldpath);
            foreach (FileInfo file in dir.GetFiles())
            {
                var attachment = new Attachment
                {
                    ID = Guid.NewGuid(),
                    CreatorID = CurrentUserID,
                    Description = file.Name,
                    Extension = file.Extension,
                    Name = file.Name,
                    Size = (int)file.Length,
                    UploadDate = DateTime.UtcNow,
                };
                DB.Attachments.Add(attachment);
                string mainDir = attachment.Path;
                if (!Directory.Exists(mainDir))
                    Directory.CreateDirectory(mainDir);
                var path = Path.Combine(mainDir, $"{attachment.ID}{attachment.Extension}");
                System.IO.File.Copy(file.FullName, path, true);

                // file.MoveTo(path);
                // file.IsReadOnly = false;
                // file.Delete();
            }
            DB.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public ActionResult RenameFilePath()
        {
            foreach (var article in DB.Articles)
            {
                foreach (var attachment in DB.Attachments)
                {
                    var og = attachment.Name.Split('.')[0];
                    article.Description = article.Description.Replace(og, attachment.ID.ToString());
                }
            }
            DB.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public ActionResult CorrectPaths()
        {
            foreach (var article in DB.Articles)
            {
                var og = "/Articles/";
                var newp = "/InService/Articles/";
                article.Description = article.Description.Replace(newp, og);
            }
            DB.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public ActionResult AddAttachmentJson()
        {
            foreach (var article in DB.Articles)
            {
                var attachments = new List<IAttachmentJson>();
                foreach (var item in DB.Attachments)
                {
                    if (article.Description.Contains($"{item.ID}")) attachments.Add(new IAttachmentJson { Description = item.Description, Extension = item.Extension, ID = item.ID, Name = item.Name, Size = item.Size, UploadDate = item.UploadDate });
                }
                article.AttachmentsJson = new HtmlString(attachments.ToJSONString()).ToHtmlString();
            }
            DB.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public ActionResult SetIconFlags()
        {
            foreach (var branch in DB.Branches)
            {
                if (branch.IconID.HasValue)
                {
                    var attachment = branch.Attachment;
                    attachment.IsIcon = true;
                }
            }
            foreach (var branch in DB.Courses)
            {
                if (branch.IconID.HasValue)
                {
                    var attachment = branch.Attachment;
                    attachment.IsIcon = true;
                }
            }
            foreach (var branch in DB.Modules)
            {
                if (branch.IconID.HasValue)
                {
                    var attachment = branch.Attachment;
                    attachment.IsIcon = true;
                }
            }
            foreach (var branch in DB.Crops)
            {
                if (branch.IconID.HasValue)
                {
                    var attachment = branch.Attachment;
                    attachment.IsIcon = true;
                }
            }
            foreach (var branch in DB.Livestocks)
            {
                if (branch.IconID.HasValue)
                {
                    var attachment = branch.Attachment;
                    attachment.IsIcon = true;
                }
            }
            foreach (var branch in DB.CropCategories)
            {
                if (branch.IconID.HasValue)
                {
                    var attachment = branch.Attachment;
                    attachment.IsIcon = true;
                }
            }
            foreach (var branch in DB.LivestockCategories)
            {
                if (branch.IconID.HasValue)
                {
                    var attachment = branch.Attachment;
                    attachment.IsIcon = true;
                }
            }
            DB.SaveChanges();
            return RedirectToAction(nameof(Index));
        }





        public ActionResult AddAttachment(int id)
        {
            var article = DB.Articles.Find(id);
            ViewBag.article = article;
            ViewBag.Title = $"Add attachment: {article.Name ?? article.Module.Name}";
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AddAttachment(int id, HttpPostedFileBase file)
        {
            var article = DB.Articles.Find(id);
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