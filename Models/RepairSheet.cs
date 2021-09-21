using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class RepairSheet
    {
        public string MoldName { get; set; }
        public int SetID { get; set; }
        public DateTime SetDate { get; set; }
        public string CavityNumber { get; set; }
        public string CavityLocationNumber { get; set; }
        public string TSDefects { get; set; }
        public DateTime DfctDate { get; set; }
        public DateTime TSDate { get; set; }
        public DateTime TlSTime { get; set; }
        public string MoldToolDescrip { get; set; }
        public string TlCorrectiveAction { get; set; }
        public DateTime SetTime { get; set; }
        public string RsSetTech { get; set; }
        public string RsPullTech { get; set; }
        public string MldRepairdBy1 { get; set; }
        public string Tech { get; set; }
        public decimal EstCycles { get; set; }
        public DateTime MldRepairedDate { get; set; }
        public int MldRepairedTime { get; set; }
        public int MldWorkOrder { get; set; }
        public int MldProductionCycles { get; set; }
        public string MldRepairComments { get; set; }
        public string CompanyName { get; set; }
        public string CompanyCNRepairSheet { get; set; }
        public string SetPressNumb { get; set; }
        public string MldSetPullNotes { get; set; }
        public string MldPullMaintRequired { get; set; }
        public DateTime MldPullDate { get; set; }
        public DateTime MldPullTime { get; set; }
        public string MoldDesc { get; set; }
        public int MoldDataID { get; set; }
        public string TINotes { get; set; }
        public string MoldConfig { get; set; }
        public string MoldConfig2 { get; set; }
        public double TIRepairTime { get; set; }
        public string DaysRun { get; set; }
        public string MoldDefectMapPath { get; set; }



    }
}