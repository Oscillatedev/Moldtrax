using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class tblRepairFix
    {
        [Key]
        public int RPRepairFixID { get; set; }
        public int DfctID { get; set; }
        public int TlReplID { get; set; }
        public int? CompanyID { get; set; }

    }
}