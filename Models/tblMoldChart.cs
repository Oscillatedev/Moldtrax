using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class tblMoldChart
    {
        [Key]
        public int ID { get; set; }
        public int? Chart { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? CompanyID { get; set; }

    }
}