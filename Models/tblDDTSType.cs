using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class tblDDTSType
    {
        public int ID { get; set; }
        public string TSType { get; set; }
        public string TSTypeDesc { get; set; }
        public int? CompanyID { get; set; }
    }
}