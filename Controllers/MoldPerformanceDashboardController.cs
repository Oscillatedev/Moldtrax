using Moldtrax.Models;
using Moldtrax.Providers;
using Moldtrax.ViewMoldel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Moldtrax.Controllers
{
    [SessionExpireFilter]
    public class MoldPerformanceDashboardController : Controller
    {
       static string constring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        SqlConnection con = new SqlConnection(constring);
        //static DateTime StartDate = new DateTime(2000, 04, 01);
        //static DateTime EndDate = new DateTime(2018,06,06);

        string StDate = "";
        string EDate = "";


        string Date = System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

        private MoldtraxDbContext Db = new MoldtraxDbContext();
        // GET: MoldPerformanceDashboard
        public ActionResult Index()
        {
            ViewBag.Companylist = new SelectList(Db.TblCompanies.ToList().OrderBy(x => x.CompanyName), "CompanyID", "CompanyName");
            //var data = Db.Database.SqlQuery<MoldDropdown>("exec procMoldDropdown").ToList<MoldDropdown>();
            //List<SelectListItem> Tech = new List<SelectListItem>();
            //foreach (var x in data)
            //{
            //    Tech.Add(new SelectListItem
            //    {
            //        Text = x.Name,
            //        Value = x.MoldDataID.ToString()
            //    });
            //}

            var data = ShrdMaster.Instance.GetMoldDropDown();
            ViewBag.MoldList = data;

            var date = Db.TblMoldCharts.Where(x => x.ID == 1).FirstOrDefault();
            string StDate = "";
            string EDate = "";

            if (date != null)
            {
                 StDate = date.StartDate.ToString("yyyy-MM-dd hh:mm:ss");
                 EDate = date.EndDate.ToString("yyyy-MM-dd hh:mm:ss");
            }
            else
            {
                tblMoldChart tbM = new tblMoldChart();
                tbM.StartDate = System.DateTime.Now;
                tbM.EndDate = System.DateTime.Now;
                tbM.Chart = 1;

                Db.TblMoldCharts.Add(tbM);
                Db.SaveChanges();

                StDate = System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
                EDate = System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            }

            ViewBag.StDate = StDate;
            ViewBag.EDate = EDate;
            return View();
        }

        public JsonResult GetMoldList()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = ShrdMaster.Instance.GetMoldDropDown();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ShowChart(int MoldID, int ChartType, DateTime StartDate, DateTime EndDate)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            string StDate = StartDate.ToString("yyyy-MM-dd hh:mm:ss");
            string EDate = EndDate.ToString("yyyy-MM-dd hh:mm:ss");

            //var cons = new SqlConnection(constring);
            //SqlConnection con = new SqlConnection(constring);

           var date = Db.TblMoldCharts.Where(x => x.ID == 1).FirstOrDefault();
            date.StartDate = StartDate;
            date.EndDate = EndDate;
            Db.SaveChanges();

            if (ChartType == 1)
            {
                    //using (SqlCommand cmd = new SqlCommand("procCompleteMaintenanceTrackingAllMolds", con))
                    //{
                    try
                    {

                        var DC = ShrdMaster.Instance.ScheduleStopvsXStop(con, StDate, EDate);

                        int Schedule = 0;
                        int UnSchedule = 0;
                        foreach (var x in DC)
                        {
                            if (x.MoldStopReason.StartsWith("X"))
                            {
                                UnSchedule += 1;
                            }
                            else
                            {
                                Schedule += 1;
                            }

                            //if (x.MoldStopReason.StartsWith("X"))
                            //{
                            //    Schedule += 0;
                            //}
                            //else
                            //{
                            //    Schedule += 1;
                            //}
                        }

                        List<CompleteMaintenanceTrackingTwo> dc = new List<CompleteMaintenanceTrackingTwo>();
                        dc.Add(new CompleteMaintenanceTrackingTwo { Count = Schedule, Name = "Schedule" });
                        dc.Add(new CompleteMaintenanceTrackingTwo { Count = UnSchedule, Name = "Unschedule" });

                    //cmd.CommandType = CommandType.StoredProcedure;

                    //    cmd.Parameters.Add("@StartDate", SqlDbType.VarChar).Value = StDate;
                    //    cmd.Parameters.Add("@EndDate", SqlDbType.VarChar).Value = EDate;
                    //    con.Open();
                    //    cmd.ExecuteNonQuery();

                    //var data = Db.Database.SqlQuery<StopReasonCharts>("exec procMoldStopReasonCostsWeb @StartDate, @EndDate",
                    //        new SqlParameter("StartDate", StDate),
                    //        new SqlParameter("EndDate", EDate)).ToList<StopReasonCharts>();

                    var data = ShrdMaster.Instance.MoldStopReasonCosts(con,StDate,EDate);

                    List<StopReasonCharts> SRC = new List<StopReasonCharts>();

                    foreach (var x in data)
                        {
                            if (x.MoldStopReason != null)
                            {

                            x.MoldStopReason = x.MoldStopReason.Replace("'","").Replace("&amp;", "&");

                                if (x.MoldStopReason.StartsWith("X"))
                                {
                                    SRC.Add(x);
                                }
                            }
                        }
                    List<StopReasonCharts> NewSRC = new List<StopReasonCharts>();
                    foreach (var x in SRC)
                    {
                        StopReasonCharts sd = new StopReasonCharts();
                        sd.LaborCost = x.LaborCost;
                        sd.LaborHours = x.LaborHours;
                        sd.MoldStopReason = x.MoldStopReason.Replace("'","");
                        sd.StopCount = x.StopCount;
                        sd.ToolingCost = x.ToolingCost;
                        sd.TotalCost = x.TotalCost;
                        NewSRC.Add(sd);
                    }


                    var MoldList = NewSRC.OrderByDescending(x => x.StopCount).Take(10).ToList();

                        var StopvsXStops = Db.Database.SqlQuery<PerformanceDashBoard>("exec procPerformacneDashboard @VarSystemDateTime, @CompanyID", new SqlParameter("@VarSystemDateTime", Date), new SqlParameter("@CompanyID", CID)).ToList<PerformanceDashBoard>();

                        var StopvsXStop = ShrdMaster.Instance.RenderRazorViewToString(this.ControllerContext, "_ScheduleStopvsXStop", dc);
                        var MoldStopReason = ShrdMaster.Instance.RenderRazorViewToString(this.ControllerContext, "_TopMoldStopReason", data.OrderByDescending(x => x.StopCount).Take(10).ToList());
                        var XStopReason = ShrdMaster.Instance.RenderRazorViewToString(this.ControllerContext, "_TopStopReasons", MoldList);

                        return Json(new { StopvsXStop, MoldStopReason, XStopReason }, JsonRequestBehavior.AllowGet);
                    }

                    catch (Exception ex)
                    {

                    }

            }
            else if (ChartType == 2)
            {
                try
                {
                    
                    string MoldStopReason = "";
                    string StopvsXStop = "";

                    if (MoldID == 0)
                    {
                        con.Open();
                        ViewBag.Title1 = "Top 10 Molds by Defect Count";
                        var dss1 = ShrdMaster.Instance.CostsPerRunTime(con, StDate, EDate, MoldID);

                        var dss = dss1.GroupBy(x => new { x.Mold, x.Description }).Select(s => new CostsPerRunTimeHourTwo
                        {
                            DefectCount = s.Sum(b => b.Defect),
                            MoldV = s.First().Mold,
                            DescriptionV = s.First().Description
                        }).OrderByDescending(x => x.DefectCount).Take(10).ToList();

                        MoldStopReason = ShrdMaster.Instance.RenderRazorViewToString(this.ControllerContext, "_MoldsByDefectCount", dss);

                        ViewBag.Title2 = "Top 10 Corrective Actions";
                        var ds = ShrdMaster.Instance.DefectndCAChart(con, StDate, EDate, 0);

                        var ts = ds.GroupBy(x => new { x.CorrectiveAction }).Select(s => new DefectandCAChartTwo
                        {
                            CorrectiveActionV = s.First().CorrectiveAction,
                            Q = s.Count()
                        }).OrderByDescending(x => x.Q).Take(10).ToList();

                        StopvsXStop = ShrdMaster.Instance.RenderRazorViewToString(this.ControllerContext, "_CorrectiveAction", ts);
                    }

                    else
                    {
                        var Mold = Db.TblMoldData.Where(x => x.MoldDataID == MoldID).FirstOrDefault();
                        ViewBag.Title1 = Mold.MoldName + " - " + "Top 10 Defect Count";
                        var dss1 = ShrdMaster.Instance.DefectbyMold(con, StDate, EDate, MoldID);

                        foreach (var x in dss1)
                        {
                            string sd = x.TroubleShootersDefects.Replace("'","");
                            x.TroubleShootersDefects = sd;
                        }

                        var dss = dss1.GroupBy(x => x.TroubleShootersDefects).Select(s => new DefectcMoldBlockandQualityTwo
                        {
                            TroubleShootersDefect = s.First().TroubleShootersDefects,
                            DefectCount = s.Sum(b => b.Count)
                        }).OrderByDescending(x => x.DefectCount).Take(10).ToList();


                        foreach (var x in dss)
                        {
                            if (x.TroubleShootersDefect != null || x.TroubleShootersDefect != "")
                            {
                                string[] sds = Regex.Split(x.TroubleShootersDefect, "</p>");
                                string s = HtmlToPlainText(sds[0]);
                                if (s != "")
                                {
                                    x.TroubleShootersDefect = RemoveSpecialCharacters(s.Replace(System.Environment.NewLine, string.Empty));
                                }
                            }
                        }


                        MoldStopReason = ShrdMaster.Instance.RenderRazorViewToString(this.ControllerContext, "_MoldBlockandQunatity", dss);

                        ViewBag.Title2 = Mold.MoldName + " - " + "Top 10 Corrective Actions";
                        var ds1 = ShrdMaster.Instance.DefectndCAChart(con, StDate, EDate, MoldID);

                        var ds = ds1.GroupBy(x => x.CorrectiveAction).Select(s => new DefectandCAChartTwo
                        {
                            CorrectiveActionV = s.First().CorrectiveAction,
                            Q = s.Count()
                        }).OrderByDescending(x => x.Q).Take(10).ToList();


                        StopvsXStop = ShrdMaster.Instance.RenderRazorViewToString(this.ControllerContext, "_CorrectiveAction", ds);
                    }

                    return Json(new { StopvsXStop, MoldStopReason }, JsonRequestBehavior.AllowGet);
                }

                catch (Exception ex)
                {

                }
            }
            else
            {
                try
                {

                    string MoldStopReason = "";
                    string StopvsXStop = "";

                    if (MoldID == 0)
                    {
                        con.Open();
                        //var data = CostsPerRunTime2(con, StDate, EDate, MoldID);
                        ViewBag.Title1 = "Top 10 Molds by Labor Cost";
                        var dss1 = ShrdMaster.Instance.CostsPerRunTime(con, StDate, EDate, MoldID);

                        var dss = dss1.GroupBy(x => new { x.Mold, x.Description }).Select(s => new CostsPerRunTimeHourThree
                        {
                            LaborCosts = s.Sum(b => b.LaborCost),
                            Mold = s.First().Mold + ": " + s.First().Description
                        }).OrderByDescending(x => x.LaborCosts).Take(10).ToList();

                        StopvsXStop = ShrdMaster.Instance.RenderRazorViewToString(this.ControllerContext, "_MoldsByLaborCosts", dss);

                        ViewBag.Title2 = "Top 10 Molds by Tooling Costs";
                        var ds2 = ShrdMaster.Instance.CostsPerRunTime(con, StDate, EDate, 0);
                        //var ds2 = ShrdMaster.Instance.DefectndCAChart(con, StDate, EDate, 0);

                        var ds = ds2.GroupBy(x => new { x.Mold, x.Description }).Select(s => new MoldToolingCostsTwo
                        {
                            TotalCost = s.Sum(b => b.ToolingCost),
                            Tooling = s.First().Mold + ": " + s.First().Description
                        }).OrderByDescending(x => x.TotalCost).Take(10).ToList();

                        MoldStopReason = ShrdMaster.Instance.RenderRazorViewToString(this.ControllerContext, "_MoldsByToolingCosts", ds);
                    }

                    else
                    {
                        //var ds1 = DefectndCAChart(con, StDate, EDate, MoldID);

                        var Mold = Db.TblMoldData.Where(x => x.MoldDataID == MoldID).FirstOrDefault();
                        ViewBag.Title1 = Mold.MoldName + " - " + "Top 10 Labor Cost";
                        var ds1 = ShrdMaster.Instance.DefectndCAChart(con, StDate, EDate, MoldID);
                        var ds = ds1.GroupBy(x => x.CorrectiveAction).Select(s => new CostsPerRunTimeHourThree
                        {
                            Mold = s.First().CorrectiveAction.Replace(@"""", ""),
                            LaborCosts = s.Sum(b => b.LaborCost)
                        }).OrderByDescending(x => x.LaborCosts).Take(10).ToList();
                         StopvsXStop = ShrdMaster.Instance.RenderRazorViewToString(this.ControllerContext, "_MoldsByLaborCosts", ds); 

                        ViewBag.Title2 =  Mold.MoldName + " - " + "Top 10 Tooling Cost";
                        var dss1 = ShrdMaster.Instance.MoldToolingCostsFunc(con, StDate, EDate, MoldID);

                        

                        var dss = dss1.GroupBy(x => x.Tooling).Select(s => new MoldToolingCostsTwo
                        {
                        Tooling = s.First().Tooling,
                        TotalCost = s.Sum(b => b.TotalCost)
                        }).OrderByDescending(x => x.TotalCost).Take(10).ToList();

                        List<MoldToolingCostsTwo> MTB = new List<MoldToolingCostsTwo>();

                        foreach (var x in dss)
                        {
                            MoldToolingCostsTwo MB = new MoldToolingCostsTwo();
                            if (x.Tooling != null)
                            {
                                string[] sds = Regex.Split(x.Tooling, "</p>");
                                string s = HtmlToPlainText(sds[0]);
                                if (s != "")
                                {
                                  s = s.Replace(System.Environment.NewLine, string.Empty);
                                  s = s.Replace("\n", " ");
                                  s = s.Replace(@"""", "");
                                }

                                MB.Tooling = s.Replace("&nbsp;", " ");
                            }

                            MB.TotalCost = x.TotalCost;
                            MTB.Add(MB);
                        }

                        MoldStopReason = ShrdMaster.Instance.RenderRazorViewToString(this.ControllerContext, "_MoldsByToolingCosts", MTB);

                    }

                    return Json(new { StopvsXStop, MoldStopReason }, JsonRequestBehavior.AllowGet);
                }

                catch (Exception ex)
                {

                }

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

        public static string ReplaceLink(Match m)
        {
            return m.ToString().Replace("\"", "'");
        }

        public static string RemoveSpecialCharacters(string str)
        {

            var NewVal = Regex.Replace(str, "[^0-9A-Za-z ,]", ",");
            //StringBuilder sb = new StringBuilder();
            //foreach (char c in str)
            //{
            //    if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
            //    {
            //        sb.Append(c);
            //    }
            //}

            return NewVal;
        }

        public void CommonDrop()
        {
            ViewBag.Companylist = new SelectList(Db.TblCompanies.ToList().OrderBy(x => x.CompanyName), "CompanyID", "CompanyName");
        }

        public ActionResult GetScheduledStopsXStops(DateTime StartDate, DateTime EndDate)
        {
            StDate = StartDate.ToString("yyyy-MM-dd hh:mm:ss");
            EDate = EndDate.ToString("yyyy-MM-dd hh:mm:ss");
            ViewBag.StartDate = StDate;
            ViewBag.EndDate = EDate;

            CommonDrop();

            var DC = ShrdMaster.Instance.ScheduleStopvsXStop(con, StDate, EDate);
            return View(DC);
        }

        public ActionResult GetStopReasonCosts(DateTime StartDate, DateTime EndDate)
        {
            StDate = StartDate.ToString("yyyy-MM-dd hh:mm:ss");
            EDate = EndDate.ToString("yyyy-MM-dd hh:mm:ss");

            ViewBag.StartDate = StDate;
            ViewBag.EndDate = EDate;
            CommonDrop();

            var DC = ShrdMaster.Instance.MoldStopReasonCosts(con, StDate, EDate); 
            return View(DC.OrderByDescending(x=> x.StopCount));
        }

        public ActionResult GetStopReasonCosts2(DateTime StartDate, DateTime EndDate)
        {
            StDate = StartDate.ToString("yyyy-MM-dd hh:mm:ss");
            EDate = EndDate.ToString("yyyy-MM-dd hh:mm:ss");

            ViewBag.StartDate = StDate;
            ViewBag.EndDate = EDate;
            CommonDrop();
            var DC = ShrdMaster.Instance.MoldStopReasonCosts(con, StDate, EDate);
            return View(DC.Where(x=> x.MoldStopReason.StartsWith("X")).OrderByDescending(x => x.StopCount));
        }

        public ActionResult CorrectiveActionReportData(DateTime StartDate, DateTime EndDate, int MID=0)
        {
            StDate = StartDate.ToString("yyyy-MM-dd hh:mm:ss");
            EDate = EndDate.ToString("yyyy-MM-dd hh:mm:ss");

            ViewBag.StartDate = StDate;
            ViewBag.EndDate = EDate;

            CommonDrop();
            var DC = ShrdMaster.Instance.DefectndCAChart(con, StDate, EDate, MID);
            return View(DC);
        }

        public ActionResult MoldBLockandQuality(DateTime StartDate, DateTime EndDate, int MID=0)
        {
            StDate = StartDate.ToString("yyyy-MM-dd hh:mm:ss");
            EDate = EndDate.ToString("yyyy-MM-dd hh:mm:ss");

            ViewBag.StartDate = StDate;
            ViewBag.EndDate = EDate;
            CommonDrop();
            if (MID == 0)
            {
                return RedirectToAction("CostsPerRunTimeCon", new { StDate, EDate, MID }); 
            }
            else
            {
                int MD = MID;
                if (MID == -1)
                {
                    MD = 0;
                }
                var data = ShrdMaster.Instance.DefectbyMold(con, StDate, EDate, MD);
                return View(data);
            }
        }

        public ActionResult CostsPerRunTimeCon(string StDate, string EDate, int MID = 0)
        {
            var data = ShrdMaster.Instance.CostsPerRunTime(con, StDate, EDate, MID);

            var MoldConfig = Db.TblDDMoldConfigs.ToList();
            var MoldConfig2 = Db.TblDDMoldConfig2s.ToList();
            CommonDrop();
            List<CostsPerRunTimeHourViewModel> CPRTH = new List<CostsPerRunTimeHourViewModel>();

            foreach (var x in data)
            {
                int Config = 0;
                int Config2 = 0;

                CostsPerRunTimeHourViewModel CPRT = new CostsPerRunTimeHourViewModel();
                CPRT.Mold = x.Mold;
                CPRT.Description = x.Description;

                if (x.Configuration != "" && x.Configuration != null)
                {
                    Config = Convert.ToInt32(x.Configuration);
                }
                var dataConfig = MoldConfig.Where(c => c.ID == Config).FirstOrDefault();
                x.Configuration = dataConfig != null ? dataConfig.MoldConfig : "";

                //if (x.MoldDataID == 69)
                //{

                //}

                if (x.Configuration2 != "" && x.Configuration2 != null)
                {
                    Config2 = Convert.ToInt32(x.Configuration2);
                }
                var dataConfig2 = MoldConfig2.Where(c => c.ID == Config2).FirstOrDefault();
                x.Configuration2 = dataConfig2 != null ? dataConfig2.MoldConfig : "";



                CPRT.Configuration = x.Configuration;
                CPRT.Configuration2 = x.Configuration2;
                CPRT.MoldStops = x.MoldStops;
                CPRT.Scheduled = x.Scheduled;
                CPRT.XStop = x.XStop;
                CPRT.XStopPercent = x.XStop != 0 ? ((x.XStop) * 100 / x.MoldStops) : 0;
                CPRT.Defect = x.Defect;
                CPRT.Quality = x.Quality;
                CPRT.Blocked = x.Blocked;
                CPRT.ToolingCost = x.ToolingCost;
                CPRT.LaborCost = x.LaborCost;
                CPRT.LaborHours = x.LaborHours;
                CPRT.TotalCost = x.TotalCost;
                CPRT.RunTimeMinutes = x.RunTimeMinutes;
                CPRT.RunTime = x.RunTime;
                CPRT.TotalRunTimeHours = x.TotalRunTimeHours;
                CPRT.CostPerHour = x.CostPerHour;
                CPRT.CycleTimeSec = x.CycleTimeSec;
                CPRT.TotalActualCyclesRun = x.TotalActualCyclesRun;

                double CycleRunPerLakh = x.TotalActualCyclesRun != 0 ? x.TotalActualCyclesRun / 100000 : 0;
                CPRT.CycleRunPerLakh = double.IsInfinity(CycleRunPerLakh) ? 0 : CycleRunPerLakh;


                double CostsperLakhCycle = x.TotalCost == 0 ? 0 : x.TotalActualCyclesRun != 0 ? (x.TotalCost / (x.TotalActualCyclesRun / 100000)) : 0;
                CPRT.CostsperLakhCycle = double.IsInfinity(CostsperLakhCycle) ? 0 : CostsperLakhCycle;

                double CostPerDefect = x.TotalCost != 0 ? x.TotalCost / x.Defect : 0;
                CPRT.CostPerDefect = double.IsInfinity(CostPerDefect) ? 0 : CostPerDefect;

                double CyclesPerDefect = x.TotalActualCyclesRun != 0 ? x.TotalActualCyclesRun / x.Defect : 0;
                CPRT.CyclesPerDefect = double.IsInfinity(CyclesPerDefect) ? 0 : CyclesPerDefect;

                double RunHrsPerDefect = x.TotalRunTimeHours != 0 ? x.TotalRunTimeHours / x.Defect : 0;
                CPRT.RunHrsPerDefect = double.IsInfinity(RunHrsPerDefect) ? 0 : RunHrsPerDefect;
                CPRTH.Add(CPRT);
            }
          
            return View(CPRTH);
        }


        public ActionResult LaborCostsFunc(DateTime StartDate, DateTime EndDate, int MID = 0)
        {
            StDate = StartDate.ToString("yyyy-MM-dd hh:mm:ss");
            EDate = EndDate.ToString("yyyy-MM-dd hh:mm:ss");
            CommonDrop();
            ViewBag.StartDate = StDate;
            ViewBag.EndDate = EDate;

            if (MID == 0)
            {
                return RedirectToAction("CostsPerRunTimeCon", new { StDate, EDate, MID });
            }
            else
            {
                return RedirectToAction("CorrectiveActionReportData", new { StartDate, EndDate, MID });
            }

        }

        public ActionResult ToolingCostFunc(DateTime StartDate, DateTime EndDate, int MID = 0)
        {
            StDate = StartDate.ToString("yyyy-MM-dd hh:mm:ss");
            EDate = EndDate.ToString("yyyy-MM-dd hh:mm:ss");

            ViewBag.StartDate = StDate;
            ViewBag.EndDate = EDate;

            CommonDrop();
            if (MID == 0)
            {
                return RedirectToAction("CostsPerRunTimeCon", new { StDate, EDate, MID });
            }
            else
            {
                return RedirectToAction("MoldToolingCostsContr", new { StDate, EDate, MID });
            }
        }

        public ActionResult MoldToolingCostsContr(string StDate, string EDate, int MID = 0)
        {
            CommonDrop();
            var data = ShrdMaster.Instance.MoldToolingCostsFunc(con, StDate, EDate, MID);
            return View(data);
        }

        public ActionResult GetDrillReport(string Name="", string StDate="", string EDate="")
        {
            string NewName = HttpUtility.UrlDecode(Name);
            List<DefectandCAChart> data = ShrdMaster.Instance.DefectndCAChartDrillDown(con,StDate,EDate,0, NewName);
            CommonDrop();
            return View(data.ToList());
        }
    }
}