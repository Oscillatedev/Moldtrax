using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class tblDocs
    {
        [Key]
        public int DocID { get; set; }
        public int DocMoldID { get; set; }
        public string DocSection { get; set; }
        public string DocName { get; set; }
        public string DocLink { get; set; }
        public int? CompanyID { get; set; }


        [NotMapped]
        public string Category { get; set; }
    }


    public class tblDDDocSection
    {
        [Key]
        public int ID { get; set; }
        public string DocSection { get; set; }
        public string DocSectionDesc { get; set; }
        public int? CompanyID { get; set; }

    }
}