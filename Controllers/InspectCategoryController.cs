using Moldtrax.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Moldtrax.Controllers
{
    public class InspectCategoryController : Controller
    {
        private MoldtraxDbContext db = new MoldtraxDbContext();
        // GET: InspectCategory
        public ActionResult Index()
        {
            ViewBag.Companylist = new SelectList(db.TblCompanies.ToList().OrderBy(x => x.CompanyName), "CompanyID", "CompanyName");
            return View();
        }

        public ActionResult GetInspectData()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblCategories.Where(x=> x.CompanyID == CID).ToList();
            return PartialView("_InspectCategory", data);
        }

        public ActionResult CreateCategory(tblCategory model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            model.CompanyID = CID;
            db.TblCategories.Add(model);
            db.SaveChanges();

            var data = db.TblCategories.Where(x=> x.CompanyID == CID).ToList();
            return PartialView("_InspectCategory", data);
        }

        public ActionResult DeleteChecksheetCate(string str)
        {
            if (str != "")
            {
                str = str.Substring(0, str.LastIndexOf(','));
                using (var ctx = new MoldtraxDbContext())
                {
                    SqlParameter sp = new SqlParameter("@value", str);
                    var result = ctx.Database.ExecuteSqlCommand("exec DeleteChecksheetCategory @value", sp);
                }
            }

            int CID = ShrdMaster.Instance.GetCompanyID();

            var data = db.TblCategories.Where(x=> x.CompanyID == CID).ToList();
            return PartialView("_InspectCategory", data);
        }

        public ActionResult SaveFocusout(tblCategory model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblCategories.Where(x => x.CatID == model.CatID).FirstOrDefault();
            data.CategoryName = model.CategoryName;
            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }
    }
}