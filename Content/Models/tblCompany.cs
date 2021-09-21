using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Moldtrax.Models
{
    public class tblCompany
    {
        [Key]
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyCity { get; set; }
        public string CompanyState { get; set; }
        public string CompanyZipCode { get; set; }
        public string CompanyCountry { get; set; }
        public string Company800 { get; set; }
        public string CompanyMainPhone { get; set; }
        public string CompanyFax { get; set; }
        public string CompanyWebsite { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyFTPsite { get; set; }
        [AllowHtml]
        public string CompanyNotes { get; set; }
        public string CompanyCNTotalTimeRun { get; set; }
        public string CompanyCNCavEff { get; set; }
        public string CompanyCNBlockedDefects { get; set; }
        public string CompanyCNRepairCosts { get; set; }
        public string CompanyCNToolingExp { get; set; }
        public string CompanyCNRepairHrs { get; set; }
        public string CompanyCNReasonPulled { get; set; }
        public string CompanyCNDefectbyPress { get; set; }
        public string CompanyCNInPressRpr { get; set; }
        public string CompanyCNRepairSheet { get; set; }
        public string CompanyCNTroubleShoot { get; set; }
        public string CompanyCNTechTips { get; set; }
        public string CompanyCNTooliong { get; set; }
        public string CompanyCNRoverSheet { get; set; }
        public string CompanyCNRepairStatus { get; set; }
        public string CompanyCNILM { get; set; }
        public string CompanyCNLastShot { get; set; }
        public double? CompanyReg { get; set; }
        public string CompanySerialNumber { get; set; }
        //public timestamp SSMA_TimeStamp { get; set; }

    }


    public class tblCompanyMain
    {
        public tblCompany TBLCompany { get; set; }
        public IEnumerable<tblCompany> TBLCompaniesList { get; set; }
    }

}