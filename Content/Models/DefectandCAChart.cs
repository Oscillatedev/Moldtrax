using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class DefectandCAChart
    {
        public string Mold { get; set; }
        public string Description { get; set; }
        public string Configuration { get; set; }
        public string Configuration2 { get; set; }
        public double? DoD { get; set; }

        //[Column("Mold Start Date")]
        public DateTime? MoldStartDate { get; set; }

        //[Column("Mold Stop Date")]
        public DateTime? MoldStopDate { get; set; }

        //[Column("Stop Reason")]
        public string StopReason { get; set; }

        //[Column("TroubleShooters Defect")]
        public string TroubleShootersDefect { get; set; }

        //[Column("Defect Type")]
        public string DefectType { get; set; }

        public string Position { get; set; }

        //[Column("Cavity ID")]
        public string CavityID { get; set; }

        //[Column("Corrective Action")]
        public string CorrectiveAction { get; set; }

        //[Column("Tooling Description")]
        public string ToolingDescription { get; set; }

        //[Column("CA Tech")]
        public string CATech { get; set; }

        //[Column("CA Date")]
        public DateTime? CADate { get; set; }

        //[Column("Repair Date")]
        public DateTime? RepairDate { get; set; }

        public int? QTY { get; set; }

        //[Column("Repair Hours")]
        public decimal? RepairHours { get; set; }

        //[Column("Labor Cost")]
        public double LaborCost { get; set; }

        //[Column("Tooling Cost")]
        public double ToolingCost { get; set; }

        //[Column("Total Cost")]
        public double TotalCost { get; set; }

        //[Column("Cycle Count")]
        public int? CycleCount { get; set; }

        //[Column("Repair Time")]
        public double? RepairTime { get; set; }

        [Column("Run Time  Hours")]
        public int? RunTimeHours { get; set; }
    }

    public class DefectandCAChartTwo
    {
        public string CorrectiveActionV { get; set; }
        public int Q { get; set; }
    }
}