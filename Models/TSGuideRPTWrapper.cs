using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class TSGuideRPTWrapper
    {
        public int TSSeqNum { get; set; }
        public int TSGuide { get; set; }
        public int MoldDataID { get; set; }
        public string TSDefects { get; set; }
        public string TSExplanation { get; set; }
        public string TSToolInv { get; set; }
        public string TSProbCause { get; set; }
        public string TSSolution { get; set; }
        public string ImagePath { get; set; }
        public string TSType { get; set; }
        public string TSPreventAction { get; set; }
        public string MoldName { get; set; }
        public string MoldDesc { get; set; }
        public string CompanyName { get; set; }
        public string CompanyCNTroubleShoot { get; set; }

    }
}