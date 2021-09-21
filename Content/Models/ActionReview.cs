using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class ActionReview
    {
        public int? TlRepllD { get; set; }
        public int? CavityNumberID { get; set; }
        public string CavityNumber { get; set; }
        public int? SetID { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? SetDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? DfctDate { get; set; }
        public string TSDefects { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? TSDate { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? TlSTime { get; set; }
        public int? DfctCavNum { get; set; }
        public string MoldToolDescrip { get; set; }
        public string TlCorrectiveAction { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public double? TIRepairTime { get; set; }
        //public string Machine { get; set; }

    }
}