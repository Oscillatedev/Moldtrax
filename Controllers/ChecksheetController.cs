using Moldtrax.Models;
using Moldtrax.Providers;
using Moldtrax.ViewMoldel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Moldtrax.Controllers
{
    [SessionExpireFilter]
    public class ChecksheetController : Controller
    {
        static string constring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        SqlConnection con = new SqlConnection(constring);
        private MoldtraxDbContext db = new MoldtraxDbContext();

        // GET: Checksheet
        public ActionResult Index()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            ViewBag.Companylist = new SelectList(db.TblCompanies.ToList().OrderBy(x => x.CompanyName), "CompanyID", "CompanyName");
            var data = ShrdMaster.Instance.GetMoldDropDown();

            if(data.Count != 0)
            {
                ViewBag.MoldList = data;
                var MoldName = data.FirstOrDefault();
                ViewBag.MoldNAME = MoldName.Text;
            }
            else
            {
                ViewBag.MoldList = new List<SelectListItem>();
                ViewBag.MoldNAME = "";
            }


            var EmpList = ShrdMaster.Instance.GetEmployeeName();

            if (EmpList.Count() != 0)
            {
                ViewBag.EmployeeName = EmpList.OrderBy(x => x.Text).ToList(); ;
            }

            else
            {
                ViewBag.EmployeeName = new List<SelectListItem>();
            }



            //ViewBag.MoldID = MoldName.Value;

            var Molddata = db.Database.SqlQuery<MoldDropdown>("exec procMoldDropdown @CompanyID", new SqlParameter("@CompanyID", CID)).ToList<MoldDropdown>();

            int MID = 0;
            if (Molddata.Count() != 0)
            {
                MID = Molddata.FirstOrDefault().MoldDataID;
            }

            ShrdMaster.Instance.DeleteChecksheetEmptyDate(MID);

            var AllMaintenance = db.TblInspections.Where(x => x.MoldDataID == MID && x.CompanyID == CID).OrderByDescending(x => x.InspectID).ToList();

            DateTime Date = new DateTime();

            DateTime? LastDate = new DateTime();

            List<FinalChecklstResult> Newdata = new List<FinalChecklstResult>();

            foreach (var x in AllMaintenance)
            {
                if (x.InspectDate != null && x.InspectDate != DateTime.MinValue)
                {
                    LastDate = x.InspectDate;
                    break;
                }
            }

            //Date = LastDate.Value.AddDays(1);

            //string NewDate = ShrdMaster.Instance.ReturnOnlyDate(Date);

            string NewDate = ShrdMaster.Instance.ReturnOnlyDate(Date);
            if (NewDate == "0001-01-02")
            {
                NewDate = "1753-01-01";
            }

            Newdata = ShrdMaster.Instance.GetChecksheetData(con, NewDate, MID, 1, 0);
            var tblCat = db.TblCategories.Where(X=> X.CompanyID == CID).ToList().OrderBy(x => x.CategoryName);

            int? Ii1 = 0;
            int? Ii2 = 0;
            int? Ii3 = 0;

            if (Newdata.Count != 0)
            {
                Ii1 = Newdata.FirstOrDefault().InspectedBy1;
                Ii2 = Newdata.FirstOrDefault().InspectedBy2;
                Ii3 = Newdata.FirstOrDefault().InspectedBy3;
            }



            var InspectList = db.TblInspections.Where(X => X.MoldDataID == MID && X.CompanyID == CID).ToList();
            int CountChecksheet = 0;
            CountChecksheet = InspectList.Count();
            ViewBag.CheckSheetCount = CountChecksheet;

            if (CountChecksheet == 0)
            {
                ViewBag.InspectedBy1 = 0;
                ViewBag.InspectedBy2 = 0;
                ViewBag.InspectedBy3 = 0;
            }
            else if (CountChecksheet == 1)
            {
                ViewBag.InspectedBy1 = Ii1;
                ViewBag.InspectedBy2 = 0;
                ViewBag.InspectedBy3 = 0;
            }
            else if (CountChecksheet == 2)
            {
                ViewBag.InspectedBy1 = Ii1;
                ViewBag.InspectedBy2 = Ii3;
                ViewBag.InspectedBy3 = 0;
            }
            else
            {
                ViewBag.InspectedBy1 = Ii1;
                ViewBag.InspectedBy2 = Ii2;
                ViewBag.InspectedBy3 = Ii3;
            }

            List<tblCategoryViewModel> TCM = new List<tblCategoryViewModel>();

            foreach (var x in tblCat)
            {
                tblCategoryViewModel TVM = new tblCategoryViewModel();
                TVM.CategoryName = x.CategoryName;
                TVM.ChecksheetData = Newdata.Where(c => c.CategoryName == x.CategoryName).OrderBy(c => c.InspectionName).ToList();

                TCM.Add(TVM);
            }

            return View(TCM);
        }

        public ActionResult DeleteInspectData(int ID, int MID)
        {
            var data = db.TblInspections.Where(x => x.InspectID == ID).FirstOrDefault();
            db.TblInspections.Remove(data);
            db.SaveChanges();
            return RedirectToAction("UpdateChecklist", new { MID = MID });
        }

        public void EditAdditionalMaintenance(tblInspections model)
        {
            if (model.InspectID != 0)
            {
                int CID = ShrdMaster.Instance.GetCompanyID();
                var data = db.TblInspections.Where(s => s.InspectID == model.InspectID).FirstOrDefault();
                data.AdditionalMaintenance = model.AdditionalMaintenance;
                db.SaveChanges();
            }
        }

        public ActionResult UpdateChecklist(int MID = 0)
        {
            //DateTime Date = System.DateTime.Now;
            DateTime Date = new DateTime();

            int CID = ShrdMaster.Instance.GetCompanyID();

            ShrdMaster.Instance.DeleteChecksheetEmptyDate(MID);
            var AllMaintenance = db.TblInspections.Where(x => x.MoldDataID == MID && x.CompanyID == CID).OrderByDescending(x => x.InspectID).ToList();
            List<FinalChecklstResult> data = new List<FinalChecklstResult>();
            DateTime? LastDate = new DateTime();
            List<tblCategoryViewModel> TCM = new List<tblCategoryViewModel>();

            var tblCat = db.TblCategories.Where(X=> X.CompanyID == CID).ToList().OrderBy(x => x.CategoryName);

            if (AllMaintenance.Count() == 0)
            {
                data = new List<FinalChecklstResult>();
                //Date  = System.DateTime.Now;
                //data =  ShrdMaster.Instance.CreateChecksheetData(con, MID);
            }
            else
            {
                //DateTime? LastDate = new DateTime();
                //LastDate = AllMaintenance.FirstOrDefault().InspectDate;

                foreach (var x in AllMaintenance)
                {
                    if (x.InspectDate != null && x.InspectDate != DateTime.MinValue)
                    {
                        LastDate = x.InspectDate;
                        break;
                    }
                }

                //Date = LastDate.Value.AddDays(1);
                string NewDate = ShrdMaster.Instance.ReturnOnlyDate(Date);
                data = ShrdMaster.Instance.GetChecksheetData(con, NewDate, MID, 1, 0);
            }


            var InspectList = db.TblInspections.Where(X => X.MoldDataID == MID).ToList();
            int CountChecksheet = 0;
            CountChecksheet = InspectList.Count();
            ViewBag.CheckSheetCount = CountChecksheet;


            foreach (var x in tblCat)
            {
                tblCategoryViewModel TCV = new tblCategoryViewModel();
                TCV.CategoryName = x.CategoryName;
                TCV.ChecksheetData = data.Where(c => c.CategoryName == x.CategoryName).OrderBy(c => c.InspectionName).ToList();
                TCM.Add(TCV);
            }

            //var data = ShrdMaster.Instance.UpdateRecords(con, NewDate, MID);

            //var tblin = db.TblInspections.Where(x => x.MoldDataID == MID).FirstOrDefault();
            //if (tblin == null)
            //{
            //    tblInspections tb = new tblInspections();
            //    tb.MoldDataID = MID;
            //    tb.InspectDate = System.DateTime.Now;

            //    db.TblInspections.Add(tb);
            //    db.SaveChanges();
            //}

            DateTime InspectedDateTo = Date.AddDays(2);
            //var Maintenance = db.TblInspections.Where(x => x.MoldDataID == MID && DbFunctions.TruncateTime(x.InspectDate) >= Date.Date && DbFunctions.TruncateTime(x.InspectDate) <= InspectedDateTo.Date).ToList();

            int? i1 = 0;
            int? i2 = 0;
            int? i3 = 0;


            tblInspections M1 = new tblInspections();
            tblInspections M2 = new tblInspections();
            tblInspections M3 = new tblInspections();

            List<tblInspections> Mdata = new List<tblInspections>();

            int? Ii1 = 0;
            int? Ii2 = 0;
            int? Ii3 = 0;

            if (data.Count != 0)
            {
                Ii1 = data.FirstOrDefault().InspectID1;
                i1 = data.FirstOrDefault().InspectedBy1;
                M1 = db.TblInspections.Where(x => x.InspectID == Ii1).FirstOrDefault();
                Ii2 = data.FirstOrDefault().InspectID2;
                Ii3 = data.FirstOrDefault().InspectID3;


                if (Ii2 != Ii1)
                {
                    i2 = data.FirstOrDefault().InspectedBy2;
                    M2 = db.TblInspections.Where(x => x.InspectID == Ii2).FirstOrDefault();
                }

                if (Ii3 != Ii2)
                {
                    i3 = data.FirstOrDefault().InspectedBy3;
                    M3 = db.TblInspections.Where(x => x.InspectID == Ii3).FirstOrDefault();
                }
             }


            Mdata.Add(M1);
            Mdata.Add(M2);
            Mdata.Add(M3);


            if (CountChecksheet == 0)
            {
                ViewBag.InspectedBy1 = 0;
                ViewBag.InspectedBy2 = 0;
                ViewBag.InspectedBy3 = 0;
            }
            else if (CountChecksheet == 1)
            {
                ViewBag.InspectedBy1 = i1;
                ViewBag.InspectedBy2 = 0;
                ViewBag.InspectedBy3 = 0;
            }
            else if (CountChecksheet == 2)
            {
                ViewBag.InspectedBy1 = i1;
                ViewBag.InspectedBy2 = i2;
                ViewBag.InspectedBy3 = 0;
            }
            else
            {
                ViewBag.InspectedBy1 = i1;
                ViewBag.InspectedBy2 = i2;
                ViewBag.InspectedBy3 = i3;
            }

            //ViewBag.InspectedBy1 = i1;
            //ViewBag.InspectedBy2 = i2;
            //ViewBag.InspectedBy3 = i3;

            var CommonPartial = RenderRazorViewToString(this.ControllerContext, "_CommonChecksheet", TCM);
            //var LastShotInspection = RenderRazorViewToString(this.ControllerContext, "_LastShotInspection", data.Where(x => x.CategoryName == "Last Shot Inspection").OrderBy(x => x.InspectionName));
            //var WearCheck = RenderRazorViewToString(this.ControllerContext, "_WearCheck", data.Where(x => x.CategoryName == "Wear Check").OrderBy(x => x.InspectionName));
            //var CheckForDamage = RenderRazorViewToString(this.ControllerContext, "_CheckForDamage", data.Where(x => x.CategoryName == "Check For Damage").OrderBy(x => x.InspectionName));
            //var Task = RenderRazorViewToString(this.ControllerContext, "_Task", data.Where(x => x.CatID == 4).OrderBy(x => x.InspectionName));
            var AdditionalMaintenance = RenderRazorViewToString(this.ControllerContext, "_AdditionalMaintenanace", Mdata);

            return Json(new { CommonPartial, AdditionalMaintenance, i1, i2, i3 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetInspectedBy(DateTime InspectedDate, int MID)
        {
            string NewDate = ShrdMaster.Instance.ReturnOnlyDate(InspectedDate);
            DateTime InspectedDateTo = InspectedDate.AddDays(2);

            var Maintenance = db.TblInspections.Where(x => x.MoldDataID == MID && DbFunctions.TruncateTime(x.InspectDate) >= InspectedDate.Date && DbFunctions.TruncateTime(x.InspectDate) <= InspectedDateTo.Date).ToList();

            //var Maintenance = db.TblInspections.Where(x => x.MoldDataID == MID && DbFunctions.TruncateTime(x.InspectDate) == Date.Date).ToList();

            //List<tblInspections> MD = new List<tblInspections>();
            //MD = Maintenance;

            int? i = 0;

            foreach (var x in Maintenance)
            {
                i = x.InspectedBy;
                break;
            }
            ViewBag.InspectedBy = i;
            return Json(i, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ChangeOnDate(int MID, DateTime Date)
        {
            string NewDate = ShrdMaster.Instance.ReturnOnlyDate(Date);
            var data = ShrdMaster.Instance.UpdateRecords(con, NewDate, MID);
            DateTime DDate = Convert.ToDateTime(NewDate);

            var LastShotInspection = RenderRazorViewToString(this.ControllerContext, "_LastShotInspection", data.Where(x => x.CategoryName == "Last Shot Inspection").OrderBy(x => x.InspectionName));
            var WearCheck = RenderRazorViewToString(this.ControllerContext, "_WearCheck", data.Where(x => x.CategoryName == "Wear Check").OrderBy(x => x.InspectionName));
            var CheckForDamage = RenderRazorViewToString(this.ControllerContext, "_CheckForDamage", data.Where(x => x.CategoryName == "Check For Damage").OrderBy(x => x.InspectionName));
            var Task = RenderRazorViewToString(this.ControllerContext, "_Task", data.Where(x => x.CatID == 4).OrderBy(x => x.InspectionName));

            DateTime InspectedDateTo = DDate.AddDays(2);
            var Maintenance = db.TblInspections.Where(x => x.MoldDataID == MID && DbFunctions.TruncateTime(x.InspectDate) >= DDate.Date && DbFunctions.TruncateTime(x.InspectDate) <= InspectedDateTo.Date).ToList();


            int? i1 = 0;
            int? i2 = 0;
            int? i3 = 0;


            if (Maintenance.Count() != 0)
            {
                i1 = Maintenance[0].InspectedBy;
                i2 = Maintenance[1].InspectedBy;
                i3 = Maintenance[2].InspectedBy;
            }


            //foreach (var x in Maintenance)
            //{
            //    i1 = x.InspectedBy;
            //    break;
            //}


            ViewBag.InspectedBy1 = i1;
            ViewBag.InspectedBy2 = i2;
            ViewBag.InspectedBy3 = i3;

            var AdditionalMaintenance = RenderRazorViewToString(this.ControllerContext, "_AdditionalMaintenanace", Maintenance);

            return Json(new { LastShotInspection, WearCheck, CheckForDamage, Task, AdditionalMaintenance, i1, i2, i3 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult NextDateFunc(int MID, DateTime Date)
        {
            string NewDate = ShrdMaster.Instance.ReturnOnlyDate(Date);

            var Inspect = db.TblInspections.Where(x => DbFunctions.TruncateTime(x.InspectDate) == Date && x.MoldDataID == MID).FirstOrDefault();

            var data = ShrdMaster.Instance.GetChecksheetData(con, NewDate, MID, 1, 0);

            DateTime DDate = Convert.ToDateTime(Date);

            List<tblCategoryViewModel> TCM = new List<tblCategoryViewModel>();
            var tblCat = db.TblCategories.ToList().OrderBy(x => x.CategoryName);

            foreach (var x in tblCat)
            {
                tblCategoryViewModel TVM = new tblCategoryViewModel();
                TVM.CategoryName = x.CategoryName;
                TVM.ChecksheetData = data.Where(c => c.CategoryName == x.CategoryName).OrderBy(c => c.InspectionName).ToList();

                TCM.Add(TVM);
            }

            var InspectList = db.TblInspections.Where(X => X.MoldDataID == MID).ToList();
            int CountChecksheet = 0;
            CountChecksheet = InspectList.Count();
            ViewBag.CheckSheetCount = CountChecksheet;

            //var LastShotInspection = RenderRazorViewToString(this.ControllerContext, "_LastShotInspection", data.Where(x => x.CategoryName == "Last Shot Inspection").OrderBy(x => x.InspectionName));
            //var WearCheck = RenderRazorViewToString(this.ControllerContext, "_WearCheck", data.Where(x => x.CategoryName == "Wear Check").OrderBy(x => x.InspectionName));
            //var CheckForDamage = RenderRazorViewToString(this.ControllerContext, "_CheckForDamage", data.Where(x => x.CategoryName == "Check For Damage").OrderBy(x => x.InspectionName));
            //var Task = RenderRazorViewToString(this.ControllerContext, "_Task", data.Where(x => x.CatID == 4).OrderBy(x => x.InspectionName));
            DateTime InspectedDateTo = DDate.AddDays(2);

            var AllMaintenance = db.TblInspections.Where(x => x.MoldDataID == MID).OrderByDescending(X => X.InspectDate).ToList();


            int? i1 = 0;
            int? i2 = 0;
            int? i3 = 0;

            tblInspections M1 = new tblInspections();
            tblInspections M2 = new tblInspections();
            tblInspections M3 = new tblInspections();
            List<tblInspections> Mdata = new List<tblInspections>();


            int? Ii1 = 0;
            int? Ii2 = 0;
            int? Ii3 = 0;


            if (data.Count != 0)
            {
                Ii1 = data.FirstOrDefault().InspectID1;
                Ii2 = data.FirstOrDefault().InspectID2;
                Ii3 = data.FirstOrDefault().InspectID3;
                i1 = data.FirstOrDefault().InspectedBy1;
                i2 = data.FirstOrDefault().InspectedBy2;
                i3 = data.FirstOrDefault().InspectedBy3;
                M1 = db.TblInspections.Where(x => x.InspectID == Ii1).FirstOrDefault();
                M2 = db.TblInspections.Where(x => x.InspectID == Ii2).FirstOrDefault();
                M3 = db.TblInspections.Where(x => x.InspectID == Ii3).FirstOrDefault();
            }

            Mdata.Add(M1);
            Mdata.Add(M2);
            Mdata.Add(M3);

            if (CountChecksheet == 0)
            {
                ViewBag.InspectedBy1 = 0;
                ViewBag.InspectedBy2 = 0;
                ViewBag.InspectedBy3 = 0;
            }
            else if (CountChecksheet == 1)
            {
                ViewBag.InspectedBy1 = i1;
                ViewBag.InspectedBy2 = 0;
                ViewBag.InspectedBy3 = 0;
            }
            else if (CountChecksheet == 2)
            {
                ViewBag.InspectedBy1 = i1;
                ViewBag.InspectedBy2 = i3;
                ViewBag.InspectedBy3 = 0;
            }
            else
            {
                ViewBag.InspectedBy1 = i1;
                ViewBag.InspectedBy2 = i2;
                ViewBag.InspectedBy3 = i3;
            }

            //ViewBag.InspectedBy1 = i1;
            //ViewBag.InspectedBy2 = i2;
            //ViewBag.InspectedBy3 = i3;

            var CommonPartial = RenderRazorViewToString(this.ControllerContext, "_CommonChecksheet", TCM);
            var AdditionalMaintenance = RenderRazorViewToString(this.ControllerContext, "_AdditionalMaintenanace", Mdata);
            return Json(new { CommonPartial, AdditionalMaintenance, i1, i2, i3 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddNewInspections(int MID, string Date = "")
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            DateTime CurrentDate = System.DateTime.Now;
            string NDATE = ShrdMaster.Instance.ReturnOnlyDate(CurrentDate);

            DateTime NDDate = new DateTime();

            if (Date != "")
            {
                NDDate = Convert.ToDateTime(Date);
            }
            else
            {
                NDDate = System.DateTime.Now;
            }

            var TBLins = db.TblInspections.Where(x => x.MoldDataID == MID && x.CompanyID == CID).ToList();

            string NewDate = "";

            if (TBLins.Count <=2 )
            {
                NewDate = ShrdMaster.Instance.ReturnOnlyDate(NDDate.AddDays(-1));
            }
            else
            {
                NewDate = ShrdMaster.Instance.ReturnOnlyDate(NDDate);
            }

            List<FinalChecklstResult> data = new List<FinalChecklstResult>();

            data = ShrdMaster.Instance.GetChecksheetData(con, NewDate, MID, 1, 1);

            var InspectList = db.TblInspections.Where(x => x.MoldDataID == MID && x.CompanyID == CID).ToList();
            int CountChecksheet = 0;
            CountChecksheet = InspectList.Count();
            ViewBag.CheckSheetCount = CountChecksheet;


            DateTime DDate = Convert.ToDateTime(NewDate);


            List<tblCategoryViewModel> TCM = new List<tblCategoryViewModel>();
            var tblCat = db.TblCategories.Where(X=> X.CompanyID == CID).ToList().OrderBy(x => x.CategoryName);

            foreach (var x in tblCat)
            {
                tblCategoryViewModel TVM = new tblCategoryViewModel();
                TVM.CategoryName = x.CategoryName;
                TVM.ChecksheetData = data.Where(c => c.CategoryName == x.CategoryName).OrderBy(c => c.InspectionName).ToList();

                TCM.Add(TVM);
            }

            DateTime InspectedDateTo = DDate.AddDays(2);

            List<tblInspections> Maintenance = new List<tblInspections>();

            int? i1 = 0;
            int? i2 = 0;
            int? i3 = 0;

            int? Ii1 = 0;
            int? Ii2 = 0;
            int? Ii3 = 0;


            tblInspections M1 = new tblInspections();
            tblInspections M2 = new tblInspections();
            tblInspections M3 = new tblInspections();

            if (data.Count != 0)
            {
                Ii1 = data.FirstOrDefault().InspectID1;
                i1 = data.FirstOrDefault().InspectedBy1;
                M1 = db.TblInspections.Where(x => x.InspectID == Ii1).FirstOrDefault();
                Ii2 = data.FirstOrDefault().InspectID2;
                Ii3 = data.FirstOrDefault().InspectID3;


                if (Ii2 != Ii1)
                {
                    i2 = data.FirstOrDefault().InspectedBy2;
                    M2 = db.TblInspections.Where(x => x.InspectID == Ii2).FirstOrDefault();
                }
                else
                {
                    i3 = data.FirstOrDefault().InspectedBy3;
                    M2 = db.TblInspections.Where(x => x.InspectID == Ii3).FirstOrDefault();
                    Ii3 = Ii2;
                }

                if (Ii3 != Ii2)
                {
                    i3 = data.FirstOrDefault().InspectedBy3;
                    M3 = db.TblInspections.Where(x => x.InspectID == Ii3).FirstOrDefault();
                }
             
            }

            Maintenance.Add(M1);
            Maintenance.Add(M2);
            Maintenance.Add(M3);

            if (CountChecksheet == 0)
            {
                ViewBag.InspectedBy1 = 0;
                ViewBag.InspectedBy2 = 0;
                ViewBag.InspectedBy3 = 0;
            }
            else if (CountChecksheet == 1)
            {
                ViewBag.InspectedBy1 = i1;
                ViewBag.InspectedBy2 = 0;
                ViewBag.InspectedBy3 = 0;
            }
            else if (CountChecksheet == 2)
            {
                ViewBag.InspectedBy1 = i1;
                ViewBag.InspectedBy2 = i2;
                ViewBag.InspectedBy3 = 0;
            }
            else
            {
                ViewBag.InspectedBy1 = i1;
                ViewBag.InspectedBy2 = i2;
                ViewBag.InspectedBy3 = i3;
            }


            var CommonPartial = RenderRazorViewToString(this.ControllerContext, "_CommonChecksheet", TCM);
            var AdditionalMaintenance = RenderRazorViewToString(this.ControllerContext, "_AdditionalMaintenanace", Maintenance);

            return Json(new { CommonPartial, AdditionalMaintenance, i1, i2, i3 }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PreviousDateFunc(int MID, DateTime Date)
        {
            string NewDate = ShrdMaster.Instance.ReturnOnlyDate(Date);


            var data = ShrdMaster.Instance.GetChecksheetData(con, NewDate, MID, 0, 0);

            DateTime DDate = Convert.ToDateTime(Date);


            List<tblCategoryViewModel> TCM = new List<tblCategoryViewModel>();
            var tblCat = db.TblCategories.ToList().OrderBy(x => x.CategoryName);

            foreach (var x in tblCat)
            {
                tblCategoryViewModel TVM = new tblCategoryViewModel();
                TVM.CategoryName = x.CategoryName;
                TVM.ChecksheetData = data.Where(c => c.CategoryName == x.CategoryName).OrderBy(c => c.InspectionName).ToList();

                TCM.Add(TVM);
            }

            var InspectList = db.TblInspections.Where(X => X.MoldDataID == MID).ToList();
            int CountChecksheet = 0;
            CountChecksheet = InspectList.Count();
            ViewBag.CheckSheetCount = CountChecksheet;

            DateTime InspectedDateTo = DDate.AddDays(2);

            var AllMaintenance = db.TblInspections.Where(x => x.MoldDataID == MID).OrderByDescending(X => X.InspectDate).ToList();


            int? i1 = 0;
            int? i2 = 0;
            int? i3 = 0;

            tblInspections M1 = new tblInspections();
            tblInspections M2 = new tblInspections();
            tblInspections M3 = new tblInspections();

            List<tblInspections> Mdata = new List<tblInspections>();

            int? Ii1 = 0;
            int? Ii2 = 0;
            int? Ii3 = 0;

            if (data.Count != 0)
            {
                Ii1 = data.FirstOrDefault().InspectID1;
                Ii2 = data.FirstOrDefault().InspectID2;
                Ii3 = data.FirstOrDefault().InspectID3;
                i1 = data.FirstOrDefault().InspectedBy1;
                i2 = data.FirstOrDefault().InspectedBy2;
                i3 = data.FirstOrDefault().InspectedBy3;
                M1 = db.TblInspections.Where(x => x.InspectID == Ii1).FirstOrDefault();
                M2 = db.TblInspections.Where(x => x.InspectID == Ii2).FirstOrDefault();
                M3 = db.TblInspections.Where(x => x.InspectID == Ii3).FirstOrDefault();
            }

            Mdata.Add(M1);
            Mdata.Add(M2);
            Mdata.Add(M3);

            if (CountChecksheet == 0)
            {
                ViewBag.InspectedBy1 = 0;
                ViewBag.InspectedBy2 = 0;
                ViewBag.InspectedBy3 = 0;
            }
            else if (CountChecksheet == 1)
            {
                ViewBag.InspectedBy1 = i1;
                ViewBag.InspectedBy2 = 0;
                ViewBag.InspectedBy3 = 0;
            }
            else if (CountChecksheet == 2)
            {
                ViewBag.InspectedBy1 = i1;
                ViewBag.InspectedBy2 = i3;
                ViewBag.InspectedBy3 = 0;
            }
            else
            {
                ViewBag.InspectedBy1 = i1;
                ViewBag.InspectedBy2 = i2;
                ViewBag.InspectedBy3 = i3;
            }

            var CommonPartial = RenderRazorViewToString(this.ControllerContext, "_CommonChecksheet", TCM);

            var AdditionalMaintenance = RenderRazorViewToString(this.ControllerContext, "_AdditionalMaintenanace", Mdata);
            return Json(new { CommonPartial, AdditionalMaintenance, i1, i2, i3 }, JsonRequestBehavior.AllowGet);
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

        public ActionResult LastShotInspection()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var Molddata = db.Database.SqlQuery<MoldDropdown>("exec procMoldDropdown @CompanyID", new SqlParameter("@CompanyID",CID)).ToList<MoldDropdown>();
            int MID = Molddata.FirstOrDefault().MoldDataID;

            var AllMaintenance = db.TblInspections.Where(x => x.MoldDataID == MID).OrderByDescending(x => x.InspectID).ToList();
            List<FinalChecklstResult> data = new List<FinalChecklstResult>();
            DateTime Date = new DateTime();
            DateTime? LastDate = new DateTime();

            if (AllMaintenance.Count() == 0)
            {
                Date = System.DateTime.Now;
                data = ShrdMaster.Instance.CreateChecksheetData(con, MID);
            }

            else
            {

                foreach (var x in AllMaintenance)
                {
                    if (x.InspectDate != null && x.InspectDate != DateTime.MinValue)
                    {
                        LastDate = x.InspectDate;
                        break;
                    }
                }

                Date = LastDate.Value.AddDays(1);
                string NewDate = ShrdMaster.Instance.ReturnOnlyDate(Date);
                data = ShrdMaster.Instance.GetChecksheetData(con, NewDate, MID, 0, 0);
            }

            DateTime InspectedDateTo = Date.AddDays(2);

            int? i1 = 0;
            int? i2 = 0;
            int? i3 = 0;

            if (data.Count != 0)
            {
                i1 = data.FirstOrDefault().InspectedBy1;
                i2 = data.FirstOrDefault().InspectedBy2;
                i3 = data.FirstOrDefault().InspectedBy3;
            }

            ViewBag.InspectedBy1 = i1;
            ViewBag.InspectedBy2 = i2;
            ViewBag.InspectedBy3 = i3;

            var FinalRst = data.Where(x => x.CategoryName == "Last Shot Inspection").OrderBy(x => x.InspectionName);
            return PartialView("_LastShotInspection", FinalRst);
        }

        public ActionResult WearCheck()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var Molddata = db.Database.SqlQuery<MoldDropdown>("exec procMoldDropdown @CompanyID", new SqlParameter("@CompanyID",CID)).ToList<MoldDropdown>();

            int MID = Molddata.FirstOrDefault().MoldDataID;

            var LastSavedDate = db.TblInspections.Where(x => x.MoldDataID == MID).OrderByDescending(x => x.InspectID).FirstOrDefault();

            var AllMaintenance = db.TblInspections.Where(x => x.MoldDataID == MID).OrderByDescending(x => x.InspectID).ToList();


            DateTime Date = new DateTime();
            DateTime? LastDate = new DateTime();

            if (AllMaintenance.Count() == 0)
            {
                Date = System.DateTime.Now;
            }

            else
            {
                foreach (var x in AllMaintenance)
                {
                    if (x.InspectDate != null && x.InspectDate != DateTime.MinValue)
                    {
                        LastDate = x.InspectDate;
                        break;
                    }
                }
                Date = LastDate.Value.AddDays(1);
            }
            string NewDate = ShrdMaster.Instance.ReturnOnlyDate(Date);
            var data = ShrdMaster.Instance.GetChecksheetData(con, NewDate, MID, 0, 0);
            var FinalRst = data.Where(x => x.CategoryName == "Wear Check").OrderBy(x => x.InspectionName);
            return PartialView("_WearCheck", FinalRst);
        }

        public ActionResult CheckForDamage()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var Molddata = db.Database.SqlQuery<MoldDropdown>("exec procMoldDropdown @CompanyID", new SqlParameter("@CompanyID", CID)).ToList<MoldDropdown>();

            int MID = Molddata.FirstOrDefault().MoldDataID;

            var LastSavedDate = db.TblInspections.Where(x => x.MoldDataID == MID).OrderByDescending(x => x.InspectID).FirstOrDefault();


            var AllMaintenance = db.TblInspections.Where(x => x.MoldDataID == MID).OrderByDescending(x => x.InspectID).ToList();

            DateTime Date = new DateTime();
            DateTime? LastDate = new DateTime();

            if (AllMaintenance.Count() == 0)
            {
                Date = System.DateTime.Now;
            }

            else
            {
                foreach (var x in AllMaintenance)
                {
                    if (x.InspectDate != null && x.InspectDate != DateTime.MinValue)
                    {
                        LastDate = x.InspectDate;
                        break;
                    }
                }
                Date = LastDate.Value.AddDays(1);
            }
            string NewDate = ShrdMaster.Instance.ReturnOnlyDate(Date);

            var data = ShrdMaster.Instance.GetChecksheetData(con, NewDate, MID, 0, 0);

            var FinalRst = data.Where(x => x.CategoryName == "Check For Damage").OrderBy(x => x.InspectionName);
            return PartialView("_CheckForDamage", FinalRst);
        }

        public ActionResult Task()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var Molddata = db.Database.SqlQuery<MoldDropdown>("exec procMoldDropdown @CompanyID", new SqlParameter("@CompanyID", CID)).ToList<MoldDropdown>();

            int MID = Molddata.FirstOrDefault().MoldDataID;

            var LastSavedDate = db.TblInspections.Where(x => x.MoldDataID == MID).OrderByDescending(x => x.InspectID).FirstOrDefault();

            var AllMaintenance = db.TblInspections.Where(x => x.MoldDataID == MID).OrderByDescending(x => x.InspectID).ToList();

            DateTime Date = new DateTime();
            DateTime? LastDate = new DateTime();

            if (AllMaintenance.Count() == 0)
            {
                Date = System.DateTime.Now;
            }

            else
            {
                foreach (var x in AllMaintenance)
                {
                    if (x.InspectDate != null && x.InspectDate != DateTime.MinValue)
                    {
                        LastDate = x.InspectDate;
                        break;
                    }
                }
                Date = LastDate.Value.AddDays(1);
            }

            string NewDate = ShrdMaster.Instance.ReturnOnlyDate(Date);
            var data = ShrdMaster.Instance.GetChecksheetData(con, NewDate, MID, 0, 0);

            var FinalRst = data.Where(x => x.CatID == 4).OrderBy(x => x.InspectionName);
            return PartialView("_Task", FinalRst);
        }

        public ActionResult AdditionalMaintenance()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            DateTime Date = new DateTime();
            var Molddata = db.Database.SqlQuery<MoldDropdown>("exec procMoldDropdown @CompanyID", new SqlParameter("@CompanyID", CID)).ToList<MoldDropdown>();

            int MID = 0;
            if (Molddata.Count != 0)
            {
                MID = Molddata.FirstOrDefault().MoldDataID;
            }

            string NewDate = ShrdMaster.Instance.ReturnOnlyDate(Date);

            List<FinalChecklstResult> data = new List<FinalChecklstResult>();
            data = ShrdMaster.Instance.GetChecksheetData(con, NewDate, MID, 1, 0);

            int? i1 = 0;
            int? i2 = 0;
            int? i3 = 0;


            tblInspections M1 = new tblInspections();
            tblInspections M2 = new tblInspections();
            tblInspections M3 = new tblInspections();

            List<tblInspections> Mdata = new List<tblInspections>();

            int? Ii1 = 0;
            int? Ii2 = 0;
            int? Ii3 = 0;

            if (data.Count != 0)
            {
                Ii1 = data.FirstOrDefault().InspectID1;
                i1 = data.FirstOrDefault().InspectedBy1;
                M1 = db.TblInspections.Where(x => x.InspectID == Ii1).FirstOrDefault();
                Ii2 = data.FirstOrDefault().InspectID2;
                Ii3 = data.FirstOrDefault().InspectID3;


                if (Ii2 != Ii1)
                {
                    i2 = data.FirstOrDefault().InspectedBy2;
                    M2 = db.TblInspections.Where(x => x.InspectID == Ii2).FirstOrDefault();
                }

                if (Ii3 != Ii2)
                {
                    i3 = data.FirstOrDefault().InspectedBy3;
                    M3 = db.TblInspections.Where(x => x.InspectID == Ii3).FirstOrDefault();
                }
            }


            Mdata.Add(M1);
            Mdata.Add(M2);
            Mdata.Add(M3);
            return PartialView("_AdditionalMaintenanace", Mdata);
        }

        public ActionResult UpdateDate(int InspectID = 0, string Date = "")
        {
            var data = db.TblInspections.Where(x => x.InspectID == InspectID).FirstOrDefault();

            DateTime DD = Date != "" ? Convert.ToDateTime(Date) : new DateTime();

            string NewDate = ShrdMaster.Instance.ReturnOnlyDate(DD);

            var CheckExistingDate = db.TblInspections.Where(x => x.MoldDataID == data.MoldDataID && DbFunctions.TruncateTime(x.InspectDate) == DD.Date).FirstOrDefault();

            if (CheckExistingDate == null)
            {
                if (data != null)
                {
                    data.InspectDate = DD == DateTime.MinValue ? new DateTime() : DD;
                    db.SaveChanges();
                }

                return Json("OK", JsonRequestBehavior.AllowGet);
            }

            return Json("Error", JsonRequestBehavior.AllowGet);

        }

        public void OkAfterUpdate(int OK, int Attention, int NoRun, int InspectionDetailID)
        {
            string ok = "Update tblInspectionDetails set ok=" + OK + ", Attention=" + Attention + ", NoRun=" + NoRun + " where ID=" + InspectionDetailID;

            SqlCommand Command = new SqlCommand(ok, con);
            con.Open();
            Command.ExecuteNonQuery();
            con.Close();
        }

        public void UpdateAdditionalMaintenane(string str, int InspectionID)
        {
            string ok = "Update tblInspections set AdditionalMaintenance=" + str + " where InspectID=" + InspectionID;
            SqlCommand Command = new SqlCommand(ok, con);
            con.Open();
            Command.ExecuteNonQuery();
            con.Close();
        }

        public void UpdateAdditionalComments(string str, int InspectionID)
        {
            string ok = "Update tblInspectionDetails set AdditionalComments='" + str + "' where ID=" + InspectionID;
            SqlCommand Command = new SqlCommand(ok, con);
            con.Open();
            Command.ExecuteNonQuery();
            con.Close();
        }


        public void UpdateInspectedBy(int InspectID = 0, int InspectedBy=0)
        {
            var data = db.TblInspections.Where(x => x.InspectID == InspectID).FirstOrDefault();
            if (data != null)
            {
                data.InspectedBy = InspectedBy;
                db.SaveChanges();
            }
        }
    }
}