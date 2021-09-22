using Moldtrax.Filters;
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
using System.Web.Routing;

namespace Moldtrax.Controllers
{
    [SessionExpireFilter]
    [CustomAuthorize]
    public class DetailMoldInfoController : Controller
    {
        private MoldtraxDbContext db = new MoldtraxDbContext();
        static string constring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        SqlConnection con = new SqlConnection(constring);
        // GET: DetailMoldInfo
        
        public ActionResult Index(int CID =0)
        {

            //ApplicationUserManager userManager = HttpContext.GetOwinContext().GetUserManager();

            var  userId = HttpContext.User.Identity.IsAuthenticated;

            if (CID == 0)
            {
                CID = ShrdMaster.Instance.GetCompanyID();
            }

            string Name = "";
            int i = 0;
            var ds = db.TblMoldData.Where(x=> x.CompanyID == CID).FirstOrDefault();

            //UpdateLog(tblMoldData);

            if (ds != null)
            {
                i = ds.MoldDataID;
            }

            HttpCookie nameCookie = new HttpCookie("SelectedMoldID");
            nameCookie.Values["SelectedMoldID"] = i.ToString();
            Response.Cookies.Add(nameCookie);

            if (Session["User"] != null)
            {
                Name = Session["User"].ToString();
            }

            var User = db.Ezy_Users.Where(x => x.UserID == Name).FirstOrDefault();

            ViewBag.CreatedDate = User.DateTimeStamp;

            var dd = ShrdMaster.Instance.CallGetAccess(Name, "sfrmMoldData");

            ViewBag.Companylist = new SelectList(db.TblCompanies.ToList().OrderBy(x => x.CompanyName), "CompanyID", "CompanyName");
            return View();
        }
        
        public JsonResult GetTraciRiskFactor(int MID = 0)
        {
            string Date = System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            var data = ShrdMaster.Instance.GetTraciRiskFactor(con, Date, MID);

            string TarciRiskFactor = "";

            if (data == null)
            {
                TarciRiskFactor = "0.00";
            }
            else
            {
                TarciRiskFactor = String.Format("{0:n2}", data.RiskFactor);
            }
            //string TarciRiskFactor = String.Format("{0:n2}", data.RiskFactor);

            return Json(TarciRiskFactor, JsonRequestBehavior.AllowGet);
        }

        public void GetMolDataID(int i = 0)
        {
            ViewBag.MoldDataID = i;
        }

        public ActionResult MoldGetData()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblMoldData.Where(x=> x.CompanyID == CID).ToList().OrderBy(x => x.MoldName);
            return PartialView("_MoldData", data);
        }

        public ActionResult MoldDetails(int ID = 0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            int i = ShrdMaster.Instance.ReturnSelectedMoldID(ID, CID);

            HttpCookie nameCookie = new HttpCookie("SelectedMoldID");
            nameCookie.Values["SelectedMoldID"] = i.ToString();
            Response.Cookies.Add(nameCookie);

            CommonDropDownVal();
            var data = db.TblMoldData.Where(x => x.MoldDataID == i).FirstOrDefault();

            var NewData = (data == null) ? new tblMoldData() : data;
            return PartialView("_MoldDetails", NewData);
        }

        public ActionResult CreateCookieMoldDataID(int ID, string FuncName)
        {

            int i = ShrdMaster.Instance.ReturnSelectedMoldID(ID);

            HttpCookie nameCookie = new HttpCookie("SelectedMoldID");
            nameCookie.Values["SelectedMoldID"] = i.ToString();
            Response.Cookies.Add(nameCookie);

            GetMolDataID(i);

            int CID = ShrdMaster.Instance.GetCompanyID();

            CommonDropDownVal();
            var NoT = db.TblDDTSType.Where(x=> x.CompanyID == CID).ToList();
            List<SelectListItem> NoType = new List<SelectListItem>();
            foreach (var x in NoT)
            {
                NoType.Add(new SelectListItem
                {
                    Text = x.TSType,
                    Value = x.ID.ToString()
                });
            }

            ViewBag.NoType = NoType.OrderBy(x => x.Text).ThenBy(x => x.Text);


            var Mold = db.TblMoldData.Where(x => x.MoldDataID == i).FirstOrDefault();
            if (FuncName == "Mold")
            {
                return PartialView("_MoldDetails", Mold);
            }

            else if (FuncName == "Tooling")
            {

                var newTool = db.TblMoldTooling.Where(x => x.MoldDataID == i && x.CompanyID == CID).ToList();
                var ToolingType = db.TblDDMoldToolingTypes.Where(x=> x.CompanyID == CID).ToList();
                int I = 0;

                foreach (var x in newTool)
                {
                    if (x.MoldToolingType != "" && x.MoldToolingType != null)
                    {
                        I = Convert.ToInt32(x.MoldToolingType);
                    }

                    var DataTool = ToolingType.Where(c => c.ID == I).FirstOrDefault();
                    x.MoldToolingTypeName = DataTool != null ? DataTool.DD_MoldToolingType : "";
                }

                var Tool = newTool.OrderBy(x => x.MoldToolingTypeName).ThenBy(x => x.MoldToolDescrip).ToList();
                return PartialView("_ToolingGetData", Tool);
            }

            else if (FuncName == "Layout")
            {
                var tblLocation = db.TblCavityLocations.Where(x => x.MoldDataID == i && x.CompanyID == CID).OrderBy(x => x.CavityLocationNumber.Length).ThenBy(x => x.CavityLocationNumber).ToList();
                int u = 0;
                if (tblLocation != null)
                {
                    u = tblLocation.Select(x => x.CavityLocationID).FirstOrDefault();
                }
                var tblNumber = db.TblCavityNumbers.Where(x => x.CavityLocationID == u && x.CompanyID == CID).OrderBy(x => x.CavityActive).ThenBy(x => x.CavityNumber).ToList();
                ViewBag.MoldDataID = i;
                LayoutData LD = new LayoutData();
                LD.tblCavityLocations = tblLocation;
                LD.tblCavityNumber = tblNumber;
                return PartialView("_MoldLayout", LD);
            }

            else if (FuncName == "IMLMap")
            {
                if (Mold.MoldMapPath != null)
                {
                    Mold.MoldMapPath = Mold.MoldMapPath != null ? Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/SiteImages/IMLImg/" + Mold.MoldMapPath) : null;
                }
                return PartialView("_IMLMap", Mold);
            }

            else if (FuncName == "TroubleShooter")
            {

                var TStype = db.TblDDTSType.Where(x=> x.CompanyID == CID).ToList();
                int TSI = 0;

                var TblMold = ShrdMaster.Instance.GetTblTSGuideList(i).ToList();
                List<tblTSGuide> Trouble = new List<tblTSGuide>();

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
                    Trouble.Add(TS);
                }

                return PartialView("_TroubleShooterGetData", Trouble.OrderBy(x => x.TSTypeName).ThenBy(x => x.TSDefects));
            }

            else if (FuncName == "TechTips")
            {
                var TechTip = db.TblTechTips.Where(x => x.MoldDataID == i && x.CompanyID == CID).FirstOrDefault();
                string letters = string.Empty;
                string numbers = string.Empty;
                if (TechTip != null)
                {
                    if (TechTip.TTPartImagePath != null)
                    {
                        TechTip.TTPartImagePath = Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/SiteImages/TechTipsImages/" + TechTip.TTPartImagePath);
                    }

                    if (TechTip.TTMoldImagePath != null)
                    {
                        TechTip.TTMoldImagePath = Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/SiteImages/TechTipsImages/" + TechTip.TTMoldImagePath);
                    }

                    if (TechTip.MCWeight != null)
                    {
                        bool ISMCWeight = TechTip.MCWeight.All(char.IsDigit);

                        if (ISMCWeight == true)
                        {
                            foreach (char c in TechTip.MCWeight)
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
                            TechTip.MCWeight = String.Format("{0:n0}", Convert.ToInt32(numbers)) + " " + letters;
                        }
                    }
                }

                return PartialView("_GetTechTips", TechTip == null ? new tblTechTips() : TechTip);
            }
            else if (FuncName == "Notes")
            {
                var Note = db.TblMoldDataNotes.Where(x => x.MoldDataID == i && x.CompanyID == CID).ToList();
                return PartialView("_GetNotesData", Note);
            }
            else
            {
                return PartialView("_ServicingGetData", Mold);
            }
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

        public void CommonDropDownVal()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            ViewBag.BaseStyle = new SelectList(db.TblDDMoldCategoryID.Where(X=> X.CompanyID == CID).ToList().OrderBy(x=> x.MoldCategoryID), "ID", "MoldCategoryID");
            ViewBag.Department = new SelectList(db.TblDDDepartmentID.Where(X=> X.CompanyID == CID).ToList().OrderBy(x=> x.DepartmentID), "ID", "DepartmentID");
            ViewBag.ProdLine = new SelectList(db.TblDDProductLine.Where(X=> X.CompanyID == CID).ToList().OrderBy(x=> x.ProductLine), "ID", "ProductLine");
            ViewBag.ProdPart = new SelectList(db.TblDDProductPart.Where(X=> X.CompanyID == CID).ToList().OrderBy(x=> x.ProductPart), "ID", "ProductPart");
            ViewBag.ResinType = new SelectList(db.TblDDMoldResinType.Where(X=> X.CompanyID == CID).ToList().OrderBy(x=> x.MoldResinType), "ID", "MoldResinType");
            ViewBag.RunnerType = new SelectList(db.TblDDMoldCav.Where(X=> X.CompanyID == CID).ToList().OrderBy(x=> x.MoldCav), "ID", "MoldCav");
            ViewBag.ClientInfo = new SelectList(db.TblCustomer.Where(X=> X.CompanyID == CID).ToList().OrderBy(x=> x.CustomerName), "CustomerID", "CustomerName");

            var ToolingType = db.TblDDMoldToolingTypes.Where(X=> X.CompanyID == CID).OrderBy(x => x.DD_MoldToolingType).ToList();
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

