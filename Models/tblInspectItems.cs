using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class tblInspectItems
    {
        [Key]
        public int InspectionID { get; set; }
        public String InspectionName { get; set; }
        public int? MoldID { get; set; }
        public int CatID { get; set; }
        public int? CompanyID { get; set; }

    }

    public class tblInspectItemsViewModel
    {
        public int InspectionID { get; set; }
        public String InspectionName { get; set; }
        public int CatID { get; set; }
        public int MoldID { get; set; }
        public string CateName { get; set; }
    }

    //public class tblInspectionsTemp
    //{
    //    public int InspectID1 { get; set; }
    //    public int InspectID2 { get; set; }
    //    public int InspectID3 { get; set; }
    //    public int MoldDataID { get; set; }
    //    public int InspectedBy{ get; set; }
    //    public DateTime Date1 { get; set; }
    //    public DateTime Date2 { get; set; }
    //    public DateTime Date3 { get; set; }
    //    public string AdditionalMaintenance1 { get; set; }
    //    public string AdditionalMaintenance2 { get; set; }
    //    public string AdditionalMaintenance3 { get; set; }
    //}

    public class tblInspections
    {
        [Key]
        public int InspectID { get; set; }
        public int? MoldDataID { get; set; }
        public int? InspectedBy { get; set; }
        public DateTime? InspectDate { get; set; }
        public string AdditionalMaintenance { get; set; }
        public int? CompanyID { get; set; }

    }

    public class tblInspectionDetails
    {
        [Key]
        public int ID { get; set; }
        public int InspectID { get; set; }
        public int InspectionID { get; set; }
        public bool OK { get; set; }
        public bool Attention { get; set; }
        public bool NoRun { get; set; }
        public string AdditionalComments { get; set; }
        public int MoldNo { get; set; }
        public int? CompanyID { get; set; }

    }

    public class FinalChecklstResult
    {
        public int CatID { get; set; }
        public string CategoryName { get; set; }
        public string InspectionName { get; set; }
        public int InspectionDetailID1 { get; set; }
        public int InspectionDetailID2 { get; set; }
        public int InspectionDetailID3 { get; set; }

        public bool Ok1 { get; set; }
        public bool Ok2 { get; set; }
        public bool Ok3 { get; set; }
        public bool Attention1 { get; set; }
        public bool Attention2 { get; set; }
        public bool Attention3 { get; set; }
        public bool NoRun1 { get; set; }
        public bool NoRun2 { get; set; }
        public bool NoRun3 { get; set; }
        public DateTime Date1 { get; set; }
        public DateTime Date2 { get; set; }
        public DateTime Date3 { get; set; }
        public string AdditionalComments1 { get; set; }
        public string AdditionalComments2 { get; set; }
        public string AdditionalComments3 { get; set; }

        public int InspectedBy1 { get; set; }
        public int InspectedBy2 { get; set; }
        public int InspectedBy3 { get; set; }

        public int InspectID1 { get; set; }
        public int InspectID2 { get; set; }
        public int InspectID3 { get; set; }

    }
}