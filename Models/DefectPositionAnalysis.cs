using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class DefectPositionAnalysis
    {
        public string Mold { get; set; }
        public string Description { get; set; }
        public string Configuration { get; set; }
        public string Configuration2 { get; set; }
        public string Press { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? StopDate { get; set; }
        public double? CycleCount { get; set; }
        public decimal? RunTimeHours { get; set; }
        public string Position { get; set; }
        public string TroubleShootersDefects { get; set; }
        public string Type { get; set; }
        public DateTime? BlockedNoted { get; set; }
        public DateTime? Time { get; set; }
        public int? Blocked { get; set; }
        public int? Quality { get; set; }
        public string BlockedByNotedBy { get; set; }
        public string CavityID { get; set; }
        public double? RepairTime { get; set; }
    }
}