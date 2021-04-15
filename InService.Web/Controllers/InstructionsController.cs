using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using InService.Data;
using Microsoft.AspNet.Identity;
using PagedList;

namespace InService.Web.Controllers
{
    [Authorize]
    public class InstructionsController : SysController
    {
        public ActionResult Index(int? p, string q)
        {
            var query = DB.Instructions.AsQueryable();
            if (!String.IsNullOrEmpty(q)) query = query.Where(l => l.Description.Contains(q));
            ViewBag.Title = "Instructions";
            ViewBag.q = q;
            return View(query.OrderByDescending(c => c.CreationDate).ToPagedList(p ?? 1, DefaultPageSize));
        }

        public ActionResult Details(int id)
        {
            var instruction = DB.Instructions.Find(id);
            ViewBag.Title = $"details";
            return View(instruction);
        }

        public ActionResult Add()
        {
            ViewBag.Title = "Add instruction";
            return View();
        }

        [ValidateAntiForgeryToken, HttpPost,ValidateInput(false)]
        public ActionResult Add(Instruction instruction)
        {
            instruction.CreatorID = CurrentUserID;
            instruction.CreationDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                DB.Instructions.Add(instruction);
                DB.SaveChanges();
                return RedirectToAction(nameof(Details), new { instruction.ID });
            }
            ViewBag.Title = "Add instruction";
            return View(instruction);
        }

        public ActionResult Edit(int id)
        {
            var instruction = DB.Instructions.Find(id);
            ViewBag.Title = $"Edit ";
            return View(instruction);
        }

        [ValidateAntiForgeryToken, HttpPost, ActionName(nameof(Edit)),ValidateInput(false)]
        public ActionResult Update(int id)
        {
            var instruction = DB.Instructions.Find(id);
            if (ModelState.IsValid)
            {
                if (TryUpdateModel(instruction))
                {
                    DB.SaveChanges();
                    return RedirectToAction(nameof(Details), new { instruction.ID });
                }
            }
            ViewBag.Title = $"Edit";
            return View(instruction);
        }

        public ActionResult Delete(int? id)
        {
            var instruction = DB.Instructions.Find(id);
            ViewBag.Title = $"Delete";
            return View(instruction);
        }

        [HttpPost, ActionName(nameof(Delete))]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Instruction instruction = DB.Instructions.Find(id);
            DB.Instructions.Remove(instruction);
            DB.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public JsonResult Search(string q)
        {
            var query = DB.Instructions.Where(c => c.Description.Contains(q));
            var instructions = new List<dynamic>();
            foreach (var item in query.OrderBy(c => c.CreationDate))
            {
                instructions.Add(new { id = item.ID, text = item.Description });
            }
            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = instructions,
            };
        }
    }
}
