using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class StartDate
    {
        public int MoldDataID { get; set; }
        public int SetID { get; set; }
        public string MoldName { get; set; }
        public string MoldDesc { get; set; }
        public DateTime SetDate { get; set; }
        public DateTime SetTime { get; set; }
        public string SetPressNumb { get; set; }
        public string Tech { get; set; }
        public DateTime MldPullDate { get; set; }
        public DateTime MldPullTime { get; set; }
    }
}