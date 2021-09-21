using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Moldtrax.Models
{
    public class tblSchedule
    {
        [Key]
        public int SchID { get; set; }
        public int schMoldDataID { get; set; }
        public int MoldDataID { get; set; }
        public int schPriority { get; set; }
        [AllowHtml]
        public string schActionItem { get; set; }


        public DateTime? schDate { get; set; }

        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode =true, DataFormatString = "{0:H:mm}")]
        public DateTime? schTime { get; set; }

        public int schCycles { get; set; }
        public string schStatus { get; set; }
        public int? CompanyID { get; set; }

    }


}