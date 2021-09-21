using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Moldtrax.Models
{
    public class tblTechTips
    {
        [Key]
        public int TechTipsID { get; set; }
        public int MoldDataID { get; set; }

        [AllowHtml]
        public string TTToolKit { get; set; }
        public string TTFlowPaths { get; set; }

        //public varbinary TTMoldImage { get; set; }
        //public varbinary TTPartImage { get; set; }

        public string TTMoldImagePath { get; set; }
        public string TTPartImagePath { get; set; }

        [AllowHtml]
        public string TTDisassmbly { get; set; }
        [AllowHtml]
        public string TTClean { get; set; }
        [AllowHtml]
        public string TTAssmbly { get; set; }
        [AllowHtml]
        public string TTFinalChk { get; set; }
        [AllowHtml]
        public string TTPolishing { get; set; }
        [AllowHtml]
        public string TTInspection { get; set; }
        public string MCDimensions { get; set; }
        public string MCHeight { get; set; }
        public string MCWidth { get; set; }
        public string MCDepth { get; set; }
        public string MCWeight { get; set; }
        public string MCWidthOpen { get; set; }
        public string MCTotalHeight { get; set; }
        public string MCEjectorStroke { get; set; }
        public string TTHRSystem { get; set; }
        public string TTHRSerialNumber { get; set; }
        public string TTHRType { get; set; }
        public string TTHRActuation { get; set; }
        public string TTHRMaxOperatTemp { get; set; }
        public string TTHRProbeType { get; set; }
        public string TTHRProbeHeater { get; set; }
        public string TTHRProbeHeaterThermoType { get; set; }
        public string TTHRManifoldHeater { get; set; }
        public string TTHRManifoldHeaterThermoType { get; set; }
        public string TTHROpenPressureMax { get; set; }
        public string TTHROpenPressureTypical { get; set; }
        public string TTHRClosePressureMax { get; set; }
        public string TTHRClosePressureTypical { get; set; }
        public string TTHRNumberZones { get; set; }
        public string TTHRNumberDrops { get; set; }
        [AllowHtml]
        public string TTHRNotes { get; set; }
        [AllowHtml]
        public string TTHRDisassembly { get; set; }
        [AllowHtml]
        public string TTHRClean { get; set; }
        [AllowHtml]
        public string TTHRAssembly { get; set; }
        [AllowHtml]
        public string TTHRFinalChk { get; set; }
        [AllowHtml]
        public string TTHRPolishing { get; set; }
        [AllowHtml]
        public string TTHRToolKit { get; set; }
        public string TTHRController { get; set; }
        public string TTHRProgramNumber { get; set; }
        public string TTHRClampPlateBoltTorque { get; set; }
        public bool TTHRDropsServicableInPress { get; set; }
        public string ImageExtension { get; set; }
        public string BridgeHeater { get; set; }
        public string BridgeThermocouple { get; set; }
        public string SprueHeater { get; set; }
        public string SprueThermocouple { get; set; }

        public int? CompanyID { get; set; }



    }
}