using Moldtrax.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace Moldtrax.Controllers
{
    public class DropdownTemplateUploadController : Controller
    {
        private MoldtraxDbContext db = new MoldtraxDbContext();
        static string constring = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        SqlConnection con = new SqlConnection(constring);
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ExportState(HttpPostedFileBase postedFile)
        {
            string filePath = string.Empty;
            try
            {
                if (postedFile != null)
                {
                    string path = Server.MapPath("~/DropDownTemplate/");
                    var RandomNo = Get8Digits();

                    string FF = Path.GetFileNameWithoutExtension(postedFile.FileName);
                    filePath = FF + "_" + RandomNo;
                    string extension = Path.GetExtension(postedFile.FileName);

                    string FullFilepath = path + filePath + extension;
                    postedFile.SaveAs(FullFilepath);

                    try
                    {
                        Parse(FullFilepath);
                        return Json("Template Uploaded successfully.", JsonRequestBehavior.AllowGet);

                    }
                    catch (Exception ex)
                    {
                        return Json(ex.Message, JsonRequestBehavior.AllowGet);
                    }
                }
            }

            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);

                //return Json("Oops something went wrong.", JsonRequestBehavior.AllowGet);
            }
            return Json("Internal Error", JsonRequestBehavior.AllowGet);
        }

        public string Get8Digits()
        {
            var bytes = new byte[4];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            uint random = BitConverter.ToUInt32(bytes, 0) % 100000000;
            return String.Format("{0:D8}", random);
        }

        public DataSet Parse(string fileName)
        {
            string connectionString = string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0 Macro;HDR=YES;IMEX=1;';", fileName);

            DataSet data = new DataSet();

            foreach (var sheetName in GetExcelSheetNames(connectionString))
            {
                switch (sheetName)
                {
                    case "BaseStyleType$":
                        using (OleDbConnection con = new OleDbConnection(connectionString))
                        {
                            var dataTable = new DataTable();
                            string query = string.Format("SELECT * FROM [{0}]", sheetName);
                            con.Open();
                            OleDbDataAdapter adapter = new OleDbDataAdapter(query, con);
                            adapter.Fill(dataTable);
                            data.Tables.Add(dataTable);
                            string[] ShetN = sheetName.Split('$');
                            BaseStyleTypeInsert(dataTable, ShetN[0]);
                            con.Close();
                        }
                        break;


                    case "Department$":
                        using (OleDbConnection con = new OleDbConnection(connectionString))
                        {
                            var dataTable = new DataTable();
                            string query = string.Format("SELECT * FROM [{0}]", sheetName);
                            con.Open();
                            OleDbDataAdapter adapter = new OleDbDataAdapter(query, con);
                            adapter.Fill(dataTable);
                            data.Tables.Add(dataTable);
                            string[] ShetN = sheetName.Split('$');
                            DepartmentInsert(dataTable, ShetN[0]);
                            con.Close();
                        }
                        break;

                    case "ProductLine$":
                        using (OleDbConnection con = new OleDbConnection(connectionString))
                        {
                            var dataTable = new DataTable();
                            string query = string.Format("SELECT * FROM [{0}]", sheetName);
                            con.Open();
                            OleDbDataAdapter adapter = new OleDbDataAdapter(query, con);
                            adapter.Fill(dataTable);
                            data.Tables.Add(dataTable);
                            string[] ShetN = sheetName.Split('$');
                            ProductLineInsert(dataTable, ShetN[0]);
                            con.Close();
                        }
                        break;

                    case "ProductPart$":
                        using (OleDbConnection con = new OleDbConnection(connectionString))
                        {
                            var dataTable = new DataTable();
                            string query = string.Format("SELECT * FROM [{0}]", sheetName);
                            con.Open();
                            OleDbDataAdapter adapter = new OleDbDataAdapter(query, con);
                            adapter.Fill(dataTable);
                            data.Tables.Add(dataTable);
                            string[] ShetN = sheetName.Split('$');
                            ProductPartInsert(dataTable, ShetN[0]);
                            con.Close();
                        }
                        break;


                    case "ResinType$":
                        using (OleDbConnection con = new OleDbConnection(connectionString))
                        {
                            var dataTable = new DataTable();
                            string query = string.Format("SELECT * FROM [{0}]", sheetName);
                            con.Open();
                            OleDbDataAdapter adapter = new OleDbDataAdapter(query, con);
                            adapter.Fill(dataTable);
                            data.Tables.Add(dataTable);
                            string[] ShetN = sheetName.Split('$');
                            ResinTypeInsert(dataTable, ShetN[0]);
                            con.Close();
                        }
                        break;

                    case "RunnerType$":
                        using (OleDbConnection con = new OleDbConnection(connectionString))
                        {
                            var dataTable = new DataTable();
                            string query = string.Format("SELECT * FROM [{0}]", sheetName);
                            con.Open();
                            OleDbDataAdapter adapter = new OleDbDataAdapter(query, con);
                            adapter.Fill(dataTable);
                            data.Tables.Add(dataTable);
                            string[] ShetN = sheetName.Split('$');
                            RunnerTypeInsert(dataTable, ShetN[0]);
                            con.Close();
                        }
                        break;

                    case "MoldToolingType$":
                        using (OleDbConnection con = new OleDbConnection(connectionString))
                        {
                            var dataTable = new DataTable();
                            string query = string.Format("SELECT * FROM [{0}]", sheetName);
                            con.Open();
                            OleDbDataAdapter adapter = new OleDbDataAdapter(query, con);
                            adapter.Fill(dataTable);
                            data.Tables.Add(dataTable);
                            string[] ShetN = sheetName.Split('$');
                            MoldToolingTypeInsert(dataTable, ShetN[0]);
                            con.Close();
                        }
                        break;

                    case "TSGuideDefectType$":
                        using (OleDbConnection con = new OleDbConnection(connectionString))
                        {
                            var dataTable = new DataTable();
                            string query = string.Format("SELECT * FROM [{0}]", sheetName);
                            con.Open();
                            OleDbDataAdapter adapter = new OleDbDataAdapter(query, con);
                            adapter.Fill(dataTable);
                            data.Tables.Add(dataTable);
                            string[] ShetN = sheetName.Split('$');
                            TSGuideDefectTypeInsert(dataTable, ShetN[0]);
                            con.Close();
                        }
                        break;

                    case "TechTipsLinks$":
                        using (OleDbConnection con = new OleDbConnection(connectionString))
                        {
                            var dataTable = new DataTable();
                            string query = string.Format("SELECT * FROM [{0}]", sheetName);
                            con.Open();
                            OleDbDataAdapter adapter = new OleDbDataAdapter(query, con);
                            adapter.Fill(dataTable);
                            data.Tables.Add(dataTable);
                            string[] ShetN = sheetName.Split('$');
                            TechTipsLinksInsert(dataTable, ShetN[0]);
                            con.Close();
                        }
                        break;

                    case "Factors$":
                        using (OleDbConnection con = new OleDbConnection(connectionString))
                        {
                            var dataTable = new DataTable();
                            string query = string.Format("SELECT * FROM [{0}]", sheetName);
                            con.Open();
                            OleDbDataAdapter adapter = new OleDbDataAdapter(query, con);
                            adapter.Fill(dataTable);
                            data.Tables.Add(dataTable);
                            string[] ShetN = sheetName.Split('$');
                            FactorsInsert(dataTable, ShetN[0]);
                            con.Close();
                        }
                        break;

                    case "MoldConfig1$":
                        using (OleDbConnection con = new OleDbConnection(connectionString))
                        {
                            var dataTable = new DataTable();
                            string query = string.Format("SELECT * FROM [{0}]", sheetName);
                            con.Open();
                            OleDbDataAdapter adapter = new OleDbDataAdapter(query, con);
                            adapter.Fill(dataTable);
                            data.Tables.Add(dataTable);
                            string[] ShetN = sheetName.Split('$');
                            MoldConfig1Insert(dataTable, ShetN[0]);
                            con.Close();
                        }
                        break;

                    case "MoldConfig2$":
                        using (OleDbConnection con = new OleDbConnection(connectionString))
                        {
                            var dataTable = new DataTable();
                            string query = string.Format("SELECT * FROM [{0}]", sheetName);
                            con.Open();
                            OleDbDataAdapter adapter = new OleDbDataAdapter(query, con);
                            adapter.Fill(dataTable);
                            data.Tables.Add(dataTable);
                            string[] ShetN = sheetName.Split('$');
                            MoldConfig2Insert(dataTable, ShetN[0]);
                            con.Close();
                        }
                        break;

                    case "StopReason$":
                        using (OleDbConnection con = new OleDbConnection(connectionString))
                        {
                            var dataTable = new DataTable();
                            string query = string.Format("SELECT * FROM [{0}]", sheetName);
                            con.Open();
                            OleDbDataAdapter adapter = new OleDbDataAdapter(query, con);
                            adapter.Fill(dataTable);
                            data.Tables.Add(dataTable);
                            string[] ShetN = sheetName.Split('$');
                            StopReasonInsert(dataTable, ShetN[0]);
                            con.Close();
                        }
                        break;

                    case "CorrectiveActionType$":
                        using (OleDbConnection con = new OleDbConnection(connectionString))
                        {
                            var dataTable = new DataTable();
                            string query = string.Format("SELECT * FROM [{0}]", sheetName);
                            con.Open();
                            OleDbDataAdapter adapter = new OleDbDataAdapter(query, con);
                            adapter.Fill(dataTable);
                            data.Tables.Add(dataTable);
                            string[] ShetN = sheetName.Split('$');
                            CorrectiveActionTypeInsert(dataTable, ShetN[0]);
                            con.Close();
                        }
                        break;

                    case "CorrectiveAction$":
                        using (OleDbConnection con = new OleDbConnection(connectionString))
                        {
                            var dataTable = new DataTable();
                            string query = string.Format("SELECT * FROM [{0}]", sheetName);
                            con.Open();
                            OleDbDataAdapter adapter = new OleDbDataAdapter(query, con);
                            adapter.Fill(dataTable);
                            data.Tables.Add(dataTable);
                            string[] ShetN = sheetName.Split('$');
                            CorrectiveActionInsert(dataTable, ShetN[0]);
                            con.Close();
                        }
                        break;

                    case "MaintenanceScheduleStatus$":
                        using (OleDbConnection con = new OleDbConnection(connectionString))
                        {
                            var dataTable = new DataTable();
                            string query = string.Format("SELECT * FROM [{0}]", sheetName);
                            con.Open();
                            OleDbDataAdapter adapter = new OleDbDataAdapter(query, con);
                            adapter.Fill(dataTable);
                            data.Tables.Add(dataTable);
                            string[] ShetN = sheetName.Split('$');
                            MaintenanceScheuleStatusInsert(dataTable, ShetN[0]);
                            con.Close();
                        }
                        break;

                    case "RepairStatus$":
                        using (OleDbConnection con = new OleDbConnection(connectionString))
                        {
                            var dataTable = new DataTable();
                            string query = string.Format("SELECT * FROM [{0}]", sheetName);
                            con.Open();
                            OleDbDataAdapter adapter = new OleDbDataAdapter(query, con);
                            adapter.Fill(dataTable);
                            data.Tables.Add(dataTable);
                            string[] ShetN = sheetName.Split('$');
                            RepairStatusInsert(dataTable, ShetN[0]);
                            con.Close();
                        }
                        break;


                    case "RepairLocation$":
                        using (OleDbConnection con = new OleDbConnection(connectionString))
                        {
                            var dataTable = new DataTable();
                            //string query = string.Format("SELECT * FROM [{0}]", sheetName);
                            string query = string.Format("SELECT * FROM [{0}]", sheetName);
                            con.Open();
                            OleDbDataAdapter adapter = new OleDbDataAdapter(query, con);
                            adapter.Fill(dataTable);
                            data.Tables.Add(dataTable);
                            string[] ShetN = sheetName.Split('$');
                            RepairLocationInsert(dataTable, ShetN[0]);
                            con.Close();
                        }
                        break;

                }
            }

            return data;
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

        public static string[] GetExcelSheetNames(string connectionString)
        {
            //System.IO.File.AppendAllText(LogFilepath, connectionString + Environment.NewLine);

            OleDbConnection con = null;
            DataTable dt = null;
            con = new OleDbConnection(connectionString);
            //System.IO.File.AppendAllText(LogFilepath, "Try to open con" + Environment.NewLine);

            try
            {
                con.Open();
                //System.IO.File.AppendAllText(LogFilepath, "Con open successfully" + Environment.NewLine);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //System.IO.File.AppendAllText(LogFilepath, ex.Message + Environment.NewLine);

            }

            dt = con.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

            if (dt == null)
            {
                return null;
            }

            String[] excelSheetNames = new String[dt.Rows.Count];
            int i = 0;

            foreach (DataRow row in dt.Rows)
            {
                excelSheetNames[i] = row["TABLE_NAME"].ToString();
                i++;
            }
            con.Close();
            con.Dispose();
            return excelSheetNames;
        }


        #region Tables

        public void BaseStyleTypeInsert(DataTable dt, string sheetName)
        {
            //string constring = db.GetConnectionString();
            //SqlConnection con = new SqlConnection(constring);
            int CID = ShrdMaster.Instance.GetCompanyID();

            using (SqlConnection connection = new SqlConnection(constring))
            {
                List<tblDDMoldCategoryID> SR = new List<tblDDMoldCategoryID>();
                var data = db.TblDDMoldCategoryID.ToList();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i].ItemArray[0] != null && dt.Rows[i].ItemArray[1] != null)
                    {
                        string Desc = dt.Rows[i].ItemArray[1].ToString() == "" ? null : dt.Rows[i].ItemArray[1].ToString();

                        var dd = data.Where(x => x.MoldCategoryID == dt.Rows[i].ItemArray[0].ToString() && x.MoldCategoryIDDesc == Desc).FirstOrDefault();
                        if (dd == null)
                        {
                            tblDDMoldCategoryID StateR = new tblDDMoldCategoryID();
                            StateR.CompanyID = CID;
                            StateR.MoldCategoryID = dt.Rows[i].ItemArray[0].ToString();
                            StateR.MoldCategoryIDDesc = dt.Rows[i].ItemArray[1].ToString();
                            SR.Add(StateR);
                        }
                    }
                }


                DataTable NewDT = ToDataTable(SR);
                connection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    //foreach (DataColumn c in NewDT.Columns)
                    bulkCopy.ColumnMappings.Add("MoldCategoryID", "MoldCategoryID");
                    bulkCopy.ColumnMappings.Add("MoldCategoryIDDesc", "MoldCategoryIDDesc");
                    bulkCopy.ColumnMappings.Add("CompanyID", "CompanyID");

                    bulkCopy.DestinationTableName = "tblDDMoldCategoryID";
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

        public void DepartmentInsert(DataTable dt, string sheetName)
        {
            //string ConString = db.GetConnectionString();
            //SqlConnection con = new SqlConnection(constring);
            int CID = ShrdMaster.Instance.GetCompanyID();

            using (SqlConnection connection = new SqlConnection(constring))
            {
                var data = db.TblDDDepartmentID.ToList();
                List<tblDDDepartmentID> SR = new List<tblDDDepartmentID>();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i].ItemArray[0] != null || dt.Rows[i].ItemArray[1] != null)
                    {
                        var dd = data.Where(x => x.DepartmentID == dt.Rows[i].ItemArray[0].ToString() && x.DepartmentIDDesc == (dt.Rows[i].ItemArray[1].ToString() == "" ? null : dt.Rows[i].ItemArray[1].ToString())).FirstOrDefault();

                        if (dd == null)
                        {
                            tblDDDepartmentID StateR = new tblDDDepartmentID();
                            StateR.CompanyID = CID;
                            StateR.DepartmentID = dt.Rows[i].ItemArray[0].ToString();
                            StateR.DepartmentIDDesc = dt.Rows[i].ItemArray[1].ToString();
                            SR.Add(StateR);
                        }
                    }
                }


                DataTable NewDT = ToDataTable(SR);
                connection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    //foreach (DataColumn c in NewDT.Columns)
                    bulkCopy.ColumnMappings.Add("DepartmentID", "DepartmentID");
                    bulkCopy.ColumnMappings.Add("DepartmentIDDesc", "DepartmentIDDesc");
                    bulkCopy.ColumnMappings.Add("CompanyID", "CompanyID");

                    bulkCopy.DestinationTableName = "tblDDDepartmentID";
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

        public void ProductLineInsert(DataTable dt, string sheetName)
        {
            //string ConString = db.GetConnectionString();
            //SqlConnection con = new SqlConnection(constring);
            int CID = ShrdMaster.Instance.GetCompanyID();
            using (SqlConnection connection = new SqlConnection(constring))
            {
                List<tblDDProductLine> SR = new List<tblDDProductLine>();
                var data = db.TblDDProductLine.ToList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i].ItemArray[0] != null || dt.Rows[i].ItemArray[1] != null)
                    {
                        var dd = data.Where(x => x.ProductLine == dt.Rows[i].ItemArray[0].ToString() && x.ProductLineDesc == (dt.Rows[i].ItemArray[1].ToString() == "" ? null : dt.Rows[i].ItemArray[1].ToString())).FirstOrDefault();

                        if (dd == null)
                        {
                            tblDDProductLine StateR = new tblDDProductLine();
                            StateR.CompanyID = CID;
                            StateR.ProductLine = dt.Rows[i].ItemArray[0].ToString();
                            StateR.ProductLineDesc = dt.Rows[i].ItemArray[1].ToString();
                            SR.Add(StateR);
                        }
                    }
                }


                DataTable NewDT = ToDataTable(SR);
                connection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    //foreach (DataColumn c in NewDT.Columns)
                    bulkCopy.ColumnMappings.Add("ProductLine", "ProductLine");
                    bulkCopy.ColumnMappings.Add("ProductLineDesc", "ProductLineDesc");
                    bulkCopy.ColumnMappings.Add("CompanyID", "CompanyID");

                    bulkCopy.DestinationTableName = "TblDDProductLine";
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

        public void ProductPartInsert(DataTable dt, string sheetName)
        {
            //string ConString = db.GetConnectionString();
            //SqlConnection con = new SqlConnection(constring);
            int CID = ShrdMaster.Instance.GetCompanyID();
            using (SqlConnection connection = new SqlConnection(constring))
            {
                List<tblDDProductPart> SR = new List<tblDDProductPart>();
                var data = db.TblDDProductPart.ToList();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i].ItemArray[0] != null || dt.Rows[i].ItemArray[1] != null)
                    {

                        var dd = data.Where(x => x.ProductPart == dt.Rows[i].ItemArray[0].ToString() && x.ProductPartDesc == (dt.Rows[i].ItemArray[1].ToString() == "" ? null : dt.Rows[i].ItemArray[1].ToString())).FirstOrDefault();

                        if (dd == null)
                        {
                            tblDDProductPart StateR = new tblDDProductPart();
                            StateR.CompanyID = CID;
                            StateR.ProductPart = dt.Rows[i].ItemArray[0].ToString();
                            StateR.ProductPartDesc = dt.Rows[i].ItemArray[1].ToString();
                            SR.Add(StateR);
                        }
                    }
                }


                DataTable NewDT = ToDataTable(SR);
                connection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    //foreach (DataColumn c in NewDT.Columns)
                    bulkCopy.ColumnMappings.Add("ProductPart", "ProductPart");
                    bulkCopy.ColumnMappings.Add("ProductPartDesc", "ProductPartDesc");
                    bulkCopy.ColumnMappings.Add("CompanyID", "CompanyID");

                    bulkCopy.DestinationTableName = "TblDDProductPart";
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

        public void ResinTypeInsert(DataTable dt, string sheetName)
        {
            //string ConString = db.GetConnectionString();
            //SqlConnection con = new SqlConnection(constring);
            int CID = ShrdMaster.Instance.GetCompanyID();

            using (SqlConnection connection = new SqlConnection(constring))
            {
                List<tblDDMoldResinType> SR = new List<tblDDMoldResinType>();
                var data = db.TblDDMoldResinType.ToList();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i].ItemArray[0] != null || dt.Rows[i].ItemArray[1] != null)
                    {
                        var dd = data.Where(x => x.MoldResinType == dt.Rows[i].ItemArray[0].ToString() && x.MoldResinTypeDesc == (dt.Rows[i].ItemArray[1].ToString() == "" ? null : dt.Rows[i].ItemArray[1].ToString())).FirstOrDefault();

                        if (dd == null)
                        {
                            tblDDMoldResinType StateR = new tblDDMoldResinType();
                            StateR.CompanyID = CID;
                            StateR.MoldResinType = dt.Rows[i].ItemArray[0].ToString();
                            StateR.MoldResinTypeDesc = dt.Rows[i].ItemArray[1].ToString();
                            SR.Add(StateR);
                        }
                    }
                }

                DataTable NewDT = ToDataTable(SR);
                connection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    //foreach (DataColumn c in NewDT.Columns)
                    bulkCopy.ColumnMappings.Add("MoldResinType", "MoldResinType");
                    bulkCopy.ColumnMappings.Add("MoldResinTypeDesc", "MoldResinTypeDesc");
                    bulkCopy.ColumnMappings.Add("CompanyID", "CompanyID");

                    bulkCopy.DestinationTableName = "TblDDMoldResinType";
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

        public void RunnerTypeInsert(DataTable dt, string sheetName)
        {
            //string ConString = db.GetConnectionString();
            //SqlConnection con = new SqlConnection(constring);
            int CID = ShrdMaster.Instance.GetCompanyID();

            using (SqlConnection connection = new SqlConnection(constring))
            {
                List<tblDDMoldCav> SR = new List<tblDDMoldCav>();
                var data = db.TblDDMoldCav.ToList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i].ItemArray[0] != null || dt.Rows[i].ItemArray[1] != null)
                    {
                        var dd = data.Where(x => x.MoldCav == dt.Rows[i].ItemArray[0].ToString() && x.MoldCavDesc == (dt.Rows[i].ItemArray[1].ToString() == "" ? null : dt.Rows[i].ItemArray[1].ToString())).FirstOrDefault();

                        if (dd == null)
                        {
                            tblDDMoldCav StateR = new tblDDMoldCav();
                            StateR.CompanyID = CID;
                            StateR.MoldCav = dt.Rows[i].ItemArray[0].ToString();
                            StateR.MoldCavDesc = dt.Rows[i].ItemArray[1].ToString();
                            SR.Add(StateR);
                        }
                    }
                }

                DataTable NewDT = ToDataTable(SR);
                connection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    //foreach (DataColumn c in NewDT.Columns)
                    bulkCopy.ColumnMappings.Add("MoldCav", "MoldCav");
                    bulkCopy.ColumnMappings.Add("MoldCavDesc", "MoldCavDesc");
                    bulkCopy.ColumnMappings.Add("CompanyID", "CompanyID");

                    bulkCopy.DestinationTableName = "TblDDMoldCav";
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

        public void MoldToolingTypeInsert(DataTable dt, string sheetName)
        {
            //string ConString = db.GetConnectionString();
            //SqlConnection con = new SqlConnection(constring);
            int CID = ShrdMaster.Instance.GetCompanyID();

            using (SqlConnection connection = new SqlConnection(constring))
            {
                List<tblDDMoldToolingType> SR = new List<tblDDMoldToolingType>();
                var data = db.TblDDMoldToolingTypes.ToList();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i].ItemArray[0] != null || dt.Rows[i].ItemArray[1] != null)
                    {
                        var dd = data.Where(x => x.DD_MoldToolingType == dt.Rows[i].ItemArray[0].ToString() && x.DD_MoldToolingTypeDesc == (dt.Rows[i].ItemArray[1].ToString() == "" ? null : dt.Rows[i].ItemArray[1].ToString())).FirstOrDefault();

                        if (dd == null)
                        {
                            tblDDMoldToolingType StateR = new tblDDMoldToolingType();
                            StateR.CompanyID = CID;
                            StateR.DD_MoldToolingType = dt.Rows[i].ItemArray[0].ToString();
                            StateR.DD_MoldToolingTypeDesc = dt.Rows[i].ItemArray[1].ToString();
                            SR.Add(StateR);
                        }
                    }
                }

                DataTable NewDT = ToDataTable(SR);
                connection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    //foreach (DataColumn c in NewDT.Columns)
                    bulkCopy.ColumnMappings.Add("DD_MoldToolingType", "DD_MoldToolingType");
                    bulkCopy.ColumnMappings.Add("DD_MoldToolingTypeDesc", "DD_MoldToolingTypeDesc");
                    bulkCopy.ColumnMappings.Add("CompanyID", "CompanyID");

                    bulkCopy.DestinationTableName = "TblDDMoldToolingType";
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

        public void TSGuideDefectTypeInsert(DataTable dt, string sheetName)
        {
            //string ConString = db.GetConnectionString();
            //SqlConnection con = new SqlConnection(constring);
            int CID = ShrdMaster.Instance.GetCompanyID();

            using (SqlConnection connection = new SqlConnection(constring))
            {
                List<tblDDTSType> SR = new List<tblDDTSType>();
                var data = db.TblDDTSType.ToList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i].ItemArray[0] != null || dt.Rows[i].ItemArray[1] != null)
                    {

                        var dd = data.Where(x => x.TSType == dt.Rows[i].ItemArray[0].ToString() && x.TSTypeDesc == (dt.Rows[i].ItemArray[1].ToString() == "" ? null : dt.Rows[i].ItemArray[1].ToString())).FirstOrDefault();

                        if (dd == null)
                        {
                            tblDDTSType StateR = new tblDDTSType();
                            StateR.CompanyID = CID;
                            StateR.TSType = dt.Rows[i].ItemArray[0].ToString();
                            StateR.TSTypeDesc = dt.Rows[i].ItemArray[1].ToString();
                            SR.Add(StateR);
                        }
                    }
                }


                DataTable NewDT = ToDataTable(SR);
                connection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    //foreach (DataColumn c in NewDT.Columns)
                    bulkCopy.ColumnMappings.Add("TSType", "TSType");
                    bulkCopy.ColumnMappings.Add("TSTypeDesc", "TSTypeDesc");
                    bulkCopy.ColumnMappings.Add("CompanyID", "CompanyID");

                    bulkCopy.DestinationTableName = "TblDDTSType";
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

        public void TechTipsLinksInsert(DataTable dt, string sheetName)
        {
            //string ConString = db.GetConnectionString();
            //SqlConnection con = new SqlConnection(constring);
            int CID = ShrdMaster.Instance.GetCompanyID();

            using (SqlConnection connection = new SqlConnection(constring))
            {
                List<tblDDDocSection> SR = new List<tblDDDocSection>();
                var data = db.TblDDDocSections.ToList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i].ItemArray[0] != null || dt.Rows[i].ItemArray[1] != null)
                    {
                        var dd = data.Where(x => x.DocSection == dt.Rows[i].ItemArray[0].ToString() && x.DocSectionDesc == (dt.Rows[i].ItemArray[1].ToString() == "" ? null : dt.Rows[i].ItemArray[1].ToString())).FirstOrDefault();

                        if (dd == null)
                        {
                            tblDDDocSection StateR = new tblDDDocSection();
                            StateR.CompanyID = CID;
                            StateR.DocSection = dt.Rows[i].ItemArray[0].ToString();
                            StateR.DocSectionDesc = dt.Rows[i].ItemArray[1].ToString();
                            SR.Add(StateR);
                        }
                    }
                }

                DataTable NewDT = ToDataTable(SR);
                connection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    //foreach (DataColumn c in NewDT.Columns)
                    bulkCopy.ColumnMappings.Add("DocSection", "DocSection");
                    bulkCopy.ColumnMappings.Add("DocSectionDesc", "DocSectionDesc");
                    bulkCopy.ColumnMappings.Add("CompanyID", "CompanyID");

                    bulkCopy.DestinationTableName = "TblDDDocSection";
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

        public void FactorsInsert(DataTable dt, string sheetName)
        {
            //string ConString = db.GetConnectionString();
            //SqlConnection con = new SqlConnection(constring);
            int CID = ShrdMaster.Instance.GetCompanyID();

            using (SqlConnection connection = new SqlConnection(constring))
            {
                List<tblDDFactors> SR = new List<tblDDFactors>();
                var data = db.TblDDFactors.ToList();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i].ItemArray[0] != null || dt.Rows[i].ItemArray[1] != null || dt.Rows[i].ItemArray[2] != null || dt.Rows[i].ItemArray[3] != null || dt.Rows[i].ItemArray[4] != null || dt.Rows[i].ItemArray[5] != null)
                    {
                        var dd = data.Where(x => x.Plastic_Type == dt.Rows[i].ItemArray[0].ToString() && x.Steel_Type == dt.Rows[i].ItemArray[2].ToString() && x.Location_Type == dt.Rows[i].ItemArray[4].ToString()).FirstOrDefault();

                        if (dd == null)
                        {
                            tblDDFactors StateR = new tblDDFactors();
                            StateR.Plastic_Type = dt.Rows[i].ItemArray[0].ToString();

                            if (!(dt.Rows[i].ItemArray[1] is DBNull))
                                StateR.PF = Convert.ToDouble(dt.Rows[i].ItemArray[1]);

                            StateR.Steel_Type = dt.Rows[i].ItemArray[2].ToString();

                            if (!(dt.Rows[i].ItemArray[3] is DBNull))
                                StateR.PF = Convert.ToDouble(dt.Rows[i].ItemArray[3]);

                            StateR.Location_Type = dt.Rows[i].ItemArray[4].ToString();

                            if (!(dt.Rows[i].ItemArray[5] is DBNull))
                                StateR.PF = Convert.ToDouble(dt.Rows[i].ItemArray[5]);

                            StateR.CompanyID = CID;

                            SR.Add(StateR);
                        }
                    }
                }

                DataTable NewDT = ToDataTable(SR);
                connection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    //foreach (DataColumn c in NewDT.Columns)
                    bulkCopy.ColumnMappings.Add("Plastic_Type", "Plastic_Type");
                    bulkCopy.ColumnMappings.Add("PF", "PF");
                    bulkCopy.ColumnMappings.Add("Steel_Type", "Steel_Type");
                    bulkCopy.ColumnMappings.Add("SF", "SF");
                    bulkCopy.ColumnMappings.Add("Location_Type", "Location_Type");
                    bulkCopy.ColumnMappings.Add("LF", "LF");
                    bulkCopy.ColumnMappings.Add("CompanyID", "CompanyID");

                    bulkCopy.DestinationTableName = "tblDDFactors";
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


        public void MoldConfig1Insert(DataTable dt, string sheetName)
        {
            //string ConString = db.GetConnectionString();
            //SqlConnection con = new SqlConnection(constring);

            int CID = ShrdMaster.Instance.GetCompanyID();

            using (SqlConnection connection = new SqlConnection(constring))
            {
                List<tblDDMoldConfig> SR = new List<tblDDMoldConfig>();
                var data = db.TblDDMoldConfigs.ToList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i].ItemArray[0] != null || dt.Rows[i].ItemArray[1] != null)
                    {

                        var dd = data.Where(x => x.MoldConfig == dt.Rows[i].ItemArray[0].ToString() && x.MoldConfigDesc == (dt.Rows[i].ItemArray[1].ToString() == "" ? null : dt.Rows[i].ItemArray[1].ToString())).FirstOrDefault();
                        if (dd == null)
                        {
                            tblDDMoldConfig StateR = new tblDDMoldConfig();
                            StateR.CompanyID = CID;
                            StateR.MoldConfig = dt.Rows[i].ItemArray[0].ToString();
                            StateR.MoldConfigDesc = dt.Rows[i].ItemArray[1].ToString();
                            SR.Add(StateR);
                        }
                    }
                }

                DataTable NewDT = ToDataTable(SR);
                connection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    //foreach (DataColumn c in NewDT.Columns)
                    bulkCopy.ColumnMappings.Add("MoldConfig", "MoldConfig");
                    bulkCopy.ColumnMappings.Add("MoldConfigDesc", "MoldConfigDesc");
                    bulkCopy.ColumnMappings.Add("CompanyID", "CompanyID");

                    bulkCopy.DestinationTableName = "TblDDMoldConfig";
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

        public void MoldConfig2Insert(DataTable dt, string sheetName)
        {
            //string ConString = db.GetConnectionString();
            //SqlConnection con = new SqlConnection(constring);

            int CID = ShrdMaster.Instance.GetCompanyID();

            using (SqlConnection connection = new SqlConnection(constring))
            {
                List<tblDDMoldConfig2> SR = new List<tblDDMoldConfig2>();
                var data = db.TblDDMoldConfig2s.ToList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i].ItemArray[0] != null || dt.Rows[i].ItemArray[1] != null)
                    {
                        var dd = data.Where(x => x.MoldConfig == dt.Rows[i].ItemArray[0].ToString() && x.MoldConfigDesc == (dt.Rows[i].ItemArray[1].ToString() == "" ? null : dt.Rows[i].ItemArray[1].ToString())).FirstOrDefault();

                        if (dd == null)
                        {
                            tblDDMoldConfig2 StateR = new tblDDMoldConfig2();
                            StateR.CompanyID = CID;
                            StateR.MoldConfig = dt.Rows[i].ItemArray[0].ToString();
                            StateR.MoldConfigDesc = dt.Rows[i].ItemArray[1].ToString();
                            SR.Add(StateR);
                        }
                    }
                }

                DataTable NewDT = ToDataTable(SR);
                connection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    //foreach (DataColumn c in NewDT.Columns)
                    bulkCopy.ColumnMappings.Add("MoldConfig", "MoldConfig");
                    bulkCopy.ColumnMappings.Add("MoldConfigDesc", "MoldConfigDesc");
                    bulkCopy.ColumnMappings.Add("CompanyID", "CompanyID");

                    bulkCopy.DestinationTableName = "TblDDMoldConfig2";
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

        public void StopReasonInsert(DataTable dt, string sheetName)
        {
            //string ConString = db.GetConnectionString();
            //SqlConnection con = new SqlConnection(constring);
            int CID = ShrdMaster.Instance.GetCompanyID();

            using (SqlConnection connection = new SqlConnection(constring))
            {
                List<tblddStopReason> SR = new List<tblddStopReason>();
                var data = db.TblddStopReasons.ToList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i].ItemArray[0] != null || dt.Rows[i].ItemArray[1] != null)
                    {
                        var dd = data.Where(x => x.StopReason == dt.Rows[i].ItemArray[0].ToString() && x.StopReasonDesc == (dt.Rows[i].ItemArray[1].ToString() == "" ? null : dt.Rows[i].ItemArray[1].ToString())).FirstOrDefault();
                        if (dd == null)
                        {
                            tblddStopReason StateR = new tblddStopReason();
                            StateR.CompanyID = CID;
                            StateR.StopReason = dt.Rows[i].ItemArray[0].ToString();
                            StateR.StopReasonDesc = dt.Rows[i].ItemArray[1].ToString();
                            SR.Add(StateR);
                        }
                    }
                }

                DataTable NewDT = ToDataTable(SR);
                connection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    //foreach (DataColumn c in NewDT.Columns)
                    bulkCopy.ColumnMappings.Add("StopReason", "StopReason");
                    bulkCopy.ColumnMappings.Add("StopReasonDesc", "StopReasonDesc");
                    bulkCopy.ColumnMappings.Add("CompanyID", "CompanyID");

                    bulkCopy.DestinationTableName = "TblddStopReason";
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

        public void CorrectiveActionTypeInsert(DataTable dt, string sheetName)
        {
            //string ConString = db.GetConnectionString();
            //SqlConnection con = new SqlConnection(constring);
            int CID = ShrdMaster.Instance.GetCompanyID();

            using (SqlConnection connection = new SqlConnection(constring))
            {
                List<tblDDTIType> SR = new List<tblDDTIType>();
                var data = db.TblDDTITypes.ToList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i].ItemArray[0] != null || dt.Rows[i].ItemArray[1] != null)
                    {
                        var dd = data.Where(x => x.TIType == dt.Rows[i].ItemArray[0].ToString() && x.TITypeDesc == (dt.Rows[i].ItemArray[1].ToString() == "" ? null : dt.Rows[i].ItemArray[1].ToString())).FirstOrDefault();

                        if (dd == null)
                        {
                            tblDDTIType StateR = new tblDDTIType();
                            StateR.CompanyID = CID;
                            StateR.TIType = dt.Rows[i].ItemArray[0].ToString();
                            StateR.TITypeDesc = dt.Rows[i].ItemArray[1].ToString();
                            SR.Add(StateR);
                        }
                    }
                }

                DataTable NewDT = ToDataTable(SR);
                connection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    //foreach (DataColumn c in NewDT.Columns)
                    bulkCopy.ColumnMappings.Add("TIType", "TIType");
                    bulkCopy.ColumnMappings.Add("TITypeDesc", "TITypeDesc");
                    bulkCopy.ColumnMappings.Add("CompanyID", "CompanyID");

                    bulkCopy.DestinationTableName = "TblDDTIType";
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

        public void CorrectiveActionInsert(DataTable dt, string sheetName)
        {
            //string ConString = db.GetConnectionString();
            //SqlConnection con = new SqlConnection(constring);
            int CID = ShrdMaster.Instance.GetCompanyID();

            using (SqlConnection connection = new SqlConnection(constring))
            {
                List<tblDDTlCorrectiveAction> SR = new List<tblDDTlCorrectiveAction>();
                var data = db.TblDDTlCorrectiveActions.ToList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i].ItemArray[0] != null || dt.Rows[i].ItemArray[1] != null)
                    {
                        var dd = data.Where(x => x.TlCorrectiveAction == dt.Rows[i].ItemArray[0].ToString() && x.TlCorrectiveActionDesc == (dt.Rows[i].ItemArray[1].ToString() == "" ? null : dt.Rows[i].ItemArray[1].ToString())).FirstOrDefault();

                        if (dd == null)
                        {
                            tblDDTlCorrectiveAction StateR = new tblDDTlCorrectiveAction();
                            StateR.CompanyID = CID;
                            StateR.TlCorrectiveAction = dt.Rows[i].ItemArray[0].ToString();
                            StateR.TlCorrectiveActionDesc = dt.Rows[i].ItemArray[1].ToString();
                            SR.Add(StateR);
                        }
                    }
                }

                DataTable NewDT = ToDataTable(SR);
                connection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    //foreach (DataColumn c in NewDT.Columns)
                    bulkCopy.ColumnMappings.Add("TlCorrectiveAction", "TlCorrectiveAction");
                    bulkCopy.ColumnMappings.Add("TlCorrectiveActionDesc", "TlCorrectiveActionDesc");
                    bulkCopy.ColumnMappings.Add("CompanyID", "CompanyID");

                    bulkCopy.DestinationTableName = "TblDDTlCorrectiveAction";
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

        public void MaintenanceScheuleStatusInsert(DataTable dt, string sheetName)
        {
            //string ConString = db.GetConnectionString();
            //SqlConnection con = new SqlConnection(constring);
            int CID = ShrdMaster.Instance.GetCompanyID();

            using (SqlConnection connection = new SqlConnection(constring))
            {
                List<tblDDschStatus> SR = new List<tblDDschStatus>();
                var data = db.TblDDschStatuses.ToList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i].ItemArray[0] != null || dt.Rows[i].ItemArray[1] != null)
                    {
                        var dd = data.Where(x => x.schStatus == dt.Rows[i].ItemArray[0].ToString() && x.schStatusDesc == (dt.Rows[i].ItemArray[1].ToString() == "" ? null : dt.Rows[i].ItemArray[1].ToString())).FirstOrDefault();

                        if (dd == null)
                        {
                            tblDDschStatus StateR = new tblDDschStatus();
                            StateR.CompanyID = CID;
                            StateR.schStatus = dt.Rows[i].ItemArray[0].ToString();
                            StateR.schStatusDesc = dt.Rows[i].ItemArray[1].ToString();
                            SR.Add(StateR);
                        }
                    }
                }

                DataTable NewDT = ToDataTable(SR);
                connection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    //foreach (DataColumn c in NewDT.Columns)
                    bulkCopy.ColumnMappings.Add("schStatus", "schStatus");
                    bulkCopy.ColumnMappings.Add("schStatusDesc", "schStatusDesc");
                    bulkCopy.ColumnMappings.Add("CompanyID", "CompanyID");

                    bulkCopy.DestinationTableName = "TblDDschStatus";
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

        public void RepairStatusInsert(DataTable dt, string sheetName)
        {
            //string ConString = db.GetConnectionString();
            //SqlConnection con = new SqlConnection(constring);
            int CID = ShrdMaster.Instance.GetCompanyID();

            using (SqlConnection connection = new SqlConnection(constring))
            {
                List<tblDDRepairStatus> SR = new List<tblDDRepairStatus>();
                var data = db.TblDDRepairStatuses.ToList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i].ItemArray[0] != null || dt.Rows[i].ItemArray[1] != null)
                    {
                        var dd = data.Where(x => x.RepairStatus == dt.Rows[i].ItemArray[0].ToString() && x.RepairStatusDescrip == (dt.Rows[i].ItemArray[1].ToString() == "" ? null : dt.Rows[i].ItemArray[1].ToString())).FirstOrDefault();
                        if (dd == null)
                        {
                            tblDDRepairStatus StateR = new tblDDRepairStatus();
                            StateR.CompanyID = CID;
                            StateR.RepairStatus = dt.Rows[i].ItemArray[0].ToString();
                            StateR.RepairStatusDescrip = dt.Rows[i].ItemArray[1].ToString();
                            SR.Add(StateR);
                        }
                    }
                }

                DataTable NewDT = ToDataTable(SR);
                connection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    //foreach (DataColumn c in NewDT.Columns)
                    bulkCopy.ColumnMappings.Add("RepairStatus", "RepairStatus");
                    bulkCopy.ColumnMappings.Add("RepairStatusDescrip", "RepairStatusDescrip");
                    bulkCopy.ColumnMappings.Add("CompanyID", "CompanyID");

                    bulkCopy.DestinationTableName = "TblDDRepairStatus";
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

        public void RepairLocationInsert(DataTable dt, string sheetName)
        {
            //string ConString = db.GetConnectionString();
            //SqlConnection con = new SqlConnection(constring);
            int CID = ShrdMaster.Instance.GetCompanyID();

            using (SqlConnection connection = new SqlConnection(constring))
            {
                List<tblDDRepairStatusLocation> SR = new List<tblDDRepairStatusLocation>();
                var data = db.TblDDRepairStatusLocations.ToList();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i].ItemArray[0] != null || dt.Rows[i].ItemArray[1] != null)
                    {
                        var dd = data.Where(x => x.RepairStatusLocation == dt.Rows[i].ItemArray[0].ToString() && x.RepairStatusLocationDescrip == (dt.Rows[i].ItemArray[1].ToString() == "" ? null : dt.Rows[i].ItemArray[1].ToString())).FirstOrDefault();
                        if (dd == null)
                        {
                            tblDDRepairStatusLocation StateR = new tblDDRepairStatusLocation();
                            StateR.CompanyID = CID;
                            StateR.RepairStatusLocation = dt.Rows[i].ItemArray[0].ToString();
                            StateR.RepairStatusLocationDescrip = dt.Rows[i].ItemArray[1].ToString();
                            SR.Add(StateR);
                        }
                    }
                }

                DataTable NewDT = ToDataTable(SR);
                connection.Open();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    //foreach (DataColumn c in NewDT.Columns)
                    bulkCopy.ColumnMappings.Add("RepairStatusLocation", "RepairStatusLocation");
                    bulkCopy.ColumnMappings.Add("RepairStatusLocationDescrip", "RepairStatusLocationDescrip");
                    bulkCopy.ColumnMappings.Add("CompanyID", "CompanyID");

                    bulkCopy.DestinationTableName = "TblDDRepairStatusLocation";
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

    }
}