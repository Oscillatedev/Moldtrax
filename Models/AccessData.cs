using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class AccessData
    {
        public int ID { get; set; }
        public int ObjectType { get; set; }
        public string ControlTypeName { get; set; }
        public string Permission { get; set; }
        public int Level { get; set; }
        public string ControlTip { get; set; }
        [Column("No Access")]
        public int NoAccess { get; set; }
        public int AllowEdits { get; set; }
        public int AllowDeletions { get; set; }
        public int AllowAdditions { get; set; }
        public int DataEntry { get; set; }
        public int Visible { get; set; }
        public int Enabled { get; set; }
        public int Locked { get; set; }
        public DateTime OperatorStamp { get; set; }
        public DateTime DateTimeStamp { get; set; }
        public string SSMA_TimeStamp { get; set; }

    }

    public class LogTable
    {
        public string TableName { get; set; }
        public string TabName { get; set; }
        public string ColName { get; set; }
        public string NewColVal { get; set; }
        public string OldColVal { get; set; }
        public int DataKey { get; set; }
        public string Action { get; set; }
        public string PageName { get; set; }
        public string LabelName { get; set; }
    }


    public enum GetPageName
    {
        [Description("Detail Mold Info")]
        DetailMoldInfo,

        [Description("Maintenance Tracking")]
        MaintenanceTracking,

        [Description("Master Schedule")]
        MasterSchedule,

        [Description("Company Information")]
        CompanyInformation,

        [Description("Admin Security Manager")]
        AdminSecurityManager,

        [Description("Admin Detail MoldList")]
        AdminDetailMoldList,

        [Description("Admin Maintenance Tracking List")]
        AdminMaintenanceTrackingList,

        [Description("Admin Checksheet Category")]
        AdminChecksheetCategory,

        [Description("Admin Checksheet Item")]
        AdminChecksheetItem,

        [Description("Mold Performance Dashboard")]
        MoldPerformanceDashboard,

        [Description("Maintenance Efficiency Dashboard")]
        MaintenanceEfficiencyDashboard,

        [Description("PM Alert Report")]
        PMAlertReport,

        [Description("Reports")]
        Reports,

        [Description("Checksheet")]
        Checksheet,

        Login

    }

    public enum GetTabName
    {
        [Description("Mold Details")]
        MoldDetails,
        Tooling,

        [Description("Cavity Layout")]
        CavityLayout,

        [Description("IML Map")]
        IMLMap,

        [Description("Troubleshoot Guide")]
        TroubleshootGuide,

        [Description("TechTips")]
        TechTips,
        Notes,
        Servicing,
        PressData,

        [Description("Maintenance Schedule")]
        MaintenanceSchedule,

        [Description("Maintenance Instructions")]
        MaintenanceInstructions,

        [Description("Defect Task")]
        DefectTask,

        [Description("Corrective Action Made")]
        CorrectiveActionMade,

        [Description("Action Review")]
        ActionReview,

        [Description("Master Schedule")]
        MasterSchedule,

        [Description("Managing Company")]
        ManagingCompany,

        Customer,
        Vendors,
        Employees,

        [Description("Document Control Num")]
        DocumentControlNum,

        [Description("Security Manager")]
        SecurityManager,

        [Description("BaseStyle Type")]
        BaseStyleType,

        Department,

        [Description("Product Line")]
        ProductLine,

        [Description("Product Part")]
        ProductPart,

        [Description("Resin Type")]
        ResinType,

        [Description("Runner Type")]
        RunnerType,

        [Description("MoldTooling Type")]
        MoldToolingType,

        [Description("TSGuide DefectType")]
        TSGuideDefectType,

        [Description("TechTips Link")]
        TechTipsLink,

        Factors,

        [Description("Mold Configuration")]
        MoldConfiguration,

        [Description("Mold Configuration2")]
        MoldConfiguration2,

        [Description("Stop Reason")]
        StopReason,

        [Description("Corrective Action Type")]
        CorrectiveActionType,

        [Description("Corrective Action")]
        CorrectiveAction,

        [Description("Maintenance Schedule Status")]
        MaintenanceScheduleStatus,

        [Description("Repair Status")]
        RepairStatus,

        [Description("Repair Location")]
        RepairLocation,

        [Description("CheckSheet Category")]
        CheckSheetCategory,

        [Description("Checksheet Item")]
        ChecksheetItem,

        [Description("MoldPerformance Dashboard")]
        MoldPerformanceDashboard,

        [Description("Maintenance Efficiency Dashboard")]
        MaintenanceEfficiencyDashboard,

        [Description("Maintenance AlertStats")]
        MaintenanceAlertStats,

        User,

        [Description("RepairSheet Report")]
        RepairSheetReport,

        [Description("Maintenance Timeline Report")]
        MaintenanceTimelineReport,

        [Description("DefectCostAnalysis Report")]
        DefectCostAnalysisReport,

        [Description("Defect Tracking Report")]
        DefectTrackingReport,

        [Description("Total TimeRun Report")]
        TotalTimeRunReport,

        [Description("IMLSheet Report")]
        IMLSheetReport,

        [Description("TroubleShooter Guide Report")]
        TroubleShooterGuideReport,

        [Description("TechTips Report")]
        TechTipsReport,

        [Description("List of Mold Tooling")]
        ListOFMoldTooling,

        [Description("Last Shot Inspection")]
        LastShotInspection,

        Organization
            
    }


    public enum GetAction
    {
        Create,
        Update,
        Delete,
        Showinglist,
        Login,
        Success,
        Unsuccess,
        Copy,
        Paste

    }


    public enum GetDBTableName
    {
        tblDocs,
        tblEmployees,
        tblMaintAlertPercentages2,
        MaintenanceTracking2,
        tblMaintAlertPercentages,
        MainRoles,
        tblMaintenanceTracking,
        tblMoldData,
        tblOrganisation,
        tblMoldDataNotes,
        tblMoldTooling,
        tblRepairFix,
        eza_Preferences,
        ezy_AuditLog,
        ezy_GroupPermissions,
        tblSchedule,
        ezy_Groups,
        tblSelectedMold,
        ezy_GroupUser,
        tblTechTips,
        ezy_Permissions,
        tblTSGuide,
        tblVendors,
        tempImage,
        Testing,
        ezy_Users,
        tblCavityLocation,
        tblCavityNumber,
        tblInspectItems,
        tblInspectionsTemp,
        tblComboSettings,
        tblInspections,
        tblCompany,
        tblInspectionDetails,
        tblCorrectiveAction,
        tblCategoryItem,
        tblCategory,
        tblCustomer,
        tblRoverSetData,
        tblDDCustomerID,
        tblDDDepartmentID,
        tblDDDfctCavNum,
        tblDDDfctDescript,
        tblDDDfctID,
        tblDDDocSection,
        tblDDEmployeeID,
        tblDDMldPullTech,
        tblDDMldRepairdBy,
        tblDDMoldCategoryID,
        tblDDMoldCav,
        tblDDMoldConfig,
        tblDDMoldConfig2,
        tblDDMoldResinType,
        tblMoldChart,
        tblDDMoldToolingType,
        FastraxLastDate,
        tblDDProductLine,
        tblDDProductPart,
        tblDDRepairStatus,
        tblDDRepairStatusLocation,
        tblDDschMoldDataID,
        tblDDFactors,
        tblDDschStatus,
        tblDDSetTech,
        tblDDStopReason,
        tblDDTIType,
        tblDDTlCorrectiveAction,
        tblDDTlReplTooling,
        tblDDTlTechnician,
        tblDDTSType,
        tblDfctBlockOff
    }

}