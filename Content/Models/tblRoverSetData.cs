using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Moldtrax.Models
{
    public class tblRoverSetData
    {
        [Key]
        public int SetID { get; set; }
        public int MoldDataID { get; set; }
        public DateTime? SetDate { get; set; }
        public DateTime? SetTime { get; set; }
        public int? SetTech { get; set; }
        public string SetPressNumb { get; set; }
        [AllowHtml]
        public string MldSetPullNotes { get; set; }
        public string MldPullMaintRequired { get; set; }
        public DateTime? MldPullDate { get; set; }
        public DateTime? MldPullTime { get; set; }
        public int? MldPullTech { get; set; }
        public string MoldConfig { get; set; }
        public Byte[] MoldDefectMap { get; set; }
        public DateTime? MldRepairedDate { get; set; }
        public double? MldRepairedTime { get; set; }
        public int? MldRepairdBy { get; set; }
        public string MldWorkOrder { get; set; }
        public int? MldProductionCycles { get; set; }
        [AllowHtml]
        public string MldRepairComments { get; set; }
        public string ImageExtension { get; set; }
        public int? CycleCounter { get; set; }
        public string MoldConfig2 { get; set; }
        public string MoldDefectMapPath { get; set; }
        public int? CompanyID { get; set; }

        [NotMapped]
        public string NewSetTime { get; set; }
        [NotMapped]
        public string NewMldPullTime { get; set; }
        [NotMapped]
        public string NewSetDate { get; set; }
        [NotMapped]
        public string NewMldPullDate { get; set; }

    }
}