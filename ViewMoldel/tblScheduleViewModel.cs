using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Moldtrax.ViewMoldel
{
    public class tblScheduleViewModel
    {
            [Key]
            public int SchID { get; set; }
            public int schMoldDataID { get; set; }
            public int MoldDataID { get; set; }
            public int schPriority { get; set; }
            [AllowHtml]
            public string schActionItem { get; set; }
           public string NewSchActionItem { get; set; }

            public DateTime? schDate { get; set; }

            public string NewSchDate { get; set; }

            public string MoldName { get; set; }
            [DataType(DataType.Time)]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:H:mm}")]
            public DateTime? schTime { get; set; }
        public string NewSchTime { get; set; }

            public int schCycles { get; set; }
            public string NewSchCycles { get; set; }
            public string schStatus { get; set; }
    }
}