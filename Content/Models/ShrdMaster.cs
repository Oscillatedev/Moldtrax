using Aspose.Diagram;
using ConvertApiDotNet;
using iTextSharp.text;
using Moldtrax.ViewMoldel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web; 
using System.Web.Mvc;

namespace Moldtrax.Models
{
    public class ShrdMaster : Controller
    {
        //string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        static string constring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        SqlConnection con = new SqlConnection(constring);
        //ADODB.ConnectionClass con = new ADODB.ConnectionClass();
        //ADODB.RecordsetClass rs = new ADODB.RecordsetClass();

        private MoldtraxDbContext db = new MoldtraxDbContext();

        private static ShrdMaster _instance;

        public static ShrdMaster Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ShrdMaster();
                }
                return _instance;
            }
        }

        public AccessData CallGetAccess(string User = "", string form = "")
        {
            using (SqlConnection con = new SqlConnection(constring))
            {
                DataTable table = new DataTable();
                SqlCommand cmd = new SqlCommand("procGetAccess", con);
                cmd.Parameters.AddWithValue("@UserID", User);
                cmd.Parameters.AddWithValue("@FormName", form);
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    da.Fill(table);
                }

                var DataList = ConvertDataTableIntoList(table).FirstOrDefault();
                return DataList;
            }
        }

        public string StripHtml(string source)
        {
            string output;

            //get rid of HTML tags
            output = Regex.Replace(source, "<[^>]*>", string.Empty);

            //get rid of multiple blank lines
            output = Regex.Replace(output, @"^\s*$\n", string.Empty, RegexOptions.Multiline);

            return output;
        }

        public List<AccessData> ConvertDataTableIntoList(DataTable dt)
        {
            List<AccessData> ADList = new List<AccessData>();

            foreach (DataRow row in dt.Rows)
            {
                AccessData AD = new AccessData();
                AD.ID = !Convert.IsDBNull(row["Id"]) ? Convert.ToInt32(row["Id"]) : 0;
                AD.ObjectType = !Convert.IsDBNull(row["ObjectType"]) ? Convert.ToInt32(row["ObjectType"]) : 0;
                AD.ControlTypeName = row["ControlTypeName"].ToString();
                AD.Permission = row["Permission"].ToString();
                AD.Level = !Convert.IsDBNull(row["Level"]) ? Convert.ToInt32(row["Level"]) : 0;
                AD.ControlTip = row["ControlTip"].ToString();
                AD.NoAccess = !Convert.IsDBNull(row["No Access"]) ? Convert.ToInt32(row["No Access"]) : 0;
                AD.AllowEdits = !Convert.IsDBNull(row["AllowEdits"]) ? Convert.ToInt32(row["AllowEdits"]) : 0;
                AD.AllowDeletions = !Convert.IsDBNull(row["AllowDeletions"]) ? Convert.ToInt32(row["AllowDeletions"]) : 0;
                AD.AllowAdditions = !Convert.IsDBNull(row["AllowAdditions"]) ? Convert.ToInt32(row["AllowAdditions"]) : 0;
                AD.DataEntry = !Convert.IsDBNull(row["DataEntry"]) ? Convert.ToInt32(row["DataEntry"]) : 0;
                AD.Visible = !Convert.IsDBNull(row["Visible"]) ? Convert.ToInt32(row["Visible"]) : 0;
                AD.Enabled = !Convert.IsDBNull(row["Enabled"]) ? Convert.ToInt32(row["Enabled"]) : 0;
                AD.Locked = !Convert.IsDBNull(row["Locked"]) ? Convert.ToInt32(row["Locked"]) : 0;
                AD.OperatorStamp = !Convert.IsDBNull(row["OperatorStamp"]) ? Convert.ToDateTime(row["OperatorStamp"]) : new DateTime();
                AD.DateTimeStamp = !Convert.IsDBNull(row["DateTimeStamp"]) ? Convert.ToDateTime(row["DateTimeStamp"]) : new DateTime();
                AD.SSMA_TimeStamp = row["SSMA_TimeStamp"].ToString();
                ADList.Add(AD);
            }

            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    AccessData AD = new AccessData();
            //    AD.ID = Convert.ToInt32(dt.Rows[i]["Id"]);
            //    AD.ObjectType = Convert.ToInt32(dt.Rows[i]["ObjectType"]);
            //    AD.ControlTypeName = dt.Rows[i]["ControlTypeName"].ToString();
            //    AD.Permission = dt.Rows[i]["Permission"].ToString();
            //    AD.Level = Convert.ToInt32(dt.Rows[i]["Level"]);
            //    AD.ControlTip = dt.Rows[i]["ControlTip"].ToString();
            //    AD.NoAccess = Convert.ToInt32(dt.Rows[i]["No Access"]);
            //    AD.AllowEdits = Convert.ToInt32(dt.Rows[i]["AllowEdits"]);
            //    AD.AllowDeletions = Convert.ToInt32(dt.Rows[i]["AllowDeletions"]);
            //    AD.AllowAdditions = Convert.ToInt32(dt.Rows[i]["AllowAdditions"]);
            //    AD.DataEntry = Convert.ToInt32(dt.Rows[i]["DataEntry"]);
            //    AD.Visible = Convert.ToInt32(dt.Rows[i]["Visible"]);
            //    AD.Enabled = Convert.ToInt32(dt.Rows[i]["Enabled"]);
            //    AD.Locked = Convert.ToInt32(dt.Rows[i]["Locked"]);
            //    AD.OperatorStamp = Convert.ToDateTime(dt.Rows[i]["OperatorStamp"]);
            //    AD.DateTimeStamp = Convert.ToDateTime(dt.Rows[i]["DateTimeStamp"]);
            //    AD.SSMA_TimeStamp = dt.Rows[i]["SSMA_TimeStamp"].ToString();
            //    ADList.Add(AD);
            //}
            return ADList;
        }

        public string ReturnUniqueName()
        {
            long ticks = DateTime.Now.Ticks;
            byte[] bytes = BitConverter.GetBytes(ticks);
            string id = Convert.ToBase64String(bytes)
                                    .Replace('+', '_')
                                    .Replace('/', '-')
                                    .TrimEnd('=');
            return id;
        }

        public IEnumerable<tblMoldTooling> GetToolingList(int ID, int CID)
        {
            //var data = db.Database.SqlQuery<tblMoldTooling>("exec sp_GetToolingData").ToList();
            var ds = new MoldtraxDbContext();
            var data = ds.TblMoldTooling.Where(x => x.MoldDataID == ID && x.CompanyID == CID).ToList();
            //var NewData = data.Where(x => x.MoldDataID == ID).ToList();
            return data;
        }


        public string RenderRazorViewToString(ControllerContext controllerContext,
    string viewName, object model)
        {
            controllerContext.Controller.ViewData.Model = model;

            using (var stringWriter = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
                var viewContext = new ViewContext(controllerContext, viewResult.View, controllerContext.Controller.ViewData, controllerContext.Controller.TempData, stringWriter);
                viewResult.View.Render(viewContext, stringWriter);
                viewResult.ViewEngine.ReleaseView(controllerContext, viewResult.View);
                return stringWriter.GetStringBuilder().ToString();
            }
        }


        public TotalCycleCount GetTotalAvailableCycle(SqlConnection con, int MID)
        {
            SqlDataAdapter da = new SqlDataAdapter("proTotalCycleCountAsp", con);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.Add("@MoldDataID", SqlDbType.Int).Value = MID;
            da.SelectCommand.Parameters.Add("@CompanyID", SqlDbType.Int).Value = GetCompanyID();

            //da.SelectCommand.Parameters.Add("@MoldDataID", SqlDbType.Int).Value = MoldID;

            DataTable dt = new DataTable();
            da.Fill(dt);

            List<TotalCycleCount> DC = new List<TotalCycleCount>();
            foreach (DataRow row in dt.Rows)
            {
                TotalCycleCount CM = new TotalCycleCount();
                CM.MoldDataID = !Convert.IsDBNull(row["MoldDataID"]) ? Convert.ToInt32(row["MoldDataID"]) : 0;
                CM.TotalCycles = !Convert.IsDBNull(row["TotalCycles"]) ? Convert.ToInt32(row["TotalCycles"]) : 0;
                DC.Add(CM);
            }

            return DC.FirstOrDefault();
        }



        public TraciRiskFactor GetTraciRiskFactor(SqlConnection con, string Date, int MID)
        {
            SqlDataAdapter da = new SqlDataAdapter("procTraciRiskFactor", con);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.Add("@SystemDateTime", SqlDbType.DateTime).Value = Date;
            da.SelectCommand.Parameters.Add("@MoldDataID", SqlDbType.Int).Value = MID;
            da.SelectCommand.Parameters.Add("@CompanyID", SqlDbType.Int).Value = GetCompanyID();

            //da.SelectCommand.Parameters.Add("@MoldDataID", SqlDbType.Int).Value = MoldID;

            DataTable dt = new DataTable();
            da.Fill(dt);

            List<TraciRiskFactor> DC = new List<TraciRiskFactor>();
            foreach (DataRow row in dt.Rows)
            {
                TraciRiskFactor CM = new TraciRiskFactor();
                CM.MoldDataID = !Convert.IsDBNull(row["MoldDataID"]) ? Convert.ToInt32(row["MoldDataID"]) : 0;
                CM.MoldName = row["MoldName"].ToString();
                CM.MoldDesc = row["MoldDesc"].ToString();
                CM.PF = !Convert.IsDBNull(row["PF"]) ? Convert.ToDouble(row["PF"]) : 0;
                CM.SF = !Convert.IsDBNull(row["SF"]) ? Convert.ToDouble(row["SF"]) : 0;
                CM.LF = !Convert.IsDBNull(row["LF"]) ? Convert.ToDouble(row["LF"]) : 0;
                CM.MoldYears = !Convert.IsDBNull(row["MoldYears"]) ? Convert.ToInt64(row["MoldYears"]) : 0;
                CM.ShotCount = !Convert.IsDBNull(row["ShotCount"]) ? Convert.ToInt64(row["ShotCount"]) : 0;
                CM.PartOne = !Convert.IsDBNull(row["Part1"]) ? Convert.ToDouble(row["Part1"]) : 0;
                CM.PartOnePointFive = !Convert.IsDBNull(row["Part1.5"]) ? Convert.ToDouble(row["Part1.5"]) : 0;
                CM.PartOnePointSix = !Convert.IsDBNull(row["Part1.6"]) ? Convert.ToDouble(row["Part1.6"]) : 0;
                CM.PartTwo = !Convert.IsDBNull(row["Part2"]) ? Convert.ToDouble(row["Part2"]) : 0;
                CM.PartTwoPointFive = !Convert.IsDBNull(row["Part2.5"]) ? Convert.ToDouble(row["Part2.5"]) : 0;
                CM.RiskFactor = !Convert.IsDBNull(row["RiskFactor"]) ? Convert.ToDouble(row["RiskFactor"]) : 0;

                DC.Add(CM);
            }

            return DC.FirstOrDefault();
        }

        public List<SelectListItem> GetMoldDropDown()
        {
            var data = db.Database.SqlQuery<MoldDropdown>("exec procMoldDropdown @CompanyID", new SqlParameter("@CompanyID", GetCompanyID())).ToList<MoldDropdown>();
            List<SelectListItem> Tech = new List<SelectListItem>();
            foreach (var x in data)
            {
                Tech.Add(new SelectListItem
                {
                    Text = x.Name,
                    Value = x.MoldDataID.ToString()
                });
            }

            return Tech;
        }

        public List<SelectListItem> GetEmployeeName()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblEmployees.Where(X=> X.CompanyID == CID).ToList();
            List<SelectListItem> Tech = new List<SelectListItem>();
            foreach (var x in data)
            {
                Tech.Add(new SelectListItem
                {
                    Text = x.LastName + "," + x.FirstName,
                    Value = x.EmployeeID.ToString()
                });
            }

            return Tech;
        }

        public List<DefectRepaired> GetDefectRepaired(SqlConnection con, int? SetID=0)
        {
            SqlDataAdapter da = new SqlDataAdapter("GetDefectRepaired", con);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.Add("@SetID", SqlDbType.Int).Value = SetID;
            da.SelectCommand.Parameters.Add("@CompanyID", SqlDbType.Int).Value = GetCompanyID();

            //da.SelectCommand.Parameters.Add("@MoldDataID", SqlDbType.Int).Value = MoldID;

            DataTable dt = new DataTable();
            da.Fill(dt);

            List<DefectRepaired> DC = new List<DefectRepaired>();

            foreach (DataRow row in dt.Rows)
            {
                DefectRepaired CM = new DefectRepaired();
                CM.DfctID = !Convert.IsDBNull(row["DfctID"]) ? Convert.ToInt32(row["DfctID"]) : 0;
                CM.SetID = !Convert.IsDBNull(row["SetID"]) ? Convert.ToInt32(row["SetID"]) : 0;
                CM.DfctDate = !Convert.IsDBNull(row["DfctDate"]) ? Convert.ToDateTime(row["DfctDate"]) : new DateTime();
                CM.TSGuide = !Convert.IsDBNull(row["TSGuide"]) ? Convert.ToInt32(row["TSGuide"]) : 0;
                CM.DfctDescript = row["DfctDescript"].ToString();
                CM.CavDefect = row["Cav & Defect"].ToString();
                CM.TSDefect = row["TSDefects"].ToString();
                DC.Add(CM);
            }

            return DC;
        }

        public List<CompleteMaintenanceTracking> ScheduleStopvsXStop(SqlConnection con, string StDate, string EDate)
        {
            SqlDataAdapter da = new SqlDataAdapter("procCompleteMaintenanceTrackingAllMolds", con);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.Add("@StartDate", SqlDbType.Date).Value = StDate;
            da.SelectCommand.Parameters.Add("@EndDate", SqlDbType.Date).Value = EDate;
            da.SelectCommand.Parameters.Add("@CompanyID", SqlDbType.Int).Value = GetCompanyID();
            da.SelectCommand.Parameters.Add("@IsOrg", SqlDbType.Int).Value = IsCalledbyOrg();

            //da.SelectCommand.Parameters.Add("@MoldDataID", SqlDbType.Int).Value = MoldID;

            DataTable dt = new DataTable();
            da.Fill(dt);

            List<CompleteMaintenanceTracking> DC = new List<CompleteMaintenanceTracking>();

            foreach (DataRow row in dt.Rows)
            {
                CompleteMaintenanceTracking CM = new CompleteMaintenanceTracking();
                CM.Mold = row["Mold"].ToString();
                CM.Description = row["Description"].ToString();
                CM.DoD = !Convert.IsDBNull(row["DoD"]) ? Convert.ToInt32(row["DoD"]) : 0;
                CM.Configuration = row["Configuration"].ToString();
                CM.Configuration2 = row["Configuration2"].ToString();
                CM.StartDate = !Convert.IsDBNull(row["Start Date"]) ? Convert.ToDateTime(row["Start Date"]) : new DateTime();
                CM.StartTime = !Convert.IsDBNull(row["Start Time"]) ? Convert.ToDateTime(row["Start Time"]) : new DateTime();
                CM.Press = row["Press"].ToString();
                CM.StartTech = row["Start Tech"].ToString();
                CM.StopDate = !Convert.IsDBNull(row["Stop Date"]) ? Convert.ToDateTime(row["Stop Date"]) : new DateTime();
                CM.StopTime = !Convert.IsDBNull(row["Stop Time"]) ? Convert.ToDateTime(row["Stop Time"]) : new DateTime();
                CM.MoldStopReason = row["Mold Stop Reason"].ToString();
                CM.CycleCount = !Convert.IsDBNull(row["Cycle Count"]) ? Convert.ToDouble(row["Cycle Count"]) : 0;
                CM.RunTimeHours = !Convert.IsDBNull(row["Run Time  Hours"]) ? Convert.ToDouble(row["Run Time  Hours"]) : 0;
                CM.StopTech = row["Stop Tech"].ToString();
                CM.RepairDate = !Convert.IsDBNull(row["Repair Date"]) ? Convert.ToDateTime(row["Repair Date"]) : new DateTime();
                CM.RepairHours = !Convert.IsDBNull(row["Repair Hours"]) ? Convert.ToDouble(row["Repair Hours"]) : 0;
                CM.Status = row["Status"].ToString();
                CM.RepairTech = row["Repair Tech"].ToString();
                CM.WorkOrder = row["Work Order"].ToString();
                CM.Actual = !Convert.IsDBNull(row["Actual"]) ? Convert.ToDouble(row["Actual"]) : 0;
                CM.Adjust = !Convert.IsDBNull(row["Adjust"]) ? Convert.ToDouble(row["Adjust"]) : 0;
                CM.SetID = !Convert.IsDBNull(row["SetID"]) ? Convert.ToDouble(row["SetID"]) : 0;
                CM.MoldDataID = !Convert.IsDBNull(row["MoldDataID"]) ? Convert.ToDouble(row["MoldDataID"]) : 0;
                DC.Add(CM);
            }
            return DC;
        }

        public List<StopReasonCharts> MoldStopReasonCosts(SqlConnection con, string StDate, string EDate)
        {
            SqlDataAdapter da = new SqlDataAdapter("procMoldStopReasonCosts", con);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.Add("@StartDate", SqlDbType.Date).Value = StDate;
            da.SelectCommand.Parameters.Add("@EndDate", SqlDbType.Date).Value = EDate;
            da.SelectCommand.Parameters.Add("@CompanyID", SqlDbType.Int).Value = GetCompanyID();
            da.SelectCommand.Parameters.Add("@IsOrg", SqlDbType.Int).Value = IsCalledbyOrg();

            //da.SelectCommand.Parameters.Add("@MoldDataID", SqlDbType.Int).Value = MoldID;

            DataTable dt = new DataTable();
            da.Fill(dt);

            List<StopReasonCharts> DC = new List<StopReasonCharts>();

            foreach (DataRow row in dt.Rows)
            {
                StopReasonCharts CM = new StopReasonCharts();
                CM.MoldStopReason = row["Mold Stop Reason"].ToString();
                //CM.MoldStopReason = Regex.Replace(row["Mold Stop Reason"].ToString(), "<[^>]*>", string.Empty).Replace("&nbsp;", "");
                CM.StopCount = !Convert.IsDBNull(row["Stop Count"]) ? Convert.ToInt32(row["Stop Count"]) : 0;
                CM.LaborHours = !Convert.IsDBNull(row["Labor Hours"]) ? Convert.ToDouble(row["Labor Hours"]) : 0;
                CM.LaborCost = !Convert.IsDBNull(row["Labor Cost"]) ? Convert.ToDouble(row["Labor Cost"]) : 0;
                CM.ToolingCost = !Convert.IsDBNull(row["Tooling Cost"]) ? Convert.ToDecimal(row["Tooling Cost"]) : 0;
                CM.TotalCost = !Convert.IsDBNull(row["Total Cost"]) ? Convert.ToDouble(row["Total Cost"]) : 0;
                DC.Add(CM);
            }

            return DC;
        }

        public List<DefectandCAChart> DefectndCAChart(SqlConnection con, string StDate, string EDate, int MoldID)
        {

            SqlDataAdapter da = new SqlDataAdapter("proc_CorrectiveActionAnalysis", con);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.Add("@StartDate", SqlDbType.Date).Value = StDate;
            da.SelectCommand.Parameters.Add("@EndDate", SqlDbType.Date).Value = EDate;
            da.SelectCommand.Parameters.Add("@MoldDataID", SqlDbType.Int).Value = MoldID;
            da.SelectCommand.Parameters.Add("@CompanyID", SqlDbType.Int).Value = GetCompanyID();
            da.SelectCommand.Parameters.Add("@IsOrg", SqlDbType.Int).Value = IsCalledbyOrg();

            DataTable dt = new DataTable();
            da.Fill(dt);

            List<DefectandCAChart> DC = new List<DefectandCAChart>();

            foreach (DataRow row in dt.Rows)
            {
                DefectandCAChart DD = new DefectandCAChart();
                DD.Mold = row["Mold"].ToString();
                DD.Description = row["Description"].ToString();
                DD.Configuration = row["Configuration"].ToString();
                DD.Configuration2 = row["Configuration2"].ToString();
                DD.DoD = !Convert.IsDBNull(row["DoD"]) ? Convert.ToDouble(row["DoD"]) : 0;
                DD.MoldStartDate = !Convert.IsDBNull(row["Mold Start Date"]) ? Convert.ToDateTime(row["Mold Start Date"]) : new DateTime();
                DD.MoldStopDate = !Convert.IsDBNull(row["Mold Stop Date"]) ? Convert.ToDateTime(row["Mold Stop Date"]) : new DateTime();
                DD.StopReason = row["Stop Reason"].ToString();
                DD.TroubleShootersDefect = row["TroubleShooters Defect"].ToString();
                DD.DefectType = row["Defect Type"].ToString();
                DD.Position = row["Position"].ToString();
                DD.CavityID = row["Cavity ID"].ToString();
                DD.CorrectiveAction = row["Corrective Action"].ToString();
                DD.ToolingDescription = Regex.Replace(row["Tooling Description"].ToString(), "<[^>]*>", string.Empty).Replace("&nbsp;", "");
                DD.CATech = row["CA Tech"].ToString();
                DD.CADate = !Convert.IsDBNull(row["CA Date"]) ? Convert.ToDateTime(row["CA Date"]) : new DateTime();
                DD.RepairDate = !Convert.IsDBNull(row["Repair Date"]) ? Convert.ToDateTime(row["Repair Date"]) : new DateTime();
                DD.QTY = !Convert.IsDBNull(row["QTY"]) ? Convert.ToInt32(row["QTY"]) : 0;
                DD.RepairHours = !Convert.IsDBNull(row["Repair Hours"]) ? Convert.ToDecimal(row["Repair Hours"]) : 0;
                DD.LaborCost = !Convert.IsDBNull(row["Labor Cost"]) ? Convert.ToDouble(row["Labor Cost"]) : 0;
                DD.ToolingCost = !Convert.IsDBNull(row["Tooling Cost"]) ? Convert.ToDouble(row["Tooling Cost"]) : 0;
                DD.TotalCost = !Convert.IsDBNull(row["Total Cost"]) ? Convert.ToDouble(row["Total Cost"]) : 0;
                DD.CycleCount = !Convert.IsDBNull(row["Cycle Count"]) ? Convert.ToInt32(row["Cycle Count"]) : 0;
                DD.RepairTime = !Convert.IsDBNull(row["Repair Time"]) ? Convert.ToDouble(row["Repair Time"]) : 0;
                DD.RunTimeHours = !Convert.IsDBNull(row["Run Time  Hours"]) ? Convert.ToInt32(row["Run Time  Hours"]) : 0;
                DC.Add(DD);

            }

            return DC;
        }

        public List<DefectandCAChart> DefectndCAChartDrillDown(SqlConnection con, string StDate, string EDate, int MoldID, string StopReason)
        {
            SqlDataAdapter da = new SqlDataAdapter("GetDrillDownReport", con);
            da.SelectCommand.CommandType = CommandType.StoredProcedure;
            da.SelectCommand.Parameters.Add("@StartDate", SqlDbType.Date).Value = StDate;
            da.SelectCommand.Parameters.Add("@EndDate", SqlDbType.Date).Value = EDate;
            da.SelectCommand.Parameters.Add("@MoldDataID", SqlDbType.Int).Value = MoldID;
            da.SelectCommand.Parameters.Add("@StopReaason", SqlDbType.NVarChar).Value = StopReason;
            da.SelectCommand.Parameters.Add("@CompanyID", SqlDbType.Int).Value = GetCompanyID();

            DataTable dt = new DataTable();
            da.Fill(dt);

            List<DefectandCAChart> DC = new List<DefectandCAChart>();

            foreach (DataRow row in dt.Rows)
            {
                DefectandCAChart DD = new DefectandCAChart();
                DD.Mold = row["Mold"].ToString();
                DD.Description = row["Description"].ToString();
                DD.Configuration = row["Configuration"].ToString();
                DD.Configuration2 = row["Configuration2"].ToString();
                DD.DoD = !Convert.IsDBNull(row["DoD"]) ? Convert.ToDouble(row["DoD"]) : 0;
                DD.MoldStartDate = !Convert.IsDBNull(row["Mold Start Date"]) ? Convert.ToDateTime(row["Mold Start Date"]) : new DateTime();
                DD.MoldStopDate = !Convert.IsDBNull(row["Mold Stop Date"]) ? Convert.ToDateTime(row["Mold Stop Date"]) : new DateTime();
                DD.StopReason = row["Stop Reason"].ToString();
                DD.TroubleShootersDefect = row["TroubleShooters Defect"].ToString();
                DD.DefectType = row["Defect Type"].ToString();
                DD.Position = row["Position"].ToString();
                DD.CavityID = row["Cavity ID"].ToString();
                DD.CorrectiveAction = row["Corrective Action"].ToString();
                DD.ToolingDescription = Regex.Replace(row["Tooling Description"].ToString(), "<[^>]*>", string.Empty).Replace("&nbsp;", "");
                DD.CATech = row["CA Tech"].ToString();
                DD.CADate = !Convert.IsDBNull(row["CA Date"]) ? Convert.ToDateTime(row["CA Date"]) : new DateTime();
                DD.RepairDate = !Convert.IsDBNull(row["Repair Date"]) ? Convert.ToDateTime(row["Repair Date"]) : new DateTime();
                DD.QTY = !Convert.IsDBNull(row["QTY"]) ? Convert.ToInt32(row["QTY"]) : 0;
                DD.RepairHours = !Convert.IsDBNull(row["Repair Hours"]) ? Convert.ToDecimal(row["Repair Hours"]) : 0;
                DD.LaborCost = !Convert.IsDBNull(row["Labor Cost"]) ? Convert.ToDouble(row["Labor Cost"]) : 0;
                DD.ToolingCost = !Convert.IsDBNull(row["Tooling Cost"]) ? Convert.ToDouble(row["Tooling Cost"]) : 0;
                DD.TotalCost = !Convert.IsDBNull(row["Total Cost"]) ? Convert.ToDouble(row["Total Cost"]) : 0;
                DD.CycleCount = !Convert.IsDBNull(row["Cycle Count"]) ? Convert.ToInt32(row["Cycle Count"]) : 0;
                DD.RepairTime = !Convert.IsDBNull(row["Repair Time"]) ? Convert.ToDouble(row["Repair Time"]) : 0;
                DD.RunTimeHours = !Convert.IsDBNull(row["Run Time  Hours"]) ? Convert.ToInt32(row["Run Time  Hours"]) : 0;
                DC.Add(DD);

            }

            return DC;
        }


        public List<CostsPerRunTimeHour> CostsPerRunTime(SqlConnection con, string StDate, string EDate, int MoldID)
        {
            int CID = GetCompanyID();
            SqlDataAdapter da2 = new SqlDataAdapter("procCostsPerRunTimeHour", con);
            da2.SelectCommand.CommandType = CommandType.StoredProcedure;
            da2.SelectCommand.Parameters.Add("@StartDate", SqlDbType.Date).Value = StDate;
            da2.SelectCommand.Parameters.Add("@EndDate", SqlDbType.Date).Value = EDate;
            da2.SelectCommand.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CID;
            da2.SelectCommand.Parameters.Add("@IsOrg", SqlDbType.Int).Value = IsCalledbyOrg();

            DataTable dt1 = new DataTable();
            da2.Fill(dt1);

            List<CostsPerRunTimeHour> DC1 = new List<CostsPerRunTimeHour>();
            foreach (DataRow row in dt1.Rows)
            {
                CostsPerRunTimeHour DD = new CostsPerRunTimeHour();
                DD.MoldDataID = !Convert.IsDBNull(row["MoldDataID"]) ? Convert.ToInt32(row["MoldDataID"]) : 0;
                DD.Mold = row["Mold"].ToString();
                DD.Description = Regex.Replace(row["Description"].ToString(), "<[^>]*>", string.Empty).Replace("&nbsp;", "").Replace("\"", " ");
                DD.Configuration = row["Configuration"].ToString();
                DD.Configuration2 = row["Configuration2"].ToString();
                DD.MoldStops = !Convert.IsDBNull(row["Mold Stops"]) ? Convert.ToInt32(row["Mold Stops"]) : 0;
                DD.Scheduled = !Convert.IsDBNull(row["Scheduled"]) ? Convert.ToInt32(row["Scheduled"]) : 0;
                DD.XStop = !Convert.IsDBNull(row["X-Stop"]) ? Convert.ToInt32(row["X-Stop"]) : 0;
                DD.Defect = !Convert.IsDBNull(row["Defect"]) ? Convert.ToInt32(row["Defect"]) : 0;
                DD.Quality = !Convert.IsDBNull(row["Quality"]) ? Convert.ToInt32(row["Quality"]) : 0;
                DD.Blocked = !Convert.IsDBNull(row["Blocked"]) ? Convert.ToInt32(row["Blocked"]) : 0;
                DD.ToolingCost = !Convert.IsDBNull(row["Tooling Cost"]) ? Convert.ToDouble(row["Tooling Cost"]) : 0;
                DD.LaborHours = !Convert.IsDBNull(row["Labor Hours"]) ? Convert.ToDouble(row["Labor Hours"]) : 0;
                DD.LaborCost = !Convert.IsDBNull(row["Labor Cost"]) ? Convert.ToDouble(row["Labor Cost"]) : 0;
                DD.TotalCost = !Convert.IsDBNull(row["Total Cost"]) ? Convert.ToDouble(row["Total Cost"]) : 0;
                DD.RunTimeMinutes = !Convert.IsDBNull(row["Run Time Minutes"]) ? Convert.ToDouble(row["Run Time Minutes"]) : 0;
                DD.RunTime = row["Run Time"].ToString();
                DD.TotalRunTimeHours = !Convert.IsDBNull(row["Total Run Time  Hours"]) ? Convert.ToDouble(row["Total Run Time  Hours"]) : 0;
                DD.CostPerHour = !Convert.IsDBNull(row["Cost Per Hour"]) ? Convert.ToDecimal(row["Cost Per Hour"]) : 0;
                DD.CycleTimeSec = !Convert.IsDBNull(row["Cycle Time Sec"]) ? Convert.ToDouble(row["Cycle Time Sec"]) : 0;
                DD.TotalActualCyclesRun = !Convert.IsDBNull(row["Total Actual Cycles Run"]) ? Convert.ToDouble(row["Total Actual Cycles Run"]) : 0;
                DC1.Add(DD);
            }

            return DC1;
        }

        static string ReplaceLink(Match m)
        {
            return m.ToString().Replace("\"", "'");
        }

        public List<DefectByMoldBlockandQuality> DefectbyMold(SqlConnection con, string StDate, string EDate, int MoldID)
        {
            int CID = GetCompanyID();
            SqlDataAdapter da2 = new SqlDataAdapter("procDefectsByMoldBlockAndQuality", con);
            da2.SelectCommand.CommandType = CommandType.StoredProcedure;
            da2.SelectCommand.Parameters.Add("@StartDate", SqlDbType.Date).Value = StDate;
            da2.SelectCommand.Parameters.Add("@EndDate", SqlDbType.Date).Value = EDate;
            da2.SelectCommand.Parameters.Add("@MoldDataID", SqlDbType.Int).Value = MoldID;
            da2.SelectCommand.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CID;
            da2.SelectCommand.Parameters.Add("@IsOrg", SqlDbType.Int).Value = IsCalledbyOrg();

            DataTable dt1 = new DataTable();
            da2.Fill(dt1);


            List<DefectByMoldBlockandQuality> DC1 = new List<DefectByMoldBlockandQuality>();

            foreach (DataRow row in dt1.Rows)
            {
                DefectByMoldBlockandQuality dd = new DefectByMoldBlockandQuality();
                dd.Mold = row["Mold"].ToString();
                dd.Description = row["Description"].ToString();
                dd.Configuration = row["Configuration"].ToString();
                dd.Configuration2 = row["Configuration2"].ToString();
                dd.TroubleShootersDefects = Regex.Replace(row["TroubleShooters Defects"].ToString(), "<[^>]*>", string.Empty).Replace("&nbsp;", "");
                dd.Type = row["Type"].ToString();
                dd.Blocked = Convert.ToBoolean(row["Blocked"]);
                dd.Quality = Convert.ToBoolean(row["Quality"]);
                dd.Count = Convert.ToInt32(row["Count"]);
                DC1.Add(dd);
            }

            //var dss = DC1.GroupBy(x => x.TroubleShootersDefects).Select(s => new DefectcMoldBlockandQualityTwo
            //{
            //    TroubleShootersDefect = s.First().TroubleShootersDefects,
            //    DefectCount = s.Sum(b=> b.Count)
            //}).OrderByDescending(x => x.DefectCount).Take(10).ToList();

            return DC1;
        }

        public List<MoldToolingCosts> MoldToolingCostsFunc(SqlConnection con, string StDate, string EDate, int MoldID)
        {
            int CID = GetCompanyID();
            SqlDataAdapter da2 = new SqlDataAdapter("procMoldToolingCosts", con);
            da2.SelectCommand.CommandType = CommandType.StoredProcedure;
            da2.SelectCommand.Parameters.Add("@StartDate", SqlDbType.Date).Value = StDate;
            da2.SelectCommand.Parameters.Add("@EndDate", SqlDbType.Date).Value = EDate;
            da2.SelectCommand.Parameters.Add("@MoldDataID", SqlDbType.Int).Value = MoldID;
            da2.SelectCommand.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CID;
            da2.SelectCommand.Parameters.Add("@IsOrg", SqlDbType.Int).Value = IsCalledbyOrg();

            DataTable dt1 = new DataTable();
            da2.Fill(dt1);

            List<MoldToolingCosts> DC1 = new List<MoldToolingCosts>();
            foreach (DataRow row in dt1.Rows)
            {
                MoldToolingCosts dd = new MoldToolingCosts();
                dd.Mold = row["Mold"].ToString();
                dd.Description = row["Description"].ToString();
                dd.Configuration = row["Configuration"].ToString();
                dd.Configuration2 = row["Configuration2"].ToString();
                dd.Tooling = row["Tooling"].ToString();
                dd.Qty = !Convert.IsDBNull(row["Qty"]) ? Convert.ToInt32(row["Qty"]) : 0;
                dd.Type = row["Type"].ToString();
                dd.DateInstalled = !Convert.IsDBNull(row["Date Installed"]) ? Convert.ToDateTime(row["Date Installed"]) : new DateTime();
                dd.RepairDate = !Convert.IsDBNull(row["Repair Date"]) ? Convert.ToDateTime(row["Repair Date"]) : new DateTime();
                dd.PartNo = row["Part No."].ToString();
                dd.DetailNo = row["Detail No."].ToString();
                dd.Vendor = row["Vendor"].ToString();
                dd.PartCost = !Convert.IsDBNull(row["Part Cost"]) ? Convert.ToDouble(row["Part Cost"]) : 0;
                dd.TotalCost = !Convert.IsDBNull(row["Total Cost"]) ? Convert.ToDouble(row["Total Cost"]) : 0;
                dd.CycleCount = !Convert.IsDBNull(row["Cycle Count"]) ? Convert.ToDouble(row["Cycle Count"]) : 0;

                DC1.Add(dd);
            }

            return DC1;
        }


        public List<MaintenanceAlertStats> GetMaintenanceAlertData2(SqlConnection con)
        {
            int CID = GetCompanyID();
            SqlDataAdapter da2 = new SqlDataAdapter("procMaintenanceAlert_TotalLife", con);
            da2.SelectCommand.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CID;

            da2.SelectCommand.CommandType = CommandType.StoredProcedure;

            DataTable dt1 = new DataTable();
            da2.Fill(dt1);


            List<MaintenanceAlertStats> DC1 = new List<MaintenanceAlertStats>();

            foreach (DataRow row in dt1.Rows)
            {
                MaintenanceAlertStats dd = new MaintenanceAlertStats();
                dd.SetPressNumb = row["SetPressNumb"].ToString();
                dd.MoldDataID = !Convert.IsDBNull(row["MoldDataID"]) ? Convert.ToInt32(row["MoldDataID"]) : 0;
                dd.MoldName = row["MoldName"].ToString();
                dd.MoldOutPressPMRedCycles = !Convert.IsDBNull(row["MoldOutPressPMRedCycles"]) ? Convert.ToInt32(row["MoldOutPressPMRedCycles"]) : 0;
                dd.MoldOutPressPMYellowCycles = !Convert.IsDBNull(row["MoldOutPressPMYellowCycles"]) ? Convert.ToInt32(row["MoldOutPressPMYellowCycles"]) : 0;

                dd.MoldOutPressPMRed = !Convert.IsDBNull(row["Cycles Over Red Limit"]) ? Convert.ToInt32(row["Cycles Over Red Limit"]) : 0;
                dd.MoldOutPressPMRed2 = !Convert.IsDBNull(row["Cycles to Reach Red Limit"]) ? Convert.ToInt32(row["Cycles to Reach Red Limit"]) : 0;
                dd.MoldOutPressPMYellow = !Convert.IsDBNull(row["Cycles to Reach Yellow Limit"]) ? Convert.ToInt32(row["Cycles to Reach Yellow Limit"]) : 0;

                dd.MoldOutPressPMFreq = !Convert.IsDBNull(row["MoldOutPressPMFreq"]) ? Convert.ToInt32(row["MoldOutPressPMFreq"]) : 0;
                dd.SumOfTotalCycles = !Convert.IsDBNull(row["SumOfTotalCycles"]) ? Convert.ToDouble(row["SumOfTotalCycles"]) : 0;
                dd.MoldDesc = row["MoldDesc"].ToString();
                dd.MoldConfig = row["MoldConfig"].ToString();
                dd.MoldConfig2 = row["MoldConfig2"].ToString();
                dd.MldPullDate = !Convert.IsDBNull(row["MldPullDate"]) ? Convert.ToDateTime(row["MldPullDate"]) : new DateTime();
                dd.RunStatusColor = row["RunStatusColor"].ToString();
                dd.CycleCounter = !Convert.IsDBNull(row["CycleCounter"]) ? Convert.ToInt32(row["CycleCounter"]) : 0;

                DC1.Add(dd);
            }

            return DC1;
        }

        public List<MaintenanceAlertStats> GetMaintenanceAlertData(SqlConnection con, string StDate)
        {
            int CID = GetCompanyID();
            SqlDataAdapter da2 = new SqlDataAdapter("procMaintenanceAlert", con);
            da2.SelectCommand.CommandType = CommandType.StoredProcedure;
            da2.SelectCommand.Parameters.Add("@SystemDateTime", SqlDbType.DateTime).Value = StDate;
            da2.SelectCommand.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CID;
            da2.SelectCommand.Parameters.Add("@IsOrg", SqlDbType.Int).Value = IsCalledbyOrg();

            DataTable dt1 = new DataTable();
            da2.Fill(dt1);


            List<MaintenanceAlertStats> DC1 = new List<MaintenanceAlertStats>();

            foreach (DataRow row in dt1.Rows)
            {
                MaintenanceAlertStats dd = new MaintenanceAlertStats();
                dd.SetPressNumb = row["SetPressNumb"].ToString();
                dd.MoldDataID = !Convert.IsDBNull(row["MoldDataID"]) ? Convert.ToInt32(row["MoldDataID"]) : 0;
                dd.MoldName = row["MoldName"].ToString();
                dd.MoldOutPressPMRedCycles = !Convert.IsDBNull(row["MoldOutPressPMRedCycles"]) ? Convert.ToInt32(row["MoldOutPressPMRedCycles"]) : 0;
                dd.MoldOutPressPMYellowCycles = !Convert.IsDBNull(row["MoldOutPressPMYellowCycles"]) ? Convert.ToInt32(row["MoldOutPressPMYellowCycles"]) : 0;

                dd.MoldOutPressPMRed = !Convert.IsDBNull(row["Cycles Over Red Limit"]) ? Convert.ToInt32(row["Cycles Over Red Limit"]) : 0;
                dd.MoldOutPressPMRed2 = !Convert.IsDBNull(row["Cycles to Reach Red Limit"]) ? Convert.ToInt32(row["Cycles to Reach Red Limit"]) : 0;
                dd.MoldOutPressPMYellow = !Convert.IsDBNull(row["Cycles to Reach Yellow Limit"]) ? Convert.ToInt32(row["Cycles to Reach Yellow Limit"]) : 0;

                dd.MoldOutPressPMFreq = !Convert.IsDBNull(row["MoldOutPressPMFreq"]) ? Convert.ToInt32(row["MoldOutPressPMFreq"]) : 0;
                dd.SumOfTotalCycles = !Convert.IsDBNull(row["SumOfTotalCycles"]) ? Convert.ToDouble(row["SumOfTotalCycles"]) : 0;
                dd.MoldDesc = row["MoldDesc"].ToString();
                dd.MoldConfig = row["MoldConfig"].ToString();
                dd.MoldConfig2 = row["MoldConfig2"].ToString();
                dd.MldPullDate = !Convert.IsDBNull(row["MldPullDate"]) ? Convert.ToDateTime(row["MldPullDate"]) : new DateTime();
                dd.RunStatusColor = row["RunStatusColor"].ToString();
                dd.CycleCounter = !Convert.IsDBNull(row["CycleCounter"]) ? Convert.ToInt32(row["CycleCounter"]) : 0;

                DC1.Add(dd);
            }

            return DC1;
        }


        public List<DefectPositionAnalysis> DefectPositionAnalysis(SqlConnection con, string StDate, string EDate)
        {
            int CID = GetCompanyID();
            SqlDataAdapter da2 = new SqlDataAdapter("procDefectPositionAnalysis", con);
            da2.SelectCommand.CommandType = CommandType.StoredProcedure;
            da2.SelectCommand.Parameters.Add("@StartDate", SqlDbType.Date).Value = StDate;
            da2.SelectCommand.Parameters.Add("@EndDate", SqlDbType.Date).Value = EDate;
            da2.SelectCommand.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CID;
            da2.SelectCommand.Parameters.Add("@IsOrg", SqlDbType.Int).Value = IsCalledbyOrg();

            DataTable dt1 = new DataTable();
            da2.Fill(dt1);


            List<DefectPositionAnalysis> DC1 = new List<DefectPositionAnalysis>();

            foreach (DataRow row in dt1.Rows)
            {
                DefectPositionAnalysis dd = new DefectPositionAnalysis();
                dd.Mold = row["Mold"].ToString();
                dd.Description = row["Description"].ToString();
                dd.Configuration = row["Configuration"].ToString();
                dd.Configuration2 = row["Configuration2"].ToString();
                dd.Press = row["Press"].ToString();
                dd.StartDate = !Convert.IsDBNull(row["Start Date"]) ? Convert.ToDateTime(row["Start Date"]) : new DateTime();
                dd.StopDate = !Convert.IsDBNull(row["Stop Date"]) ? Convert.ToDateTime(row["Stop Date"]) : new DateTime();
                dd.CycleCount = !Convert.IsDBNull(row["Cycle Count"]) ? Convert.ToDouble(row["Cycle Count"]) : 0;
                dd.RunTimeHours = !Convert.IsDBNull(row["Run Time  Hours"]) ? Convert.ToDecimal(row["Run Time  Hours"]) : 0;
                dd.Position = row["Position"].ToString();
                dd.TroubleShootersDefects = row["TroubleShooters Defects"].ToString();
                dd.Type = row["Type"].ToString();
                dd.BlockedNoted = !Convert.IsDBNull(row["Blocked / Noted"]) ? Convert.ToDateTime(row["Blocked / Noted"]) : new DateTime();
                dd.Time = !Convert.IsDBNull(row["Time"]) ? Convert.ToDateTime(row["Time"]) : new DateTime();
                dd.Blocked = !Convert.IsDBNull(row["Blocked"]) ? Convert.ToInt32(row["Blocked"]) : 0;
                dd.Quality = !Convert.IsDBNull(row["Quality"]) ? Convert.ToInt32(row["Quality"]) : 0;
                dd.BlockedByNotedBy = row["Blocked By / Noted By"].ToString();
                dd.CavityID = row["Cavity ID"].ToString();
                dd.RepairTime = !Convert.IsDBNull(row["Repair Time"]) ? Convert.ToDouble(row["Repair Time"]) : 0;

                DC1.Add(dd);
            }

            return DC1;
        }


        public List<RepairSheet> RepairSheetList(SqlConnection con, string StDate, string EDate, int MoldID)
        {
            int CID = GetCompanyID();
            SqlDataAdapter da2 = new SqlDataAdapter("procRepairSheet", con);
            da2.SelectCommand.CommandType = CommandType.StoredProcedure;
            if (StDate != "" || EDate != "")
            {
                da2.SelectCommand.Parameters.Add("@MoldDataID", SqlDbType.Float).Value = MoldID;
                da2.SelectCommand.Parameters.Add("@StartDate", SqlDbType.Date).Value = StDate;
                da2.SelectCommand.Parameters.Add("@EndDate", SqlDbType.Date).Value = EDate;
            }

            else
            {
                da2.SelectCommand.Parameters.Add("@MoldDataID", SqlDbType.Float).Value = MoldID;
            }

            da2.SelectCommand.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CID;

            DataTable dt1 = new DataTable();
            da2.Fill(dt1);
            List<RepairSheet> DC1 = new List<RepairSheet>();

            try
            {
                int i = 0;
                foreach (DataRow row in dt1.Rows)
                {
                    i += 1;
                    RepairSheet dd = new RepairSheet();
                    dd.MoldName = row["MoldName"].ToString();
                    dd.SetID = !Convert.IsDBNull(row["SetID"]) ? Convert.ToInt32(row["SetID"]) : 0;
                    dd.SetDate = !Convert.IsDBNull(row["SetDate"]) ? Convert.ToDateTime(row["SetDate"]) : new DateTime();
                    dd.CavityNumber = row["CavityNumber"].ToString();
                    dd.CavityLocationNumber = row["CavityLocationNumber"].ToString();
                    dd.TSDefects = row["TSDefects"].ToString();
                    dd.DfctDate = !Convert.IsDBNull(row["DfctDate"]) ? Convert.ToDateTime(row["DfctDate"]) : new DateTime();
                    dd.TSDate = !Convert.IsDBNull(row["TSDate"]) ? Convert.ToDateTime(row["TSDate"]) : new DateTime();
                    dd.TlSTime = !Convert.IsDBNull(row["TlSTime"]) ? Convert.ToDateTime(row["TlSTime"]) : new DateTime();
                    dd.MoldToolDescrip = row["MoldToolDescrip"].ToString();
                    dd.TlCorrectiveAction = row["TlCorrectiveAction"].ToString();
                    dd.SetTime = !Convert.IsDBNull(row["SetTime"]) ? Convert.ToDateTime(row["SetTime"]) : new DateTime();
                    dd.RsSetTech = row["RsSetTech"].ToString();
                    dd.RsPullTech = row["RsPullTech"].ToString();
                    dd.MldRepairdBy1 = row["MldRepairdBy1"].ToString();
                    dd.Tech = row["Tech"].ToString();
                    dd.EstCycles = !Convert.IsDBNull(row["EstCycles"]) ? Convert.ToDecimal(row["EstCycles"]) : 0;
                    dd.MldRepairedDate = !Convert.IsDBNull(row["MldRepairedDate"]) ? Convert.ToDateTime(row["MldRepairedDate"]) : new DateTime();
                    dd.MldRepairedTime = !Convert.IsDBNull(row["MldRepairedTime"]) ? Convert.ToInt32(row["MldRepairedTime"]) : 0;
                    dd.MldWorkOrder = !Convert.IsDBNull(row["MldWorkOrder"]) ? Convert.ToInt32(row["MldWorkOrder"]) : 0;
                    dd.MldProductionCycles = !Convert.IsDBNull(row["MldProductionCycles"]) ? Convert.ToInt32(row["MldProductionCycles"]) : 0;
                    dd.MldRepairComments = row["MldRepairComments"].ToString();
                    dd.CompanyName = row["CompanyName"].ToString();
                    dd.CompanyCNRepairSheet = row["CompanyCNRepairSheet"].ToString();
                    dd.SetPressNumb = row["SetPressNumb"].ToString();
                    dd.MldSetPullNotes = row["MldSetPullNotes"].ToString();
                    dd.MldPullMaintRequired = row["MldPullMaintRequired"].ToString();
                    dd.MldPullDate = !Convert.IsDBNull(row["MldPullDate"]) ? Convert.ToDateTime(row["MldPullDate"]) : new DateTime();
                    dd.MldPullTime = !Convert.IsDBNull(row["MldPullTime"]) ? Convert.ToDateTime(row["MldPullTime"]) : new DateTime();
                    dd.MoldDesc = row["MoldDesc"].ToString();
                    dd.MoldDataID = !Convert.IsDBNull(row["MoldDataID"]) ? Convert.ToInt32(row["MoldDataID"]) : 0;
                    dd.TINotes = row["TINotes"].ToString();
                    dd.MoldConfig = row["MoldConfig"].ToString();
                    dd.MoldConfig2 = row["MoldConfig2"].ToString();
                    dd.TIRepairTime = !Convert.IsDBNull(row["TIRepairTime"]) ? Convert.ToDouble(row["TIRepairTime"]) : 0;
                    dd.DaysRun = row["DaysRun"].ToString();
                    dd.MoldDefectMapPath = row["MoldDefectMapPath"].ToString(); 
                    DC1.Add(dd);
                }

            }

            catch (Exception ex)
            {

            }


            return DC1;
        }


        public List<StartDate> StartEndDateReport(SqlConnection con, int MoldID)
        {
            int CID = GetCompanyID();
            SqlDataAdapter da2 = new SqlDataAdapter("procStarDateReport", con);
            da2.SelectCommand.CommandType = CommandType.StoredProcedure;
            da2.SelectCommand.Parameters.Add("@MoldDataID", SqlDbType.Int).Value = MoldID;
            da2.SelectCommand.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CID;

            DataTable dt1 = new DataTable();
            da2.Fill(dt1);
            List<StartDate> DC1 = new List<StartDate>();

            try
            {
                int i = 0;
                foreach (DataRow row in dt1.Rows)
                {
                    i += 1;
                    StartDate dd = new StartDate();
                    dd.MoldDataID = !Convert.IsDBNull(row["MoldDataID"]) ? Convert.ToInt32(row["MoldDataID"]) : 0;
                    dd.SetID = !Convert.IsDBNull(row["SetID"]) ? Convert.ToInt32(row["SetID"]) : 0;
                    dd.MoldName = row["MoldName"].ToString();
                    dd.MoldDesc = row["MoldDesc"].ToString();
                    dd.SetDate = !Convert.IsDBNull(row["SetDate"]) ? Convert.ToDateTime(row["SetDate"]) : new DateTime();
                    dd.SetTime = !Convert.IsDBNull(row["SetDate"]) ? Convert.ToDateTime(row["SetTime"]) : new DateTime();
                    dd.SetPressNumb = row["SetPressNumb"].ToString();
                    dd.Tech = row["Tech"].ToString();
                    dd.MldPullDate = !Convert.IsDBNull(row["MldPullDate"]) ? Convert.ToDateTime(row["MldPullDate"]) : new DateTime();
                    dd.MldPullTime = !Convert.IsDBNull(row["MldPullTime"]) ? Convert.ToDateTime(row["MldPullTime"]) : new DateTime(); ;

                    DC1.Add(dd);
                }
            }

            catch (Exception ex)
            {

            }
            return DC1;
        }


        public List<MaintenanceTimeline> MaintenenceTimeline(SqlConnection con, string StDate, string EDate, int MoldID)
        {
            int CID = GetCompanyID();
            SqlDataAdapter da2 = new SqlDataAdapter("procMaintenanceTimeline", con);
            da2.SelectCommand.CommandType = CommandType.StoredProcedure;
            da2.SelectCommand.Parameters.Add("@MoldDataID", SqlDbType.Float).Value = MoldID;
            da2.SelectCommand.Parameters.Add("@StartDate", SqlDbType.Date).Value = StDate;
            da2.SelectCommand.Parameters.Add("@EndDate", SqlDbType.Date).Value = EDate;
            da2.SelectCommand.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CID;

            DataTable dt1 = new DataTable();
            da2.Fill(dt1);
            List<MaintenanceTimeline> DC1 = new List<MaintenanceTimeline>();

            try
            {
                int i = 0;
                foreach (DataRow row in dt1.Rows)
                {
                    i += 1;
                    MaintenanceTimeline dd = new MaintenanceTimeline();
                    dd.MoldDataID = !Convert.IsDBNull(row["MoldDataID"]) ? Convert.ToInt32(row["MoldDataID"]) : 0;
                    dd.MoldName = row["MoldName"].ToString();
                    dd.MoldDesc = row["MoldDesc"].ToString();
                    dd.SetDate = !Convert.IsDBNull(row["SetDate"]) ? Convert.ToDateTime(row["SetDate"]) : new DateTime();
                    dd.SetTime = !Convert.IsDBNull(row["SetTime"]) ? Convert.ToDateTime(row["SetTime"]) : new DateTime();
                    dd.ProductLine = row["ProductLine"].ToString();
                    dd.ProductPart = row["ProductPart"].ToString();
                    dd.MldPullDate = !Convert.IsDBNull(row["MldPullDate"]) ? Convert.ToDateTime(row["MldPullDate"]) : new DateTime();
                    dd.MldPullTime = !Convert.IsDBNull(row["MldPullTime"]) ? Convert.ToDateTime(row["MldPullTime"]) : new DateTime();
                    dd.SetPressNumb = row["SetPressNumb"].ToString();
                    dd.MldPullMaintRequired = row["MldPullMaintRequired"].ToString();
                    dd.MldSetPullNotes = row["MldSetPullNotes"].ToString();
                    dd.EmployeeID = row["EmployeeID"].ToString();
                    dd.FirstName = row["FirstName"].ToString();
                    dd.LastName = row["LastName"].ToString();
                    dd.MoldToolDescrip = row["MoldToolDescrip"].ToString();
                    dd.MldRepairComments = row["MldRepairComments"].ToString();
                    dd.SetID = !Convert.IsDBNull(row["SetID"]) ? Convert.ToInt32(row["SetID"]) : 0;
                    dd.CompanyName = row["CompanyName"].ToString();
                    dd.CompanyCNToolingExp = row["CompanyCNToolingExp"].ToString();
                    dd.MoldConfig = row["MoldConfig"].ToString();
                    dd.MoldConfig2 = row["MoldConfig2"].ToString();
                    dd.MTXTotalTimeCal = MTXTotalTimeCal(dd.SetDate,dd.SetTime,dd.MldPullDate,dd.MldPullTime);
                    DC1.Add(dd);
                }
            }

            catch (Exception ex)
            {

            }

            return DC1;
        }


        public List<DefectCostandAnalysis> DefectCostAnalysis(SqlConnection con, string StDate, string EDate, int MoldID)
        {
            int CID = GetCompanyID();
            SqlDataAdapter da2 = new SqlDataAdapter("procDefectCostAnalysisrpt", con);
            da2.SelectCommand.CommandType = CommandType.StoredProcedure;
            da2.SelectCommand.Parameters.Add("@MoldDataID", SqlDbType.Float).Value = MoldID;
            da2.SelectCommand.Parameters.Add("@StartDate", SqlDbType.Date).Value = StDate;
            da2.SelectCommand.Parameters.Add("@EndDate", SqlDbType.Date).Value = EDate;
            da2.SelectCommand.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CID;

            DataTable dt1 = new DataTable();
            da2.Fill(dt1);
            List<DefectCostandAnalysis> DC1 = new List<DefectCostandAnalysis>();

            try
            {
                int i = 0;
                foreach (DataRow row in dt1.Rows)
                {
                    i += 1;
                    DefectCostandAnalysis dd = new DefectCostandAnalysis();
                    dd.SetDate = !Convert.IsDBNull(row["SetDate"]) ? Convert.ToDateTime(row["SetDate"]) : new DateTime();
                    dd.MoldDataID = !Convert.IsDBNull(row["MoldDataID"]) ? Convert.ToInt32(row["MoldDataID"]) : 0;
                    dd.SetTime = !Convert.IsDBNull(row["SetTime"]) ? Convert.ToDateTime(row["SetTime"]) : new DateTime();
                    dd.MoldName = row["MoldName"].ToString();
                    dd.MoldDesc = row["MoldDesc"].ToString();
                    dd.SetPressNumb = row["SetPressNumb"].ToString();
                    dd.DfctID = row["DfctID"].ToString();
                    dd.TSDefects = row["TSDefects"].ToString();
                    dd.DfctCavNum = !Convert.IsDBNull(row["DfctCavNum"]) ? Convert.ToInt32(row["DfctCavNum"]) : 0;
                    dd.DfctDescript = row["DfctDescript"].ToString();
                    dd.ProductLine = row["ProductLine"].ToString();
                    dd.ProductPart = row["ProductPart"].ToString();
                    dd.DfctDate = !Convert.IsDBNull(row["DfctDate"]) ? Convert.ToDateTime(row["DfctDate"]) : new DateTime();
                    dd.EmployeeID = row["EmployeeID"].ToString();
                    dd.DfctNotes = row["DfctNotes"].ToString();
                    dd.MldPullDate = !Convert.IsDBNull(row["MldPullDate"]) ? Convert.ToDateTime(row["MldPullDate"]) : new DateTime();
                    dd.MldPullTime = !Convert.IsDBNull(row["MldPullTime"]) ? Convert.ToDateTime(row["MldPullTime"]) : new DateTime();
                    dd.CavityLocationNumber = row["CavityLocationNumber"].ToString();
                    dd.DfctTime = !Convert.IsDBNull(row["DfctTime"]) ? Convert.ToDateTime(row["DfctTime"]) : new DateTime();
                    dd.CavityNumber = row["CavityNumber"].ToString();
                    dd.CompanyName = row["CompanyName"].ToString();
                    dd.CompanyCNBlockedDefects = row["CompanyCNBlockedDefects"].ToString();
                    dd.SetTech = row["SetTech"].ToString();
                    dd.TlCorrectiveAction = row["TlCorrectiveAction"].ToString();
                    dd.TlTechnician = !Convert.IsDBNull(row["TlTechnician"]) ? Convert.ToInt32(row["TlTechnician"]) : 0;
                    dd.TlQuantity = !Convert.IsDBNull(row["TlQuantity"]) ? Convert.ToInt32(row["TlQuantity"]) : 0;
                    dd.CaTech = row["CaTech"].ToString();
                    dd.MoldToolDescrip = row["MoldToolDescrip"].ToString();
                    dd.DftcEstTime = !Convert.IsDBNull(row["DftcEstTime"]) ? Convert.ToInt32(row["DftcEstTime"]) : 0;
                    dd.LineTotal = !Convert.IsDBNull(row["LineTotal"]) ? Convert.ToInt32(row["LineTotal"]) : 0;
                    dd.TimeCost = !Convert.IsDBNull(row["TimeCost"]) ? Convert.ToDouble(row["TimeCost"]) : 0;
                    dd.SubTotalCost = !Convert.IsDBNull(row["SubTotalCost"]) ? Convert.ToDouble(row["SubTotalCost"]) : 0;
                    dd.CompanyCNRepairCosts = row["CompanyCNRepairCosts"].ToString();
                    dd.MoldToolingPartNumber = row["MoldToolingPartNumber"].ToString();
                    dd.MoldToolCost = !Convert.IsDBNull(row["MoldToolCost"]) ? Convert.ToInt32(row["MoldToolCost"]) : 0;
                    dd.TlReplID = !Convert.IsDBNull(row["TlReplID"]) ? Convert.ToInt32(row["TlReplID"]) : 0;
                    dd.MoldConfig = row["MoldConfig"].ToString();
                    dd.MoldConfig2 = row["MoldConfig2"].ToString();
                    dd.TIRepairTime= !Convert.IsDBNull(row["TIRepairTime"]) ? Convert.ToDouble(row["TIRepairTime"]) : 0;
                    dd.MTXTotalTimeCal = MTXTotalTimeCal(dd.SetDate, dd.SetTime, dd.MldPullDate, dd.MldPullTime);
                    DC1.Add(dd);
                }
            }

            catch (Exception ex)
            {

            }

            return DC1;
        }

        public void UpdateTotalCycles(int MoldID)
        {
            try
            {
                int CID = GetCompanyID();
                SqlCommand cmd = new SqlCommand("Sp_UpdateTotalCycle", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MoldID", MoldID);
                cmd.Parameters.AddWithValue("@CompanyID", CID);

                con.Open();
                int rowAffected = cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                con.Close();
            }
        }


        public void DeleteChecksheetEmptyDate(int MoldID)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("DeleteCheckSheetDate", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MoldID", MoldID);
                con.Open();
                int rowAffected = cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                con.Close();
            }
        }


        public List<DefectTracking> DefectTracking(SqlConnection con, string StDate, string EDate, int MoldID)
        {
            int CID = GetCompanyID();

            SqlDataAdapter da2 = new SqlDataAdapter("procDefectTracking", con);
            da2.SelectCommand.CommandType = CommandType.StoredProcedure;
            da2.SelectCommand.Parameters.Add("@MoldDataID", SqlDbType.Float).Value = MoldID;
            da2.SelectCommand.Parameters.Add("@StartDate", SqlDbType.Date).Value = StDate;
            da2.SelectCommand.Parameters.Add("@EndDate", SqlDbType.Date).Value = EDate;
            da2.SelectCommand.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CID;

            DataTable dt1 = new DataTable();
            da2.Fill(dt1);
            List<DefectTracking> DC1 = new List<DefectTracking>();

            try
            {
                int i = 0;
                foreach (DataRow row in dt1.Rows)
                {
                    i += 1;
                    DefectTracking dd = new DefectTracking();
                    dd.SetDate = !Convert.IsDBNull(row["SetDate"]) ? Convert.ToDateTime(row["SetDate"]) : new DateTime();
                    dd.MoldDataID = !Convert.IsDBNull(row["MoldDataID"]) ? Convert.ToInt32(row["MoldDataID"]) : 0;
                    dd.SetTime = !Convert.IsDBNull(row["SetTime"]) ? Convert.ToDateTime(row["SetTime"]) : new DateTime();
                    dd.MoldName = row["MoldName"].ToString();
                    dd.MoldDesc = row["MoldDesc"].ToString();
                    dd.SetPressNumb = row["SetPressNumb"].ToString();
                    dd.DfctID = row["DfctID"].ToString();
                    dd.DfctCavNum = !Convert.IsDBNull(row["DfctCavNum"]) ? Convert.ToInt32(row["DfctCavNum"]) : 0;
                    dd.DfctDescript = row["DfctDescript"].ToString();
                    dd.ProductLine = row["ProductLine"].ToString();
                    dd.ProductPart = row["ProductPart"].ToString();
                    dd.DfctDate = !Convert.IsDBNull(row["DfctDate"]) ? Convert.ToDateTime(row["DfctDate"]) : new DateTime();
                    dd.EmployeeID = row["EmployeeID"].ToString();
                    dd.DfctNotes = row["DfctNotes"].ToString();
                    dd.DftcEstTime = !Convert.IsDBNull(row["DftcEstTime"]) ? Convert.ToInt32(row["DftcEstTime"]) : 0;
                    dd.MldPullDate = !Convert.IsDBNull(row["MldPullDate"]) ? Convert.ToDateTime(row["MldPullDate"]) : new DateTime();
                    dd.MldPullTime = !Convert.IsDBNull(row["MldPullTime"]) ? Convert.ToDateTime(row["MldPullTime"]) : new DateTime();
                    dd.CavityLocationNumber = row["CavityLocationNumber"].ToString();
                    dd.DfctTime = !Convert.IsDBNull(row["DfctTime"]) ? Convert.ToDateTime(row["DfctTime"]) : new DateTime();
                    dd.CavityNumber = row["CavityNumber"].ToString();
                    dd.CompanyName = row["CompanyName"].ToString();
                    dd.CompanyCNBlockedDefects = row["CompanyCNBlockedDefects"].ToString();
                    dd.SetTech = row["SetTech"].ToString();
                    dd.MoldConfig = row["MoldConfig"].ToString();
                    dd.MoldConfig2 = row["MoldConfig2"].ToString();
                    dd.SetID = !Convert.IsDBNull(row["SetID"]) ? Convert.ToInt32(row["SetID"]) : 0;
                    dd.MTXTotalTimeCal = MTXTotalTimeCal(dd.SetDate, dd.SetTime, dd.MldPullDate, dd.MldPullTime);
                    DC1.Add(dd);
                }
            }

            catch (Exception ex)
            {

            }

            return DC1;
        }



        public List<TotalTimeRun> TotalTimeRun(SqlConnection con, string StDate, string EDate, int MoldID)
        {
            int CID = GetCompanyID();
            SqlDataAdapter da2 = new SqlDataAdapter("procTotalTimeRunrpt", con);
            da2.SelectCommand.CommandType = CommandType.StoredProcedure;
            da2.SelectCommand.Parameters.Add("@MoldDataID", SqlDbType.Float).Value = MoldID;
            da2.SelectCommand.Parameters.Add("@StartDate", SqlDbType.Date).Value = StDate;
            da2.SelectCommand.Parameters.Add("@EndDate", SqlDbType.Date).Value = EDate;
            da2.SelectCommand.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CID;

            DataTable dt1 = new DataTable();
            da2.Fill(dt1);
            List<TotalTimeRun> DC1 = new List<TotalTimeRun>();

            try
            {
                int i = 0;
                foreach (DataRow row in dt1.Rows)
                {
                    i += 1;
                    TotalTimeRun dd = new TotalTimeRun();
                    dd.MoldDataID = !Convert.IsDBNull(row["MoldDataID"]) ? Convert.ToInt32(row["MoldDataID"]) : 0;
                    dd.MoldName = row["MoldName"].ToString();
                    dd.MoldDesc = row["MoldDesc"].ToString();
                    dd.SetDate = !Convert.IsDBNull(row["SetDate"]) ? Convert.ToDateTime(row["SetDate"]) : new DateTime();
                    dd.StartDate = !Convert.IsDBNull(row["StartDate"]) ? Convert.ToDateTime(row["StartDate"]) : new DateTime();
                    dd.StartTime = !Convert.IsDBNull(row["StartTime"]) ? Convert.ToDateTime(row["StartTime"]) : new DateTime();
                    dd.StopDate = !Convert.IsDBNull(row["StopDate"]) ? Convert.ToDateTime(row["StopDate"]) : new DateTime();
                    dd.StopTime = !Convert.IsDBNull(row["StopTime"]) ? Convert.ToDateTime(row["StopTime"]) : new DateTime();
                    dd.TotalTime = row["TotalTime"].ToString();
                    dd.MTXTotalMinsH = row["MTXTotalMinsH"].ToString();
                    dd.ProductLine = row["ProductLine"].ToString();
                    dd.ProductPart = row["ProductPart"].ToString();
                    dd.SetPressNumb = row["SetPressNumb"].ToString();
                    dd.CompanyName = row["CompanyName"].ToString();
                    dd.CompanyCNTotalTimeRun = row["CompanyCNTotalTimeRun"].ToString();
                    dd.MoldConfig = row["MoldConfig"].ToString();
                    dd.MoldConfig2 = row["MoldConfig2"].ToString();
                    dd.MldProductionCycles = !Convert.IsDBNull(row["MldProductionCycles"]) ? Convert.ToInt32(row["MldProductionCycles"]) : 0;

                    DC1.Add(dd);
                }
            }

            catch (Exception ex)
            {

            }

            return DC1;
        }

        public List<CurrentMoldRunning> GetCurrentMoldRunning2(SqlConnection con)
        {
            SqlDataAdapter da2 = new SqlDataAdapter("GetCurrentMoldRunningByCycleCount", con);
            da2.SelectCommand.CommandType = CommandType.StoredProcedure;

            DataTable dt1 = new DataTable();
            da2.Fill(dt1);
            List<CurrentMoldRunning> DC1 = new List<CurrentMoldRunning>();

            try
            {
                int i = 0;
                foreach (DataRow row in dt1.Rows)
                {
                    i += 1;
                    CurrentMoldRunning dd = new CurrentMoldRunning();
                    dd.Status = row["Status"].ToString();
                    dd.Count = !Convert.IsDBNull(row["Count"]) ? Convert.ToInt32(row["Count"]) : 0;
                    dd.TotalRunning = !Convert.IsDBNull(row["Total Running"]) ? Convert.ToInt32(row["Total Running"]) : 0;
                    dd.Percentage = !Convert.IsDBNull(row["Percentage"]) ? Convert.ToInt32(row["Percentage"]) : 0;
                    DC1.Add(dd);
                }
            }
            catch (Exception ex)
            {

            }
            return DC1;
        }

        public List<CurrentMoldRunning> GetCurrentMoldRunning(SqlConnection con)
        {
            int CID = GetCompanyID();
            SqlDataAdapter da2 = new SqlDataAdapter("GetCurrentMoldRunning", con);


            da2.SelectCommand.CommandType = CommandType.StoredProcedure;

            DataTable dt1 = new DataTable();
            da2.Fill(dt1);
            List<CurrentMoldRunning> DC1 = new List<CurrentMoldRunning>();

            try
            {
                int i = 0;
                foreach (DataRow row in dt1.Rows)
                {
                    i += 1;
                    CurrentMoldRunning dd = new CurrentMoldRunning();
                    dd.Status = row["Status"].ToString();
                    dd.Count = !Convert.IsDBNull(row["Count"]) ? Convert.ToInt32(row["Count"]) : 0;
                    dd.TotalRunning = !Convert.IsDBNull(row["Total Running"]) ? Convert.ToInt32(row["Total Running"]) : 0;
                    dd.Percentage = !Convert.IsDBNull(row["Percentage"]) ? Convert.ToInt32(row["Percentage"]) : 0;
                    DC1.Add(dd);
                }
            }

            catch (Exception ex)
            {

            }

            return DC1;
        }


        public List<TSGuideRPTWrapper> TSGuideRPTWrapper(SqlConnection con, int MoldID)
        {
            int CID = GetCompanyID();
            SqlDataAdapter da2 = new SqlDataAdapter("ProcTSGuideRPTWrapper", con);
            da2.SelectCommand.CommandType = CommandType.StoredProcedure;
            da2.SelectCommand.Parameters.Add("@MoldDataID", SqlDbType.Int).Value = MoldID;
            da2.SelectCommand.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CID;

            DataTable dt1 = new DataTable();
            da2.Fill(dt1);
            List<TSGuideRPTWrapper> DC1 = new List<TSGuideRPTWrapper>();

            try
            {
                int i = 0;
                foreach (DataRow row in dt1.Rows)
                {
                    i += 1;
                    TSGuideRPTWrapper dd = new TSGuideRPTWrapper();
                    dd.TSSeqNum = !Convert.IsDBNull(row["TSSeqNum"]) ? Convert.ToInt32(row["TSSeqNum"]) : 0;
                    dd.TSGuide = !Convert.IsDBNull(row["TSSeqNum"]) ? Convert.ToInt32(row["TSGuide"]) : 0;
                    dd.MoldDataID = !Convert.IsDBNull(row["TSSeqNum"]) ? Convert.ToInt32(row["MoldDataID"]) : 0;
                    dd.TSDefects = row["TSDefects"].ToString();
                    dd.TSExplanation = row["TSExplanation"].ToString();
                    dd.TSToolInv = row["TSToolInv"].ToString();
                    dd.TSProbCause = row["TSProbCause"].ToString();
                    dd.TSSolution = row["TSSolution"].ToString();
                    dd.ImagePath =  row["ImagePath"].ToString();
                    dd.TSType = row["TSType"].ToString();
                    dd.TSPreventAction = row["TSPreventAction"].ToString();
                    dd.MoldName = row["MoldName"].ToString();
                    dd.MoldDesc = row["MoldDesc"].ToString();
                    dd.CompanyCNTroubleShoot = row["CompanyCNTroubleShoot"].ToString();
                    dd.CompanyName = row["CompanyName"].ToString();
                    DC1.Add(dd);
                }
            }

            catch (Exception ex)
            {

            }

            return DC1;
        }

        public List<ToolingMaintenanceInstruction> GetMaintenanceTooling(SqlConnection con, int MoldID)
        {
            int CID = GetCompanyID();
            SqlDataAdapter da2 = new SqlDataAdapter("proc_GetToolingCorrective", con);
            da2.SelectCommand.CommandType = CommandType.StoredProcedure;
            da2.SelectCommand.Parameters.Add("@MoldID", SqlDbType.Int).Value = MoldID;
            da2.SelectCommand.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CID;

            DataTable dt1 = new DataTable();
            da2.Fill(dt1);
            List<ToolingMaintenanceInstruction> DC1 = new List<ToolingMaintenanceInstruction>();

            try
            {
                int i = 0;
                foreach (DataRow row in dt1.Rows)
                {
                    i += 1;
                    ToolingMaintenanceInstruction dd = new ToolingMaintenanceInstruction();
                    dd.MoldToolingID = !Convert.IsDBNull(row["MoldToolingID"]) ? Convert.ToInt32(row["MoldToolingID"]) : 0;
                    dd.MoldToolDescrip = row["MoldToolDescrip"].ToString();
                    dd.MoldToolingPartNumber = row["MoldToolingPartNumber"].ToString();
                    dd.POH = row["POH"].ToString();
                    DC1.Add(dd);
                }
            }

            catch (Exception ex)
            {

            }

            return DC1;
        }


        public List<CavNumDropdown> CavDropDown(SqlConnection con, int MoldID)
        {
            int CID = GetCompanyID();
            SqlDataAdapter da2 = new SqlDataAdapter("procDfctCavNumDropdown", con);
            da2.SelectCommand.CommandType = CommandType.StoredProcedure;
            da2.SelectCommand.Parameters.Add("@MoldDataID", SqlDbType.Int).Value = MoldID;
            da2.SelectCommand.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CID;

            DataTable dt1 = new DataTable();
            da2.Fill(dt1);
            List<CavNumDropdown> DC1 = new List<CavNumDropdown>();

            try
            {
                int i = 0;
                foreach (DataRow row in dt1.Rows)
                {
                    i += 1;
                    CavNumDropdown dd = new CavNumDropdown();
                    dd.CavityNumberID = !Convert.IsDBNull(row["CavityNumberID"]) ? Convert.ToInt32(row["CavityNumberID"]) : 0;
                    dd.CavityActive = !Convert.IsDBNull(row["CavityActive"]) ? Convert.ToBoolean(row["CavityActive"]) : false;
                    dd.CavID = row["Cav ID"].ToString();
                    dd.Pos = row["Pos"].ToString();
                    dd.Act = row["Act"].ToString();
                    dd.MoldDataID = !Convert.IsDBNull(row["MoldDataID"]) ? Convert.ToInt32(row["MoldDataID"]) : 0;
                    DC1.Add(dd);
                }
            }

            catch (Exception ex)
            {

            }

            return DC1;
        }


        public DataTable IMLSheet(SqlConnection con, int MoldID=0)
        {
            int CID = GetCompanyID();
            SqlDataAdapter da2 = new SqlDataAdapter("procIMLSheet", con);
            da2.SelectCommand.CommandType = CommandType.StoredProcedure;
            da2.SelectCommand.Parameters.Add("@MoldID", SqlDbType.Int).Value = MoldID;
            da2.SelectCommand.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CID;

            DataTable dt1 = new DataTable();
            da2.Fill(dt1);

            //DataSet Ds = new DataSet();
            //Ds.Tables.Add(dt1);

            return dt1;
        }


        public DataTable TechTips(SqlConnection con, int MoldID = 0)
        {
            int CID = GetCompanyID();
            SqlDataAdapter da2 = new SqlDataAdapter("procTechTipsReport", con);
            da2.SelectCommand.CommandType = CommandType.StoredProcedure;
            da2.SelectCommand.Parameters.Add("@MoldID", SqlDbType.Int).Value = MoldID;
            da2.SelectCommand.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CID;

            DataTable dt1 = new DataTable();
            da2.Fill(dt1);

            //DataSet Ds = new DataSet();
            //Ds.Tables.Add(dt1);

            return dt1;
        }

        public DataTable MaintenanceAlertStats(SqlConnection con)
        {
            int CID = GetCompanyID();
            string Date = System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            SqlDataAdapter da2 = new SqlDataAdapter("procMaintenanceAlert", con);
            da2.SelectCommand.CommandType = CommandType.StoredProcedure;
            da2.SelectCommand.Parameters.Add("@SystemDateTime", SqlDbType.Date).Value = Date;
            da2.SelectCommand.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CID;
            da2.SelectCommand.Parameters.Add("@IsOrg", SqlDbType.Int).Value = IsCalledbyOrg();

            DataTable dt1 = new DataTable();
            da2.Fill(dt1);
            return dt1;
        }


        public DataTable MoldTooling(SqlConnection con, int MoldID = 0)
        {
            int CID = GetCompanyID();
            SqlDataAdapter da2 = new SqlDataAdapter("procMoldToolingRpt", con);
            da2.SelectCommand.CommandType = CommandType.StoredProcedure;
            da2.SelectCommand.Parameters.Add("@MoldID", SqlDbType.Int).Value = MoldID;
            da2.SelectCommand.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CID;

            DataTable dt1 = new DataTable();
            da2.Fill(dt1);

            //DataSet Ds = new DataSet();
            //Ds.Tables.Add(dt1);

            return dt1;
        }

        public DataTable LastShot (SqlConnection con, int MoldID = 0)
        {
            int CID = GetCompanyID();
            SqlDataAdapter da2 = new SqlDataAdapter("procLastShotReport", con);
            da2.SelectCommand.CommandType = CommandType.StoredProcedure;
            da2.SelectCommand.Parameters.Add("@MoldID", SqlDbType.Int).Value = MoldID;
            da2.SelectCommand.Parameters.Add("@CompanyID", SqlDbType.Int).Value = CID;

            DataTable dt1 = new DataTable();
            da2.Fill(dt1);

         

            return dt1;
        }


        public DataTable ShowMOLDTRAXReport(SqlConnection con, string Name, string Startdate, string EndDate)
        {
            SqlDataAdapter da2 = new SqlDataAdapter(Name, con);
            da2.SelectCommand.CommandType = CommandType.StoredProcedure;
            da2.SelectCommand.Parameters.Add("@StartDate", SqlDbType.Date).Value = Startdate;
            da2.SelectCommand.Parameters.Add("@EndDate", SqlDbType.Date).Value = EndDate;
            da2.SelectCommand.Parameters.Add("@CompanyID", SqlDbType.Int).Value = GetCompanyID();
            da2.SelectCommand.Parameters.Add("@IsOrg", SqlDbType.Int).Value = IsCalledbyOrg();

            DataTable dt1 = new DataTable();
            da2.Fill(dt1);

            //foreach (DataRow dr in dt1.Rows) // search whole table
            //{
            //    var sds = dr["Cycle Count"].ToString();
            //    dr["Cycle Count"] = dr["Cycle Count"].ToString() != "" ? String.Format("{0:N}", dr["Cycle Count"].ToString()) : dr["Cycle Count"].ToString();
            //}
            //DataSet Ds = new DataSet();
            //Ds.Tables.Add(dt1);

            return dt1;
        }


        public DataTable MoldListWrapper(SqlConnection con, string Name)
        {
            SqlDataAdapter da2 = new SqlDataAdapter(Name, con);
            da2.SelectCommand.CommandType = CommandType.StoredProcedure;
            da2.SelectCommand.Parameters.Add("@CompanyID", SqlDbType.Int).Value = GetCompanyID();
            da2.SelectCommand.Parameters.Add("@IsOrg", SqlDbType.Int).Value = IsCalledbyOrg();

            DataTable dt1 = new DataTable();
            da2.Fill(dt1);
            return dt1;
        }

        public DataTable InventoryTracking(SqlConnection con, string Name)
        {
            SqlDataAdapter da2 = new SqlDataAdapter(Name, con);
            da2.SelectCommand.CommandType = CommandType.StoredProcedure;
            da2.SelectCommand.Parameters.Add("@CompanyID", SqlDbType.Int).Value = GetCompanyID();
            da2.SelectCommand.Parameters.Add("@IsOrg", SqlDbType.Int).Value = IsCalledbyOrg();

            DataTable dt1 = new DataTable();
            da2.Fill(dt1);

            //DataSet Ds = new DataSet();
            //Ds.Tables.Add(dt1);

            return dt1;
        }

        public DataTable MaintenanceAlert(SqlConnection con, string Name, DateTime CurrentDate)
        {
            SqlDataAdapter da2 = new SqlDataAdapter(Name, con);
            da2.SelectCommand.CommandType = CommandType.StoredProcedure;
            da2.SelectCommand.Parameters.Add("@SystemDateTime", SqlDbType.DateTime).Value = CurrentDate;

            DataTable dt1 = new DataTable();
            da2.Fill(dt1);

            //DataSet Ds = new DataSet();
            //Ds.Tables.Add(dt1);

            return dt1;
        }

        public List<tblTSGuideViewModel> GetTblTSGuideList(int MoldID)
        {
            var dd = db.Database.SqlQuery<tblTSGuideViewModel>("exec proc_GetTblTSGuide @MoldID, @CompanyID", new SqlParameter("@MoldID", MoldID), new SqlParameter("@CompanyID", GetCompanyID())).ToList();
            return dd;
        }

        public List<TblRoverSetDataViewModel> GetMaintenanceTblRoverSetData(int MoldID)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            SqlDataAdapter da2 = new SqlDataAdapter("proc_GettblRoverSetData", con);
            da2.SelectCommand.CommandType = CommandType.StoredProcedure;
            da2.SelectCommand.Parameters.Add("@MoldID", SqlDbType.Int).Value = MoldID;
            da2.SelectCommand.Parameters.Add("@CompanyID", SqlDbType.Int).Value = GetCompanyID();

            DataTable dt1 = new DataTable();
            da2.Fill(dt1);
            List<TblRoverSetDataViewModel> DC1 = new List<TblRoverSetDataViewModel>();

            try
            {
                int i = 0;
                foreach (DataRow row in dt1.Rows)
                {
                    i += 1;
                    TblRoverSetDataViewModel dd = new TblRoverSetDataViewModel();

                    dd.SetID = !Convert.IsDBNull(row["SetID"]) ? Convert.ToInt32(row["SetID"]) : 0;
                    dd.MoldDataID = !Convert.IsDBNull(row["MoldDataID"]) ? Convert.ToInt32(row["MoldDataID"]) : 0;
                    dd.SetDate = !Convert.IsDBNull(row["SetDate"]) ? Convert.ToDateTime(row["SetDate"]) : new DateTime();
                    dd.SetTime = !Convert.IsDBNull(row["SetTime"]) ? Convert.ToDateTime(row["SetTime"]) : new DateTime();
                    dd.SetTech = !Convert.IsDBNull(row["SetTech"]) ? Convert.ToInt32(row["SetTech"]) : 0; 
                    dd.SetPressNumb =  row["SetPressNumb"].ToString();
                    dd.MldPullMaintRequired = row["MldPullMaintRequired"].ToString();
                    dd.MldPullDate = !Convert.IsDBNull(row["MldPullDate"]) ? Convert.ToDateTime(row["MldPullDate"]) : new DateTime();
                    dd.MldPullTime = !Convert.IsDBNull(row["MldPullTime"]) ? Convert.ToDateTime(row["MldPullTime"]) : new DateTime();
                    dd.MldPullTech = !Convert.IsDBNull(row["MldPullTech"]) ? Convert.ToInt32(row["MldPullTech"]) : 0;
                    dd.MoldConfig = row["MoldConfig"].ToString();
                    dd.MldRepairedDate = !Convert.IsDBNull(row["MldRepairedDate"]) ? Convert.ToDateTime(row["MldRepairedDate"]) : new DateTime();
                    dd.MldRepairedTime = !Convert.IsDBNull(row["MldRepairedTime"]) ? Convert.ToDouble(row["MldRepairedTime"]) : 0;
                    dd.MldRepairdBy = !Convert.IsDBNull(row["MldRepairdBy"]) ? Convert.ToInt32(row["MldRepairdBy"]) : 0;
                    dd.MldWorkOrder = row["MldWorkOrder"].ToString();
                    dd.MldProductionCycles = !Convert.IsDBNull(row["MldProductionCycles"]) ? Convert.ToInt32(row["MldProductionCycles"]) : 0;
                    dd.ImageExtension = row["ImageExtension"].ToString();
                    dd.CycleCounter = !Convert.IsDBNull(row["CycleCounter"]) ? Convert.ToInt32(row["CycleCounter"]) : 0;
                    dd.MoldConfig2 = row["MoldConfig2"].ToString();
                    dd.MoldDefectMapPath = row["MoldDefectMapPath"].ToString();

                    DC1.Add(dd);
                }
            }

            catch (Exception ex)
            {

            }

            return DC1;
        }

        //public void UpdateChecksheetDate(int InspectID, DateTime Date)
        //{
        //    using (SqlConnection con = new SqlConnection(constring))
        //    {
        //        using (SqlCommand cmd = new SqlCommand("proc_UpdateDateinTblInspection", con))
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;

        //            cmd.Parameters.Add("@InspectID", SqlDbType.Int).Value = InspectID;
        //            cmd.Parameters.Add("@Date", SqlDbType.NVarChar).Value = Date;

        //            con.Open();
        //            cmd.ExecuteNonQuery();
        //        }
        //    }
        //}


        public List<tblRoverSetData> GetTblRoverSetData()
        {
            SqlDataAdapter da2 = new SqlDataAdapter("Proc_GetALLTblRoverSetData", con);
            da2.SelectCommand.Parameters.Add("@CompanyID", SqlDbType.Int).Value = GetCompanyID();
            da2.SelectCommand.CommandType = CommandType.StoredProcedure;

            DataTable dt1 = new DataTable();
            da2.Fill(dt1);
            List<tblRoverSetData> DC1 = new List<tblRoverSetData>();

            try
            {
                int i = 0;
                foreach (DataRow row in dt1.Rows)
                {
                    i += 1;
                    tblRoverSetData dd = new tblRoverSetData();
                    dd.SetID = !Convert.IsDBNull(row["SetID"]) ? Convert.ToInt32(row["SetID"]) : 0;
                    dd.MoldDataID = !Convert.IsDBNull(row["MoldDataID"]) ? Convert.ToInt32(row["MoldDataID"]) : 0;
                    dd.SetDate = !Convert.IsDBNull(row["SetDate"]) ? Convert.ToDateTime(row["SetDate"]) : new DateTime();
                    dd.SetTime = !Convert.IsDBNull(row["SetTime"]) ? Convert.ToDateTime(row["SetTime"]) : new DateTime();
                    dd.SetTech = !Convert.IsDBNull(row["SetTech"]) ? Convert.ToInt32(row["SetTech"]) : 0;
                    dd.SetPressNumb = row["SetPressNumb"].ToString();
                    //dd.MldPullMaintRequired = row["MldPullMaintRequired"].ToString();
                    dd.MldPullDate = !Convert.IsDBNull(row["MldPullDate"]) ? Convert.ToDateTime(row["MldPullDate"]) : new DateTime();
                    dd.MldPullTime = !Convert.IsDBNull(row["MldPullTime"]) ? Convert.ToDateTime(row["MldPullTime"]) : new DateTime();
                    dd.MldPullTech = !Convert.IsDBNull(row["MldPullTech"]) ? Convert.ToInt32(row["MldPullTech"]) : 0;
                    dd.MoldConfig = row["MoldConfig"].ToString();
                    dd.MldRepairedDate = !Convert.IsDBNull(row["MldRepairedDate"]) ? Convert.ToDateTime(row["MldRepairedDate"]) : new DateTime();
                    dd.MldRepairedTime = !Convert.IsDBNull(row["MldRepairedTime"]) ? Convert.ToDouble(row["MldRepairedTime"]) : 0;
                    dd.MldRepairdBy = !Convert.IsDBNull(row["MldRepairdBy"]) ? Convert.ToInt32(row["MldRepairdBy"]) : 0;
                    dd.MldWorkOrder = row["MldWorkOrder"].ToString();
                    dd.MldProductionCycles = !Convert.IsDBNull(row["MldProductionCycles"]) ? Convert.ToInt32(row["MldProductionCycles"]) : 0;
                    //dd.ImageExtension = row["ImageExtension"].ToString();
                    dd.CycleCounter = !Convert.IsDBNull(row["CycleCounter"]) ? Convert.ToInt32(row["CycleCounter"]) : 0;
                    dd.MoldConfig2 = row["MoldConfig2"].ToString();
                    //dd.MoldDefectMapPath = row["MoldDefectMapPath"].ToString();

                    DC1.Add(dd);
                }
            }

            catch (Exception ex)
            {

            }

            return DC1;
        }



        //public static string RenderRazorViewToString(ControllerContext controllerContext, string viewName, object model)
        //{
        //    controllerContext.Controller.ViewData.Model = model;

        //    using (var stringWriter = new StringWriter())
        //    {
        //        try
        //        {
        //            var viewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
        //            var viewContext = new ViewContext(controllerContext, viewResult.View, controllerContext.Controller.ViewData, controllerContext.Controller.TempData, stringWriter);
        //            viewResult.View.Render(viewContext, stringWriter);
        //            viewResult.ViewEngine.ReleaseView(controllerContext, viewResult.View);
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //        return stringWriter.GetStringBuilder().ToString();
        //    }
        //}

        public string ReturnDate(DateTime dd)
        {
            string date = dd.ToString("yyyy-MM-dd hh:mm:ss");
            return date;
        }

        public string ReturnOnlyDate(DateTime dd)
        {
            string date = dd.ToString("yyyy-MM-dd");
            return date;
        }

        public string MTXTotalTimeCal(DateTime? vSDate1, DateTime? vSTime1, DateTime? vPDate1, DateTime? vPTime1)
        {
            string vSDate = vSDate1 != null ? Convert.ToDateTime(vSDate1).ToShortDateString() : null;
            string vSTime = vSTime1 != null ? Convert.ToDateTime(vSTime1).ToShortTimeString() : null;
            string vPDate = vPDate1 != null ? Convert.ToDateTime(vPDate1).ToShortDateString() : null;
            string vPTime = vPTime1 != null ? Convert.ToDateTime(vPTime1).ToShortTimeString() : null;


            string MTXTotalTimeCal = "";
            double dTDays = 0;
            string sTDays = "";
            double dTHours = 0;
            string sTHours = "";
            double dTMins = 0;
            double dsTMins = 0;
            string sTMins = "";
            string sTMC = "";
            string sTHC = "";

            if (vSDate == null)
            {
                return MTXTotalTimeCal = "";
            }
            else if (vSTime == null)
            {
                return MTXTotalTimeCal = "";
            }
            else if (vPDate == null)
            {
                return MTXTotalTimeCal = "";
            }
            else if (vPDate == "1/1/0001")
            {
                return MTXTotalTimeCal = "";
            }
            else if (vPTime == null)
            {
                return MTXTotalTimeCal = "";
            }


            //if (vSDate == null && vSTime == null && vPDate == null && vPDate == "1/1/0001" && vPTime == null)
            //{
            //    return MTXTotalTimeCal = "";
            //}

            if (CheckDate(vSDate.ToString()))
            {
                TimeSpan ts = Convert.ToDateTime((vPDate + " " + vPTime)).Subtract(Convert.ToDateTime((vSDate + " " + vSTime)));
                dTMins = ts.TotalMinutes;
            }
            else
            {
                dTMins = 0;
            }

            dTDays = Math.Truncate((dTMins / 60 / 24));

            dTHours = (dTDays * 24) - Math.Truncate((dTMins / 60));

            //Calculating Hours

            if (dTHours > 0)
            {
                dTHours = 24 - dTHours;
            }
            else if (dTHours == 0)
            {
                dTHours = 0;
            }
            else if (dTHours < 0)
            {
                dTHours = dTHours * -1;
            }

            //Calculating Minutes

            dsTMins = Math.Truncate(dTMins) - (((dTDays * 24) + dTHours) * 60);

            //Checking Plural

            if (dTDays == 0)
            {
                sTDays = "";
            }
            else if (dTDays == 1)
            {
                sTDays = dTDays + " Day";
            }
            else if (dTDays > 1)
            {
                sTDays = dTDays + " Days";
            }

            if (dTHours == 0)
            {
                sTHours = "";
            }
            else if (dTHours == 1)
            {
                sTHours = dTHours + " Hour";
            }
            else if (dTHours > 1)
            {
                sTHours = dTHours + " Hours";
            }

            if (dsTMins == 0)
            {
                sTMins = "";
            }
            else if (dsTMins == 1)
            {
                sTMins = dsTMins + " Minute";
            }
            else if (dsTMins > 1)
            {
                sTMins = dsTMins + " Minutes";
            }

            //Comma Grammer

            if (sTDays == "")
            {
                sTHC = "";
            }
            else
            {
                sTHC = ", ";
            }

            if (sTHours == "")
            {
                sTMC = "";
            }
            else
            {
                sTMC = ", ";
            }

            if (sTMins == "")
            {
                sTMC = "";
            }

            MTXTotalTimeCal = sTDays + sTHC + sTHours + sTMC + sTMins;
            return MTXTotalTimeCal;
        }

        public double MTXEstCycleTime(DateTime? vSDate1, DateTime? vSTime1, DateTime? vPDate1, DateTime? vPTime1)
        {
            string vSDate = vSDate1 != null ? Convert.ToDateTime(vSDate1).ToShortDateString() : null;
            string vSTime = vSTime1 != null ? Convert.ToDateTime(vSTime1).ToShortTimeString() : null;
            string vPDate = vPDate1 != null ? Convert.ToDateTime(vPDate1).ToShortDateString() : null;
            string vPTime = vPTime1 != null ? Convert.ToDateTime(vPTime1).ToShortTimeString() : null;

            double dTMins = 0;

            if (vSDate == null)
            {
                return dTMins = 0;
            }
            else if (vSDate == "1/1/0001")
            {
                return dTMins = 0;
            }
            else if (vSTime == null)
            {
                return dTMins = 0;
            }


            //if (vSDate == null && vSDate == "1/1/0001" && vSTime == null )
            //{
            //    return dTMins = 0;
            //}
            

            if (vPDate == null || vPDate == "1/1/0001" || vPTime == null)
            {
                if (vSDate == null || vSDate == "1/1/0001")
                {
                    return dTMins = 0;
                }
                else
                {
                    vPDate = System.DateTime.Now.ToShortDateString();
                }
            }
            else if (vPTime == null)
            {
                if (vSTime == null)
                {
                    return dTMins = 0;
                }
                else
                {
                    vPTime = System.DateTime.Now.ToShortTimeString();
                }
            }

            if (CheckDate(vSDate.ToString()))
            {
                TimeSpan ts = Convert.ToDateTime((vPDate + " " + vPTime)).Subtract(Convert.ToDateTime((vSDate + " " + vSTime)));
                dTMins = Convert.ToDouble(ts.TotalMinutes);
            }
            else
            {
                dTMins = 0;
            }

            return dTMins;
        }

        protected bool CheckDate(String date)
        {
            try
            {
                DateTime dt = DateTime.Parse(date);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public int ReturnSelectedMoldID(int ID = 0, int CID=0)
        {
            int i = 0;
            if (ID == 0)
            {
                var ds = db.TblMoldData.Where(x=> x.CompanyID == CID).OrderBy(x => x.MoldName).FirstOrDefault();
                if (ds != null)
                {
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

        public List<FinalChecklstResult> CreateChecksheetData(SqlConnection con, int Mid = 0)
        {
            SqlDataAdapter da2 = new SqlDataAdapter("proc_CreateChecksheetData", con);
            da2.SelectCommand.CommandType = CommandType.StoredProcedure;
            da2.SelectCommand.Parameters.Add("@MID", SqlDbType.Int).Value = Mid;

            DataTable dt1 = new DataTable();
            da2.Fill(dt1);
            List<FinalChecklstResult> DC1 = new List<FinalChecklstResult>();

            try
            {
                int i = 0;
                foreach (DataRow row in dt1.Rows)
                {
                    i += 1;
                    FinalChecklstResult dd = new FinalChecklstResult();
                    dd.CatID = !Convert.IsDBNull(row["CatID"]) ? Convert.ToInt32(row["CatID"]) : 0;
                    dd.CategoryName = row["CategoryName"].ToString();
                    dd.InspectionName = row["InspectionName"].ToString();
                    dd.InspectionDetailID1 = !Convert.IsDBNull(row["InspectionDetailID1"]) ? Convert.ToInt32(row["InspectionDetailID1"]) : 0;
                    dd.InspectionDetailID2 = !Convert.IsDBNull(row["InspectionDetailID2"]) ? Convert.ToInt32(row["InspectionDetailID2"]) : 0;
                    dd.InspectionDetailID3 = !Convert.IsDBNull(row["InspectionDetailID3"]) ? Convert.ToInt32(row["InspectionDetailID3"]) : 0;
                    dd.Ok1 = !Convert.IsDBNull(row["Ok1"]) ? Convert.ToBoolean(row["Ok1"]) : false;
                    dd.Ok2 = !Convert.IsDBNull(row["Ok2"]) ? Convert.ToBoolean(row["Ok2"]) : false;
                    dd.Ok3 = !Convert.IsDBNull(row["Ok3"]) ? Convert.ToBoolean(row["Ok3"]) : false;
                    dd.Attention1 = !Convert.IsDBNull(row["Attention1"]) ? Convert.ToBoolean(row["Attention1"]) : false;
                    dd.Attention2 = !Convert.IsDBNull(row["Attention2"]) ? Convert.ToBoolean(row["Attention2"]) : false;
                    dd.Attention3 = !Convert.IsDBNull(row["Attention3"]) ? Convert.ToBoolean(row["Attention3"]) : false;
                    dd.NoRun1 = !Convert.IsDBNull(row["NoRun1"]) ? Convert.ToBoolean(row["NoRun1"]) : false;
                    dd.NoRun2 = !Convert.IsDBNull(row["NoRun2"]) ? Convert.ToBoolean(row["NoRun2"]) : false;
                    dd.NoRun3 = !Convert.IsDBNull(row["NoRun3"]) ? Convert.ToBoolean(row["NoRun3"]) : false;
                    dd.Date1 = !Convert.IsDBNull(row["Date1"]) ? Convert.ToDateTime(row["Date1"]) : new DateTime();
                    dd.Date2 = !Convert.IsDBNull(row["Date2"]) ? Convert.ToDateTime(row["Date2"]) : new DateTime();
                    dd.Date3 = !Convert.IsDBNull(row["Date3"]) ? Convert.ToDateTime(row["Date3"]) : new DateTime();
                    dd.AdditionalComments1 = row["AdditionalComments1"].ToString();
                    dd.AdditionalComments2 = row["AdditionalComments2"].ToString();
                    dd.AdditionalComments3 = row["AdditionalComments3"].ToString();
                    dd.InspectedBy1 = !Convert.IsDBNull(row["InspectedBy1"]) ? Convert.ToInt32(row["InspectedBy1"]) : 0;
                    dd.InspectedBy2 = !Convert.IsDBNull(row["InspectedBy2"]) ? Convert.ToInt32(row["InspectedBy2"]) : 0;
                    dd.InspectedBy3 = !Convert.IsDBNull(row["InspectedBy3"]) ? Convert.ToInt32(row["InspectedBy3"]) : 0;
                    dd.InspectID1 = !Convert.IsDBNull(row["InspectID1"]) ? Convert.ToInt32(row["InspectID1"]) : 0;
                    dd.InspectID2 = !Convert.IsDBNull(row["InspectID2"]) ? Convert.ToInt32(row["InspectID2"]) : 0;
                    dd.InspectID3 = !Convert.IsDBNull(row["InspectID3"]) ? Convert.ToInt32(row["InspectID3"]) : 0;

                    DC1.Add(dd);
                }
            }
            catch (Exception ex)
            {

            }

            return DC1;

        }

        public List<FinalChecklstResult> GetChecksheetData(SqlConnection con, string Date="", int Mid = 0, int IsNext=0, int ISNew = 0)
        {
            if (Date == "0001-01-01")
            {
                Date = "1753-01-01";
            }

            SqlDataAdapter da2 = new SqlDataAdapter("proc_GetChecksheetData", con);
            da2.SelectCommand.CommandType = CommandType.StoredProcedure;
            da2.SelectCommand.Parameters.Add("@dtInpectDate", SqlDbType.DateTime).Value = Date;
            da2.SelectCommand.Parameters.Add("@MID", SqlDbType.Int).Value = Mid;
            da2.SelectCommand.Parameters.Add("@IsNext", SqlDbType.Int).Value = IsNext;
            da2.SelectCommand.Parameters.Add("@IsNew", SqlDbType.Int).Value = ISNew;
            da2.SelectCommand.Parameters.Add("@CompanyID", SqlDbType.Int).Value = GetCompanyID();

            DataTable dt1 = new DataTable();
            da2.Fill(dt1);
            List<FinalChecklstResult> DC1 = new List<FinalChecklstResult>();

            try
            {
                int i = 0;
                foreach (DataRow row in dt1.Rows)
                {
                    i += 1;
                    FinalChecklstResult dd = new FinalChecklstResult();
                    dd.CatID = !Convert.IsDBNull(row["CatID"]) ? Convert.ToInt32(row["CatID"]) : 0;
                    dd.CategoryName = row["CategoryName"].ToString();
                    dd.InspectionName = row["InspectionName"].ToString();
                    dd.InspectionDetailID1 = !Convert.IsDBNull(row["InspectionDetailID1"]) ? Convert.ToInt32(row["InspectionDetailID1"]) : 0;
                    dd.InspectionDetailID2 = !Convert.IsDBNull(row["InspectionDetailID2"]) ? Convert.ToInt32(row["InspectionDetailID2"]) : 0;
                    dd.InspectionDetailID3 = !Convert.IsDBNull(row["InspectionDetailID3"]) ? Convert.ToInt32(row["InspectionDetailID3"]) : 0;
                    dd.Ok1 = !Convert.IsDBNull(row["Ok1"]) ? Convert.ToBoolean(row["Ok1"]) : false;
                    dd.Ok2 = !Convert.IsDBNull(row["Ok2"]) ? Convert.ToBoolean(row["Ok2"]) : false;
                    dd.Ok3 = !Convert.IsDBNull(row["Ok3"]) ? Convert.ToBoolean(row["Ok3"]) : false;
                    dd.Attention1 = !Convert.IsDBNull(row["Attention1"]) ? Convert.ToBoolean(row["Attention1"]) : false;
                    dd.Attention2 = !Convert.IsDBNull(row["Attention2"]) ? Convert.ToBoolean(row["Attention2"]) : false;
                    dd.Attention3 = !Convert.IsDBNull(row["Attention3"]) ? Convert.ToBoolean(row["Attention3"]) : false;
                    dd.NoRun1 = !Convert.IsDBNull(row["NoRun1"]) ? Convert.ToBoolean(row["NoRun1"]) : false;
                    dd.NoRun2 = !Convert.IsDBNull(row["NoRun2"]) ? Convert.ToBoolean(row["NoRun2"]) : false;
                    dd.NoRun3 = !Convert.IsDBNull(row["NoRun3"]) ? Convert.ToBoolean(row["NoRun3"]) : false;
                    dd.Date1 = !Convert.IsDBNull(row["Date1"]) ? Convert.ToDateTime(row["Date1"]) : new DateTime();
                    dd.Date2 = !Convert.IsDBNull(row["Date2"]) ? Convert.ToDateTime(row["Date2"]) : new DateTime();
                    dd.Date3 = !Convert.IsDBNull(row["Date3"]) ? Convert.ToDateTime(row["Date3"]) : new DateTime();
                    dd.AdditionalComments1 = row["AdditionalComments1"].ToString();
                    dd.AdditionalComments2 = row["AdditionalComments2"].ToString();
                    dd.AdditionalComments3 = row["AdditionalComments3"].ToString();
                    dd.InspectedBy1 = !Convert.IsDBNull(row["InspectedBy1"]) ? Convert.ToInt32(row["InspectedBy1"]) : 0;
                    dd.InspectedBy2 = !Convert.IsDBNull(row["InspectedBy2"]) ? Convert.ToInt32(row["InspectedBy2"]) : 0;
                    dd.InspectedBy3 = !Convert.IsDBNull(row["InspectedBy3"]) ? Convert.ToInt32(row["InspectedBy3"]) : 0;
                    dd.InspectID1 = !Convert.IsDBNull(row["InspectID1"]) ? Convert.ToInt32(row["InspectID1"]) : 0;
                    dd.InspectID2 = !Convert.IsDBNull(row["InspectID2"]) ? Convert.ToInt32(row["InspectID2"]) : 0;
                    dd.InspectID3 = !Convert.IsDBNull(row["InspectID3"]) ? Convert.ToInt32(row["InspectID3"]) : 0;

                    DC1.Add(dd);
                }
            }
            catch (Exception ex)
            {

            }

            return DC1;

        }

        public List<FinalChecklstResult> UpdateRecords(SqlConnection con, string Date, int Mid=0)
        {
            SqlDataAdapter da2 = new SqlDataAdapter("proc_UpdateCheckSheet", con);
            da2.SelectCommand.CommandType = CommandType.StoredProcedure;
            da2.SelectCommand.Parameters.Add("@dtInpectDate", SqlDbType.DateTime).Value = Date;
            da2.SelectCommand.Parameters.Add("@MID", SqlDbType.Int).Value = Mid;


            DataTable dt1 = new DataTable();
            da2.Fill(dt1);

            //DataSet Ds = new DataSet();
            //Ds.Tables.Add(dt1);

            List<FinalChecklstResult> DC1 = new List<FinalChecklstResult>();

            try
            {
                int i = 0;
                foreach (DataRow row in dt1.Rows)
                {
                    i += 1;
                    FinalChecklstResult dd = new FinalChecklstResult();
                    dd.CatID = !Convert.IsDBNull(row["CatID"]) ? Convert.ToInt32(row["CatID"]) : 0;
                    dd.CategoryName = row["CategoryName"].ToString();
                    dd.InspectionName = row["InspectionName"].ToString();
                    dd.InspectionDetailID1 = !Convert.IsDBNull(row["InspectionDetailID1"]) ? Convert.ToInt32(row["InspectionDetailID1"]) : 0;
                    dd.InspectionDetailID2 = !Convert.IsDBNull(row["InspectionDetailID2"]) ? Convert.ToInt32(row["InspectionDetailID2"]) : 0;
                    dd.InspectionDetailID3 = !Convert.IsDBNull(row["InspectionDetailID3"]) ? Convert.ToInt32(row["InspectionDetailID3"]) : 0;
                    dd.Ok1 = !Convert.IsDBNull(row["Ok1"]) ? Convert.ToBoolean(row["Ok1"]) : false;
                    dd.Ok2 = !Convert.IsDBNull(row["Ok2"]) ? Convert.ToBoolean(row["Ok2"]) : false;
                    dd.Ok3 = !Convert.IsDBNull(row["Ok3"]) ? Convert.ToBoolean(row["Ok3"]) : false;
                    dd.Attention1 = !Convert.IsDBNull(row["Attention1"]) ? Convert.ToBoolean(row["Attention1"]) : false;
                    dd.Attention2 = !Convert.IsDBNull(row["Attention2"]) ? Convert.ToBoolean(row["Attention2"]) : false;
                    dd.Attention3 = !Convert.IsDBNull(row["Attention3"]) ? Convert.ToBoolean(row["Attention3"]) : false;
                    dd.NoRun1 = !Convert.IsDBNull(row["NoRun1"]) ? Convert.ToBoolean(row["NoRun1"]) : false;
                    dd.NoRun2 = !Convert.IsDBNull(row["NoRun2"]) ? Convert.ToBoolean(row["NoRun2"]) : false;
                    dd.NoRun3 = !Convert.IsDBNull(row["NoRun3"]) ? Convert.ToBoolean(row["NoRun3"]) : false;
                    dd.Date1 = !Convert.IsDBNull(row["Date1"]) ? Convert.ToDateTime(row["Date1"]) : new DateTime();
                    dd.Date2 = !Convert.IsDBNull(row["Date2"]) ? Convert.ToDateTime(row["Date2"]) : new DateTime();
                    dd.Date3 = !Convert.IsDBNull(row["Date3"]) ? Convert.ToDateTime(row["Date3"]) : new DateTime();
                    dd.AdditionalComments1 = row["AdditionalComments1"].ToString();
                    dd.AdditionalComments2 = row["AdditionalComments2"].ToString();
                    dd.AdditionalComments3 = row["AdditionalComments3"].ToString();

                    DC1.Add(dd);
                }
            }
            catch (Exception ex)
            {

            }

            return DC1;
        }

        public string ConvertVisioToImg(string path)
        {
            var FilePath = System.IO.Path.GetDirectoryName(path);
            var Extension = System.IO.Path.GetExtension(path);
            var FileName = System.IO.Path.GetFileNameWithoutExtension(path);

            PrintVisioToTIFF(path, FilePath, Extension.Replace(".", ""));
            return FileName + ".jpg";

            //var FilePath = System.IO.Path.GetDirectoryName(path);
            //var FileName = System.IO.Path.GetFileNameWithoutExtension(path);

            //Diagram dg = new Diagram(path);
            //Aspose.Diagram.Saving.ImageSaveOptions imgopts = new Aspose.Diagram.Saving.ImageSaveOptions(SaveFileFormat.PNG);
            //imgopts.Resolution = 200;
            //dg.Save(FilePath + "\\" + FileName + ".png", imgopts);
            ////File.Delete(path);
            //return FileName + ".png";
        }

        public async void PrintVisioToTIFF(string VisioFilePath, string Filepath, string Extension)
        {

            var convertApi = new ConvertApi("J9XcdhxyrEg4hO5H");
            //var convertApi = new ConvertApi("AE7dwGYSpTJoXZwk");

            var convert = await convertApi.ConvertAsync(Extension, "png",
            new ConvertApiFileParam("File", VisioFilePath)
            );
            await convert.SaveFilesAsync(Filepath);

            //if (Extension == ".vsd")
            //{
            //    var convert = await convertApi.ConvertAsync("vsd", "png",
            //    new ConvertApiFileParam("File", VisioFilePath)
            //);
            //    await convert.SaveFilesAsync(Filepath);
            //}
            //else if (Extension == ".vsdx")
            //{
            //    var convert = await convertApi.ConvertAsync("vsdx", "png",
            //    new ConvertApiFileParam("File", VisioFilePath)
            //);
            //    await convert.SaveFilesAsync(Filepath);
            //}

        }

        public string encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }


        public string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        public int GetCompanyID()
        {
            int CID = 0;
            //var CompID = System.Web.HttpContext.Current.Session["CompanyID"].ToString();
            if (!string.IsNullOrWhiteSpace(System.Web.HttpContext.Current.Session["CompanyID"].ToString()))
            {
                CID = Convert.ToInt32(System.Web.HttpContext.Current.Session["CompanyID"].ToString());
            }
            return CID;
        }

        public int IsCalledbyOrg()
        {
            int ID = 0;

            if (!string.IsNullOrWhiteSpace(System.Web.HttpContext.Current.Session["RoleID"].ToString()))
            {
                int RoleID = Convert.ToInt32(System.Web.HttpContext.Current.Session["RoleID"]);

                if (RoleID == 1)
                {
                    ID = 1;
                }
                else
                {
                    ID = 0;
                }
            }

            return ID;
        }


        public string GetUserName()
        {
            string Name = "";
            //var CompID = System.Web.HttpContext.Current.Session["User"].ToString();
            if (!string.IsNullOrWhiteSpace(System.Web.HttpContext.Current.Session["User"].ToString()))
            {
                Name = System.Web.HttpContext.Current.Session["User"].ToString();
            }

            return Name;
        }

        public void UpdateLog(LogTable obj)
        {
            try
            {
                EzyAuditLog EA = new EzyAuditLog();
                EA.DateTime = System.DateTime.Now;
                EA.User = GetUserName();
                EA.Action = obj.Action;
                EA.DataKey = obj.DataKey.ToString();
                EA.CompanyID = GetCompanyID();
                EA.TableName = obj.TableName;
                EA.NewValue = obj.NewColVal;
                EA.OldValue = obj.OldColVal;
                EA.PageName = obj.PageName;
                EA.LabelName = obj.LabelName;

                db.EzyAuditLogs.Add(EA);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            //string sql = "select * from " + obj.TableName + " where " + obj.ColName + " = " + obj.ColName + "";
            //con.Open();
            //SqlCommand cmd = new SqlCommand(sql, con);
            //SqlDataReader RD = cmd.ExecuteReader();

            //foreach (var x in RD)
            //{

            //}
        }

        public void UpdateAuditLog(LogTable obj)
        {
            try
            {
                EzyAuditLog EA = new EzyAuditLog();
                EA.DateTime = System.DateTime.Now;
                EA.User = GetUserName();
                EA.Action = obj.Action;
                EA.DataKey = obj.DataKey.ToString();
                EA.CompanyID = GetCompanyID();
                EA.TableName = obj.TableName;

                db.EzyAuditLogs.Add(EA);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

            public List<ezy_groups> GetGroupList()
        {
            var data = db.Ezy_Groups.ToList();
            List<ezy_groups> EZ = new List<ezy_groups>();

            foreach (var x in data)
            {
                if (x.GroupName == "Admins")
                {
                    x.ID = 0;
                    EZ.Add(x);
                }
                else if (x.GroupName == "Repair Tech")
                {
                    x.ID = 1;
                    EZ.Add(x);
                }
                else if (x.GroupName == "Process Tech")
                {
                    x.ID = 2;
                    EZ.Add(x);
                }
                else if (x.GroupName == "Engineer")
                {
                    x.ID = 3;
                    EZ.Add(x);
                }
                else if (x.GroupName == "Users")
                {
                    x.ID = 4;
                    EZ.Add(x);
                }
            }

            var nd = EZ.OrderBy(x => x.ID).ToList();

            return nd;
        }

    }
}