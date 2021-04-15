using InService.Data;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InService.Web.Controllers
{
    public class ExtensionOfficersController : SysController
    {
        // GET: ExtensionOfficers
        public ActionResult Index(int? p)
        {
            var query = DB.ExtensionOfficers.AsQueryable();
            ViewBag.Title = "Extension officers";
            return View(new PagedList.PagedList<ExtensionOfficer>(query.OrderByDescending(c => c.CreationDate), p ?? 1, DefaultPageSize));
        }

        public ActionResult Details(Guid id)
        {
            var officer = DB.ExtensionOfficers.Find(id);
            ViewBag.Title = $"{officer.Fullname}";
            return View(officer);
        }
        public ActionResult Add()
        {
            ViewBag.Provinces = new SelectList(DB.Provinces.OrderBy(c => c.Name), nameof(Province.ID), nameof(Province.Name));
            ViewBag.Districts = new SelectList(DB.Districts.OrderBy(c => c.Name), nameof(District.ID), nameof(District.Name));
            ViewBag.Title = "Add extension officer";
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Add(ExtensionOfficer officer)
        {
            officer.CreationDate = DateTime.UtcNow;
            officer.ID = Guid.NewGuid();
            DB.ExtensionOfficers.Add(officer);
            DB.SaveChanges();
            return RedirectToAction(nameof(Details), new { officer.ID });
        }

        public ActionResult Edit(Guid id)
        {
            var officer = DB.ExtensionOfficers.Find(id);
            ViewBag.Title = $"{officer.Fullname}";
            ViewBag.Provinces = new SelectList(DB.Provinces.OrderBy(c => c.Name), nameof(Province.ID), nameof(Province.Name), officer.ProvinceID);
            ViewBag.Districts = new SelectList(DB.Districts.OrderBy(c => c.Name), nameof(District.ID), nameof(District.Name), officer.DistrictID);
            ViewBag.Title = "Edit extension officer";
            return View(officer);
        }
        [HttpPost, ValidateAntiForgeryToken, ActionName(nameof(Edit))]
        public ActionResult Update(Guid id)
        {
            var officer = DB.ExtensionOfficers.Find(id);
            if (TryUpdateModel(officer))
            {
                DB.SaveChanges();
                return RedirectToAction(nameof(Details), new { officer.ID });
            }
            ViewBag.Title = $"{officer.Fullname}";
            ViewBag.Provinces = new SelectList(DB.Provinces.OrderBy(c => c.Name), nameof(Province.ID), nameof(Province.Name), officer.ProvinceID);
            ViewBag.Districts = new SelectList(DB.Districts.OrderBy(c => c.Name), nameof(District.ID), nameof(District.Name), officer.DistrictID);
            ViewBag.Title = "Edit extension officer";
            return View(officer);
        }

        public ActionResult Upload()
        {
            ViewBag.Title = "Upload file";
            return View();
        }
        [ValidateAntiForgeryToken, HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            using (var package = new ExcelPackage(file.InputStream))
            {
                var workSheet = package.Workbook.Worksheets.First();
                var noOfCol = workSheet.Dimension.End.Column;
                var noOfRow = workSheet.Dimension.End.Row;
                if (noOfCol != 4)
                {
                    ModelState.AddModelError(nameof(file), "Invalid file");
                    return View();
                }
                for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                {
                    var ecnumber = workSheet.Cells[rowIterator, 1].Text;
                    var fname = workSheet.Cells[rowIterator, 2].Text;
                    var lastmane = workSheet.Cells[rowIterator, 3].Text;
                    var nationalid = workSheet.Cells[rowIterator, 4].Text;
                    var officer = new ExtensionOfficer
                    {
                        ECNumber = ecnumber,
                        Firstname = fname,
                        Surname = lastmane,
                        CreationDate = DateTime.UtcNow,
                        NationalID = nationalid,
                        ID = Guid.NewGuid(),
                    };
                    DB.ExtensionOfficers.Add(officer);
                }
            }
            DB.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}