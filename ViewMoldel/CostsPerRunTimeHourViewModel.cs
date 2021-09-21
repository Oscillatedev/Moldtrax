using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Moldtrax.ViewMoldel
{
    public class CostsPerRunTimeHourViewModel
    {
        public int MoldDataID { get; set; }
        public string Mold { get; set; }
        public string Description { get; set; }
        public string Configuration { get; set; }
        public string Configuration2 { get; set; }
        public int MoldStops { get; set; }
        public int Scheduled { get; set; }
        public int XStop { get; set; }
        public double XStopPercent { get; set; }
        public int Defect { get; set; }
        public int Quality { get; set; }
        public int Blocked { get; set; }
        public double ToolingCost { get; set; }
        public double LaborHours { get; set; }
        public double LaborCost { get; set; }
        public double TotalCost { get; set; }
        public double RunTimeMinutes { get; set; }
        public string RunTime { get; set; }
        public double TotalRunTimeHours { get; set; }
        public decimal CostPerHour { get; set; }
        public double CycleTimeSec { get; set; }
        public double TotalActualCyclesRun { get; set; }
        public double CycleRunPerLakh { get; set; }
        public double CostsperLakhCycle { get; set; }
        public double CostPerDefect { get; set; }
        public double CyclesPerDefect { get; set; }
        public double RunHrsPerDefect { get; set; }
    }
}