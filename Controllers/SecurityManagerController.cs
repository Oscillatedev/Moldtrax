using Moldtrax.Models;
using Moldtrax.Providers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Moldtrax.Controllers
{
    [SessionExpireFilter]
    public class SecurityManagerController : Controller
    {
        private MoldtraxDbContext db = new MoldtraxDbContext();
        //private MoldtraxMasterDbContext MDB = new MoldtraxMasterDbContext();

        // GET: SecurityManager
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult CallByCompnay()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.Ezy_Users.Where(x => x.CompanyID == CID).ToList();
            var MoldData = RenderRazorViewToString(this.ControllerContext, "_SecurityManagerUserGetList", data);

            ViewBag.MemberOf = new SelectList(new List<ezy_groupuser>(), "GroupName", "GroupName");
            ViewBag.GrpUser = new SelectList(new List<ezy_groups>(), "GroupName", "GroupName");
            var Tblgroupview = RenderRazorViewToString(this.ControllerContext, "_GroupsandMembers",null); 

            return Json(new { MoldData, Tblgroupview }, JsonRequestBehavior.AllowGet);
        }


        public static string RenderRazorViewToString(ControllerContext controllerContext, string viewName, object model)
        {
            controllerContext.Controller.ViewData.Model = model;

            using (var stringWriter = new StringWriter())
            {
                try
                {
                    var viewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
                    var viewContext = new ViewContext(controllerContext, viewResult.View, controllerContext.Controller.ViewData, controllerContext.Controller.TempData, stringWriter);
                    viewResult.View.Render(viewContext, stringWriter);
                    viewResult.ViewEngine.ReleaseView(controllerContext, viewResult.View);
                }
                catch (Exception ex)
                {

                }
                return stringWriter.GetStringBuilder().ToString();
            }
        }




        public ActionResult GetUserData()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            //MainEzyUser MEU = new MainEzyUser();
            //MEU.GetEzyUserlist = db.Ezy_Users.ToList();
            var data = db.Ezy_Users.Where(x=> x.CompanyID == CID).ToList();
            return PartialView("_SecurityManagerUserGetList", data);
        }

        public ActionResult GetUserList(int CID=0)
        {
            var data = db.Ezy_Users.Where(x => x.CompanyID == CID).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddUser(ezy_Users obj)
        {
            if (obj != null)
            {
                //var User = MDB.Users.Where(x => x.UserID == model.UserID && x.Password == model.Password).FirstOrDefault();
                //if (User != null)
                //{
                //    ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString = User.Database;
                //}

                var ddt = db.Ezy_Users.Where(x => x.UserID == obj.UserID).FirstOrDefault();

                if (ddt == null)
                {
                    try
                    {
                        obj.CompanyID = ShrdMaster.Instance.GetCompanyID();
                        obj.Password = ShrdMaster.Instance.encrypt(obj.Password);
                        obj.RoleID = 2;
                        obj.DateTimeStamp = System.DateTime.Now;
                        db.Ezy_Users.Add(obj);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {

                    }
                }

                else
                {
                    ddt.DateTimeStamp = System.DateTime.Now;
                    ddt.UserID = obj.UserID;
                    ddt.FullName = obj.FullName;
                    ddt.Password = obj.Password;
                    db.SaveChanges();
                }
            }

            int CID = ShrdMaster.Instance.GetCompanyID();

            var data = db.Ezy_Users.Where(x=> x.CompanyID == CID).ToList();
            return PartialView("_SecurityManagerUserGetList", data);
        }

        public ActionResult DeleteUser(string Userid="")
        {
            var data = db.Ezy_Users.Where(x => x.UserID == Userid).FirstOrDefault();
            db.Ezy_Users.Remove(data);
            db.SaveChanges();

            var sd = db.Ezy_Users.ToList();
            return PartialView("_SecurityManagerUserGetList", sd);
        }


        public ActionResult ChangeUserVal(string Userid="")
        {
            var data = db.Ezy_Users.Where(x => x.UserID == Userid).FirstOrDefault();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMemberandGroups(string Userid="")
        {

            if (Userid != "")
            {
                var usr = db.Ezy_Users.Where(x => x.UserID == Userid).FirstOrDefault();
                var GrpUsr = db.Ezy_Groupusers.Where(x => x.UserID == Userid).ToList();
                ViewBag.MemberOf = new SelectList(GrpUsr, "GroupName", "GroupName");
                ViewBag.GrpUser  = new SelectList(ShrdMaster.Instance.GetGroupList(), "GroupName", "GroupName");
            }
            else
            {
                ViewBag.MemberOf = new SelectList(new List<ezy_groupuser>(), "GroupName", "GroupName");
                ViewBag.GrpUser = new SelectList(new List<ezy_groups>(), "GroupName", "GroupName");
            }
            return PartialView("_GroupsandMembers");
        }

        public ActionResult SaveGroupstoMembers(string GroupsName="", string UserID="")
        {
            var ss = db.Ezy_Groupusers.Where(x => x.UserID == UserID).FirstOrDefault();
            if (ss != null)
            {
                db.Ezy_Groupusers.Remove(ss);
                db.SaveChanges();
                return Json("Del", JsonRequestBehavior.AllowGet);
            }
            else
            {

                ezy_groupuser ds = new ezy_groupuser();
                ds.GroupName = GroupsName;
                ds.UserID = UserID;
                db.Ezy_Groupusers.Add(ds);
                db.SaveChanges();
                return Json("ok", JsonRequestBehavior.AllowGet);
            }
           

            //if (GroupsName != "" && UserID != "")
            //{
            //    var ss = db.Ezy_Groupusers.Where(x => x.UserID == UserID && x.GroupName == GroupsName).FirstOrDefault();
            //    if (ss == null)
            //    {
            //        ezy_groupuser ds = new ezy_groupuser();
            //        ds.GroupName = GroupsName;
            //        ds.UserID = UserID;
            //        db.Ezy_Groupusers.Add(ds);
            //        db.SaveChanges();
            //        return Json("ok", JsonRequestBehavior.AllowGet);
            //    }
            //    else
            //    {
            //        return Json("error", JsonRequestBehavior.AllowGet);
            //    }
            //}

            //return Json("NullEror", JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteGroupstoMembers(string GroupsName = "", string UserID = "")
        {
            if (GroupsName != "" && UserID != "")
            {
                var ss = db.Ezy_Groupusers.Where(x => x.UserID == UserID && x.GroupName == GroupsName).FirstOrDefault();
                if (ss != null)
                {
                    db.Ezy_Groupusers.Remove(ss);
                    db.SaveChanges();
                    return Json("ok", JsonRequestBehavior.AllowGet);
                }
            }

            return Json("NullEror", JsonRequestBehavior.AllowGet);
        }
    }
}