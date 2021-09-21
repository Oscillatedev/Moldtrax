using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class CompleteMaintenanceTracking
    {
        public string Mold{ get; set; }
        public string Description{ get; set; }
        public int DoD { get; set; }
        public string Configuration { get; set; }
        public string Configuration2 { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? StartTime { get; set; }
        public string Press { get; set; }
        public string StartTech { get; set; }
        public DateTime? StopDate { get; set; }
        public DateTime? StopTime{ get; set; }
        public string MoldStopReason{ get; set; }
        public double? CycleCount{ get; set; }
        public double? RunTimeHours{ get; set; }
        public string StopTech{ get; set; }
        public DateTime? RepairDate{ get; set; }
        public double? RepairHours{ get; set; }
        public string Status{ get; set; }
        public string RepairTech{ get; set; }
        public string WorkOrder { get; set; }
        public double? Actual { get; set; }
        public double? Adjust { get; set; }
        public double? SetID{ get; set; }
        public double? MoldDataID { get; set; }

    }

    public class CompleteMaintenanceTrackingTwo
    {
        public string Name { get; set; }
        public int Count { get; set; }

    }
}