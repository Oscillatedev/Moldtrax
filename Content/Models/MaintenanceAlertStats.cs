using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class MaintenanceAlertStats
    {
        public string SetPressNumb { get; set; }
        public int? MoldDataID { get; set; }
        public string MoldName { get; set; }
        public int? MoldOutPressPMRedCycles{ get; set; }
        public int? MoldOutPressPMYellowCycles{ get; set; }
        [Column("Cycles Over Red Limit")]
        public int? MoldOutPressPMRed { get; set; }
        [Column("Cycles to Reach Red Limit")]
        public int? MoldOutPressPMRed2 { get; set; }

        public int? MoldOutPressPMFreq { get; set; }

        [Column("Cycles to Reach Yellow Limit")]
        public int? MoldOutPressPMYellow { get; set; }

        public double? SumOfTotalCycles { get; set; }
        public string MoldDesc { get; set; }
        public string MoldConfig { get; set; }
        public string MoldConfig2 { get; set; }
        public DateTime? MldPullDate { get; set; }
        public string RunStatusColor { get; set; }
        public double? CycleCounter { get; set; }
    }

    public class PerformanceDashBoard
    {
        public string Status { get; set; }
        public int Cnt { get; set; }
        public double CurrentMoldsRunning { get; set; }
    }

    public class CurrentMoldRunning
    {
        public string Status { get; set; }
        public int Count { get; set; }
        public int TotalRunning { get; set; }
        public int Percentage { get; set; }
    }


    public class MainCurrentMoldRunning
    {
        public List<CurrentMoldRunning> CurrentMoldRunning1 { get; set; }
        public List<CurrentMoldRunning> CurrentMoldRunning2 { get; set; }
    }

}