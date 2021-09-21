using Moldtrax.Models;
using Moldtrax.ViewMoldel;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using System.IO;
using Moldtrax.Providers;

namespace Moldtrax.Controllers
{
    [SessionExpireFilter]
    public class MasterScheduleController : Controller
    {
        private MoldtraxDbContext db = new MoldtraxDbContext();
        // GET: MasterSchedule
        public ActionResult Index(int CID=0)
        {
            if (CID == 0)
            {
                CID = ShrdMaster.Instance.GetCompanyID();
            }

            ViewBag.Companylist = new SelectList(db.TblCompanies.ToList().OrderBy(x => x.CompanyName), "CompanyID", "CompanyName");
            return View();
        }

        public ActionResult GetData()
        {
            DropDownCall();
            int CID = ShrdMaster.Instance.GetCompanyID();

            var data = db.TblSchedules.Where(x=> x.CompanyID == CID).ToList().Select(x => new tblScheduleViewModel
            {
                SchID = x.SchID,
                schMoldDataID = x.schMoldDataID,
                MoldDataID = x.MoldDataID,
                MoldName = db.TblMoldData.Where(s => s.MoldDataID == x.schMoldDataID).Select(s => s.MoldName + ": " + s.MoldDesc).FirstOrDefault(),
                schPriority = x.schPriority,
                schActionItem = x.schActionItem,
                schDate = x.schDate,
                NewSchDate = Convert.ToDateTime(x.schDate).ToShortDateString(),
                NewSchTime = Convert.ToDateTime(x.schTime).ToString("HH:mm"),
                NewSchCycles = string.Format("{0:n0}", x.schCycles),
                schStatus = x.schStatus
            }).OrderBy(x => x.schDate).ThenBy(x => x.MoldName).ToList();

            return PartialView("_MasterScheduleData", data);
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

        public FileResult ExportMasterSchedule(List<MasterScheduleViewModel> model)
        {
            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[7] { new DataColumn("Date Noted"),
                                            new DataColumn("Time"),
                                            new DataColumn("Mold"),
                                            new DataColumn("Priority"),
                                            new DataColumn("Action Item"),
                                            new DataColumn("Cycles"),
            new DataColumn("Status")});

            foreach (var x in model)
            {
                dt.Rows.Add(x.DateNoted, x.Time, x.Mold, x.Priority, x.ActionItem, x.Cycles, x.Status);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Master Schedule.xlsx");
                }
            }
        }


        [HttpPost]
        public FileResult Export()
        {
            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[7] { new DataColumn("Date Noted"),
                                            new DataColumn("Time"),
                                            new DataColumn("Mold"),
                                            new DataColumn("Priority"),
                                            new DataColumn("Action Item"),
                                            new DataColumn("Cycles"),
            new DataColumn("Status")});

            int CID = ShrdMaster.Instance.GetCompanyID();

            var SchStatus = db.TblDDschStatuses.Where(x=> x.CompanyID == CID).ToList();

            var tblsch = db.TblSchedules.Where(x => x.CompanyID == CID).ToList();

            List<tblScheduleViewModel> TSV = new List<tblScheduleViewModel>();

            foreach (var x in tblsch)
            {
                tblScheduleViewModel ts = new tblScheduleViewModel();
                ts.SchID = x.SchID;
                ts.schMoldDataID = x.schMoldDataID;
                ts.MoldDataID = x.MoldDataID;
                ts.MoldName = db.TblMoldData.Where(s => s.MoldDataID == x.schMoldDataID && x.CompanyID == CID).Select(s => s.MoldName + ": " + s.MoldDesc).FirstOrDefault();
                ts.schPriority = x.schPriority;
                ts.schActionItem = x.schActionItem != null ? HtmlToPlainText(x.schActionItem) : null;
                ts.schDate = x.schDate;
                ts.NewSchDate = Convert.ToDateTime(x.schDate).ToShortDateString();
                ts.NewSchTime = Convert.ToDateTime(x.schTime).ToString("HH:mm");
                ts.NewSchCycles = string.Format("{0:n0}", x.schCycles);

                var sa = SchStatus.Where(s => s.ID == Convert.ToInt32(x.schStatus == null ? "0" : x.schStatus)).FirstOrDefault();

                ts.schStatus = sa == null ? "" : sa.schStatus;
                TSV.Add(ts);
            }

