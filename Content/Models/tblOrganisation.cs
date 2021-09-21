using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Moldtrax.Models
{
    public class tblOrganisation
    {
        [Key]
        public int OrgID { get; set; }
        public string OrgName { get; set; }
        public string OrgAddress { get; set; }
        public string OrgCity { get; set; }
        public string OrgState { get; set; }
        public string OrgZipCode { get; set; }
        public string OrgCountry { get; set; }
        public string Org800 { get; set; }
        public string OrgMainPhone { get; set; }
        public string OrgFax { get; set; }
        public string OrgWebsite { get; set; }
        public string OrgEmail { get; set; }
        public string OrgFTPsite { get; set; }
        [AllowHtml]
        public string OrgNotes { get; set; }
        public string OrgCNTotalTimeRun { get; set; }
        public string OrgCNCavEff { get; set; }
        public string OrgCNBlockedDefects { get; set; }
        public string OrgCNRepairCosts { get; set; }
        public string OrgCNToolingExp { get; set; }
        public string OrgCNRepairHrs { get; set; }
        public string OrgCNReasonPulled { get; set; }
        public string OrgCNDefectbyPress { get; set; }
        public string OrgCNInPressRpr { get; set; }
        public string OrgCNRepairSheet { get; set; }
        public string OrgCNTroubleShoot { get; set; }
        public string OrgCNTechTips { get; set; }
        public string OrgCNTooliong { get; set; }
        public string OrgCNRoverSheet { get; set; }
        public string OrgCNRepairStatus { get; set; }
        public string OrgCNILM { get; set; }
        public string OrgCNLastShot { get; set; }
        public double? OrgReg { get; set; }
        public string OrgSerialNumber { get; set; }
    }
}