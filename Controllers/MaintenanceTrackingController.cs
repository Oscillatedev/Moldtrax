using Moldtrax.Models;
using Moldtrax.Providers;
using Moldtrax.ViewMoldel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Moldtrax.Controllers
{
    [SessionExpireFilter]
    public class MaintenanceTrackingController : Controller
    {
        private MoldtraxDbContext db = new MoldtraxDbContext();
        static string constring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        SqlConnection con = new SqlConnection(constring);
        // GET: MaintenanceTracking
        public ActionResult Index(int CID=0)
        {
            if (CID == 0)
            {
                CID = ShrdMaster.Instance.GetCompanyID();
            }
            var dd = db.TblMoldData.Where(X=> X.CompanyID == CID).ToList().Select(x => new tblMoldData{ MoldDataID = x.MoldDataID, MoldName = x.MoldName + ": " + x.MoldDesc });
            ViewBag.MoldText = new SelectList(dd.ToList().OrderBy(x=> x.MoldName), "MoldDataID", "MoldName");
            int MID;
            MID = ReturnSelectedMoldID(0);
            var data = db.TblMoldData.Where(x => x.MoldDataID == MID).FirstOrDefault();
            if (data != null)
            {
                if (data.MoldTotalCycles == 0)
                {
                    ShrdMaster.Instance.UpdateTotalCycles(MID);
                }
                data.MoldResinType = MID.ToString();
            }

            ViewBag.Companylist = new SelectList(db.TblCompanies.ToList().OrderBy(x=> x.CompanyName), "CompanyID", "CompanyName");
            return View(data == null ? new tblMoldData() : data);
        }

        public List<tblDDMoldConfig> ReturnMoldConfig()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            var data = db.TblDDMoldConfigs.Where(X=> X.CompanyID == CID).ToList();
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

        public List<tblDDMoldConfig2> ReturnMoldConfig2()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            var data = db.TblDDMoldConfig2s.Where(X=> X.CompanyID == CID).ToList();
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

        public void CommonDrop()
        {

            var ConfigData = ReturnMoldConfig();
            List<SelectListItem> MoldConfig = new List<SelectListItem>();
            foreach (var x in ConfigData)
            {
                MoldConfig.Add(new SelectListItem
                {
                    Text = x.MoldConfig,
                    Value = x.ID.ToString()
                });
            }

            var ConfigData2 = ReturnMoldConfig2();
            List<SelectListItem> MoldConfig2 = new List<SelectListItem>();
            foreach (var x in ConfigData2)
            {
                MoldConfig2.Add(new SelectListItem
                {
                    Text = x.MoldConfig,
                    Value = x.ID.ToString()
                });
            }

            int CID = ShrdMaster.Instance.GetCompanyID();

            ViewBag.MoldConfigData = MoldConfig;
            ViewBag.MoldConfig2 = MoldConfig2; 
            var Technician = db.Database.SqlQuery<TechnicianDropDown>("exec procTechnicianDropdown @CompanyID", new SqlParameter("@CompanyID", CID)).ToList();
            List<SelectListItem> Tech = new List<SelectListItem>();
            foreach (var x in Technician)
            {
                Tech.Add(new SelectListItem
                {
                    Text = x.Name,
                    Value =x.EmployeeID.ToString()
                });
            }

            ViewBag.TechnicianList = Tech;
            var MainDropdown = db.Database.SqlQuery<MainRequiredDropDown>("exec procMldPullMaintRequiredDropdown @CompanyID", new SqlParameter("@CompanyID",CID)).ToList();
            List<SelectListItem> StopReason = new List<SelectListItem>();
            foreach (var x in MainDropdown)
            {
                StopReason.Add(new SelectListItem
                {
                    Text = x.StopReason,
                    Value = x.ID.ToString()
                });
            }

            ViewBag.MainDropdown = StopReason;
        }

        public ActionResult GetMaintenanceData(int MoldID=0)
        {
            CommonDrop();
            int NMoldID = ReturnSelectedMoldID(MoldID);
            int NoofRec = 0;
            var data = ShrdMaster.Instance.GetMaintenanceTblRoverSetData(NMoldID).OrderBy(x => x.SetDate).ToList();
            NoofRec = data.Count();
            ViewBag.TotalMold = NoofRec;
            return PartialView("_MoldData", data);
        }

        public ActionResult MaintenanceSchedule()
        {
            return View();
        }

        public int ReturnSelectedMoldID(int ID = 0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            int i = 0;
            if (ID == 0)
            {
                //var data = db.TblMoldData.ToList();
                var data = db.TblMoldData.Where(X=> X.CompanyID == CID).OrderBy(x => x.MoldName).ToList();
                if (data.Count() != 0)
                {
                    var ds = data.FirstOrDefault();
                    i = ds.MoldDataID;
                }
            }

            else if (ID == -1)
            {
                i = 0;
            }
            else
            {
                i = ID;
            }

            return i;
        }


        public void SaveFocusOutMoldInfo(tblRoverSetData model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            if (model.SetID == 0)
            {
                try
                {
                    model.SetDate = model.SetDate == DateTime.MinValue ? null : model.SetDate;
                    model.MldPullDate = model.MldPullDate == DateTime.MinValue ? null : model.MldPullDate;
                    model.CompanyID = CID;
                    db.TblRoverSetDatas.Add(model);
                    db.SaveChanges();
                    //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.PressData.ToString(), GetAction.Create.ToString());

                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                try
                {
                    var dd = db.TblRoverSetDatas.Where(s => s.SetID == model.SetID).OrderBy(x => x.SetDate).FirstOrDefault();
                    dd.SetPressNumb = model.SetPressNumb;
                    dd.SetDate = model.SetDate == DateTime.MinValue ? null : model.SetDate;
                    dd.SetTime = model.SetTime;
                    dd.SetTech = model.SetTech;
                    dd.MoldConfig = model.MoldConfig;
                    dd.MoldConfig2 = model.MoldConfig2;
                    dd.MldPullDate = model.MldPullDate == DateTime.MinValue ? null : model.MldPullDate;
                    dd.MldPullTime = model.MldPullTime;
                    dd.MldPullTech = model.MldPullTech;
                    dd.MldPullMaintRequired = model.MldPullMaintRequired;
                    dd.MoldDataID = model.MoldDataID;
                    dd.CycleCounter = model.CycleCounter;
                    db.SaveChanges();
                    //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.PressData.ToString(), GetAction.Update.ToString());

                }
                catch (Exception ex)
                { }
            }
        }


        public ActionResult AddMoldData()
        {
            CommonDrop();
            return PartialView("_AddMoldData");
        }

        public ActionResult SaveMoldInfoList(List<tblRoverSetData> model, int Moldid)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            if (model != null)
            {
                foreach (var x in model)
                {
                var dd = db.TblRoverSetDatas.Where(s => s.SetID == x.SetID).OrderBy(w => w.SetDate).FirstOrDefault();
                    if (dd != null)
                    {
                        dd.SetPressNumb = x.SetPressNumb;
                        dd.SetDate = x.SetDate == DateTime.MinValue ? null : x.SetDate;
                        dd.SetTime = x.SetTime;
                        dd.SetTech = x.SetTech;
                        dd.MoldConfig = x.MoldConfig;
                        dd.MoldConfig2 = x.MoldConfig2;
                        dd.MldPullDate = x.MldPullDate == DateTime.MinValue ? null : x.MldPullDate;
                        dd.MldPullTime = x.MldPullTime;
                        dd.MldPullTech = x.MldPullTech;
                        dd.MldPullMaintRequired = x.MldPullMaintRequired;
                        //dd.MoldDataID = x.MoldDataID;
                        dd.CycleCounter = x.CycleCounter;
                        db.SaveChanges();
                        //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.PressData.ToString(), GetAction.Update.ToString());

                    }

                    //{
                    //    MainIns.MldProductionCycles = Convert.ToInt32(x.CycleCounter);
                    //    db.SaveChanges();
                    //}
                }
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveSingleMold(tblRoverSetData model, int MoldID)
        {
            var dd = db.TblRoverSetDatas.Where(s => s.SetID == model.SetID).OrderBy(w => w.SetDate).FirstOrDefault();
            int CID = ShrdMaster.Instance.GetCompanyID();

            if (dd != null)
            {
                dd.SetPressNumb = model.SetPressNumb;
                dd.SetDate = model.SetDate == DateTime.MinValue ? null : model.SetDate;
                dd.SetTime = model.SetTime;
                dd.SetTech = model.SetTech;
                dd.MoldConfig = model.MoldConfig;
                dd.MoldConfig2 = model.MoldConfig2;
                dd.MldPullDate = model.MldPullDate == DateTime.MinValue ? null : model.MldPullDate;
                dd.MldPullTime = model.MldPullTime;
                dd.MldPullTech = model.MldPullTech;
                dd.MldPullMaintRequired = model.MldPullMaintRequired;
                //dd.MoldDataID = x.MoldDataID;
                dd.CycleCounter = model.CycleCounter;
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.PressData.ToString(), GetAction.Update.ToString());

            }
            ShrdMaster.Instance.UpdateTotalCycles(MoldID);
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveMoldInfo(tblRoverSetData model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            if (model.SetID == 0)
            {
                try
                {
                    model.SetDate = model.SetDate == DateTime.MinValue ? null : model.SetDate;
                    model.CompanyID = CID;
                    model.MldPullDate = model.MldPullDate == DateTime.MinValue ? null : model.MldPullDate;
                    db.TblRoverSetDatas.Add(model);
                    db.SaveChanges();
                    //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.PressData.ToString(), GetAction.Create.ToString());

                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                try
                {
                    var dd = db.TblRoverSetDatas.Where(s => s.SetID == model.SetID).OrderBy(x => x.SetDate).FirstOrDefault();
                    dd.SetPressNumb = model.SetPressNumb;
                    dd.SetDate = model.SetDate == DateTime.MinValue ? null : model.SetDate;
                    dd.SetTime = model.SetTime;
                    dd.SetTech = model.SetTech;
                    dd.MoldConfig = model.MoldConfig;
                    dd.MoldConfig2 = model.MoldConfig2;
                    dd.MldPullDate = model.MldPullDate == DateTime.MinValue ? null : model.MldPullDate;
                    dd.MldPullTime = model.MldPullTime;
                    dd.MldPullTech = model.MldPullTech;
                    dd.MldPullMaintRequired = model.MldPullMaintRequired;
                    dd.MoldDataID = model.MoldDataID;
                    dd.CycleCounter = model.CycleCounter;
                    db.SaveChanges();
                    //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.PressData.ToString(), GetAction.Update.ToString());

                }

                catch (Exception ex)
                { }
            }

            if (model != null)
            {
                ShrdMaster.Instance.UpdateTotalCycles(model.MoldDataID);
            }

            int NoofRec = 0;

            NoofRec = ShrdMaster.Instance.GetMaintenanceTblRoverSetData(model.MoldDataID).OrderBy(x => x.SetDate).Count();
            ViewBag.TotalMold = NoofRec;
            List<TblRoverSetDataViewModel> data = new List<TblRoverSetDataViewModel>();

            return Json("ok", JsonRequestBehavior.AllowGet);
            //return PartialView("_MoldData", data);
        }

        public ActionResult DeleteMoldRec(string str, int MoldID=0)
        {
            if (str != "")
            {
                str = str.Substring(0, str.LastIndexOf(','));
                using (var ctx = new MoldtraxDbContext())
                {
                    SqlParameter sp = new SqlParameter("@value", str);
                    var result = ctx.Database.ExecuteSqlCommand("exec procDeleteMaintenance @value", sp);
                }
            }

            CommonDrop();

            ShrdMaster.Instance.UpdateTotalCycles(MoldID);
            int NoofRec = 0;
            NoofRec = ShrdMaster.Instance.GetMaintenanceTblRoverSetData(MoldID).OrderBy(x => x.SetDate).Count();
            ViewBag.TotalMold = NoofRec;
            List<TblRoverSetDataViewModel> data = new List<TblRoverSetDataViewModel>();

            return PartialView("_MoldData", data);
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


        public ActionResult CompanyChangeMaintenanceTrack()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var Molddata = db.TblMoldData.Where(x => x.CompanyID == CID).ToList().OrderBy(x => x.MoldName)
                .Select(x => new tblMoldData { MoldDataID = x.MoldDataID, MoldName = x.MoldName + ": " + x.MoldDesc });

            //int MoldID = Molddata.FirstOrDefault().MoldDataID;
            //CommonDrop();
            //GetMaintenanceScheduleDropDown();
            ////var tblroversetdata = db.TblRoverSetDatas.Where(x => x.MoldDataID == MoldID).ToList();
            //var MD = ShrdMaster.Instance.GetMaintenanceTblRoverSetData(MoldID).OrderBy(x => x.SetDate).ToList();

            //var MoldDataView = RenderRazorViewToString(this.ControllerContext, "_MoldData", MD);
            
            return Json(Molddata, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddMoldInfo(tblRoverSetData model)
        {

            if (model.SetID == 0)
            {
                try
                {
                    model.SetDate = model.SetDate == DateTime.MinValue ? null : model.SetDate;
                    model.CompanyID = ShrdMaster.Instance.GetCompanyID();
                    model.MldPullDate = model.MldPullDate == DateTime.MinValue ? null : model.MldPullDate;
                    db.TblRoverSetDatas.Add(model);
                    db.SaveChanges();
                    //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.PressData.ToString(), GetAction.Create.ToString());
                }
                catch (Exception ex)
                {

                }
            }

            if (model != null)
            {
                ShrdMaster.Instance.UpdateTotalCycles(model.MoldDataID);
            }
            CommonDrop();
            //int NMoldID = ReturnSelectedMoldID(MoldID);
            //var data = sd.GetMaintenanceTblRoverSetData(model.MoldDataID).OrderBy(x => x.SetDate).ToList();

            int NoofRec = 0;

            NoofRec = ShrdMaster.Instance.GetMaintenanceTblRoverSetData(model.MoldDataID).OrderBy(x => x.SetDate).Count();

            ViewBag.TotalMold = NoofRec;

            if (model != null)
            {
                model.NewMldPullDate = Convert.ToDateTime(model.MldPullDate).ToString("MM/dd/yyyy");
                model.NewMldPullTime = Convert.ToDateTime(model.MldPullTime).ToString("HH:mm");
                model.NewSetDate = Convert.ToDateTime(model.SetDate).ToString("MM/dd/yyyy");
                model.NewSetTime = Convert.ToDateTime(model.SetTime).ToString("HH:mm");
            }

            List<TblRoverSetDataViewModel> data = new List<TblRoverSetDataViewModel>();
            return Json(model, JsonRequestBehavior.AllowGet);
            //return PartialView("_MoldData", data);
        }

        public JsonResult MoldSorting(int Sort = 0, int MoldID = 0)
            {
            int CID = 0;
            CID = ShrdMaster.Instance.GetCompanyID();

            List<tblRoverSetData> data = new List<tblRoverSetData>();

            data = db.TblRoverSetDatas.Where(x => x.MoldDataID == MoldID && x.CompanyID == CID).ToList();

            if (data.Count() != 0 || data != null)
            {
                if (Sort == 0)
                {
                    data = data.OrderBy(x => x.SetDate).Take(10).ToList();
                }
                else
                {
                    data = data.OrderByDescending(x => x.SetDate).Take(10).ToList();
                }
            }

            List<TblRoverSetDataViewModel> Nquery = new List<TblRoverSetDataViewModel>();

            foreach (var x in data)
            {
                TblRoverSetDataViewModel tsv = new TblRoverSetDataViewModel();
                tsv.MoldDataID = x.MoldDataID;
                tsv.SetID = x.SetID;
                tsv.SetDate = x.SetDate;
                tsv.SetTime = x.SetTime;
                tsv.SetTech = x.SetTech;
                tsv.SetPressNumb = x.SetPressNumb;
                tsv.MldPullMaintRequired = x.MldPullMaintRequired;
                tsv.MldPullDate = x.MldPullDate;
                tsv.MldPullTime = x.MldPullTime;
                tsv.MldPullTech = x.MldPullTech;
                tsv.MoldConfig = x.MoldConfig;
                tsv.MldRepairedDate = x.MldRepairedDate;
                tsv.MldRepairedTime = x.MldRepairedTime;
                tsv.MldRepairdBy = x.MldRepairdBy;
                tsv.MldWorkOrder = x.MldWorkOrder;
                tsv.MldProductionCycles = x.MldProductionCycles;
                tsv.ImageExtension = x.ImageExtension;
                tsv.CycleCounter = x.CycleCounter;
                tsv.MoldConfig2 = x.MoldConfig2;
                tsv.MoldDefectMapPath = x.MoldDefectMapPath;
                tsv.NewSetTime = Convert.ToDateTime(x.SetTime).ToString("HH:mm");
                tsv.NewMldPullTime = Convert.ToDateTime(x.MldPullTime).ToString("HH:mm");
                tsv.NewSetDate = Convert.ToDateTime(x.SetDate).ToString("MM/dd/yyyy");
                tsv.NewMldPullDate = Convert.ToDateTime(x.MldPullDate).ToString("MM/dd/yyyy");
                Nquery.Add(tsv);
            }

            return new JsonResult()
            {
                ContentType = "application/json",
                Data = Nquery,
                MaxJsonLength = int.MaxValue,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
            };
        }

        public JsonResult MoldTimeSorting(int Sort = 0, int MoldID=0)
        {
            int CID = 0;
            CID = ShrdMaster.Instance.GetCompanyID();

            List<tblRoverSetData> data = new List<tblRoverSetData>();

            data = db.TblRoverSetDatas.Where(x => x.MoldDataID == MoldID && x.CompanyID == CID).ToList();

            if (data.Count() != 0 || data != null)
            {
                if (Sort == 0)
                {
                    data = data.OrderBy(x => Convert.ToDateTime(x.SetTime).TimeOfDay).Take(10).ToList();
                }
                else
                {
                    data = data.OrderByDescending(x => Convert.ToDateTime(x.SetTime).TimeOfDay).Take(10).ToList();
                }
            }

            List<TblRoverSetDataViewModel> Nquery = new List<TblRoverSetDataViewModel>();

            foreach (var x in data)
            {
                TblRoverSetDataViewModel tsv = new TblRoverSetDataViewModel();
                tsv.MoldDataID = x.MoldDataID;
                tsv.SetID = x.SetID;
                tsv.SetDate = x.SetDate;
                tsv.SetTime = x.SetTime;
                tsv.SetTech = x.SetTech;
                tsv.SetPressNumb = x.SetPressNumb;
                tsv.MldPullMaintRequired = x.MldPullMaintRequired;
                tsv.MldPullDate = x.MldPullDate;
                tsv.MldPullTime = x.MldPullTime;
                tsv.MldPullTech = x.MldPullTech;
                tsv.MoldConfig = x.MoldConfig;
                tsv.MldRepairedDate = x.MldRepairedDate;
                tsv.MldRepairedTime = x.MldRepairedTime;
                tsv.MldRepairdBy = x.MldRepairdBy;
                tsv.MldWorkOrder = x.MldWorkOrder;
                tsv.MldProductionCycles = x.MldProductionCycles;
                tsv.ImageExtension = x.ImageExtension;
                tsv.CycleCounter = x.CycleCounter;
                tsv.MoldConfig2 = x.MoldConfig2;
                tsv.MoldDefectMapPath = x.MoldDefectMapPath;
                tsv.NewSetTime = Convert.ToDateTime(x.SetTime).ToString("HH:mm");
                tsv.NewMldPullTime = Convert.ToDateTime(x.MldPullTime).ToString("HH:mm");
                tsv.NewSetDate = Convert.ToDateTime(x.SetDate).ToString("MM/dd/yyyy");
                tsv.NewMldPullDate = Convert.ToDateTime(x.MldPullDate).ToString("MM/dd/yyyy");
                Nquery.Add(tsv);
            }

            return new JsonResult()
            {
                ContentType = "application/json",
                Data = Nquery,
                MaxJsonLength = int.MaxValue,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
            };
        }

        public JsonResult GetTblRoversetDataFilter(int MoldID = 0, int pageIndex = 0, int pageSize = 0, int Order = 0, int DateOrder=0)
        {
            //System.Threading.Thread.Sleep(4000);

            //var query = sd.GetMaintenanceTblRoverSetData(MoldID).OrderBy(x => x.SetDate).Skip(pageIndex);

            //List<TblRoverSetDataViewModel> query = new IEnumerable<TblRoverSetDataViewModel>();
            IEnumerable<TblRoverSetDataViewModel> query = new List<TblRoverSetDataViewModel>();

            if (Order == 0)
            {
                if (DateOrder == 0)
                {
                    query = ShrdMaster.Instance.GetMaintenanceTblRoverSetData(MoldID).OrderBy(x => x.SetDate).Skip(pageIndex).ToList();
                }
                else
                {
                    query = ShrdMaster.Instance.GetMaintenanceTblRoverSetData(MoldID).OrderBy(x => Convert.ToDateTime(x.SetTime).TimeOfDay).Skip(pageIndex).ToList();
                }
            }
            else
            {
                if (DateOrder == 0)
                {
                    query = ShrdMaster.Instance.GetMaintenanceTblRoverSetData(MoldID).OrderByDescending(x => x.SetDate).Skip(pageIndex).ToList();
                }
                else
                {
                    query = ShrdMaster.Instance.GetMaintenanceTblRoverSetData(MoldID).OrderByDescending(x => Convert.ToDateTime(x.SetTime).TimeOfDay).Skip(pageIndex).ToList();
                }
            }

            //var query = db.TblRoverSetDatas.Where(x => x.MoldDataID == MoldID).ToList().OrderBy(x=> x.SetDate).Skip(pageIndex);
            int count = query.Count() > 10 ? 10 : query.Count();
            query = query.Take(count);

            List<TblRoverSetDataViewModel> Nquery = new List<TblRoverSetDataViewModel>();

            foreach (var x in query)
            {
                TblRoverSetDataViewModel tsv = new TblRoverSetDataViewModel();
                tsv.MoldDataID = x.MoldDataID;
                tsv.SetID = x.SetID;
                tsv.SetDate = x.SetDate;
                tsv.SetTime = x.SetTime;
                tsv.SetTech = x.SetTech;
                tsv.SetPressNumb = x.SetPressNumb;
                tsv.MldPullMaintRequired = x.MldPullMaintRequired;
                tsv.MldPullDate = x.MldPullDate;
                tsv.MldPullTime = x.MldPullTime;
                tsv.MldPullTech = x.MldPullTech;
                tsv.MoldConfig = x.MoldConfig;
                tsv.MldRepairedDate = x.MldRepairedDate;
                tsv.MldRepairedTime = x.MldRepairedTime;
                tsv.MldRepairdBy = x.MldRepairdBy;
                tsv.MldWorkOrder = x.MldWorkOrder;
                tsv.MldProductionCycles = x.MldProductionCycles;
                tsv.ImageExtension = x.ImageExtension;
                tsv.CycleCounter = x.CycleCounter;
                tsv.MoldConfig2 = x.MoldConfig2;
                tsv.MoldDefectMapPath = x.MoldDefectMapPath;
                tsv.NewSetTime = Convert.ToDateTime(x.SetTime).ToString("HH:mm");
                tsv.NewMldPullTime = Convert.ToDateTime(x.MldPullTime).ToString("HH:mm");
                tsv.NewSetDate = Convert.ToDateTime(x.SetDate).ToString("MM/dd/yyyy");
                tsv.NewMldPullDate = Convert.ToDateTime(x.MldPullDate).ToString("MM/dd/yyyy");
                Nquery.Add(tsv);
            }

            return new JsonResult()
            {
                ContentType = "application/json",
                Data = Nquery,
                MaxJsonLength = int.MaxValue,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
            };
        }

        #region Maintenance Scheduled

        public ActionResult MaintenanceScheduleGetData(int MoldID=0, int MOLDChange=0)
        {
            var ds = new MoldtraxDbContext();
            GetMaintenanceScheduleDropDown();
            //int MID = ReturnSelectedMoldID(MoldID);

            //var TSC = ds.Database.SqlQuery<TotalCycleCount>("exec proTotalCycleCountAsp @MoldDataID", new SqlParameter("MoldDataID", MID)).ToList<TotalCycleCount>();

            int NewMoldID = ReturnSelectedMoldID(MoldID);

            //ShrdMaster.Instance.UpdateTotalCycles(NewMoldID);
            //var MD = ShrdMaster.Instance.GetMaintenanceTblRoverSetData(NewMoldID).OrderBy(x => x.SetDate).ToList();
            int NoofRec = 0;
            NoofRec = ShrdMaster.Instance.GetMaintenanceTblRoverSetData(NewMoldID).OrderBy(x => x.SetDate).Count();
            ViewBag.TotalMold = NoofRec;

            List<TblRoverSetDataViewModel> MD = new List<TblRoverSetDataViewModel>();

            //if (TSC.Count() == 0)
            //{
            //    ViewBag.CounterVal = 0;
            //}
            //else
            //{
            //    TotalCycleCount cs = TSC.FirstOrDefault();
            //    ViewBag.CounterVal = cs.TotalCycles;
            //}

            CommonDrop();
            MaintenanceScheduleCommon MSC = new MaintenanceScheduleCommon();
            //var OInfo = db.TblMoldData.Where(x => x.MoldDataID == MID).FirstOrDefault();

            var MDD = ds.TblMoldData.Where(x => x.MoldDataID == NewMoldID).FirstOrDefault();

            //if (MDD.MoldTotalCycles == null)
            //{
            //    int? CounterVal = 0;
            //    foreach (var x in MD)
            //    {
            //        CounterVal += x.CycleCounter == null ? 0 : x.CycleCounter;
            //    }

            //    //ViewBag.CounterVal = CounterVal;
            //    MDD.MoldTotalCycles = CounterVal;
            //}

            int CID = ShrdMaster.Instance.GetCompanyID();



            var TBS = ds.TblSchedules.Where(x => x.schMoldDataID == NewMoldID && x.CompanyID == CID).ToList();

            MSC.MoldData = MDD == null ? new tblMoldData() : MDD;
            MSC.TbScheduleList = TBS == null ? new List<tblSchedule>() : TBS;

            if (MOLDChange == 0)
            {
                return PartialView("_MaintenanceSchedule", MSC);
            }

            else
            {
                var MoldData = RenderRazorViewToString(this.ControllerContext, "_MoldData", MD);
                var OtherInfo = RenderRazorViewToString(this.ControllerContext, "_MaintenanceSchedule", MSC);


                return new JsonResult()
                {
                    ContentType = "application/json",
                    Data = new { MoldData, OtherInfo },
                    MaxJsonLength = int.MaxValue
                };

                //return PartialView("_MaintenanceSchedule", new { MoldData, OtherInfo });
            }
        }

        //public ActionResult ResetPMAlert(int MoldID)
        //{
        //    var NewDB = new MoldtraxDbContext();
        //    var Mold = NewDB.TblMoldData.Where(x => x.MoldDataID == MoldID).FirstOrDefault();
        //    Mold.MoldOutPressPMYellowCycles = (Convert.ToInt32(Mold.MoldOutPressPMYellowCycles) + Convert.ToInt32(Mold.MoldOutPressPMFreq));
        //    Mold.MoldOutPressPMRedCycles = (Convert.ToInt32(Mold.MoldOutPressPMRedCycles) + Convert.ToInt32(Mold.MoldOutPressPMFreqRed));
        //    db.SaveChanges();
        //    //var TSC = NewDB.Database.SqlQuery<TotalCycleCount>("exec proTotalCycleCountAsp @MoldDataID", new SqlParameter("MoldDataID", MoldID));
        //    var NewTotalCount = ShrdMaster.Instance.GetTotalAvailableCycle(con, MoldID);
        //    //var NewTotalCount1 = ShrdMaster.Instance.GetTotalAvailableCycle(con, MoldID);
        //    return Json(new { Mold, NewTotalCount.TotalCycles } , JsonRequestBehavior.AllowGet);
        //}

        public void UpdateAllMold()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            var data = db.TblMoldData.Where(X=> X.CompanyID == CID).ToList();
            foreach (var x in data)
            {
                ShrdMaster.Instance.UpdateTotalCycles(x.MoldDataID);
            }
        }


        public ActionResult ResetPMAlert(tblMoldData model)
        {
            var ds = new MoldtraxDbContext();
            int CID = ShrdMaster.Instance.GetCompanyID();

            GetMaintenanceScheduleDropDown();
            int MID = ReturnSelectedMoldID(model.MoldDataID);

            int NewMoldID = ReturnSelectedMoldID(model.MoldDataID);
            var MD = ShrdMaster.Instance.GetMaintenanceTblRoverSetData(NewMoldID).OrderBy(x => x.SetDate).ToList();

            //double? CounterVal = 0;
            //foreach (var x in MD)
            //{
            //    CounterVal += x.CycleCounter == null ? 0 : x.CycleCounter;
            //}

            //ViewBag.CounterVal = CounterVal;

            var data = db.TblMoldData.Where(x => x.MoldDataID == model.MoldDataID).FirstOrDefault();
            data.RepairStatus = model.RepairStatus;
            //data.MoldTotalCycles = data.MoldTotalCycles + (Convert.ToInt32(model.MoldOutPressPMFreq));

            //if (data.MoldTotalCycles > data.MoldOutPressPMYellowCycles)
            //{
            //    data.MoldTotalCycles = data.MoldTotalCycles - Convert.ToInt32(model.MoldOutPressPMFreq);
            //} 
            data.MoldOutPressPMYellowCycles = (Convert.ToInt32(data.MoldOutPressPMFreq) + Convert.ToInt32(data.MoldTotalCycles));
            data.MoldOutPressPMRedCycles =    (Convert.ToInt32(data.MoldOutPressPMFreqRed) + Convert.ToInt32(data.MoldTotalCycles));

            data.RepairStatusLocationId = model.RepairStatusLocationId;
            data.MoldInPressPMFreq = model.MoldInPressPMFreq;
            data.MoldOutPressPMFreq = model.MoldOutPressPMFreq;
            data.MoldCyclesToRedLimit = model.MoldCyclesToRedLimit;
            //data.MoldOutPressPMYellowCycles = model.MoldOutPressPMYellowCycles;
            data.MoldOutPressPMFreqRed = model.MoldOutPressPMFreqRed;
            //data.MoldOutPressPMRedCycles = model.MoldOutPressPMRedCycles;
            db.SaveChanges();

            //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.MaintenanceSchedule.ToString(), GetAction.Update.ToString());


            CommonDrop();

            var TSC = ds.Database.SqlQuery<TotalCycleCount>("exec proTotalCycleCountAsp @MoldDataID", new SqlParameter("MoldDataID", MID)).ToList<TotalCycleCount>();

            MaintenanceScheduleCommon MSC = new MaintenanceScheduleCommon();
            //var OInfo = db.TblMoldData.Where(x => x.MoldDataID == MID).FirstOrDefault();

            var MDD = db.TblMoldData.Where(x => x.MoldDataID == model.MoldDataID).FirstOrDefault();
            var TBS = db.TblSchedules.Where(x => x.schMoldDataID == model.MoldDataID && x.CompanyID == CID).ToList();

            MSC.MoldData = MDD == null ? new tblMoldData() : MDD;
            MSC.TbScheduleList = TBS == null ? new List<tblSchedule>() : TBS;

            return PartialView("_MaintenanceSchedule", MSC);
        }

        public void GetTotalAvailableCycles(int MoldID=0)
        {
            double? TotalCycle = 0;

            if (MoldID != 0)
            {
                var data = ShrdMaster.Instance.GetMaintenanceTblRoverSetData(MoldID).OrderBy(x => x.SetDate).ToList();
                foreach (var x in data)
                {
                    TotalCycle += x.CycleCounter == null ? 0 : x.CycleCounter;
                }
            }

            ViewBag.CounterVal = TotalCycle;
        }


        public void GetMaintenanceScheduleDropDown()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var Repair = db.Database.SqlQuery<RepairStatusDropdown>("procRepairStatusDropdown @CompanyID", new SqlParameter("@CompanyID", CID)).ToList();
            List<SelectListItem> RepairStat = new List<SelectListItem>();
            foreach (var x in Repair)
            {
                RepairStat.Add(new SelectListItem
                {
                    Text = x.RepairStatus.ToString(),
                    Value = x.ID.ToString()
                });
            }

            ViewBag.RepairStatusVal = RepairStat;

            var MoldLoc = db.Database.SqlQuery<MoldLocationDropdown>("procMoldLocationDropdown @CompanyID", new SqlParameter("@CompanyID", CID)).ToList();
            //List<SelectListItem> Tech = new List<SelectListItem>();
            //foreach (var x in MoldLoc)
            //{
            //    Tech.Add(new SelectListItem
            //    {
            //        Text = x.RepairStatusLocation.ToString(),
            //        Value = x.ID.ToString()
            //    });
            //}

            //ViewBag.MoldLocation =  Tech;

            ViewBag.MoldLocation23 = new SelectList(MoldLoc, "ID", "RepairStatusLocation");

            var data = db.Database.SqlQuery<SchStatusDropDown>("procSchStatusDropdown @CompanyID", new SqlParameter("@CompanyID", CID)).ToList();
            List<SelectListItem> Status = new List<SelectListItem>();
            foreach (var x in data)
            {
                Status.Add(new SelectListItem
                {
                    Text = x.schStatus.ToString(),
                    Value = x.ID.ToString()
                });
            }

            ViewBag.StatusVal = Status;
        }

        public ActionResult MaintenanceScheduleSubForm(int MoldID=0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            GetMaintenanceScheduleDropDown();
            int MID = ReturnSelectedMoldID(MoldID);
            var DATA = db.TblSchedules.Where(x => x.schMoldDataID == MID && x.CompanyID == CID).ToList();
            return PartialView("_MaintenanceScheduleSubTab", DATA);
        }


        public ActionResult SaveTroubleList(List<tblSchedule> model)
        {
            if (model != null)
            {
                int CID = ShrdMaster.Instance.GetCompanyID();

                foreach (var x in model)
                {
                    var data = db.TblSchedules.Where(s => s.SchID == x.SchID).FirstOrDefault();
                    if (data != null)
                    {
                        data.schDate = x.schDate == DateTime.MinValue ? null : x.schDate;
                        data.schTime = x.schTime;
                        data.schPriority = x.schPriority;
                        data.schActionItem = x.schActionItem;
                        data.schCycles = x.schCycles;
                        data.schStatus = x.schStatus;
                        db.SaveChanges();
                        //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.MaintenanceSchedule.ToString(), GetAction.Update.ToString());

                    }
                }
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public void SaveTroubleTrackingFocusOut(tblSchedule model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            var data = db.TblSchedules.Where(s => s.SchID == model.SchID).FirstOrDefault();
            if (data != null)
            {
                data.schDate = model.schDate == DateTime.MinValue ? null : model.schDate;
                data.schTime = model.schTime;
                data.schPriority = model.schPriority;
                data.schActionItem = model.schActionItem;
                data.schCycles = model.schCycles;
                data.schStatus = model.schStatus;
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.MaintenanceSchedule.ToString(), GetAction.Update.ToString());
            }
        }

        public ActionResult SaveTroubleTracking(tblSchedule model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            if (model.SchID == 0)
            {
                try
                {
                    if (model.schTime != null)
                    {
                        model.schTime = new DateTime(1899, 12, 30) + model.schTime.Value.TimeOfDay;
                    }

                    model.CompanyID = CID;
                    model.schDate = model.schDate == DateTime.MinValue ? null : model.schDate;
                    model.CompanyID = ShrdMaster.Instance.GetCompanyID();
                    db.TblSchedules.Add(model);
                    db.SaveChanges();
                    //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.MaintenanceSchedule.ToString(), GetAction.Create.ToString());


                }
                catch (Exception EX)
                {

                }

            }
            else
            {
                var data = db.TblSchedules.Where(s => s.SchID == model.SchID).FirstOrDefault();

                data.schDate = model.schDate == DateTime.MinValue ? null : model.schDate;
                data.schTime = model.schTime;
                data.schPriority = model.schPriority;
                data.schActionItem = model.schActionItem;
                data.schCycles = model.schCycles;
                data.schStatus = model.schStatus;
                data.schMoldDataID = model.schMoldDataID;
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.MaintenanceSchedule.ToString(), GetAction.Update.ToString());

            }

            GetMaintenanceScheduleDropDown();
            //int MIDD = 0;
            //foreach (var x in model)
            //{
            //    MIDD = x.schMoldDataID;
            //    break;
            //}
            int MID = ReturnSelectedMoldID(model.schMoldDataID);
            GetTotalAvailableCycles(MID);
            //var TSC = db.Database.SqlQuery<TotalCycleCount>("exec proTotalCycleCountAsp @MoldDataID", new SqlParameter("MoldDataID", MID));
            //TotalCycleCount cs = TSC.FirstOrDefault();
            //ViewBag.CounterVal = cs.TotalCycles;
            MaintenanceScheduleCommon MSC = new MaintenanceScheduleCommon();
            var Moldd = db.TblMoldData.Where(x => x.MoldDataID == MID).FirstOrDefault();
            var tblsc = db.TblSchedules.Where(x => x.schMoldDataID == model.schMoldDataID && x.CompanyID == CID).ToList();

            MSC.MoldData = Moldd;
            MSC.TbScheduleList = tblsc;
            //var DATA = db.TblSchedules.Where(x => x.schMoldDataID == MID).ToList();
            return PartialView("_MaintenanceSchedule", MSC);
            //return Json("ok", JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteSubMainData(string str, int MoldID)
        {

            if (str != "")
            {
                str = str.Substring(0, str.LastIndexOf(','));
                using (var ctx = new MoldtraxDbContext())
                {
                    SqlParameter sp = new SqlParameter("@value", str);
                    var result = ctx.Database.ExecuteSqlCommand("exec procDeleteMaintenanceTrackingSubForm @value", sp);
                }
            }
            int MID = ReturnSelectedMoldID(MoldID);
            //var TSC = db.Database.SqlQuery<TotalCycleCount>("exec proTotalCycleCountAsp @MoldDataID", new SqlParameter("MoldDataID", MID));
            //TotalCycleCount cs = TSC.FirstOrDefault();
            //ViewBag.CounterVal = cs.TotalCycles;

            GetTotalAvailableCycles(MID);
            int CID = ShrdMaster.Instance.GetCompanyID();

            MaintenanceScheduleCommon MSC = new MaintenanceScheduleCommon();
            var Mdata = db.TblMoldData.Where(x => x.MoldDataID == MoldID).FirstOrDefault();
            var data = db.TblSchedules.Where(x => x.schMoldDataID == MoldID && x.CompanyID == CID).ToList();
            GetMaintenanceScheduleDropDown();
            MSC.MoldData = Mdata;
            MSC.TbScheduleList = data;
            return PartialView("_MaintenanceSchedule", MSC);
        }

        public ActionResult SaveMoldFocusOutMaintenance(tblMoldData model)
        {


            var ds = new MoldtraxDbContext();
            GetMaintenanceScheduleDropDown();
            int MID = ReturnSelectedMoldID(model.MoldDataID);
            int CID = ShrdMaster.Instance.GetCompanyID();

            var TSC = ds.Database.SqlQuery<TotalCycleCount>("exec proTotalCycleCountAsp @MoldDataID, @CompanyID", new SqlParameter("MoldDataID", MID), new SqlParameter("@CompanyID", CID)).ToList<TotalCycleCount>();

            int NewMoldID = ReturnSelectedMoldID(model.MoldDataID);

            var MD = ShrdMaster.Instance.GetMaintenanceTblRoverSetData(NewMoldID).OrderBy(x => x.SetDate).ToList();

            double? CounterVal = 0;
            foreach (var x in MD)
            {
                CounterVal += x.CycleCounter == null ? 0 : x.CycleCounter;
            }

            ViewBag.CounterVal = CounterVal;

            var data = db.TblMoldData.Where(x => x.MoldDataID == model.MoldDataID).FirstOrDefault();
            data.RepairStatus = model.RepairStatus;
            data.RepairStatusLocationId = model.RepairStatusLocationId;
            data.MoldInPressPMFreq = model.MoldInPressPMFreq;
            data.MoldOutPressPMFreq = model.MoldOutPressPMFreq;
            data.MoldCyclesToRedLimit = model.MoldCyclesToRedLimit;
            data.MoldOutPressPMYellowCycles = model.MoldOutPressPMYellowCycles;
            data.MoldOutPressPMFreqRed = model.MoldOutPressPMFreqRed;
            data.MoldOutPressPMRedCycles = model.MoldOutPressPMRedCycles;

            db.SaveChanges();

            //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.MaintenanceSchedule.ToString(), GetAction.Update.ToString());


            CommonDrop();
            MaintenanceScheduleCommon MSC = new MaintenanceScheduleCommon();
            //var OInfo = db.TblMoldData.Where(x => x.MoldDataID == MID).FirstOrDefault();

            var MDD = db.TblMoldData.Where(x => x.MoldDataID == model.MoldDataID).FirstOrDefault();
            var TBS = db.TblSchedules.Where(x => x.schMoldDataID == model.MoldDataID && x.CompanyID == CID).ToList();


            MSC.MoldData = MDD == null ? new tblMoldData() : MDD;
            MSC.TbScheduleList = TBS == null ? new List<tblSchedule>() : TBS;

            return PartialView("_MaintenanceSchedule", MSC);
        }

        public ActionResult SaveMoldMaintenance(tblMoldData model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            var data = db.TblMoldData.Where(x => x.MoldDataID == model.MoldDataID).FirstOrDefault();
            data.RepairStatus = model.RepairStatus;
            data.RepairStatusLocationId = model.RepairStatusLocationId;
            data.MoldInPressPMFreq = model.MoldInPressPMFreq;
            data.MoldOutPressPMFreq = model.MoldOutPressPMFreq;
            data.MoldCyclesToRedLimit = model.MoldCyclesToRedLimit;
            data.MoldOutPressPMYellowCycles = model.MoldOutPressPMYellowCycles;
            data.MoldOutPressPMFreqRed = model.MoldOutPressPMFreqRed;
            data.MoldOutPressPMRedCycles = model.MoldOutPressPMRedCycles;

            db.SaveChanges();
            //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.MaintenanceSchedule.ToString(), GetAction.Update.ToString());

            return Json("", JsonRequestBehavior.AllowGet);
        }

        public void OpenCalculator()
        {
            System.Diagnostics.Process.Start("calc");
        }

        #endregion

        #region Maintenance Instructions

        public ActionResult MaintenanceInstructionGetData(int SetID=0, int MoldID=0, int MOLDChange=0)
        {
            CommonDrop();

            int ID = ShrdMaster.Instance.ReturnSelectedMoldID(MoldID);
            var MID = db.TblMoldData.Where(x => x.MoldDataID == ID).FirstOrDefault();

            var SetVal = ShrdMaster.Instance.GetMaintenanceTblRoverSetData(MoldID).OrderBy(x => x.SetDate).ToList();
            //db.TblRoverSetDatas.Where(x => x.MoldDataID == MoldID).Select(x => x.SetID).ToList();
            tblRoverSetData OInfo = new tblRoverSetData();
            int CID = 0;

            CID = ShrdMaster.Instance.GetCompanyID();

            if (SetVal.Count() != 0)
            {
                if (SetID == 0)
                {
                    OInfo = db.TblRoverSetDatas.Where(x => x.MoldDataID == MoldID && x.CompanyID == CID).OrderBy(x => x.SetDate).FirstOrDefault();
                }
                else
                {
                    OInfo = db.TblRoverSetDatas.Where(x => x.SetID == SetID && x.CompanyID == CID).OrderBy(x => x.SetDate).FirstOrDefault();
                }
            }

            ViewBag.EstTime = ShrdMaster.Instance.MTXTotalTimeCal(OInfo.SetDate, OInfo.SetTime, OInfo.MldPullDate, OInfo.MldPullTime);
            double Cycle = ShrdMaster.Instance.MTXEstCycleTime(OInfo.SetDate, OInfo.SetTime, OInfo.MldPullDate, OInfo.MldPullTime);

            double? MCP = 0;

            if (MID != null)
            {
               MCP  = MID.MoldCyclesPerMinute == null ? 0 : MID.MoldCyclesPerMinute;
            }

            ViewBag.AvailableCycle = (Cycle) * 60 / MCP;

            //ViewBag.AvailableCycle = (Cycle < 0 ? 0 : Cycle) * 60 / MCP;

            OInfo.MoldDefectMapPath = OInfo.MoldDefectMapPath != null ? Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/SiteImages/MaintenanceInstruction/" + OInfo.MoldDefectMapPath) : null;
            //var CorrectiveTotalHrs = db.TblCorrectiveActions.Where(x => x.SetID == SetID).ToList();

            //int TotalHours = 0;
            //foreach (var x in CorrectiveTotalHrs)
            //{
            //    TotalHours += Convert.ToInt32(x.TIRepairTime);
            //}

            //OInfo.MldRepairedTime = TotalHours;

            if (MOLDChange == 0)
            {
                return PartialView("_MaintenanceInstructionGetData", OInfo == null ? new tblRoverSetData() : OInfo);
            }

            else
            {

                int NoofRec = 0;
                NoofRec = ShrdMaster.Instance.GetMaintenanceTblRoverSetData(MoldID).OrderBy(x => x.SetDate).Count();
                ViewBag.TotalMold = NoofRec;

                List<TblRoverSetDataViewModel> MdData = new List<TblRoverSetDataViewModel>();

                var MoldData = RenderRazorViewToString(this.ControllerContext, "_MoldData", MdData);
                var OtherInfo = RenderRazorViewToString(this.ControllerContext, "_MaintenanceInstructionGetData", OInfo == null ? new tblRoverSetData() : OInfo);

                return new JsonResult()
                {
                    ContentType = "application/json",
                    Data = new { MoldData, OtherInfo },
                    MaxJsonLength = int.MaxValue
                };

                //return Json(new { MoldData, OtherInfo }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UplaodInstructionImg(int SetID=0)
        {
            string fname = "";
            string fname1 = "";
            string IMGName = "";
            int CID = ShrdMaster.Instance.GetCompanyID();

            string UniqueString = ShrdMaster.Instance.ReturnUniqueName();
            try
            {

                //  Get all files from Request object 
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                    //string filename = Path.GetFileName(Request.Files[i].FileName);  

                    HttpPostedFileBase file = files[i];

                    //var dd = db.TblTechTips.Where(x => x.TSGuide == TsGuide).FirstOrDefault();

                    // Checking for Internet Explorer  
                    if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                    {
                        string[] testfiles = file.FileName.Split(new char[] { '\\' });
                        fname = testfiles[testfiles.Length - 1];
                    }
                    else
                    {
                        fname = "MaintenanceIns_"+ UniqueString + file.FileName;
                        fname1 = "MaintenanceIns_"+ UniqueString + file.FileName;
                        IMGName = "MaintenanceIns_" + UniqueString + file.FileName;
                    }

                    var FileExtension = Path.GetExtension(fname);
                    //dd.ImagePath = fname; E:\MoldtraxAsp\Moldtrax\SiteImages\TTPartImages\
                    //db.SaveChanges();
                    // Get the complete folder path and store the file inside it.  
                    fname = Path.Combine(Server.MapPath("~/SiteImages/MaintenanceInstruction/"), fname);
                    file.SaveAs(fname);

                    if (FileExtension == ".vsdx" || FileExtension == ".vsd")
                    {
                        var FileP = ShrdMaster.Instance.ConvertVisioToImg(fname);
                        fname1 = FileP;
                    }
                    else
                    {
                        fname1 = IMGName;
                    }
                }

               
                    var data = db.TblRoverSetDatas.Where(x => x.SetID == SetID && x.CompanyID == CID).OrderBy(x => x.SetDate).FirstOrDefault();

                    if (data.MoldDefectMapPath != null)
                    {
                        string path = Path.Combine(Server.MapPath("~/SiteImages/MaintenanceInstruction/"), data.MoldDefectMapPath);
                        DeleteFile(path);
                    }

                data.MoldDefectMapPath = fname1;
                db.SaveChanges();
                CommonDrop();
                data.MoldDefectMapPath = data.MoldDefectMapPath != null ? Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/SiteImages/MaintenanceInstruction/" + data.MoldDefectMapPath) : null;
                return Json(data.MoldDefectMapPath, JsonRequestBehavior.AllowGet);

                // Returns message that successfully uploaded  
            }
            catch (Exception ex)
            {
                return Json("Error occurred. Error details: " + ex.Message);
            }

            //return Json("", JsonRequestBehavior.AllowGet);
        }

        public void DeleteFile(string Path)
        {
            System.IO.File.Delete(Path);
        }

        public ActionResult DeleteInstructionImg(int SetID=0)
        {
            if (SetID!= 0)
            {
                int CID = ShrdMaster.Instance.GetCompanyID();

                var data = db.TblRoverSetDatas.Where(x => x.SetID == SetID && x.CompanyID == CID).OrderBy(x => x.SetDate).FirstOrDefault();

                if (data.MoldDefectMapPath != null)
                {
                    string path = Path.Combine(Server.MapPath("~/SiteImages/MaintenanceInstruction/"), data.MoldDefectMapPath);
                    DeleteFile(path);
                }

                data.MoldDefectMapPath = null;
                db.SaveChanges();
            }
            return Json("ok");
        }

        [HttpPost]
        public ActionResult SaveMainIns(tblRoverSetData model)
        {
            try
            {
                int CID = ShrdMaster.Instance.GetCompanyID();

                var data = db.TblRoverSetDatas.Where(x => x.SetID == model.SetID).OrderBy(x => x.SetDate).FirstOrDefault();
                data.MldRepairedDate = model.MldRepairedDate == DateTime.MinValue ? null : model.MldRepairedDate;
                data.MldRepairedTime = model.MldRepairedTime;
                data.MldProductionCycles = model.MldProductionCycles;
                data.MldRepairdBy = model.MldRepairdBy;
                data.MldSetPullNotes = model.MldSetPullNotes;
                data.MldWorkOrder = model.MldWorkOrder;
                data.MldRepairComments = model.MldRepairComments;
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.MaintenanceInstructions.ToString(), GetAction.Update.ToString());

            }
            catch (Exception ex)
            {

            }

            return Json("ok");
        }

        public ActionResult SortStartDate(List<tblRoverSetData> model, string Order="asc")
        {
            List<tblRoverSetData> Data = new List<tblRoverSetData>();
            if (Order == "asc")
            {
                Data = model.OrderBy(x => x.SetDate).ToList();
            }
            else
            {
                Data = model.OrderByDescending(x => x.SetDate).ToList();
            }
            return Json(Data, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region Defects / Task

        public void DefectCommonDropDown(int MoldID=0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.Database.SqlQuery<DfctDescriptionDropDoWN>("procDfctDescriptDropdown @MoldDataID, @CompanyID",new SqlParameter("MoldDataID", MoldID), new SqlParameter("@CompanyID", CID)).ToList();

            List<SelectListItem> Defect = new List<SelectListItem>();
            foreach (var x in data)
            {
                    Defect.Add(new SelectListItem
                    {
                        Text = x.Type + "\xA0" + "\xA0" + " | " + "\xA0" + "\xA0" + x.TSExplanation,
                        Value = x.TSGuide.ToString() 
                    });
            }

            ViewBag.RepairTask = Defect.OrderBy(x=> x.Text);

            var Technician = db.Database.SqlQuery<TechnicianDropDown>("exec procTechnicianDropdown @CompanyID", new SqlParameter("@CompanyID", CID)).ToList();
            List<SelectListItem> Tech = new List<SelectListItem>();
            foreach (var x in Technician)
            {
                Tech.Add(new SelectListItem
                {
                    Text = x.Name,
                    Value = x.EmployeeID.ToString()
                });
            }

            ViewBag.Technician = Tech.OrderBy(x=> x.Text);

            var CavNumDrop = ShrdMaster.Instance.CavDropDown(con, MoldID);

            List<SelectListItem> CavNum = new List<SelectListItem>();
            //CavNum.Add(new SelectListItem { Text = "Pos | Cav ID | Act", Value = "0" });
            foreach (var x in CavNumDrop.OrderByDescending(x=> x.Act).ThenBy(x=> x.Pos.Length))
            {
                CavNum.Add(new SelectListItem
                {
                    Text = x.Pos + " | "+ x.CavID + " " + " | " + " " + x.Act,
                    Value = x.CavityNumberID.ToString(),
                    
                });
            }

            List<SelectListItem> CavNum1 = new List<SelectListItem>();
            foreach (var x in CavNumDrop)
            {
                CavNum1.Add(new SelectListItem
                {
                    Text = x.CavID,
                    Value = x.CavityNumberID.ToString()
                });
            }

            ViewBag.NewCavNum = CavNum1.OrderBy(x => x.Text).ThenBy(x => x.Text).ToList();
            ViewBag.CavNum = CavNum;

            var dd = db.TblMoldData.Where(X=> X.CompanyID == CID).ToList().Select(x => new tblMoldData { MoldDataID = x.MoldDataID, MoldName = x.MoldName + ": " + x.MoldDesc });
            List<SelectListItem> Tech1 = new List<SelectListItem>();
            foreach (var x in dd.OrderBy(x => x.MoldName))
            {
                Tech1.Add(new SelectListItem
                {
                    Text = x.MoldName,
                    Value = x.MoldDataID.ToString()
                });
            }
            ViewBag.MoldConfigList = Tech1.OrderBy(x=> x.Text).ToList();
        }

        public ActionResult GetDefecttTaskData(int SetID = 0, int MoldID=0, int MOLDChange=0)
        {
            int MID = ShrdMaster.Instance.ReturnSelectedMoldID(MoldID);
            var TBLMold = db.TblMoldData.Where(x => x.MoldDataID == MID).FirstOrDefault();
            var SetVal =ShrdMaster.Instance.GetMaintenanceTblRoverSetData(MoldID).OrderBy(x => x.SetDate).Select(x => x.SetID).ToList();
            //db.TblRoverSetDatas.Where(x => x.MoldDataID == MoldID).Select(x => x.SetID).ToList();

            DefectCommonDropDown(MoldID);
            var data = db.TblDfctBlockOffs.Where(x => x.SetID == SetID).FirstOrDefault();
            List<tblDfctBlockOff> OInfo = new List<tblDfctBlockOff>();

            //var MMD = db.TblRoverSetDatas.Where(x => x.MoldDataID == MoldID).FirstOrDefault();
            if (SetVal.Count() != 0)
            {
                if (SetID == 0)
                {

                    int CID = ShrdMaster.Instance.GetCompanyID();

                    int setID = db.TblRoverSetDatas.Where(x => x.MoldDataID == MoldID && x.CompanyID == CID).OrderBy(x => x.SetDate).Select(x => x.SetID).FirstOrDefault();
                    if (setID == 0)
                    {
                        OInfo = db.TblDfctBlockOffs.Where(x => x.SetID == null && x.CompanyID == CID).ToList();
                    }

                    else
                    {
                        OInfo = db.TblDfctBlockOffs.Where(x => x.SetID == setID).ToList();
                    }
                }
                else
                {
                    OInfo = db.TblDfctBlockOffs.Where(x => x.SetID == SetID).ToList();
                }
            }

            if (MOLDChange == 0)
            {
                return PartialView("_DefectandTaskGetData", OInfo);
            }

            else
            {
                CommonDrop();
                var MData = ShrdMaster.Instance.GetMaintenanceTblRoverSetData(MoldID).OrderBy(x => x.SetDate).ToList();
                var MoldData = RenderRazorViewToString(this.ControllerContext, "_MoldData", MData);
                var OtherInfo = RenderRazorViewToString(this.ControllerContext, "_DefectandTaskGetData", OInfo);
                return Json(new { MoldData, OtherInfo }, JsonRequestBehavior.AllowGet);
                //return PartialView("_DefectandTaskGetData", new {MoldData, OtherInfo});
            }
        }


        [HttpPost]
        public ActionResult SaveDefectsList(List<tblDfctBlockOff> model)
        {

            int CID = ShrdMaster.Instance.GetCompanyID();
            var NewDB = new MoldtraxDbContext();
            if (model != null)
            {
                foreach (var x in model)
                {
                    var data = NewDB.TblDfctBlockOffs.Where(s => s.DfctID == x.DfctID).FirstOrDefault();
                    if (data != null)
                    {
                        data.DfctDate = x.DfctDate == DateTime.MinValue ? null : x.DfctDate;
                        data.DfctTime = x.DfctTime;
                        data.DfctBlocked = x.DfctBlocked;
                        data.DfctQC = x.DfctQC;
                        data.DfctCavNum = x.DfctCavNum;
                        data.DfctDescript = x.DfctDescript;
                        data.EmployeeID = x.EmployeeID;
                        data.DfctNotes = x.DfctNotes;
                        data.DftcRepaired = x.DftcRepaired;
                        data.DftcEstTime = x.DftcEstTime;
                        db.Entry(data).State = EntityState.Modified;
                        db.SaveChanges();
                        //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.DefectTask.ToString(), GetAction.Update.ToString());

                    }

                }
            }

            //int MoldID = 0;
            //foreach (var x in model)
            //{
            //    MoldID = x.MoldID;
            //    break;
            //}
            return Json("", JsonRequestBehavior.AllowGet);
        }




        [HttpPost]
        public ActionResult SaveDefects(tblDfctBlockOff model, int MainMoldID=0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            if (model != null)
            {

                if (model.DfctID == 0)
                {
                    model.CompanyID = CID;
                    db.TblDfctBlockOffs.Add(model);
                    db.SaveChanges();
                    //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.DefectTask.ToString(), GetAction.Create.ToString());

                }
                else
                {
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.DefectTask.ToString(), GetAction.Update.ToString());

                }
            }

            //int MoldID = 0;
            //foreach (var x in model)
            //{
            //    MoldID = x.MoldID;
            //    break;
            //}

            var data = db.TblDfctBlockOffs.Where(x => x.SetID == model.SetID && x.CompanyID == CID).ToList();

            DefectCommonDropDown(model.MoldID);
            return PartialView("_DefectandTaskGetData", data);
            //return Json("", JsonRequestBehavior.AllowGet);
        }

        public void SaveDefectsFocusOut(tblDfctBlockOff model)
        {
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
            //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.DefectTask.ToString(), GetAction.Update.ToString());

        }

        public void CopyDefectData(List<tblDfctBlockOff> model)
        {
            Session["CopyDate"] = model;
            //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.DefectTask.ToString(), GetAction.Copy.ToString());

        }

        public ActionResult PasteDefectDate(int SetID, int MoldID)
        {
            var list = (List<tblDfctBlockOff>)Session["CopyDate"];
            int CID = ShrdMaster.Instance.GetCompanyID();

            foreach (var x in list)
            {
                tblDfctBlockOff tb = new tblDfctBlockOff();
                tb.SetID = SetID;
                tb.DfctDate = x.DfctDate;
                tb.DfctTime = x.DfctTime;
                tb.DfctBlocked = x.DfctBlocked;
                tb.DfctQC = x.DfctQC;
                tb.CompanyID = CID;
                tb.DfctCavNum = x.DfctCavNum;
                tb.DfctDescript = x.DfctDescript;
                tb.EmployeeID = x.EmployeeID;
                tb.DfctNotes = x.DfctNotes;
                tb.DftcRepaired = x.DftcRepaired;
                tb.DftcEstTime = x.DftcEstTime;

                db.TblDfctBlockOffs.Add(tb);
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.DefectTask.ToString(), GetAction.Paste.ToString());
            }

            DefectCommonDropDown(MoldID);
           var data = db.TblDfctBlockOffs.Where(x => x.SetID == SetID && x.CompanyID == CID).ToList();
           return PartialView("_DefectandTaskGetData", data);
        }


        public ActionResult DeleteDefects(string str, int SetID=0, int MoldID=0)
        {
            if (str != "")
            {
                str = str.Substring(0, str.LastIndexOf(','));
                using (var ctx = new MoldtraxDbContext())
                {
                    SqlParameter sp = new SqlParameter("@value", str);
                    var result = ctx.Database.ExecuteSqlCommand("exec DeleteDfctBlockOff @value", sp);
                }
            }
            int CID = ShrdMaster.Instance.GetCompanyID();

            var DATA = db.TblDfctBlockOffs.Where(x => x.SetID == SetID && x.CompanyID == CID).ToList();
            //foreach (var x in DATA)
            //{
            //    MoldID = x.MoldID;
            //    break;
            //}
            DefectCommonDropDown(MoldID);
            return PartialView("_DefectandTaskGetData", DATA);
        }
        #endregion

        #region Corrective Action Mode

        public void CommonCorrectiveDropDown(int MoldID=0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            var Corrective = db.TblDDTITypes.Where(X=> X.CompanyID == CID).ToList().OrderBy(x => x.TIType);
            List<SelectListItem> CorrectiveAction = new List<SelectListItem>();
            foreach (var x in Corrective)
            {
                CorrectiveAction.Add(new SelectListItem
                {
                    Text = x.TIType,
                    Value = x.ID.ToString()
                });
            }

            ViewBag.CorrectiveType = CorrectiveAction;
            //var data = db.TblMoldTooling.ToList();

            var data = ShrdMaster.Instance.GetMaintenanceTooling(con, MoldID);

            List<tblMoldTooling> tbl = new List<tblMoldTooling>();

            List<SelectListItem> Defect = new List<SelectListItem>();
            List<SelectListItem> ToolingDefect = new List<SelectListItem>();
            //Defect.Add(new SelectListItem { Text = "MoldToolDescrip | MoldTooling | POH", Value = "0" });
            foreach (var x in data.OrderBy(x=> x.MoldToolDescrip))
            {
                if (x.MoldToolDescrip != null || x.MoldToolDescrip != "")
                {
                    string[] sds =Regex.Split(x.MoldToolDescrip, "</p>");
                    string s = HtmlToPlainText(sds[0]);

                    if (s != "")
                    {
                        s = s.Replace(System.Environment.NewLine, string.Empty);
                        Defect.Add(new SelectListItem
                        {
                            Text = s.Replace("&nbsp;"," ") + "|" + "\xA0" +  x.POH + "\xA0" + "\xA0",
                            Value = x.MoldToolingID.ToString()
                        });

                    }

                }
            }

            var CorrectiveAc = db.TblDDTlCorrectiveActions.Where(x=> x.CompanyID == CID).ToList();
            List<SelectListItem> Defect1 = new List<SelectListItem>();
            foreach (var x in CorrectiveAc.OrderBy(x=> x.TlCorrectiveAction))
            {
                Defect1.Add(new SelectListItem
                {
                    Text = x.TlCorrectiveAction,
                    Value = x.ID.ToString()
                });
            }

            ViewBag.CorrectiveAct = Defect1;
            ViewBag.ReplTooling = Defect.OrderBy(x=> x.Text);
            ViewBag.NewTooling = Defect.OrderBy(x=> x.Text);

            var Technician = db.Database.SqlQuery<TechnicianDropDown>("exec procTechnicianDropdown @CompanyID", new SqlParameter("@CompanyID", CID)).ToList();
            List<SelectListItem> Tech = new List<SelectListItem>();
            foreach (var x in Technician.OrderBy(x=> x.Name))
            {
                Tech.Add(new SelectListItem
                {
                    Text = x.Name,
                    Value = x.EmployeeID.ToString()
                });
            }

            ViewBag.Technician = Tech;
        }

        public ActionResult AddUpdatePoH(int Qty = 0, int TID = 0, int MoldID = 0, int SID=0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            tblCorrectiveAction tb = new tblCorrectiveAction();
            tb.TlQuantity = Qty;
            tb.SetID = SID;
            tb.CompanyID = CID;
            db.TblCorrectiveActions.Add(tb);
            db.SaveChanges();
            //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.CorrectiveActionMade.ToString(), GetAction.Create.ToString());




            var CorrectiveAction = db.TblCorrectiveActions.Where(x => x.TlReplID == tb.TlReplID).FirstOrDefault();

                if (CorrectiveAction != null)
                {
                    //CorrectiveAction.TlQuantity = Qty;
                    //db.SaveChanges();

                    var Mold = db.TblMoldTooling.Where(x => x.MoldToolingID == CorrectiveAction.TlReplTooling).FirstOrDefault();
                    Mold.MoldToolingPartsOnHand = Mold.MoldToolingPartsOnHand - Qty;
                    db.SaveChanges();

                    //return Json("", JsonRequestBehavior.AllowGet);

                    List<SelectListItem> Defect = new List<SelectListItem>();
                    var data = ShrdMaster.Instance.GetMaintenanceTooling(con, MoldID);

                    foreach (var x in data.OrderBy(x => x.MoldToolDescrip))
                    {
                        if (x.MoldToolDescrip != null || x.MoldToolDescrip != "")
                        {
                            string[] sds = Regex.Split(x.MoldToolDescrip, "</p>");
                            string s = HtmlToPlainText(sds[0]);

                            //string[] s1 = Regex.Split(x.MoldToolDescrip, "&nbsp;");
                            //string s = Regex.Replace(s1[0], "<[^>]*>", string.Empty);

                            if (s != "")
                            {
                                s = s.Replace(System.Environment.NewLine, string.Empty);
                                Defect.Add(new SelectListItem
                                {
                                    Text = s.Replace("&nbsp;", " ") + "|" + "\xA0" + x.POH + "\xA0" + "\xA0",
                                    Value = x.MoldToolingID.ToString()
                                });

                            }
                        }
                    }

                    return Json(Defect, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdatePoH(tblCorrectiveAction model, int CID = 0, int MoldID = 0)
        {
            if (CID == 0)
            {

            }

            var CorrectiveAction = db.TblCorrectiveActions.Where(x => x.TlReplID == CID).FirstOrDefault();

            if (CorrectiveAction != null)
            {
                CorrectiveAction.TlQuantity = model.TlQuantity;
                CorrectiveAction.TSDate = model.TSDate;
                CorrectiveAction.TIType = model.TIType;
                CorrectiveAction.TlReplTooling = model.TlReplTooling;
                CorrectiveAction.TlQuantity = model.TlQuantity;
                CorrectiveAction.TlCorrectiveAction = model.TlCorrectiveAction;
                CorrectiveAction.TlSTime = model.TlSTime;
                CorrectiveAction.TlTechnician = model.TlTechnician;
                CorrectiveAction.TIRepairTime = model.TIRepairTime;
                CorrectiveAction.TINotes = model.TINotes;
                //CorrectiveAction.TlReplID = model.TlReplID;

                db.SaveChanges();

                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.CorrectiveActionMade.ToString(), GetAction.Update.ToString());


                var Mold = db.TblMoldTooling.Where(x => x.MoldToolingID == CorrectiveAction.TlReplTooling).FirstOrDefault();
                Mold.MoldToolingPartsOnHand = Mold.MoldToolingPartsOnHand - model.TlQuantity;
                db.SaveChanges();

                List<SelectListItem> Defect = new List<SelectListItem>();
                var data = ShrdMaster.Instance.GetMaintenanceTooling(con, MoldID);

                foreach (var x in data.OrderBy(x => x.MoldToolDescrip))
                {
                    if (x.MoldToolDescrip != null || x.MoldToolDescrip != "")
                    {
                        string[] sds = Regex.Split(x.MoldToolDescrip, "</p>");
                        string s = HtmlToPlainText(sds[0]);

                        if (s != "")
                        {
                            s = s.Replace(System.Environment.NewLine, string.Empty);
                            Defect.Add(new SelectListItem
                            {
                                Text = s.Replace("&nbsp;", " ") + "|" + "\xA0" + x.POH + "\xA0" + "\xA0",
                                Value = x.MoldToolingID.ToString()
                            });

                        }
                    }
                }

                return Json(Defect, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        private static string HtmlToPlainText(string html)
        {
            const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
            const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
            const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
            var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
            var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
            var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

            var text = html;
            //Decode html specific characters
            text = System.Net.WebUtility.HtmlDecode(text);
            //Remove tag whitespace/line breaks
            text = tagWhiteSpaceRegex.Replace(text, "><");
            //Replace <br /> with line breaks
            text = lineBreakRegex.Replace(text, Environment.NewLine);
            //Strip formatting
            text = stripFormattingRegex.Replace(text, string.Empty);

            return text;
        }

        public ActionResult GetCorrectiveData(int SetID=0, int MoldID=0, int MOLDChange=0)
        {
            int MID = 0;
            MID = ShrdMaster.Instance.ReturnSelectedMoldID(MoldID);
            CommonCorrectiveDropDown(MoldID);

            var TBLMold = db.TblMoldData.Where(x => x.MoldDataID == MID).FirstOrDefault();
            var SetVal = ShrdMaster.Instance.GetMaintenanceTblRoverSetData(MoldID).OrderBy(x=> x.SetDate).Select(x => x.SetID).ToList();
            //db.TblRoverSetDatas.Where(x => x.MoldDataID == MoldID).Select(x => x.SetID).ToList();
            MainMaintenanceInstruction OInfo = new MainMaintenanceInstruction();
            List<SelectListItem> Defect1 = new List<SelectListItem>();

            if (SetVal.Count() != 0)
            {
                if (SetID == 0)
                {
                    int CID = 0;
                    CID = ShrdMaster.Instance.GetCompanyID();

                    int setID = db.TblRoverSetDatas.Where(x => x.MoldDataID == MoldID && x.CompanyID == CID).OrderBy(x => x.SetDate).Select(x => x.SetID).FirstOrDefault();
                    var DefectRep = ShrdMaster.Instance.GetDefectRepaired(con, setID).ToList();

                    foreach (var x in DefectRep)
                    {
                        Defect1.Add(new SelectListItem
                        {
                            Text = x.CavDefect == "" ? ": " + x.TSDefect + ":" + Convert.ToDateTime(x.DfctDate).ToShortDateString() : x.CavDefect ,
                            Value = x.DfctID.ToString()
                        });
                    }


                    OInfo.DefctRep = Defect1;
                    OInfo.MainList = db.TblCorrectiveActions.Where(x => x.SetID == setID && x.CompanyID == CID).ToList();

                    //var TlReplIDData = db.TblCorrectiveActions.Where(x => x.SetID == setID).FirstOrDefault();
                    //if (TlReplIDData != null)
                    //{
                    //    var tbldata = db.TblRepairFixes.Where(x => x.TlReplID == TlReplIDData.TlReplID).ToList();
                    //    OInfo.TblRepairFixes = tbldata;
                    //}
                    //else
                    //{
                    //    List<tblRepairFix> TBL = new List<tblRepairFix>();
                    //    OInfo.TblRepairFixes = TBL;
                    //}
                }
                else
                {
                    int CID = ShrdMaster.Instance.GetCompanyID();

                    OInfo.MainList = db.TblCorrectiveActions.Where(X => X.SetID == SetID && X.CompanyID == CID).ToList();
                    var DefectRep = ShrdMaster.Instance.GetDefectRepaired(con, SetID).ToList();

                    foreach (var x in DefectRep)
                    {
                        Defect1.Add(new SelectListItem
                        {
                            Text = x.CavDefect == "" ? ": " + x.TSDefect + ":" + Convert.ToDateTime(x.DfctDate).ToShortDateString() : x.CavDefect,
                            Value = x.DfctID.ToString()
                        });
                    }

                    OInfo.DefctRep = Defect1;
                    //var TlReplIDData = db.TblCorrectiveActions.Where(x => x.SetID == SetID).FirstOrDefault();
                    //if (TlReplIDData != null)
                    //{
                    //    var tbldata = db.TblRepairFixes.Where(x => x.TlReplID == TlReplIDData.TlReplID).ToList();
                    //    OInfo.TblRepairFixes = tbldata;
                    //}
                    //else
                    //{
                    //    List<tblRepairFix> TBL = new List<tblRepairFix>();
                    //    OInfo.TblRepairFixes = TBL;
                    //}
                }
            }
            else
            {
                OInfo.MainList = new List<tblCorrectiveAction>();
                OInfo.DefctRep = new List<SelectListItem>();
                //OInfo.TblRepairFixes = new List<tblRepairFix>();
            }

            int TLREPLID = 0;
            int DCTid = 0;
            if (OInfo.MainList.Count() != 0)
            {
                TLREPLID = OInfo.MainList.FirstOrDefault().TlReplID;
                DCTid = db.TblRepairFixes.Where(x => x.TlReplID == TLREPLID).Select(x => x.DfctID).FirstOrDefault();
            }

            ViewBag.RepairFixID = DCTid;
            //if (DCTid != 0)
            //{
            //    int TLreplID = OInfo.MainList.FirstOrDefault().TlReplID;
            //    var tbldata = db.TblRepairFixes.Where(x => x.TlReplID == TLreplID ).ToList();
            //    //OInfo.TblRepairFixes = tbldata;
            //}

            if (MOLDChange == 0)
            {
                return PartialView("_GetCorrectiveData", OInfo );
            }
            else
            {
                CommonDrop();

                int NoofRec = 0;
                NoofRec = ShrdMaster.Instance.GetMaintenanceTblRoverSetData(MoldID).OrderBy(x => x.SetDate).Count();
                ViewBag.TotalMold = NoofRec;

                List<TblRoverSetDataViewModel> MD = new List<TblRoverSetDataViewModel>();

                var MoldData = RenderRazorViewToString(this.ControllerContext, "_MoldData", MD);
                var OtherInfo = RenderRazorViewToString(this.ControllerContext, "_GetCorrectiveData", OInfo);
                return Json(new { MoldData, OtherInfo }, JsonRequestBehavior.AllowGet);
                //return PartialView("_GetCorrectiveData", new {MoldData,OtherInfo });
            }
        }

        public ActionResult SaveCorrectiveActionList(List<tblCorrectiveAction> model)
        {
            if (model != null)
            {
                int? SetID = 0;
                int CID = ShrdMaster.Instance.GetCompanyID();

                foreach (var x in model)
                {
                    var data = db.TblCorrectiveActions.Where(s => s.TlReplID == x.TlReplID).FirstOrDefault();
                    SetID = data.SetID;
                    if (data != null)
                    {
                        data.DfctID = x.DfctID;
                        data.TlReplTooling = x.TlReplTooling;
                        data.TlCorrectiveAction = x.TlCorrectiveAction;

                        data.TSDate = x.TSDate == DateTime.MinValue ? null : x.TSDate;
                        data.TlSTime = x.TlSTime;
                        data.TlTechnician = x.TlTechnician;
                        data.TIFDate = x.TIFDate == DateTime.MinValue ? null : x.TIFDate;
                        data.TIFTime = x.TIFTime;
                        data.TINotes = x.TINotes;
                        data.TIType = x.TIType;
                        data.TlQuantity = x.TlQuantity;
                        data.TIRepairTime = x.TIRepairTime;
                        db.Entry(data).State = EntityState.Modified;
                        db.SaveChanges();
                        //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.CorrectiveActionMade.ToString(), GetAction.Update.ToString());

                    }
                    //db.Entry(x).State = EntityState.Modified;
                    //db.SaveChanges();
                }

                var Oinfo = db.TblCorrectiveActions.Where(x => x.SetID == SetID && x.CompanyID == CID).ToList();

                double TotalHours = 0;
                foreach (var x in Oinfo)
                {
                    TotalHours += Convert.ToDouble(x.TIRepairTime);
                }

                var tblRover = db.TblRoverSetDatas.Where(x => x.SetID == SetID).FirstOrDefault();
                tblRover.MldRepairedTime = TotalHours;
                db.SaveChanges();
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveCorrective(tblCorrectiveAction model, int MoldID=0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            if (model.TlReplID == 0)
            {
                model.CompanyID = CID;
                db.TblCorrectiveActions.Add(model);
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.CorrectiveActionMade.ToString(), GetAction.Create.ToString());


                var MoldTool = db.TblMoldTooling.Where(x => x.MoldToolingID == model.TlReplTooling).FirstOrDefault();
                if (MoldTool != null)
                {
                    MoldTool.MoldToolingPartsOnHand = MoldTool.MoldToolingPartsOnHand - model.TlQuantity;
                    db.SaveChanges();
                }

                var Oinfo = db.TblCorrectiveActions.Where(x => x.SetID == model.SetID && x.CompanyID == CID).ToList();
                int TotalHours = 0;
                foreach (var x in Oinfo)
                {
                    TotalHours += Convert.ToInt32(x.TIRepairTime);
                }

                var tblRover = db.TblRoverSetDatas.Where(x => x.SetID == model.SetID).FirstOrDefault();
                tblRover.MldRepairedTime = TotalHours;
                db.SaveChanges();

            }
            else
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.CorrectiveActionMade.ToString(), GetAction.Update.ToString());

            }

            MainMaintenanceInstruction data = new MainMaintenanceInstruction();

            data.MainList = db.TblCorrectiveActions.Where(x => x.SetID == model.SetID && x.CompanyID == CID).ToList();

            //int sid = 0;
            //if (model.SetID == null)
            //{
            //    sid = model.SetID;
            //}
            var dsts = ShrdMaster.Instance.GetDefectRepaired(con, model.SetID).ToList();
            List<SelectListItem> Defect1 = new List<SelectListItem>();

            foreach (var x in dsts)
            {
                Defect1.Add(new SelectListItem
                {
                    Text = x.CavDefect == "" ? ": " + x.TSDefect + ":" + Convert.ToDateTime(x.DfctDate).ToShortDateString() : x.CavDefect,
                    Value = x.DfctID.ToString()
                });
            }

            data.DefctRep = Defect1;
            //data.TblRepairFixes = new List<tblRepairFix>();

            CommonCorrectiveDropDown(MoldID);
            return PartialView("_GetCorrectiveData", data);
        }

        public void SaveCorrectiveFocusOut(tblCorrectiveAction model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            var data = db.TblCorrectiveActions.Where(x => x.TlReplID == model.TlReplID).FirstOrDefault();
            data.TlReplID = model.TlReplID;
            data.SetID = model.SetID;
            data.TlReplTooling = model.TlReplTooling;
            data.TlCorrectiveAction = model.TlCorrectiveAction;
            data.TSDate = model.TSDate;
            data.TlSTime = model.TlSTime;
            data.TlTechnician = model.TlTechnician;
            data.TIFDate = model.TIFDate;
            data.TIFTime = model.TIFTime;
            data.TINotes = model.TINotes;
            data.TIType = model.TIType;
            data.TlQuantity = model.TlQuantity;
            data.TIRepairTime = model.TIRepairTime;
            db.SaveChanges();
            //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.CorrectiveActionMade.ToString(), GetAction.Update.ToString());

        }

        public ActionResult DeleteCorrective(string str="", int SetID=0, int MoldID=0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            if (str != "")
            {
                str = str.Substring(0, str.LastIndexOf(','));
                using (var ctx = new MoldtraxDbContext())
                {
                    SqlParameter sp = new SqlParameter("@value", str);
                    var result = ctx.Database.ExecuteSqlCommand("exec DeleteCorrectiveAction @value", sp);
                }

                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.CorrectiveActionMade.ToString(), GetAction.Delete.ToString());

            }

            //var data = db.TblCorrectiveActions.Where(x => x.SetID == SetID).ToList();

            MainMaintenanceInstruction data = new MainMaintenanceInstruction();

            data.MainList = db.TblCorrectiveActions.Where(x => x.SetID == SetID && x.CompanyID == CID).ToList();
            List<SelectListItem> Defect1 = new List<SelectListItem>();
            var dsts = ShrdMaster.Instance.GetDefectRepaired(con, SetID).ToList();

            foreach (var x in dsts)
            {
                Defect1.Add(new SelectListItem
                {
                    Text = x.CavDefect == "" ? ": " + x.TSDefect + ":" + Convert.ToDateTime(x.DfctDate).ToShortDateString() : x.CavDefect,
                    Value = x.DfctID.ToString()
                });
            }

            data.DefctRep = Defect1;

            CommonCorrectiveDropDown(MoldID);


            var Oinfo = db.TblCorrectiveActions.Where(x => x.SetID == SetID && x.CompanyID == CID).ToList();

            int TotalHours = 0;
            foreach (var x in Oinfo)
            {
                TotalHours += Convert.ToInt32(x.TIRepairTime);
            }

            //int tlrep = 0;
            //    tlrep = Oinfo.FirstOrDefault().TlReplID;
            //if (tlrep != 0)
            //{
            //    var tbldata = db.TblRepairFixes.Where(x => x.TlReplID == tlrep).ToList();
            //    data.TblRepairFixes = tbldata;
            //}
            //else
            //{
            //    data.TblRepairFixes = new List<tblRepairFix>();
            //}

                var tblRover = db.TblRoverSetDatas.Where(x => x.SetID == SetID).FirstOrDefault();
            tblRover.MldRepairedTime = TotalHours;
            db.SaveChanges();

            return PartialView("_GetCorrectiveData", data);
            //return Json("ok");

        }

        public void CopyCorrectiveData(List<tblCorrectiveAction> model)
        {
            Session["CopyCorrectiveDate"] = model;
            //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.CorrectiveActionMade.ToString(), GetAction.Copy.ToString());

        }

        public ActionResult PasteCorrectiveDate(int SetID, int MoldID)
        {
            MainMaintenanceInstruction data = new MainMaintenanceInstruction();
            var list = (List<tblCorrectiveAction>)Session["CopyCorrectiveDate"];


            int CID = ShrdMaster.Instance.GetCompanyID();

            foreach (var x in list)
            {
                tblCorrectiveAction tb = new tblCorrectiveAction();
                tb.SetID = SetID;
                tb.DfctID = x.DfctID;
                tb.TlReplTooling = x.TlReplTooling;
                tb.TlCorrectiveAction = x.TlCorrectiveAction;
                tb.TSDate = x.TSDate;
                tb.TlSTime = x.TlSTime;
                tb.TlTechnician = x.TlTechnician;
                tb.CompanyID = CID;
                tb.TIFDate = x.TIFDate;
                tb.TIFTime = x.TIFTime;
                tb.TINotes = x.TINotes;
                tb.TIType = x.TIType;
                //tb.TlQuantity = x.TlQuantity;
                tb.TIRepairTime = x.TIRepairTime;

                db.TblCorrectiveActions.Add(tb);
                db.SaveChanges();
            }

            //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.CorrectiveActionMade.ToString(), GetAction.Paste.ToString());


            CommonCorrectiveDropDown(MoldID);

            List<SelectListItem> Defect1 = new List<SelectListItem>();
            var dsts = ShrdMaster.Instance.GetDefectRepaired(con, SetID).ToList();

            foreach (var x in dsts)
            {
                Defect1.Add(new SelectListItem
                {
                    Text = x.CavDefect == "" ? ": " + x.TSDefect + ":" + Convert.ToDateTime(x.DfctDate).ToShortDateString() : x.CavDefect,
                    Value = x.DfctID.ToString()
                });
            }

            data.DefctRep = Defect1;
            var data1 = db.TblCorrectiveActions.Where(x => x.SetID == SetID && x.CompanyID == CID).ToList();
            data.MainList = data1;
            //data.TblRepairFixes = new List<tblRepairFix>();

            return PartialView("_GetCorrectiveData", data);
        }

        public void SaveDefectsRepaired(int DfctID = 0, int TIRepID = 0)
        {
            if (TIRepID != 0)
            {
                var data = db.TblRepairFixes.Where(x => x.TlReplID == TIRepID).FirstOrDefault();
                int CID = ShrdMaster.Instance.GetCompanyID();

                if (data == null)
                {

                    tblRepairFix tbl = new tblRepairFix();
                    tbl.DfctID = DfctID;
                    tbl.CompanyID = CID;
                    tbl.TlReplID = TIRepID;
                    db.TblRepairFixes.Add(tbl);
                    db.SaveChanges();
                    //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.DefectTask.ToString(), GetAction.Create.ToString());

                }
                else
                {
                    data.DfctID = DfctID;
                    db.SaveChanges();
                    //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.DefectTask.ToString(), GetAction.Update.ToString());

                }
            }
        }


        public ActionResult GetDefectRepaired(int TIRepID=0)
        {
            var Newdb = new MoldtraxDbContext();
            int i = 0;
            var data = db.TblRepairFixes.Where(x => x.TlReplID == TIRepID).FirstOrDefault();
            if (data != null)
            {
                i = data.DfctID;
            }

            //var data = Newdb.TblCorrectiveActions.Where(x => x.TlReplID == TIRepID).FirstOrDefault();
            return Json(i, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteDefect(int TBLID=0)
        {
            var data = db.TblRepairFixes.Where(x => x.RPRepairFixID == TBLID).FirstOrDefault();
            db.TblRepairFixes.Remove(data);
            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Action Review


        protected string Linkify(string SearchText)
        {
            // this will find links like:
            // http://www.mysite.com
            // as well as any links with other characters directly in front of it like:
            // href="http://www.mysite.com"
            // you can then use your own logic to determine which links to linkify
            Regex regx = new Regex(@"\b(((\S+)?)(@|mailto\:|(news|(ht|f)tp(s?))\://)\S+)\b", RegexOptions.IgnoreCase);
            SearchText = SearchText.Replace("&nbsp;", " ");
            MatchCollection matches = regx.Matches(SearchText);

            foreach (Match match in matches)
            {
                if (match.Value.StartsWith("http"))
                { // if it starts with anything else then dont linkify -- may already be linked!
                    SearchText = SearchText.Replace(match.Value, "<a href='" + match.Value + "'>" + match.Value + "</a>");
                }
            }

            return SearchText + " ";
        }


        public ActionResult ActionReviewGetData(int MoldID = 0, int SetID=0, int MOLDChange = 0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            if (MOLDChange == 0)
            {
                var OInfo = db.Database.SqlQuery<ActionReview>("GetActionReview @SetID, @CompanyID", new SqlParameter("SetID", SetID), new SqlParameter("@CompanyID", CID)).ToList();
                List<ActionReview> LST = new List<ActionReview>();

                foreach (var x in OInfo)
                {
                    ActionReview AR = new ActionReview();
                    AR.TlRepllD = x.TlRepllD;
                    AR.CavityNumberID = x.CavityNumberID;
                    AR.CavityNumber = x.CavityNumber;
                    AR.SetID = x.SetID;
                    AR.SetDate = x.SetDate;
                    AR.DfctDate = x.DfctDate;
                    AR.TSDefects = x.TSDefects;
                    AR.TSDate = x.TSDate;
                    AR.TlSTime = x.TlSTime;
                    AR.DfctCavNum = x.DfctCavNum;
                    //AR.MoldToolDescrip = x.MoldToolDescrip != null ? Regex.Replace(x.MoldToolDescrip.ToString().Replace("&nbsp;"," "), "<(.|\n)*?>", "") : null;
                    AR.MoldToolDescrip = x.MoldToolDescrip;
                    AR.TlCorrectiveAction = x.TlCorrectiveAction;
                    AR.TIRepairTime = x.TIRepairTime;
                    LST.Add(AR);
                }


                foreach (var x in LST)
                {
                    if (x.MoldToolDescrip != null && x.MoldToolDescrip != "")
                    {
                        string[] sds = Regex.Split(x.MoldToolDescrip, "</p>");
                        string s = HtmlToPlainText(sds[0]);

                        if (s != "")
                        {
                            s = s.Replace(System.Environment.NewLine, string.Empty);
                            x.MoldToolDescrip = s.Replace("&nbsp;", " ");
                        }
                    }
                }

                return PartialView("_ActionReview", LST.OrderBy(x=> x.CavityNumber).ToList());
            }

            else
            {

                CommonDrop();
                int MID = ReturnSelectedMoldID(MoldID);


                int NoofRec = 0;
                NoofRec = ShrdMaster.Instance.GetMaintenanceTblRoverSetData(MID).OrderBy(x => x.SetDate).Count();
                ViewBag.TotalMold = NoofRec;
                List<TblRoverSetDataViewModel> MD = new List<TblRoverSetDataViewModel>();

                var MoldData = RenderRazorViewToString(this.ControllerContext, "_MoldData", MD);
                int SETid = MD.Select(X=> X.SetID).FirstOrDefault();

                var OInfo = db.Database.SqlQuery<ActionReview>("GetActionReview @SetID, @CompanyID", new SqlParameter("SetID", SETid), new SqlParameter("@CompanyID", CID)).ToList();

                List<ActionReview> LST = new List<ActionReview>();

                foreach (var x in OInfo)
                {
                    ActionReview AR = new ActionReview();
                    AR.TlRepllD = x.TlRepllD;
                    AR.CavityNumberID = x.CavityNumberID;
                    AR.CavityNumber = x.CavityNumber;
                    AR.SetID = x.SetID;
                    AR.SetDate = x.SetDate;
                    AR.DfctDate = x.DfctDate;
                    AR.TSDefects = x.TSDefects;
                    AR.TSDate = x.TSDate;
                    AR.TlSTime = x.TlSTime;
                    AR.DfctCavNum = x.DfctCavNum;
                    AR.MoldToolDescrip = x.MoldToolDescrip;
                    AR.TlCorrectiveAction = x.TlCorrectiveAction;
                    AR.TIRepairTime = x.TIRepairTime;
                    LST.Add(AR);
                }


                foreach (var x in LST)
                {
                    if (x.MoldToolDescrip != null && x.MoldToolDescrip != "")
                    {
                        string[] sds = Regex.Split(x.MoldToolDescrip, "</p>");
                        string s = HtmlToPlainText(sds[0]);

                        if (s != "")
                        {
                            s = s.Replace(System.Environment.NewLine, string.Empty);
                            x.MoldToolDescrip = s.Replace("&nbsp;", " ");
                        }
                    }
                }

                var OtherInfo = RenderRazorViewToString(this.ControllerContext, "_ActionReview", LST.OrderBy(x => x.CavityNumber).ToList());
                return Json(new { MoldData, OtherInfo }, JsonRequestBehavior.AllowGet);
                //return PartialView("_ActionReview", new {MoldData, OtherInfo });
            }
        }
        #endregion

        #region Trouble Shooter Guide

        public ActionResult TroubleShooterGetData(int ID = 0, int MOLDChange = 0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            ViewBag.NoType = new SelectList(db.TblDDTSType.Where(x=> x.CompanyID == CID).ToList().OrderBy(x=> x.TSType).ThenBy(x=> x.TSType), "ID", "TSType");

            var dd = db.TblMoldData.Where(x=> x.CompanyID == CID).ToList().Select(x => new tblMoldData { MoldDataID = x.MoldDataID, MoldName = x.MoldName + ": " + x.MoldDesc });
            List<SelectListItem> Tech = new List<SelectListItem>();
            foreach (var x in dd.OrderBy(x => x.MoldName))
            {
                Tech.Add(new SelectListItem
                {
                    Text = x.MoldName,
                    Value = x.MoldDataID.ToString()
                });
            }
            ViewBag.MoldConfigList = Tech;

            int i = ReturnSelectedMoldID(ID);

            var TStype = db.TblDDTSType.Where(x=> x.CompanyID == CID).ToList();
            int TSI = 0;

            var TblMold = ShrdMaster.Instance.GetTblTSGuideList(i).ToList();
            List<tblTSGuide> OInfo = new List<tblTSGuide>();

            foreach (var x in TblMold)
            {
                tblTSGuide TS = new tblTSGuide();
                TS.TSGuide = x.TSGuide;
                TS.MoldDataID = x.MoldDataID;
                TS.TSSeqNum = x.TSSeqNum;
                TS.TSDefects = x.TSDefects;
                TS.TSExplanation = x.TSExplanation;
                TS.TSProbCause = x.TSProbCause;
                TS.TSToolInv = x.TSToolInv;
                TS.TSSolution = x.TSSolution;
                TS.TSType = x.TSType;
                if (TS.TSType != null && TS.TSType != "")
                {
                    TSI = Convert.ToInt32(x.TSType);
                    var Tsdd = TStype.Where(c => c.ID == TSI).FirstOrDefault();
                    TS.TSTypeName = Tsdd != null ? Tsdd.TSType : "";
                }

                TS.TSPreventAction = x.TSPreventAction;
                TS.ImageExtension = x.ImageExtension;
                TS.fileName = x.fileName;
                TS.ImagePath = x.ImagePath != null ? Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/TroubleShooterImage/" + x.ImagePath) : x.ImagePath;
                OInfo.Add(TS);

            }


            //var OInfo = db.TblTSGuide.Where(x => x.MoldDataID == i).ToList();
            //var OInfo = ShrdMaster.Instance.GetTblTSGuideList(i).Select(x => new tblTSGuide
            //{
            //    TSGuide = x.TSGuide,
            //    MoldDataID = x.MoldDataID,
            //    TSSeqNum = x.TSSeqNum,
            //    TSDefects = x.TSDefects,
            //    TSExplanation = x.TSExplanation,
            //    TSProbCause = x.TSProbCause,
            //    TSToolInv = x.TSToolInv,
            //    TSSolution = x.TSSolution,
            //    TSType = x.TSType,
            //    TSPreventAction = x.TSPreventAction,
            //    ImageExtension = x.ImageExtension,
            //    fileName = x.fileName,
            //    ImagePath = x.ImagePath != null ? Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/TroubleShooterImage/" + x.ImagePath) : null
            //});

            //var TStype = db.TblDDTSType.ToList();
            //int TSI = 0;

            //foreach (var x in OInfo)
            //{
            //    if (x.TSType != null && x.TSType != "")
            //    {
            //        TSI = Convert.ToInt32(x.TSType);
            //        var Tsdd = TStype.Where(c => c.ID == TSI).FirstOrDefault();
            //        x.TSType = Tsdd != null ? Tsdd.TSType : "";

            //    }
            //}

            if (MOLDChange == 0)
            {
                return PartialView("_TroubleShooterGetData", OInfo.OrderBy(x => x.TSTypeName).ThenBy(x => x.TSDefects));
            }

            else
            {
                CommonDrop();

                int NoofRec = 0;

                NoofRec = ShrdMaster.Instance.GetMaintenanceTblRoverSetData(ID).OrderBy(x => x.SetDate).Count();

                ViewBag.TotalMold = NoofRec;
                List<TblRoverSetDataViewModel> MD = new List<TblRoverSetDataViewModel>();

                var MoldData = RenderRazorViewToString(this.ControllerContext, "_MoldData", MD);
                var OtherInfo = RenderRazorViewToString(this.ControllerContext, "_TroubleShooterGetData", OInfo.OrderBy(x=> x.TSTypeName).ThenBy(x => x.TSDefects));
                return Json(new { MoldData, OtherInfo }, JsonRequestBehavior.AllowGet);
                //return PartialView("_TroubleShooterGetData", new {MoldData, OtherInfo });
            }
            //
        }

        public void CommonFunc()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            ViewBag.NoType = new SelectList(db.TblDDTSType.Where(x=> x.CompanyID == CID).ToList().OrderBy(x => x.TSType), "ID", "TSType");
        }

        [HttpGet]
        public ActionResult CreateTroubleShooter(int MoldID = 0)
        {
            CommonFunc();
            ViewBag.MoldID = MoldID;
            return PartialView("_CreateTroubleshooter");
        }

        //[HttpPost]
        //public ActionResult CreateTroubleShooter(tblTSGuide model)
        //{
        //    HttpPostedFileBase file = Request.Files[0];

        //    string fname;

        //    // Checking for Internet Explorer  
        //    if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
        //    {
        //        string[] testfiles = file.FileName.Split(new char[] { '\\' });
        //        fname = testfiles[testfiles.Length - 1];
        //    }
        //    else
        //    {
        //        fname = file.FileName;
        //    }

        //    // Get the complete folder path and store the file inside it.  
        //    tblTSGuide tbs = new tblTSGuide();
        //    tbs.TSType = model.TSType;
        //    tbs.TSDefects = model.TSDefects;
        //    tbs.TSExplanation = model.TSExplanation;
        //    tbs.TSProbCause = model.TSProbCause;
        //    tbs.TSSolution = model.TSSolution;
        //    tbs.TSPreventAction = model.TSPreventAction;
        //    tbs.MoldDataID = model.MoldDataID;
        //    string NewFName = tbs.MoldDataID + "_" + fname;
        //    fname = Path.Combine(Server.MapPath("~/TroubleShooterImage/"), NewFName);
        //    file.SaveAs(fname);
        //    tbs.ImagePath = NewFName;

        //    db.TblTSGuide.Add(tbs);
        //    db.SaveChanges();

        //    ViewBag.NoType = new SelectList(db.TblDDTSType.ToList(), "ID", "TSType");
        //    var Trouble = db.TblTSGuide.Where(x => x.MoldDataID == model.MoldDataID).ToList();
        //    return PartialView("_TroubleShooterGetData", Trouble);
        //}
        public void SaveTroubleShooter(tblTSGuideViewModel model)
        {
            var tb = db.TblTSGuide.Where(c => c.TSGuide == model.TSGuide).FirstOrDefault();
            int CID = ShrdMaster.Instance.GetCompanyID();

            if (tb == null)
            {
                tblTSGuide TSG = new tblTSGuide();
                TSG.TSGuide = model.TSGuide;
                TSG.TSType = model.TSType;
                TSG.CompanyID = CID;
                TSG.TSDefects = model.TSDefects;
                TSG.ImagePath = Path.GetFileName(model.ImagePath);
                TSG.TSExplanation = model.TSExplanation;
                TSG.TSProbCause = model.TSProbCause;
                TSG.TSSolution = model.TSSolution;
                TSG.TSPreventAction = model.TSPreventAction;
                TSG.MoldDataID = model.MoldDataID;
                db.TblTSGuide.Add(TSG);
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.TroubleshootGuide.ToString(), GetAction.Create.ToString());

            }

            else
            {
                tb.TSGuide = model.TSGuide;
                tb.TSType = model.TSType;
                tb.TSDefects = model.TSDefects;
                tb.TSExplanation = model.TSExplanation;
                tb.TSProbCause = model.TSProbCause;
                tb.TSSolution = model.TSSolution;
                tb.TSPreventAction = model.TSPreventAction;
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.TroubleshootGuide.ToString(), GetAction.Update.ToString());

            }
        }

        [HttpPost]
        public ActionResult CreateTroubleShooter(tblTSGuide model)
        {
            // Get the complete folder path and store the file inside it.  
            tblTSGuide tbs = new tblTSGuide();
            string UniqueNAME = ShrdMaster.Instance.ReturnUniqueName();

            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files[0];
                string fname;
                string ImgName = "";
                // Checking for Internet Explorer  
                if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                {
                    string[] testfiles = file.FileName.Split(new char[] { '\\' });
                    fname = testfiles[testfiles.Length - 1];
                }
                else
                {
                    fname = file.FileName;
                    ImgName = file.FileName;
                }

                string NewFName = UniqueNAME + "_" + fname;
                var FileExtension = Path.GetExtension(fname);
                //fname = Path.Combine(Server.MapPath("~/TroubleShooterImage/"), fname);

                //file.SaveAs(fname);
                fname = Path.Combine(Server.MapPath("~/TroubleShooterImage/"), NewFName);
                file.SaveAs(fname);


                if (FileExtension == ".vsdx" || FileExtension == ".vsd")
                {
                    var FileP = ShrdMaster.Instance.ConvertVisioToImg(fname);
                    tbs.ImagePath = FileP;
                }
                else
                {
                    tbs.ImagePath = NewFName;
                }
                //tbs.ImagePath = NewFName;
            }

            int CID = ShrdMaster.Instance.GetCompanyID();


            tbs.TSType = model.TSType;
            tbs.CompanyID = CID;
            tbs.TSDefects = model.TSDefects;
            tbs.TSExplanation = model.TSExplanation;
            tbs.TSProbCause = model.TSProbCause;
            tbs.TSSolution = model.TSSolution;
            tbs.TSPreventAction = model.TSPreventAction;
            tbs.MoldDataID = model.MoldDataID;


            db.TblTSGuide.Add(tbs);
            db.SaveChanges();
            //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.TroubleshootGuide.ToString(), GetAction.Create.ToString());

            CommonFunc();
            var dd = db.TblMoldData.Where(X=> X.CompanyID == CID).ToList().Select(x => new tblMoldData { MoldDataID = x.MoldDataID, MoldName = x.MoldName + ": " + x.MoldDesc });
            List<SelectListItem> Tech = new List<SelectListItem>();
            foreach (var x in dd.OrderBy(x => x.MoldName))
            {
                Tech.Add(new SelectListItem
                {
                    Text = x.MoldName,
                    Value = x.MoldDataID.ToString()
                });
            }
            ViewBag.MoldConfigList = Tech;

            var Trouble12 = db.TblTSGuide.Where(x => x.MoldDataID == model.MoldDataID && x.CompanyID == CID).ToList();
            List<tblTSGuide> Trouble = new List<tblTSGuide>();
            var TStype = db.TblDDTSType.Where(X=> X.CompanyID == CID).ToList();
            int TSI = 0;

            foreach (var x in Trouble12)
            {
                tblTSGuide TSG = new tblTSGuide();
                TSG.TSGuide = x.TSGuide;
                TSG.MoldDataID = x.MoldDataID;
                TSG.TSSeqNum = x.TSSeqNum;
                TSG.TSDefects = x.TSDefects;
                TSG.TSExplanation = x.TSExplanation;
                TSG.CompanyID = CID;
                TSG.TSProbCause = x.TSProbCause;
                TSG.TSToolInv = x.TSToolInv;
                TSG.TSSolution = x.TSSolution;
                TSG.TSType = x.TSType;
                if (TSG.TSType != null && TSG.TSType != "")
                {
                    TSI = Convert.ToInt32(TSG.TSType);
                    var Tsdd = TStype.Where(c => c.ID == TSI).FirstOrDefault();
                    TSG.TSTypeName = Tsdd != null ? Tsdd.TSType : "";
                }

                TSG.TSPreventAction = x.TSPreventAction;
                TSG.ImageExtension = x.ImageExtension;
                TSG.fileName = x.fileName;
                TSG.ImagePath = x.ImagePath != null ? Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/TroubleShooterImage/" + x.ImagePath) : null;
                Trouble.Add(TSG);
            }


            //var Trouble = db.TblTSGuide.Where(x => x.MoldDataID == model.MoldDataID).Select(x => new tblTSGuide
            //{
            //    TSGuide = x.TSGuide,
            //    MoldDataID = x.MoldDataID,
            //    TSSeqNum = x.TSSeqNum,
            //    TSDefects = x.TSDefects,
            //    TSExplanation = x.TSExplanation,
            //    TSProbCause = x.TSProbCause,
            //    TSToolInv = x.TSToolInv,
            //    TSSolution = x.TSSolution,
            //    TSType = x.TSType,
            //    TSPreventAction = x.TSPreventAction,
            //    ImageExtension = x.ImageExtension,
            //    fileName = x.fileName,
            //    ImagePath = Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/TroubleShooterImage/" + x.ImagePath)
            //}).ToList();

            return PartialView("_TroubleShooterGetData", Trouble.OrderBy(x => x.TSTypeName).ThenBy(x => x.TSDefects));
        }

        //[HttpPost]
        //public ActionResult CreatetrbleShooter(tblTSGuide model, HttpPostedFileBase AddImg)
        //{
        //    if (model != null)
        //    {
        //        tblTSGuide tbs = new tblTSGuide();
        //        tbs.TSType = model.TSType;
        //        tbs.TSDefects = model.TSDefects;
        //        tbs.TSExplanation = model.TSExplanation;
        //        tbs.TSProbCause = model.TSProbCause;
        //        tbs.TSSolution = model.TSSolution;
        //        tbs.TSPreventAction = model.TSPreventAction;
        //        tbs.MoldDataID = model.MoldDataID;
        //        string TroubleShooterPath = Server.MapPath("~/TroubleShooterImage/");
        //        if (AddImg != null)
        //        {
        //            string FName = Path.GetFileName(AddImg.FileName);
        //            string NewFName = tbs.MoldDataID + "_" + FName;
        //            string FilePath = TroubleShooterPath + NewFName;
        //            AddImg.SaveAs(FilePath);
        //            tbs.ImagePath = NewFName;
        //        }

        //        db.TblTSGuide.Add(tbs);
        //        db.SaveChanges();
        //    }

        //    return RedirectToAction("Index", "MaintenanceTracking");

        //}




        [HttpPost]
        public JsonResult AutoComplete(string prefix)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            var dd = db.TblMoldData.Where(X=> X.CompanyID == CID).ToList().Select(x => new { ID = x.MoldDataID, Name = x.MoldName + ": " + x.MoldDesc });
            var customers = dd.Where(x => x.Name.ToUpper().Contains(prefix.ToUpper())).Select(x => new
            {
                label = x.Name,
                val = x.ID

            }).ToList();

            return Json(customers);
        }




        [HttpPost]
        public ActionResult UplaodImage(int TsGuide = 0)
        {
            try
            {
                //  Get all files from Request object  
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                    //string filename = Path.GetFileName(Request.Files[i].FileName);  

                    HttpPostedFileBase file = files[i];
                    string fname;
                    string ImgName = "";
                    string UniqueNAME = ShrdMaster.Instance.ReturnUniqueName();
                    var dd = db.TblTSGuide.Where(x => x.TSGuide == TsGuide).FirstOrDefault();

                    // Checking for Internet Explorer  
                    if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                    {
                        string[] testfiles = file.FileName.Split(new char[] { '\\' });
                        fname = testfiles[testfiles.Length - 1];
                    }
                    else
                    {
                        fname = UniqueNAME + "_" + file.FileName;
                        ImgName = UniqueNAME + "_" + file.FileName;
                    }

                    // Get the complete folder path and store the file inside it.  
                    var FileExtension = Path.GetExtension(fname);
                    fname = Path.Combine(Server.MapPath("~/TroubleShooterImage/"), fname);

                    file.SaveAs(fname);

                    if (FileExtension == ".vsdx" || FileExtension == ".vsd")
                    {
                        var FileP = ShrdMaster.Instance.ConvertVisioToImg(fname);
                        dd.ImagePath = FileP;
                        db.SaveChanges();
                        var FilePath = Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/TroubleShooterImage/" + FileP);
                        return Json(FilePath);
                    }
                    else
                    {
                        int CID = ShrdMaster.Instance.GetCompanyID();

                        dd.ImagePath = ImgName;
                        db.SaveChanges();
                        var FilePath = Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/TroubleShooterImage/" + ImgName);
                        return Json(FilePath);
                    }
                }
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.TroubleshootGuide.ToString(), GetAction.Update.ToString());

                // Returns message that successfully uploaded  
                return Json("File Uploaded Successfully!");
            }
            catch (Exception ex)
            {
                return Json("Error occurred. Error details: " + ex.Message);
            }
        }

        public ActionResult DeleteTrouble(string str, int MoldID = 0)
        {
            //str = str.Substring(0, str.LastIndexOf(','));
            //string[] CompIds = str.Split(',');

            if (str != "")
            {
                //var dd = db.TblTSGuide.Where(c => c.TSGuide == Convert.ToInt32(x)).FirstOrDefault();
                //db.TblTSGuide.Remove(dd);
                //db.SaveChanges();

                str = str.Substring(0, str.LastIndexOf(','));
                using (var ctx = new MoldtraxDbContext())
                {
                    SqlParameter sp = new SqlParameter("@value", str);
                    var result = ctx.Database.ExecuteSqlCommand("exec sp_DeleteTroubleShooter @value", sp);
                }

                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.TroubleshootGuide.ToString(), GetAction.Delete.ToString());

            }

            CommonFunc();
            int CID = ShrdMaster.Instance.GetCompanyID();

            var Trouble12 = db.TblTSGuide.Where(x => x.MoldDataID == MoldID && x.CompanyID == CID).ToList();
            List<tblTSGuide> data = new List<tblTSGuide>();
            var TStype = db.TblDDTSType.ToList();
            int TSI = 0;

            foreach (var x in Trouble12)
            {
                tblTSGuide TSG = new tblTSGuide();
                TSG.TSGuide = x.TSGuide;
                TSG.MoldDataID = x.MoldDataID;
                TSG.TSSeqNum = x.TSSeqNum;
                TSG.TSDefects = x.TSDefects;
                TSG.TSExplanation = x.TSExplanation;
                TSG.TSProbCause = x.TSProbCause;
                TSG.CompanyID = CID;
                TSG.TSToolInv = x.TSToolInv;
                TSG.TSSolution = x.TSSolution;
                TSG.TSType = x.TSType;
                if (TSG.TSType != null && TSG.TSType != "")
                {
                    TSI = Convert.ToInt32(TSG.TSType);
                    var Tsdd = TStype.Where(c => c.ID == TSI).FirstOrDefault();
                    TSG.TSTypeName = Tsdd != null ? Tsdd.TSType : "";
                }
                TSG.TSPreventAction = x.TSPreventAction;
                TSG.ImageExtension = x.ImageExtension;
                TSG.fileName = x.fileName;
                TSG.ImagePath = x.ImagePath != null ? Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/TroubleShooterImage/" + x.ImagePath) : null;
                data.Add(TSG);
            }


            //var data = db.TblTSGuide.Where(x => x.MoldDataID == MoldID).Select(x => new tblTSGuide
            //{
            //    TSGuide = x.TSGuide,
            //    MoldDataID = x.MoldDataID,
            //    TSSeqNum = x.TSSeqNum,
            //    TSDefects = x.TSDefects,
            //    TSExplanation = x.TSExplanation,
            //    TSProbCause = x.TSProbCause,
            //    TSToolInv = x.TSToolInv,
            //    TSSolution = x.TSSolution,
            //    TSType = x.TSType,
            //    TSPreventAction = x.TSPreventAction,
            //    ImageExtension = x.ImageExtension,
            //    fileName = x.fileName,
            //    ImagePath = Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/TroubleShooterImage/" + x.ImagePath)
            //}).ToList();

            var dd = db.TblMoldData.Where(X=> X.CompanyID == CID).ToList().Select(x => new tblMoldData { MoldDataID = x.MoldDataID, MoldName = x.MoldName + ": " + x.MoldDesc });
            List<SelectListItem> Tech = new List<SelectListItem>();
            foreach (var x in dd.OrderBy(x => x.MoldName))
            {
                Tech.Add(new SelectListItem
                {
                    Text = x.MoldName,
                    Value = x.MoldDataID.ToString()
                });
            }
            ViewBag.MoldConfigList = Tech;
            return PartialView("_TroubleShooterGetData", data.OrderBy(x => x.TSTypeName).ThenBy(x => x.TSDefects));

        }

        public ActionResult EditTroubleShooter(string str, int MoldID)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            str = str.Substring(0, str.LastIndexOf(','));
            List<int> TroubleList = str.Split(',').Select(int.Parse).ToList();
            var Trouble = db.TblTSGuide.Where(X=>X.CompanyID == CID).ToList();

            List<tblTSGuide> tl = new List<tblTSGuide>();

            foreach (var x in TroubleList)
            {
                var ds = Trouble.Where(s => s.TSGuide == x).FirstOrDefault();
                tl.Add(ds);
            }
            return PartialView("_EditTroubleShooter", tl);
        }

        public ActionResult PasteCopyTroubleShooter(List<tblTSGuideViewModel> model, int MainMoldID=0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            foreach (var x in model)
            {
                tblTSGuide TD  = new tblTSGuide();
                TD.MoldDataID = x.MoldDataID;
                TD.CompanyID = CID;
                TD.TSSeqNum = x.TSSeqNum;
                TD.TSDefects = x.TSDefects;
                TD.TSExplanation = x.TSExplanation;
                TD.TSProbCause = x.TSProbCause;
                TD.TSToolInv = x.TSToolInv;
                TD.TSSolution = x.TSSolution;
                TD.ImagePath = Path.GetFileName(x.ImagePath);
                TD.TSType = x.TSType;
                TD.TSPreventAction = x.TSPreventAction;
                TD.fileName = x.fileName;
                db.TblTSGuide.Add(TD);
                db.SaveChanges();
            }
            var TStype = db.TblDDTSType.Where(X=> X.CompanyID == CID).ToList();
            int TSI = 0;

            var TblMold = ShrdMaster.Instance.GetTblTSGuideList(MainMoldID).ToList();
            List<tblTSGuide> data = new List<tblTSGuide>();

            foreach (var x in TblMold)
            {
                tblTSGuide TS = new tblTSGuide();
                TS.TSGuide = x.TSGuide;
                TS.MoldDataID = x.MoldDataID;
                TS.TSSeqNum = x.TSSeqNum;
                TS.TSDefects = x.TSDefects;
                TS.TSExplanation = x.TSExplanation;
                TS.TSProbCause = x.TSProbCause;
                TS.TSToolInv = x.TSToolInv;
                TS.TSSolution = x.TSSolution;
                TS.TSType = x.TSType;
                TS.CompanyID = CID;
                if (TS.TSType != null && TS.TSType != "")
                {
                    TSI = Convert.ToInt32(x.TSType);
                    var Tsdd = TStype.Where(c => c.ID == TSI).FirstOrDefault();
                    TS.TSTypeName = Tsdd != null ? Tsdd.TSType : "";
                }

                TS.TSPreventAction = x.TSPreventAction;
                TS.ImageExtension = x.ImageExtension;
                TS.fileName = x.fileName;
                TS.ImagePath = x.ImagePath != null ? Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/TroubleShooterImage/" + x.ImagePath) : x.ImagePath;
                data.Add(TS);
            }

            //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.TroubleshootGuide.ToString(), GetAction.Paste.ToString());


            //var data = ShrdMaster.Instance.GetTblTSGuideList(MainMoldID).Select(x => new tblTSGuide
            //{
            //    TSGuide = x.TSGuide,
            //    MoldDataID = x.MoldDataID,
            //    TSSeqNum = x.TSSeqNum,
            //    TSDefects = x.TSDefects,
            //    TSExplanation = x.TSExplanation,
            //    TSProbCause = x.TSProbCause,
            //    TSToolInv = x.TSToolInv,
            //    TSSolution = x.TSSolution,
            //    TSType = x.TSType,
            //    TSPreventAction = x.TSPreventAction,
            //    ImageExtension = x.ImageExtension,
            //    fileName = x.fileName,
            //    ImagePath = x.ImagePath != null ? Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/TroubleShooterImage/" + x.ImagePath) : null
            //});

            CommonFunc();
            var dd = db.TblMoldData.Where(X=> X.CompanyID == CID).ToList().Select(x => new tblMoldData { MoldDataID = x.MoldDataID, MoldName = x.MoldName + ": " + x.MoldDesc });
            List<SelectListItem> Tech = new List<SelectListItem>();
            foreach (var x in dd.OrderBy(x => x.MoldName))
            {
                Tech.Add(new SelectListItem
                {
                    Text = x.MoldName,
                    Value = x.MoldDataID.ToString()
                });
            }

            //var TStype = db.TblDDTSType.ToList();
            //int TSI = 0;


            //foreach (var x in data)
            //{
            //    if (x.TSType != null && x.TSType != "")
            //    {
            //        TSI = Convert.ToInt32(x.TSType);
            //        var Tsdd = TStype.Where(c => c.ID == TSI).FirstOrDefault();
            //        x.TSTypeName = Tsdd != null ? Tsdd.TSType : "";

            //    }
            //}

            ViewBag.MoldConfigList = Tech;
            return PartialView("_TroubleShooterGetData", data.OrderBy(x => x.TSTypeName).ThenBy(x => x.TSDefects));
            //return Json(data, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public ActionResult EditTroubleShooter(List<tblTSGuideViewModel> model)
        //{
        //    var dd = db.TblTSGuide.ToList();

        //    foreach (var x in model)
        //    {
        //        var tb = dd.Where(c => c.TSGuide == x.TSGuide).FirstOrDefault();
        //        tb.TSGuide = x.TSGuide;
        //        tb.TSType = x.TSType;
        //        tb.TSDefects = x.TSDefects;
        //        tb.TSExplanation = x.TSExplanation;
        //        tb.TSProbCause = x.TSProbCause;
        //        tb.TSSolution = x.TSSolution;
        //        tb.TSPreventAction = x.TSPreventAction;

        //        db.SaveChanges();

        //    }
        //    return Json("", JsonRequestBehavior.AllowGet);
        //}


        [HttpPost]
        public ActionResult EditTroubleShooter(List<tblTSGuideViewModel> model)
        {
            //var dd = ShrdMaster.Instance.GetTblTSGuideList();
            //var dd = db.TblTSGuide.ToList();
            int CID = ShrdMaster.Instance.GetCompanyID();

            if (model != null)
            {
                foreach (var x in model)
                {
                    var tb = db.TblTSGuide.Where(c => c.TSGuide == x.TSGuide).FirstOrDefault();
                    if (tb != null)
                    {
                        tb.TSGuide = x.TSGuide;
                        tb.TSType = x.TSType;
                        tb.TSDefects = x.TSDefects;
                        tb.TSExplanation = x.TSExplanation;
                        tb.TSProbCause = x.TSProbCause;
                        tb.TSSolution = x.TSSolution;
                        tb.TSPreventAction = x.TSPreventAction;
                        db.SaveChanges();
                    }
                }

                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.TroubleshootGuide.ToString(), GetAction.Update.ToString());

            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteImage(int TsGuide = 0)
        {
            var da = db.TblTSGuide.Where(x => x.TSGuide == TsGuide).FirstOrDefault();
            if (da != null)
            {
                da.ImagePath = null;
                db.SaveChanges();
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportTroubleData(HttpPostedFileBase postedFile, int MID = 0)
        {
            string filePath = string.Empty;
            if (postedFile != null)
            {
                string path = Server.MapPath("~/DropDownTemplate/");

                System.IO.DirectoryInfo di = new DirectoryInfo(path);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = path + Path.GetFileName(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);
                postedFile.SaveAs(filePath);

                string connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0 Macro;HDR=YES;IMEX=1;';", filePath);

                DataSet data = new DataSet();

                using (OleDbConnection con = new OleDbConnection(connectionString))
                {
                    con.Open();
                    var dataTable = new DataTable();
                    DataTable dtExcelSchema;
                    dtExcelSchema = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                    //string query = string.Format("SELECT * FROM [{0}]", sheetName);
                    string query = string.Format("SELECT * FROM [{0}]", sheetName);

                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, con);
                    adapter.Fill(dataTable);
                    data.Tables.Add(dataTable);
                    string[] ShetN = sheetName.Split('$');
                    TroubleShooterDataInsert(dataTable, ShetN[0], MID);
                    con.Close();
                }
            }

            var NoT = db.TblDDTSType.ToList();
            List<SelectListItem> NoType = new List<SelectListItem>();
            foreach (var x in NoT)
            {
                NoType.Add(new SelectListItem
                {
                    Text = x.TSType,
                    Value = x.ID.ToString()
                });
            }

            int CID = ShrdMaster.Instance.GetCompanyID();

            ViewBag.NoType = NoType.OrderBy(x => x.Text).ThenBy(x => x.Text);

            var dd = db.TblMoldData.Where(X=> X.CompanyID == CID).ToList().Select(x => new tblMoldData { MoldDataID = x.MoldDataID, MoldName = x.MoldName + ": " + x.MoldDesc });
            List<SelectListItem> Tech = new List<SelectListItem>();
            foreach (var x in dd.OrderBy(x => x.MoldName))
            {
                Tech.Add(new SelectListItem
                {
                    Text = x.MoldName,
                    Value = x.MoldDataID.ToString()
                });
            }

            ViewBag.MoldConfigList = Tech;

            var TblMold = ShrdMaster.Instance.GetTblTSGuideList(MID).ToList();

            List<tblTSGuide> TblTSGuideList = new List<tblTSGuide>();
            int TSI = 0;
            var TStype = db.TblDDTSType.Where(X=> X.CompanyID == CID).ToList();

            //string dbName = "";
            //if (System.Web.HttpContext.Current.Session != null && System.Web.HttpContext.Current.Session["DbName"] != null)
            //    dbName = Convert.ToString(System.Web.HttpContext.Current.Session["DbName"]);
            //else
            //{
            //    return RedirectToAction("Login", "Account");
            //}

            foreach (var x in TblMold)
            {
                tblTSGuide TS = new tblTSGuide();
                TS.TSGuide = x.TSGuide;
                TS.MoldDataID = x.MoldDataID;
                TS.TSSeqNum = x.TSSeqNum;
                TS.TSDefects = x.TSDefects;
                TS.TSExplanation = x.TSExplanation;
                TS.CompanyID = CID;
                TS.TSProbCause = x.TSProbCause;
                TS.TSToolInv = x.TSToolInv;
                TS.TSSolution = x.TSSolution;
                TS.TSType = x.TSType;
                if (TS.TSType != null && TS.TSType != "")
                {
                    TSI = Convert.ToInt32(x.TSType);
                    var Tsdd = TStype.Where(c => c.ID == TSI).FirstOrDefault();
                    TS.TSTypeName = Tsdd != null ? Tsdd.TSType : "";
                }

                TS.TSPreventAction = x.TSPreventAction;
                TS.ImageExtension = x.ImageExtension;
                TS.fileName = x.fileName;
                TS.ImagePath = x.ImagePath != null ? Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/TroubleShooterImage/" + x.ImagePath) : x.ImagePath;
                TblTSGuideList.Add(TS);
            }

            return PartialView("_TroubleShooterGetData", TblTSGuideList.OrderBy(x => x.TSTypeName).ThenBy(x => x.TSDefects).ToList());
        }

        public void TroubleShooterDataInsert(DataTable dt, string sheetName, int MID = 0)
        {
            //string constring = db.GetConnectionString();
            //SqlConnection con = new SqlConnection(constring);

            using (SqlConnection connection = new SqlConnection(constring))
            {
                int CID = ShrdMaster.Instance.GetCompanyID();

                List<tblTSGuide> SR = new List<tblTSGuide>();
                var data = db.TblDDMoldCategoryID.Where(X=> X.CompanyID == CID).ToList();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i].ItemArray[0] != null && dt.Rows[i].ItemArray[1] != null)
                    {
                        string Desc = dt.Rows[i].ItemArray[1].ToString() == "" ? null : dt.Rows[i].ItemArray[1].ToString();

                        var dd = data.Where(x => x.MoldCategoryID == dt.Rows[i].ItemArray[0].ToString() && x.MoldCategoryIDDesc == Desc).FirstOrDefault();
                        var ToolingTypeData = db.TblDDTSType.Where(X=> X.CompanyID == CID).ToList();

                        if (dd == null)
                        {
                            //TblDDMoldToolingTypes
                            tblTSGuide Tool = new tblTSGuide();

                            var ST = ToolingTypeData.Where(x => x.TSType == dt.Rows[i].ItemArray[0].ToString()).FirstOrDefault();
                            Tool.TSType = ST == null ? "0" : ST.ID.ToString();
                            Tool.MoldDataID = MID;
                            Tool.CompanyID = CID;
                            Tool.TSDefects = dt.Rows[i].ItemArray[1].ToString();
                            Tool.TSExplanation = dt.Rows[i].ItemArray[2].ToString();
                            Tool.TSProbCause = dt.Rows[i].ItemArray[3].ToString();
                            Tool.TSSolution = dt.Rows[i].ItemArray[4].ToString();
                            Tool.TSPreventAction = dt.Rows[i].ItemArray[5].ToString();
                            SR.Add(Tool);
                        }
                    }
                }


                DataTable NewDT = ToDataTable(SR);
                connection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    //foreach (DataColumn c in NewDT.Columns)
                    bulkCopy.ColumnMappings.Add("MoldDataID", "MoldDataID");
                    bulkCopy.ColumnMappings.Add("TSType", "TSType");
                    bulkCopy.ColumnMappings.Add("CompanyID", "CompanyID");
                    bulkCopy.ColumnMappings.Add("TSDefects", "TSDefects");
                    bulkCopy.ColumnMappings.Add("TSExplanation", "TSExplanation");
                    bulkCopy.ColumnMappings.Add("TSProbCause", "TSProbCause");
                    bulkCopy.ColumnMappings.Add("TSSolution", "TSSolution");
                    bulkCopy.ColumnMappings.Add("TSPreventAction", "TSPreventAction");

                    bulkCopy.DestinationTableName = "tblTSGuide";
                    try
                    {
                        bulkCopy.WriteToServer(NewDT);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                connection.Close();
            }
        }

        #endregion

        #region Tooling
        public ActionResult ToolingGetData(int ID = 0, int MOLDChange = 0)
        {

            int CID = ShrdMaster.Instance.GetCompanyID();
            int i = ReturnSelectedMoldID(ID);
            CommonDropDownVal();
            var data = ShrdMaster.Instance.GetToolingList(i, CID);

            var ToolingType = db.TblDDMoldToolingTypes.Where(X=> X.CompanyID == CID).ToList();
            int I = 0;

            foreach (var x in data)
            {
                if (x.MoldToolingType != "" && x.MoldToolingType != null)
                {
                    I = Convert.ToInt32(x.MoldToolingType);
                }

                var Tool = ToolingType.Where(c => c.ID == I).FirstOrDefault();
                x.MoldToolingTypeName = Tool != null ? Tool.DD_MoldToolingType : "";
            }

            var OInfo = data.Where(x => x.MoldDataID == i && x.CompanyID == CID).OrderBy(x => x.MoldToolingTypeName).ThenBy(x => x.MoldToolDescrip).ToList();

            if (MOLDChange == 0)
            {
                return PartialView("_ToolingGetData", OInfo);
            }
            else
            {
                CommonDrop();
                int NoofRec = 0;
                NoofRec = ShrdMaster.Instance.GetMaintenanceTblRoverSetData(i).OrderBy(x => x.SetDate).Count();
                ViewBag.TotalMold = NoofRec;
                List<TblRoverSetDataViewModel> MD = new List<TblRoverSetDataViewModel>();

                var MoldData = RenderRazorViewToString(this.ControllerContext, "_MoldData", MD);
                var OtherInfo = RenderRazorViewToString(this.ControllerContext, "_ToolingGetData", OInfo);
                return Json(new { MoldData, OtherInfo }, JsonRequestBehavior.AllowGet);
                //return PartialView("_ToolingGetData", new {MoldData, OtherInfo });
            }
        }

        public void CommonDropDownVal()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            ViewBag.BaseStyle = new SelectList(db.TblDDMoldCategoryID.Where(X=> X.CompanyID == CID).ToList(), "ID", "MoldCategoryID");
            ViewBag.Department = new SelectList(db.TblDDDepartmentID.Where(X => X.CompanyID == CID).ToList(), "ID", "DepartmentID");
            ViewBag.ProdLine = new SelectList(db.TblDDProductLine.Where(X => X.CompanyID == CID).ToList(), "ID", "ProductLine");
            ViewBag.ProdPart = new SelectList(db.TblDDProductPart.Where(X => X.CompanyID == CID).ToList(), "ID", "ProductPart");
            ViewBag.ResinType = new SelectList(db.TblDDMoldResinType.Where(X => X.CompanyID == CID).ToList(), "ID", "MoldResinType");
            ViewBag.RunnerType = new SelectList(db.TblDDMoldCav.Where(X => X.CompanyID == CID).ToList(), "ID", "MoldCav");
            ViewBag.ClientInfo = new SelectList(db.TblCustomer.Where(X => X.CompanyID == CID).ToList(), "CustomerID", "CustomerName");



            var ToolingType = db.TblDDMoldToolingTypes.Where(X => X.CompanyID == CID).OrderBy(x => x.DD_MoldToolingType).ToList();
            List<SelectListItem> ToolingTyp = new List<SelectListItem>();
            foreach (var x in ToolingType)
            {
                ToolingTyp.Add(new SelectListItem
                {
                    Text = x.DD_MoldToolingType,
                    Value = x.ID.ToString()
                });
            }

            ViewBag.MoldTooling = ToolingTyp;

            var dd = db.TblMoldData.Where(X => X.CompanyID == CID).ToList().Select(x => new tblMoldData { MoldDataID = x.MoldDataID, MoldName = x.MoldName + ": " + x.MoldDesc });
            List<SelectListItem> Tech = new List<SelectListItem>();
            foreach (var x in dd.OrderBy(x => x.MoldName))
            {
                Tech.Add(new SelectListItem
                {
                    Text = x.MoldName,
                    Value = x.MoldDataID.ToString()
                });
            }
            ViewBag.MoldConfigList = Tech;
        }

        public ActionResult CreateTooling(int MoldID=0)
        {
            ViewBag.MoldID = MoldID;
            CommonDropDownVal();
            return PartialView("_CreateTooling", new tblMoldTooling());
        }

        [HttpPost]
        public ActionResult CreateTooling(tblMoldTooling model)
        {
            CommonDropDownVal();
            List<tblMoldTooling> data = new List<tblMoldTooling>();
            if (model != null)
            {
                int CID = ShrdMaster.Instance.GetCompanyID();
                model.CompanyID = CID;
                db.TblMoldTooling.Add(model);
                db.SaveChanges();

                data = db.TblMoldTooling.Where(x => x.MoldDataID == model.MoldDataID && x.CompanyID == CID).ToList();

                var ToolingType = db.TblDDMoldToolingTypes.Where(X => X.CompanyID == CID).ToList();
                int I = 0;

                foreach (var x in data)
                {
                    if (x.MoldToolingType != "" && x.MoldToolingType != null)
                    {
                        I = Convert.ToInt32(x.MoldToolingType);
                    }

                    var Tool = ToolingType.Where(c => c.ID == I).FirstOrDefault();
                    x.MoldToolingTypeName = Tool != null ? Tool.DD_MoldToolingType : "";
                }

                data = data.OrderBy(x => x.MoldToolingTypeName).ThenBy(x => x.MoldToolDescrip).ToList();
            }

            CommonDropDownVal();
            //var NewData = db.TblMoldTooling.Where(x => x.MoldDataID == MoldID).ToList();
            //return PartialView("_ToolingGetData", NewData);

            return PartialView("_ToolingGetData", data);
        }

        //[HttpPost]
        //public ActionResult EditTooling(List<tblMoldTooling> model)
        //{
        //    var dd = db.TblMoldTooling.ToList();

        //    foreach (var x in model)
        //    {
        //        var tb = dd.Where(c => c.MoldToolingID == x.MoldToolingID).FirstOrDefault();

        //        tb.MoldToolingType = x.MoldToolingType;
        //        tb.MoldToolDescrip = x.MoldToolDescrip;
        //        tb.MoldToolingPartNumber = x.MoldToolingPartNumber;
        //        tb.MoldToolingPrintNumber = x.MoldToolingPrintNumber;
        //        tb.MoldToolingVendor = x.MoldToolingVendor;
        //        tb.MoldToolCost = x.MoldToolCost;
        //        tb.MoldToolingPartsOnHand = x.MoldToolingPartsOnHand;
        //        tb.MoldToolingReorderLevel = x.MoldToolingReorderLevel;
        //        tb.MoldToolingNumOrdered = x.MoldToolingNumOrdered;
        //        tb.MoldToolingDateOrdered = x.MoldToolingDateOrdered;
        //        tb.MoldToolingNumReceived = x.MoldToolingNumReceived;

        //        db.SaveChanges();
        //    }
        //    return Json("", JsonRequestBehavior.AllowGet);
        //}

        public void SaveToolingFunc(tblMoldTooling model)
        {
            if (model.MoldToolingID != 0)
            {
                int CID = ShrdMaster.Instance.GetCompanyID();

                var tb = db.TblMoldTooling.Where(c => c.MoldToolingID == model.MoldToolingID).FirstOrDefault();
                if (tb == null)
                {
                    model.CompanyID = CID; 
                    db.TblMoldTooling.Add(model);
                    db.SaveChanges();
                    //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.Tooling.ToString(), GetAction.Create.ToString());

                }

                else
                {
                    tb.MoldToolingType = model.MoldToolingType;
                    tb.MoldToolDescrip = model.MoldToolDescrip;
                    tb.MoldToolingPartNumber = model.MoldToolingPartNumber;
                    tb.MoldToolingPrintNumber = model.MoldToolingPrintNumber;
                    tb.MoldToolingVendor = model.MoldToolingVendor;
                    tb.MoldToolCost = model.MoldToolCost;
                    tb.Location = model.Location;
                    tb.MoldToolingPartsOnHand = model.MoldToolingPartsOnHand;
                    tb.MoldToolingReorderLevel = model.MoldToolingReorderLevel;
                    tb.MoldToolingNumOrdered = model.MoldToolingNumOrdered;
                    tb.MoldToolingDateOrdered = model.MoldToolingDateOrdered;
                    tb.MoldToolingNumReceived = model.MoldToolingNumReceived;
                    db.SaveChanges();
                    //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.Tooling.ToString(), GetAction.Update.ToString());

                }
            }
        }


        public ActionResult PasteToolingData(List<tblMoldTooling> model, int MainMoldID=0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            foreach (var x in model)
            {
                tblMoldTooling TB = new tblMoldTooling();
                TB.MoldDataID = x.MoldDataID;
                TB.CompanyID = CID;
                TB.MoldToolingType = x.MoldToolingType;
                TB.MoldToolDescrip = x.MoldToolDescrip;
                TB.MoldToolingPartNumber = x.MoldToolingPartNumber;
                TB.MoldToolCost = x.MoldToolCost;
                TB.MoldCostDate = x.MoldCostDate;
                TB.MoldManHours = x.MoldManHours;
                TB.Location = x.Location;
                TB.MoldToolingImage = x.MoldToolingImage;
                TB.MoldToolingVendor = x.MoldToolingVendor;
                TB.MoldToolingPrintNumber = x.MoldToolingPrintNumber;
                TB.MoldToolingPartsOnHand = x.MoldToolingPartsOnHand;
                TB.MoldToolingReorderLevel = x.MoldToolingReorderLevel;
                TB.MoldToolingNumOrdered = x.MoldToolingNumOrdered;
                TB.MoldToolingDateOrdered = x.MoldToolingDateOrdered;
                TB.MoldToolingNumReceived = x.MoldToolingNumReceived;

                db.TblMoldTooling.Add(TB);
                db.SaveChanges();
            }

            CommonDropDownVal();
            //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.Tooling.ToString(), GetAction.Paste.ToString());

            var data = db.TblMoldTooling.Where(x => x.MoldDataID == MainMoldID && x.CompanyID == CID).ToList();
            return PartialView("_ToolingGetData", data);
        }


        [HttpPost]
        public ActionResult EditTooling(List<tblMoldTooling> model)
        {
            if (model != null)
            {
                foreach (var x in model)
                {
                    int CID = ShrdMaster.Instance.GetCompanyID();

                    var tb = db.TblMoldTooling.Where(c => c.MoldToolingID == x.MoldToolingID).FirstOrDefault();
                    if (tb != null)
                    {
                        tb.MoldToolingType = x.MoldToolingType;
                        tb.MoldToolDescrip = x.MoldToolDescrip;
                        tb.MoldToolingPartNumber = x.MoldToolingPartNumber;
                        tb.Location = x.Location;
                        tb.MoldToolingPrintNumber = x.MoldToolingPrintNumber;
                        tb.MoldToolingVendor = x.MoldToolingVendor;
                        tb.MoldToolCost = x.MoldToolCost;
                        tb.MoldToolingPartsOnHand = x.MoldToolingPartsOnHand;
                        tb.MoldToolingReorderLevel = x.MoldToolingReorderLevel;
                        tb.MoldToolingNumOrdered = x.MoldToolingNumOrdered;
                        tb.MoldToolingDateOrdered = x.MoldToolingDateOrdered;
                        tb.MoldToolingNumReceived = x.MoldToolingNumReceived;
                        db.SaveChanges();
                    }
                }

                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.Tooling.ToString(), GetAction.Update.ToString());

            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteTooling(string str, int MoldID)
        {
            //str = str.Substring(0, str.LastIndexOf(','));
            //string[] CompIds = str.Split(',');

            if (str != "")
            {
                //var dd = db.TblTSGuide.Where(c => c.TSGuide == Convert.ToInt32(x)).FirstOrDefault();
                //db.TblTSGuide.Remove(dd);
                //db.SaveChanges();

                str = str.Substring(0, str.LastIndexOf(','));
                using (var ctx = new MoldtraxDbContext())
                {
                    SqlParameter sp = new SqlParameter("@value", str);
                    var result = ctx.Database.ExecuteSqlCommand("exec procDeleteTooling @value", sp);
                }

                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.Tooling.ToString(), GetAction.Delete.ToString());

            }

            CommonDropDownVal();
            int CID = ShrdMaster.Instance.GetCompanyID();

            var NewData = db.TblMoldTooling.Where(x => x.MoldDataID == MoldID && x.CompanyID == CID).ToList();
            var ToolingType = db.TblDDMoldToolingTypes.Where(X=> X.CompanyID == CID).ToList();
            int I = 0;

            foreach (var x in NewData)
            {
                if (x.MoldToolingType != "" && x.MoldToolingType != null)
                {
                    I = Convert.ToInt32(x.MoldToolingType);
                }

                var Tool = ToolingType.Where(c => c.ID == I).FirstOrDefault();
                x.MoldToolingTypeName = Tool != null ? Tool.DD_MoldToolingType : "";
            }

            return PartialView("_ToolingGetData", NewData.OrderBy(x => x.MoldToolingTypeName).ThenBy(x => x.MoldToolDescrip).ToList());
        }

        public ActionResult ExportToolingData(HttpPostedFileBase postedFile, int MID = 0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            string filePath = string.Empty;
            if (postedFile != null)
            {
                string path = Server.MapPath("~/DropDownTemplate/");
                System.IO.DirectoryInfo di = new DirectoryInfo(path);

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = path + Path.GetFileName(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);
                postedFile.SaveAs(filePath);

                string connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0 Macro;HDR=YES;IMEX=1;';", filePath);

                DataSet data = new DataSet();

                using (OleDbConnection con = new OleDbConnection(connectionString))
                {
                    con.Open();
                    var dataTable = new DataTable();
                    DataTable dtExcelSchema;
                    dtExcelSchema = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                    //string query = string.Format("SELECT * FROM [{0}]", sheetName);
                    string query = string.Format("SELECT * FROM [{0}]", sheetName);
                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, con);
                    adapter.Fill(dataTable);
                    data.Tables.Add(dataTable);
                    string[] ShetN = sheetName.Split('$');
                    ToolingDataInsert(dataTable, ShetN[0], MID);
                    con.Close();
                }
            }

            CommonDropDownVal();

            var data1 = ShrdMaster.Instance.GetToolingList(MID, CID);

            var ToolingType = db.TblDDMoldToolingTypes.Where(X=> X.CompanyID == CID).ToList();
            int I = 0;

            foreach (var x in data1)
            {
                if (x.MoldToolingType != "" && x.MoldToolingType != null)
                {
                    I = Convert.ToInt32(x.MoldToolingType);
                }

                var Tool = ToolingType.Where(c => c.ID == I).FirstOrDefault();
                x.MoldToolingTypeName = Tool != null ? Tool.DD_MoldToolingType : "";
            }

            var NewData = data1.OrderBy(x => x.MoldToolingTypeName).ThenBy(x => x.MoldToolDescrip).ToList();
            return PartialView("_ToolingGetData", NewData);
        }

        public void ToolingDataInsert(DataTable dt, string sheetName, int MID = 0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            using (SqlConnection connection = new SqlConnection(constring))
            {

                List<tblMoldTooling> SR = new List<tblMoldTooling>();
                var data = db.TblDDMoldCategoryID.Where(X => X.CompanyID == CID).ToList();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i].ItemArray[0] != null && dt.Rows[i].ItemArray[1] != null)
                    {
                        string Desc = dt.Rows[i].ItemArray[1].ToString() == "" ? null : dt.Rows[i].ItemArray[1].ToString();

                        var dd = data.Where(x => x.MoldCategoryID == dt.Rows[i].ItemArray[0].ToString() && x.MoldCategoryIDDesc == Desc).FirstOrDefault();
                        var ToolingTypeData = db.TblDDMoldToolingTypes.ToList();

                        if (dd == null)
                        {
                            tblMoldTooling Tool = new tblMoldTooling();

                            var ST = ToolingTypeData.Where(x => x.DD_MoldToolingType == dt.Rows[i].ItemArray[0].ToString()).FirstOrDefault();

                            Tool.MoldToolingType = ST == null ? "0" : ST.ID.ToString();
                            Tool.MoldDataID = MID;
                            Tool.MoldToolDescrip = dt.Rows[i].ItemArray[1].ToString();
                            Tool.Location = dt.Rows[i].ItemArray[2].ToString();
                            Tool.MoldToolingPartNumber = dt.Rows[i].ItemArray[3].ToString();
                            Tool.CompanyID = ShrdMaster.Instance.GetCompanyID();
                            Tool.MoldToolingPrintNumber = dt.Rows[i].ItemArray[4].ToString();
                            Tool.MoldToolingVendor = dt.Rows[i].ItemArray[5].ToString();
                            if (!(dt.Rows[i].ItemArray[6] is DBNull))
                                Tool.MoldToolCost = Convert.ToDecimal(String.Format("{0:N}", dt.Rows[i].ItemArray[6]));
                            if (!(dt.Rows[i].ItemArray[7] is DBNull))
                                Tool.MoldToolingPartsOnHand = Convert.ToInt32(dt.Rows[i].ItemArray[7]);

                            if (!(dt.Rows[i].ItemArray[8] is DBNull))
                                Tool.MoldToolingReorderLevel = Convert.ToInt32(dt.Rows[i].ItemArray[8]);


                            if (!(dt.Rows[i].ItemArray[9] is DBNull))
                                Tool.MoldToolingNumOrdered = Convert.ToInt32(dt.Rows[i].ItemArray[9]);


                            if (!(dt.Rows[i].ItemArray[10] is DBNull))
                                Tool.MoldToolingDateOrdered = Convert.ToDateTime(dt.Rows[i].ItemArray[10]);


                            if (!(dt.Rows[i].ItemArray[11] is DBNull))
                                Tool.MoldToolingNumReceived = Convert.ToInt32(dt.Rows[i].ItemArray[11]);


                            SR.Add(Tool);
                        }
                    }
                }


                DataTable NewDT = ToDataTable(SR);
                connection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.ColumnMappings.Add("MoldDataID", "MoldDataID");
                    bulkCopy.ColumnMappings.Add("Location", "Location");
                    bulkCopy.ColumnMappings.Add("MoldToolingType", "MoldToolingType");
                    bulkCopy.ColumnMappings.Add("MoldToolDescrip", "MoldToolDescrip");
                    bulkCopy.ColumnMappings.Add("CompanyID", "CompanyID");
                    bulkCopy.ColumnMappings.Add("MoldToolingPartNumber", "MoldToolingPartNumber");
                    bulkCopy.ColumnMappings.Add("MoldToolingPrintNumber", "MoldToolingPrintNumber");
                    bulkCopy.ColumnMappings.Add("MoldToolingVendor", "MoldToolingVendor");
                    bulkCopy.ColumnMappings.Add("MoldToolCost", "MoldToolCost");
                    bulkCopy.ColumnMappings.Add("MoldToolingPartsOnHand", "MoldToolingPartsOnHand");
                    bulkCopy.ColumnMappings.Add("MoldToolingReorderLevel", "MoldToolingReorderLevel");
                    bulkCopy.ColumnMappings.Add("MoldToolingNumOrdered", "MoldToolingNumOrdered");
                    bulkCopy.ColumnMappings.Add("MoldToolingDateOrdered", "MoldToolingDateOrdered");
                    bulkCopy.ColumnMappings.Add("MoldToolingNumReceived", "MoldToolingNumReceived");

                    bulkCopy.DestinationTableName = "tblMoldTooling";
                    try
                    {
                        bulkCopy.WriteToServer(NewDT);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                connection.Close();
            }
        }

        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties by using reflection   
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names  
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {

                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
        #endregion

        #region Tech Tips

        public ActionResult GetTechTips(int ID = 0, int MOLDChange = 0)
        {
            int i = ReturnSelectedMoldID(ID);
            int CID = ShrdMaster.Instance.GetCompanyID();

            var data = db.TblTechTips.Where(x => x.MoldDataID == i && x.CompanyID == CID).FirstOrDefault();

            string letters = string.Empty;
            string numbers = string.Empty;
            if (data != null)
            {
                data.TTPartImagePath = data.TTPartImagePath != null ? Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/SiteImages/TechTipsImages/" + data.TTPartImagePath) : null;
                data.TTMoldImagePath = data.TTMoldImagePath != null ? Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/SiteImages/TechTipsImages/" + data.TTMoldImagePath) : null;
                if (data.MCWeight != null)
                {
                    bool ISMCWeight = data.MCWeight.All(char.IsDigit);

                    if (ISMCWeight == true)
                    {
                        foreach (char c in data.MCWeight)
                        {
                            if (Char.IsLetter(c))
                            {
                                letters += c;
                            }
                            if (Char.IsNumber(c))
                            {
                                numbers += c;
                            }
                        }
                        data.MCWeight = String.Format("{0:n0}", Convert.ToInt32(numbers)) + " " + letters;
                    }
                }
            }
            //return PartialView("_GetTechTips", data == null ? new tblTechTips() : data);

            //data.TTPartImagePath = data.TTPartImagePath != null ? Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/SiteImages/TechTipsImages/" + data.TTPartImagePath) : null;
            //data.TTMoldImagePath = data.TTMoldImagePath != null ? Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/SiteImages/TechTipsImages/" + data.TTMoldImagePath) : null;

            ViewBag.MoldDataID = i;

            var OInfo = data == null ? new tblTechTips() : data;

            if (MOLDChange == 0)
            {
                return PartialView("_GetTechTips", OInfo);
            }

            else
            {
                CommonDrop();
                int NoofRec = 0;

                NoofRec = ShrdMaster.Instance.GetMaintenanceTblRoverSetData(i).OrderBy(x => x.SetDate).Count();
                ViewBag.TotalMold = NoofRec;
                List<TblRoverSetDataViewModel> MD = new List<TblRoverSetDataViewModel>();

                var MoldData = RenderRazorViewToString(this.ControllerContext, "_MoldData", MD);
                var OtherInfo = RenderRazorViewToString(this.ControllerContext, "_GetTechTips", OInfo);
                return Json(new { MoldData, OtherInfo }, JsonRequestBehavior.AllowGet);
                //return PartialView("_GetTechTips", new {MoldData, OtherInfo });
            }
            //HttpCookie nameCookie = new HttpCookie("SelectedMoldID");
            //nameCookie.Values["SelectedMoldID"] = i.ToString();
            //Response.Cookies.Add(nameCookie);

            //return View(data);
        }

        public void CommonTTLinkFunc()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            var data = db.TblDDDocSections.Where(X=> X.CompanyID == CID).ToList();
            List<SelectListItem> Category = new List<SelectListItem>();
            foreach (var x in data)
            {
                Category.Add(new SelectListItem
                {
                    Text = x.DocSection,
                    Value = x.ID.ToString()
                });
            }

            ViewBag.CategoryVal = Category.OrderBy(X=> X.Text).ToList();
        }

        public ActionResult TechTipsMoldSpec(int ID = 0)
        {
            int i = ReturnSelectedMoldID(ID);
            var data = db.TblTechTips.Where(x => x.MoldDataID == i).FirstOrDefault();
            return PartialView("_TechTipsMoldSpecData", data);
        }

        [HttpPost]
        public ActionResult SaveTechTipsMoldSpec(tblTechTips model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            if (model.TechTipsID == 0)
            {
                model.CompanyID = CID;
                db.TblTechTips.Add(model);
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.TechTips.ToString(), GetAction.Create.ToString());

            }

            else
            {
                var data = db.TblTechTips.Where(x => x.TechTipsID == model.TechTipsID).FirstOrDefault();
                data.MCHeight = model.MCHeight;
                data.MCWidth = model.MCWidth;
                data.MCDepth = model.MCDepth;
                data.MCWeight = model.MCWeight;
                data.MCWidthOpen = model.MCWidthOpen;
                data.MCEjectorStroke = model.MCEjectorStroke;
                data.MCTotalHeight = model.MCTotalHeight;

                data.TTHRSystem = model.TTHRSystem;
                data.TTHRSerialNumber = model.TTHRSerialNumber;
                data.TTHRProgramNumber = model.TTHRProgramNumber;
                data.TTHRType = model.TTHRType;
                data.TTHRActuation = model.TTHRActuation;
                data.TTHRProbeType = model.TTHRProbeType;
                data.TTHRController = model.TTHRController;
                data.TTHRNumberZones = model.TTHRNumberZones;
                data.TTHRNumberDrops = model.TTHRNumberDrops;
                data.TTHROpenPressureMax = model.TTHROpenPressureMax;
                data.TTHROpenPressureTypical = model.TTHROpenPressureTypical;
                data.TTHRClosePressureMax = model.TTHRClosePressureMax;
                data.TTHRClosePressureTypical = model.TTHRClosePressureTypical;
                data.TTHRProbeHeater = model.TTHRProbeHeater;
                data.TTHRProbeHeaterThermoType = model.TTHRProbeHeaterThermoType;
                data.TTHRManifoldHeater = model.TTHRManifoldHeater;
                data.TTHRManifoldHeaterThermoType = model.TTHRManifoldHeaterThermoType;
                data.BridgeHeater = model.BridgeHeater;

                data.BridgeThermocouple = model.BridgeThermocouple;
                data.SprueHeater = model.SprueHeater;
                data.SprueThermocouple = model.SprueThermocouple;
                data.TTHRMaxOperatTemp = model.TTHRMaxOperatTemp;
                data.TTHRClampPlateBoltTorque = model.TTHRClampPlateBoltTorque;
                data.TTHRDisassembly = model.TTHRDisassembly;
                data.TTHRClean = model.TTHRClean;
                data.TTHRAssembly = model.TTHRAssembly;
                data.TTHRFinalChk = model.TTHRFinalChk;
                data.TTHRPolishing = model.TTHRPolishing;
                data.TTHRToolKit = model.TTHRToolKit;

                data.TTDisassmbly = model.TTDisassmbly;

                data.TTClean = model.TTClean;
                data.TTAssmbly = model.TTAssmbly;
                data.TTFinalChk = model.TTFinalChk;
                data.TTToolKit = model.TTToolKit;
                data.TTPolishing = model.TTPolishing;
                data.TTHRNotes = model.TTHRNotes;
                data.TTHRDropsServicableInPress = model.TTHRDropsServicableInPress;

                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.TechTips.ToString(), GetAction.Create.ToString());

            }

            return Json("", JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Index");
        }

        //public void DeleteFile(string Path)
        //{
        //    System.IO.File.Delete(Path);
        //}

        public ActionResult UplaodTTPartImage(int TTID = 0, int MoldDataID = 0)
        {
            string fname = "";
            string fname1 = "";
            string IMGName = "";
            string UniqueName = ShrdMaster.Instance.ReturnUniqueName();


            try
            {
                tblTechTips tt = new tblTechTips();
                //  Get all files from Request object 
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                    //string filename = Path.GetFileName(Request.Files[i].FileName);  

                    HttpPostedFileBase file = files[i];

                    //var dd = db.TblTechTips.Where(x => x.TSGuide == TsGuide).FirstOrDefault();

                    // Checking for Internet Explorer  
                    if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                    {
                        string[] testfiles = file.FileName.Split(new char[] { '\\' });
                        fname = testfiles[testfiles.Length - 1];
                    }
                    else
                    {
                        fname = "PartImage_" + UniqueName + "_" + file.FileName;
                        fname1 = "PartImage_" + UniqueName + "_" + file.FileName;
                        IMGName = "PartImage_" + UniqueName + "_" + file.FileName;
                    }

                    var FileExtension = Path.GetExtension(fname);
                    fname = Path.Combine(Server.MapPath("~/SiteImages/TechTipsImages/"), fname);
                    file.SaveAs(fname);

                    if (FileExtension == ".vsdx" || FileExtension == ".vsd")
                    {
                        var FileP = ShrdMaster.Instance.ConvertVisioToImg(fname);
                        fname1 = FileP;
                    }
                    else
                    {
                        fname1 = IMGName;
                    }
                }

                int CID = ShrdMaster.Instance.GetCompanyID();



                if (TTID == 0)
                {
                    tt.TTPartImagePath = fname1;
                    tt.MoldDataID = MoldDataID;
                    tt.CompanyID = CID;
                    db.TblTechTips.Add(tt);
                    db.SaveChanges();
                    tt.TTPartImagePath = Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/SiteImages/TechTipsImages/" + fname1);
                    return Json(tt, JsonRequestBehavior.AllowGet);
                }

                else
                {
                    var data = db.TblTechTips.Where(x => x.TechTipsID == TTID).FirstOrDefault();
                    try
                    {
                        if (data.TTPartImagePath != null)
                        {
                            string path = Path.Combine(Server.MapPath("~/SiteImages/TechTipsImages/"), data.TTPartImagePath);
                            DeleteFile(path);
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                    data.TTPartImagePath = fname1;
                    db.SaveChanges();
                    data.TTPartImagePath = Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/SiteImages/TechTipsImages/" + fname1);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }

                // Returns message that successfully uploaded  
            }
            catch (Exception ex)
            {
                return Json("Error occurred. Error details: " + ex.Message);
            }
        }



        public ActionResult UplaodTTMoldImage(int TTID = 0, int MoldDataID = 0)
        {
            string fname = "";
            string fname1 = "";
            string ImgName = "";
            string UniqueName = ShrdMaster.Instance.ReturnUniqueName();
            try
            {

                //  Get all files from Request object 
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/";  
                    //string filename = Path.GetFileName(Request.Files[i].FileName);  

                    HttpPostedFileBase file = files[i];

                    //var dd = db.TblTechTips.Where(x => x.TSGuide == TsGuide).FirstOrDefault();

                    // Checking for Internet Explorer  
                    if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                    {
                        string[] testfiles = file.FileName.Split(new char[] { '\\' });
                        fname = testfiles[testfiles.Length - 1];
                    }
                    else
                    {
                        fname = "MoldImage_" + UniqueName + "_" + file.FileName;
                        fname1 = "MoldImage_" + UniqueName + "_" + file.FileName;
                        ImgName = "MoldImage_" + UniqueName + "_" + file.FileName;
                    }

                    var FileExtension = Path.GetExtension(fname);

                    //dd.ImagePath = fname; E:\MoldtraxAsp\Moldtrax\SiteImages\TTPartImages\
                    //db.SaveChanges();
                    // Get the complete folder path and store the file inside it.  
                    fname = Path.Combine(Server.MapPath("~/SiteImages/TechTipsImages/"), fname);
                    file.SaveAs(fname);

                    if (FileExtension == ".vsdx" || FileExtension == ".vsd")
                    {
                        var FileP = ShrdMaster.Instance.ConvertVisioToImg(fname);
                        fname1 = FileP;
                    }
                    else
                    {
                        fname1 = ImgName;
                    }
                }

                int CID = ShrdMaster.Instance.GetCompanyID();

                if (TTID == 0)
                {
                    tblTechTips tt = new tblTechTips();
                    tt.TTMoldImagePath = fname1;
                    tt.MoldDataID = MoldDataID;
                    tt.CompanyID = CID;
                    db.TblTechTips.Add(tt);
                    db.SaveChanges();
                    tt.TTMoldImagePath = Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/SiteImages/TechTipsImages/" + fname1);
                    return Json(tt.TechTipsID, JsonRequestBehavior.AllowGet);
                }

                else
                {
                    var data = db.TblTechTips.Where(x => x.TechTipsID == TTID).FirstOrDefault();

                    if (data.TTMoldImagePath != null)
                    {
                        string path = Path.Combine(Server.MapPath("~/SiteImages/TechTipsImages/"), data.TTMoldImagePath);
                        DeleteFile(path);
                    }

                    data.TTMoldImagePath = fname1;
                    db.SaveChanges();
                    data.TTMoldImagePath = Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/SiteImages/TechTipsImages/" + fname1);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }

                // Returns message that successfully uploaded  
            }
            catch (Exception ex)
            {
                return Json("Error occurred. Error details: " + ex.Message);
            }
        }

        public ActionResult DeleteTTPartImage(int TTID = 0)
        {
            if (TTID != 0)
            {
                var data = db.TblTechTips.Where(x => x.TechTipsID == TTID).FirstOrDefault();

                if (data.TTPartImagePath != null)
                {
                    try
                    {
                        string path = Path.Combine(Server.MapPath("~/SiteImages/TechTipsImages/"), data.TTPartImagePath);
                        DeleteFile(path);

                    }
                    catch (Exception ex)
                    {

                    }
                }

                data.TTPartImagePath = null;
                db.SaveChanges();
            }

            return Json("ok");

        }

        public ActionResult DeleteTTMoldImage(int TTID = 0)
        {
            if (TTID != 0)
            {
                var data = db.TblTechTips.Where(x => x.TechTipsID == TTID).FirstOrDefault();

                if (data.TTMoldImagePath != null)
                {
                    string path = Path.Combine(Server.MapPath("~/SiteImages/TechTipsImages/"), data.TTMoldImagePath);
                    DeleteFile(path);
                }
                data.TTMoldImagePath = null;
                db.SaveChanges();

                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.TechTips.ToString(), GetAction.Update.ToString());

            }

            return Json("ok");

        }

        public ActionResult TechTipsHotRunner(int ID = 0)
        {
            int i = ReturnSelectedMoldID(ID);
            var data = db.TblTechTips.Where(x => x.MoldDataID == i).FirstOrDefault();
            return PartialView("_TTHotRunner", data);
        }

        [HttpPost]
        public ActionResult SaveHotRunner(tblTechTips model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            if (model.TechTipsID == 0)
            {
                model.CompanyID = CID;
                db.TblTechTips.Add(model);
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.TechTips.ToString(), GetAction.Create.ToString());

            }

            else
            {
                var data = db.TblTechTips.Where(x => x.TechTipsID == model.TechTipsID).FirstOrDefault();
                data.TTHRSystem = model.TTHRSystem;
                data.TTHRSerialNumber = model.TTHRSerialNumber;
                data.TTHRProgramNumber = model.TTHRProgramNumber;
                data.CompanyID = CID;
                data.TTHRType = model.TTHRType;
                data.TTHRActuation = model.TTHRActuation;
                data.TTHRProbeType = model.TTHRProbeType;
                data.TTHRController = model.TTHRController;
                data.TTHRNumberZones = model.TTHRNumberZones;
                data.TTHRNumberDrops = model.TTHRNumberDrops;
                data.TTHROpenPressureMax = model.TTHROpenPressureMax;
                data.TTHROpenPressureTypical = model.TTHROpenPressureTypical;
                data.TTHRClosePressureMax = model.TTHRClosePressureMax;
                data.TTHRClosePressureTypical = model.TTHRClosePressureTypical;
                data.TTHRProbeHeater = model.TTHRProbeHeater;
                data.TTHRProbeHeaterThermoType = model.TTHRProbeHeaterThermoType;
                data.TTHRManifoldHeater = model.TTHRManifoldHeater;
                data.TTHRManifoldHeaterThermoType = model.TTHRManifoldHeaterThermoType;
                data.BridgeHeater = model.BridgeHeater;
                data.BridgeThermocouple = model.BridgeThermocouple;
                data.SprueHeater = model.SprueHeater;
                data.SprueThermocouple = model.SprueThermocouple;
                data.TTHRMaxOperatTemp = model.TTHRMaxOperatTemp;
                data.TTHRClampPlateBoltTorque = model.TTHRClampPlateBoltTorque;
                data.TTHRDisassembly = model.TTHRDisassembly;
                data.TTHRClean = model.TTHRClean;
                data.TTHRAssembly = model.TTHRAssembly;
                data.TTHRFinalChk = model.TTHRFinalChk;
                data.TTHRPolishing = model.TTHRPolishing;
                data.TTHRToolKit = model.TTHRToolKit;
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.TechTips.ToString(), GetAction.Update.ToString());

            }

            return RedirectToAction("Index");
        }


        public List<tblDocs> GetTTLinksList(int MoldID)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            var data = db.TblDocs.Where(X => X.DocMoldID == MoldID && X.CompanyID == CID).ToList();
            List<tblDocs> String = new List<tblDocs>();
            List<tblDocs> Num = new List<tblDocs>();

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

            String = String.OrderBy(x => x.DocSection).ThenBy(x=> x.DocName).ToList();
            Num = Num.OrderBy(x => x.DocSection).ThenBy(x => x.DocName).ToList();
            String.AddRange(Num);
            return String;
        }



        public ActionResult TTLinksGetData(int ID = 0)
        {

            CommonTTLinkFunc();

            int i = ReturnSelectedMoldID(ID);
            ViewBag.MoldDataID = i;

            var data = GetTTLinksList(i);
            //var data = db.TblDocs.Where(x => x.DocMoldID == i).OrderBy(x => x.DocSection).ThenBy(x => x.DocName).ToList();
            return PartialView("_TTLinks", data);
        }

        public void SaveTTLinksAutoFocus(tblDocs model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            var data = db.TblDocs.Where(s => s.DocID == model.DocID).FirstOrDefault();
            if (data != null)
            {
                data.DocSection = model.DocSection;
                data.DocName = model.DocName;
                data.DocLink = model.DocLink;
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.TechTips.ToString(), GetAction.Update.ToString());

            }
        }

        [HttpPost]
        public ActionResult SaveTTLinks(tblDocs model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            if (model.DocID == 0)
            {
                model.CompanyID = CID;
                db.TblDocs.Add(model);
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.TechTips.ToString(), GetAction.Create.ToString());

            }
            else
            {
                var data = db.TblDocs.Where(s => s.DocID == model.DocID).FirstOrDefault();
                data.DocSection = model.DocSection;
                data.DocName = model.DocName;
                data.DocLink = model.DocLink;
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.TechTips.ToString(), GetAction.Update.ToString());
            }

            int i = ShrdMaster.Instance.ReturnSelectedMoldID(model.DocMoldID);
            GetMolDataID(i);
            var data12 = GetTTLinksList(i);
            //var data12 = db.TblDocs.Where(x => x.DocMoldID == model.DocMoldID).OrderBy(x => x.DocSection).ThenBy(x => x.DocName).ToList();

            CommonTTLinkFunc();
            return PartialView("_TTLinks", data12);
            //return Json("ok", JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public ActionResult SaveTTLinks(List<tblDocs> model)
        //{
        //    foreach (var x in model)
        //    {
        //        if (x.DocID == 0)
        //        {
        //            db.TblDocs.Add(x);
        //            db.SaveChanges();
        //        }
        //        else
        //        {
        //            var data = db.TblDocs.Where(s => s.DocID == x.DocID).FirstOrDefault();
        //            data.DocSection = x.DocSection;
        //            data.DocName = x.DocName;
        //            data.DocLink = x.DocLink;
        //            db.SaveChanges();
        //        }
        //    }

        //    int i = ReturnSelectedMoldID(model.Select(x => x.DocMoldID).FirstOrDefault());
        //    GetMolDataID(i);
        //    ViewBag.CategoryVal = new SelectList(db.TblDDDocSections.ToList(), "DocSection", "DocSection");
        //    return PartialView("_TTLinks", model);

        //}

        public void GetMolDataID(int i = 0)
        {
            ViewBag.MoldDataID = i;
        }

        public ActionResult DeleteTTLinks(string str, int MoldID = 0)
        {
            if (str != "")
            {
                str = str.Substring(0, str.LastIndexOf(','));
                using (var ctx = new MoldtraxDbContext())
                {
                    SqlParameter sp = new SqlParameter("@value", str);
                    var result = ctx.Database.ExecuteSqlCommand("exec procDeletetTTLinks @value", sp);
                }
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.TechTips.ToString(), GetAction.Delete.ToString());

            }


            CommonTTLinkFunc();

            int i = ReturnSelectedMoldID(MoldID);
            GetMolDataID(i);

            var data = GetTTLinksList(i);
            return PartialView("_TTLinks", data);
        }

        #endregion

        #region Layout

        public ActionResult GetLayoutData(int ID = 0, int MOLDChange = 0)
        {
            int i = ReturnSelectedMoldID(ID);
            int CID = ShrdMaster.Instance.GetCompanyID();


            var tblLocation = db.TblCavityLocations.Where(x => x.MoldDataID == i && x.CompanyID == CID).OrderBy(x=> x.CavityLocationNumber.Length).ThenBy(x=> x.CavityLocationNumber).ToList();

            ViewBag.MoldDataID = i;
            int u = 0;
            if (tblLocation != null)
            {
                u = tblLocation.Select(x => x.CavityLocationID).FirstOrDefault();
            }

            var tblNumber = db.TblCavityNumbers.Where(x => x.CavityLocationID == u && x.CompanyID == CID).OrderByDescending(x=> x.CavityActive).ThenBy(x=> x.CavityNumber).ToList();

            LayoutData OInfo = new LayoutData();
            OInfo.tblCavityLocations = tblLocation;
            OInfo.tblCavityNumber = tblNumber;
            if (MOLDChange == 0)
            {
                return PartialView("_MoldLayout", OInfo);
            }

            else
            {
                CommonDrop();
                int NoofRec = 0;
                NoofRec = ShrdMaster.Instance.GetMaintenanceTblRoverSetData(i).OrderBy(x => x.SetDate).Count();
                ViewBag.TotalMold = NoofRec;
                List<TblRoverSetDataViewModel> MD = new List<TblRoverSetDataViewModel>();

                var MoldData = RenderRazorViewToString(this.ControllerContext, "_MoldData", MD);
                var OtherInfo = RenderRazorViewToString(this.ControllerContext, "_MoldLayout", OInfo);
                return Json(new { MoldData, OtherInfo }, JsonRequestBehavior.AllowGet);
                //return PartialView("_MoldLayout", new {MoldData, OtherInfo });
            }
        }

        public ActionResult CreateNewCavityLocation(int MoldID = 0, string CavityNum = "0")
        {
            tblCavityNumber tbl = new tblCavityNumber();
            int CID = ShrdMaster.Instance.GetCompanyID();

            if (CavityNum != "0")
            {
                tblCavityLocation dd = new tblCavityLocation();
                dd.CavityLocationNumber = CavityNum;
                dd.MoldDataID = MoldID;
                dd.CompanyID = CID;
                db.TblCavityLocations.Add(dd);
                db.SaveChanges();
                tbl.CavityLocationID = dd.CavityLocationID;
                tbl.CavityNumber = CavityNum;
            }

            if (tbl.CavityNumberID == 0)
            {
                tbl.CompanyID = CID;
                db.TblCavityNumbers.Add(tbl);
                db.SaveChanges();
            }
            //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.CavityLayout.ToString(), GetAction.Create.ToString());


            var tblLocation = ReturnCavityLocation(MoldID);
            //var tblLocation = db.TblCavityLocations.Where(x => x.MoldDataID == MoldID).OrderBy(x => x.CavityLocationNumber.Length).ThenBy(x => x.CavityLocationNumber).ToList();

            int u = 0;
            if (tblLocation != null)
            {
                u = tblLocation.Select(x => x.CavityLocationID).FirstOrDefault();
            }

            var tblNumber = db.TblCavityNumbers.Where(x => x.CavityLocationID == u && x.CompanyID == CID).OrderByDescending(x => x.CavityActive).ThenBy(x => x.CavityNumber).ToList();
            ViewBag.MoldDataID = MoldID;
            LayoutData LD = new LayoutData();
            LD.tblCavityLocations = tblLocation;
            LD.tblCavityNumber = tblNumber;
            return PartialView("_MoldLayout", LD);
        }


        public ActionResult DeleteCavityNumber(string str)
        {
            if (str != "")
            {
                str = str.Substring(0, str.LastIndexOf(','));
                using (var ctx = new MoldtraxDbContext())
                {
                    SqlParameter sp = new SqlParameter("@value", str);
                    var result = ctx.Database.ExecuteSqlCommand("exec sp_deleteCavityNumbers @value", sp);
                }
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }


        public List<tblCavityLocation> ReturnCavityLocation(int MoldID)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            var data = db.TblCavityLocations.Where(X => X.MoldDataID == MoldID && X.CompanyID == CID).ToList();
            List<tblCavityLocation> String = new List<tblCavityLocation>();
            List<tblCavityLocation> Num = new List<tblCavityLocation>();

            foreach (var x in data)
            {
                int n;
                bool isNumeric = int.TryParse(x.CavityLocationNumber, out n);

                if (!string.IsNullOrEmpty(x.CavityLocationNumber) && isNumeric)
                {
                    Num.Add(x);
                }
                else
                {
                    String.Add(x);
                }
            }

            String = String.OrderBy(x => x.CavityLocationNumber).ToList();
            Num = Num.OrderBy(x => x.CavityLocationNumber.Length).ThenBy(x => x.CavityLocationNumber).ToList();

            //int maxlen = Num.Max(x => x.CavityLocationNumber.Length);
            //Num.OrderBy(x => x.CavityLocationNumber.PadLeft(maxlen, '0'));

            Num.AddRange(String);
            return Num;
        }

        public ActionResult GetCavityNumberList(int CavityID = 0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            var data = db.TblCavityNumbers.Where(x => x.CavityLocationID == CavityID && x.CompanyID == CID).OrderByDescending(x=> x.CavityActive).ThenBy(x=> x.CavityNumber).ToList();
            return PartialView("_tblCavityNumberData", data);
        }

        public ActionResult UpdateCavityLocation(int MoldID = 0, int CavityNo = 0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            var tblC = db.TblCavityLocations.Where(x => x.MoldDataID == MoldID && x.CompanyID == CID).ToList();
            var no = tblC.Count() - CavityNo;
            if (no < 0)
            {
                if (tblC.Count() != 0)
                {
                    var asd = tblC.LastOrDefault();
                    string LastNo = asd.CavityLocationNumber;

                    for (int i = Convert.ToInt32(LastNo); i <= CavityNo; i++)
                    {
                        tblCavityLocation tbl = new tblCavityLocation();
                        tbl.MoldDataID = MoldID;
                        tbl.CompanyID = CID;
                        tbl.CavityLocationNumber = i.ToString();
                        db.TblCavityLocations.Add(tbl);
                        db.SaveChanges();

                        tblCavityNumber TBNum = new tblCavityNumber();
                        TBNum.CavityLocationID = tbl.CavityLocationID;
                        TBNum.CavityActive = true;
                        TBNum.CompanyID = CID;
                        TBNum.CavityNumber = i.ToString();
                        db.TblCavityNumbers.Add(TBNum);
                        db.SaveChanges();
                    }
                }

                else
                {
                    for (int i = 1; i <= CavityNo; i++)
                    {
                        tblCavityLocation tbl = new tblCavityLocation();
                        tbl.MoldDataID = MoldID;
                        tbl.CompanyID = CID;
                        tbl.CavityLocationNumber = i.ToString();
                        db.TblCavityLocations.Add(tbl);
                        db.SaveChanges();

                        tblCavityNumber TBNum = new tblCavityNumber();
                        TBNum.CavityLocationID = tbl.CavityLocationID;
                        TBNum.CavityActive = true;
                        TBNum.CompanyID = CID;
                        TBNum.CavityNumber = i.ToString();
                        db.TblCavityNumbers.Add(TBNum);
                        db.SaveChanges();
                    }
                }
            }

            //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.CavityLayout.ToString(), GetAction.Create.ToString());

            var tblLocation = db.TblCavityLocations.Where(x => x.MoldDataID == MoldID && x.CompanyID == CID).OrderBy(x=> x.CavityLocationNumber.Length).ThenBy(x=> x.CavityLocationNumber).ToList();

            int u = 0;
            if (tblLocation != null)
            {
                u = tblLocation.Select(x => x.CavityLocationID).FirstOrDefault();
            }

            var tblNumber = db.TblCavityNumbers.Where(x => x.CavityLocationID == u && x.CompanyID == CID).OrderByDescending(x=> x.CavityActive).ThenBy(x=> x.CavityNumber).ToList();

            LayoutData LD = new LayoutData();
            LD.tblCavityLocations = tblLocation;
            LD.tblCavityNumber = tblNumber;

            return PartialView("_MoldLayout", LD);
        }

        public ActionResult CreateMoldPosition(string Data, int MoldID = 0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            tblCavityLocation dd = new tblCavityLocation();
            dd.CavityLocationNumber = Data;
            dd.CompanyID = CID;
            dd.MoldDataID = MoldID;
            db.TblCavityLocations.Add(dd);
            db.SaveChanges();

            tblCavityNumber td = new tblCavityNumber();
            td.CavityLocationID = dd.CavityLocationID;
            td.CompanyID = CID;
            db.TblCavityNumbers.Add(td);
            db.SaveChanges();
            //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.CavityLayout.ToString(), GetAction.Create.ToString());

            var tblLocation = db.TblCavityLocations.Where(x => x.MoldDataID == MoldID && x.CompanyID == CID).OrderBy(x=> x.CavityLocationNumber.Length).ThenBy(x=> x.CavityLocationNumber).ToList();

            int u = 0;
            if (tblLocation != null)
            {
                u = tblLocation.Select(x => x.CavityLocationID).FirstOrDefault();
            }

            var tblNumber = db.TblCavityNumbers.Where(x => x.CavityLocationID == u && x.CompanyID == CID).OrderByDescending(x=> x.CavityActive).ThenBy(x=> x.CavityNumber).ToList();
            ViewBag.MoldDataID = MoldID;
            LayoutData LD = new LayoutData();
            LD.tblCavityLocations = tblLocation;
            LD.tblCavityNumber = tblNumber;

            return PartialView("_MoldLayout", LD);
        }

        public ActionResult DeleteSelectedMoldPosition(int ID = 0, int MoldID = 0)
        {
            var data = db.TblCavityLocations.Where(x => x.CavityLocationID == ID).FirstOrDefault();
            db.TblCavityLocations.Remove(data);
            db.SaveChanges();

            int CID = ShrdMaster.Instance.GetCompanyID();

            var data1 = db.TblCavityNumbers.Where(x => x.CavityLocationID == ID && x.CompanyID == CID).ToList();
            foreach (var x in data1)
            {

                db.TblCavityNumbers.Remove(x);
                db.SaveChanges();
            }

            //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.CavityLayout.ToString(), GetAction.Delete.ToString());


            var tblLocation = db.TblCavityLocations.Where(x => x.MoldDataID == MoldID && x.CompanyID == CID).OrderBy(x=> x.CavityLocationNumber.Length).ThenBy(x=> x.CavityLocationNumber).ToList();
            int u = 0;
            if (tblLocation != null)
            {
                u = tblLocation.Select(x => x.CavityLocationID).FirstOrDefault();
            }
            ViewBag.MoldDataID = MoldID;

            var tblNumber = db.TblCavityNumbers.Where(x => x.CavityLocationID == u && x.CompanyID == CID).OrderByDescending(x=> x.CavityActive).ThenBy(x=> x.CavityNumber).ToList();

            LayoutData LD = new LayoutData();
            LD.tblCavityLocations = tblLocation;
            LD.tblCavityNumber = tblNumber;

            return PartialView("_MoldLayout", LD);
        }



        public ActionResult DeleteAllMoldPosition(int MoldID = 0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblCavityLocations.Where(x => x.MoldDataID == MoldID && x.CompanyID == CID).ToList();
            var tblloc = db.TblCavityNumbers.Where(X=> X.CompanyID == CID).ToList();
            foreach (var x in data)
            {
                var ds = tblloc.Where(s => s.CavityLocationID == x.CavityLocationID).ToList();
                foreach (var y in ds)
                {
                    db.TblCavityNumbers.Remove(y);
                    db.SaveChanges();
                }

                db.TblCavityLocations.Remove(x);
                db.SaveChanges();
            }

            //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.CavityLayout.ToString(), GetAction.Delete.ToString());

            var tblLocation = db.TblCavityLocations.Where(x => x.MoldDataID == MoldID && x.CompanyID == CID).OrderBy(x=> x.CavityLocationNumber.Length).ThenBy(x=> x.CavityLocationNumber).ToList();
            int u = 0;
            if (tblLocation != null)
            {
                u = tblLocation.Select(x => x.CavityLocationID).FirstOrDefault();
            }

            var tblNumber = db.TblCavityNumbers.Where(x => x.CavityLocationID == u && x.CompanyID == CID).OrderByDescending(x=> x.CavityActive).ThenBy(x=> x.CavityNumber).ToList();
            ViewBag.MoldDataID = MoldID;
            LayoutData LD = new LayoutData();
            LD.tblCavityLocations = tblLocation;
            LD.tblCavityNumber = tblNumber;

            return PartialView("_MoldLayout", LD);
        }


        public void EditCavityNumFocusOut(tblCavityNumber model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
            //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.CavityLayout.ToString(), GetAction.Update.ToString());

        }


        public ActionResult EditCavityNum(tblCavityNumber model, int MoldID = 0, string CavityNum = "0")
        {
            //if (model.CavityLocationID == 0)
            //{
            //    tblCavityLocation dd = new tblCavityLocation();
            //    dd.CavityLocationNumber = CavityNum;
            //    dd.MoldDataID = MoldID;
            //    db.TblCavityLocations.Add(dd);
            //    db.SaveChanges();
            //    model.CavityLocationID = dd.CavityLocationID;
            //}
            int CID = ShrdMaster.Instance.GetCompanyID();

            if (model.CavityNumberID == 0)
            {
                model.CompanyID = CID;
                db.TblCavityNumbers.Add(model);
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MaintenanceTracking.ToString(), GetTabName.CavityLayout.ToString(), GetAction.Create.ToString());

            }
            //else
            //{
            //    db.Entry(model).State = EntityState.Modified;
            //    db.SaveChanges();
            //}
            var tblLocation = db.TblCavityLocations.Where(x => x.MoldDataID == MoldID && x.CompanyID == CID).OrderBy(x => x.CavityLocationNumber.Length).ThenBy(x => x.CavityLocationNumber).ToList();

            int u = 0;
            if (tblLocation != null)
            {
                u = tblLocation.Select(x => x.CavityLocationID).FirstOrDefault();
            }

            var tblNumber = db.TblCavityNumbers.Where(x => x.CavityLocationID == u && x.CompanyID == CID).OrderByDescending(x => x.CavityActive).ThenBy(x => x.CavityNumber).ToList();
            ViewBag.MoldDataID = MoldID;
            LayoutData LD = new LayoutData();
            LD.tblCavityLocations = tblLocation;
            LD.tblCavityNumber = tblNumber;
            return PartialView("_MoldLayout", LD);

            //var data = db.TblCavityNumbers.Where(x => x.CavityLocationID == model.CavityLocationID).ToList();

            //return Json("ok", JsonRequestBehavior.AllowGet);
        }
        //[HttpPost]
        //public ActionResult EditCavityNum(List<tblCavityNumber> model)
        //{
        //    foreach (var x in model)
        //    {
        //        if (x.CavityNumberID == 0)
        //        {
        //            db.TblCavityNumbers.Add(x);
        //            db.SaveChanges();
        //        }
        //        else
        //        {
        //            db.Entry(x).State = EntityState.Modified;
        //            db.SaveChanges();
        //        }
        //    }

        //    return Json("ok", JsonRequestBehavior.AllowGet);
        //}
        #endregion

        public double? CalculateCycleCounter(int MoldID=0)
        {
            var data = ShrdMaster.Instance.GetMaintenanceTblRoverSetData(MoldID).OrderBy(x => x.SetDate).ToList();

            double? CycleCounter=0;
            foreach (var x in data)
            {
                CycleCounter += x.CycleCounter;
            }

            return CycleCounter;
        }

    }
}