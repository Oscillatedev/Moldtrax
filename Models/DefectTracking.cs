using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class DefectTracking
    {
        public DateTime SetDate { get; set; }
        public int MoldDataID { get; set; }
        public DateTime SetTime { get; set; }
        public string MoldName { get; set; }
        public string MoldDesc { get; set; }
        public string SetPressNumb { get; set; }
        public string DfctID { get; set; }
        public int DfctCavNum { get; set; }
        public string DfctDescript { get; set; }
        public string ProductLine { get; set; }
        public string ProductPart { get; set; }
        public DateTime DfctDate { get; set; }
        public string EmployeeID { get; set; }
        public string DfctNotes { get; set; }
        public int DftcEstTime { get; set; }
        public DateTime MldPullDate { get; set; }
        public DateTime MldPullTime { get; set; }
        public string CavityLocationNumber { get; set; }
        public DateTime DfctTime { get; set; }
        public string CavityNumber { get; set; }
        public string CompanyName { get; set; }
        public string CompanyCNBlockedDefects { get; set; }
        public string SetTech { get; set; }
        public string MoldConfig { get; set; }
        public string MoldConfig2 { get; set; }
        public int SetID { get; set; }
        public string MTXTotalTimeCal { get; set; }

    }
}