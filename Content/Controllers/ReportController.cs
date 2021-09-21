using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Reporting.WebForms;
using Moldtrax.Models;
using Moldtrax.Providers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Moldtrax.Controllers
{
    [SessionExpireFilter]
    public class ReportController : Controller
    {
        private MoldtraxDbContext db = new MoldtraxDbContext();
        static string constring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        SqlConnection con = new SqlConnection(constring);
        // GET: Reports
        public ActionResult Index()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            ViewBag.Companylist = new SelectList(db.TblCompanies.ToList().OrderBy(x => x.CompanyName), "CompanyID", "CompanyName");
            var dd = db.TblMoldData.Where(X=> X.CompanyID == CID).ToList().Select(x => new tblMoldData { MoldDataID = x.MoldDataID, MoldName = x.MoldName + ": " + x.MoldDesc });

            var MoldData = db.TblMoldData.Where(X=> X.CompanyID == CID).ToList().OrderBy(x => x.MoldName);
            List<SelectListItem> Tech = new List<SelectListItem>();

            foreach (var x in MoldData)
            {
                Tech.Add(new SelectListItem
                {
                    Text = x.MoldName + ": " + x.MoldDesc,
                    Value = x.MoldDataID.ToString()
                });
            }

            ViewBag.MoldDataID = Tech;

            ViewBag.MoldText = new SelectList(dd.ToList().OrderBy(x => x.MoldName), "MoldDataID", "MoldName");

            try
            {
                var FasttraxDate = db.FastraxLastDates.FirstOrDefault();
                ViewBag.StartDate = FasttraxDate.StartDate;
                ViewBag.EndDate = FasttraxDate.EndDate;
            }
            catch (Exception ex)
            {

            }

            return View();
        }


        public void CompanyDrop()
        {
            ViewBag.Companylist = new SelectList(db.TblCompanies.ToList().OrderBy(x => x.CompanyName), "CompanyID", "CompanyName");
        }

        #region Statistical


        public ActionResult RepairSheet(string StartDate = "", string EndDate="" , int MOLDID = 0)
        {
            //if (StartDate == null || EndDate == null)
            //{
            //    StartDate = System.DateTime.Now;
            //    EndDate = System.DateTime.Now;
            //}
            //else
            //{

            //}
            CompanyDrop();

            string StDate = StartDate;
            string EDate = EndDate;

            if (StartDate != "" || EndDate != "")
            {
                StDate = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd hh:mm:ss");
                EDate = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd hh:mm:ss");
            }
            else
            {
                StDate = "";
                EDate = "";
            }


            //
            //var data = ShrdMaster.Instance.RepairSheetList(con, ShrdMaster.Instance.ReturnDate(StartDate), ShrdMaster.Instance.ReturnDate(EndDate), MOLDID);
            DataSet ds = new DataSet();
            var data = ShrdMaster.Instance.RepairSheetList(con, StDate, EDate, MOLDID);
            var MoldConfig = db.TblDDMoldConfigs.ToList();
            var MoldConfig2 = db.TblDDMoldConfig2s.ToList();
            var StopReason = db.TblddStopReasons.ToList();
            var CorrectiveAction = db.TblDDTlCorrectiveActions.ToList();

            int Config = 0;
            int Config2 = 0;
            int MLDMaintreq = 0;
            int Corrective = 0;

            foreach (var x in data) // search whole table
            {
                x.MoldDefectMapPath = Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/SiteImages/MaintenanceInstruction/" + x.MoldDefectMapPath);
                if (x.MoldToolDescrip != null || x.MoldToolDescrip != "")
                {
                    string[] sds = Regex.Split(x.MoldToolDescrip, "</p>");
                    string s = HtmlToPlainText(sds[0]);

                    if (s != "")
                    {
                        s = s.Replace(System.Environment.NewLine, string.Empty);
                        x.MoldToolDescrip = s.Replace("&nbsp;", " ");
                    }
                }


                if (x.MoldConfig != "" && x.MoldConfig != null)
                {
                    Config = Convert.ToInt32(x.MoldConfig);
                }
                var dataConfig = MoldConfig.Where(c => c.ID == Config).FirstOrDefault();
                x.MoldConfig = dataConfig != null ? dataConfig.MoldConfig : "";



                if (x.MoldConfig2 != "" && x.MoldConfig2 != null)
                {
                    Config2 = Convert.ToInt32(x.MoldConfig2);
                }
                var dataConfig2 = MoldConfig2.Where(c => c.ID == Config2).FirstOrDefault();
                x.MoldConfig2 = dataConfig2 != null ? dataConfig2.MoldConfig : "";


                if (x.MldPullMaintRequired != "" && x.MldPullMaintRequired != null)
                {
                    MLDMaintreq = Convert.ToInt32(x.MldPullMaintRequired);
                }
                var STreasin = StopReason.Where(c => c.ID == MLDMaintreq).FirstOrDefault();
                x.MldPullMaintRequired = STreasin != null ? STreasin.StopReason : "";


                if (x.TlCorrectiveAction != "" && x.TlCorrectiveAction != null)
                {
                    Corrective = Convert.ToInt32(x.TlCorrectiveAction);
                }

                var CorrectiveAct = CorrectiveAction.Where(c => c.ID == Corrective).FirstOrDefault();
                x.TlCorrectiveAction = CorrectiveAct != null ? CorrectiveAct.TlCorrectiveAction : "";



            }

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.LocalReport.EnableExternalImages = true;
            reportViewer.PageCountMode = PageCountMode.Actual;
            //var newpagesetting = new System.Drawing.Printing.PageSettings();
            //newpagesetting.Margins = new System.Drawing.Printing.Margins(10, 10, 10, 10);
            //reportViewer.SetPageSettings(newpagesetting);

            //reportViewer.Width = Unit.Percentage(500);
            //reportViewer.Height = Unit.Percentage(600);

            //SqlDataAdapter adp = new SqlDataAdapter("SELECT * FROM Employee_tbt", con);

            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\RepairSheet.rdlc";
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", data));

            ViewBag.ReportViewer = reportViewer;
            return View();
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



        public ActionResult MaintenanceTimeline(DateTime StartDate, DateTime EndDate, int MOLDID = 0)
        {
            CompanyDrop();
            string StDate = StartDate.ToString("yyyy-MM-dd hh:mm:ss");
            string EDate = EndDate.ToString("yyyy-MM-dd hh:mm:ss");

            var data = ShrdMaster.Instance.MaintenenceTimeline(con, StDate, EDate, MOLDID);

            var Config1 = db.TblDDMoldConfigs.ToList();
            var Config2 = db.TblDDMoldConfig2s.ToList();
            var StopReason = db.TblddStopReasons.ToList();
            var ProductPart = db.TblDDProductPart.ToList();
            var ProductLine = db.TblDDProductLine.ToList();


            int MoldCONFIG = 0;
            int MoldConfig2 = 0;
            int MLDMaintreq = 0;
            int ProductPartID = 0;
            int ProductLineID = 0;


            foreach (var x in data)
            {

                if (x.MoldConfig != "" && x.MoldConfig != null)
                {
                    MoldCONFIG = Convert.ToInt32(x.MoldConfig);
                }
                var MCon1 = Config1.Where(c => c.ID == MoldCONFIG).FirstOrDefault();
                x.MoldConfig = MCon1 != null ? MCon1.MoldConfig : "";

                if (x.MoldConfig2 != "" && x.MoldConfig2 != null)
                {
                    MoldConfig2 = Convert.ToInt32(x.MoldConfig2);
                }
                var MCon2 = Config2.Where(c => c.ID == MoldConfig2).FirstOrDefault();
                x.MoldConfig2 = MCon2 != null ? MCon2.MoldConfig : "";

                if (x.MldPullMaintRequired != "" && x.MldPullMaintRequired != null)
                {
                    MLDMaintreq = Convert.ToInt32(x.MldPullMaintRequired);
                }
                var STreasin = StopReason.Where(c => c.ID == MLDMaintreq).FirstOrDefault();
                x.MldPullMaintRequired = STreasin != null ? STreasin.StopReason : "";

                if (x.ProductPart != "" && x.ProductPart != null)
                {
                    ProductPartID = Convert.ToInt32(x.ProductPart);
                }
                var ProductPt = ProductPart.Where(c => c.ID == ProductPartID).FirstOrDefault();
                x.ProductPart = ProductPt != null ? ProductPt.ProductPart : "";


                if (x.ProductLine != "" && x.ProductLine != null)
                {
                    ProductLineID = Convert.ToInt32(x.ProductLine);
                }
                var ProductLn = ProductLine.Where(c => c.ID == ProductLineID).FirstOrDefault();
                x.ProductLine = ProductLn != null ? ProductLn.ProductLine : "";
            }

            DataSet ds = new DataSet();

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.PageCountMode = PageCountMode.Actual;

            //reportViewer.Width = Unit.Percentage(500);
            //reportViewer.Height = Unit.Percentage(600);

            //SqlDataAdapter adp = new SqlDataAdapter("SELECT * FROM Employee_tbt", con);

            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\MaintenanceTimelines.rdlc";
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", data));

            ViewBag.ReportViewer = reportViewer;
            return View();
        }

        public ActionResult DefectCostAnalysis(DateTime StartDate, DateTime EndDate, int MOLDID = 0)
        {
            string StDate = StartDate.ToString("yyyy-MM-dd hh:mm:ss");
            string EDate = EndDate.ToString("yyyy-MM-dd hh:mm:ss");

            var data = ShrdMaster.Instance.DefectCostAnalysis(con, StDate, EDate, MOLDID);

            CompanyDrop();

            var Config1 = db.TblDDMoldConfigs.ToList();
            var Config2 = db.TblDDMoldConfig2s.ToList();
            //var StopReason = db.TblddStopReasons.ToList();
            var ProductPart = db.TblDDProductPart.ToList();
            var ProductLine = db.TblDDProductLine.ToList();


            int MoldCONFIG = 0;
            int MoldConfig2 = 0;
            //int MLDMaintreq = 0;
            int ProductPartID = 0;
            int ProductLineID = 0;


            foreach (var x in data)
            {

                if (x.MoldConfig != "" && x.MoldConfig != null)
                {
                    MoldCONFIG = Convert.ToInt32(x.MoldConfig);
                }
                var MCon1 = Config1.Where(c => c.ID == MoldCONFIG).FirstOrDefault();
                x.MoldConfig = MCon1 != null ? MCon1.MoldConfig : "";

                if (x.MoldConfig2 != "" && x.MoldConfig2 != null)
                {
                    MoldConfig2 = Convert.ToInt32(x.MoldConfig2);
                }
                var MCon2 = Config2.Where(c => c.ID == MoldConfig2).FirstOrDefault();
                x.MoldConfig2 = MCon2 != null ? MCon2.MoldConfig : "";

                if (x.ProductPart != "" && x.ProductPart != null)
                {
                    ProductPartID = Convert.ToInt32(x.ProductPart);
                }

                var ProductPt = ProductPart.Where(c => c.ID == ProductPartID).FirstOrDefault();
                x.ProductPart = ProductPt != null ? ProductPt.ProductPart : "";


                if (x.ProductLine != "" && x.ProductLine != null)
                {
                    ProductLineID = Convert.ToInt32(x.ProductLine);
                }
                var ProductLn = ProductLine.Where(c => c.ID == ProductLineID).FirstOrDefault();
                x.ProductLine = ProductLn != null ? ProductLn.ProductLine : "";
            }


            DataSet ds = new DataSet();

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.PageCountMode = PageCountMode.Actual;

            //reportViewer.Width = Unit.Percentage(500);
            //reportViewer.Height = Unit.Percentage(600);

            //SqlDataAdapter adp = new SqlDataAdapter("SELECT * FROM Employee_tbt", con);

            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\DefectCostAnalysis.rdlc";
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", data));

            ViewBag.ReportViewer = reportViewer;
            return View();

        }

        public ActionResult DefectTracking(DateTime StartDate, DateTime EndDate, int MOLDID = 0)
        {
            string StDate = StartDate.ToString("yyyy-MM-dd hh:mm:ss");
            string EDate = EndDate.ToString("yyyy-MM-dd hh:mm:ss");

            CompanyDrop();

            var data = ShrdMaster.Instance.DefectTracking(con, StDate, EDate, MOLDID);
            //DataSet ds = new DataSet();
            var Config1 = db.TblDDMoldConfigs.ToList();
            var Config2 = db.TblDDMoldConfig2s.ToList();
            var ProductPart = db.TblDDProductPart.ToList();
            var ProductLine = db.TblDDProductLine.ToList();


            int MoldCONFIG = 0;
            int MoldConfig2 = 0;
            int ProductPartID = 0;
            int ProductLineID = 0;


            foreach (var x in data)
            {

                if (x.MoldConfig != "" && x.MoldConfig != null)
                {
                    MoldCONFIG = Convert.ToInt32(x.MoldConfig);
                }
                var MCon1 = Config1.Where(c => c.ID == MoldCONFIG).FirstOrDefault();
                x.MoldConfig = MCon1 != null ? MCon1.MoldConfig : "";

                if (x.MoldConfig2 != "" && x.MoldConfig2 != null)
                {
                    MoldConfig2 = Convert.ToInt32(x.MoldConfig2);
                }
                var MCon2 = Config2.Where(c => c.ID == MoldConfig2).FirstOrDefault();
                x.MoldConfig2 = MCon2 != null ? MCon2.MoldConfig : "";

                if (x.ProductPart != "" && x.ProductPart != null)
                {
                    ProductPartID = Convert.ToInt32(x.ProductPart);
                }
                var ProductPt = ProductPart.Where(c => c.ID == ProductPartID).FirstOrDefault();
                x.ProductPart = ProductPt != null ? ProductPt.ProductPart : "";


                if (x.ProductLine != "" && x.ProductLine != null)
                {
                    ProductLineID = Convert.ToInt32(x.ProductLine);
                }
                var ProductLn = ProductLine.Where(c => c.ID == ProductLineID).FirstOrDefault();
                x.ProductLine = ProductLn != null ? ProductLn.ProductLine : "";
            }


            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.PageCountMode = PageCountMode.Actual;

            //reportViewer.Width = Unit.Percentage(500);
            //reportViewer.Height = Unit.Percentage(600);

            //SqlDataAdapter adp = new SqlDataAdapter("SELECT * FROM Employee_tbt", con);

            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\DefectTracking2.rdlc";
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", data));

            ViewBag.ReportViewer = reportViewer;
            return View();

        }

        public ActionResult TotalTimeRun(DateTime StartDate, DateTime EndDate, int MOLDID = 0)
        {
            string StDate = StartDate.ToString("yyyy-MM-dd hh:mm:ss");
            string EDate = EndDate.ToString("yyyy-MM-dd hh:mm:ss");

            var data = ShrdMaster.Instance.TotalTimeRun(con, StDate, EDate, MOLDID);

            CompanyDrop();

            var Config1 = db.TblDDMoldConfigs.ToList();
            var Config2 = db.TblDDMoldConfig2s.ToList();
            var ProductPart = db.TblDDProductPart.ToList();
            var ProductLine = db.TblDDProductLine.ToList();


            int MoldCONFIG = 0;
            int MoldConfig2 = 0;
            int ProductPartID = 0;
            int ProductLineID = 0;


            foreach (var x in data)
            {

                if (x.MoldConfig != "" && x.MoldConfig != null)
                {
                    MoldCONFIG = Convert.ToInt32(x.MoldConfig);
                }
                var MCon1 = Config1.Where(c => c.ID == MoldCONFIG).FirstOrDefault();
                x.MoldConfig = MCon1 != null ? MCon1.MoldConfig : "";

                if (x.MoldConfig2 != "" && x.MoldConfig2 != null)
                {
                    MoldConfig2 = Convert.ToInt32(x.MoldConfig2);
                }
                var MCon2 = Config2.Where(c => c.ID == MoldConfig2).FirstOrDefault();
                x.MoldConfig2 = MCon2 != null ? MCon2.MoldConfig : "";

                if (x.ProductPart != "" && x.ProductPart != null)
                {
                    ProductPartID = Convert.ToInt32(x.ProductPart);
                }
                var ProductPt = ProductPart.Where(c => c.ID == ProductPartID).FirstOrDefault();
                x.ProductPart = ProductPt != null ? ProductPt.ProductPart : "";


                if (x.ProductLine != "" && x.ProductLine != null)
                {
                    ProductLineID = Convert.ToInt32(x.ProductLine);
                }
                var ProductLn = ProductLine.Where(c => c.ID == ProductLineID).FirstOrDefault();
                x.ProductLine = ProductLn != null ? ProductLn.ProductLine : "";
            }
            //DataSet ds = new DataSet();

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.PageCountMode = PageCountMode.Actual;

            //reportViewer.Width = Unit.Percentage(500);
            //reportViewer.Height = Unit.Percentage(600);

            //SqlDataAdapter adp = new SqlDataAdapter("SELECT * FROM Employee_tbt", con);

            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\TotalTimeRun2.rdlc";
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", data));

            ViewBag.ReportViewer = reportViewer;
            return View();

        }

        #endregion

        #region Reference


        public ActionResult IMLSheet(int MOLDID)
        {
            var data  = ShrdMaster.Instance.IMLSheet(con, MOLDID);
            foreach (DataRow dr in data.Rows) // search whole table
            {
                dr["MoldMapPath"] = Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/SiteImages/IMLImg/" + dr["MoldMapPath"]); //change the name
            }

            CompanyDrop();

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.LocalReport.EnableExternalImages = true;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\InjectionMoldLayoutSheet.rdlc";
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", data));

            //string path = new Uri(Server.MapPath("~/TroubleShooterImage")).AbsoluteUri;

            //var parameter = new ReportParameter[1];
            //parameter[0] = new ReportParameter("ImagePath", path);

            //reportViewer.LocalReport.SetParameters(parameter);

            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(80);
            reportViewer.Height = Unit.Percentage(80);
            reportViewer.PageCountMode = PageCountMode.Actual;

            ViewBag.ReportViewer = reportViewer;
            return View();
        }

        public ActionResult TroubleShooterGuides(int MOLDID)
        {
            var data = ShrdMaster.Instance.TSGuideRPTWrapper(con, MOLDID);

            CompanyDrop();

            List<TSGuideRPTWrapper> qryTSGuideRpt_Wrapper = new List<TSGuideRPTWrapper>();
            var TStype = db.TblDDTSType.ToList();
            int TSI = 0;

            if (data.Count() != 0)
            {
                foreach (var x in data)
                {
                    TSGuideRPTWrapper TS = new TSGuideRPTWrapper();
                    TS.TSSeqNum = x.TSSeqNum;
                    TS.TSGuide = x.TSGuide;
                    TS.MoldDataID = x.MoldDataID;
                    TS.TSDefects = x.TSDefects;
                    TS.TSExplanation = x.TSExplanation;
                    TS.TSToolInv = x.TSToolInv;
                    TS.TSProbCause = x.TSProbCause;
                    TS.TSSolution = x.TSSolution;
                    TS.ImagePath = Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/TroubleShooterImage/" + x.ImagePath);
                    if (x.TSType != null && x.TSType != "")
                    {
                        TSI = Convert.ToInt32(x.TSType);
                        var Tsdd = TStype.Where(c => c.ID == TSI).FirstOrDefault();
                        x.TSType = Tsdd != null ? Tsdd.TSType : "";
                    }
                    TS.TSPreventAction = x.TSPreventAction;
                    TS.MoldName = x.MoldName;
                    TS.MoldDesc = x.MoldDesc;
                    TS.CompanyCNTroubleShoot = x.CompanyCNTroubleShoot;
                    TS.CompanyName = x.CompanyName;
                    qryTSGuideRpt_Wrapper.Add(TS);
                }
            }

            else
            {
                TSGuideRPTWrapper TS = new TSGuideRPTWrapper();
                qryTSGuideRpt_Wrapper.Add(TS);
            }


            try
            {
                //var tblTSGuide_Local = db.TblTSGuide.ToList();
            
                //    var DS = qryTSGuideRpt_Wrapper.Join(tblTSGuide_Local,
            //        e1 => e1.TSGuide,
            //        e2 => e2.TSGuide,
            //        (e1, e2) => new
            //        {
            //            TSSeqNums = e1.TSSeqNum,
            //            TSGuides = e1.TSGuide,
            //            MoldDataIDs = e1.MoldDataID,
            //            TSDefectss = e1.TSDefects,
            //            TsExplanations = e1.TSExplanation,
            //            TSToolInvs = e1.TSToolInv,
            //            TSProbCauses = e1.TSProbCause,
            //            TSSolutions = e1.TSSolution,
            //            TSImage = e1.ImagePath,
            //            TSTypes = e1.TSType,
            //            TSPreventActions = e1.TSPreventAction,
            //            MoldNames = e1.MoldName,
            //            MoldDescs = e1.MoldDesc,
            //            CompanyNames = e1.CompanyName,
            //            CompanyCNTroubleShoots = e1.CompanyCNTroubleShoot
            //        }).ToList();
            }

            catch (Exception ex)
            {

            }
            

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.LocalReport.EnableExternalImages = true;
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\TroubleShootersGuide.rdlc";
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", qryTSGuideRpt_Wrapper.OrderBy(x=> x.TSType).ThenBy(x=> x.TSDefects)));

            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(80);
            reportViewer.Height = Unit.Percentage(80);
            reportViewer.PageCountMode = PageCountMode.Actual;

            ViewBag.ReportViewer = reportViewer;


            return View();
        }

        public ActionResult TechTipsReport(int MOLDID)
        {
            CompanyDrop();
            var data = ShrdMaster.Instance.TechTips(con, MOLDID);
            ReportViewer reportViewer = new ReportViewer();

            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\TechTips.rdlc";
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", data));

            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(80);
            reportViewer.Height = Unit.Percentage(80);
            reportViewer.PageCountMode = PageCountMode.Actual;

            ViewBag.ReportViewer = reportViewer;
            return View();
        }

        public ActionResult MoldTooling(int MOLDID)
        {
            var data = ShrdMaster.Instance.MoldTooling(con, MOLDID);
            CompanyDrop();
            var ProductPart = db.TblDDProductPart.ToList();
            var ProductLine = db.TblDDProductLine.ToList();
            var ToolingType = db.TblDDMoldToolingTypes.ToList();

            int ProductPartID = 0;
            int ProductLineID = 0;
            int ToolingID = 0;

            foreach (DataRow dr in data.Rows)
            {

                if (dr["ProductPart"].ToString() != "" && dr["ProductPart"] != null)
                {
                    ProductPartID = Convert.ToInt32(dr["ProductPart"]);
                }
                var ProductPt = ProductPart.Where(c => c.ID == ProductPartID).FirstOrDefault();
                dr["ProductPart"] = ProductPt != null ? ProductPt.ProductPart : "";


                if (dr["ProductLine"].ToString() != "" && dr["ProductLine"] != null)
                {
                    ProductLineID = Convert.ToInt32(dr["ProductLine"]);
                }
                var ProductLn = ProductLine.Where(c => c.ID == ProductLineID).FirstOrDefault();
                dr["ProductLine"] = ProductLn != null ? ProductLn.ProductLine : "";

                if (dr["MoldToolingType"].ToString() != "" && dr["MoldToolingType"] != null)
                {
                    ToolingID = Convert.ToInt32(dr["MoldToolingType"]);
                }
                var ToolingTypeVal = ToolingType.Where(c => c.ID == ToolingID).FirstOrDefault();
                dr["MoldToolingType"] = ToolingTypeVal != null ? ToolingTypeVal.DD_MoldToolingType : "";
                
    }


            ReportViewer reportViewer = new ReportViewer();

            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\MoldTooling.rdlc";
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", data));

            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(80);
            reportViewer.Height = Unit.Percentage(80);
            reportViewer.PageCountMode = PageCountMode.Actual;

            ViewBag.ReportViewer = reportViewer;
            return View();
        }


        public ActionResult LastShotInspection(int MOLDID)
        {
            CompanyDrop();
            var data = ShrdMaster.Instance.LastShot(con, MOLDID);

            foreach (DataRow dr in data.Rows) // search whole table
            {
                dr["TTPartImagePath"] = Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/SiteImages/TechTipsImages/" + dr["TTPartImagePath"]); //change the name
            }

            ReportViewer reportViewer = new ReportViewer();
            reportViewer.LocalReport.EnableExternalImages = true;

            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\LastShotRpt.rdlc";
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", data));

            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(80);
            reportViewer.Height = Unit.Percentage(80);
            reportViewer.PageCountMode = PageCountMode.Actual;

            ViewBag.ReportViewer = reportViewer;
            return View();
        }

        #endregion


        public ActionResult CallStartandEndDate(int MID = 0)
        {
            var data = ShrdMaster.Instance.StartEndDateReport(con, MID);
            var StartDate = data.GroupBy(x => new { x.SetDate, x.MoldDataID }).Where(p=> p.First().MoldDataID == MID).Select(s => new StartDate
            {
                SetDate = s.First().SetDate
            }).OrderByDescending(x => x.SetDate);

            CompanyDrop();

            var EndDate = data.GroupBy(x => new { x.MldPullDate, x.MoldDataID }).Where(p => p.First().MoldDataID == MID).Select(s => new StartDate
            {
                MldPullDate = s.First().MldPullDate
            }).OrderByDescending(x => x.MldPullDate);

            List<string> Sdate = new List<string>();
            List<string> Edate = new List<string>();

            foreach (var x in StartDate.OrderBy(x=> x.SetDate))
            {
                Sdate.Add(x.SetDate.ToShortDateString());
            }

            foreach (var x in EndDate.OrderBy(x=> x.MldPullDate))
            {
                Edate.Add(x.MldPullDate.ToShortDateString());
            }

            return Json(new { Sdate, Edate }, JsonRequestBehavior.AllowGet);
        }

        #region Fastrax Report

        public ActionResult CompleteMaintenanceTrackingAllMoldFastrax(string StDate, string EDate)
        {
            CompanyDrop();
            DataTable dt = new DataTable();
            string Procedure = "procCompleteMaintenanceTrackingAllMolds";
            dt = CompleteMaintenanceTrackingAllMold(Procedure, StDate, EDate);
           

            ViewBag.ReportName = "Complete Maintenance Tracking All Mold";
            return View(dt);
        }


        public ActionResult CorrectiveActionAnalysisFastrax(string StDate, string EDate)
        {
            CompanyDrop();
            DataTable dt = new DataTable();
            string Procedure = "proc_CorrectiveActionAnalysis";
            dt = CorrectiveActionAnalysis(Procedure, StDate, EDate);
            foreach (DataRow dr in dt.Rows) // search whole table
            {
                dr["Tooling Description"] = Regex.Replace(dr["Tooling Description"].ToString(), "<[^>]*>", string.Empty).Replace("&nbsp;", ""); //change the name
            }
            ViewBag.ReportName = "Corrective Action Analysis";
            return View(dt);
        }


        public ActionResult InventoryTrackingFastrax(string StDate, string EDate)
        {
            CompanyDrop();
            DataTable dt = new DataTable();
            string Procedure = "procInventoryTracking";
            dt = InventoryTracking(Procedure);
            foreach (DataRow dr in dt.Rows) // search whole table
            {
                dr["Description"] = Regex.Replace(dr["Description"].ToString(), "<[^>]*>", string.Empty).Replace("&nbsp;", "");
            }
            ViewBag.ReportName = "Inventory Tracking";
            return View(dt);
        }


        public ActionResult MoldListFastrax(string StDate, string EDate)
        {
            CompanyDrop();
            DataTable dt = new DataTable();
            string Procedure = "procMoldList";
            dt = MoldList(Procedure);
            ViewBag.ReportName = "Mold List";
            return View(dt);
        }

        public ActionResult DistinctMoldListFastrax(string StDate, string EDate)
        {
            CompanyDrop();
            DataTable dt = new DataTable();
            string Procedure = "procMoldList";
            dt = MoldList(Procedure);
            DataView view = new DataView(dt);
            DataTable distinctValues = new DataTable();
            distinctValues = view.ToTable(true, "Mold Name");

            ViewBag.ReportName = "Mold List";
            return View(dt);
        }


        #endregion

        public ActionResult ShowMoldtraxReport(string Name, DateTime StartDate, DateTime EndDate)
        {
            CompanyDrop();
            string StDate = StartDate.ToString("yyyy-MM-dd hh:mm:ss");
            string EDate = EndDate.ToString("yyyy-MM-dd hh:mm:ss");
            DataTable dt = new DataTable();

            var FastraxDate = db.FastraxLastDates.FirstOrDefault();
            if (FastraxDate != null)
            {
                FastraxDate.StartDate = StartDate;
                FastraxDate.EndDate = EndDate;
                db.SaveChanges();
            }
            else
            {
                FastraxLastDate FD = new FastraxLastDate();
                FD.StartDate = StartDate;
                FD.EndDate = EndDate;
                db.FastraxLastDates.Add(FD);
                db.SaveChanges();
            }


            string Procedure = "";
            switch (Name)
            {
                case "Complete Maintenance Tracking All Mold":
                    //Procedure = "procCompleteMaintenanceTrackingAllMolds";
                    //dt = CompleteMaintenanceTrackingAllMold(Procedure, StDate, EDate);
                    return RedirectToAction("CompleteMaintenanceTrackingAllMoldFastrax", new { StDate, EDate }); 

                case "Corrective Action Analysis":
                    //Procedure = "proc_CorrectiveActionAnalysis";
                    //dt = CorrectiveActionAnalysis(Procedure, StDate, EDate);
                    //foreach (DataRow dr in dt.Rows) // search whole table
                    //{
                    //    dr["Tooling Description"] = Regex.Replace(dr["Tooling Description"].ToString(), "<[^>]*>", string.Empty).Replace("&nbsp;", ""); //change the name
                    //}
                    //break;
                    return RedirectToAction("CorrectiveActionAnalysisFastrax", new { StDate, EDate }); 

                case "Mold Performance Report":
                    Procedure = "procCostsPerRunTimeHour";
                    return RedirectToAction("CostsPerRunTimeCon", "MoldPerformanceDashboard", new { StDate, EDate });
                    //dt = ShrdMaster.Instance.ShowMOLDTRAXReport(con, Procedure, StDate, EDate);
                    //break;

                case "Defect Position Analysis":
                    Procedure = "procDefectPositionAnalysis";
                    return RedirectToAction("DefectDiscoveredbyTech", "MaintenanceEfficiencyDashboard", new {StartDate = StDate, EndDate= EDate });
                    //dt = ShrdMaster.Instance.ShowMOLDTRAXReport(con, Procedure, StDate, EDate);
                    //break;

                case "Defects by Mold Block and Quality":
                    Procedure = "procDefectsByMoldBlockAndQuality";
                    return RedirectToAction("MoldBLockandQuality", "MoldPerformanceDashboard", new { StartDate = StDate, EndDate = EDate, MID=-1 });

                    //dt = ShrdMaster.Instance.ShowMOLDTRAXReport(con, Procedure, StDate, EDate);
                    //break;

                case "Inventory Tracking":
                    //Procedure = "procInventoryTracking";
                    //dt = InventoryTracking(Procedure);
                    //foreach (DataRow dr in dt.Rows) // search whole table
                    //{
                    //    dr["Description"] = Regex.Replace(dr["Description"].ToString(), "<[^>]*>", string.Empty).Replace("&nbsp;", "");
                    //}
                    //break;
                    return RedirectToAction("InventoryTrackingFastrax", new { StDate, EDate });


                case "Maintenance Alert":
                    Procedure = "procMaintenanceAlert";
                    return RedirectToAction("ShowViewData", "MaintenanceAlertStat");

                case "Mold List":
                    //Procedure = "procMoldList";
                    //dt = MoldList(Procedure); 
                    //break;
                    return RedirectToAction("MoldListFastrax", new { StDate, EDate });

                case "Mold Stop Reason Costs":
                    Procedure = "procMoldStopReasonCosts";
                    return RedirectToAction("GetStopReasonCosts", "MoldPerformanceDashboard", new { StartDate = StDate, EndDate = EDate });
                    //dt = ShrdMaster.Instance.ShowMOLDTRAXReport(con, Procedure, StDate, EDate);
                    //break;

                case "Mold Tooling Costs":
                    Procedure = "procMoldToolingCosts";
                    return RedirectToAction("MoldToolingCostsContr", "MoldPerformanceDashboard", new {StDate, EDate });

            }
            ViewBag.ReportName = Name;
            //var data = ShrdMaster.Instance.ShowMOLDTRAXReport(con, Procedure, StDate, EDate);
            return View(dt);
        }

        public DataTable CompleteMaintenanceTrackingAllMold(string Procedure, string StDate, string EDate)
        {
            CompanyDrop();
            DataTable dt = new DataTable();
            dt = ShrdMaster.Instance.ShowMOLDTRAXReport(con, Procedure, StDate, EDate);

            dt.Columns.Add("New Start Date", typeof(string));
            dt.Columns.Add("New Start Time", typeof(string));
            dt.Columns.Add("New Stop Date", typeof(string));
            dt.Columns.Add("New Stop Time", typeof(string));
            dt.Columns.Add("New Repair Date", typeof(string));
            dt.Columns.Add("New Run Time  Hours", typeof(string));
            dt.Columns.Add("New Actual", typeof(string));
            dt.Columns.Add("New Adjust", typeof(string));


            for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
            {
                //Start Date
                if (!(dt.Rows[rowIndex]["Start Date"] is DBNull))
                {
                    dt.Rows[rowIndex]["New Start Date"] = Convert.ToDateTime(dt.Rows[rowIndex]["Start Date"]).ToShortDateString();
                }
                else
                {
                    dt.Rows[rowIndex]["New Start Date"] = null;
                }

                //Start Time
                if (!(dt.Rows[rowIndex]["Start Time"] is DBNull))
                {
                    dt.Rows[rowIndex]["New Start Time"] = Convert.ToDateTime(dt.Rows[rowIndex]["Start Time"]).ToShortTimeString();
                }
                else
                {
                    dt.Rows[rowIndex]["New Start Time"] = null;
                }

                //Stop Date
                if (!(dt.Rows[rowIndex]["Stop Date"] is DBNull))
                {
                    dt.Rows[rowIndex]["New Stop Date"] = Convert.ToDateTime(dt.Rows[rowIndex]["Stop Date"]).ToShortDateString();
                }
                else
                {
                    dt.Rows[rowIndex]["New Stop Date"] = null;
                }

                if (!(dt.Rows[rowIndex]["Stop Date"] is DBNull))
                {
                    dt.Rows[rowIndex]["New Stop Date"] = Convert.ToDateTime(dt.Rows[rowIndex]["Stop Date"]).ToShortDateString();
                }
                else
                {
                    dt.Rows[rowIndex]["New Stop Date"] = null;
                }

                //Stop Time
                if (!(dt.Rows[rowIndex]["Stop Time"] is DBNull))
                {
                    dt.Rows[rowIndex]["New Stop Time"] = Convert.ToDateTime(dt.Rows[rowIndex]["Stop Time"]).ToShortTimeString();
                }
                else
                {
                    dt.Rows[rowIndex]["New Stop Time"] = null;
                }

                //Repair Date

                if (!(dt.Rows[rowIndex]["Repair Date"] is DBNull))
                {
                    dt.Rows[rowIndex]["New Repair Date"] = Convert.ToDateTime(dt.Rows[rowIndex]["Repair Date"]).ToShortDateString();
                }
                else
                {
                    dt.Rows[rowIndex]["New Repair Date"] = null;
                }

                //Run time Hours

                if (!(dt.Rows[rowIndex]["Run Time  Hours"] is DBNull))
                {
                    dt.Rows[rowIndex]["New Run Time  Hours"] = Math.Round(Convert.ToDouble(dt.Rows[rowIndex]["Run Time  Hours"]) , 2);
                }
                else
                {
                    dt.Rows[rowIndex]["New Run Time  Hours"] = null;
                }

                //Actual
                if (!(dt.Rows[rowIndex]["Actual"] is DBNull))
                {
                    dt.Rows[rowIndex]["New Actual"] = Math.Round(Convert.ToDouble(dt.Rows[rowIndex]["Actual"]), 2);
                }
                else
                {
                    dt.Rows[rowIndex]["New Actual"] = null;
                }
                //Adjust
                if (!(dt.Rows[rowIndex]["Adjust"] is DBNull))
                {
                    dt.Rows[rowIndex]["New Adjust"] = Math.Round(Convert.ToDouble(dt.Rows[rowIndex]["Adjust"]));
                }
                else
                {
                    dt.Rows[rowIndex]["New Adjust"] = null;
                }

            }

            int columnNumber = dt.Columns["Start Date"].Ordinal;
            dt.Columns.Remove("Start Date");
            dt.Columns["New Start Date"].SetOrdinal(columnNumber);
            dt.Columns["New Start Date"].ColumnName = "Start Date";

            int columnNumber1 = dt.Columns["Start Time"].Ordinal;
            dt.Columns.Remove("Start Time");
            dt.Columns["New Start Time"].SetOrdinal(columnNumber1);
            dt.Columns["New Start Time"].ColumnName = "Start Time";

            int columnNumber2 = dt.Columns["Stop Date"].Ordinal;
            dt.Columns.Remove("Stop Date");
            dt.Columns["New Stop Date"].SetOrdinal(columnNumber2);
            dt.Columns["New Stop Date"].ColumnName = "Stop Date";


            int columnNumber3 = dt.Columns["Stop Time"].Ordinal;
            dt.Columns.Remove("Stop Time");
            dt.Columns["New Stop Time"].SetOrdinal(columnNumber3);
            dt.Columns["New Stop Time"].ColumnName = "Stop Time";

            int columnNumber4 = dt.Columns["Repair Date"].Ordinal;
            dt.Columns.Remove("Repair Date");
            dt.Columns["New Repair Date"].SetOrdinal(columnNumber4);
            dt.Columns["New Repair Date"].ColumnName = "Repair Date";

            int columnNumber5 = dt.Columns["Run Time  Hours"].Ordinal;
            dt.Columns.Remove("Run Time  Hours");
            dt.Columns["New Run Time  Hours"].SetOrdinal(columnNumber5);
            dt.Columns["New Run Time  Hours"].ColumnName = "Run Time  Hours";

            int columnNumber6 = dt.Columns["Actual"].Ordinal;
            dt.Columns.Remove("Actual");
            dt.Columns["New Actual"].SetOrdinal(columnNumber6);
            dt.Columns["New Actual"].ColumnName = "Actual";

            int columnNumber7 = dt.Columns["Adjust"].Ordinal;
            dt.Columns.Remove("Adjust");
            dt.Columns["New Adjust"].SetOrdinal(columnNumber7);
            dt.Columns["New Adjust"].ColumnName = "Adjust";

            return dt;
        }

        public DataTable CorrectiveActionAnalysis(string Procedure, string StDate, string EDate)
        {
            CompanyDrop();
            DataTable dt = new DataTable();
            dt = ShrdMaster.Instance.ShowMOLDTRAXReport(con, Procedure, StDate, EDate);

            dt.Columns.Add("New Mold Start Date", typeof(string));
            dt.Columns.Add("New Mold Stop Date", typeof(string));
            dt.Columns.Add("New CA Date", typeof(string));
            dt.Columns.Add("New Repair Date", typeof(string));

            dt.Columns.Add("New Labor Cost", typeof(string));
            dt.Columns.Add("New Tooling Cost", typeof(string));
            dt.Columns.Add("New Total Cost", typeof(string));
            dt.Columns.Add("New Cycle Count", typeof(string));


            for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
            {
                //Start Date
                if (!(dt.Rows[rowIndex]["Mold Start Date"] is DBNull))
                {
                    dt.Rows[rowIndex]["New Mold Start Date"] = Convert.ToDateTime(dt.Rows[rowIndex]["Mold Start Date"]).ToShortDateString();
                }
                else
                {
                    dt.Rows[rowIndex]["New Mold Start Date"] = null;
                }

                //Start Time
                if (!(dt.Rows[rowIndex]["Mold Stop Date"] is DBNull))
                {
                    dt.Rows[rowIndex]["New Mold Stop Date"] = Convert.ToDateTime(dt.Rows[rowIndex]["Mold Stop Date"]).ToShortDateString();
                }
                else
                {
                    dt.Rows[rowIndex]["New Mold Stop Date"] = null;
                }

                //Stop Date
                if (!(dt.Rows[rowIndex]["CA Date"] is DBNull))
                {
                    dt.Rows[rowIndex]["New CA Date"] = Convert.ToDateTime(dt.Rows[rowIndex]["CA Date"]).ToShortDateString();
                }
                else
                {
                    dt.Rows[rowIndex]["New CA Date"] = null;
                }

                if (!(dt.Rows[rowIndex]["Repair Date"] is DBNull))
                {
                    dt.Rows[rowIndex]["New Repair Date"] = Convert.ToDateTime(dt.Rows[rowIndex]["Repair Date"]).ToShortDateString();
                }
                else
                {
                    dt.Rows[rowIndex]["New Repair Date"] = null;
                }

                
                if (!(dt.Rows[rowIndex]["Labor Cost"] is DBNull))
                {
                    dt.Rows[rowIndex]["New Labor Cost"] = Math.Round(Convert.ToDouble(dt.Rows[rowIndex]["Labor Cost"]),2);
                }
                else
                {
                    dt.Rows[rowIndex]["New Labor Cost"] = null;
                }

                ////Repair Date

                if (!(dt.Rows[rowIndex]["Tooling Cost"] is DBNull))
                {
                    dt.Rows[rowIndex]["New Tooling Cost"] = String.Format("{0:C}", dt.Rows[rowIndex]["Tooling Cost"]);
                }
                else
                {
                    dt.Rows[rowIndex]["New Tooling Cost"] = null;
                }

                ////Run time Hours

                if (!(dt.Rows[rowIndex]["Total Cost"] is DBNull))
                {
                    dt.Rows[rowIndex]["New Total Cost"] = String.Format("{0:C}", dt.Rows[rowIndex]["Total Cost"]); 
                }
                else
                {
                    dt.Rows[rowIndex]["New Total Cost"] = null;
                }

                ////Actual
                if (!(dt.Rows[rowIndex]["Cycle Count"] is DBNull))
                {
                    dt.Rows[rowIndex]["New Cycle Count"] = String.Format("{0:n0}", dt.Rows[rowIndex]["Cycle Count"]);
                }
                else
                {
                    dt.Rows[rowIndex]["New Cycle Count"] = null;
                }
                ////Adjust
                //if (!(dt.Rows[rowIndex]["Adjust"] is DBNull))
                //{
                //    dt.Rows[rowIndex]["New Adjust"] = Math.Round(Convert.ToDouble(dt.Rows[rowIndex]["Adjust"]));
                //}
                //else
                //{
                //    dt.Rows[rowIndex]["New Adjust"] = null;
                //}

            }

            int columnNumber = dt.Columns["Mold Start Date"].Ordinal;
            dt.Columns.Remove("Mold Start Date");
            dt.Columns["New Mold Start Date"].SetOrdinal(columnNumber);
            dt.Columns["New Mold Start Date"].ColumnName = "Mold Start Date";

            int columnNumber1 = dt.Columns["Mold Stop Date"].Ordinal;
            dt.Columns.Remove("Mold Stop Date");
            dt.Columns["New Mold Stop Date"].SetOrdinal(columnNumber1);
            dt.Columns["New Mold Stop Date"].ColumnName = "Mold Stop Date";

            int columnNumber2 = dt.Columns["CA Date"].Ordinal;
            dt.Columns.Remove("CA Date");
            dt.Columns["New CA Date"].SetOrdinal(columnNumber2);
            dt.Columns["New CA Date"].ColumnName = "CA Date";


            int columnNumber3 = dt.Columns["Repair Date"].Ordinal;
            dt.Columns.Remove("Repair Date");
            dt.Columns["New Repair Date"].SetOrdinal(columnNumber3);
            dt.Columns["New Repair Date"].ColumnName = "New Repair Date";

            int columnNumber4 = dt.Columns["Labor Cost"].Ordinal;
            dt.Columns.Remove("Labor Cost");
            dt.Columns["New Labor Cost"].SetOrdinal(columnNumber4);
            dt.Columns["New Labor Cost"].ColumnName = "Labor Cost";

            int columnNumber5 = dt.Columns["Tooling Cost"].Ordinal;
            dt.Columns.Remove("Tooling Cost");
            dt.Columns["New Tooling Cost"].SetOrdinal(columnNumber5);
            dt.Columns["New Tooling Cost"].ColumnName = "Tooling Cost";

            int columnNumber6 = dt.Columns["Total Cost"].Ordinal;
            dt.Columns.Remove("Total Cost");
            dt.Columns["New Total Cost"].SetOrdinal(columnNumber6);
            dt.Columns["New Total Cost"].ColumnName = "Total Cost";

            int columnNumber7 = dt.Columns["Cycle Count"].Ordinal;
            dt.Columns.Remove("Cycle Count");
            dt.Columns["New Cycle Count"].SetOrdinal(columnNumber7);
            dt.Columns["New Cycle Count"].ColumnName = "Cycle Count";

            return dt;
        }

        public DataTable MoldPerformanceReport(string Procedure, string StDate, string EDate)
        {
            CompanyDrop();
            DataTable dt = new DataTable();
            dt = ShrdMaster.Instance.ShowMOLDTRAXReport(con, Procedure, StDate, EDate);

            dt.Columns.Add("New Tooling Cost", typeof(string));
            dt.Columns.Add("New Labor Cost", typeof(string));
            dt.Columns.Add("New Total Cost", typeof(string));
            dt.Columns.Add("New Cost Per Hour", typeof(string));

            dt.Columns.Add("New Total Actual Cycles Run", typeof(string));
            dt.Columns.Add("New Tooling Cost", typeof(string));
            dt.Columns.Add("New Total Cost", typeof(string));
            dt.Columns.Add("New Cycle Count", typeof(string));


            for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
            {
                //Start Date
                if (!(dt.Rows[rowIndex]["Mold Start Date"] is DBNull))
                {
                    dt.Rows[rowIndex]["New Mold Start Date"] = Convert.ToDateTime(dt.Rows[rowIndex]["Mold Start Date"]).ToShortDateString();
                }
                else
                {
                    dt.Rows[rowIndex]["New Mold Start Date"] = null;
                }

                //Start Time
                if (!(dt.Rows[rowIndex]["Mold Stop Date"] is DBNull))
                {
                    dt.Rows[rowIndex]["New Mold Stop Date"] = Convert.ToDateTime(dt.Rows[rowIndex]["Mold Stop Date"]).ToShortDateString();
                }
                else
                {
                    dt.Rows[rowIndex]["New Mold Stop Date"] = null;
                }

                //Stop Date
                if (!(dt.Rows[rowIndex]["CA Date"] is DBNull))
                {
                    dt.Rows[rowIndex]["New CA Date"] = Convert.ToDateTime(dt.Rows[rowIndex]["CA Date"]).ToShortDateString();
                }
                else
                {
                    dt.Rows[rowIndex]["New CA Date"] = null;
                }

                if (!(dt.Rows[rowIndex]["Repair Date"] is DBNull))
                {
                    dt.Rows[rowIndex]["New Repair Date"] = Convert.ToDateTime(dt.Rows[rowIndex]["Repair Date"]).ToShortDateString();
                }
                else
                {
                    dt.Rows[rowIndex]["New Repair Date"] = null;
                }


                if (!(dt.Rows[rowIndex]["Labor Cost"] is DBNull))
                {
                    dt.Rows[rowIndex]["New Labor Cost"] = Math.Round(Convert.ToDouble(dt.Rows[rowIndex]["Labor Cost"]), 2);
                }
                else
                {
                    dt.Rows[rowIndex]["New Labor Cost"] = null;
                }

                ////Repair Date

                if (!(dt.Rows[rowIndex]["Tooling Cost"] is DBNull))
                {
                    dt.Rows[rowIndex]["New Tooling Cost"] = String.Format("{0:C}", dt.Rows[rowIndex]["Tooling Cost"]);
                }
                else
                {
                    dt.Rows[rowIndex]["New Tooling Cost"] = null;
                }

                ////Run time Hours

                if (!(dt.Rows[rowIndex]["Total Cost"] is DBNull))
                {
                    dt.Rows[rowIndex]["New Total Cost"] = String.Format("{0:C}", dt.Rows[rowIndex]["Total Cost"]);
                }
                else
                {
                    dt.Rows[rowIndex]["New Total Cost"] = null;
                }

                ////Actual
                if (!(dt.Rows[rowIndex]["Cycle Count"] is DBNull))
                {
                    dt.Rows[rowIndex]["New Cycle Count"] = String.Format("{0:n0}", dt.Rows[rowIndex]["Cycle Count"]);
                }
                else
                {
                    dt.Rows[rowIndex]["New Cycle Count"] = null;
                }
                ////Adjust
                //if (!(dt.Rows[rowIndex]["Adjust"] is DBNull))
                //{
                //    dt.Rows[rowIndex]["New Adjust"] = Math.Round(Convert.ToDouble(dt.Rows[rowIndex]["Adjust"]));
                //}
                //else
                //{
                //    dt.Rows[rowIndex]["New Adjust"] = null;
                //}

            }

            int columnNumber = dt.Columns["Mold Start Date"].Ordinal;
            dt.Columns.Remove("Mold Start Date");
            dt.Columns["New Mold Start Date"].SetOrdinal(columnNumber);
            dt.Columns["New Mold Start Date"].ColumnName = "Mold Start Date";

            int columnNumber1 = dt.Columns["Mold Stop Date"].Ordinal;
            dt.Columns.Remove("Mold Stop Date");
            dt.Columns["New Mold Stop Date"].SetOrdinal(columnNumber1);
            dt.Columns["New Mold Stop Date"].ColumnName = "Mold Stop Date";

            int columnNumber2 = dt.Columns["CA Date"].Ordinal;
            dt.Columns.Remove("CA Date");
            dt.Columns["New CA Date"].SetOrdinal(columnNumber2);
            dt.Columns["New CA Date"].ColumnName = "CA Date";


            int columnNumber3 = dt.Columns["Repair Date"].Ordinal;
            dt.Columns.Remove("Repair Date");
            dt.Columns["New Repair Date"].SetOrdinal(columnNumber3);
            dt.Columns["New Repair Date"].ColumnName = "New Repair Date";

            int columnNumber4 = dt.Columns["Labor Cost"].Ordinal;
            dt.Columns.Remove("Labor Cost");
            dt.Columns["New Labor Cost"].SetOrdinal(columnNumber4);
            dt.Columns["New Labor Cost"].ColumnName = "Labor Cost";

            int columnNumber5 = dt.Columns["Tooling Cost"].Ordinal;
            dt.Columns.Remove("Tooling Cost");
            dt.Columns["New Tooling Cost"].SetOrdinal(columnNumber5);
            dt.Columns["New Tooling Cost"].ColumnName = "Tooling Cost";

            int columnNumber6 = dt.Columns["Total Cost"].Ordinal;
            dt.Columns.Remove("Total Cost");
            dt.Columns["New Total Cost"].SetOrdinal(columnNumber6);
            dt.Columns["New Total Cost"].ColumnName = "Total Cost";

            int columnNumber7 = dt.Columns["Cycle Count"].Ordinal;
            dt.Columns.Remove("Cycle Count");
            dt.Columns["New Cycle Count"].SetOrdinal(columnNumber7);
            dt.Columns["New Cycle Count"].ColumnName = "Cycle Count";

            return dt;

        }

        public DataTable InventoryTracking(string Procedure)
        {
            CompanyDrop();
            DataTable dt = new DataTable();
            dt = ShrdMaster.Instance.InventoryTracking(con, Procedure);

            dt.Columns.Add("New Cost", typeof(string));
            dt.Columns.Add("New Date Ordered", typeof(string));

            for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
            {
                //Start Date
                if (!(dt.Rows[rowIndex]["Cost"] is DBNull))
                {
                    dt.Rows[rowIndex]["New Cost"] = String.Format("{0:C}",dt.Rows[rowIndex]["Cost"]);
                }
                else
                {
                    dt.Rows[rowIndex]["New Cost"] = null;
                }

                //Start Time
                if (!(dt.Rows[rowIndex]["Date Ordered"] is DBNull))
                {
                    dt.Rows[rowIndex]["New Date Ordered"] = Convert.ToDateTime(dt.Rows[rowIndex]["Date Ordered"]).ToShortDateString();
                }
                else
                {
                    dt.Rows[rowIndex]["New Date Ordered"] = null;
                }
            }

            int columnNumber = dt.Columns["Cost"].Ordinal;
            dt.Columns.Remove("Cost");
            dt.Columns["New Cost"].SetOrdinal(columnNumber);
            dt.Columns["New Cost"].ColumnName = "Cost";

            int columnNumber1 = dt.Columns["Date Ordered"].Ordinal;
            dt.Columns.Remove("Date Ordered");
            dt.Columns["New Date Ordered"].SetOrdinal(columnNumber1);
            dt.Columns["New Date Ordered"].ColumnName = "Date Ordered";

            return dt;

        }

        public DataTable MoldList(string Procedure)
        {
            CompanyDrop();
            DataTable dt = new DataTable();
            dt = ShrdMaster.Instance.MoldListWrapper(con, Procedure);

            dt.Columns.Add("New Date Acquired", typeof(string));
            dt.Columns.Add("New Date De-Activated", typeof(string));

            for (int rowIndex = 0; rowIndex < dt.Rows.Count; rowIndex++)
            {
                //Start Date
                if (!(dt.Rows[rowIndex]["Date Acquired"] is DBNull))
                {
                    dt.Rows[rowIndex]["New Date Acquired"] = Convert.ToDateTime(dt.Rows[rowIndex]["Date Acquired"]).ToShortDateString();
                }
                else
                {
                    dt.Rows[rowIndex]["New Date Acquired"] = null;
                }

                //Start Time
                if (!(dt.Rows[rowIndex]["Date De-Activated"] is DBNull))
                {
                    dt.Rows[rowIndex]["New Date De-Activated"] = Convert.ToDateTime(dt.Rows[rowIndex]["Date De-Activated"]).ToShortDateString();
                }
                else
                {
                    dt.Rows[rowIndex]["New Date De-Activated"] = null;
                }
            }

            int columnNumber = dt.Columns["Date Acquired"].Ordinal;
            dt.Columns.Remove("Date Acquired");
            dt.Columns["New Date Acquired"].SetOrdinal(columnNumber);
            dt.Columns["New Date Acquired"].ColumnName = "Date Acquired";

            int columnNumber1 = dt.Columns["Date De-Activated"].Ordinal;
            dt.Columns.Remove("Date De-Activated");
            dt.Columns["New Date De-Activated"].SetOrdinal(columnNumber1);
            dt.Columns["New Date De-Activated"].ColumnName = "Date De-Activated";

            return dt;

        }


        public ActionResult PrintFile(string StartDate="", string EndDate="", int MoldID=0, string ReportName="")
        {

            string deviceInfo = "";
            CompanyDrop();

            string StDate = StartDate;
            string EDate = EndDate;

            if (StartDate != "" || EndDate != "")
            {
                StDate = Convert.ToDateTime(StartDate).ToString("yyyy-MM-dd hh:mm:ss");
                EDate = Convert.ToDateTime(EndDate).ToString("yyyy-MM-dd hh:mm:ss");
            }
            else
            {
                StDate = "";
                EDate = "";
            }

            LocalReport localReport = new LocalReport();

            string ReportPath = "";
            //Statistical Print
            if (ReportName == "RepairSheet")
            {
                var data = ShrdMaster.Instance.RepairSheetList(con, StDate, EDate, MoldID);

                var MoldConfig = db.TblDDMoldConfigs.ToList();
                var MoldConfig2 = db.TblDDMoldConfig2s.ToList();
                var StopReason = db.TblddStopReasons.ToList();
                var CorrectiveAction = db.TblDDTlCorrectiveActions.ToList();

                int Config = 0;
                int Config2 = 0;
                int MLDMaintreq = 0;
                int Corrective = 0;

                foreach (var x in data) // search whole table
                {
                    x.MoldDefectMapPath = Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/SiteImages/MaintenanceInstruction/" + x.MoldDefectMapPath);
                    if (x.MoldToolDescrip != null || x.MoldToolDescrip != "")
                    {
                        string[] sds = Regex.Split(x.MoldToolDescrip, "</p>");
                        string s = HtmlToPlainText(sds[0]);

                        if (s != "")
                        {
                            s = s.Replace(System.Environment.NewLine, string.Empty);
                            x.MoldToolDescrip = s.Replace("&nbsp;", " ");
                        }
                    }

                    if (x.MoldConfig != "" && x.MoldConfig != null)
                    {
                        Config = Convert.ToInt32(x.MoldConfig);
                    }
                    var dataConfig = MoldConfig.Where(c => c.ID == Config).FirstOrDefault();
                    x.MoldConfig = dataConfig != null ? dataConfig.MoldConfig : "";

                    if (x.MoldConfig2 != "" && x.MoldConfig2 != null)
                    {
                        Config2 = Convert.ToInt32(x.MoldConfig2);
                    }
                    var dataConfig2 = MoldConfig2.Where(c => c.ID == Config2).FirstOrDefault();
                    x.MoldConfig2 = dataConfig2 != null ? dataConfig2.MoldConfig : "";


                    if (x.MldPullMaintRequired != "" && x.MldPullMaintRequired != null)
                    {
                        MLDMaintreq = Convert.ToInt32(x.MldPullMaintRequired);
                    }
                    var STreasin = StopReason.Where(c => c.ID == MLDMaintreq).FirstOrDefault();
                    x.MldPullMaintRequired = STreasin != null ? STreasin.StopReason : "";


                    if (x.TlCorrectiveAction != "" && x.TlCorrectiveAction != null)
                    {
                        Corrective = Convert.ToInt32(x.TlCorrectiveAction);
                    }

                    var CorrectiveAct = CorrectiveAction.Where(c => c.ID == Corrective).FirstOrDefault();
                    x.TlCorrectiveAction = CorrectiveAct != null ? CorrectiveAct.TlCorrectiveAction : "";

                }


                localReport.DataSources.Add(new ReportDataSource("DataSet1", data));
                localReport.EnableExternalImages = true;

                ReportPath = @"Reports\RepairSheetNewPrint.rdlc";
                //ReportPath = @"Reports\RepairSheet.rdlc";


                deviceInfo = @"<DeviceInfo>
                <OutputFormat>PDF</OutputFormat>
               <PageWidth>9.2in</PageWidth>
                <PageHeight>8.5in</PageHeight>
                <MarginTop>0.25in</MarginTop>
                <MarginLeft>0.2in</MarginLeft>
                <MarginRight>0in</MarginRight>
                <MarginBottom>0.25in</MarginBottom>
            </DeviceInfo>";

            }

            else if (ReportName == "MaintenanceTimeLine")
            {
                var data = ShrdMaster.Instance.MaintenenceTimeline(con, StDate, EDate, MoldID);

                var Config1 = db.TblDDMoldConfigs.ToList();
                var Config2 = db.TblDDMoldConfig2s.ToList();
                var StopReason = db.TblddStopReasons.ToList();
                var ProductPart = db.TblDDProductPart.ToList();
                var ProductLine = db.TblDDProductLine.ToList();


                int MoldCONFIG = 0;
                int MoldConfig2 = 0;
                int MLDMaintreq = 0;
                int ProductPartID = 0;
                int ProductLineID = 0;


                foreach (var x in data)
                {

                    if (x.MoldConfig != "" && x.MoldConfig != null)
                    {
                        MoldCONFIG = Convert.ToInt32(x.MoldConfig);
                    }
                    var MCon1 = Config1.Where(c => c.ID == MoldCONFIG).FirstOrDefault();
                    x.MoldConfig = MCon1 != null ? MCon1.MoldConfig : "";

                    if (x.MoldConfig2 != "" && x.MoldConfig2 != null)
                    {
                        MoldConfig2 = Convert.ToInt32(x.MoldConfig2);
                    }
                    var MCon2 = Config2.Where(c => c.ID == MoldConfig2).FirstOrDefault();
                    x.MoldConfig2 = MCon2 != null ? MCon2.MoldConfig : "";

                    if (x.MldPullMaintRequired != "" && x.MldPullMaintRequired != null)
                    {
                        MLDMaintreq = Convert.ToInt32(x.MldPullMaintRequired);
                    }
                    var STreasin = StopReason.Where(c => c.ID == MLDMaintreq).FirstOrDefault();
                    x.MldPullMaintRequired = STreasin != null ? STreasin.StopReason : "";

                    if (x.ProductPart != "" && x.ProductPart != null)
                    {
                        ProductPartID = Convert.ToInt32(x.ProductPart);
                    }
                    var ProductPt = ProductPart.Where(c => c.ID == ProductPartID).FirstOrDefault();
                    x.ProductPart = ProductPt != null ? ProductPt.ProductPart : "";


                    if (x.ProductLine != "" && x.ProductLine != null)
                    {
                        ProductLineID = Convert.ToInt32(x.ProductLine);
                    }
                    var ProductLn = ProductLine.Where(c => c.ID == ProductLineID).FirstOrDefault();
                    x.ProductLine = ProductLn != null ? ProductLn.ProductLine : "";
                }


                localReport.DataSources.Add(new ReportDataSource("DataSet1", data));
                ReportPath = @"Reports\MaintenanceTimelines.rdlc";

                deviceInfo = @"<DeviceInfo>
                <OutputFormat>PDF</OutputFormat>
               <PageWidth>11in</PageWidth>
                <PageHeight>8.5in</PageHeight>
                <MarginTop>0.25in</MarginTop>
                <MarginLeft>0.50in</MarginLeft>
                <MarginRight>0.25in</MarginRight>
                <MarginBottom>0.25in</MarginBottom>
            </DeviceInfo>";

            }

            else if (ReportName == "DefectCostAnalysis")
            {

                var data = ShrdMaster.Instance.DefectCostAnalysis(con, StDate, EDate, MoldID);

                var Config1 = db.TblDDMoldConfigs.ToList();
                var Config2 = db.TblDDMoldConfig2s.ToList();
                //var StopReason = db.TblddStopReasons.ToList();
                var ProductPart = db.TblDDProductPart.ToList();
                var ProductLine = db.TblDDProductLine.ToList();


                int MoldCONFIG = 0;
                int MoldConfig2 = 0;
                //int MLDMaintreq = 0;
                int ProductPartID = 0;
                int ProductLineID = 0;


                foreach (var x in data)
                {

                    if (x.MoldConfig != "" && x.MoldConfig != null)
                    {
                        MoldCONFIG = Convert.ToInt32(x.MoldConfig);
                    }
                    var MCon1 = Config1.Where(c => c.ID == MoldCONFIG).FirstOrDefault();
                    x.MoldConfig = MCon1 != null ? MCon1.MoldConfig : "";

                    if (x.MoldConfig2 != "" && x.MoldConfig2 != null)
                    {
                        MoldConfig2 = Convert.ToInt32(x.MoldConfig2);
                    }
                    var MCon2 = Config2.Where(c => c.ID == MoldConfig2).FirstOrDefault();
                    x.MoldConfig2 = MCon2 != null ? MCon2.MoldConfig : "";

                    if (x.ProductPart != "" && x.ProductPart != null)
                    {
                        ProductPartID = Convert.ToInt32(x.ProductPart);
                    }
                    var ProductPt = ProductPart.Where(c => c.ID == ProductPartID).FirstOrDefault();
                    x.ProductPart = ProductPt != null ? ProductPt.ProductPart : "";


                    if (x.ProductLine != "" && x.ProductLine != null)
                    {
                        ProductLineID = Convert.ToInt32(x.ProductLine);
                    }
                    var ProductLn = ProductLine.Where(c => c.ID == ProductLineID).FirstOrDefault();
                    x.ProductLine = ProductLn != null ? ProductLn.ProductLine : "";
                }



                localReport.DataSources.Add(new ReportDataSource("DataSet1", data));
                ReportPath = @"Reports\DefectCostAnalysis.rdlc";
                deviceInfo = @"<DeviceInfo>
                <OutputFormat>PDF</OutputFormat>
               <PageWidth>11in</PageWidth>
                <PageHeight>8.5in</PageHeight>
                <MarginTop>0.25in</MarginTop>
                <MarginLeft>0.25in</MarginLeft>
                <MarginRight>0.25in</MarginRight>
                <MarginBottom>0.25in</MarginBottom>
            </DeviceInfo>";
            }

            else if (ReportName == "DefectTracking")
            {
                var data = ShrdMaster.Instance.DefectTracking(con, StDate, EDate, MoldID);

                var Config1 = db.TblDDMoldConfigs.ToList();
                var Config2 = db.TblDDMoldConfig2s.ToList();
                var ProductPart = db.TblDDProductPart.ToList();
                var ProductLine = db.TblDDProductLine.ToList();


                int MoldCONFIG = 0;
                int MoldConfig2 = 0;
                int ProductPartID = 0;
                int ProductLineID = 0;


                foreach (var x in data)
                {

                    if (x.MoldConfig != "" && x.MoldConfig != null)
                    {
                        MoldCONFIG = Convert.ToInt32(x.MoldConfig);
                    }
                    var MCon1 = Config1.Where(c => c.ID == MoldCONFIG).FirstOrDefault();
                    x.MoldConfig = MCon1 != null ? MCon1.MoldConfig : "";

                    if (x.MoldConfig2 != "" && x.MoldConfig2 != null)
                    {
                        MoldConfig2 = Convert.ToInt32(x.MoldConfig2);
                    }
                    var MCon2 = Config2.Where(c => c.ID == MoldConfig2).FirstOrDefault();
                    x.MoldConfig2 = MCon2 != null ? MCon2.MoldConfig : "";

                    if (x.ProductPart != "" && x.ProductPart != null)
                    {
                        ProductPartID = Convert.ToInt32(x.ProductPart);
                    }
                    var ProductPt = ProductPart.Where(c => c.ID == ProductPartID).FirstOrDefault();
                    x.ProductPart = ProductPt != null ? ProductPt.ProductPart : "";


                    if (x.ProductLine != "" && x.ProductLine != null)
                    {
                        ProductLineID = Convert.ToInt32(x.ProductLine);
                    }
                    var ProductLn = ProductLine.Where(c => c.ID == ProductLineID).FirstOrDefault();
                    x.ProductLine = ProductLn != null ? ProductLn.ProductLine : "";
                }


                localReport.DataSources.Add(new ReportDataSource("DataSet1", data));
                ReportPath = @"Reports\DefectTracking2.rdlc";
                deviceInfo = @"<DeviceInfo>
                <OutputFormat>PDF</OutputFormat>
               <PageWidth>11in</PageWidth>
                <PageHeight>8.5in</PageHeight>
                <MarginTop>0.25in</MarginTop>
                <MarginLeft>0.75in</MarginLeft>
                <MarginRight>0.25in</MarginRight>
                <MarginBottom>0.25in</MarginBottom>
            </DeviceInfo>";
            }

            else if (ReportName == "TotalTimeRun")
            {
                var data = ShrdMaster.Instance.TotalTimeRun(con, StDate, EDate, MoldID);

                var Config1 = db.TblDDMoldConfigs.ToList();
                var Config2 = db.TblDDMoldConfig2s.ToList();
                var ProductPart = db.TblDDProductPart.ToList();
                var ProductLine = db.TblDDProductLine.ToList();


                int MoldCONFIG = 0;
                int MoldConfig2 = 0;
                int ProductPartID = 0;
                int ProductLineID = 0;


                foreach (var x in data)
                {

                    if (x.MoldConfig != "" && x.MoldConfig != null)
                    {
                        MoldCONFIG = Convert.ToInt32(x.MoldConfig);
                    }
                    var MCon1 = Config1.Where(c => c.ID == MoldCONFIG).FirstOrDefault();
                    x.MoldConfig = MCon1 != null ? MCon1.MoldConfig : "";

                    if (x.MoldConfig2 != "" && x.MoldConfig2 != null)
                    {
                        MoldConfig2 = Convert.ToInt32(x.MoldConfig2);
                    }
                    var MCon2 = Config2.Where(c => c.ID == MoldConfig2).FirstOrDefault();
                    x.MoldConfig2 = MCon2 != null ? MCon2.MoldConfig : "";

                    if (x.ProductPart != "" && x.ProductPart != null)
                    {
                        ProductPartID = Convert.ToInt32(x.ProductPart);
                    }
                    var ProductPt = ProductPart.Where(c => c.ID == ProductPartID).FirstOrDefault();
                    x.ProductPart = ProductPt != null ? ProductPt.ProductPart : "";


                    if (x.ProductLine != "" && x.ProductLine != null)
                    {
                        ProductLineID = Convert.ToInt32(x.ProductLine);
                    }
                    var ProductLn = ProductLine.Where(c => c.ID == ProductLineID).FirstOrDefault();
                    x.ProductLine = ProductLn != null ? ProductLn.ProductLine : "";
                }


                localReport.DataSources.Add(new ReportDataSource("DataSet1", data));
                ReportPath = @"Reports\TotalTimeRun2.rdlc";
                deviceInfo = @"<DeviceInfo>
                <OutputFormat>PDF</OutputFormat>
               <PageWidth>11in</PageWidth>
                <PageHeight>8.5in</PageHeight>
                <MarginTop>0.25in</MarginTop>
                <MarginLeft>0.75in</MarginLeft>
                <MarginRight>0.25in</MarginRight>
                <MarginBottom>0.25in</MarginBottom>
            </DeviceInfo>";
            }

            //Reference Print
            else if (ReportName == "IMLSheet")
            {
                var data = ShrdMaster.Instance.IMLSheet(con, MoldID);
                foreach (DataRow dr in data.Rows) // search whole table
                {
                    dr["MoldMapPath"] = Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/SiteImages/IMLImg/" + dr["MoldMapPath"]); //change the name
                }

                localReport.EnableExternalImages = true;
                ReportPath = @"Reports\InjectionMoldLayoutSheet.rdlc";
                localReport.DataSources.Add(new ReportDataSource("DataSet1", data));

                deviceInfo = @"<DeviceInfo>
                <OutputFormat>PDF</OutputFormat>
               <PageWidth>8.5in</PageWidth>
                <PageHeight>12in</PageHeight>
                <MarginTop>0.25in</MarginTop>
                <MarginLeft>0.25in</MarginLeft>
                <MarginRight>0.25in</MarginRight>
                <MarginBottom>0.25in</MarginBottom>
            </DeviceInfo>";
            }

            else if (ReportName == "TroubleShooterGuide")
            {
                var data = ShrdMaster.Instance.TSGuideRPTWrapper(con, MoldID);

                List<TSGuideRPTWrapper> qryTSGuideRpt_Wrapper = new List<TSGuideRPTWrapper>();
                if (data.Count() != 0)
                {
                    foreach (var x in data)
                    {
                        TSGuideRPTWrapper TS = new TSGuideRPTWrapper();
                        TS.TSSeqNum = x.TSSeqNum;
                        TS.TSGuide = x.TSGuide;
                        TS.MoldDataID = x.MoldDataID;
                        TS.TSDefects = x.TSDefects;
                        TS.TSExplanation = x.TSExplanation;
                        TS.TSToolInv = x.TSToolInv;
                        TS.TSProbCause = x.TSProbCause;
                        TS.TSSolution = x.TSSolution;
                        TS.ImagePath = Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/TroubleShooterImage/" + x.ImagePath);
                        TS.TSType = x.TSType;
                        TS.TSPreventAction = x.TSPreventAction;
                        TS.MoldName = x.MoldName;
                        TS.MoldDesc = x.MoldDesc;
                        TS.CompanyCNTroubleShoot = x.CompanyCNTroubleShoot;
                        TS.CompanyName = x.CompanyName;
                        qryTSGuideRpt_Wrapper.Add(TS);
                    }
                }

                else
                {
                    TSGuideRPTWrapper TS = new TSGuideRPTWrapper();
                    qryTSGuideRpt_Wrapper.Add(TS);
                }

                localReport.EnableExternalImages = true;
                localReport.DataSources.Add(new ReportDataSource("DataSet1", qryTSGuideRpt_Wrapper.OrderBy(X=> X.TSType)));
                ReportPath = @"Reports\TroubleShootersGuide.rdlc";

                deviceInfo = @"<DeviceInfo>
                <OutputFormat>PDF</OutputFormat>
               <PageWidth>11in</PageWidth>
                <PageHeight>8.5in</PageHeight>
                <MarginTop>0.25in</MarginTop>
                <MarginLeft>0.25in</MarginLeft>
                <MarginRight>0.25in</MarginRight>
                <MarginBottom>0.25in</MarginBottom>
            </DeviceInfo>";


            }

            else if (ReportName == "TechTipReport")
            {
                var data = ShrdMaster.Instance.TechTips(con, MoldID);
                localReport.DataSources.Add(new ReportDataSource("DataSet1", data));
                ReportPath = @"Reports\TechTips.rdlc";

                deviceInfo = @"<DeviceInfo>
                <OutputFormat>PDF</OutputFormat>
                <PageWidth>11in</PageWidth>
                <PageHeight>8.5in</PageHeight>
                <MarginTop>0.25in</MarginTop>
                <MarginLeft>0.25in</MarginLeft>
                <MarginRight>0.25in</MarginRight>
                <MarginBottom>0.25in</MarginBottom>
                </DeviceInfo>";
            }
            else if (ReportName == "MoldTooling")
            {
                var data = ShrdMaster.Instance.MoldTooling(con, MoldID);
                var ProductPart = db.TblDDProductPart.ToList();
                var ProductLine = db.TblDDProductLine.ToList();
                var ToolingType = db.TblDDMoldToolingTypes.ToList();

                int ProductPartID = 0;
                int ProductLineID = 0;
                int ToolingID = 0;

                foreach (DataRow dr in data.Rows)
                {

                    if (dr["ProductPart"].ToString() != "" && dr["ProductPart"] != null)
                    {
                        ProductPartID = Convert.ToInt32(dr["ProductPart"]);
                    }
                    var ProductPt = ProductPart.Where(c => c.ID == ProductPartID).FirstOrDefault();
                    dr["ProductPart"] = ProductPt != null ? ProductPt.ProductPart : "";


                    if (dr["ProductLine"].ToString() != "" && dr["ProductLine"] != null)
                    {
                        ProductLineID = Convert.ToInt32(dr["ProductLine"]);
                    }
                    var ProductLn = ProductLine.Where(c => c.ID == ProductLineID).FirstOrDefault();
                    dr["ProductLine"] = ProductLn != null ? ProductLn.ProductLine : "";

                    if (dr["MoldToolingType"].ToString() != "" && dr["MoldToolingType"] != null)
                    {
                        ToolingID = Convert.ToInt32(dr["MoldToolingType"]);
                    }
                    var ToolingTypeVal = ToolingType.Where(c => c.ID == ToolingID).FirstOrDefault();
                    dr["MoldToolingType"] = ToolingTypeVal != null ? ToolingTypeVal.DD_MoldToolingType : "";

                }
                ReportPath = @"Reports\MoldTooling.rdlc";
                localReport.DataSources.Add(new ReportDataSource("DataSet1", data));

                deviceInfo = @"<DeviceInfo>
                <OutputFormat>PDF</OutputFormat>
                <PageWidth>11in</PageWidth>
                <PageHeight>8.5in</PageHeight>
                <MarginTop>0.25in</MarginTop>
                <MarginLeft>0.25in</MarginLeft>
                <MarginRight>0.25in</MarginRight>
                <MarginBottom>0.25in</MarginBottom>
                </DeviceInfo>";
            }
            else if (ReportName == "LastShotInspection")
            {

                var data = ShrdMaster.Instance.LastShot(con, MoldID);

                foreach (DataRow dr in data.Rows) // search whole table
                {
                    dr["TTPartImagePath"] = Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/SiteImages/TechTipsImages/" + dr["TTPartImagePath"]); //change the name
                }

                localReport.EnableExternalImages = true;

                ReportPath =  @"Reports\LastShotRpt.rdlc";
                localReport.DataSources.Add(new ReportDataSource("DataSet1", data));

                //var data = ShrdMaster.Instance.MoldTooling(con, MoldID);
                //ReportPath = @"Reports\MoldTooling.rdlc";
                //localReport.DataSources.Add(new ReportDataSource("DataSet1", data));

                deviceInfo = @"<DeviceInfo>
                <OutputFormat>PDF</OutputFormat>
                <PageWidth>10.5in</PageWidth>
                <PageHeight>9in</PageHeight>
                <MarginTop>0.25in</MarginTop>
                <MarginLeft>0.25in</MarginLeft>
                <MarginRight>0.25in</MarginRight>
                <MarginBottom>0.25in</MarginBottom>
                </DeviceInfo>";
            }


            localReport.ReportPath = ReportPath;

            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension = "pdf";

            //The DeviceInfo settings should be changed based on the reportType

            //http://msdn2.microsoft.com/en-us/library/ms155397.aspx


            //string deviceInfo =
            // @"<DeviceInfo>
            //    <OutputFormat>PDF</OutputFormat>
            //   <PageWidth>11in</PageWidth>
            //    <PageHeight>8.5in</PageHeight>
            //    <MarginTop>0.25in</MarginTop>
            //    <MarginLeft>0.25in</MarginLeft>
            //    <MarginRight>0.25in</MarginRight>
            //    <MarginBottom>0.25in</MarginBottom>
            //</DeviceInfo>";


            Warning[] warnings;

            string[] streams;

            byte[] renderedBytes;

            string StorePDFPath = @"C:\Websites\Moldtrax.infodatixhosting.com\Reports\Summary.pdf";
            //string StorePDFPath = @"E:\Projects\Moldtrax.infodatixhosting.com\Mold\Reports\Summary.pdf";
            //Render the report

            renderedBytes = localReport.Render(
        reportType,
        deviceInfo,
        out mimeType,
        out encoding,
        out fileNameExtension,
        out streams,
        out warnings);

            var doc = new Document();
            var reader = new PdfReader(renderedBytes);

            using (FileStream fs = new FileStream(StorePDFPath, FileMode.Create))
            {
                PdfStamper stamper = new PdfStamper(reader, fs);
                string Printer = "";
                if (Printer == null || Printer == "")
                {
                    stamper.JavaScript = "var pp = getPrintParams();pp.interactive = pp.constants.interactionLevel.automatic;pp.printerName = getPrintParams().printerName;print(pp);\r";
                    stamper.Close();
                }
                else
                {
                    stamper.JavaScript = "var pp = getPrintParams();pp.interactive = pp.constants.interactionLevel.automatic;pp.printerName = " + Printer + ";print(pp);\r";
                    stamper.Close();
                }
            }
            reader.Close();

            FileStream fss = new FileStream(StorePDFPath, FileMode.Open);
            byte[] bytes = new byte[fss.Length];
            fss.Read(bytes, 0, Convert.ToInt32(fss.Length));
            fss.Close();
            System.IO.File.Delete(StorePDFPath);
            return File(bytes, "application/pdf");
        }
    }
}