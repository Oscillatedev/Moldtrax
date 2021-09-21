using Moldtrax.Models;
using Moldtrax.Providers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Moldtrax.Controllers
{
    [SessionExpireFilter]
    public class TroubleShooterGuideController : Controller
    {
        private MoldtraxDbContext db = new MoldtraxDbContext();
        // GET: TroubleShooterGuide
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetData(int ID=0)
        {
            CommonFunc();
            var data = db.TblTSGuide.Where(x => x.MoldDataID == ID).ToList();
            return PartialView("_TroubleShooterGetData", data);
            //
        }

        public void CommonFunc()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            ViewBag.NoType = new SelectList(db.TblDDTSType.Where(X=> X.CompanyID == CID).ToList().OrderBy(x => x.TSType).ThenBy(x => x.TSType), "ID", "TSType");
        }

        public ActionResult Create()
        {
            CommonFunc();
            return PartialView("_CreateTroubleshooter");
        }

        //[HttpPost]
        //public ActionResult Create(tblTSGuide model, HttpPostedFileBase AddImg)
        //{
        //    if (model != null)
        //    {
        //        tblTSGuide tbs = new tblTSGuide();
        //            tbs.TSType = model.TSType;
        //            tbs.TSDefects = model.TSDefects;
        //            tbs.TSExplanation = model.TSExplanation;
        //            tbs.TSProbCause = model.TSProbCause;
        //            tbs.TSSolution = model.TSSolution;
        //            tbs.TSPreventAction = model.TSPreventAction;
        //            tbs.MoldDataID = model.MoldDataID;
        //        string TroubleShooterPath = Server.MapPath("~/TroubleShooterImage/");
        //        if (AddImg != null)
        //        {         
        //            string FName = Path.GetFileName(AddImg.FileName);
        //            string NewFName = tbs.MoldDataID + "_" + FName;
        //            string FilePath = TroubleShooterPath + NewFName;
        //            AddImg.SaveAs(FilePath);
        //            tbs.ImagePath = NewFName;
        //        }

        //        db.TblTSGuide.Add(tbs);
        //        db.SaveChanges();
        //    }

        //    return RedirectToAction("Index", "DetailMoldInfo");

        //}

        [HttpPost]
        public ActionResult Create(tblTSGuide model)
        {
            // Get the complete folder path and store the file inside it.  
            tblTSGuide tbs = new tblTSGuide();

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

                string NewFName = tbs.MoldDataID + "_" + fname;
                var FileExtension = Path.GetExtension(fname);
                //fname = Path.Combine(Server.MapPath("~/TroubleShooterImage/"), fname);

                //file.SaveAs(fname);
                fname = Path.Combine(Server.MapPath("~/TroubleShooterImage/"), NewFName);
                file.SaveAs(fname);


                if (FileExtension == ".vsdx" || FileExtension == ".vsd")
                {
                    var FileP = ShrdMaster.Instance.ConvertVisioToImg(fname);
                    tbs.ImagePath = FileP;
                }
                else
                {
                    tbs.ImagePath = ImgName;
                }
                //tbs.ImagePath = NewFName;
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

            ViewBag.NoType = new SelectList(db.TblDDTSType.ToList(), "ID", "TSType");
            var dd = db.TblMoldData.ToList().Select(x => new tblMoldData { MoldDataID = x.MoldDataID, MoldName = x.MoldName + ": " + x.MoldDesc });
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
            var Trouble = db.TblTSGuide.Where(x => x.MoldDataID == model.MoldDataID).ToList();
            List<tblTSGuide> TroubleDD = new List<tblTSGuide>();

            foreach (var x in Trouble)
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
                TSG.TSPreventAction = x.TSPreventAction;
                TSG.ImageExtension = x.ImageExtension;
                TSG.fileName = x.fileName;
                TSG.ImagePath = Request.Url.Scheme + "://" + Request.Url.Authority + Url.Content("~/TroubleShooterImage/" + x.ImagePath);
                TroubleDD.Add(TSG);
            }
            return PartialView("_TroubleShooterGetData", TroubleDD);
        }
    }
}