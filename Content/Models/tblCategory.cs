using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class tblCategory
    {
        [Key]
        public int CatID { get; set; }
        public string CategoryName { get; set; }
        public int? CompanyID { get; set; }

    }
}