            //var data = tblsch.Select(x => new tblScheduleViewModel
            //{
            //    SchID = x.SchID,
            //    schMoldDataID = x.schMoldDataID,
            //    MoldDataID = x.MoldDataID,
            //    MoldName = db.TblMoldData.Where(s => s.MoldDataID == x.schMoldDataID && x.CompanyID == CID).Select(s => s.MoldName + ": " + s.MoldDesc).FirstOrDefault(),
            //    schPriority = x.schPriority,
            //    schActionItem = x.schActionItem != null ? HtmlToPlainText(x.schActionItem) : null,
            //    schDate = x.schDate,
            //    NewSchDate = Convert.ToDateTime(x.schDate).ToShortDateString(),
            //    NewSchTime = Convert.ToDateTime(x.schTime).ToString("HH:mm"),
            //    NewSchCycles = string.Format("{0:n0}", x.schCycles),
            //    schStatus = x.schStatus != null ? SchStatus.Where(s => s.ID == Convert.ToInt32(x.schStatus)).FirstOrDefault().schStatus : ""
            //}).OrderBy(x => x.schDate).ThenBy(x => x.MoldName).ToList();

            foreach (var x in TSV.OrderBy(x => x.schDate).ThenBy(x => x.MoldName).ToList())
            {
                dt.Rows.Add(Convert.ToDateTime(x.schDate).ToShortDateString(), x.NewSchTime, x.MoldName, x.schPriority, x.schActionItem, x.NewSchCycles, x.schStatus);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Master Schedule.xlsx");
                }
            }
        }
   

        public ActionResult OrderbyDesc()
        {
            DropDownCall();
            var data = db.TblSchedules.ToList().Select(x => new tblScheduleViewModel
            {
                SchID = x.SchID,
                schMoldDataID = x.schMoldDataID,
                MoldDataID = x.MoldDataID,
                MoldName = db.TblMoldData.Where(s => s.MoldDataID == x.schMoldDataID).Select(s => s.MoldName + ": " + s.MoldDesc).FirstOrDefault(),
                schPriority = x.schPriority,
                schActionItem = x.schActionItem,
                schDate = x.schDate,
                NewSchDate = Convert.ToDateTime(x.schDate).ToShortDateString(),
                NewSchTime = Convert.ToDateTime(x.schTime).ToString("HH:mm"),
                NewSchCycles = string.Format("{0:n0}", x.schCycles),
                schStatus = x.schStatus
            }).OrderByDescending(x => x.schDate).ToList();

            return PartialView("_MasterScheduleData", data);
        }


        public ActionResult OrderbyAsc()
        {
            DropDownCall();
            var data = db.TblSchedules.ToList().Select(x => new tblScheduleViewModel
            {
                SchID = x.SchID,
                schMoldDataID = x.schMoldDataID,
                MoldDataID = x.MoldDataID,
                MoldName = db.TblMoldData.Where(s => s.MoldDataID == x.schMoldDataID).Select(s => s.MoldName + ": " + s.MoldDesc).FirstOrDefault(),
                schPriority = x.schPriority,
                schActionItem = x.schActionItem,
                schDate = x.schDate,
                NewSchDate = Convert.ToDateTime(x.schDate).ToShortDateString(),
                NewSchTime = Convert.ToDateTime(x.schTime).ToString("HH:mm"),
                NewSchCycles = string.Format("{0:n0}", x.schCycles),
                schStatus = x.schStatus
            }).OrderBy(x => x.schDate).ToList();

            return PartialView("_MasterScheduleData", data);
        }


        public void DropDownCall()
        {
            var dd = db.TblMoldData.ToList().Select(x => new tblMoldData { MoldDataID = x.MoldDataID, MoldName = x.MoldName + ": " + x.MoldDesc });
            List<SelectListItem> Defect1 = new List<SelectListItem>();
            foreach (var x in dd.OrderBy(x=> x.MoldName))
            {
                Defect1.Add(new SelectListItem
                {
                    Text = x.MoldName,
                    Value = x.MoldDataID.ToString()
                });
            }
            int CID = ShrdMaster.Instance.GetCompanyID();
            ViewBag.MoldText = Defect1;
            var data = db.Database.SqlQuery<SchStatusDropDown>("procSchStatusDropdown @CompanyID", new SqlParameter("@CompanyID", CID)).ToList();

            List<SelectListItem> Status = new List<SelectListItem>();
            foreach (var x in data)
            {
                Status.Add(new SelectListItem
                {
                    Text = x.schStatus,
                    Value = x.ID.ToString()
                });
            }

            ViewBag.StatusVal = Status;
        }


        public ActionResult SaveTroubleTracking(tblSchedule model)
        {
            
                    try
                    {
                    if (model.schTime != null)
                    {
                        model.schTime = new DateTime(1899, 12, 30) + model.schTime.Value.TimeOfDay;
                    }

                model.CompanyID = ShrdMaster.Instance.GetCompanyID();

                    db.TblSchedules.Add(model);
                    db.SaveChanges();

                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MasterSchedule.ToString(), GetTabName.MasterSchedule.ToString(), GetAction.Create.ToString());
            }

            catch (Exception EX)
                    {

                    }

                
            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        public void SaveFocusOutTroubleTracking(tblSchedule model)
        {
            var data = db.TblSchedules.Where(x => x.SchID == model.SchID).FirstOrDefault();

            if (data != null)
            {
                if (model.schTime != null)
                {
                    data.schTime = new DateTime(1899, 12, 30) + model.schTime.Value.TimeOfDay;
                }
                data.schMoldDataID = model.schMoldDataID;
                data.MoldDataID = model.MoldDataID;
                data.schPriority = model.schPriority;
                data.schActionItem = model.schActionItem;
                data.schDate = model.schDate;
                data.schTime = model.schTime;
                data.schCycles = model.schCycles;
                data.schStatus = model.schStatus;
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MasterSchedule.ToString(), GetTabName.MasterSchedule.ToString(), GetAction.Update.ToString());

            }
        }

        public ActionResult DeleteSubMainData(string str)
        {
            if (str != "")
            {

                str = str.Substring(0, str.LastIndexOf(','));
                using (var ctx = new MoldtraxDbContext())
                {
                    SqlParameter sp = new SqlParameter("@value", str);
                    var result = ctx.Database.ExecuteSqlCommand("exec procDeleteMaintenanceTrackingSubForm @value", sp);
                }
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.MasterSchedule.ToString(), GetTabName.MasterSchedule.ToString(), GetAction.Delete.ToString());

            }

            var data = db.TblSchedules.ToList();
            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GeneratePDF()
        {
            using (MoldtraxDbContext db = new MoldtraxDbContext())
            {
                DropDownCall();
                var data = db.TblSchedules.ToList();


                List<tblScheduleViewModel> tblsList = new List<tblScheduleViewModel>();
                foreach (var x in data)
                {
                    tblScheduleViewModel tbls = new tblScheduleViewModel();
                    tbls.SchID = x.SchID;
                    tbls.schMoldDataID = x.schMoldDataID;
                    tbls.MoldDataID = x.MoldDataID;
                    tbls.schPriority = x.schPriority;
                    tbls.schActionItem = x.schActionItem;
                    tbls.NewSchActionItem = x.schActionItem != null ? Regex.Replace(x.schActionItem, "<[^>]*>", string.Empty) : "";
                    tbls.schDate = x.schDate;
                    tbls.schTime = x.schTime;
                    tbls.schCycles = x.schCycles;
                    tbls.schStatus = x.schStatus;
                    tblsList.Add(tbls);
                }

                var report = new PartialViewAsPdf("_MasterPDFPage", tblsList);
                return report;
            }
        }
    }
}