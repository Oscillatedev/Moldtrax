using Moldtrax.Models;
using Moldtrax.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Moldtrax.Controllers
{
    [SessionExpireFilter]
    public class AuditLogController : Controller
    {
        private MoldtraxDbContext db = new MoldtraxDbContext();
        // GET: AuditLog
        public ActionResult Index()
        {
            var CID = ShrdMaster.Instance.GetCompanyID();
            var Companylist = db.TblCompanies.ToList();
            ViewBag.Companylist = new SelectList(Companylist.ToList().OrderBy(x => x.CompanyName), "CompanyID", "CompanyName");
            var data = db.EzyAuditLogs.Where(x => x.CompanyID == CID).OrderByDescending(x => x.ID).ToList().Select(x => new EzyAuditLog
            {
                ID = x.ID,
                DateTime = x.DateTime,
                User = x.User,
                Action = x.Action,
                DataKey = x.DataKey,
                PageName = x.PageName,
                CompanyID = x.CompanyID,
                CompanyName = Companylist.Where(c=> c.CompanyID == x.CompanyID).Select(c=> c.CompanyName).FirstOrDefault(),
                OldValue = x.OldValue,
                NewValue = x.NewValue,
                TableName = x.TableName,
                LabelName = x.LabelName
            }).ToList();

            return View(data);
        }
    }
}