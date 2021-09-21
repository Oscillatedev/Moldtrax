using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class MaintenanceTimeline
    {
        public int MoldDataID { get; set; }
        public string MoldName { get; set; }
        public string MoldDesc { get; set; }
        public DateTime SetDate { get; set; }
        public DateTime SetTime { get; set; }
        public string ProductLine { get; set; }
        public string ProductPart { get; set; }
        public DateTime MldPullDate { get; set; }
        public DateTime MldPullTime { get; set; }
        public string SetPressNumb { get; set; }
        public string MldPullMaintRequired { get; set; }
        public string MldSetPullNotes { get; set; }
        public string EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MoldToolDescrip { get; set; }
        public string MldRepairComments { get; set; }
        public int SetID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyCNToolingExp { get; set; }
        public string MoldConfig { get; set; }
        public string MoldConfig2 { get; set; }
        public string MTXTotalTimeCal { get; set; }
    }
}