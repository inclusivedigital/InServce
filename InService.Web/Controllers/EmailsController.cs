using InService.Data;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using InService.Lib.Auth;
using InService.Web.Models;

namespace InService.Web.Controllers
{
    [Authorize]
    public class EmailsController : SysController
    {
        public ActionResult Index(int? p)
        {
            var query = DB.EmailConfigs.AsQueryable();
            ViewBag.Title = "Emails";
            return View(new PagedList.PagedList<EmailConfig>(query.OrderBy(c => c.Name), p ?? 1, DefaultPageSize));
        }

        public ActionResult Details(int ID)
        {
            var config = DB.EmailConfigs.Find(ID);
            ViewBag.Title = $"Email config: {config.Name}";
            ViewBag.CurUserEmail = CurrentUser.Email ?? "mapikuw@gmail.com";
            return View(config);
        }

        public ActionResult Add()
        {
            ViewBag.Title = "Add new email config..";
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Add(EmailConfig config, List<int> Targets, string Password)
        {
            config.CreationDate = DateTime.UtcNow;
            config.CreatorID = CurrentUserID;
            config.StatusID = 1;
            if (Targets != null) Targets.ForEach(c => config.TargetID |= c);
            config.ComputeHash(Password);
            DB.EmailConfigs.Add(config);
            DB.SaveChanges();
            return RedirectToAction(nameof(Details), new { config.ID });
        }

        public ActionResult Edit(int ID)
        {
            var config = DB.EmailConfigs.Find(ID);
            ViewBag.Title = $"Edit email config: {config.Name}";
            return View(config);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(int ID, string Password, List<int> Targets)
        {
            var config = DB.EmailConfigs.Find(ID);
            var pwd = config.Hash.GetPassword(config.Host.ToLower().Trim());
            if (TryUpdateModel(config, new string[] { nameof(EmailConfig.Name), nameof(EmailConfig.Host), nameof(EmailConfig.Port), nameof(EmailConfig.SenderID), nameof(EmailConfig.Username), nameof(EmailConfig.EnableSSL) }))
            {
                config.ComputeHash(string.IsNullOrEmpty(Password) ? pwd : Password);
                config.Target = 0;
                if (Targets != null) Targets.ForEach(c => config.TargetID |= c);
                DB.SaveChanges();
                return RedirectToAction(nameof(Details), new { ID });
            }
            ViewBag.Title = $"Edit email config: {config.Name}";
            return View(config);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Test(int ID, string Email)
        {
            var config = DB.EmailConfigs.Find(ID);
            ViewBag.Title = $"Test email configuration: {config.Name}";
            var sender = new EmailSender(config);
            ViewBag.Email = Email;
            ViewBag.Exception = await sender.SendEmail("Test message from In service", $"Test email from In service", Email);
            return View(config);
        }
    }
}