using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class tblDDFactors
    {
        public int ID { get; set; }
        public string Plastic_Type { get; set; }
        public double PF { get; set; }
        public string Steel_Type { get; set; }
        public double SF { get; set; }
        public string Location_Type { get; set; }
        public double LF { get; set; }
        public int? CompanyID { get; set; }

    }
}