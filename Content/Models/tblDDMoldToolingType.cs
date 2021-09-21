using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class tblDDMoldToolingType
    {
        public int ID { get; set; }
        public string DD_MoldToolingType { get; set; }
        public string DD_MoldToolingTypeDesc { get; set; }
        public int? CompanyID { get; set; }

    }
}