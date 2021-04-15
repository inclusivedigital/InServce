using InService.Data;
using InService.Lib.Auth;
using InService.Web.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace InService.Web.Controllers
{
    public class ExaminationsController : SysController
    {
        [AllowAnonymous]
        public ActionResult Index(int? p, int? CID, int? VID)
        {
            var query = DB.Examinations.ToList().AsQueryable();
            if (CID.HasValue)
            {
                var course = DB.Courses.Find(CID);
                ViewBag.course = course;
                query = query.Where(c => c.CourseID == CID);
            }
            if (VID.HasValue)
            {
                query = query.Where(c => c.ValueChainID == VID);
            }

            ViewBag.CID = CID;
            ViewBag.VID = VID;
            ViewBag.Title = "Examinations";
            return View(new PagedList.PagedList<Examination>(query.OrderByDescending(c => c.CreationDate), p ?? 1, DefaultPageSize));
        }

        public ActionResult Details(int id)
        {
            var examination = DB.Examinations.Find(id);
            ViewBag.Title = "Examination";
            ViewBag.currencies = DB.Currencies.OrderBy(c => c.Name);
            ViewBag.paymentmethods = DB.PaymentMethods.OrderBy(c => c.Name);
            var ExchngeRates = DB.ExchangeRates.OrderByDescending(c => c.CreationDate);
            ViewBag.ExchngeRates = ExchngeRates;
            ViewBag.instructions = DB.Instructions.FirstOrDefault(c => c.ExamTypeID == examination.TypeID);
            return View(examination);
        }

        public ActionResult Edit(int id)
        {
            var examination = DB.Examinations.Find(id);
            ViewBag.Title = "Examinations";
            ViewBag.courses = new SelectList(DB.Courses.OrderBy(c => c.Name), nameof(Course.ID), nameof(Course.Name), examination.CourseID);
            ViewBag.modules = new SelectList(DB.Modules.OrderBy(c => c.Name), nameof(Module.ID), nameof(Module.Name), examination.ModuleID);
            ViewBag.valuechains = new SelectList(DB.ValueChains.OrderBy(c => c.Name), nameof(ValueChain.ID), nameof(ValueChain.Name), examination.ValueChainID);
            return View(examination);
        }

        [ValidateAntiForgeryToken, HttpPost, ActionName(nameof(Edit))]
        public ActionResult Update(int id, int? Hours, int? Minutes, DateTime? StartTime, DateTime? EndTime, List<int> FlagsID, List<int> AudienceID)
        {
            var examination = DB.Examinations.Find(id);
            int d = ((Hours ?? 0) * 60) + (Minutes ?? 0);
            if (d == 0) ModelState.AddModelError(string.Empty, "Invalid duration");
            examination.Duration = d;
            if (TryUpdateModel(examination))
            {
                if (StartTime.HasValue) examination.StartDate = examination.StartDate.Date.Add(StartTime.Value - StartTime.Value.Date);
                if (EndTime.HasValue) examination.EndDate = examination.EndDate.Date.Add(EndTime.Value - EndTime.Value.Date);
                if (FlagsID != null)
                {
                    examination.FlagsID = 0;
                    examination.FlagsID = FlagsID.Sum(c => c);
                }
                if (AudienceID != null)
                {
                    examination.TargetAudienceID = 0;
                    examination.TargetAudienceID = AudienceID.Sum(c => c);
                }
                DB.SaveChanges();
                return RedirectToAction(nameof(Details), new { examination.ID });
            }
            else
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {

                    }
                }
            }
            ViewBag.Title = "Examinations";
            ViewBag.courses = new SelectList(DB.Courses.OrderBy(c => c.Name), nameof(Course.ID), nameof(Course.Name), examination.CourseID);
            ViewBag.modules = new SelectList(DB.Modules.OrderBy(c => c.Name), nameof(Module.ID), nameof(Module.Name), examination.ModuleID);
            ViewBag.valuechains = new SelectList(DB.ValueChains.OrderBy(c => c.Name), nameof(ValueChain.ID), nameof(ValueChain.Name), examination.ValueChainID);
            return View(examination);
        }

        public ActionResult Add()
        {
            ViewBag.courses = new SelectList(DB.Courses.OrderBy(c => c.Name), nameof(Course.ID), nameof(Course.Name));
            ViewBag.modules = new SelectList(DB.Modules.OrderBy(c => c.Name), nameof(Module.ID), nameof(Module.Name));
            ViewBag.valuechains = new SelectList(DB.ValueChains.OrderBy(c => c.Name), nameof(ValueChain.ID), nameof(ValueChain.Name));
            ViewBag.Title = "Add new examination";
            return View();
        }


        [ValidateAntiForgeryToken, HttpPost]
        public ActionResult Add(Examination examination, int? Hours, int? Minutes, DateTime? StartTime, DateTime? EndTime, List<int> FlagsID, List<int> AudienceID)
        {
            if (StartTime.HasValue) examination.StartDate = examination.StartDate.Date.Add(StartTime.Value - StartTime.Value.Date);
            if (EndTime.HasValue) examination.EndDate = examination.EndDate.Date.Add(EndTime.Value - EndTime.Value.Date);
            examination.CreationDate = DateTime.Now;
            examination.CreatorID = CurrentUserID;
            examination.Year = DateTime.Now.Year;
            if (examination.ModuleID.HasValue)
            {
                var module = DB.Modules.Find(examination.ModuleID);
                examination.Topic = module.Name;
            }
            else examination.Topic = AccountGenerator(6);
            examination.FlagsID = 0;
            if (FlagsID != null) examination.FlagsID = FlagsID.Sum(c => c);
            int d = (Hours ?? 0) * 60;
            d += Minutes ?? 0;
            if (d == 0) ModelState.AddModelError(string.Empty, "Invalid duration");
            examination.Duration = d;
            examination.TargetAudienceID = 0;
            if (AudienceID != null) examination.TargetAudienceID = AudienceID.Sum(c => c);
            //   if (ModelState.IsValid)
            //  {
            DB.Examinations.Add(examination);
            DB.SaveChanges();
            return RedirectToAction("Add", "Questions", new { examination.ID });
            // }
            //ViewBag.courses = new SelectList(DB.Courses.OrderBy(c => c.Name), nameof(Course.ID), nameof(Course.Name), examination.CourseID);
            //ViewBag.modules = new SelectList(DB.Modules.OrderBy(c => c.Name), nameof(Module.ID), nameof(Module.Name), examination.ModuleID);
            //ViewBag.valuechains = new SelectList(DB.ValueChains.OrderBy(c => c.Name), nameof(ValueChain.ID), nameof(ValueChain.Name), examination.ValueChainID);
            //ViewBag.Title = "Add new examination";
            //return View();
        }

        public ActionResult AddAttachment(int id)
        {
            var examination = DB.Examinations.Find(id);
            ViewBag.Title = "Add attachment";
            ViewBag.examination = examination;
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AddAttachment(Attachment attachment, int id, HttpPostedFileBase file)
        {
            var examination = DB.Examinations.Find(id);
            if (file != null)
            {
                // string mainDir = Server.MapPath(attachment.Path);
                string mainDir = attachment.Path;
                if (!Directory.Exists(mainDir))
                    Directory.CreateDirectory(mainDir);
                attachment.Size = file.ContentLength;
                attachment.Extension = Path.GetExtension(file.FileName);
                attachment.UploadDate = DateTime.UtcNow;
                var today = DateTime.Now;
                DB.Attachments.Add(attachment);
                var attachments = examination.Attachments;
                attachments.Add(attachment);
                DB.SaveChanges();
                examination.AttachmentsJson = attachments.ToJSONString();
                DB.SaveChanges();
                file.SaveAs(Path.Combine(mainDir, $"{attachment.ID}{Path.GetExtension(file.FileName)}"));
            }
            return RedirectToAction(nameof(Details), new { examination.ID });
        }

        public ActionResult DeleteAttachment(List<int> AID, int id)
        {
            var examination = DB.Examinations.Find(id);
            foreach (var i in AID)
            {
                var attach = DB.Attachments.Find(i);
                // string mainDir = Server.MapPath(attach.Path);
                string mainDir = attach.Path;
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
                var attachments = examination.Attachments;
                var newattachments = attachments.Remove(attach);
                DB.Attachments.Remove(attach);
                examination.AttachmentsJson = newattachments.ToJSONString();
            }
            DB.SaveChanges();
            return RedirectToAction(nameof(Details), new { examination.ID });
        }

        public ActionResult Instructions(int id)
        {
            var examination = DB.Examinations.Find(id);
            ViewBag.examination = examination;
            ViewBag.instructions = DB.Instructions.FirstOrDefault(c => c.ExamTypeID == examination.TypeID);
            ViewBag.Title = "Examination instructions";
            ViewBag.User = CurrentUser;
            return View();
        }

        public ActionResult Delete(int id)
        {
            var examination = DB.Examinations.Find(id);
            ViewBag.Title = "Delete examination";
            ViewBag.currencies = DB.Currencies.OrderBy(c => c.Name);
            ViewBag.paymentmethods = DB.PaymentMethods.OrderBy(c => c.Name);
            var ExchngeRates = DB.ExchangeRates.OrderByDescending(c => c.CreationDate);
            ViewBag.ExchngeRates = ExchngeRates;
            return View(examination);
        }

        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Delete))]
        public ActionResult DeleteConformed(int id)
        {
            var examination = DB.Examinations.Find(id);
            if (examination.Questions.Any())
            {
                foreach (var qustion in examination.Questions)
                    if (qustion.Answers.Any()) DB.Answers.RemoveRange(qustion.Answers);
                DB.Questions.RemoveRange(examination.Questions);
            }
            if (examination.ExaminationPrices.Any()) DB.ExaminationPrices.RemoveRange(examination.ExaminationPrices);
            if (examination.UserDeductions.Any()) DB.UserDeductions.RemoveRange(examination.UserDeductions);
            if (examination.UserExaminations.Any())
            {
                foreach (var item in examination.UserExaminations)
                    if (item.UserExaminationDetails.Any()) DB.UserExaminationDetails.RemoveRange(item.UserExaminationDetails);
                DB.UserExaminations.RemoveRange(examination.UserExaminations);
            }
            DB.Examinations.Remove(examination);
            DB.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}

