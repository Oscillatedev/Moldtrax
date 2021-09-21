using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class tblMaintAlertPercentages
    {
        [Key]
        public int Id { get; set; }
        public string Status { get; set; }
        public int Running { get; set; }
        public int TotalRunning { get; set; }
        public int Percentage { get; set; }
        public int? CompanyID { get; set; }

    }
}