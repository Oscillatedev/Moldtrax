using Moldtrax.Models;
using Moldtrax.Providers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Moldtrax.Controllers
{
    [SessionExpireFilter]
    public class AdministrationController : Controller
    {
        private MoldtraxDbContext db = new MoldtraxDbContext();

        // GET: Administration
        public ActionResult Index()
        {
            ViewBag.Companylist = new SelectList(db.TblCompanies.ToList().OrderBy(x => x.CompanyName), "CompanyID", "CompanyName");
            return View();
        }

        public ActionResult DetailMoldLists()
        {
            ViewBag.Companylist = new SelectList(db.TblCompanies.ToList().OrderBy(x => x.CompanyName), "CompanyID", "CompanyName");
            return View();
        }

        #region Base Style Type

        public ActionResult BaseDataAscedingOrder()
        {
            return PartialView("_GetBaseData", ReturnMoldList());
        }

        public ActionResult BaseDataDescndingOrder()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            var data = db.TblDDMoldCategoryID.Where(x=> x.CompanyID == CID).ToList();
            List<tblDDMoldCategoryID> String = new List<tblDDMoldCategoryID>();
            List<tblDDMoldCategoryID> Num = new List<tblDDMoldCategoryID>();

            foreach (var x in data)
            {
                if (!string.IsNullOrEmpty(x.MoldCategoryID) && char.IsDigit(x.MoldCategoryID[0]))
                {
                    Num.Add(x);
                }
                else
                {
                    String.Add(x);
                }
            }

            String = String.OrderByDescending(x => x.MoldCategoryID).ToList();
            Num = Num.OrderByDescending(x => x.MoldCategoryID).ToList();
            Num.AddRange(String);
            return PartialView("_GetBaseData", Num);
        }

        public List<tblDDMoldCategoryID> ReturnMoldList()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDMoldCategoryID.Where(x=> x.CompanyID == CID).ToList();
            List<tblDDMoldCategoryID> String = new List<tblDDMoldCategoryID>();
            List<tblDDMoldCategoryID> Num = new List<tblDDMoldCategoryID>();

            foreach (var x in data)
            {
                
                if (!string.IsNullOrEmpty(x.MoldCategoryID) && char.IsDigit(x.MoldCategoryID[0]))
                {
                    Num.Add(x);
                }
                else
                {
                    String.Add(x);
                }
            }

            String = String.OrderBy(x => x.MoldCategoryID).ToList();
            String.AddRange(Num.OrderBy(x => x.MoldCategoryID).ToList());

            //foreach (var thing in data.OrderBy(x => x.MoldCategoryID, new SemiNumericComparer()))
            //{
            //    tbl.Add(thing);
            //}

            return String;
        }


        public ActionResult GetBaseData()
        {
            return PartialView("_GetBaseData", ReturnMoldList());
        }

        public void SaveBaseStyleFocusOut(tblDDMoldCategoryID model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
            //ShrdMaster.Instance.UpdateAuditLog(GetPageName.AdminSecurityManager.ToString(), GetTabName.User.ToString(), GetAction.Update.ToString());

        }

        public ActionResult SaveBaseStyle(tblDDMoldCategoryID model)
        {
            if (model.ID == 0)
            {
                int CID = ShrdMaster.Instance.GetCompanyID();
                model.CompanyID = CID;
                db.TblDDMoldCategoryID.Add(model);
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.AdminSecurityManager.ToString(), GetTabName.User.ToString(), GetAction.Create.ToString());

            }
            //else
            //{
            //    //var data = db.TblDDMoldCategoryID.Where(s => s.ID == x.ID).FirstOrDefault();
            //    //data.MoldCategoryID = x.MoldCategoryID;
            //    //data.MoldCategoryIDDesc = x.MoldCategoryIDDesc;
            //    db.Entry(model).State = EntityState.Modified;
            //    db.SaveChanges();
            //}

            //var data = db.TblDDMoldCategoryID.OrderBy(x => x.MoldCategoryID.Length).ThenBy(x => x.MoldCategoryID).ToList();

            return PartialView("_GetBaseData", ReturnMoldList());
            //return Json("OK", JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteBaseStyle(string str="")
        {
            if (str != "")
            {
                str = str.Substring(0, str.LastIndexOf(','));
                using (var ctx = new MoldtraxDbContext())
                {
                    SqlParameter sp = new SqlParameter("@value", str);
                    var result = ctx.Database.ExecuteSqlCommand("exec procDeleteBaseStyle @value", sp);
                    //ShrdMaster.Instance.UpdateAuditLog(GetPageName.AdminSecurityManager.ToString(), GetTabName.User.ToString(), GetAction.Delete.ToString());

                }
            }

            //var data = db.TblDDMoldCategoryID.OrderBy(x => x.MoldCategoryID.Length).ThenBy(x => x.MoldCategoryID).ToList();
            return PartialView("_GetBaseData", ReturnMoldList());
        }

        #endregion

        #region Department
        public ActionResult DepartmentAscedingOrder()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDDepartmentID.Where(x=> x.CompanyID == CID).ToList().OrderBy(x => x.DepartmentID);
            return PartialView("_DepartmentGetData", data);
        }

        public ActionResult DepartmentDescndingOrder()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDDepartmentID.Where(x=> x.CompanyID == CID).ToList().OrderByDescending(x => x.DepartmentID);
            return PartialView("_DepartmentGetData", data);
        }


        public ActionResult GetDepartment()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDDepartmentID.Where(x=> x.CompanyID == CID).ToList().OrderBy(x=> x.DepartmentID);
            return PartialView("_DepartmentGetData", data);
        }

        public void SaveDepartmentFocusOut(tblDDDepartmentID model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
            //ShrdMaster.Instance.UpdateAuditLog(GetPageName.AdminSecurityManager.ToString(), GetTabName.Department.ToString(), GetAction.Update.ToString());

        }

        public ActionResult SaveDepartment(tblDDDepartmentID model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            if (model.ID == 0)
            {
                model.CompanyID = CID;
                db.TblDDDepartmentID.Add(model);
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.AdminSecurityManager.ToString(), GetTabName.Department.ToString(), GetAction.Create.ToString());

            }
            //else
            //{
            //    //va2r data = db.TblDDDepartmentID.Where(s => s.ID == x.ID).FirstOrDefault();
            //    //data.DepartmentID = x.DepartmentID;
            //    //data.DepartmentIDDesc = x.DepartmentIDDesc;
            //    db.Entry(x).State = EntityState.Modified;
            //    db.SaveChanges();
            //}
            var data = db.TblDDDepartmentID.Where(x=> x.CompanyID == CID).ToList();

            return PartialView("_DepartmentGetData", data.OrderBy(x => x.DepartmentID));
        }

        public ActionResult DeleteDepartment(string str = "")
        {
            if (str != "")
            {
                str = str.Substring(0, str.LastIndexOf(','));
                using (var ctx = new MoldtraxDbContext())
                {
                    SqlParameter sp = new SqlParameter("@value", str);
                    var result = ctx.Database.ExecuteSqlCommand("exec procDepartment @value", sp);
                    //ShrdMaster.Instance.UpdateAuditLog(GetPageName.AdminSecurityManager.ToString(), GetTabName.Department.ToString(), GetAction.Delete.ToString());

                }
            }
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDDepartmentID.Where(x=> x.CompanyID == CID).ToList().OrderBy(x => x.DepartmentID);
            return PartialView("_DepartmentGetData", data);
        }

        #endregion

        #region Product Line
        public ActionResult ProductLineAscedingOrder()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDProductLine.Where(x=> x.CompanyID == CID).OrderBy(x => x.ProductLine).ToList();
            return PartialView("_ProductGetData", data);
        }

        public ActionResult ProductLineDescndingOrder()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDProductLine.Where(x=> x.CompanyID == CID).OrderByDescending(x => x.ProductLine).ToList();
            return PartialView("_ProductGetData", data);
        }


        public ActionResult GetProduct()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDProductLine.Where(x=> x.CompanyID == CID).OrderBy(x=> x.ProductLine).ToList();
            return PartialView("_ProductGetData", data);
        }

        public void SaveProductFocusOut(tblDDProductLine model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
            //ShrdMaster.Instance.UpdateAuditLog(GetPageName.AdminSecurityManager.ToString(), GetTabName.ProductLine.ToString(), GetAction.Update.ToString());

        }

        public ActionResult SaveProduct(tblDDProductLine model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            if (model.ID == 0)
            {
                model.CompanyID = CID;
                db.TblDDProductLine.Add(model);
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.AdminSecurityManager.ToString(), GetTabName.ProductLine.ToString(), GetAction.Update.ToString());

            }
            //else
            //{
            //    //va2r data = db.TblDDDepartmentID.Where(s => s.ID == x.ID).FirstOrDefault();
            //    //data.DepartmentID = x.DepartmentID;
            //    //data.DepartmentIDDesc = x.DepartmentIDDesc;
            //    db.Entry(x).State = EntityState.Modified;
            //    db.SaveChanges();
            //}
            var data = db.TblDDProductLine.Where(x=> x.CompanyID == CID).OrderBy(x => x.ProductLine).ToList();
            return PartialView("_ProductGetData", data);
        }

        public ActionResult DeleteProductLine(string str = "")
        {
            if (str != "")
            {
                str = str.Substring(0, str.LastIndexOf(','));
                using (var ctx = new MoldtraxDbContext())
                {
                    SqlParameter sp = new SqlParameter("@value", str);
                    var result = ctx.Database.ExecuteSqlCommand("exec procDeleteProductLine @value", sp);
                }
            }
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDProductLine.Where(x=> x.CompanyID == CID).OrderBy(x => x.ProductLine).ToList();
            return PartialView("_ProductGetData", data);
        }

        #endregion

        #region Porduct Part

        public ActionResult ProductPartAscedingOrder()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDProductPart.Where(x=> x.CompanyID == CID).ToList().OrderBy(x => x.ProductPart);
            return PartialView("_ProductPartGetData", data);
        }

        public ActionResult ProductPartDescndingOrder()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDProductPart.Where(x=> x.CompanyID == CID).ToList().OrderByDescending(x => x.ProductPart);
            return PartialView("_ProductPartGetData", data);
        }

        public ActionResult GetProductPart()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDProductPart.Where(x=> x.CompanyID == CID).ToList().OrderBy(x=> x.ProductPart);
            return PartialView("_ProductPartGetData", data);
        }

        public void SaveProductPartFocusOut(tblDDProductPart model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
        }

        public ActionResult SaveProductPart(tblDDProductPart model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            if (model.ID == 0)
            {
                model.CompanyID = CID;
                db.TblDDProductPart.Add(model);
                db.SaveChanges();
            }
            //else
            //{
            //    //va2r data = db.TblDDDepartmentID.Where(s => s.ID == x.ID).FirstOrDefault();
            //    //data.DepartmentID = x.DepartmentID;
            //    //data.DepartmentIDDesc = x.DepartmentIDDesc;
            //    db.Entry(x).State = EntityState.Modified;
            //    db.SaveChanges();
            //}


            var data = db.TblDDProductPart.Where(x=> x.CompanyID == CID).ToList();
            return PartialView("_ProductPartGetData", data.OrderBy(x => x.ProductPart));
            //return Json("OK", JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteProductPart(string str = "")
        {
            if (str != "")
            {
                str = str.Substring(0, str.LastIndexOf(','));
                using (var ctx = new MoldtraxDbContext())
                {
                    SqlParameter sp = new SqlParameter("@value", str);
                    var result = ctx.Database.ExecuteSqlCommand("exec procDeleteProductPart @value", sp);
                }
            }

            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDProductPart.Where(x=> x.CompanyID == CID).ToList().OrderBy(x => x.ProductPart);
            return PartialView("_ProductPartGetData", data);
        }
        #endregion


        #region Resin Type

        public ActionResult ResinTypeAscedingOrder()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            var data = db.TblDDMoldResinType.Where(x=> x.CompanyID == CID).ToList().OrderBy(x => x.MoldResinType);
            return PartialView("_ResinTypeGetData", data);
        }

        public ActionResult ResinTypeDescndingOrder()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDMoldResinType.Where(x=> x.CompanyID == CID).ToList().OrderByDescending(x => x.MoldResinType);
            return PartialView("_ResinTypeGetData", data);
        }

        public ActionResult GetResinType()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDMoldResinType.Where(x=> x.CompanyID == CID).ToList().OrderBy(x => x.MoldResinType);
            return PartialView("_ResinTypeGetData", data);
        }

        public void SaveResinTypeFocusOut(tblDDMoldResinType model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
        }

        public ActionResult SaveResinType(tblDDMoldResinType model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            if (model.ID == 0)
            {
                model.CompanyID = CID;
                db.TblDDMoldResinType.Add(model);
                db.SaveChanges();
            }
            //else
            //{
            //    //va2r data = db.TblDDDepartmentID.Where(s => s.ID == x.ID).FirstOrDefault();
            //    //data.DepartmentID = x.DepartmentID;
            //    //data.DepartmentIDDesc = x.DepartmentIDDesc;
            //    db.Entry(x).State = EntityState.Modified;
            //    db.SaveChanges();
            //}


            var data = db.TblDDMoldResinType.Where(x=> x.CompanyID == CID).ToList();

            return PartialView("_ResinTypeGetData", data.OrderBy(x => x.MoldResinType));
            //return Json("OK", JsonRequestBehavior.AllowGet);
        }


        public ActionResult DeleteResinType(string str = "")
        {
            if (str != "")
            {
                str = str.Substring(0, str.LastIndexOf(','));
                using (var ctx = new MoldtraxDbContext())
                {
                    SqlParameter sp = new SqlParameter("@value", str);
                    var result = ctx.Database.ExecuteSqlCommand("exec procDeleteResinType @value", sp);
                }
            }

            int CID = ShrdMaster.Instance.GetCompanyID();

            var data = db.TblDDMoldResinType.Where(x=> x.CompanyID == CID).ToList().OrderBy(x => x.MoldResinType);
            return PartialView("_ResinTypeGetData", data);
        }
        #endregion

        #region Runner Type

        public ActionResult RunnerTypeAscedingOrder()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDMoldCav.Where(x=> x.CompanyID == CID).ToList().OrderBy(x => x.MoldCav);
            return PartialView("_RunnerTypeGetData", data);
        }

        public ActionResult RunnerTypeDescndingOrder()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDMoldCav.Where(x=> x.CompanyID == CID).ToList().OrderByDescending(x => x.MoldCav);
            return PartialView("_RunnerTypeGetData", data);
        }

        public ActionResult GetRunnerType()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDMoldCav.Where(x=> x.CompanyID == CID).ToList().OrderBy(x=> x.MoldCav);
            return PartialView("_RunnerTypeGetData", data);
        }

        public void SaveRunnerTypeFocusOut(tblDDMoldCav model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
        }

        public ActionResult SaveRunnerType(tblDDMoldCav model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            if (model.ID == 0)
            {
                model.CompanyID = CID;
                db.TblDDMoldCav.Add(model);
                db.SaveChanges();
            }
            //else
            //{
            //    //va2r data = db.TblDDDepartmentID.Where(s => s.ID == x.ID).FirstOrDefault();
            //    //data.DepartmentID = x.DepartmentID;
            //    //data.DepartmentIDDesc = x.DepartmentIDDesc;
            //    db.Entry(x).State = EntityState.Modified;
            //    db.SaveChanges();
            //}


            var data = db.TblDDMoldCav.Where(x=> x.CompanyID == CID).ToList();
            return PartialView("_RunnerTypeGetData", data.OrderBy(x => x.MoldCav));
            //return Json("OK", JsonRequestBehavior.AllowGet);
        }


        public ActionResult DeleteRunnerType(string str = "")
        {
            if (str != "")
            {
                str = str.Substring(0, str.LastIndexOf(','));
                using (var ctx = new MoldtraxDbContext())
                {
                    SqlParameter sp = new SqlParameter("@value", str);
                    var result = ctx.Database.ExecuteSqlCommand("exec procDeleteRunnerType @value", sp);
                }
            }
            int CID = ShrdMaster.Instance.GetCompanyID();

            var data = db.TblDDMoldCav.Where(x=> x.CompanyID == CID).ToList().OrderBy(x => x.MoldCav);
            return PartialView("_RunnerTypeGetData", data);
        }
        #endregion

        #region Mold Tooling Type

        public ActionResult MoldToolingTypeAscedingOrder()
        {
            return PartialView("_MoldToolingTypeGetData", ReturnToolingList());
        }


        public ActionResult MoldToolingTypeDescndingOrder()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDMoldToolingTypes.Where(x=> x.CompanyID == CID).ToList();
            List<tblDDMoldToolingType> String = new List<tblDDMoldToolingType>();
            List<tblDDMoldToolingType> Num = new List<tblDDMoldToolingType>();

            foreach (var x in data)
            {
                if (!string.IsNullOrEmpty(x.DD_MoldToolingType) && char.IsDigit(x.DD_MoldToolingType[0]))
                {
                    Num.Add(x);
                }
                else
                {
                    String.Add(x);
                }
            }

            String = String.OrderByDescending(x => x.DD_MoldToolingType).ToList();
           Num= Num.OrderByDescending(x => x.DD_MoldToolingType).ToList();
            Num.AddRange(String);

            return PartialView("_MoldToolingTypeGetData", Num);
        }

        public List<tblDDMoldToolingType> ReturnToolingList()
        {

            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDMoldToolingTypes.Where(x=> x.CompanyID == CID).ToList();
            List<tblDDMoldToolingType> String = new List<tblDDMoldToolingType>();
            List<tblDDMoldToolingType> Num = new List<tblDDMoldToolingType>();

            foreach (var x in data)
            {
                if (!string.IsNullOrEmpty(x.DD_MoldToolingType) && char.IsDigit(x.DD_MoldToolingType[0]))
                {
                    Num.Add(x);
                }
                else
                {
                    String.Add(x);
                }
            }

            String = String.OrderBy(x => x.DD_MoldToolingType).ToList();
            String.AddRange(Num.OrderBy(x => x.DD_MoldToolingType).ToList());

            return String;
        }

        public ActionResult GetToolingType()
        {
            //var data = db.TblDDMoldToolingTypes.OrderBy(x => x.DD_MoldToolingType.Length).ThenBy(x=> x.DD_MoldToolingType).ToList();
            return PartialView("_MoldToolingTypeGetData", ReturnToolingList());
        }

        public void SaveToolingTypeFocusOut(tblDDMoldToolingType model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
        }

        public ActionResult SaveToolingType(tblDDMoldToolingType model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            if (model.ID == 0)
            {
                model.CompanyID = CID;
                db.TblDDMoldToolingTypes.Add(model);
                db.SaveChanges();
            }
            //else
            //{
            //    //va2r data = db.TblDDDepartmentID.Where(s => s.ID == x.ID).FirstOrDefault();
            //    //data.DepartmentID = x.DepartmentID;
            //    //data.DepartmentIDDesc = x.DepartmentIDDesc;
            //    db.Entry(x).State = EntityState.Modified;
            //    db.SaveChanges();
            //}
            //var data = db.TblDDMoldToolingTypes.OrderBy(x => x.DD_MoldToolingType.Length).ThenBy(x => x.DD_MoldToolingType).ToList();
            return PartialView("_MoldToolingTypeGetData", ReturnToolingList());
        }


        public ActionResult DeleteMoldToolingType(string str = "")
        {
            if (str != "")
            {
                str = str.Substring(0, str.LastIndexOf(','));
                using (var ctx = new MoldtraxDbContext())
                {
                    SqlParameter sp = new SqlParameter("@value", str);
                    var result = ctx.Database.ExecuteSqlCommand("exec procDeleteMoldToolingType @value", sp);
                }
            }

            //var data = db.TblDDMoldToolingTypes.OrderBy(x => x.DD_MoldToolingType.Length).ThenBy(x => x.DD_MoldToolingType).ToList();
            return PartialView("_MoldToolingTypeGetData", ReturnToolingList());
        }

        #endregion

        #region TS Guide Defect Type

        public ActionResult TSGuideAscedingOrder()
        {
            return PartialView("_TSGuideGetData", ReturnTSGuideDefect());
        }

        public ActionResult TSGuideDescndingOrder()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDTSType.Where(x=> x.CompanyID == CID).ToList();
            List<tblDDTSType> String = new List<tblDDTSType>();
            List<tblDDTSType> Num = new List<tblDDTSType>();

            foreach (var x in data)
            {
                if (!string.IsNullOrEmpty(x.TSType) && char.IsDigit(x.TSType[0]))
                {
                    Num.Add(x);
                }
                else
                {
                    String.Add(x);
                }
            }

            String = String.OrderByDescending(x => x.TSType).ToList();
            Num = Num.OrderByDescending(x => x.TSType).ToList();
            Num.AddRange(String);
            return PartialView("_TSGuideGetData", Num);
        }

        public List<tblDDTSType> ReturnTSGuideDefect()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDTSType.Where(x=> x.CompanyID == CID).ToList();
            List<tblDDTSType> String = new List<tblDDTSType>();
            List<tblDDTSType> Num = new List<tblDDTSType>();

            foreach (var x in data)
            {
                if (!string.IsNullOrEmpty(x.TSType) && char.IsDigit(x.TSType[0]))
                {
                    Num.Add(x);
                }
                else
                {
                    String.Add(x);
                }
            }

            String = String.OrderBy(x => x.TSType).ToList();
            String.AddRange(Num.OrderBy(x => x.TSType).ToList());

            return String;
        }


        public ActionResult GetTSGuideType()
        {
            //var data = db.TblDDTSType.OrderBy(x=> x.TSType.Length).ThenBy(x=> x.TSType).ToList();
            return PartialView("_TSGuideGetData", ReturnTSGuideDefect());
        }

        public void SaveTSGuideTypeFocusOut(tblDDTSType model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
        }

        public ActionResult SaveTSGuideType(tblDDTSType model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            if (model.ID == 0)
            {
                model.CompanyID = CID;
                db.TblDDTSType.Add(model);
                db.SaveChanges();
            }
            //else
            //{
            //    db.Entry(x).State = EntityState.Modified;
            //    db.SaveChanges();
            //}
            //var data = db.TblDDTSType.OrderBy(x => x.TSType.Length).ThenBy(x => x.TSType).ToList();
            return PartialView("_TSGuideGetData", ReturnTSGuideDefect());
            //return Json("OK", JsonRequestBehavior.AllowGet);
        }


        public ActionResult DeleteTSGuideType(string str = "")
        {
            if (str != "")
            {
                str = str.Substring(0, str.LastIndexOf(','));
                using (var ctx = new MoldtraxDbContext())
                {
                    SqlParameter sp = new SqlParameter("@value", str);
                    var result = ctx.Database.ExecuteSqlCommand("exec procDeleteTSGuideType @value", sp);
                }
            }

            //var data = db.TblDDTSType.OrderBy(x => x.TSType.Length).ThenBy(x => x.TSType).ToList();
            return PartialView("_TSGuideGetData", ReturnTSGuideDefect());
        }
        #endregion

        #region Tech Tips Links

        public ActionResult TechTipsAscedingOrder()
        {
            return PartialView("_TechTipsGetData", ReturnTechTipsList());
        }

        public ActionResult TechTipsDescndingOrder()
        {

            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDDocSections.Where(x=> x.CompanyID == CID).ToList();
            List<tblDDDocSection> String = new List<tblDDDocSection>();
            List<tblDDDocSection> Num = new List<tblDDDocSection>();

            foreach (var x in data)
            {
                if (!string.IsNullOrEmpty(x.DocSection) && char.IsDigit(x.DocSection[0]))
                {
                    Num.Add(x);
                }
                else
                {
                    String.Add(x);
                }
            }

            String = String.OrderByDescending(x => x.DocSection).ToList();
            Num = Num.OrderByDescending(x => x.DocSection).ToList();
            Num.AddRange(String);
            return PartialView("_TechTipsGetData", Num);
        }

        public List<tblDDDocSection> ReturnTechTipsList()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDDocSections.Where(x=> x.CompanyID == CID).ToList();
            List<tblDDDocSection> String = new List<tblDDDocSection>();
            List<tblDDDocSection> Num = new List<tblDDDocSection>();

            foreach (var x in data)
            {
                if (!string.IsNullOrEmpty(x.DocSection) && char.IsDigit(x.DocSection[0]))
                {
                    Num.Add(x);
                }
                else
                {
                    String.Add(x);
                }
            }

            String = String.OrderBy(x => x.DocSection).ToList();
            String.AddRange(Num.OrderBy(x => x.DocSection).ToList());

            return String;
        }

        public ActionResult GetTechTips()
        {
            //var data = db.TblDDDocSections.OrderBy(x=> x.DocSection.Length).ThenBy(x=> x.DocSection).ToList();
            return PartialView("_TechTipsGetData", ReturnTechTipsList());
        }

        public void SaveTechTipsFocusOut(tblDDDocSection model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
        }

        public ActionResult SaveTechTips(tblDDDocSection model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            if (model.ID == 0)
            {
                model.CompanyID = CID;
                db.TblDDDocSections.Add(model);
                db.SaveChanges();
            }
            //else
            //{
            //    db.Entry(x).State = EntityState.Modified;
            //    db.SaveChanges();
            //}
            //var data = db.TblDDDocSections.OrderBy(x => x.DocSection.Length).ThenBy(x => x.DocSection).ToList();
            return PartialView("_TechTipsGetData", ReturnTechTipsList());
        }


        public ActionResult DeleteTechTips(string str = "")
        {
            if (str != "")
            {
                str = str.Substring(0, str.LastIndexOf(','));
                using (var ctx = new MoldtraxDbContext())
                {
                    SqlParameter sp = new SqlParameter("@value", str);
                    var result = ctx.Database.ExecuteSqlCommand("exec procDeleteTechTips @value", sp);
                }
            }

            //var data = db.TblDDDocSections.OrderBy(x => x.DocSection.Length).ThenBy(x => x.DocSection).ToList();
            return PartialView("_TechTipsGetData", ReturnTechTipsList());
        }
        #endregion

        #region Factors

        public ActionResult GetFactors()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDFactors.Where(x=> x.CompanyID == CID).ToList().OrderBy(x => x.Plastic_Type);
            return PartialView("_FactorsGetData", data);
        }

        public ActionResult FactorsAscedingOrder()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDFactors.Where(x=> x.CompanyID == CID).ToList().OrderBy(x => x.Plastic_Type);
            return PartialView("_FactorsGetData", data);
        }

        public ActionResult FactorsDescndingOrder()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDFactors.Where(x=> x.CompanyID == CID).ToList().OrderByDescending(x => x.Plastic_Type);
            return PartialView("_FactorsGetData", data);
        }

        public ActionResult SaveFactors(tblDDFactors model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            model.CompanyID = CID;
            db.TblDDFactors.Add(model);
            db.SaveChanges();

            var data = db.TblDDFactors.Where(x=> x.CompanyID == CID).ToList().OrderBy(x => x.Plastic_Type);
            return PartialView("_FactorsGetData", data);
        }

        public ActionResult DeleteFactors(string str = "")
        {
            if (str != "")
            {
                str = str.Substring(0, str.LastIndexOf(','));
                using (var ctx = new MoldtraxDbContext())
                {
                    SqlParameter sp = new SqlParameter("@value", str);
                    var result = ctx.Database.ExecuteSqlCommand("exec procDeleteFactors @value", sp);
                }
            }
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDFactors.Where(x=> x.CompanyID == CID).ToList().OrderBy(x => x.Plastic_Type);
            return PartialView("_FactorsGetData", data);
        }

        public void SaveFactorsFocusOut(tblDDFactors model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
        }

        #endregion

        public ActionResult EnterLicenseCode()
        {
            return View();
        }

        public ActionResult MaintenanceTrackingLists()
        {
            ViewBag.Companylist = new SelectList(db.TblCompanies.ToList().OrderBy(x => x.CompanyName), "CompanyID", "CompanyName");
            return View();
        }

        #region Mold Configuration

        public List<tblDDMoldConfig> ReturnMoldConfig()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDMoldConfigs.Where(x=> x.CompanyID == CID).ToList();
            List<tblDDMoldConfig> String = new List<tblDDMoldConfig>();
            List<tblDDMoldConfig> Num = new List<tblDDMoldConfig>();

            foreach (var x in data)
            {
                if (!string.IsNullOrEmpty(x.MoldConfig) && char.IsDigit(x.MoldConfig[0]))
                {
                    Num.Add(x);
                }
                else
                {
                    String.Add(x);
                }
            }

            String = String.OrderBy(x => x.MoldConfig).ToList();
            String.AddRange(Num.OrderBy(x => x.MoldConfig).ToList());

            return String;
        }

        
        public ActionResult MoldDescndingOrder()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDMoldConfigs.Where(x=> x.CompanyID == CID).ToList();
            List<tblDDMoldConfig> String = new List<tblDDMoldConfig>();
            List<tblDDMoldConfig> Num = new List<tblDDMoldConfig>();

            foreach (var x in data)
            {
                if (!string.IsNullOrEmpty(x.MoldConfig) && char.IsDigit(x.MoldConfig[0]))
                {
                    Num.Add(x);
                }
                else
                {
                    String.Add(x);
                }
            }

           String = String.OrderByDescending(x => x.MoldConfig).ToList();

           Num = Num.OrderByDescending(x => x.MoldConfig).ToList();
           Num.AddRange(String);

            return PartialView("_MoldConfigGetData", Num);
        }



        public ActionResult MoldAscedingOrder()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDMoldConfigs.Where(x=> x.CompanyID == CID).ToList();
            List<tblDDMoldConfig> String = new List<tblDDMoldConfig>();
            List<tblDDMoldConfig> Num = new List<tblDDMoldConfig>();

            foreach (var x in data)
            {
                if (!string.IsNullOrEmpty(x.MoldConfig) && char.IsDigit(x.MoldConfig[0]))
                {
                    Num.Add(x);
                }
                else
                {
                    String.Add(x);
                }
            }
            String = String.OrderBy(x => x.MoldConfig).ToList();
            String.AddRange(Num.OrderBy(x => x.MoldConfig).ToList());
            return PartialView("_MoldConfigGetData", String);
        }

        public ActionResult GetMoldConfig()
        {
            //var data = db.TblDDMoldConfigs.OrderBy(x=> x.MoldConfig.Length).ThenBy(x=> x.MoldConfig).ToList();
            return PartialView("_MoldConfigGetData", ReturnMoldConfig());
        }

        public void SaveMoldConfigFocusOut(tblDDMoldConfig model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
        }

        public ActionResult SaveMoldConfig(tblDDMoldConfig model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            if (model.ID == 0)
            {
                model.CompanyID = CID;
                db.TblDDMoldConfigs.Add(model);
                db.SaveChanges();
            }
            //else
            //{
            //    db.Entry(x).State = EntityState.Modified;
            //    db.SaveChanges();
            //}
            //var data = db.TblDDMoldConfigs.OrderBy(x => x.MoldConfig.Length).ThenBy(x => x.MoldConfig).ToList();
            return PartialView("_MoldConfigGetData", ReturnMoldConfig());
        }


        public ActionResult DeleteMoldConfig(string str = "")
        {
            if (str != "")
            {
                str = str.Substring(0, str.LastIndexOf(','));
                using (var ctx = new MoldtraxDbContext())
                {
                    SqlParameter sp = new SqlParameter("@value", str);
                    var result = ctx.Database.ExecuteSqlCommand("exec procDeleteMoldConfig @value", sp);
                }
            }

            //var data = db.TblDDMoldConfigs.OrderBy(x => x.MoldConfig.Length).ThenBy(x => x.MoldConfig).ToList();
            return PartialView("_MoldConfigGetData", ReturnMoldConfig());
        }

        #endregion


        #region Mold Configuration 2

        public ActionResult Mold2AscedingOrder()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDMoldConfig2s.Where(x=> x.CompanyID == CID).ToList();
            List<tblDDMoldConfig2> String = new List<tblDDMoldConfig2>();
            List<tblDDMoldConfig2> Num = new List<tblDDMoldConfig2>();

            foreach (var x in data)
            {
                if (!string.IsNullOrEmpty(x.MoldConfig) && char.IsDigit(x.MoldConfig[0]))
                {
                    Num.Add(x);
                }
                else
                {
                    String.Add(x);
                }
            }

            String = String.OrderBy(x => x.MoldConfig).ToList();
            String.AddRange(Num.OrderBy(x => x.MoldConfig).ToList());

            return PartialView("_MoldConfig2GetData", ReturnMoldConfig2()); ;
        }

        public ActionResult Mold2DescndingOrder()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDMoldConfig2s.Where(x=> x.CompanyID == CID).ToList();
            List<tblDDMoldConfig2> String = new List<tblDDMoldConfig2>();
            List<tblDDMoldConfig2> Num = new List<tblDDMoldConfig2>();

            foreach (var x in data)
            {
                if (!string.IsNullOrEmpty(x.MoldConfig) && char.IsDigit(x.MoldConfig[0]))
                {
                    Num.Add(x);
                }
                else
                {
                    String.Add(x);
                }
            }

           String = String.OrderByDescending(x => x.MoldConfig).ToList();
           Num = Num.OrderBy(x => x.MoldConfig).ToList();
           Num.AddRange(String);

            return PartialView("_MoldConfig2GetData", Num);
        }

        public List<tblDDMoldConfig2> ReturnMoldConfig2()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDMoldConfig2s.Where(x=> x.CompanyID == CID).ToList();
            List<tblDDMoldConfig2> String = new List<tblDDMoldConfig2>();
            List<tblDDMoldConfig2> Num = new List<tblDDMoldConfig2>();

            foreach (var x in data)
            {
                if (!string.IsNullOrEmpty(x.MoldConfig) && char.IsDigit(x.MoldConfig[0]))
                {
                    Num.Add(x);
                }
                else
                {
                    String.Add(x);
                }
            }

            String = String.OrderBy(x => x.MoldConfig).ToList();
            String.AddRange(Num.OrderBy(x => x.MoldConfig).ToList());

            return String;
        }

        public ActionResult GetMoldConfig2()
        {
            //var data = db.TblDDMoldConfig2s.ToList().OrderBy(x => x.MoldConfig);
            return PartialView("_MoldConfig2GetData", ReturnMoldConfig2());
        }

        public void SaveMoldConfig2FocusOut(tblDDMoldConfig2 model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
        }

        public ActionResult SaveMoldConfig2(tblDDMoldConfig2 model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            if (model.ID == 0)
            {
                model.CompanyID = CID;
                db.TblDDMoldConfig2s.Add(model);
                db.SaveChanges();
            }
            //else
            //{
            //    db.Entry(x).State = EntityState.Modified;
            //    db.SaveChanges();
            //}

            //var data = db.TblDDMoldConfig2s.ToList();
            return PartialView("_MoldConfig2GetData", ReturnMoldConfig2());
        }

        public ActionResult DeleteMoldConfig2(string str = "")
        {
            if (str != "")
            {
                str = str.Substring(0, str.LastIndexOf(','));
                using (var ctx = new MoldtraxDbContext())
                {
                    SqlParameter sp = new SqlParameter("@value", str);
                    var result = ctx.Database.ExecuteSqlCommand("exec procDeleteMoldConfig2 @value", sp);
                }
            }

            //var data = db.TblDDMoldConfig2s.ToList().OrderBy(x => x.MoldConfig);
            return PartialView("_MoldConfig2GetData", ReturnMoldConfig2());
        }

        #endregion


        #region Stop Reason

        public ActionResult MoldStopAscedingOrder()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblddStopReasons.Where(x=> x.CompanyID == CID).OrderBy(x => x.StopReason).ToList();
           return PartialView("_MoldStopReasonData", data);
        }

        public ActionResult MoldStopDescndingOrder()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblddStopReasons.Where(x=> x.CompanyID == CID).OrderByDescending(x => x.StopReason).ToList();
            return PartialView("_MoldStopReasonData", data);
        }

        public ActionResult GetStopReason()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblddStopReasons.Where(x=> x.CompanyID == CID).OrderBy(x=> x.StopReason).ToList();
            return PartialView("_MoldStopReasonData", data);
        }

        public void SaveStopReasonFocusOut(tblddStopReason model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
        }
        public ActionResult SaveStopReason(tblddStopReason model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            if (model.ID == 0)
            {
                model.CompanyID = CID;
                db.TblddStopReasons.Add(model);
                db.SaveChanges();
            }
            //else
            //{
            //    db.Entry(x).State = EntityState.Modified;
            //    db.SaveChanges();
            //}
            var data = db.TblddStopReasons.Where(x=> x.CompanyID == CID).ToList();
            return PartialView("_MoldStopReasonData", data.OrderBy(x => x.StopReason));
        }


        public ActionResult DeleteStopReason(string str = "")
        {
            if (str != "")
            {
                str = str.Substring(0, str.LastIndexOf(','));
                using (var ctx = new MoldtraxDbContext())
                {
                    SqlParameter sp = new SqlParameter("@value", str);
                    var result = ctx.Database.ExecuteSqlCommand("exec procDeleteStopReason @value", sp);
                }
            }

            int CID = ShrdMaster.Instance.GetCompanyID();

            var data = db.TblddStopReasons.Where(x=> x.CompanyID == CID).OrderBy(x => x.StopReason).ToList();
            return PartialView("_MoldStopReasonData", data);
        }
        #endregion


        #region Corrective ACTION Type

        public ActionResult CorrectiveActionTypeAscedingOrder()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDTITypes.Where(x=> x.CompanyID == CID).OrderBy(x => x.TIType).ToList();
            return PartialView("_CorrecctiveActionTypeData", data);
        }

        public ActionResult CorrectiveActionTypeDescndingOrder()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDTITypes.Where(x=> x.CompanyID == CID).OrderByDescending(x => x.TIType).ToList();
            return PartialView("_CorrecctiveActionTypeData", data);
        }


        public ActionResult GetCorrectiveActionType()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDTITypes.Where(x=> x.CompanyID == CID).OrderBy(x=> x.TIType).ToList();
            return PartialView("_CorrecctiveActionTypeData", data);
        }

        public void SaveCorrectiveActionTypeFocusOut(tblDDTIType model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
        }

        public ActionResult SaveCorrectiveActionType(tblDDTIType model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            if (model.ID == 0)
            {
                model.CompanyID = CID;
                db.TblDDTITypes.Add(model);
                db.SaveChanges();
            }
            //else
            //{
            //    db.Entry(x).State = EntityState.Modified;
            //    db.SaveChanges();
            //}

            var data = db.TblDDTITypes.Where(x=> x.CompanyID == CID).OrderBy(x=> x.TIType).ToList();
            return PartialView("_CorrecctiveActionTypeData", data);
        }


        public ActionResult DeleteCorrectiveActionType(string str = "")
        {
            if (str != "")
            {
                str = str.Substring(0, str.LastIndexOf(','));
                using (var ctx = new MoldtraxDbContext())
                {
                    SqlParameter sp = new SqlParameter("@value", str);
                    var result = ctx.Database.ExecuteSqlCommand("exec procDeleteCorrectiveActionType @value", sp);
                }
            }

            int CID = ShrdMaster.Instance.GetCompanyID();

            var data = db.TblDDTITypes.Where(x=> x.CompanyID == CID).OrderBy(x => x.TIType).ToList();
            return PartialView("_CorrecctiveActionTypeData", data);
        }
        #endregion


        #region Corrective ACTION

        public ActionResult CorrectiveActionAscedingOrder()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDTlCorrectiveActions.Where(x=> x.CompanyID == CID).OrderBy(x => x.TlCorrectiveAction).ToList();
            return PartialView("_CorrecctiveActionData", data);
        }

        public ActionResult CorrectiveActionDescndingOrder()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDTlCorrectiveActions.Where(x=> x.CompanyID == CID).OrderByDescending(x => x.TlCorrectiveAction).ToList();
            return PartialView("_CorrecctiveActionData", data);
        }

        public ActionResult GetCorrectiveAction()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDTlCorrectiveActions.Where(x=> x.CompanyID == CID).OrderBy(x=> x.TlCorrectiveAction).ToList();
            return PartialView("_CorrecctiveActionData", data);
        }

        public void SaveCorrectiveActionFocusOut(tblDDTlCorrectiveAction model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
        }

        public ActionResult SaveCorrectiveAction(tblDDTlCorrectiveAction model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            if (model.ID == 0)
            {
                model.CompanyID = CID;
                db.TblDDTlCorrectiveActions.Add(model);
                db.SaveChanges();
            }
            //else
            //{
            //    db.Entry(x).State = EntityState.Modified;
            //    db.SaveChanges();
            //}
            var data = db.TblDDTlCorrectiveActions.Where(x=>x.CompanyID == CID).OrderBy(x => x.TlCorrectiveAction).ToList();
            return PartialView("_CorrecctiveActionData", data);
        }

        public ActionResult DeleteCorrectiveAction(string str = "")
        {
            if (str != "")
            {
                str = str.Substring(0, str.LastIndexOf(','));
                using (var ctx = new MoldtraxDbContext())
                {
                    SqlParameter sp = new SqlParameter("@value", str);
                    var result = ctx.Database.ExecuteSqlCommand("exec procDeleteCorrectiveAction @value", sp);
                }
            }
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDTlCorrectiveActions.Where(x=> x.CompanyID == CID).OrderBy(x => x.TlCorrectiveAction).ToList();
            return PartialView("_CorrecctiveActionData", data);
        }
        #endregion


        #region Maintenance Schedule

        public ActionResult MaintenanceScheAscedingOrder()
        {
           return PartialView("_MaintenanceScheduleData", ReturnMainenanceSchedule());
        }

        public ActionResult MaintenanceScheDescndingOrder()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDschStatuses.Where(x=> x.CompanyID == CID).ToList();
            List<tblDDschStatus> String = new List<tblDDschStatus>();
            List<tblDDschStatus> Num = new List<tblDDschStatus>();

            foreach (var x in data)
            {
                if (!string.IsNullOrEmpty(x.schStatus) && char.IsDigit(x.schStatus[0]))
                {
                    Num.Add(x);
                }
                else
                {
                    String.Add(x);
                }
            }

            String = String.OrderByDescending(x => x.schStatus).ToList();
            Num = Num.OrderByDescending(x => x.schStatus).ToList();
            Num.AddRange(String);
            return PartialView("_MaintenanceScheduleData", Num);
        }


        public List<tblDDschStatus> ReturnMainenanceSchedule()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDschStatuses.Where(x=>x.CompanyID == CID).ToList();
            List<tblDDschStatus> String = new List<tblDDschStatus>();
            List<tblDDschStatus> Num = new List<tblDDschStatus>();

            foreach (var x in data)
            {
                if (!string.IsNullOrEmpty(x.schStatus) && char.IsDigit(x.schStatus[0]))
                {
                    Num.Add(x);
                }
                else
                {
                    String.Add(x);
                }
            }

            String = String.OrderBy(x => x.schStatus).ToList();
            String.AddRange(Num.OrderBy(x => x.schStatus).ToList());

            return String;
        }


        public ActionResult GetMaintenanceSchedule()
        {
            //var data = db.TblDDschStatuses.OrderBy(x=> x.schStatus.Length).ThenBy(x=> x.schStatus).ToList();
            return PartialView("_MaintenanceScheduleData", ReturnMainenanceSchedule());
        }

        public void SaveMaintenanceScheduleFocusOut(tblDDschStatus model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
        }

        public ActionResult SaveMaintenanceSchedule(tblDDschStatus model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            if (model.ID == 0)
            {
                model.CompanyID = CID;
                db.TblDDschStatuses.Add(model);
                db.SaveChanges();
            }

            //else
            //{
            //    db.Entry(x).State = EntityState.Modified;
            //    db.SaveChanges();
            //}

            //var data = db.TblDDschStatuses.OrderBy(x => x.schStatus.Length).ThenBy(x => x.schStatus).ToList();

            return PartialView("_MaintenanceScheduleData", ReturnMainenanceSchedule());
        }

        public ActionResult DeleteMaintenanceSch(string str = "")
        {
            if (str != "")
            {
                str = str.Substring(0, str.LastIndexOf(','));
                using (var ctx = new MoldtraxDbContext())
                {
                    SqlParameter sp = new SqlParameter("@value", str);
                    var result = ctx.Database.ExecuteSqlCommand("exec procDeleteMaintenanceSch @value", sp);
                }
            }

            //var data = db.TblDDschStatuses.OrderBy(x => x.schStatus.Length).ThenBy(x => x.schStatus).ToList();
            return PartialView("_MaintenanceScheduleData", ReturnMainenanceSchedule());
        }
        #endregion



        #region Repair Status

        public ActionResult RepairStatusAscedingOrder()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDRepairStatuses.Where(x=> x.CompanyID == CID).OrderBy(x => x.RepairStatus).ToList();
            return PartialView("_RepairStatusData", data);
        }

        public ActionResult RepairStatusDescndingOrder()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDRepairStatuses.Where(x=> x.CompanyID == CID ).OrderByDescending(x => x.RepairStatus).ToList();
            return PartialView("_RepairStatusData", data);
        }

        public ActionResult GetRepairStatus()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDRepairStatuses.Where(x=>x.CompanyID == CID).OrderBy(x=> x.RepairStatus).ToList();
            return PartialView("_RepairStatusData", data);
        }

        public void SaveRepairStatusFocusOut(tblDDRepairStatus model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
        }

        public ActionResult SaveRepairStatus(tblDDRepairStatus model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            if (model.ID == 0)
            {
                model.CompanyID = CID;
                db.TblDDRepairStatuses.Add(model);
                db.SaveChanges();
            }
            //else
            //{
            //    db.Entry(x).State = EntityState.Modified;
            //    db.SaveChanges();
            //}
            var data = db.TblDDRepairStatuses.Where(x=> x.CompanyID == CID).OrderBy(x => x.RepairStatus).ToList();
            return PartialView("_RepairStatusData", data);
        }


        public ActionResult DeleteRepairStatus(string str = "")
        {
            if (str != "")
            {
                str = str.Substring(0, str.LastIndexOf(','));
                using (var ctx = new MoldtraxDbContext())
                {
                    SqlParameter sp = new SqlParameter("@value", str);
                    var result = ctx.Database.ExecuteSqlCommand("exec procDeleteRepairStatus @value", sp);
                }
            }
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDRepairStatuses.Where(x=> x.CompanyID == CID).OrderBy(x => x.RepairStatus).ToList();
            return PartialView("_RepairStatusData", data);
        }
        #endregion



        #region Repair Location

        public ActionResult RepairLocationAscedingOrder()
        {
            return PartialView("_RepairLocationData", ReturnRepairLocation());
        }

        public ActionResult RepairLocationDescndingOrder()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDRepairStatusLocations.Where(x=> x.CompanyID == CID).ToList();
            List<tblDDRepairStatusLocation> String = new List<tblDDRepairStatusLocation>();
            List<tblDDRepairStatusLocation> Num = new List<tblDDRepairStatusLocation>();

            foreach (var x in data)
            {
                if (!string.IsNullOrEmpty(x.RepairStatusLocation) && char.IsDigit(x.RepairStatusLocation[0]))
                {
                    Num.Add(x);
                }
                else
                {
                    String.Add(x);
                }
            }

            String = String.OrderByDescending(x => x.RepairStatusLocation).ToList();
            Num = Num.OrderByDescending(x => x.RepairStatusLocation).ToList();
            Num.AddRange(String);
            return PartialView("_RepairLocationData", Num);
        }

        public List<tblDDRepairStatusLocation> ReturnRepairLocation()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDRepairStatusLocations.Where(x=> x.CompanyID == CID).ToList();
            List<tblDDRepairStatusLocation> String = new List<tblDDRepairStatusLocation>();
            List<tblDDRepairStatusLocation> Num = new List<tblDDRepairStatusLocation>();

            foreach (var x in data)
            {
                if (!string.IsNullOrEmpty(x.RepairStatusLocation) && char.IsDigit(x.RepairStatusLocation[0]))
                {
                    Num.Add(x);
                }
                else
                {
                    String.Add(x);
                }
            }

            String = String.OrderBy(x => x.RepairStatusLocation).ToList();
            String.AddRange(Num.OrderBy(x => x.RepairStatusLocation).ToList());

            return String;
        }

        public ActionResult GetRepairLocation()
        {
            //var data = db.TblDDRepairStatusLocations.OrderBy(x=> x.RepairStatusLocation.Length).ThenBy(x=> x.RepairStatusLocation).ToList();
            return PartialView("_RepairLocationData", ReturnRepairLocation());
        }

        public void SaveRepairLocationFocusOut(tblDDRepairStatusLocation model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
        }

        public ActionResult SaveRepairLocation(tblDDRepairStatusLocation model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            if (model.ID == 0)
            {
                model.CompanyID = CID;
                db.TblDDRepairStatusLocations.Add(model);
                db.SaveChanges();
            }
            //else
            //{
            //    db.Entry(x).State = EntityState.Modified;
            //    db.SaveChanges();
            //}

            //var data = db.TblDDRepairStatusLocations.OrderBy(x => x.RepairStatusLocation.Length).ThenBy(x => x.RepairStatusLocation).ToList();
            return PartialView("_RepairLocationData", ReturnRepairLocation());

        }

        public ActionResult DeleteRepairLocation(string str = "")
        {
            if (str != "")
            {
                str = str.Substring(0, str.LastIndexOf(','));
                using (var ctx = new MoldtraxDbContext())
                {
                    SqlParameter sp = new SqlParameter("@value", str);
                    var result = ctx.Database.ExecuteSqlCommand("exec procDeleteRepairLocation @value", sp);
                }
            }

            //var data = db.TblDDRepairStatusLocations.OrderBy(x => x.RepairStatusLocation.Length).ThenBy(x => x.RepairStatusLocation).ToList();
            return PartialView("_RepairLocationData", ReturnRepairLocation());
        }
        #endregion


        #region CheckSheet

        public ActionResult CheckSheet()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            ViewBag.Companylist = new SelectList(db.TblCompanies.ToList().OrderBy(x => x.CompanyName), "CompanyID", "CompanyName");
            ChecksheetDropDown();
            //var MDdata = ShrdMaster.Instance.GetMoldDropDown();
            //ViewBag.MoldList = MDdata;

            return View();
        }

        public void ChecksheetDropDown()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var dd = db.TblCategories.Where(x=> x.CompanyID == CID).OrderBy(x=> x.CategoryName).ToList();
            List<SelectListItem> Tech = new List<SelectListItem>();
            foreach (var x in dd)
            {
                Tech.Add(new SelectListItem
                {
                    Text = x.CategoryName,
                    Value = x.CatID.ToString()
                });
            }
            ViewBag.Category = Tech;
            var MDdata = ShrdMaster.Instance.GetMoldDropDown();
            ViewBag.MoldList = MDdata;
        }


        public ActionResult GetCheckSheetDataByCompany()
        {
            var MDdata = ShrdMaster.Instance.GetMoldDropDown();
            int CID = ShrdMaster.Instance.GetCompanyID();
            var Molddata = db.Database.SqlQuery<MoldDropdown>("exec procMoldDropdown @CompanyID", new SqlParameter("@CompanyID", CID)).ToList<MoldDropdown>();

            int MID = 0;
            if (Molddata.Count() != 0)
            {
                MID = Molddata.FirstOrDefault().MoldDataID;
            }

            var CategoryList = db.TblCategories.Where(x => x.CompanyID == CID).ToList();

            var data = db.TblInspectItems.Where(x => x.CompanyID == CID).ToList().Where(x => x.MoldID == MID).Select(x => new tblInspectItemsViewModel
            {
                CatID = x.CatID,
                CateName = CategoryList.Where(c => c.CatID == x.CatID).Select(c => c.CategoryName).FirstOrDefault(),
                InspectionID = x.InspectionID,
                InspectionName = x.InspectionName
            }).OrderBy(x => x.CateName).ThenBy(x => x.InspectionName).ToList();

            ChecksheetDropDown();

            var ChecksheetDiv = RenderRazorViewToString(this.ControllerContext, "_ChecksheetData", data);
            return Json(new { ChecksheetDiv, Molddata });
        }


        public static string RenderRazorViewToString(ControllerContext controllerContext, string viewName, object model)
        {
            controllerContext.Controller.ViewData.Model = model;

            using (var stringWriter = new StringWriter())
            {
                try
                {
                    var viewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
                    var viewContext = new ViewContext(controllerContext, viewResult.View, controllerContext.Controller.ViewData, controllerContext.Controller.TempData, stringWriter);
                    viewResult.View.Render(viewContext, stringWriter);
                    viewResult.ViewEngine.ReleaseView(controllerContext, viewResult.View);
                }
                catch (Exception ex)
                {

                }
                return stringWriter.GetStringBuilder().ToString();
            }
        }

        public ActionResult SaveCopyInspectItem(List<tblInspectItems> model, int MainMoldID=0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            foreach (var x in model)
            {
                x.CompanyID = CID;
                db.TblInspectItems.Add(x);
                db.SaveChanges();
            }

            var CategoryList = db.TblCategories.Where(x=> x.CompanyID == CID).ToList();
            var data = db.TblInspectItems.Where(x => x.MoldID == MainMoldID && x.CompanyID == CID).ToList().Select(x => new tblInspectItemsViewModel
            {
                CatID = x.CatID,
                CateName = CategoryList.Where(c => c.CatID == x.CatID).Select(c => c.CategoryName).FirstOrDefault(),
                InspectionID = x.InspectionID,
                InspectionName = x.InspectionName
            }).OrderBy(x => x.CateName).ThenBy(x => x.InspectionName).ToList();

            ChecksheetDropDown();
            return PartialView("_ChecksheetData", data);
        }

        public ActionResult GetCheckSheetData()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var Molddata = db.Database.SqlQuery<MoldDropdown>("exec procMoldDropdown @CompanyID", new SqlParameter("@CompanyID",CID)).ToList<MoldDropdown>();

            int MID = 0;
            if (Molddata.Count() != 0)
            {
               MID = Molddata.FirstOrDefault().MoldDataID;
            }

            var CategoryList = db.TblCategories.Where(x=> x.CompanyID == CID).ToList();

            var data = db.TblInspectItems.ToList().Where(x=> x.MoldID == MID && x.CompanyID == CID).Select(x => new tblInspectItemsViewModel
            {
                CatID = x.CatID,
                CateName = CategoryList.Where(c => c.CatID == x.CatID).Select(c => c.CategoryName).FirstOrDefault(),
                InspectionID = x.InspectionID,
                InspectionName = x.InspectionName
            }).OrderBy(x => x.CateName).ThenBy(x=> x.InspectionName).ToList();

            ChecksheetDropDown();
            return PartialView("_ChecksheetData", data);
        }

        public ActionResult OnMoldChange(int MID)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var CategoryList = db.TblCategories.Where(x=> x.CompanyID == CID).ToList();
            var data = db.TblInspectItems.ToList().Where(x => x.MoldID == MID).Select(x => new tblInspectItemsViewModel
            {
                CatID = x.CatID,
                CateName = CategoryList.Where(c => c.CatID == x.CatID).Select(c => c.CategoryName).FirstOrDefault(),
                InspectionID = x.InspectionID,
                InspectionName = x.InspectionName
            }).OrderBy(x => x.CateName).ThenBy(x => x.InspectionName).ToList();

            ChecksheetDropDown();

            return PartialView("_ChecksheetData", data);
        }

        public ActionResult DeleteChecksheetAction(string str, int MID=0)
        {
            if (str != "")
            {
                str = str.Substring(0, str.LastIndexOf(','));
                using (var ctx = new MoldtraxDbContext())
                {
                    SqlParameter sp = new SqlParameter("@value", str);
                    var result = ctx.Database.ExecuteSqlCommand("exec procDeleteChecksheet @value", sp);
                }
            }
            int CID = ShrdMaster.Instance.GetCompanyID();
            var CategoryList = db.TblCategories.Where(x=> x.CompanyID == CID).ToList();
            var data = db.TblInspectItems.ToList().Where(x=> x.MoldID == MID).Select(x => new tblInspectItemsViewModel
            {
                CatID = x.CatID,
                CateName = CategoryList.Where(c => c.CatID == x.CatID).Select(c => c.CategoryName).FirstOrDefault(),
                InspectionID = x.InspectionID,
                InspectionName = x.InspectionName
            }).OrderBy(x=> x.CateName).ToList();

            ChecksheetDropDown();

            return PartialView("_ChecksheetData", data);
        }

        //public void CommonDropDown()
        //{
        //    var MDdata = ShrdMaster.Instance.GetMoldDropDown();
        //    ViewBag.MoldList = MDdata;
        //}

        public ActionResult AscendingOrderList(int MID=0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var CategoryList = db.TblCategories.Where(x=> x.CompanyID == CID).ToList();

            var data = db.TblInspectItems.ToList().Where(x=> x.MoldID == MID).Select(x => new tblInspectItemsViewModel
            {
                CatID = x.CatID,
                CateName = CategoryList.Where(c => c.CatID == x.CatID).Select(c => c.CategoryName).FirstOrDefault(),
                InspectionID = x.InspectionID,
                InspectionName = x.InspectionName
            }).OrderBy(x => x.InspectionName).ToList();

            ChecksheetDropDown();
            return PartialView("_ChecksheetData", data);
        }

        public ActionResult DescendingOrderList(int MID=0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var CategoryList = db.TblCategories.Where(x=> x.CompanyID==CID).ToList();
            var data = db.TblInspectItems.ToList().Where(x=> x.MoldID == MID).Select(x => new tblInspectItemsViewModel
            {
                CatID = x.CatID,
                CateName = CategoryList.Where(c => c.CatID == x.CatID).Select(c => c.CategoryName).FirstOrDefault(),
                InspectionID = x.InspectionID,
                InspectionName = x.InspectionName
            }).OrderByDescending(x => x.InspectionName).ToList();

            ChecksheetDropDown();
            return PartialView("_ChecksheetData", data);
        }

        public ActionResult EditCheckSheetFocusOut(tblInspectItems model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblInspectItems.Where(x => x.InspectionID == model.InspectionID).FirstOrDefault();
            if (data != null)
            {
                data.InspectionName = model.InspectionName;
                data.CatID = model.CatID;
                db.SaveChanges();
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveChecksheetData(tblInspectItems model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            model.CompanyID = CID;
            db.TblInspectItems.Add(model);
            db.SaveChanges();

            var CategoryList = db.TblCategories.Where(x=> x.CompanyID == CID).ToList();

            var data = db.TblInspectItems.ToList().Where(x=> x.MoldID == model.MoldID).Select(x => new tblInspectItemsViewModel
            {
                CatID = x.CatID,
                CateName = CategoryList.Where(c => c.CatID == x.CatID).Select(c => c.CategoryName).FirstOrDefault(),
                InspectionID = x.InspectionID,
                InspectionName = x.InspectionName
            }).OrderBy(x=> x.CateName).ToList();

            ChecksheetDropDown();
            return PartialView("_ChecksheetData", data);
        }

        #endregion

        #region Organisation

        public ActionResult OrganisationInfo()
        {
            ViewBag.Companylist = new SelectList(db.TblCompanies.ToList().OrderBy(x => x.CompanyName), "CompanyID", "CompanyName");
            var data = db.TblOrganisations.ToList();
            tblOrganisation TBO = new tblOrganisation();

            if (data.FirstOrDefault() != null)
            {
                TBO = data.FirstOrDefault();
            }
            else
            {
                TBO = new tblOrganisation();
            }

            return View(TBO);
        }


        public void SaveOrganisation(tblOrganisation obj)
        {
            if (obj != null)
            {
                db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        #endregion

    }
}