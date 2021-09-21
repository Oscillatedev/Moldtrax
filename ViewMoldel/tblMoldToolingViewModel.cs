using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Moldtrax.ViewMoldel
{
    public class tblMoldToolingViewModel
    {
        public int MoldToolingID { get; set; }
        public int? MoldDataID { get; set; }
        public string MoldToolingType { get; set; }
        public string MoldToolingTypeName { get; set; }

        [AllowHtml]
        public string MoldToolDescrip { get; set; }
        public string MoldToolingPartNumber { get; set; }
        public decimal? MoldToolCost { get; set; }
        public DateTime? MoldCostDate { get; set; }
        public decimal? MoldManHours { get; set; }
        public string MoldToolingImage { get; set; }
        public string MoldToolingVendor { get; set; }
        public string MoldToolingPrintNumber { get; set; }
        public int? MoldToolingPartsOnHand { get; set; }
        public int? MoldToolingReorderLevel { get; set; }
        public int? MoldToolingNumOrdered { get; set; }
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [DisplayFormat(DataFormatString = "{0:yyyy-mm-dd}", ApplyFormatInEditMode = true)]
        public DateTime? MoldToolingDateOrdered { get; set; }
        public int? MoldToolingNumReceived { get; set; }
    }
}