using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Reporting.WebForms;
using Moldtrax.Models;
using Moldtrax.Providers;
using Newtonsoft.Json;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Moldtrax.Controllers
{
    [SessionExpireFilter]
    public class MaintenanceAlertStatController : Controller
    {
        private MoldtraxDbContext db = new MoldtraxDbContext();
        string Date = System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
        static string constring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        SqlConnection con = new SqlConnection(constring);

        // GET: MaintenanceAlertStatsss
        public ActionResult Index()
        {
            ViewBag.Companylist = new SelectList(db.TblCompanies.ToList().OrderBy(x => x.CompanyName), "CompanyID", "CompanyName");
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.Database.SqlQuery<PerformanceDashBoard>("exec procPerformacneDashboard @VarSystemDateTime, @CompanyID", new SqlParameter("@VarSystemDateTime", Date), new SqlParameter("@CompanyID",CID)).ToList();
            var data1 = db.Database.SqlQuery<PerformanceDashBoard>("exec procPerformacneDashboardByCycleCount @VarSystemDateTime, @CompanyID", new SqlParameter("@VarSystemDateTime", Date), new SqlParameter("@CompanyID",CID)).ToList();

            var newdata = ShrdMaster.Instance.GetCurrentMoldRunning(con);
            var newdata1 = ShrdMaster.Instance.GetCurrentMoldRunning2(con);

            MainCurrentMoldRunning ds = new MainCurrentMoldRunning();

            ds.CurrentMoldRunning1 = newdata;

            List<CurrentMoldRunning> CMR = new List<CurrentMoldRunning>();
            //foreach (var x in newdata1)
            //{
            //    CurrentMoldRunning CM = new CurrentMoldRunning();
            //    CM.Percentage = x.Percentage;
            //    CM.Status = x.Status;
            //    CM.TotalRunning = x.TotalRunning;
            //    CM.Count = x.Count;
            //    if (x.Status == "PM Approaching")
            //    {
            //        CM.Color = "Yellow";
            //    }
            //    else if (x.Status == "PM Good")
            //    {
            //        CM.Color = "Green";
            //    }
            //    else if (x.Status == "PM Past Due")
            //    {
            //        CM.Color = "Red";
            //    }
            //    else
            //    {
            //        CM.Color = "White";
            //    }

            //    CMR.Add(CM);
            //}

            //ds.CurrentMoldRunning2 = CMR;

            ds.CurrentMoldRunning2 = newdata1.OrderBy(x=> x.Status).ToList();


            int i = 0;
            foreach (var x in newdata)
            {
                i += x.Count;
            }

            int i1 = 0;
            foreach (var x in newdata1)
            {
                i1 += x.Count;
            }

            ViewBag.CurrentMold = i;
            ViewBag.CurrentMold2 = i1;
            return View(ds);
        }

        public ActionResult GetData()
        {
            List<DataPoint> DP = new List<DataPoint>();

            DP.Add(new DataPoint() { Name = "Fruit", Y = 26 });
            DP.Add(new DataPoint() { Name = "Protein", Y = 20 });
            DP.Add(new DataPoint() { Name = "Vegetables", Y = 5 });
            DP.Add(new DataPoint() { Name = "Dairy", Y = 3 });
            DP.Add(new DataPoint() { Name = "Grains", Y = 7 });
            DP.Add(new DataPoint() { Name = "Fat", Y = 30 });
            DP.Add(new DataPoint() { Name = "Others", Y = 17 });

            return Json(DP.Select(p => new { p.Name, p.Y }), JsonRequestBehavior.AllowGet);
            //var key = new Chart(width: 600, height: 400)
            //    .AddSeries(
            //    chartType: "pie",
            //    legend: "Rainfall",
            //    xValue: new[] { "Jan", "Feb","Mar","Apr","May" },
            //    yValues: new[] { "23", "34","12","44","14" }).Write();

            //return null;
        }

        public ActionResult PrintPDF()
        {
            using (MoldtraxDbContext db = new MoldtraxDbContext())
            {
                var data = db.Database.SqlQuery<PerformanceDashBoard>("exec procPerformacneDashboard @VarSystemDateTime", new SqlParameter("@VarSystemDateTime", Date)).ToList<PerformanceDashBoard>();
                var report = new PartialViewAsPdf("_MaintenanceAlert", data.FirstOrDefault());
                return report;
            }
        }

        public ActionResult PrintFile(int Print = 0)
        {

            string deviceInfo = "";
            LocalReport localReport = new LocalReport();
            ReportViewer reportViewer = new ReportViewer();

            string ReportPath = "";
            //Statistical Print
            List<PMAlertReports> PMD = new List<PMAlertReports>();

            var DDD = ShrdMaster.Instance.GetTblRoverSetData();

            foreach (var s in db.TblMoldData.ToList())
            {
                var AllData = DDD.Where(d => d.MoldDataID == s.MoldDataID).ToList();

                if (AllData.Count() > 0)
                {
                    int? TotalLifeCycles = 0;

                    foreach (var dd in AllData)
                    {
                        TotalLifeCycles += dd.CycleCounter;
                    }

                    var data = AllData.OrderByDescending(d => d.SetID).FirstOrDefault();

                    string date = data.SetDate.Value.ToShortDateString();

                    if (date != "" && date != "1/1/0001" && date != "01/01/0001")
                    {
                        int ConfigID1 = 0;
                        int ConfigID2 = 0;

                        if (data.MoldConfig != null && data.MoldConfig != "")
                        {
                            ConfigID1 = Convert.ToInt32(data.MoldConfig);
                        }

                        if (data.MoldConfig2 != null && data.MoldConfig2 != "")
                        {
                            ConfigID2 = Convert.ToInt32(data.MoldConfig2);
                        }

                        var Config1 = db.TblDDMoldConfigs.Where(x => x.ID == ConfigID1).FirstOrDefault();
                        var Config2 = db.TblDDMoldConfig2s.Where(x => x.ID == ConfigID2).FirstOrDefault();

                        PMAlertReports PM = new PMAlertReports();
                        PM.Press = data.SetPressNumb;
                        PM.Mold = s.MoldName;
                        PM.Description = s.MoldDesc;
                        PM.Config1 = Config1 == null ? "" : Config1.MoldConfig;
                        PM.Config2 = Config2 == null ? "" : Config2.MoldConfig;
                        PM.StartDate = data.SetDate;
                        PM.StopDate = Convert.ToDateTime(data.MldPullDate).ToShortDateString() == "1/1/0001" ? "no stop date" : Convert.ToDateTime(data.MldPullDate).ToShortDateString();
                        PM.NewStopDate = Convert.ToDateTime(data.MldPullDate).ToShortDateString() == "1/1/0001" ? Convert.ToDateTime("01/01/3000") : Convert.ToDateTime(data.MldPullDate);

                        if (s.MoldOutPressPMYellowCycles == null && s.MoldOutPressPMRedCycles == null)
                        {
                            PM.CyclesToReachYellowLimits = 0;
                            PM.CyclesToReachRedLimits = 0;
                            PM.HoursToReachYellowLimits = 0;
                            PM.HoursToReachRedLimits = 0;

                            PM.NewCyclesToReachYellowLimits = 100000000;
                            PM.NewCyclesToReachRedLimits = 100000000;
                            PM.NewHoursToReachYellowLimits = 100000000;
                            PM.NewHoursToReachRedLimits = 100000000;

                            PM.Color = "Default";
                        }
                        else
                        {

                            PM.CyclesToReachYellowLimits = Convert.ToInt32(s.MoldOutPressPMYellowCycles) - Convert.ToInt32(TotalLifeCycles);
                            PM.CyclesToReachRedLimits = Convert.ToInt32(s.MoldOutPressPMRedCycles) - Convert.ToInt32(TotalLifeCycles);
                            PM.HoursToReachYellowLimits = (PM.CyclesToReachYellowLimits * s.MoldCyclesPerMinute) / 3600;
                            PM.HoursToReachRedLimits = (PM.CyclesToReachRedLimits * s.MoldCyclesPerMinute) / 3600;


                            PM.NewCyclesToReachYellowLimits = Convert.ToInt32(s.MoldOutPressPMYellowCycles) - Convert.ToInt32(TotalLifeCycles);
                            PM.NewCyclesToReachRedLimits = Convert.ToInt32(s.MoldOutPressPMRedCycles) - Convert.ToInt32(TotalLifeCycles);
                            PM.NewHoursToReachYellowLimits = (PM.CyclesToReachYellowLimits * s.MoldCyclesPerMinute) / 3600;
                            PM.NewHoursToReachRedLimits = (PM.CyclesToReachRedLimits * s.MoldCyclesPerMinute) / 3600;


                            if (TotalLifeCycles >= s.MoldOutPressPMRedCycles)
                            {
                                PM.Color = "Red";
                            }
                            else if (TotalLifeCycles >= s.MoldOutPressPMYellowCycles)
                            {
                                PM.Color = "Yellow";
                            }
                            else
                            {
                                PM.Color = "Green";
                            }
                        }


                        PMD.Add(PM);
                    }
                }
            }

            ReportPath = @"Reports\PMAlertReport.rdlc";
                localReport.DataSources.Add(new ReportDataSource("DataSet1", PMD.OrderBy(X=> X.Mold)));

                reportViewer.ProcessingMode = ProcessingMode.Local;
                reportViewer.SizeToReportContent = true;

                deviceInfo = @"<DeviceInfo>
                <OutputFormat>PDF</OutputFormat>
                <PageWidth>14.0in</PageWidth>
                <PageHeight>8.27in</PageHeight>
                <MarginTop>0.25in</MarginTop>
                <MarginLeft>0.25in</MarginLeft>
                <MarginRight>0.25in</MarginRight>
                <MarginBottom>0.25in</MarginBottom>
            </DeviceInfo>";

            //else
            //{
            //    var data = ShrdMaster.Instance.GetMaintenanceAlertData2(con);
            //    ReportPath = @"Reports\MaintenanceAlertStats.rdlc";
            //    localReport.DataSources.Add(new ReportDataSource("DataSet1", data));

            //    reportViewer.ProcessingMode = ProcessingMode.Local;
            //    reportViewer.SizeToReportContent = true;

            //    deviceInfo = @"<DeviceInfo>
            //    <OutputFormat>PDF</OutputFormat>
            //    <PageWidth>11.69in</PageWidth>
            //    <PageHeight>8.27in</PageHeight>
            //    <MarginTop>0.25in</MarginTop>
            //    <MarginLeft>0.25in</MarginLeft>
            //    <MarginRight>0.25in</MarginRight>
            //    <MarginBottom>0.25in</MarginBottom>
            //</DeviceInfo>";

            //}

            localReport.ReportPath = ReportPath;

            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension = "pdf";

            Warning[] warnings;

            string[] streams;

            byte[] renderedBytes;

            string StorePDFPath = @"C:\Websites\Moldtrax.infodatixhosting.com\Reports\Summary.pdf";
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

        public ActionResult ShowViewData()
        {
            //var data = db.Database.SqlQuery<MaintenanceAlertStats>("exec ")   ShrdMaster.Instance.MaintenanceAlertStats(con);
            //var data = db.Database.SqlQuery<MaintenanceAlertStats>("exec procMaintenanceAlert @SystemDateTime", new SqlParameter("@SystemDateTime", Date)).ToList<MaintenanceAlertStats>();
            var data = ShrdMaster.Instance.GetMaintenanceAlertData(con, Date);
            ReportViewer reportViewer = new ReportViewer();
            List<MaintenanceAlertStats> MTS = new List<MaintenanceAlertStats>();
            
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\MaintenanceAlertStats.rdlc";
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", data));

            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            //reportViewer.Width = Unit.Percentage(80);
            //reportViewer.Height = Unit.Percentage(80);

            ViewBag.ReportViewer = reportViewer;

            return View(data);
        }

        public ActionResult ShowViewData2()
        {
            var data = ShrdMaster.Instance.GetMaintenanceAlertData2(con);
            ReportViewer reportViewer = new ReportViewer();
            List<MaintenanceAlertStats> MTS = new List<MaintenanceAlertStats>();

            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\MaintenanceAlertStats.rdlc";
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", data));

            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            //reportViewer.Width = Unit.Percentage(80);
            //reportViewer.Height = Unit.Percentage(80);

            ViewBag.ReportViewer = reportViewer;

            return View(data);
        }

        public ActionResult GetPMALertReport()
        {
            List<PMAlertReports> PMD = new List<PMAlertReports>();

            var DDD = ShrdMaster.Instance.GetTblRoverSetData();
            int SortOrder = 1;
            ViewBag.Companylist = new SelectList(db.TblCompanies.ToList().OrderBy(x => x.CompanyName), "CompanyID", "CompanyName");
            foreach (var s in db.TblMoldData.ToList())
            {
                var AllData = DDD.Where(d => d.MoldDataID == s.MoldDataID).ToList();

                if (AllData.Count() > 0)
                {
                    int? TotalLifeCycles = 0;

                    foreach (var dd in AllData)
                    {
                        TotalLifeCycles += dd.CycleCounter;
                    }

                    var data = AllData.OrderByDescending(d => d.SetID).FirstOrDefault();

                    string date = data.SetDate.Value.ToShortDateString();

                    if (date != "" && date != "1/1/0001" && date != "01/01/0001") 
                    {
                        int ConfigID1 = 0;
                        int ConfigID2 = 0;

                        if (data.MoldConfig != null && data.MoldConfig != "")
                        {
                            ConfigID1 = Convert.ToInt32(data.MoldConfig);
                        }

                        if (data.MoldConfig2 != null && data.MoldConfig2 != "")
                        {
                            ConfigID2 = Convert.ToInt32(data.MoldConfig2);
                        }

                        var Config1 = db.TblDDMoldConfigs.Where(x => x.ID == ConfigID1).FirstOrDefault();
                        var Config2 = db.TblDDMoldConfig2s.Where(x => x.ID == ConfigID2).FirstOrDefault();

                        PMAlertReports PM = new PMAlertReports();
                        PM.Press = data.SetPressNumb;
                        PM.Mold = s.MoldName;
                        PM.Description = s.MoldDesc;
                        PM.Config1 = Config1 == null ? "" : Config1.MoldConfig;
                        PM.Config2 = Config2 == null ? "" : Config2.MoldConfig;
                        PM.StartDate = data.SetDate;
                        PM.StopDate = Convert.ToDateTime(data.MldPullDate).ToShortDateString() == "1/1/0001" ? "no stop date" : Convert.ToDateTime(data.MldPullDate).ToShortDateString();
                        PM.NewStopDate = Convert.ToDateTime(data.MldPullDate).ToShortDateString() == "1/1/0001" ? Convert.ToDateTime("01/01/3000") : Convert.ToDateTime(data.MldPullDate);

                        if (s.MoldOutPressPMYellowCycles == null && s.MoldOutPressPMRedCycles == null)
                        {
                            PM.CyclesToReachYellowLimits = 0;
                            PM.CyclesToReachRedLimits = 0;
                            PM.HoursToReachYellowLimits = 0;
                            PM.HoursToReachRedLimits = 0;

                            PM.NewCyclesToReachYellowLimits= 100000000;
                            PM.NewCyclesToReachRedLimits = 100000000;
                            PM.NewHoursToReachYellowLimits = 100000000;
                            PM.NewHoursToReachRedLimits = 100000000;

                            PM.Color = "Default";
                        }
                        else
                        {

                            PM.CyclesToReachYellowLimits = Convert.ToInt32(s.MoldOutPressPMYellowCycles) - Convert.ToInt32(TotalLifeCycles);
                            PM.CyclesToReachRedLimits = Convert.ToInt32(s.MoldOutPressPMRedCycles) - Convert.ToInt32(TotalLifeCycles);
                            PM.HoursToReachYellowLimits = (PM.CyclesToReachYellowLimits * s.MoldCyclesPerMinute) / 3600;
                            PM.HoursToReachRedLimits = (PM.CyclesToReachRedLimits * s.MoldCyclesPerMinute) / 3600;


                            PM.NewCyclesToReachYellowLimits = Convert.ToInt32(s.MoldOutPressPMYellowCycles) - Convert.ToInt32(TotalLifeCycles);
                            PM.NewCyclesToReachRedLimits = Convert.ToInt32(s.MoldOutPressPMRedCycles) - Convert.ToInt32(TotalLifeCycles);
                            PM.NewHoursToReachYellowLimits = (PM.CyclesToReachYellowLimits * s.MoldCyclesPerMinute) / 3600;
                            PM.NewHoursToReachRedLimits = (PM.CyclesToReachRedLimits * s.MoldCyclesPerMinute) / 3600;


                            if (TotalLifeCycles >= s.MoldOutPressPMRedCycles)
                            {
                                PM.Color = "Red";
                            }
                            else if (TotalLifeCycles >= s.MoldOutPressPMYellowCycles)
                            {
                                PM.Color = "Yellow";
                            }
                            else
                            {
                                PM.Color = "Green";
                            }
                        }


                        PMD.Add(PM);
                    }
                }
            }

            foreach (var x in PMD.OrderByDescending(z => z.NewStopDate))
            {
                if (x.NewStopDate.ToShortDateString() == "01/01/3000")
                {
                    x.NewStopSortOrder = 0;
                }
                else
                {
                    x.NewStopSortOrder = SortOrder++;
                }
            }
            


            ReportViewer reportViewer = new ReportViewer();
            reportViewer.LocalReport.EnableExternalImages = true;

            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\PMAlertReport.rdlc";
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", PMD.OrderBy(x=> x.Mold)));

            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(80);
            reportViewer.Height = Unit.Percentage(80);
            reportViewer.PageCountMode = PageCountMode.Actual;

            ViewBag.ReportViewer = reportViewer;
            return View();

        }
    }
}