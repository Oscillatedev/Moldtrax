using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Moldtrax.Models
{
    public class tblTSGuide
    {
        [Key]
        public int TSGuide { get; set; }
        public int? MoldDataID { get; set; }
        public int? TSSeqNum { get; set; }
        public string TSDefects { get; set; }
        [AllowHtml]
        public string TSExplanation { get; set; }
        [AllowHtml]
        public string TSProbCause { get; set; }
        public string TSToolInv { get; set; }
        [AllowHtml]
        public string TSSolution { get; set; }
        public byte[] TSImage { get; set; }
        public string TSType { get; set; }

        [NotMapped]
        public string TSTypeName { get; set; }
        [AllowHtml]
        public string TSPreventAction { get; set; }
        //public timestamp SSMA_TimeStamp { get; set; }
        public string ImageExtension { get; set; }
        public string fileName { get; set; }

        public string ImagePath { get; set; }
        public int? CompanyID { get; set; }

    }
}