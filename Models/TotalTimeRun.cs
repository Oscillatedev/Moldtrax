using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class TotalTimeRun
    {
        public int MoldDataID { get; set; }
        public string MoldName { get; set; }
        public string MoldDesc { get; set; }
        public DateTime SetDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime StopDate { get; set; }
        public DateTime StopTime { get; set; }
        public string TotalTime { get; set; }
        public string MTXTotalMinsH { get; set; }
        public string ProductLine { get; set; }
        public string ProductPart { get; set; }
        public string SetPressNumb {get; set;}
        public string CompanyName { get; set; }
        public string CompanyCNTotalTimeRun { get; set; }
        public string MoldConfig { get; set; }
        public string MoldConfig2 { get; set; }
        public int MldProductionCycles { get; set; }

    }
}