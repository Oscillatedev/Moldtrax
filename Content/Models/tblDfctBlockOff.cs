using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class tblDfctBlockOff
    {
        [Key]
        public int DfctID { get; set; }
        public int? SetID { get; set; }
        public DateTime? DfctDate { get; set; }
        public DateTime? DfctTime { get; set; }
        public bool DfctBlocked { get; set; }
        public bool DfctQC { get; set; }
        public int? DfctCavNum { get; set; }
        public int? DfctDescript { get; set; }
        public int? EmployeeID { get; set; }
        public string DfctNotes { get; set; }
        public bool DftcRepaired { get; set; }
        public double? DftcEstTime { get; set; }
        public int? CompanyID { get; set; }


        [NotMapped]
        public int MoldID { get; set; }
        ////public timestamp SSMA_TimeStamp { get; set; }

    }
}