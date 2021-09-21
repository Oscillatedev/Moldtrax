using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class MoldtraxDbContext : DbContext
    {
        public MoldtraxDbContext() : base("DefaultConnection")
        {
            var objectContext = (this as IObjectContextAdapter).ObjectContext;
            objectContext.CommandTimeout = 0;
        }

        //public MoldtraxDbContext() : base()
        //{
        //    //var UserID = Microsoft.AspNet.Identity.User
        //    ApplicationUser User = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());

        //    string DbName = "";
        //    this.Database.Connection.ConnectionString = "data source=.;initial catalog="+DbName+"; integrated security=true; MultipleActiveResultSets=true;";
        //}

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<ezy_Users> Ezy_Users { get; set; }
        public DbSet<tblMoldData> TblMoldData { get; set; }
        public DbSet<tblDDMoldCategoryID> TblDDMoldCategoryID { get; set; }
        public DbSet<tblDDDepartmentID> TblDDDepartmentID { get; set; }
        public DbSet<tblDDProductLine> TblDDProductLine { get; set; }
        public DbSet<tblDDProductPart> TblDDProductPart { get; set; }
        public DbSet<tblDDMoldResinType> TblDDMoldResinType { get; set; }
        public DbSet<tblDDMoldCav> TblDDMoldCav { get; set; }
        public DbSet<tblCustomer> TblCustomer { get; set; }
        public DbSet<tblDDTSType> TblDDTSType { get; set; }
        public DbSet<tblTSGuide> TblTSGuide { get; set; }
        public DbSet<tblDDMoldToolingType> TblDDMoldToolingTypes { get; set; }
        public DbSet<tblMoldTooling> TblMoldTooling { get; set; }
        public DbSet<tblMoldDataNotes> TblMoldDataNotes { get; set; }
        public DbSet<tblTechTips> TblTechTips { get; set; }
        public DbSet<tblDocs> TblDocs { get; set; }
        public DbSet<tblDDDocSection> TblDDDocSections { get; set; }
        public DbSet<tblCavityLocation> TblCavityLocations { get; set; }
        public DbSet<tblCavityNumber> TblCavityNumbers { get; set; }

        public DbSet<tblRoverSetData> TblRoverSetDatas { get; set; }
        public DbSet<tblDDMoldConfig> TblDDMoldConfigs { get; set; }
        public DbSet<tblDDMoldConfig2> TblDDMoldConfig2s { get; set; }
        //public DbSet<RepairStatusDropdown> RepairStatusDropdowns { get; set; }
        //public DbSet<MoldLocationDropdown> MoldLocationDropdowns { get; set; }
        public DbSet<tblSchedule> TblSchedules { get; set; }
        public DbSet<tblDfctBlockOff> TblDfctBlockOffs { get; set; }

        public DbSet<tblCorrectiveAction> TblCorrectiveActions { get; set; }
        public DbSet<tblDDTIType> TblDDTITypes { get; set; }

        public DbSet<tblDDTlCorrectiveAction> TblDDTlCorrectiveActions { get; set; }

        public DbSet<tblCompany> TblCompanies { get; set; }
        public DbSet<tblVendors> TblVendors { get;set;}
        public DbSet<tblEmployees> TblEmployees { get; set; }
        public DbSet<ezy_groups> Ezy_Groups { get; set; }
        public DbSet<ezy_groupuser> Ezy_Groupusers { get; set; }

        public DbSet<tblddStopReason> TblddStopReasons { get; set; }
        public DbSet<tblDDschStatus> TblDDschStatuses { get; set; }
        public DbSet<tblDDRepairStatus> TblDDRepairStatuses { get; set; }
        public DbSet<tblDDRepairStatusLocation> TblDDRepairStatusLocations { get; set; }
        public DbSet<tblMaintAlertPercentages> TblMaintAlertPercentages { get; set; }
        public DbSet<tblDDFactors> TblDDFactors { get; set; }
        public DbSet<tblInspections> TblInspections { get; set; }
        //public DbSet<tblInspectionsTemp> TblInspectionsTemps { get; set; }
        public DbSet<tblInspectItems> TblInspectItems { get; set; }
        public DbSet<tblInspectionDetails> TblInspectionDetails { get; set; }
        public DbSet<tblRepairFix> TblRepairFixes { get; set; }
        public DbSet<tblCategory> TblCategories { get; set; }
        public DbSet<tblMoldChart> TblMoldCharts { get; set; }
        public DbSet<FastraxLastDate> FastraxLastDates { get; set; }
        public DbSet<tblOrganisation> TblOrganisations { get; set; }
        public DbSet<EzyAuditLog> EzyAuditLogs { get; set; }
    }
}