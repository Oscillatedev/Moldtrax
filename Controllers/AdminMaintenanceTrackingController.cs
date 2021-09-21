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
    public class AdminMaintenanceTrackingController : Controller
    {
        private MoldtraxDbContext db = new MoldtraxDbContext();
        // GET: AdminMaintenanceTracking
        public ActionResult Index()
        {
            return View();
        }

        #region Mold Configuration

        public ActionResult MoldConfig()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblDDMoldConfigs.Where(X=>X.CompanyID == CID).ToList();
            return PartialView("_MoldConfig", data);

        }

        #endregion
    }
}