            ViewBag.PlasticFactorVal = new SelectList(db.TblDDFactors.Where(X=> X.CompanyID == CID).ToList().OrderBy(x=> x.Plastic_Type), "PF", "Plastic_Type");
            ViewBag.SteelFactorVal = new SelectList(db.TblDDFactors.Where(X=> X.CompanyID == CID).ToList().OrderBy(x=> x.Steel_Type), "SF", "Steel_Type");
            ViewBag.LocationFactorVal = new SelectList(db.TblDDFactors.Where(X=> X.CompanyID == CID).ToList().OrderBy(x=> x.Location_Type), "LF", "Location_Type");
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
        }

        public ActionResult SearchMold(string str = "")
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            if (str == "")
            {
                var tb = db.TblMoldData.Where(x=> x.CompanyID == CID).ToList().OrderBy(x => x.MoldName);
                return PartialView("_MoldData", tb);
            }
            else
            {
                List<tblMoldData> MoldList = new List<tblMoldData>();
                MoldList = db.TblMoldData.Where(x => x.CompanyID == CID).ToList();
                var tb = MoldList.Where(x => x.MoldName.Contains(str) || x.MoldDesc.Contains(str)).ToList();
                return PartialView("_MoldData", tb);
            }
        }

        public void SaveMoldFocusOut(tblMoldData model, string MoldDateBuilt="", string DateAcquired="", string DateRetired="")
        {
            if (model.MoldDataID != 0)
            {
                if (MoldDateBuilt != "")
                {
                    model.MoldDateBuilt = Convert.ToDateTime(MoldDateBuilt);
                }
                if (DateAcquired != "")
                {
                    model.DateAcquired = Convert.ToDateTime(DateAcquired);
                }
                if (DateRetired != "")
                {
                    model.DateRetired = Convert.ToDateTime(DateRetired);
                }

                int CID = ShrdMaster.Instance.GetCompanyID();

                var data = db.TblMoldData.Where(x => x.MoldDataID == model.MoldDataID && x.CompanyID == CID).FirstOrDefault();
                if (data != null)
                {
                    data.MoldName = model.MoldName;
                    data.MoldDesc = model.MoldDesc;
                    data.ModelNumber = model.ModelNumber;
                    data.SerialNumber = model.SerialNumber;
                    data.MoldCyclesPerMinute = model.MoldCyclesPerMinute;
                    data.MoldCategoryID = model.MoldCategoryID;
                    data.DepartmentID = model.DepartmentID;
                    data.ProductLine = model.ProductLine;
                    data.ProductPart = model.ProductPart;
                    data.DoD = model.DoD;
                    data.TotalShots = model.TotalShots;
                    data.MoldResinType = model.MoldResinType;
                    data.MoldCav = model.MoldCav;
                    data.CavityTotal = model.CavityTotal;
                    data.PlasticFactor = model.PlasticFactor;
                    data.SteelFactor = model.SteelFactor;
                    data.MoldNozzleSize = model.MoldNozzleSize;
                    data.MoldSprueSize = model.MoldSprueSize;
                    data.MoldRunnerSize = model.MoldRunnerSize;
                    data.MoldGateSize = model.MoldGateSize;
                    data.MoldResinVendor = model.MoldResinVendor;
                    data.MoldResinVendorPhone = model.MoldResinVendorPhone;
                    data.MoldProjEngFirstName = model.MoldProjEngFirstName;
                    data.MoldProjEngLastName = model.MoldProjEngLastName;
                    data.MoldProjEngPhone = model.MoldProjEngPhone;
                    data.MoldDateBuilt = model.MoldDateBuilt;
                    data.DateAcquired = model.DateAcquired;
                    data.PurchasePrice = model.PurchasePrice;
                    data.DateRetired = model.DateRetired;
                    data.LocationFactor = model.LocationFactor;
                    data.PF = model.PF;
                    data.SF = model.SF;
                    data.LF = model.LF;
                    data.CustomerID = model.CustomerID;
                    data.CustomerComments = model.CustomerComments;
                    data.Comments = model.Comments;
                    db.SaveChanges();
                    //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.MoldDetails.ToString(), GetAction.Update.ToString());

                }


            }
        }

        public ActionResult SaveMold(tblMoldData obj)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            CommonDropDownVal();
            if (obj.MoldName != null)
            {
                if (obj.MoldDataID == 0)
                {
                    obj.CompanyID = CID; /*db.TblCompanies.Select(x=> x.CompanyID).FirstOrDefault();*/
                    db.TblMoldData.Add(obj);
                    db.SaveChanges();
                    //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.MoldDetails.ToString(), GetAction.Create.ToString());


                }

                else
                {
                    try
                    {
                        var data = db.TblMoldData.Where(x => x.MoldDataID == obj.MoldDataID && x.CompanyID == CID).FirstOrDefault();
                        if (data != null)
                        {
                            data.MoldName = obj.MoldName;
                            data.MoldDesc = obj.MoldDesc;
                            data.ModelNumber = obj.ModelNumber;
                            data.SerialNumber = obj.SerialNumber;
                            data.MoldCyclesPerMinute = obj.MoldCyclesPerMinute;
                            data.MoldCategoryID = obj.MoldCategoryID;
                            data.DepartmentID = obj.DepartmentID;
                            data.ProductLine = obj.ProductLine;
                            data.ProductPart = obj.ProductPart;
                            data.DoD = obj.DoD;
                            data.TotalShots = obj.TotalShots;
                            data.MoldResinType = obj.MoldResinType;
                            data.MoldCav = obj.MoldCav;
                            data.CavityTotal = obj.CavityTotal;
                            data.SteelFactor = obj.SteelFactor;
                            data.MoldNozzleSize = obj.MoldNozzleSize;
                            data.MoldSprueSize = obj.MoldSprueSize;
                            data.MoldRunnerSize = obj.MoldRunnerSize;
                            data.MoldGateSize = obj.MoldGateSize;
                            data.MoldResinVendor = obj.MoldResinVendor;
                            data.MoldResinVendorPhone = obj.MoldResinVendorPhone;
                            data.MoldProjEngFirstName = obj.MoldProjEngFirstName;
                            data.MoldProjEngLastName = obj.MoldProjEngLastName;
                            data.MoldProjEngPhone = obj.MoldProjEngPhone;
                            data.MoldDateBuilt = obj.MoldDateBuilt;
                            data.DateAcquired = obj.DateAcquired;
                            data.PurchasePrice = obj.PurchasePrice;
                            data.DateRetired = obj.DateRetired;
                            data.LocationFactor = obj.LocationFactor;
                            data.PF = obj.PF;
                            data.SF = obj.SF;
                            data.LF = obj.LF;
                            data.CustomerID = obj.CustomerID;
                            data.CustomerComments = obj.CustomerComments;
                            data.Comments = obj.Comments;
                            db.SaveChanges();
                            //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.MoldDetails.ToString(), GetAction.Update.ToString());

                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }

            var ds = db.TblMoldData.Where(X=> X.CompanyID == CID).OrderBy(x => x.MoldName).ToList();
            var MoldDetails = RenderRazorViewToString(this.ControllerContext, "_MoldDetails", obj);
            var MoldData = RenderRazorViewToString(this.ControllerContext, "_MoldData", ds);
            var MoldID = obj.MoldDataID;

            return Json(new { MoldDetails, MoldData, MoldID }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CopyToolingMold(List<tblMoldTooling> model, int MainMoldID=0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            var dd = db.TblMoldTooling.Where(X=> X.CompanyID == CID).ToList();
            if (model != null)
            {
                foreach (var x in model)
                {
                    var tb = dd.Where(c => c.MoldToolingID == x.MoldToolingID && x.CompanyID == CID).FirstOrDefault();
                    if (tb == null)
                    {
                        tblMoldTooling tbb = new tblMoldTooling();
                        tbb.MoldToolingType = x.MoldToolingType;
                        tbb.MoldDataID = x.MoldDataID;
                        tbb.CompanyID = CID;
                        tbb.MoldToolDescrip = x.MoldToolDescrip;
                        tbb.MoldToolingPartNumber = x.MoldToolingPartNumber;
                        tbb.MoldToolingPrintNumber = x.MoldToolingPrintNumber;
                        tbb.MoldToolingVendor = x.MoldToolingVendor;
                        tbb.MoldToolCost = x.MoldToolCost;
                        tbb.MoldToolingPartsOnHand = x.MoldToolingPartsOnHand;
                        tbb.MoldToolingReorderLevel = x.MoldToolingReorderLevel;
                        tbb.MoldToolingNumOrdered = x.MoldToolingNumOrdered;
                        tbb.MoldToolingDateOrdered = x.MoldToolingDateOrdered;
                        tbb.MoldToolingNumReceived = x.MoldToolingNumReceived;
                        db.TblMoldTooling.Add(tbb);
                        db.SaveChanges();

                    }
                    else
                    {
                        tb.MoldToolingType = x.MoldToolingType;
                        tb.MoldToolDescrip = x.MoldToolDescrip;
                        tb.MoldToolingPartNumber = x.MoldToolingPartNumber;
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

                    //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.Tooling.ToString(), GetAction.Copy.ToString());

                }

                int i = ShrdMaster.Instance.ReturnSelectedMoldID(MainMoldID);

                CommonDropDownVal();

                var data = ShrdMaster.Instance.GetToolingList(i, CID);
                var NewData = data.OrderBy(x => x.MoldToolingType).ThenBy(x => x.MoldToolDescrip).ToList();
                return PartialView("_ToolingGetData", NewData);
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteMold(int ID)
        {
            if (ID != 0)
            {
                var data = db.TblMoldData.Where(x => x.MoldDataID == ID).FirstOrDefault();
                db.TblMoldData.Remove(data);
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.MoldDetails.ToString(), GetAction.Delete.ToString());

                //return Json("Deleted", JsonRequestBehavior.AllowGet);
            }

            int CID = ShrdMaster.Instance.GetCompanyID();

            CommonDropDownVal();
            var ds = db.TblMoldData.Where(X=> X.CompanyID == CID).OrderBy(x => x.MoldName).ToList();
            var MoldDetails = RenderRazorViewToString(this.ControllerContext, "_MoldDetails", ds.FirstOrDefault());
            var MoldData = RenderRazorViewToString(this.ControllerContext, "_MoldData", ds);

            return Json(new { MoldDetails, MoldData }, JsonRequestBehavior.AllowGet);
            //return Json("Oops something went wrong", JsonRequestBehavior.AllowGet);
        }

        public ActionResult SortingMoldList(int ID = 0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            if (ID == 0)
            {
                var tb = db.TblMoldData.Where(x=> x.CompanyID == CID).ToList().OrderBy(x => x.MoldName);
                return PartialView("_MoldData", tb);

            }
            else
            {
                var tb = db.TblMoldData.Where(x=> x.CompanyID == CID).ToList().OrderByDescending(x => x.MoldName);
                return PartialView("_MoldData", tb);
            }
        }


        public ActionResult ExportMoldData(HttpPostedFileBase MoldPostedFile)
        {
            string filePath = string.Empty;
            if (MoldPostedFile != null)
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

                filePath = path + Path.GetFileName(MoldPostedFile.FileName);
                string extension = Path.GetExtension(MoldPostedFile.FileName);
                MoldPostedFile.SaveAs(filePath);

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
                    try
                    {
                        MoldDataInsert(dataTable, ShetN[0]);
                    }
                    catch (Exception ex)
                    {
                        WriteLog(ex.Message);
                    }
                    con.Close();
                }
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }


        public void MoldDataInsert(DataTable dt, string sheetName)
        {
            //string constring = db.GetConnectionString();
            //SqlConnection con = new SqlConnection(constring);

            using (SqlConnection connection = new SqlConnection(constring))
            {

                List<tblMoldData> SR = new List<tblMoldData>();
                var data = db.TblDDMoldCategoryID.ToList();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string Name = dt.Rows[i].ItemArray[0].ToString() == "" ? null : dt.Rows[i].ItemArray[0].ToString();
                    string Desc = dt.Rows[i].ItemArray[1].ToString() == "" ? null : dt.Rows[i].ItemArray[1].ToString();

                    if (Name != null && Desc != null)
                    { 
                        var dd = data.Where(x => x.MoldCategoryID == dt.Rows[i].ItemArray[0].ToString() && x.MoldCategoryIDDesc == Desc).FirstOrDefault();
                        var ToolingTypeData = db.TblDDMoldToolingTypes.ToList();

                        if (dd == null)
                        {
                            //TblDDMoldToolingTypes
                            tblMoldData Mold = new tblMoldData();


                            Mold.MoldName = (dt.Rows[i].ItemArray[0].ToString() == "" ? null : dt.Rows[i].ItemArray[0].ToString());
                            Mold.MoldDesc = (dt.Rows[i].ItemArray[1].ToString() == "" ? null : dt.Rows[i].ItemArray[1].ToString());
                            Mold.ModelNumber = dt.Rows[i].ItemArray[2].ToString() == "" ? null : dt.Rows[i].ItemArray[2].ToString();
                            Mold.SerialNumber = dt.Rows[i].ItemArray[3].ToString() == "" ? null : dt.Rows[i].ItemArray[3].ToString();
                            //Mold.MoldCyclesPerMinute = Convert.ToDouble(dt.Rows[i].ItemArray[4]);
                            if (!(dt.Rows[i].ItemArray[4] is DBNull))
                                Mold.MoldCyclesPerMinute = Convert.ToDouble(dt.Rows[i].ItemArray[4].ToString().Replace(",", ""));

                            if (!(dt.Rows[i].ItemArray[5] is DBNull))
                                Mold.DoD = Convert.ToDouble(dt.Rows[i].ItemArray[5].ToString().Replace(",", ""));

                            Mold.MoldNozzleSize = (dt.Rows[i].ItemArray[6].ToString() == "" ? null : dt.Rows[i].ItemArray[6].ToString());

                            Mold.MoldSprueSize = (dt.Rows[i].ItemArray[7].ToString() == "" ? null : dt.Rows[i].ItemArray[7].ToString());
                            Mold.MoldRunnerSize = (dt.Rows[i].ItemArray[8].ToString() == "" ? null : dt.Rows[i].ItemArray[8].ToString());
                            Mold.MoldGateSize = (dt.Rows[i].ItemArray[9].ToString() == "" ? null : dt.Rows[i].ItemArray[9].ToString());

                            if (!(dt.Rows[i].ItemArray[10] is DBNull))
                                Mold.MoldDateBuilt = Convert.ToDateTime(dt.Rows[i].ItemArray[10]);

                            if (!(dt.Rows[i].ItemArray[11] is DBNull))
                                Mold.DateAcquired = Convert.ToDateTime(dt.Rows[i].ItemArray[11]);

                            if (!(dt.Rows[i].ItemArray[12] is DBNull))
                                Mold.TotalShots = Convert.ToDouble(dt.Rows[i].ItemArray[12].ToString().Replace(",", ""));

                            if (!(dt.Rows[i].ItemArray[13] is DBNull))
                                Mold.CavityTotal = Convert.ToInt32(dt.Rows[i].ItemArray[13].ToString().Replace(",", ""));

                            //Mold.DoD = Convert.da dt.Rows[i].ItemArray[3].ToString();
                            //Mold.MoldNozzleSize = dt.Rows[i].ItemArray[3].ToString();

                            Mold.MoldResinVendor = (dt.Rows[i].ItemArray[14].ToString() == "" ? null : dt.Rows[i].ItemArray[14].ToString());
                            Mold.MoldResinVendorPhone = (dt.Rows[i].ItemArray[15].ToString() == "" ? null : dt.Rows[i].ItemArray[15].ToString());
                            Mold.MoldProjEngFirstName = (dt.Rows[i].ItemArray[16].ToString() == "" ? null : dt.Rows[i].ItemArray[16].ToString());
                            Mold.MoldProjEngLastName = (dt.Rows[i].ItemArray[17].ToString() == "" ? null : dt.Rows[i].ItemArray[17].ToString());
                            Mold.MoldProjEngPhone = (dt.Rows[i].ItemArray[18].ToString() == "" ? null : dt.Rows[i].ItemArray[18].ToString());

                            if (!(dt.Rows[i].ItemArray[19] is DBNull))
                                Mold.PurchasePrice = Convert.ToDecimal(dt.Rows[i].ItemArray[19].ToString().Replace(",", ""));

                            if (!(dt.Rows[i].ItemArray[20] is DBNull))
                                Mold.DateRetired = Convert.ToDateTime(dt.Rows[i].ItemArray[20]);

                            Mold.CustomerComments = (dt.Rows[i].ItemArray[21].ToString() == "" ? null : dt.Rows[i].ItemArray[21].ToString());
                            Mold.Comments = (dt.Rows[i].ItemArray[22].ToString() == "" ? null : dt.Rows[i].ItemArray[22].ToString());
                            Mold.CompanyID = ShrdMaster.Instance.GetCompanyID();

                            db.TblMoldData.Add(Mold);
                            db.SaveChanges();

                           CreaCavityLocation(Mold.MoldDataID, Mold.CavityTotal);
                        }
                    }
                }
            }
        }

        public void CreaCavityLocation(int MoldID = 0, int? CavityNo = 0)
        {
            var MoldD = db.TblMoldData.Where(x => x.MoldDataID == MoldID).FirstOrDefault();
            MoldD.CavityTotal = CavityNo;
            db.SaveChanges();

            var tblC = db.TblCavityLocations.Where(x => x.MoldDataID == MoldID).ToList();

            int CavN = 0;
            int CavNumInt = Convert.ToInt32(ReturnLastCavityLocaation(tblC));
            if (CavNumInt != null)
            {
                CavN = CavNumInt;
            }
            //var no = tblC.Count() - CavityNo;
            //int no = CavityNo - CavNumInt.Count();

            if (CavityNo > CavN)
            {
                if (tblC.Count() != 0)
                {
                    //var asd = tblC.LastOrDefault();
                    var asd = ReturnLastCavityLocaation(tblC);

                    int LastNo = asd;

                    for (int i = Convert.ToInt32(LastNo) + 1; i <= CavityNo; i++)
                    {
                        tblCavityLocation tbl = new tblCavityLocation();
                        tbl.MoldDataID = MoldID;
                        tbl.CompanyID = ShrdMaster.Instance.GetCompanyID();
                        tbl.CavityLocationNumber = i.ToString();
                        db.TblCavityLocations.Add(tbl);
                        db.SaveChanges();

                        tblCavityNumber TBNum = new tblCavityNumber();
                        TBNum.CavityLocationID = tbl.CavityLocationID;
                        TBNum.CavityActive = true;
                        TBNum.CavityNumber = i.ToString();
                        tbl.CompanyID = ShrdMaster.Instance.GetCompanyID();
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
                        tbl.CavityLocationNumber = i.ToString();
                        tbl.CompanyID = ShrdMaster.Instance.GetCompanyID();
                        db.TblCavityLocations.Add(tbl);
                        db.SaveChanges();

                        tblCavityNumber TBNum = new tblCavityNumber();
                        TBNum.CavityLocationID = tbl.CavityLocationID;
                        TBNum.CavityActive = true;
                        TBNum.CavityNumber = i.ToString();
                        TBNum.CompanyID = ShrdMaster.Instance.GetCompanyID();
                        db.TblCavityNumbers.Add(TBNum);
                        db.SaveChanges();
                    }
                }
            }

        }



        #region Trouble Shooter Guide

        public ActionResult TroubleShooterGetData(int ID = 0)
        {

            int CID = ShrdMaster.Instance.GetCompanyID();
            var NoT = db.TblDDTSType.Where(X=> X.CompanyID == CID).ToList();
            List<SelectListItem> NoType = new List<SelectListItem>();
            foreach (var x in NoT)
            {
                NoType.Add(new SelectListItem
                {
                    Text = x.TSType,
                    Value = x.ID.ToString()
                });
            }
            ViewBag.NoType = NoType.OrderBy(x=> x.Text).ThenBy(x=> x.Text);


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
            int i = ShrdMaster.Instance.ReturnSelectedMoldID(ID);

            var TStype = db.TblDDTSType.Where(X=>X.CompanyID == CID).ToList();
            int TSI = 0;

            var TblMold = ShrdMaster.Instance.GetTblTSGuideList(i).ToList();
            List<tblTSGuide> data = new List<tblTSGuide>();

            foreach (var x in TblMold)
            {
                tblTSGuide TS = new tblTSGuide();
                TS.TSGuide = x.TSGuide;
                TS.CompanyID = CID;
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
                data.Add(TS);
            }

            return PartialView("_TroubleShooterGetData", data.OrderBy(x => x.TSTypeName).ThenBy(x => x.TSDefects).ToList());
        }

        [HttpPost]
        public ActionResult CreateTroubleShooter(tblTSGuide model)
        {
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
            //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.TroubleshootGuide.ToString(), GetAction.Create.ToString());


            var NoT = db.TblDDTSType.Where(X=> X.CompanyID == CID).ToList();
            List<SelectListItem> NoType = new List<SelectListItem>();
            foreach (var x in NoT)
            {
                NoType.Add(new SelectListItem
                {
                    Text = x.TSType,
                    Value = x.ID.ToString()
                });
            }

            ViewBag.NoType = NoType.OrderBy(x => x.Text).ThenBy(x => x.Text);

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

            var Trouble12 = db.TblTSGuide.Where(x => x.MoldDataID == model.MoldDataID && x.CompanyID == CID).ToList();
            List<tblTSGuide> Trouble = new List<tblTSGuide>();
            var TStype = db.TblDDTSType.ToList();
            int TSI = 0;

            foreach (var x in Trouble12)
            {
                tblTSGuide TSG = new tblTSGuide();
                TSG.TSGuide = x.TSGuide;
                TSG.MoldDataID = x.MoldDataID;
                TSG.TSSeqNum = x.TSSeqNum;
                TSG.TSDefects = x.TSDefects;
                TSG.CompanyID = CID;
                TSG.TSExplanation = x.TSExplanation;
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
                TSG.ImagePath = x.ImagePath != null ? Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/TroubleShooterImage/" + x.ImagePath) : x.ImagePath;
                Trouble.Add(TSG);
            }

            return PartialView("_TroubleShooterGetData", Trouble.OrderBy(x => x.TSTypeName).ThenBy(x => x.TSDefects));
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
                        //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.TroubleshootGuide.ToString(), GetAction.Update.ToString());

                        var FilePath = Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/TroubleShooterImage/" + FileP);
                        return Json(FilePath);
                    }
                    else
                    {
                        dd.ImagePath = ImgName;
                        db.SaveChanges();
                        //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.TroubleshootGuide.ToString(), GetAction.Update.ToString());

                        var FilePath = Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/TroubleShooterImage/" + ImgName);
                        return Json(FilePath);
                    }

                }
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
            if (str != "")
            {
                str = str.Substring(0, str.LastIndexOf(','));
                using (var ctx = new MoldtraxDbContext())
                {
                    SqlParameter sp = new SqlParameter("@value", str);
                    var result = ctx.Database.ExecuteSqlCommand("exec sp_DeleteTroubleShooter @value", sp);
                }
            }

            int CID = ShrdMaster.Instance.GetCompanyID();

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
            var NoT = db.TblDDTSType.Where(X=> X.CompanyID == CID).ToList();
            List<SelectListItem> NoType = new List<SelectListItem>();
            foreach (var x in NoT)
            {
                NoType.Add(new SelectListItem
                {
                    Text = x.TSType,
                    Value = x.ID.ToString()
                });
            }
            ViewBag.NoType = NoType.OrderBy(x => x.Text).ThenBy(x => x.Text);


            var tbl = db.TblTSGuide.Where(x => x.MoldDataID == MoldID && x.CompanyID == CID).ToList();
            List<tblTSGuide> data = new List<tblTSGuide>();
            var TStype = db.TblDDTSType.ToList();
            int TSI = 0;

            foreach (var x in tbl)
            {
                tblTSGuide TSG = new tblTSGuide();
                TSG.TSGuide = x.TSGuide;
                TSG.MoldDataID = x.MoldDataID;
                TSG.TSSeqNum = x.TSSeqNum;
                TSG.TSDefects = x.TSDefects;
                TSG.CompanyID = CID;
                TSG.TSExplanation = x.TSExplanation;
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
                TSG.ImagePath = x.ImagePath != null ? Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/TroubleShooterImage/" + x.ImagePath) : x.ImagePath;
                data.Add(TSG);
            }
            return PartialView("_TroubleShooterGetData", data.OrderBy(x => x.TSTypeName).ThenBy(x => x.TSDefects));
        }

        public ActionResult EditTroubleShooter(string str, int MoldID = 0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            str = str.Substring(0, str.LastIndexOf(','));
            List<int> TroubleList = str.Split(',').Select(int.Parse).ToList();
            var Trouble = db.TblTSGuide.Where(X=> X.CompanyID == CID).ToList();

            List<tblTSGuide> tl = new List<tblTSGuide>();

            foreach (var x in TroubleList)
            {
                var ds = Trouble.Where(s => s.TSGuide == x).FirstOrDefault();
                ds.ImagePath = Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/TroubleShooterImage/" + ds.ImagePath);
                tl.Add(ds);
            }
            return PartialView("_EditTroubleShooter", tl);
        }

        public ActionResult SaveTroubleShooterList(List<tblTSGuideViewModel> model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            if (model != null)
            {
                int i = 0;
                i = model.Select(x => x.MoldDataID).FirstOrDefault();

                foreach (var x in model)
                {
                    var tb = db.TblTSGuide.Where(c => c.TSGuide == x.TSGuide).FirstOrDefault();
                    tb.TSGuide = x.TSGuide;
                    tb.TSType = x.TSType;
                    tb.TSDefects = x.TSDefects;
                    tb.TSExplanation = x.TSExplanation;
                    tb.TSProbCause = x.TSProbCause;
                    tb.TSSolution = x.TSSolution;
                    tb.TSPreventAction = x.TSPreventAction;
                    db.SaveChanges();
                }

                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.TroubleshootGuide.ToString(), GetAction.Update.ToString());

            }
            return Json("", JsonRequestBehavior.AllowGet);
            }


        [HttpPost]
        public ActionResult EditTroubleShooter(List<tblTSGuideViewModel> model, int MainMoldID=0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            if (model != null)
            {
                int i = 0;
                i = model.Select(x => x.MoldDataID).FirstOrDefault();

                foreach (var x in model)
                {
                    var tb = db.TblTSGuide.Where(c => c.TSGuide == x.TSGuide).FirstOrDefault();

                    if (tb == null)
                    {
                        tblTSGuide TSG = new tblTSGuide();
                        TSG.TSGuide = x.TSGuide;
                        TSG.TSType = x.TSType;
                        TSG.CompanyID = CID;
                        TSG.TSDefects = x.TSDefects;
                        TSG.ImagePath = Path.GetFileName(x.ImagePath);
                        TSG.TSExplanation = x.TSExplanation;
                        TSG.TSProbCause = x.TSProbCause;
                        TSG.TSSolution = x.TSSolution;
                        TSG.TSPreventAction = x.TSPreventAction;
                        TSG.MoldDataID = x.MoldDataID;
                        db.TblTSGuide.Add(TSG);
                        db.SaveChanges();
                        //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.TroubleshootGuide.ToString(), GetAction.Create.ToString());

                    }

                    else
                    {
                        tb.TSGuide = x.TSGuide;
                        tb.TSType = x.TSType;
                        tb.TSDefects = x.TSDefects;
                        tb.TSExplanation = x.TSExplanation;
                        tb.TSProbCause = x.TSProbCause;
                        tb.TSSolution = x.TSSolution;
                        tb.TSPreventAction = x.TSPreventAction;

                        db.SaveChanges();
                        //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.TroubleshootGuide.ToString(), GetAction.Update.ToString());

                    }
                }

                var TblMold = ShrdMaster.Instance.GetTblTSGuideList(MainMoldID).ToList();
                List<tblTSGuide> data = new List<tblTSGuide>();
                var TStype = db.TblDDTSType.Where(X=> X.CompanyID == CID).ToList();
                int TSI = 0;

                foreach (var x in TblMold)
                {
                    tblTSGuide TS = new tblTSGuide();
                    TS.TSGuide = x.TSGuide;
                    TS.MoldDataID = x.MoldDataID;
                    TS.TSSeqNum = x.TSSeqNum;
                    TS.CompanyID = CID;
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
                    data.Add(TS);
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
                return PartialView("_TroubleShooterGetData", data.OrderBy(x => x.TSTypeName).ThenBy(x => x.TSDefects));
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
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.TroubleshootGuide.ToString(), GetAction.Update.ToString());

            }

            return Json("", JsonRequestBehavior.AllowGet);

        }


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
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.TroubleshootGuide.ToString(), GetAction.Create.ToString());

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
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.TroubleshootGuide.ToString(), GetAction.Update.ToString());

            }
        }

        public ActionResult ExportTroubleData(HttpPostedFileBase postedFile, int MID = 0)
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
                    try
                    {
                        TroubleShooterDataInsert(dataTable, ShetN[0], MID);
                    }
                    catch (Exception ex)
                    {
                        WriteLog(ex.Message);
                    }
                    con.Close();
                }
            }

            var NoT = db.TblDDTSType.Where(X=> X.CompanyID == CID).ToList();
            List<SelectListItem> NoType = new List<SelectListItem>();
            foreach (var x in NoT)
            {
                NoType.Add(new SelectListItem
                {
                    Text = x.TSType,
                    Value = x.ID.ToString()
                });
            }
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

            foreach (var x in TblMold)
            {
                tblTSGuide TS = new tblTSGuide();
                TS.TSGuide = x.TSGuide;
                TS.MoldDataID = x.MoldDataID;
                TS.TSSeqNum = x.TSSeqNum;
                TS.CompanyID = CID;
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
                TblTSGuideList.Add(TS);
            }

            return PartialView("_TroubleShooterGetData", TblTSGuideList.OrderBy(x => x.TSTypeName).ThenBy(x => x.TSDefects).ToList());
        }

        public void TroubleShooterDataInsert(DataTable dt, string sheetName, int MID = 0)
        {

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
                        var ToolingTypeData = db.TblDDTSType.ToList();

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


        public void WriteLog(string strLog)
        {
            try
            {
                string logFilePath = Server.MapPath("~/SiteImages/" + "Log.txt");
                //string logFilePath = Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/SiteImages/" + dbName + "/") + "Log.txt";
                FileInfo logFileInfo = new FileInfo(logFilePath);
                DirectoryInfo logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
                if (!logDirInfo.Exists) logDirInfo.Create();
                using (FileStream fileStream = new FileStream(logFilePath, FileMode.Append))
                {
                    using (StreamWriter log = new StreamWriter(fileStream))
                    {
                        string NewLine = "======================== " + System.DateTime.Now + " =======================================================================";
                        log.WriteLine(NewLine);
                        log.WriteLine(strLog);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region Tooling
        public ActionResult ToolingGetData(int ID = 0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            int i = ShrdMaster.Instance.ReturnSelectedMoldID(ID);
            CommonDropDownVal();

            var data = ShrdMaster.Instance.GetToolingList(i, CID);

            var ToolingType = db.TblDDMoldToolingTypes.Where(x=> x.CompanyID == CID).ToList();
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

            var NewData = data.Where(x => x.MoldDataID == i).OrderBy(x => x.MoldToolingTypeName).ThenBy(x => x.MoldToolDescrip).ToList();

            return PartialView("_ToolingGetData", NewData);
        }

        public ActionResult CreateTooling()
        {
            CommonDropDownVal();
            return PartialView("_CreateTooling", new tblMoldTooling());
        }

        [HttpPost]
        public ActionResult CreateTooling(tblMoldTooling model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            CommonDropDownVal();
            List<tblMoldTooling> data = new List<tblMoldTooling>();
            if (model != null)
            {
                model.CompanyID = CID;
                db.TblMoldTooling.Add(model);
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.Tooling.ToString(), GetAction.Create.ToString());

                data = db.TblMoldTooling.Where(x => x.MoldDataID == model.MoldDataID && x.CompanyID == CID).ToList();
            }

            return PartialView("_ToolingGetData", data);
        }

        public void SaveToolingFunc(tblMoldTooling model)
        {
            if (model.MoldToolingID != 0)
            {
                var tb = db.TblMoldTooling.Where(c => c.MoldToolingID == model.MoldToolingID).FirstOrDefault();
                if (tb == null)
                {
                    model.CompanyID = ShrdMaster.Instance.GetCompanyID();
                    db.TblMoldTooling.Add(model);
                    db.SaveChanges();
                    //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.Tooling.ToString(), GetAction.Create.ToString());

                }

                else
                {
                    try
                    {
                        tb.MoldToolingType = model.MoldToolingType;
                        tb.MoldToolDescrip = model.MoldToolDescrip;
                        tb.MoldToolingPartNumber = model.MoldToolingPartNumber;
                        tb.MoldToolingPrintNumber = model.MoldToolingPrintNumber;
                        tb.MoldToolingVendor = model.MoldToolingVendor;
                        tb.MoldToolCost = model.MoldToolCost;
                        tb.MoldToolingPartsOnHand = model.MoldToolingPartsOnHand;
                        tb.MoldToolingReorderLevel = model.MoldToolingReorderLevel;
                        tb.MoldToolingNumOrdered = model.MoldToolingNumOrdered;
                        tb.MoldToolingDateOrdered = model.MoldToolingDateOrdered;
                        tb.MoldToolingNumReceived = model.MoldToolingNumReceived;
                        db.SaveChanges();
                        //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.Tooling.ToString(), GetAction.Update.ToString());

                    }

                    catch (Exception ex)
                    {

                    }
                }
            }
        }

        [HttpPost]
        public ActionResult SaveToolingList(List<tblMoldTooling> model)
        {
            var dd = db.TblMoldTooling.ToList();
            if (model != null)
            {
                foreach (var x in model)
                {
                    var tb = dd.Where(c => c.MoldToolingID == x.MoldToolingID).FirstOrDefault();

                    tb.MoldToolingType = x.MoldToolingType;
                    tb.MoldToolDescrip = x.MoldToolDescrip;
                    tb.MoldToolingPartNumber = x.MoldToolingPartNumber;
                    tb.MoldToolingPrintNumber = x.MoldToolingPrintNumber;
                    tb.Location = x.Location;
                    tb.MoldToolingVendor = x.MoldToolingVendor;
                    tb.MoldToolCost = x.MoldToolCost;
                    tb.MoldToolingPartsOnHand = x.MoldToolingPartsOnHand;
                    tb.MoldToolingReorderLevel = x.MoldToolingReorderLevel;
                    tb.MoldToolingNumOrdered = x.MoldToolingNumOrdered;
                    tb.MoldToolingDateOrdered = x.MoldToolingDateOrdered;
                    tb.MoldToolingNumReceived = x.MoldToolingNumReceived;
                    db.SaveChanges();
                }
                    //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.Tooling.ToString(), GetAction.Update.ToString());

            }
            return Json("", JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult EditTooling(List<tblMoldTooling> model, int MainMoldID)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            var dd = db.TblMoldTooling.Where(x=> x.CompanyID == CID).ToList();

            if (model != null)
            {
                foreach (var x in model)
                {
                    var tb = dd.Where(c => c.MoldToolingID == x.MoldToolingID).FirstOrDefault();
                    if (tb == null)
                    {
                        tblMoldTooling tbb = new tblMoldTooling();
                        tbb.MoldToolingType = x.MoldToolingType;
                        tbb.MoldDataID = x.MoldDataID;
                        tbb.MoldToolDescrip = x.MoldToolDescrip;
                        tbb.Location = x.Location;
                        tbb.MoldToolingPartNumber = x.MoldToolingPartNumber;
                        tbb.MoldToolingPrintNumber = x.MoldToolingPrintNumber;
                        tbb.MoldToolingVendor = x.MoldToolingVendor;
                        tbb.MoldToolCost = x.MoldToolCost;
                        tbb.MoldToolingPartsOnHand = x.MoldToolingPartsOnHand;
                        tbb.MoldToolingReorderLevel = x.MoldToolingReorderLevel;
                        tbb.MoldToolingNumOrdered = x.MoldToolingNumOrdered;
                        tbb.MoldToolingDateOrdered = x.MoldToolingDateOrdered;
                        tbb.MoldToolingNumReceived = x.MoldToolingNumReceived;
                        db.TblMoldTooling.Add(tbb);
                        db.SaveChanges();
                        //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.Tooling.ToString(), GetAction.Create.ToString());

                    }
                    else
                    {
                        tb.MoldToolingType = x.MoldToolingType;
                        tb.MoldToolDescrip = x.MoldToolDescrip;
                        tb.MoldToolingPartNumber = x.MoldToolingPartNumber;
                        tb.MoldToolingPrintNumber = x.MoldToolingPrintNumber;
                        tb.MoldToolingVendor = x.MoldToolingVendor;
                        tb.MoldToolCost = x.MoldToolCost;
                        tb.Location = x.Location;
                        tb.MoldToolingPartsOnHand = x.MoldToolingPartsOnHand;
                        tb.MoldToolingReorderLevel = x.MoldToolingReorderLevel;
                        tb.MoldToolingNumOrdered = x.MoldToolingNumOrdered;
                        tb.MoldToolingDateOrdered = x.MoldToolingDateOrdered;
                        tb.MoldToolingNumReceived = x.MoldToolingNumReceived;
                        db.SaveChanges();
                        //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.Tooling.ToString(), GetAction.Update.ToString());
                    }
                }

                int i = ShrdMaster.Instance.ReturnSelectedMoldID(MainMoldID);

                //int CID = GetCompanyID();
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

                var NewData = data.Where(x => x.MoldDataID == i && x.CompanyID == CID).OrderBy(x => x.MoldToolingTypeName).ThenBy(x => x.MoldToolDescrip).ToList();
                return PartialView("_ToolingGetData", NewData);
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteTooling(string str, int MoldID)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            if (str != "")
            {
                str = str.Substring(0, str.LastIndexOf(','));
                using (var ctx = new MoldtraxDbContext())
                {
                    SqlParameter sp = new SqlParameter("@value", str);
                    var result = ctx.Database.ExecuteSqlCommand("exec procDeleteTooling @value", sp);
                }
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.Tooling.ToString(), GetAction.Delete.ToString());


            }

            CommonDropDownVal();
            var NewData = db.TblMoldTooling.Where(x => x.MoldDataID == MoldID && x.CompanyID == CID).ToList();
            return PartialView("_ToolingGetData", NewData);
        }

        public ActionResult ExportToolingData(HttpPostedFileBase postedFile, int MID = 0)
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
                    string query = string.Format("SELECT * FROM [{0}]", sheetName);
                    OleDbDataAdapter adapter = new OleDbDataAdapter(query, con);
                    adapter.Fill(dataTable);
                    data.Tables.Add(dataTable);
                    string[] ShetN = sheetName.Split('$');
                    try
                    {
                        ToolingDataInsert(dataTable, ShetN[0], MID);
                    }
                    catch (Exception ex)
                    {
                        WriteLog(ex.Message);
                    }
                    con.Close();
                }
            }

            CommonDropDownVal();

            int CID = ShrdMaster.Instance.GetCompanyID();

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
                var data = db.TblDDMoldCategoryID.Where(X=> X.CompanyID == CID).ToList();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i].ItemArray[0].ToString() != "" && dt.Rows[i].ItemArray[1].ToString() != "")
                    {
                        string Desc = dt.Rows[i].ItemArray[1].ToString() == "" ? null : dt.Rows[i].ItemArray[1].ToString();

                        var dd = data.Where(x => x.MoldCategoryID == dt.Rows[i].ItemArray[0].ToString() && x.MoldCategoryIDDesc == Desc).FirstOrDefault();
                        var ToolingTypeData = db.TblDDMoldToolingTypes.Where(x=> x.CompanyID == CID).ToList();

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


        #region Notes

        public ActionResult GetNotesList(int ID = 0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            int i = ShrdMaster.Instance.ReturnSelectedMoldID(ID);
            var data = db.TblMoldDataNotes.Where(x => x.MoldDataID == i && x.CompanyID == CID).ToList();
            return PartialView("_GetNotesData", data);
        }

        public ActionResult SaveSelectedNotesList(List<tblMoldDataNotes> model)
        {
            if (model != null)
            {
                foreach (var x in model)
                {
                    int CID = ShrdMaster.Instance.GetCompanyID();

                    var data = db.TblMoldDataNotes.Where(s => s.MoldDataNotesAutoID == x.MoldDataNotesAutoID).FirstOrDefault();
                    data.MoldDataNotesDate = x.MoldDataNotesDate;
                    data.MoldDataNotesMemo = string.IsNullOrWhiteSpace(x.MoldDataNotesMemo) ? null : x.MoldDataNotesMemo;
                    db.SaveChanges();
                    //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.Notes.ToString(), GetAction.Update.ToString());

                }
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddNotes()
        {
            return PartialView("_AddNotes");
        }

        public void SaveNotesFocusOut(tblMoldDataNotes model)
         {

            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblMoldDataNotes.Where(s => s.MoldDataNotesAutoID == model.MoldDataNotesAutoID).FirstOrDefault();
            data.MoldDataNotesDate = model.MoldDataNotesDate;
            data.MoldDataNotesMemo = string.IsNullOrWhiteSpace(model.MoldDataNotesMemo) ? null : model.MoldDataNotesMemo;
            db.SaveChanges();
            //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.Notes.ToString(), GetAction.Update.ToString());

        }

        public ActionResult SaveNotes(tblMoldDataNotes model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            if (model.MoldDataNotesAutoID == 0)
            {
                model.MoldDataNotesMemo = string.IsNullOrWhiteSpace(model.MoldDataNotesMemo) ? null : model.MoldDataNotesMemo;
                model.CompanyID = CID;
                db.TblMoldDataNotes.Add(model);
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.Notes.ToString(), GetAction.Create.ToString());

            }

            var data1 = db.TblMoldDataNotes.Where(x => x.MoldDataID == model.MoldDataID && x.CompanyID == CID).ToList();
            return PartialView("_GetNotesData", data1);
        }


        public ActionResult DeleteNotes(string str, int MoldID)
        {
            if (str != "")
            {

                str = str.Substring(0, str.LastIndexOf(','));
                using (var ctx = new MoldtraxDbContext())
                {
                    SqlParameter sp = new SqlParameter("@value", str);
                    var result = ctx.Database.ExecuteSqlCommand("exec procDeleteNotes @value", sp);
                }

                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.Notes.ToString(), GetAction.Delete.ToString());
            }

            int CID = ShrdMaster.Instance.GetCompanyID();

            var data = db.TblMoldDataNotes.Where(x => x.MoldDataID == MoldID && x.CompanyID == CID).ToList();
            return PartialView("_GetNotesData", data);
        }

        #endregion

        #region Tech Tips

        public ActionResult GetTechTips(int ID=0)
        {
            var NewDB = new MoldtraxDbContext();
            int i = ShrdMaster.Instance.ReturnSelectedMoldID(ID);
            var data = NewDB.TblTechTips.Where(x => x.MoldDataID == i).FirstOrDefault();
            ViewBag.MoldDataID = i;
            HttpCookie nameCookie = new HttpCookie("SelectedMoldID");
            nameCookie.Values["SelectedMoldID"] = i.ToString();
            Response.Cookies.Add(nameCookie);
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
            return PartialView("_GetTechTips", data == null ? new tblTechTips() : data);
            //return View(data);
        }

        public ActionResult TechTipsMoldSpec(int ID=0)
        {
            int i = ShrdMaster.Instance.ReturnSelectedMoldID(ID);
            var data = db.TblTechTips.Where(x => x.MoldDataID == i).FirstOrDefault();
            return PartialView("_TechTipsMoldSpecData", data);
        }

        public List<tblDocs> GetTTLinksList(int MoldID)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            var data = db.TblDocs.Where(X=> X.DocMoldID == MoldID && X.CompanyID == CID).ToList();
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

        public ActionResult AddTTLinks()
        {
            CommonTTLinkFunc();
            return PartialView("_AddTTLinks");
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
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.TechTips.ToString(), GetAction.Create.ToString());


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
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.TechTips.ToString(), GetAction.Update.ToString());

            }
            //return PartialView("_GetTechTips", model);
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public void DeleteFile(string Path)
        {
          System.IO.File.Delete(Path);
        }
        
        public ActionResult UplaodTTPartImage(int TTID=0, int MoldDataID=0)
        {
            string fname="";
            string fname1 = "";
            string IMGName = "";
            string UniqueName = ShrdMaster.Instance.ReturnUniqueName();
            var NewDB = new MoldtraxDbContext();

            try
            {
                tblTechTips tt = new tblTechTips();
                //  Get all files from Request object 
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                    {
                        string[] testfiles = file.FileName.Split(new char[] { '\\' });
                        fname = testfiles[testfiles.Length - 1];
                    }
                    else
                    {
                        fname = "PartImage_" + UniqueName + "_" + file.FileName;
                        fname1 = "PartImage_" + UniqueName + "_" + file.FileName;
                        IMGName = "PartImage_"+ UniqueName + "_" + file.FileName;
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

                    NewDB.TblTechTips.Add(tt);
                    NewDB.SaveChanges();
                    //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.TechTips.ToString(), GetAction.Create.ToString());

                    tt.TTPartImagePath = Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/SiteImages/TechTipsImages/" + fname1);
                    return Json(tt, JsonRequestBehavior.AllowGet);
                }

                else
                {
                    var data = NewDB.TblTechTips.Where(x => x.TechTipsID == TTID).FirstOrDefault();
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
                    NewDB.SaveChanges();
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
            var NewDB = new MoldtraxDbContext();

            try
            {

                //  Get all files from Request object 
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];

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
                    NewDB.TblTechTips.Add(tt);
                    NewDB.SaveChanges();

                    //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.TechTips.ToString(), GetAction.Create.ToString());

                    tt.TTMoldImagePath = Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/SiteImages/TechTipsImages/" + fname1);
                    return Json(tt.TechTipsID, JsonRequestBehavior.AllowGet);
                }

                else
                {
                    var data = NewDB.TblTechTips.Where(x => x.TechTipsID == TTID).FirstOrDefault();

                    if (data.TTMoldImagePath != null)
                    { 
                        string path = Path.Combine(Server.MapPath("~/SiteImages/TechTipsImages/"), data.TTMoldImagePath);
                        DeleteFile(path);
                    }

                    data.TTMoldImagePath = fname1;
                    NewDB.SaveChanges();
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

        public ActionResult DeleteTTPartImage(int TTID=0)
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
                int CID = ShrdMaster.Instance.GetCompanyID();
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
            }

            return Json("ok");

        }

        public ActionResult TechTipsHotRunner(int ID=0)
        {
            int i = ShrdMaster.Instance.ReturnSelectedMoldID(ID);
            var data = db.TblTechTips.Where(x => x.MoldDataID == i).FirstOrDefault();
            return PartialView("_TTHotRunner",data);
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
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.TechTips.ToString(), GetAction.Create.ToString());

            }

            else
            {
                var data = db.TblTechTips.Where(x => x.TechTipsID == model.TechTipsID).FirstOrDefault();
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
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.TechTips.ToString(), GetAction.Update.ToString());

            }

            return RedirectToAction("Index");
        }

        public ActionResult TTLinksGetData(int ID=0)
        {
            CommonTTLinkFunc();

            int i = ShrdMaster.Instance.ReturnSelectedMoldID(ID);
            GetMolDataID(i);

            var data = GetTTLinksList(i);
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
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.TechTips.ToString(), GetAction.Update.ToString());

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
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.TechTipsLink.ToString(), GetAction.Create.ToString());

            }
            else
            {
                var data = db.TblDocs.Where(s => s.DocID == model.DocID).FirstOrDefault();
                data.DocSection = model.DocSection;
                data.DocName = model.DocName;
                data.DocLink = model.DocLink;
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.TechTipsLink.ToString(), GetAction.Update.ToString());

            }

            int i = ShrdMaster.Instance.ReturnSelectedMoldID(model.DocMoldID);
            GetMolDataID(i);
            var data12 = GetTTLinksList(i);
            CommonTTLinkFunc();
            return PartialView("_TTLinks", data12);
            //return Json("ok", JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteTTLinks(string str, int MoldID=0)
        {
            if (str != "")
            {
                str = str.Substring(0, str.LastIndexOf(','));
                using (var ctx = new MoldtraxDbContext())
                {
                    SqlParameter sp = new SqlParameter("@value", str);
                    var result = ctx.Database.ExecuteSqlCommand("exec procDeletetTTLinks @value", sp);
                }
            }

            CommonTTLinkFunc();

            int i = ShrdMaster.Instance.ReturnSelectedMoldID(MoldID);
            GetMolDataID(i);

            var data = GetTTLinksList(i);
            return PartialView("_TTLinks", data);
        }

        #endregion


        #region IMLMAP

        public ActionResult IMLImageLoad(int ID=0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            int i = ShrdMaster.Instance.ReturnSelectedMoldID(ID);
            var data = db.TblMoldData.Where(x => x.MoldDataID == i).FirstOrDefault();
            data.CompanyID = CID;
            if (data != null)
            {
                data.MoldMapPath = data.MoldMapPath != null ? Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/SiteImages/IMLImg/" + data.MoldMapPath) : null;
            }

            GetMolDataID(i);
            return PartialView("_IMLMap", data == null ? new tblMoldData() : data);
        }

        public ActionResult SaveIMLImg(int MoldID=0)
        {
            string fname = "";
            string fname1 = "";
            string ImgName = "";
            string UniqueName = ShrdMaster.Instance.ReturnUniqueName();
            var data = db.TblMoldData.Where(x => x.MoldDataID == MoldID).FirstOrDefault();
            int CID = ShrdMaster.Instance.GetCompanyID();

            try
            {
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
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
                    fname = Path.Combine(Server.MapPath("~/SiteImages/IMLImg/"), fname);
                    file.SaveAs(fname);

                    if (FileExtension == ".jpg" || FileExtension == ".jpeg" || FileExtension == ".png")
                    {
                        data.MoldMapPath = ImgName;
                        db.SaveChanges();
                    }
                    else
                    {
                        var FileP = ShrdMaster.Instance.ConvertVisioToImg(fname);
                        data.MoldMapPath = FileP;
                    }
                }
                GetMolDataID(data.MoldDataID);
                data.MoldMapPath = data.MoldMapPath != null ? Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/SiteImages/IMLImg/" + data.MoldMapPath) : null;
            }

            catch (Exception ex)
            {

            }
            return PartialView("_IMLMap", data);

        }

        [HttpPost]
        public ActionResult DeleteIMLIML(int MoldID=0)
        {
            if (MoldID != 0)
            {
                var data = db.TblMoldData.Where(x => x.MoldDataID == MoldID).FirstOrDefault();

                if (data.MoldMapPath != null)
                {
                    string path = Path.Combine(Server.MapPath("~/SiteImages/IMLImg/"), data.MoldMapPath);
                    DeleteFile(path);
                }

                data.MoldMapPath = null;
                db.SaveChanges();
            }

            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region Servicing

        public ActionResult GetServicingData(int ID=0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

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

            int i = ShrdMaster.Instance.ReturnSelectedMoldID(ID);
            var data = db.TblMoldData.Where(x => x.MoldDataID == i).FirstOrDefault();
            return PartialView("_ServicingGetData", data == null ? new tblMoldData() : data);
        }

        public ActionResult SaveCopyServicing(tblMoldData model, int MID)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            if (MID != 0)
            {
                var data = db.TblMoldData.Where(x => x.MoldDataID == MID).FirstOrDefault();
                data.MoldService01 = string.IsNullOrWhiteSpace(model.MoldService01) ? null : model.MoldService01;
                data.MoldService02 = string.IsNullOrWhiteSpace(model.MoldService02) ? null : model.MoldService02;
                data.MoldService03 = string.IsNullOrWhiteSpace(model.MoldService03) ? null : model.MoldService03;
                data.MoldService04 = string.IsNullOrWhiteSpace(model.MoldService04) ? null : model.MoldService04;
                data.MoldService05 = string.IsNullOrWhiteSpace(model.MoldService05) ? null : model.MoldService05;
                data.MoldService06 = string.IsNullOrWhiteSpace(model.MoldService06) ? null : model.MoldService06;
                data.MoldService07 = string.IsNullOrWhiteSpace(model.MoldService07) ? null : model.MoldService07;
                data.MoldService08 = string.IsNullOrWhiteSpace(model.MoldService08) ? null : model.MoldService08;
                data.MoldService09 = string.IsNullOrWhiteSpace(model.MoldService09) ? null : model.MoldService09;
                data.MoldService10 = string.IsNullOrWhiteSpace(model.MoldService10) ? null : model.MoldService10;

                data.MoldDefect01 = string.IsNullOrWhiteSpace(model.MoldDefect01) ? null : model.MoldDefect01;
                data.MoldDefect02 = string.IsNullOrWhiteSpace(model.MoldDefect02) ? null : model.MoldDefect02;
                data.MoldDefect03 = string.IsNullOrWhiteSpace(model.MoldDefect03) ? null : model.MoldDefect03;
                data.MoldDefect04 = string.IsNullOrWhiteSpace(model.MoldDefect04) ? null : model.MoldDefect04;
                data.MoldDefect05 = string.IsNullOrWhiteSpace(model.MoldDefect05) ? null : model.MoldDefect05;
                data.MoldDefect06 = string.IsNullOrWhiteSpace(model.MoldDefect06) ? null : model.MoldDefect06;
                data.MoldDefect07 = string.IsNullOrWhiteSpace(model.MoldDefect07) ? null : model.MoldDefect07;
                data.MoldDefect08 = string.IsNullOrWhiteSpace(model.MoldDefect08) ? null : model.MoldDefect08;
                data.MoldDefect09 = string.IsNullOrWhiteSpace(model.MoldDefect09) ? null : model.MoldDefect09;
                data.MoldDefect10 = string.IsNullOrWhiteSpace(model.MoldDefect10) ? null : model.MoldDefect10;

                data.MoldReasonPulled01 = string.IsNullOrWhiteSpace(model.MoldReasonPulled01) ? null : model.MoldReasonPulled01;
                data.MoldReasonPulled02 = string.IsNullOrWhiteSpace(model.MoldReasonPulled02) ? null : model.MoldReasonPulled02;
                data.MoldReasonPulled03 = string.IsNullOrWhiteSpace(model.MoldReasonPulled03) ? null : model.MoldReasonPulled03;
                data.MoldReasonPulled04 = string.IsNullOrWhiteSpace(model.MoldReasonPulled04) ? null : model.MoldReasonPulled04;
                data.MoldReasonPulled05 = string.IsNullOrWhiteSpace(model.MoldReasonPulled05) ? null : model.MoldReasonPulled05;
                data.MoldReasonPulled06 = string.IsNullOrWhiteSpace(model.MoldReasonPulled06) ? null : model.MoldReasonPulled06;
                data.MoldReasonPulled07 = string.IsNullOrWhiteSpace(model.MoldReasonPulled07) ? null : model.MoldReasonPulled07;
                data.MoldReasonPulled08 = string.IsNullOrWhiteSpace(model.MoldReasonPulled08) ? null : model.MoldReasonPulled08;
                data.MoldReasonPulled09 = string.IsNullOrWhiteSpace(model.MoldReasonPulled09) ? null : model.MoldReasonPulled09;
                data.MoldReasonPulled10 = string.IsNullOrWhiteSpace(model.MoldReasonPulled10) ? null : model.MoldReasonPulled10;

                db.SaveChanges();

                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.Servicing.ToString(), GetAction.Copy.ToString());

            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveServicing(tblMoldData model)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            try
            {
                var data = db.TblMoldData.Where(x => x.MoldDataID == model.MoldDataID).FirstOrDefault();
                data.MoldService01 = string.IsNullOrWhiteSpace(model.MoldService01) ? null : model.MoldService01;
                data.MoldService02 = string.IsNullOrWhiteSpace(model.MoldService02) ? null : model.MoldService02;
                data.MoldService03 = string.IsNullOrWhiteSpace(model.MoldService03) ? null : model.MoldService03;
                data.MoldService04 = string.IsNullOrWhiteSpace(model.MoldService04) ? null : model.MoldService04;
                data.MoldService05 = string.IsNullOrWhiteSpace(model.MoldService05) ? null : model.MoldService05;
                data.MoldService06 = string.IsNullOrWhiteSpace(model.MoldService06) ? null : model.MoldService06;
                data.MoldService07 = string.IsNullOrWhiteSpace(model.MoldService07) ? null : model.MoldService07;
                data.MoldService08 = string.IsNullOrWhiteSpace(model.MoldService08) ? null : model.MoldService08;
                data.MoldService09 = string.IsNullOrWhiteSpace(model.MoldService09) ? null : model.MoldService09;
                data.MoldService10 = string.IsNullOrWhiteSpace(model.MoldService10) ? null : model.MoldService10;

                data.MoldDefect01 = string.IsNullOrWhiteSpace(model.MoldDefect01) ? null : model.MoldDefect01;
                data.MoldDefect02 = string.IsNullOrWhiteSpace(model.MoldDefect02) ? null : model.MoldDefect02;
                data.MoldDefect03 = string.IsNullOrWhiteSpace(model.MoldDefect03) ? null : model.MoldDefect03;
                data.MoldDefect04 = string.IsNullOrWhiteSpace(model.MoldDefect04) ? null : model.MoldDefect04;
                data.MoldDefect05 = string.IsNullOrWhiteSpace(model.MoldDefect05) ? null : model.MoldDefect05;
                data.MoldDefect06 = string.IsNullOrWhiteSpace(model.MoldDefect06) ? null : model.MoldDefect06;
                data.MoldDefect07 = string.IsNullOrWhiteSpace(model.MoldDefect07) ? null : model.MoldDefect07;
                data.MoldDefect08 = string.IsNullOrWhiteSpace(model.MoldDefect08) ? null : model.MoldDefect08;
                data.MoldDefect09 = string.IsNullOrWhiteSpace(model.MoldDefect09) ? null : model.MoldDefect09;
                data.MoldDefect10 = string.IsNullOrWhiteSpace(model.MoldDefect10) ? null : model.MoldDefect10;

                data.MoldReasonPulled01 = string.IsNullOrWhiteSpace(model.MoldReasonPulled01) ? null : model.MoldReasonPulled01;
                data.MoldReasonPulled02 = string.IsNullOrWhiteSpace(model.MoldReasonPulled02) ? null : model.MoldReasonPulled02;
                data.MoldReasonPulled03 = string.IsNullOrWhiteSpace(model.MoldReasonPulled03) ? null : model.MoldReasonPulled03;
                data.MoldReasonPulled04 = string.IsNullOrWhiteSpace(model.MoldReasonPulled04) ? null : model.MoldReasonPulled04;
                data.MoldReasonPulled05 = string.IsNullOrWhiteSpace(model.MoldReasonPulled05) ? null : model.MoldReasonPulled05;
                data.MoldReasonPulled06 = string.IsNullOrWhiteSpace(model.MoldReasonPulled06) ? null : model.MoldReasonPulled06;
                data.MoldReasonPulled07 = string.IsNullOrWhiteSpace(model.MoldReasonPulled07) ? null : model.MoldReasonPulled07;
                data.MoldReasonPulled08 = string.IsNullOrWhiteSpace(model.MoldReasonPulled08) ? null : model.MoldReasonPulled08;
                data.MoldReasonPulled09 = string.IsNullOrWhiteSpace(model.MoldReasonPulled09) ? null : model.MoldReasonPulled09;
                data.MoldReasonPulled10 = string.IsNullOrWhiteSpace(model.MoldReasonPulled10) ? null : model.MoldReasonPulled10;

                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.Servicing.ToString(), GetAction.Update.ToString());



            }

            catch (Exception ex)
            {

            }
            
            return Json("", JsonRequestBehavior.AllowGet);
        }

        #endregion


        #region Layout

        public List<tblCavityLocation> ReturnCavityLocation(int MoldID)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            var data = db.TblCavityLocations.Where(X=> X.MoldDataID == MoldID && X.CompanyID == CID).ToList();
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

            Num.AddRange(String);
            return Num;
        }

        public ActionResult GetLayoutData(int ID=0)
        {
            int i = ShrdMaster.Instance.ReturnSelectedMoldID(ID);

            var tblLocation = ReturnCavityLocation(i);

            ViewBag.MoldDataID = i;
            int u = 0;
            if (tblLocation != null)
            {
                u = tblLocation.Select(x=> x.CavityLocationID).FirstOrDefault();
            }

            int CID = ShrdMaster.Instance.GetCompanyID();


            var tblNumber = db.TblCavityNumbers.Where(x => x.CavityLocationID == u && x.CompanyID == CID).OrderByDescending(x=> x.CavityActive).ThenBy(x=> x.CavityNumber).ToList();

            LayoutData LD = new LayoutData();
            LD.tblCavityLocations = tblLocation.ToList();
            LD.tblCavityNumber = tblNumber;
            return PartialView("_MoldLayout", LD);
        }

        public ActionResult CreateTbLCavityNumber()
        {
            return PartialView("_AddTBLCavityLocation");
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

        public ActionResult GetCavityNumberList(int CavityID=0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            var data = db.TblCavityNumbers.Where(x => x.CavityLocationID == CavityID && x.CompanyID == CID).OrderByDescending(x => x.CavityActive).ThenBy(x => x.CavityNumber).ToList();
            return PartialView("_tblCavityNumberData", data);
        }

        public List<tblCavityLocation> ReturntblCavNumber(List<tblCavityLocation> Cav)
        {
            List<tblCavityLocation> Num = new List<tblCavityLocation>();

            foreach (var x in Cav)
            {
                if (Regex.IsMatch(x.CavityLocationNumber, @"^\d+$"))
                {
                    Num.Add(x);
                }
            }

            Num = Num.ToList();
            return Num;
        }

        public int ReturnLastCavityLocaation(List<tblCavityLocation> Cav)
        {
            int ReturnCavNum = 0;
            List<tblCavityLocation> Num = new List<tblCavityLocation>();

            foreach (var x in Cav)
            {
                if (Regex.IsMatch(x.CavityLocationNumber, @"^\d+$"))
                {
                    Num.Add(x);
                }
            }

            if (Num.Count != 0)
            {
                Num = Num.OrderBy(x => x.CavityLocationNumber.Length).ThenBy(x => x.CavityLocationNumber).ToList();
                ReturnCavNum = Convert.ToInt32(Num.LastOrDefault().CavityLocationNumber);
            }

            return ReturnCavNum;
        }

        public ActionResult UpdateCavityLocation(int MoldID=0, int CavityNo=0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            var MoldD = db.TblMoldData.Where(x => x.MoldDataID == MoldID && x.CompanyID == CID).FirstOrDefault();
            MoldD.CavityTotal = CavityNo;
            db.SaveChanges();

            var tblC = db.TblCavityLocations.Where(x => x.MoldDataID == MoldID && x.CompanyID == CID).ToList();

            int CavN = 0;
            int CavNumInt = Convert.ToInt32(ReturnLastCavityLocaation(tblC));
            if (CavNumInt != null)
            {
                CavN = CavNumInt;
            }

            if (CavityNo > CavN)
            {
                if (tblC.Count() != 0)
                {
                    var asd = ReturnLastCavityLocaation(tblC);

                    int LastNo = asd;

                    for (int i =Convert.ToInt32(LastNo) + 1; i <= CavityNo; i++)
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

                    //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.CavityLayout.ToString(), GetAction.Create.ToString());

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

                    //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.CavityLayout.ToString(), GetAction.Create.ToString());

                }
            }

            var tblLocation = ReturnCavityLocation(MoldID);

            int u = 0;
            if (tblLocation != null)
            {
                u = tblLocation.Select(x => x.CavityLocationID).FirstOrDefault();
            }

            var tblNumber = db.TblCavityNumbers.Where(x => x.CavityLocationID == u && x.CompanyID == CID).OrderByDescending(x => x.CavityActive).ThenBy(x => x.CavityNumber).ToList();

            LayoutData LD = new LayoutData();
            LD.tblCavityLocations = tblLocation;
            LD.tblCavityNumber = tblNumber;

            return PartialView("_MoldLayout", LD);
        }

        public ActionResult CreateMoldPosition(string Data, int MoldID=0)
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

            //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.CavityLayout.ToString(), GetAction.Create.ToString());


            var tblLocation = ReturnCavityLocation(MoldID);

            int u = 0;
            if (tblLocation != null)
            {
                u = tblLocation.Select(x => x.CavityLocationID).FirstOrDefault();
            }

            var tblNumber = db.TblCavityNumbers.Where(x => x.CavityLocationID == u).OrderByDescending(x => x.CavityActive).ThenBy(x => x.CavityNumber).ToList();
            ViewBag.MoldDataID = MoldID;
            LayoutData LD = new LayoutData();
            LD.tblCavityLocations = tblLocation;
            LD.tblCavityNumber = tblNumber;

            return PartialView("_MoldLayout", LD);
        }

        public ActionResult DeleteSelectedMoldPosition(int ID=0, int MoldID=0)
        {
            var data = db.TblCavityLocations.Where(x => x.CavityLocationID == ID).FirstOrDefault();
            db.TblCavityLocations.Remove(data);
            db.SaveChanges();

            var data1 = db.TblCavityNumbers.Where(x => x.CavityLocationID == ID).ToList();
            foreach (var x in data1)
            {
                db.TblCavityNumbers.Remove(x);
                db.SaveChanges();
            }

            //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.CavityLayout.ToString(), GetAction.Delete.ToString());


            var tblLocation = ReturnCavityLocation(MoldID);
            
            int u = 0;
            if (tblLocation != null)
            {
                u = tblLocation.Select(x => x.CavityLocationID).FirstOrDefault();
            }
            ViewBag.MoldDataID = MoldID;
            int CID = ShrdMaster.Instance.GetCompanyID();

            var tblNumber = db.TblCavityNumbers.Where(x => x.CavityLocationID == u && x.CompanyID == CID).OrderByDescending(x => x.CavityActive).ThenBy(x => x.CavityNumber).ToList();

            LayoutData LD = new LayoutData();
            LD.tblCavityLocations = tblLocation;
            LD.tblCavityNumber = tblNumber;

            return PartialView("_MoldLayout", LD);
        }
        


        public ActionResult DeleteAllMoldPosition(int MoldID = 0)
        {

            SqlCommand cmd = new SqlCommand("proc_DeleteAllMoldPosition", con);
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@MoldID", MoldID));
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();

            var tblLocation = ReturnCavityLocation(MoldID);
            int u = 0;
            if (tblLocation != null)
            {
                u = tblLocation.Select(x => x.CavityLocationID).FirstOrDefault();
            }
            int CID = ShrdMaster.Instance.GetCompanyID();

            var tblNumber = db.TblCavityNumbers.Where(x => x.CavityLocationID == u && x.CompanyID == CID).OrderByDescending(x => x.CavityActive).ThenBy(x => x.CavityNumber).ToList();
            ViewBag.MoldDataID = MoldID;
            LayoutData LD = new LayoutData();
            LD.tblCavityLocations = tblLocation;
            LD.tblCavityNumber = tblNumber;

            return PartialView("_MoldLayout", LD);
        }

        public void EditCavityNumFocusOut(tblCavityNumber model)
        {
            try
            {
                int CID = ShrdMaster.Instance.GetCompanyID();
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.CavityLayout.ToString(), GetAction.Update.ToString());

            }
            catch (Exception ex)
            {

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
                dd.CompanyID = CID;
                dd.MoldDataID = MoldID;
                db.TblCavityLocations.Add(dd);
                db.SaveChanges();
                tbl.CavityLocationID = dd.CavityLocationID;
                tbl.CavityNumber = CavityNum;
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.CavityLayout.ToString(), GetAction.Create.ToString());
            }

            if (tbl.CavityNumberID == 0)
            {
                tbl.CompanyID = CID;
                db.TblCavityNumbers.Add(tbl);
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.CavityLayout.ToString(), GetAction.Create.ToString());

            }


            var tblLocation = ReturnCavityLocation(MoldID);

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

        [HttpPost]
        public ActionResult EditCavityNum(tblCavityNumber model, int MoldID=0, string CavityNum="0")
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            if (model.CavityNumberID == 0)
            {
                model.CompanyID = CID;
                db.TblCavityNumbers.Add(model);
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.DetailMoldInfo.ToString(), GetTabName.CavityLayout.ToString(), GetAction.Update.ToString());

            }

            var tblLocation = ReturnCavityLocation(MoldID);

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
        #endregion
    }
}