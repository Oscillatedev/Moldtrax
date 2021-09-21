using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class tblCavityNumber
    {
        [Key]
        public int CavityNumberID { get; set; }
        public int? CavityLocationID { get; set; }
        public bool CavityActive { get; set; }
        public string CavityNumber { get; set; }
        public DateTime? CavityDateInstalled { get; set; }
        public DateTime? CavityDateRemoved { get; set; }
        public string CavityNotes { get; set; }
        public int? CompanyID { get; set; }
    }

    public class tblCavityLocation
    {
        [Key]
        public int CavityLocationID { get; set; }
        public int MoldDataID { get; set; }
        public string CavityLocationNumber { get; set; }
        public int? CompanyID { get; set; }
    }

    public class LayoutData
    {
        public List<tblCavityNumber> tblCavityNumber { get; set; }
        public List<tblCavityLocation> tblCavityLocations { get; set; }
    }
}