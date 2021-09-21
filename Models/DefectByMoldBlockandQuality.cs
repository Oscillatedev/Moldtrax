using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class DefectByMoldBlockandQuality
    {

        public string Mold { get; set; }
        public string Description { get; set; }
        public string Configuration { get; set; }
        public string Configuration2 { get; set; }
        public string TroubleShootersDefects { get; set; }
        public string Type { get; set; }
        public bool Blocked { get; set; }
        public bool Quality { get; set; }
        public int Count { get; set; }
    }

    public class DefectcMoldBlockandQualityTwo
    {
        public string TroubleShootersDefect { get; set; }
        public int DefectCount { get; set; }
    }
}