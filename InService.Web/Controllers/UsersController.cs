using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using InService.Lib.Auth;
using InService.Data;
using PagedList;
using PagedList.Mvc;
using InService.Web.Auth;
using InService.Lib;
using InService.Web.Models;

namespace InService.Web.Controllers
{
    [Authorize]
    public class UsersController : SysController
    {
        public ActionResult Index(int? p, string q)
        {
            var query = DB.Users.AsQueryable();
            if (!String.IsNullOrEmpty(q)) query = query.Where(c => c.Name.Contains(q) || c.LoginID.Contains(q));
            ViewBag.Title = "Users";
            return View(query.OrderByDescending(c => c.CreationDate).ToPagedList(p ?? 1, DefaultPageSize));
        }

        public ActionResult Details(int? id)
        {
            User user = DB.Users.Find(id);
            ViewBag.Title = $"User details: {user.Name}";
            return View(user);
        }

        public ActionResult Add()
        {
            ViewBag.Countries = new SelectList(DB.Countries.OrderBy(c => c.Name), nameof(Country.ID), nameof(Country.Name));
            ViewBag.Title = "Add user";
            return View();
        }

        [ValidateAntiForgeryToken, HttpPost]
        public ActionResult Add(User user, List<int> Rights)
        {
            user.CreatorID = CurrentUserID;
            user.CreationDate = DateTime.UtcNow;
            user.StatusID = (int)UserStatus.ACTIVE;
            if (DB.Users.Any(c => c.LoginID == user.LoginID)) ModelState.AddModelError(nameof(user.LoginID), $"User {user.LoginID} already exists. Register a different user name.");
            //if (Rights == null) ModelState.AddModelError(string.Empty, "Give access rights to this user!");
            if (user.RoleID == 0) ModelState.AddModelError(nameof(user.Role), "User role is required!");
            user.AccessRightID = 0;
            if (Rights != null) Rights.ForEach(r => user.AccessRightID += r);
            if (ModelState.IsValid)
            {
                DB.Users.Add(user);
                DB.SaveChanges();
                return RedirectToAction(nameof(Details), new { user.ID });
            }
            ViewBag.Title = "Add user";
            ViewBag.Countries = new SelectList(DB.Countries.OrderBy(c => c.Name), nameof(Country.ID), nameof(Country.Name), user.CountryID);
            return View(user);
        }

        [AllowAnonymous]
        public ActionResult Edit(int? id)
        {
            var user = DB.Users.Find(id);
            ViewBag.Title = $"Edit user: {user.Name}";
            ViewBag.Countries = new SelectList(DB.Countries.OrderBy(c => c.Name), nameof(Country.ID), nameof(Country.Name), user.CountryID);
            return View(user);
        }

        [AllowAnonymous, ValidateAntiForgeryToken, HttpPost, ActionName(nameof(Edit))]
        public ActionResult Update(int id, List<int> Rights)
        {
            var user = DB.Users.Find(id);
            user.AccessRightID = 0;
            if (Rights != null) Rights.ForEach(c => user.AccessRightID += c);
            if (ModelState.IsValid)
            {
                if (TryUpdateModel(user, new string[] { nameof(Data.User.Name), nameof(Data.User.LoginID), nameof(Data.User.Hash), nameof(Data.User.Email), nameof(Data.User.Mobile), nameof(Data.User.RoleID), nameof(Data.User.CountryID), nameof(Data.User.ProvinceID), nameof(Data.User.DistrictID), nameof(Data.User.WardID), nameof(Data.User.AccessRightID) }))
                {
                    DB.SaveChanges();
                    return RedirectToAction(nameof(Details), new { user.ID });
                }
            }
            ViewBag.Title = $"Edit user: {user.Name}";
            ViewBag.Countries = new SelectList(DB.Countries.OrderBy(c => c.Name), nameof(Country.ID), nameof(Country.Name), user.CountryID);
            return View(user);
        }


        public ActionResult Unlock(int id)
        {
            var user = DB.Users.Find(id);
            user.Hash = null;
            user.StatusID = (int)UserStatus.ACTIVE;
            if (TryUpdateModel(user)) DB.SaveChanges();
            return RedirectToAction(nameof(Details), new { user.ID });
        }

        public ActionResult Lock(int id)
        {
            var user = DB.Users.Find(id);
            user.Hash = null;
            user.StatusID = (int)UserStatus.BLOCKED;
            if (TryUpdateModel(user)) DB.SaveChanges();
            return RedirectToAction(nameof(Details), new { user.ID });
        }

        public ActionResult Reset()
        {
            ViewBag.Title = "Reset my password";
            return View();
        }

        [AllowAnonymous, HttpPost, ValidateAntiForgeryToken]
        public ActionResult Reset(ResetViewModel model)
        {
            if (ModelState.IsValid)
            {
                var myid = User.Identity.GetUserId<int>();
                var user = DB.Users.Find(myid);
                user.Hash = InServiceIUser.GetPasswordHash(user.LoginID, model.Password);
                if (TryUpdateModel(user))
                {
                    DB.SaveChanges();
                    Request.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    return RedirectToAction(nameof(ResetDone));
                }
            }
            ViewBag.Title = "Reset";
            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetDone()
        {
            ViewBag.Title = "Password reset success";
            return View();
        }

        public ActionResult Delete(int? id)
        {
            var i = DB.Users.Find(id);
            ViewBag.Title = "Delete user";
            return View(i);
        }

        [ValidateAntiForgeryToken, HttpPost, ActionName(nameof(Delete))]
        public ActionResult DeleteConfirmed(int id)
        {
            var i = DB.Users.Find(id);
            DB.Users.Remove(i);
            DB.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}


