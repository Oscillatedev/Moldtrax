using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class tblDDMoldConfig
    {
        [Key]
        public int ID { get; set; }
        public string MoldConfig { get; set; }
        public string MoldConfigDesc { get; set; }
        public int? CompanyID { get; set; }

    }

    public class tblDDMoldConfig2
    {
        [Key]
        public int ID { get; set; }
        public string MoldConfig { get; set; }
        public string MoldConfigDesc { get; set; }
        public int? CompanyID { get; set; }

    }

    public class TechnicianDropDown
    {
        public int EmployeeID { get; set; }
        public string Name { get; set; }
        public int? CompanyID { get; set; }

    }

    public class MainRequiredDropDown
    {
        public int? CompanyID { get; set; }
        public string StopReason { get; set; }
        public string StopReasonDesc { get; set; }
        public int ID { get; set; }
    }
}