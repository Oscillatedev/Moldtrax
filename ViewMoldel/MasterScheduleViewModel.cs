using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Moldtrax.ViewMoldel
{
    public class MasterScheduleViewModel
    {
        public string DateNoted { get; set; }
        public string Time { get; set; }
        public string Mold { get; set; }
        public string Priority { get; set; }
        public string ActionItem { get; set; }
        public string Cycles { get; set; }
        public string Status { get; set; }
    }
}