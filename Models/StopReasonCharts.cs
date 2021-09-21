using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class StopReasonCharts
    {
        //[Column("Mold Stop Reason")]
        public string MoldStopReason { get; set; }

        //[Column("Stop Count")]
        public int? StopCount { get; set; }

        //[Column("Labor Hours")]
        public double? LaborHours { get; set; }

        //[Column("Labor Cost")]
        public double? LaborCost { get; set; }

        //[Column("Tooling Cost")]
        public decimal? ToolingCost { get; set; }

        //[Column("Total Cost")]
        public double? TotalCost { get; set; }
    }
}