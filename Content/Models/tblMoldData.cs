using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Moldtrax.Models
{
    public class tblMoldData
    {
        [Key]
        public int MoldDataID { get; set; }
        public int? CompanyID { get; set; }
        public int? CustomerID { get; set; }
        public string MoldName { get; set; }
        public string MoldDesc { get; set; }
        public string MoldGroupName { get; set; }
        public int? CavityRows { get; set; }
        public int? CavityColmns { get; set; }
        public int? CavityTotal { get; set; }
        public string ProductLine { get; set; }
        public string ProductPart { get; set; }
        public string MoldCategoryID { get; set; }
        public string RepairStatusID { get; set; }
        public string DepartmentID { get; set; }
        public int? VendorID { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string ModelNumber { get; set; }
        public string SerialNumber { get; set; }
        public string BarcodeNumber { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? DateAcquired { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? DateRetired { get; set; }

        public decimal? PurchasePrice { get; set; }
        public decimal? CurrentValue { get; set; }

        [AllowHtml]
        public string Comments { get; set; }
        public DateTime? NextSchedMaint { get; set; }
        public string MoldConfig { get; set; }
        public string MoldResinType { get; set; }
        public string MoldResinVendor { get; set; }
        public string MoldResinVendorPhone { get; set; }
        public DateTime? MoldCalInspect { get; set; }
        public DateTime? MoldNextDueDate { get; set; }
        public DateTime? MoldLastToolingStack { get; set; }
        public string MOLDPMFreq { get; set; }
        public string MoldCav { get; set; }
        public string MoldProjEngFirstName { get; set; }
        public string MoldProjEngLastName { get; set; }
        public string MoldProjEngPhone { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? MoldDateBuilt { get; set; }
        public string MoldRunnerSize { get; set; }
        public string MoldNozzleSize { get; set; }
        public string MoldSprueSize { get; set; }
        public string MoldGateSize { get; set; }
        public string CustomerComments { get; set; }

        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public double? MoldCyclesPerMinute { get; set; }
        public string MoldService01 { get; set; }
        public string MoldService02 { get; set; }
        public string MoldService03 { get; set; }
        public string MoldService04 { get; set; }
        public string MoldService05 { get; set; }
        public string MoldService06 { get; set; }
        public string MoldService07 { get; set; }
        public string MoldService08 { get; set; }
        public string MoldService09 { get; set; }
        public string MoldService10 { get; set; }
        public string MoldDefect01 { get; set; }
        public string MoldDefect02 { get; set; }
        public string MoldDefect03 { get; set; }
        public string MoldDefect04 { get; set; }
        public string MoldDefect05 { get; set; }
        public string MoldDefect06 { get; set; }
        public string MoldDefect07 { get; set; }
        public string MoldDefect08 { get; set; }
        public string MoldDefect09 { get; set; }
        public string MoldDefect10 { get; set; }
        public string MoldDefectComment01 { get; set; }
        public string MoldReasonPulled01 { get; set; }
        public string MoldReasonPulled02 { get; set; }
        public string MoldReasonPulled03 { get; set; }
        public string MoldReasonPulled04 { get; set; }
        public string MoldReasonPulled05 { get; set; }
        public string MoldReasonPulled06 { get; set; }
        public string MoldReasonPulled07 { get; set; }
        public string MoldReasonPulled08 { get; set; }
        public string MoldReasonPulled09 { get; set; }
        public string MoldReasonPulled10 { get; set; }
        public string MoldReasonPulledComment1 { get; set; }
        public string MoldInPressPMFreq { get; set; }
        public int? MoldOutPressPMFreq { get; set; }
        public int? MoldOutPressPMYellowCycles { get; set; }
        public int? MoldOutPressPMRedCycles { get; set; }
        public string RepairStatus { get; set; }
        //public char MoldMap { get; set; }
        public string CounterView { get; set; }
        public int? MoldTotalCycles { get; set; }
        public string RunStatusColor { get; set; }
        public int? MoldCyclesToYellowLimit { get; set; }
        public int? MoldCyclesToRedLimit { get; set; }
        public int? MoldCyclesOverRedLimit { get; set; }
        public int? RepairStatusLocationId { get; set; }
        //public byte[] SSMA_TimeStamp { get; set; }
        public string ImageExtension { get; set; }
        public int? MoldOutPressPMFreqRed { get; set; }
        public double? DoD { get; set; }
        public string PlasticFactor { get; set; }
        public string SteelFactor { get; set; }
        public string LocationFactor { get; set; }
        public double? PF { get; set; }
        public double? SF { get; set; }
        public double? LF { get; set; }
        public string MoldMapPath { get; set; }
        public double? TotalShots { get; set; }
    }

    public class MainMoldData
    {
        public IEnumerable<tblMoldData> TBLList { get; set; }
        public tblMoldData TblDetails { get; set; }
    }
}