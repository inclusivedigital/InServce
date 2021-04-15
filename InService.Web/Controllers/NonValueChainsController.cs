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
    public class NonValueChainsController : SysController
    {
        public ActionResult Index(int? p)
        {
            var query = DB.NonValueChains.AsQueryable();
            ViewBag.Title = "Non value chains";
            return View(new PagedList.PagedList<NonValueChain>(query.OrderBy(c => c.Name), p ?? 1, DefaultPageSize));
        }

        [AllowAnonymous]
        public ActionResult Details(int id)
        {
            var valueChain = DB.NonValueChains.Find(id);
            ViewBag.Title = $"{valueChain.Name}";
            return View(valueChain);
        }

        public ActionResult Remove(int id)
        {
            var valueChain = DB.NonValueChains.Find(id);
            DB.NonValueChains.Remove(valueChain);
            DB.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public ActionResult Add(int? BranchID)
        {
            ViewBag.Title = "Add a non value Chain";
            ViewBag.branches = new SelectList(DB.Branches.OrderBy(c => c.Name), nameof(Branch.ID), nameof(Branch.Name), BranchID);
            if (DB.NonValueChains.Any()) ViewBag.parents = new SelectList(DB.NonValueChains.OrderBy(c => c.Name), nameof(NonValueChain.ID), nameof(NonValueChain.Name));
            return View();
        }

        [ValidateAntiForgeryToken, HttpPost]
        public ActionResult Add(NonValueChain valueChain, HttpPostedFileBase file)
        {
            valueChain.CreatorID = User.Identity.GetUserId<int>();
            valueChain.CreationDate = DateTime.UtcNow;
            if (DB.NonValueChains.Any(c => c.Name.Contains(valueChain.Name))) ModelState.AddModelError(nameof(valueChain.Name), "The value Chain and code already exists");
            if (ModelState.IsValid)
            {
                DB.NonValueChains.Add(valueChain);
                DB.SaveChanges();
                return RedirectToAction(nameof(Details), new { valueChain.ID });
            }
            if (DB.NonValueChains.Any()) ViewBag.parents = new SelectList(DB.NonValueChains.OrderBy(c => c.Name), nameof(NonValueChain.ID), nameof(NonValueChain.Name), valueChain.ParentID);
            ViewBag.branches = new SelectList(DB.Branches.OrderBy(c => c.Name), nameof(Branch.ID), nameof(Branch.Name), valueChain.BranchID);
            ViewBag.Title = "Add a value Chain";
            return View(valueChain);
        }

        public ActionResult Edit(int id)
        {
            var valueChain = DB.NonValueChains.Find(id);
            ViewBag.Title = $"Edit {valueChain.Name}";
            ViewBag.parents = new SelectList(DB.NonValueChains.Where(c => c.ID != valueChain.ID).OrderBy(c => c.Name), nameof(NonValueChain.ID), nameof(NonValueChain.Name), valueChain.ParentID);
            ViewBag.branches = new SelectList(DB.Branches.OrderBy(c => c.Name), nameof(Branch.ID), nameof(Branch.Name), valueChain.BranchID);
            return View(valueChain);
        }

        [ValidateAntiForgeryToken, HttpPost, ActionName(nameof(Edit))]
        public ActionResult Update(int id)
        {
            var valueChain = DB.NonValueChains.Find(id);
            if (TryUpdateModel(valueChain, new string[] { nameof(NonValueChain.Name) }))
            {
                if (ModelState.IsValid)
                {
                    DB.SaveChanges();
                    return RedirectToAction(nameof(Details), new { valueChain.ID });
                }
            }
            ViewBag.Title = $"Edit {valueChain.Name}";
            ViewBag.parents = new SelectList(DB.NonValueChains.Where(c => c.ID != valueChain.ID).OrderBy(c => c.Name), nameof(NonValueChain.ID), nameof(NonValueChain.Name), valueChain.ParentID);
            ViewBag.branches = new SelectList(DB.Branches.OrderBy(c => c.Name), nameof(Branch.ID), nameof(Branch.Name), valueChain.BranchID);
            return View(valueChain);
        }

        [AllowAnonymous]
        public JsonResult Search(string q)
        {
            var query = DB.NonValueChains.ToList().Where(c => c.Name.ToLower().Contains(q.ToLower()));
            var papers = new List<dynamic>();
            foreach (var item in query.Take(5))
            {
                var list = new List<Select2Model>
                {
                    new Select2Model{Caption="Value Chain",Value=item.Name},
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
    }
}