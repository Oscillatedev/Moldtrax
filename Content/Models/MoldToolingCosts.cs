using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class MoldToolingCosts
    {
        public string Mold { get; set; }
        public string Description { get; set; }
        public string Configuration { get; set; }
        public string Configuration2 { get; set; }
        public string Tooling { get; set; }
        public int Qty { get; set; }
        public string Type { get; set; }
        public DateTime? DateInstalled { get; set; }
        public DateTime? RepairDate { get; set; }
        public string  PartNo { get; set; }
        public string  DetailNo { get; set; }
        public string  Vendor { get; set; }
        public double? PartCost { get; set; }
        public double? TotalCost { get; set; }
        public double? CycleCount { get; set; }
    }

    public class MoldToolingCostsTwo
    {
        public string Tooling { get; set; }
        public double? TotalCost { get; set; }
    }
}