using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class DefectCostandAnalysis
    {
        public DateTime SetDate { get; set; }
        public int MoldDataID { get; set; }
        public DateTime SetTime { get; set; }
        public string MoldName { get; set; }
        public string MoldDesc { get; set; }
        public string SetPressNumb { get; set; }
        public string DfctID { get; set; }
        public string TSDefects { get; set; }
        public int DfctCavNum { get; set; }
        public string DfctDescript { get; set; }
        public string ProductLine { get; set; }
        public string ProductPart { get; set; }
        public DateTime DfctDate { get; set; }
        public string EmployeeID { get; set; }
        public string DfctNotes { get; set; }
        public DateTime MldPullDate { get; set; }
        public DateTime MldPullTime { get; set; }
        public string CavityLocationNumber { get; set; }
        public DateTime DfctTime { get; set; }
        public string CavityNumber { get; set; }
        public string CompanyName { get; set; }
        public string CompanyCNBlockedDefects { get; set; }
        public string SetTech { get; set; }
        public string TlCorrectiveAction { get; set; }
        public int TlTechnician { get; set; }
        public int TlQuantity { get; set; }
        public string CaTech { get; set; }
        public int DftcEstTime { get; set; }
        public int LineTotal { get; set; }
        public double TimeCost { get; set; }
        public double SubTotalCost { get; set; }
        public string CompanyCNRepairCosts { get; set; }
        public string MoldToolDescrip { get; set; }
        public string MoldToolingPartNumber { get; set; }
        public int MoldToolCost { get; set; }
        public int TlReplID { get; set; }
        public string MoldConfig { get; set; }
        public string MoldConfig2 { get; set; }
        public double TIRepairTime { get; set; }
        public string MTXTotalTimeCal { get; set; }
    }
}