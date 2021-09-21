using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Moldtrax.ViewMoldel
{
    public class tblTSGuideViewModel
    {
        public int TSGuide { get; set; }
        public int MoldDataID { get; set; }
        public int? TSSeqNum { get; set; }
        public string TSDefects { get; set; }
        [AllowHtml]
        public string TSExplanation { get; set; }
        [AllowHtml]
        public string TSProbCause { get; set; }
        public string TSToolInv { get; set; }
        [AllowHtml]
        public string TSSolution { get; set; }
        public string TSType { get; set; }

        [NotMapped]
        public string TsTypeName { get; set; }
        [AllowHtml]
        public string TSPreventAction { get; set; }
        //public timestamp SSMA_TimeStamp { get; set; }
        public string ImageExtension { get; set; }
        public string fileName { get; set; }

        public string ImagePath { get; set; }
        public HttpPostedFileBase ImageFilePath { get; set; }
    }

    public class ImageViewModel
    {
        public HttpFileCollectionBase ImageFilePath { get; set; }
        public int TsGuide { get; set; }
    }

    public class CommonTblTSGuide
    {
        public List<tblTSGuideViewModel> TblGuide { get; set; }
        public List<ImageViewModel> ImageFilePath { get; set; }
    }
}