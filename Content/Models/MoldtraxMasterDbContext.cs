using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace Moldtrax.Models
{
    public class MoldtraxMasterDbContext : DbContext
    {
        public MoldtraxMasterDbContext() : base("MasterDefaultConnection")
        {
            var objectContext = (this as IObjectContextAdapter).ObjectContext;
            objectContext.CommandTimeout = 0;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<MoldtraxMasterDbContext>(null);
            //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<MasterMoldtraxUser> Users { get; set; }
        public DbSet<MasterUser> MasterUsers { get; set; }
    }
}