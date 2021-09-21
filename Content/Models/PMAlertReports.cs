using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class PMAlertReports
    {
        public string Press { get; set; }
        public string Mold { get; set; }
        public string Description { get; set; }
        public string Config1 { get; set; }
        public string Config2 { get; set; }
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        //[DataType(DataType.Date)]
        public string StopDate { get; set; }

        public DateTime NewStopDate { get; set; }

        public int NewStopSortOrder { get; set; }

        public double? CyclesToReachYellowLimits { get; set; }
        public double? NewCyclesToReachYellowLimits { get; set; }


        public double? HoursToReachYellowLimits { get; set; }
        public double? NewHoursToReachYellowLimits { get; set; }

        public double? CyclesToReachRedLimits { get; set; }
        public double? NewCyclesToReachRedLimits { get; set; }

        public double? HoursToReachRedLimits { get; set; }
        public double? NewHoursToReachRedLimits { get; set; }

        public string Color { get; set; }
    }
}