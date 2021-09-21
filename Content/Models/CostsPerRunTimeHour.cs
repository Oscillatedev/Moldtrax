using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class CostsPerRunTimeHour
    {
        public int MoldDataID { get; set; }
        public string Mold { get; set; }
        public string Description { get; set; }
        public string Configuration { get; set; }
        public string Configuration2 { get; set; }
        [Column("Mold Stops")]
        public int MoldStops { get; set; }

        public int Scheduled { get; set; }

        [Column("X-Stop")]
        public int XStop { get; set; }
        public int Defect { get; set; }
        public int Quality { get; set; }
        public int Blocked { get; set; }

        [Column("Tooling Cost")]
        public double ToolingCost { get; set; }

        [Column("Labor Hours")]
        public double LaborHours { get; set; }

        [Column("Labor Cost")]
        public double LaborCost { get; set; }

        [Column("Total Cost")]
        public double TotalCost { get; set; }

        [Column("Run Time Minutes")]
        public double RunTimeMinutes { get; set; }

        [Column("Run Time")]
        public string RunTime { get; set; }

        [Column("Total Run Time  Hours")]
        public double TotalRunTimeHours { get; set; }

        [Column("Cost Per Hour")]
        public decimal CostPerHour { get; set; }

        [Column("Cycle Time Sec")]
        public double CycleTimeSec { get; set; }

        [Column("Total Actual Cycles Run")]
        public double TotalActualCyclesRun { get; set; }

    }

    public class CostsPerRunTimeHourTwo
    {
        public string MoldV { get; set; }
        public string DescriptionV { get; set; }
        public int DefectCount { get; set; }
    }

    public class CostsPerRunTimeHourThree
    {
        public string Mold { get; set; }
        public double? LaborCosts { get; set; }
    }

    public class CommonChartProp
    {
        public string Name { get; set; }
        public double Quantity { get; set; }
    }

    public class CommonChartProp2
    {
        public string Name { get; set; }
        public double? Quantity { get; set; }
    }
}