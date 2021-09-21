using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Moldtrax.Models
{
    public class tblCorrectiveAction
    {
        [Key]
        public int TlReplID { get; set; }
        public int? DfctID { get; set; }
        public int? SetID { get; set; }
        public int? TlReplTooling { get; set; }
        public string TlCorrectiveAction { get; set; }
        public DateTime? TSDate { get; set; }
        public DateTime? TlSTime { get; set; }
        public int? TlTechnician { get; set; }
        public DateTime? TIFDate { get; set; }
        public DateTime? TIFTime { get; set; }
        [AllowHtml]
        public string TINotes { get; set; }
        public string TIType { get; set; }
        public int? TlQuantity { get; set; }
        public double? TIRepairTime { get; set; }
        public int? CompanyID { get; set; }

        //public timestamp SSMA_TimeStamp { get; set; }

    }

    public class MainMaintenanceInstruction
    {
       public List<tblCorrectiveAction> MainList { get; set; }
       public List<SelectListItem> DefctRep { get; set; }
        //public List<tblRepairFix> TblRepairFixes { get; set; }
    }

    public class DefectRepaired
    {
        public int DfctID { get; set; }
        public int SetID { get; set; }
        public int TSGuide { get; set; }
        public string DfctDescript { get; set; }
        public string CavDefect { get; set; }
        public string TSDefect { get; set; }
        public DateTime? DfctDate { get; set; }
    }
}