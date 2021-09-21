using Moldtrax.Models;
using Moldtrax.Providers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Moldtrax.Controllers
{

    [SessionExpireFilter]
    public class CompanyInfoController : Controller
    {
        private MoldtraxDbContext db = new MoldtraxDbContext();
        // GET: CompanyInfo
        public ActionResult Index(int CID=0)
        {
            if (CID == 0)
            {

            }

            ViewBag.Companylist = new SelectList(db.TblCompanies.ToList().OrderBy(x => x.CompanyName), "CompanyID", "CompanyName");
            return View();
        }


        public ActionResult ManagingCompanyGetData()
        {
            tblCompanyMain TCM = new tblCompanyMain();

            string UID = Session["User"].ToString();

            int CID = ShrdMaster.Instance.GetCompanyID();

            var UserRole = db.Ezy_Users.Where(x => x.UserID == UID).FirstOrDefault().RoleID;

            List<tblCompany> tc = new List<tblCompany>();

            if (UserRole == 1)
            {
                tc = db.TblCompanies.ToList();
            }
            else
            {
                tc = db.TblCompanies.Where(x => x.CompanyID == CID).ToList();
            }

            ViewBag.IsOrgorCom = UserRole == 1 ? true : false;

            var data = tc.FirstOrDefault();
            TCM.TBLCompany = data == null ? new tblCompany() : data;
            TCM.TBLCompaniesList = tc == null ? new List<tblCompany>() : tc;

            return PartialView("_Company", TCM);
        }

        public ActionResult GetCompanyDetails(int CID=0)
        {
            var data = db.TblCompanies.Where(x => x.CompanyID == CID).FirstOrDefault();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CreateCompany(string CompanyName, int CID=0)
        {
            if (CID == 0)
            {
                tblCompany TC = new tblCompany();
                TC.CompanyName = CompanyName;
                db.TblCompanies.Add(TC);
                db.SaveChanges();

                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.CompanyInformation.ToString(), GetTabName.ManagingCompany.ToString(), GetAction.Create.ToString());

                return Json(TC.CompanyID, JsonRequestBehavior.AllowGet);
            }

            return Json(CID, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateCompanys(tblCompany obj)
            {
            var data = db.TblCompanies.Where(x => x.CompanyID == obj.CompanyID).FirstOrDefault();

            if (data != null)
            {
                //tblCompany tbl = new tblCompany();
                data.CompanyName = obj.CompanyName;
                data.CompanyAddress = obj.CompanyAddress;
                data.CompanyCity = obj.CompanyCity;
                data.CompanyState = obj.CompanyState;
                data.CompanyZipCode = obj.CompanyZipCode;
                data.CompanyCountry = obj.CompanyCountry;
                data.Company800 = obj.Company800;
                data.CompanyMainPhone = obj.CompanyMainPhone;
                data.CompanyFax = obj.CompanyFax;
                data.CompanyWebsite = obj.CompanyWebsite;
                data.CompanyEmail = obj.CompanyEmail;
                data.CompanyFTPsite = obj.CompanyFTPsite;
                data.CompanyNotes = obj.CompanyNotes;
                data.CompanyCNTotalTimeRun = obj.CompanyCNTotalTimeRun;
                data.CompanyCNCavEff = obj.CompanyCNCavEff;
                data.CompanyCNBlockedDefects = obj.CompanyCNBlockedDefects;
                data.CompanyCNRepairCosts = obj.CompanyCNRepairCosts;
                data.CompanyCNToolingExp = obj.CompanyCNToolingExp;
                data.CompanyCNRepairHrs = obj.CompanyCNRepairHrs;
                data.CompanyCNReasonPulled = obj.CompanyCNReasonPulled;
                data.CompanyCNDefectbyPress = obj.CompanyCNDefectbyPress;
                data.CompanyCNInPressRpr = obj.CompanyCNInPressRpr;
                data.CompanyCNRepairSheet = obj.CompanyCNRepairSheet;
                data.CompanyCNTroubleShoot = obj.CompanyCNTroubleShoot;
                data.CompanyCNTechTips = obj.CompanyCNTechTips;
                data.CompanyCNTooliong = obj.CompanyCNTooliong;
                data.CompanyCNRoverSheet = obj.CompanyCNRoverSheet;
                data.CompanyCNRepairStatus = obj.CompanyCNRepairStatus;
                data.CompanyCNILM = obj.CompanyCNILM;
                data.CompanyCNLastShot = obj.CompanyCNLastShot;
                data.CompanyReg = obj.CompanyReg;
                data.CompanySerialNumber = obj.CompanySerialNumber;

                //db.Entry(obj).State = EntityState.Modified;
                db.SaveChanges();

                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.CompanyInformation.ToString(), GetTabName.ManagingCompany.ToString(), GetAction.Update.ToString());

            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeleteCompany(int CID)
        {
            var data = db.TblCompanies.Where(x => x.CompanyID == CID).FirstOrDefault();
            if (data != null)
            {
                db.TblCompanies.Remove(data);
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.CompanyInformation.ToString(), GetTabName.ManagingCompany.ToString(), GetAction.Delete.ToString());

            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveInfo(tblCompany model)
        {
            if (model.CompanyID != 0)
            {
                var data = db.TblCompanies.Where(x => x.CompanyID == model.CompanyID).FirstOrDefault();
                data.CompanyName = model.CompanyName;
                data.CompanyAddress = model.CompanyAddress;
                data.CompanyCity = model.CompanyCity;
                data.CompanyState = model.CompanyState;
                data.CompanyZipCode = model.CompanyZipCode;
                data.CompanyCountry = model.CompanyCountry;
                data.CompanyMainPhone = model.CompanyMainPhone;
                data.CompanyFax = model.CompanyFax;
                data.Company800 = model.Company800;
                data.CompanyWebsite = model.CompanyWebsite;
                data.CompanyEmail = model.CompanyEmail;
                data.CompanyNotes = model.CompanyNotes;

                db.SaveChanges();

                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.CompanyInformation.ToString(), GetTabName.ManagingCompany.ToString(), GetAction.Update.ToString());

            }
            else
            {
                db.TblCompanies.Add(model);
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.CompanyInformation.ToString(), GetTabName.ManagingCompany.ToString(), GetAction.Create.ToString());

            }
            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCustomerList()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            var Cust = db.TblCustomer.Where(x=> x.CompanyID == CID).ToList().OrderBy(x=> x.CustomerName);

            TblCustomerMain TM = new TblCustomerMain();

            if (Cust.Count() != 0)
            {
                TM.TBLCustomer = Cust.FirstOrDefault();
                TM.TblCustomerList = Cust;
            }

            else
            {
                TM.TBLCustomer = new tblCustomer();
                TM.TblCustomerList = new List<tblCustomer>();
            }

            return PartialView("_Customer", TM);
        }

        public ActionResult CustomerDetail(int CustID =0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var Cust = db.TblCustomer.Where(x=> x.CompanyID == CID).ToList().OrderBy(x=> x.CustomerName);

            if (CustID == -1)
            {
                TblCustomerMain TM = new TblCustomerMain();
                TM.TBLCustomer = new tblCustomer();
                TM.TblCustomerList = Cust;

                return PartialView("_Customer", TM);
            }

            else
            {
                int ID = ReturnCustID(CustID);
                var data = db.TblCustomer.Where(x => x.CustomerID == ID).FirstOrDefault();

                TblCustomerMain TM = new TblCustomerMain();
                TM.TBLCustomer = data;
                TM.TblCustomerList = Cust;

                return PartialView("_Customer", TM);
            }
        }

        public int ReturnCustID(int CustID=0)
        {
            int ID = 0;
            if (CustID == 0)
            {
                ID = db.TblCustomer.Select(x => x.CustomerID).FirstOrDefault();
            }
            else
            {
                ID = CustID;
            }

            return ID;
        }

        public ActionResult SaveFocusOutCustomerDetails(tblCustomer model)
        {
            try
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.CompanyInformation.ToString(), GetTabName.Customer.ToString(), GetAction.Update.ToString());

            }
            catch (Exception ex)
            {

            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveCustomerDetails(tblCustomer model)
        {
            if (model.CustomerID == 0)
            {
                model.CompanyID = ShrdMaster.Instance.GetCompanyID(); ;
                db.TblCustomer.Add(model);
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.CompanyInformation.ToString(), GetTabName.ManagingCompany.ToString(), GetAction.Create.ToString());

            }
            else
            {
                try
                {
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    //ShrdMaster.Instance.UpdateAuditLog(GetPageName.CompanyInformation.ToString(), GetTabName.ManagingCompany.ToString(), GetAction.Update.ToString());
                }
                catch (Exception ex)
                {

                }
            }

            int CID = ShrdMaster.Instance.GetCompanyID();

            TblCustomerMain TM = new TblCustomerMain();
            TM.TBLCustomer = model;
            TM.TblCustomerList = db.TblCustomer.Where(x=> x.CompanyID == CID).ToList().OrderBy(x=> x.CustomerName);
            return PartialView("_Customer", TM);
        }


        public ActionResult CustomerDelete(int CustID=0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            var data = db.TblCustomer.Where(x => x.CustomerID == CustID).FirstOrDefault();
            if (data != null)
            {
            db.TblCustomer.Remove(data);
            db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.CompanyInformation.ToString(), GetTabName.ManagingCompany.ToString(), GetAction.Delete.ToString());

            }

            var Cust = db.TblCustomer.Where(x=> x.CompanyID == CID).ToList().OrderBy(x => x.CustomerName);

            TblCustomerMain TM = new TblCustomerMain();
            TM.TBLCustomer = Cust.FirstOrDefault();
            TM.TblCustomerList = Cust;

            return PartialView("_Customer", TM);
        }




        #region Vendors

        public ActionResult GetVendorList()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            var data = db.TblVendors.Where(x=> x.CompanyID == CID).ToList().OrderBy(x=> x.VendorName);

            TBLVendorMain TV = new TBLVendorMain();
            if (data.Count() != 0)
            {
                TV.TblVendors = data.FirstOrDefault();
                TV.TblVendorsList = data;
            }
            else
            {
                TV.TblVendors = new tblVendors();
                TV.TblVendorsList = new List<tblVendors>();
            }

            return PartialView("_VendorList", TV);
        }

        public ActionResult SaveFocusOuVendorDetails(tblVendors model)
        {
            try
            {
                model.CompanyID = ShrdMaster.Instance.GetCompanyID();
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.CompanyInformation.ToString(), GetTabName.Vendors.ToString(), GetAction.Update.ToString());
            }
            catch (Exception ex)
            {

            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveVendorDetails(tblVendors model)
        {
            if (model.VendorID == 0)
            {
                model.CompanyID = ShrdMaster.Instance.GetCompanyID();
                db.TblVendors.Add(model);
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.CompanyInformation.ToString(), GetTabName.Vendors.ToString(), GetAction.Create.ToString());
            }
            else
            {
                try
                {
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    //ShrdMaster.Instance.UpdateAuditLog(GetPageName.CompanyInformation.ToString(), GetTabName.Vendors.ToString(), GetAction.Update.ToString());

                }
                catch (Exception ex)
                {

                }
            }

            int CID = ShrdMaster.Instance.GetCompanyID();

            var data = db.TblVendors.Where(x=> x.CompanyID == CID).ToList().OrderBy(x=> x.VendorName);
            TBLVendorMain TV = new TBLVendorMain();
            TV.TblVendors = model;
            TV.TblVendorsList = data;
            return PartialView("_VendorList", TV);
        }


        public int ReturnVenID(int VenID = 0)
        {
            int ID = 0;
            if (VenID == 0)
            {
                ID = db.TblVendors.Select(x => x.VendorID).FirstOrDefault();
            }
            else
            {
                ID = VenID;
            }

            return ID;
        }

        public ActionResult VendorDetail(int VenID = 0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblVendors.Where(x=> x.CompanyID == CID).OrderBy(x=> x.VendorName).ToList();
            TBLVendorMain TV = new TBLVendorMain();
            if (VenID == -1)
            {
                TV.TblVendors = new tblVendors();
                TV.TblVendorsList = data;
                return PartialView("_VendorList", TV);
            }

            else
            {
                int ID = ReturnVenID(VenID);
                var v = db.TblVendors.Where(x => x.VendorID == ID).FirstOrDefault();
                TV.TblVendors = v;
                TV.TblVendorsList = data;
                return PartialView("_VendorList", TV);
            }

        }

        public ActionResult VendorDelete(int CustID = 0)
        {
            var data = db.TblVendors.Where(x => x.VendorID == CustID).FirstOrDefault();
            if (data != null)
            {
                db.TblVendors.Remove(data);
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.CompanyInformation.ToString(), GetTabName.Vendors.ToString(), GetAction.Delete.ToString());

            }

            int CID = ShrdMaster.Instance.GetCompanyID();
            var tblv = db.TblVendors.Where(x=> x.CompanyID == CID).ToList().OrderBy(x => x.VendorName);
            TBLVendorMain TV = new TBLVendorMain();
            TV.TblVendors = tblv.FirstOrDefault();
            TV.TblVendorsList = tblv;
            return PartialView("_VendorList", TV);
        }

        #endregion



        #region Employee

        public ActionResult GetEmployeeList()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblEmployees.Where(x=> x.CompanyID == CID).ToList().OrderBy(x=> x.LastName);

            TbLEmployeeMain TE = new TbLEmployeeMain();

            if (data.Count() != 0)
            {
                TE.TblEmployees = data.FirstOrDefault();
                TE.TblEmployeesList = data;
            }
            else
            {
                TE.TblEmployees = new tblEmployees();
                TE.TblEmployeesList = new List<tblEmployees>();
            }
          
            return PartialView("_EmployeeList", TE);
        }


        public ActionResult SaveFocusOutEmployeeDetails(tblEmployees model)
        {
            try
            {
                model.CompanyID = ShrdMaster.Instance.GetCompanyID();
                model.EmplHireDate = model.EmplHireDate == DateTime.MinValue ? null : model.EmplHireDate;
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.CompanyInformation.ToString(), GetTabName.Employees.ToString(), GetAction.Update.ToString());

            }
            catch (Exception ex)
            {

            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveEmployeeDetails(tblEmployees model)
        {
            if (model.EmployeeID == 0)
            {
                model.EmplHireDate = model.EmplHireDate == DateTime.MinValue ? null : model.EmplHireDate;
                model.CompanyID = ShrdMaster.Instance.GetCompanyID();
                db.TblEmployees.Add(model);
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.CompanyInformation.ToString(), GetTabName.Employees.ToString(), GetAction.Create.ToString());
            }
            else
            {
                try
                {
                    model.EmplHireDate = model.EmplHireDate == DateTime.MinValue ? null : model.EmplHireDate;
                    db.Entry(model).State = EntityState.Modified;
                    db.SaveChanges();
                    //ShrdMaster.Instance.UpdateAuditLog(GetPageName.CompanyInformation.ToString(), GetTabName.Employees.ToString(), GetAction.Update.ToString());

                }
                catch (Exception ex)
                {

                }
            }

            int CID = ShrdMaster.Instance.GetCompanyID();

            var data = db.TblEmployees.Where(x=> x.CompanyID == CID).ToList().OrderBy(x => x.LastName);
            TbLEmployeeMain TE = new TbLEmployeeMain();
            TE.TblEmployees = model;
            TE.TblEmployeesList = data;
            return PartialView("_EmployeeList", TE);
        }


        public int ReturnEmpID(int VenID = 0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();

            int ID = 0;
            if (VenID == 0)
            {
                var data = db.TblEmployees.Where(x=> x.CompanyID == CID).OrderBy(X=> X.LastName).ToList();
                ID = data.Select(x => x.EmployeeID).First();
            }
            else
            {
                ID = VenID;
            }

            return ID;
        }

        public ActionResult EmployeeDetail(int VenID = 0)
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblEmployees.Where(x=> x.CompanyID == CID).OrderBy(x=> x.LastName).ToList();
            TbLEmployeeMain TE = new TbLEmployeeMain();
 

            if (VenID == -1)
            {
                TE.TblEmployees = new tblEmployees();
                TE.TblEmployeesList = data;
                return PartialView("_EmployeeList", TE);
            }

            else
            {
                int ID = ReturnEmpID(VenID);
                var TBE = db.TblEmployees.Where(x => x.EmployeeID == ID).FirstOrDefault();
                TBE.EmplHireDate = TBE.EmplHireDate == null ? new DateTime() : TBE.EmplHireDate;
                TE.TblEmployees = TBE;
                TE.TblEmployeesList = data;
                return PartialView("_EmployeeList", TE);
            }
        }

        public ActionResult EmployeeDelete(int CustID = 0)
        {
            var data = db.TblEmployees.Where(x => x.EmployeeID == CustID).FirstOrDefault();
            if (data != null)
            {
                db.TblEmployees.Remove(data);
                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.CompanyInformation.ToString(), GetTabName.Employees.ToString(), GetAction.Delete.ToString());

            }

            int CID = ShrdMaster.Instance.GetCompanyID();

            var TBE = db.TblEmployees.Where(x=> x.CompanyID == CID).ToList().OrderBy(x => x.LastName);
            TbLEmployeeMain TE = new TbLEmployeeMain();
            TE.TblEmployees = TBE.FirstOrDefault();
            TE.TblEmployeesList = TBE;
            return PartialView("_EmployeeList", TE);
        }

        #endregion

        #region Document Control Numbers

        public ActionResult GetDocumentCompany()
        {
            int CID = ShrdMaster.Instance.GetCompanyID();
            var data = db.TblCompanies.Where(x=> x.CompanyID == CID).FirstOrDefault();
            return PartialView("_Document", data);
        }

        public ActionResult SaveDoc(tblCompany model)
        {
            if (model.CompanyID != 0)
            {
                var data = db.TblCompanies.Where(x => x.CompanyID == model.CompanyID).FirstOrDefault();
                data.CompanyCNRepairSheet = model.CompanyCNRepairSheet;
                data.CompanyCNToolingExp = model.CompanyCNToolingExp;
                data.CompanyCNRepairCosts = model.CompanyCNRepairCosts;
                data.CompanyCNBlockedDefects = model.CompanyCNBlockedDefects;
                data.CompanyCNTotalTimeRun = model.CompanyCNTotalTimeRun;
                data.CompanyCNILM = model.CompanyCNILM;
                data.CompanyCNTroubleShoot = model.CompanyCNTroubleShoot;
                data.CompanyCNTechTips = model.CompanyCNTechTips;
                data.CompanyCNTooliong = model.CompanyCNTooliong;
                data.CompanyCNLastShot = model.CompanyCNLastShot;
                data.CompanyID = ShrdMaster.Instance.GetCompanyID();

                db.SaveChanges();
                //ShrdMaster.Instance.UpdateAuditLog(GetPageName.CompanyInformation.ToString(), GetTabName.DocumentControlNum.ToString(), GetAction.Update.ToString());

            }

            return Json("", JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}