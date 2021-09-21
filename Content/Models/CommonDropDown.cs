using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Moldtrax.Models
{
    public class CommonDropDown
    {

    }

    public class TotalCycleCount
    {
        public int? MoldDataID { get; set; }
        public double? TotalCycles { get; set; }
    }

    public class RepairStatusDropdown
    {
        public int ID { get; set; }
        public string RepairStatus { get; set; }
        public string RepairStatusDescrip { get; set; }
    }

    public class MoldLocationDropdown
    {
        public int ID { get; set; }
        public string RepairStatusLocation { get; set; }
        public string RepairStatusLocationDescrip { get; set; }
    }

    public class SchStatusDropDown
    {
        public int ID { get; set; }
        public string schStatus { get; set; }
        public string schStatusDesc { get; set; }
    }

    public class DfctDescriptionDropDoWN
    {
        public int TSGuide { get; set; }
        public int? TSSeqNum { get; set; }
        public string Type { get; set; }
        public string TSExplanation { get; set; }
        public int MoldDataID { get; set; }
    }

    public class CavNumDropdown
    {
        public int CavityNumberID { get; set; }
        public bool CavityActive { get; set; }
        [Column("Cav ID")]
        public string CavID { get; set; }

        public string Pos { get; set; }
        public string Act { get; set; }
        public int MoldDataID { get; set; }

    }

    public class tblDDTIType
    {
        public int ID { get; set; }
        public string TIType { get; set; }
        public string TITypeDesc { get; set; }
        public int? CompanyID { get; set; }

    }

    public class tblDDTlCorrectiveAction
    {
        public int ID { get; set; }
        public string TlCorrectiveAction { get; set; }
        public string TlCorrectiveActionDesc { get; set; }
        public int? CompanyID { get; set; }

    }

    public class tblddStopReason
    {
        [Key]
        public int ID { get; set; }
        public string StopReason { get; set; }
        public string StopReasonDesc { get; set; }
        public int? CompanyID { get; set; }

    }

    public class tblDDschStatus
    {
        [Key]
        public int ID { get; set; }
        public string schStatus { get; set; }
        public string schStatusDesc { get; set; }
        public int? CompanyID { get; set; }

    }


    public class tblDDRepairStatus
    {
        [Key]
        public int ID { get; set; }
        public string RepairStatus { get; set; }
        public string RepairStatusDescrip { get; set; }
        public int? CompanyID { get; set; }

    }

    public class tblDDRepairStatusLocation
    {
        [Key]
        public int ID { get; set; }
        public string RepairStatusLocation { get; set; }
        public string RepairStatusLocationDescrip { get; set; }
        public int? CompanyID { get; set; }

    }

    public class MaintenanceScheduleCommon
    {
        public tblMoldData MoldData { get; set; }
        public List<tblSchedule> TbScheduleList { get; set; }
    }

    public class MoldDropdown
    {
        public int MoldDataID { get; set; }
        public string Name { get; set; }
    }


    public class DataPoint
    {
        public string Name { get; set; }
        public double? Y { get; set; }
    }

    public class ToolingMaintenanceInstruction
    {
        public int MoldToolingID { get; set; }
        public string MoldToolDescrip { get; set; }
        public string MoldToolingPartNumber { get; set; }
        public string POH { get; set; }

    }

    public class GetCavDropdown
    {
        public string Text { get; set; }
        public string Value { get; set; }
        public string Name { get; set; }
        public string POH { get; set; }

    }

}