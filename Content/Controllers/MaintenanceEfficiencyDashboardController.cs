using Moldtrax.Models;
using Moldtrax.Providers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Moldtrax.Controllers
{
    [SessionExpireFilter]
    public class MaintenanceEfficiencyDashboardController : Controller
    {
        private MoldtraxDbContext Db = new MoldtraxDbContext();
        static string constring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        SqlConnection con = new SqlConnection(constring);
        static DateTime StartDate = new DateTime(2000, 04, 01);
        static DateTime EndDate = new DateTime(2018, 06, 06);

        // GET: MaintenanceEfficiencyDashboard
        public ActionResult Index()
        {
            CommonDrop();
            var date = Db.TblMoldCharts.Where(x => x.ID == 1).FirstOrDefault();
            string StDate = date.StartDate.ToString("yyyy-MM-dd hh:mm:ss");
            string EDate = date.EndDate.ToString("yyyy-MM-dd hh:mm:ss");

            ViewBag.StDate = StDate;
            ViewBag.EDate = EDate;
            return View();
        }


        public void CommonDrop()
        {
            ViewBag.Companylist = new SelectList(Db.TblCompanies.ToList().OrderBy(x => x.CompanyName), "CompanyID", "CompanyName");
        }

        

        public ActionResult ShowChart(int ChartType, DateTime StartDate, DateTime EndDate)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            string StDate = StartDate.ToString("yyyy-MM-dd hh:mm:ss");
            string EDate = EndDate.ToString("yyyy-MM-dd hh:mm:ss");

            string FirstChart = "";
            string SecondChart = "";

            var date = Db.TblMoldCharts.Where(x => x.ID == 1).FirstOrDefault();
            date.StartDate = StartDate;
            date.EndDate = EndDate;
            Db.SaveChanges();


            if (ChartType == 1)
            {
                var data = ShrdMaster.Instance.ScheduleStopvsXStop(con, StDate, EDate);
                var dss = data.GroupBy(x => new { x.RepairTech }).Select(s => new CommonChartProp
                {
                    Name = s.First().RepairTech,
                    Quantity = Math.Round(s.Average(p => p.DoD), 2),
                }).Where(x=> x.Name != null).OrderBy(x => x.Name).ToList();

                FirstChart = ShrdMaster.Instance.RenderRazorViewToString(this.ControllerContext, "_AvgMoldDegreeofDifficulty", dss);

                var dss1 = data.GroupBy(x => new { x.RepairTech }).Select(s => new CommonChartProp
                {
                    Name = s.First().RepairTech,
                    Quantity = s.Count(),
                }).Where(x => x.Name != null).OrderBy(x => x.Name).ToList();

                SecondChart = ShrdMaster.Instance.RenderRazorViewToString(this.ControllerContext, "_RepairSheetAssignedCount", dss1);

                return Json(new { FirstChart, SecondChart }, JsonRequestBehavior.AllowGet);
            }
            else if (ChartType == 2)
            {
                var data = ShrdMaster.Instance.DefectndCAChart(con, StDate, EDate, 0);
                var dss = data.GroupBy(x => new { x.CATech }).Select(s => new CommonChartProp
                {
                    Name = s.First().CATech,
                    Quantity = s.Count(),
                }).Where(x => x.Name != null).OrderBy(x => x.Name).ToList();

                FirstChart = ShrdMaster.Instance.RenderRazorViewToString(this.ControllerContext, "_CorrectiveActionPerformedByTech", dss);

                var data2 = ShrdMaster.Instance.DefectPositionAnalysis(con, StDate, EDate);

                var dss1 = data2.GroupBy(x => new { x.BlockedByNotedBy }).Select(s => new CommonChartProp
                {
                    Name = s.First().BlockedByNotedBy,
                    Quantity = s.Count(),
                }).Where(x => x.Name != null).OrderByDescending(x => x.Name).Take(10).ToList();

                SecondChart = ShrdMaster.Instance.RenderRazorViewToString(this.ControllerContext, "_DefectDiscoveredByTech", dss1);

                return Json(new { FirstChart, SecondChart }, JsonRequestBehavior.AllowGet);
            }
            else if (ChartType == 3)
            {
                var data = ShrdMaster.Instance.DefectndCAChart(con, StDate, EDate, 0);
                var dss = data.GroupBy(x => new { x.CATech }).Select(s => new CommonChartProp
                {
                    Name = s.First().CATech,
                    Quantity = Math.Round(s.Sum(v => v.LaborCost), 0),
                }).Where(x => x.Name != null).OrderBy(x => x.Name).ToList();

                FirstChart = ShrdMaster.Instance.RenderRazorViewToString(this.ControllerContext, "_LaborCostByTech", dss);

                var dss1 = data.GroupBy(x => new { x.CATech }).Select(s => new CommonChartProp
                {
                    Name = s.First().CATech,
                    Quantity = Math.Round(s.Sum(v => v.ToolingCost), 0),
                }).Where(x => x.Name != null).OrderBy(x => x.Name).ToList();

                SecondChart = ShrdMaster.Instance.RenderRazorViewToString(this.ControllerContext, "_ToolingCostByTech", dss1);

                return Json(new { FirstChart, SecondChart }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var data = ShrdMaster.Instance.DefectndCAChart(con, StDate, EDate, 0);
                var dss = data.GroupBy(x => new { x.CATech }).Select(s => new CommonChartProp
                {
                    Name = s.First().CATech,
                    Quantity = Math.Round(s.Sum(v => v.TotalCost), 0),
                }).Where(x => x.Name != null).OrderBy(x => x.Name).ToList();

                FirstChart = ShrdMaster.Instance.RenderRazorViewToString(this.ControllerContext, "_TotalCostofRepairPerTech", dss);
                return Json(new { FirstChart }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AvgMoldDegreeofDifficulty(DateTime StartDate, DateTime EndDate)
        {
            CommonDrop();
            string StDate = StartDate.ToString("yyyy-MM-dd hh:mm:ss");
            string EDate = EndDate.ToString("yyyy-MM-dd hh:mm:ss");
            var data = ShrdMaster.Instance.ScheduleStopvsXStop(con, StDate, EDate);
            return View(data);
        }

        public ActionResult CorrectiveActionsPerformedbyTech(DateTime StartDate, DateTime EndDate)
        {
            CommonDrop();
            string StDate = StartDate.ToString("yyyy-MM-dd hh:mm:ss");
            string EDate = EndDate.ToString("yyyy-MM-dd hh:mm:ss");
            var data = ShrdMaster.Instance.DefectndCAChart(con, StDate, EDate,0);
            return View(data.OrderByDescending(x=> x.TroubleShootersDefect));
        }

        public ActionResult DefectDiscoveredbyTech(DateTime StartDate, DateTime EndDate)
        {
            CommonDrop();
            string StDate = StartDate.ToString("yyyy-MM-dd hh:mm:ss");
            string EDate = EndDate.ToString("yyyy-MM-dd hh:mm:ss");
            var data = ShrdMaster.Instance.DefectPositionAnalysis(con, StDate, EDate);
            return View(data);
        }


    }
}