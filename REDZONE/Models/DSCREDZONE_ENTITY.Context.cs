﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace REDZONE.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DSC_MTRC_DEV_Entities : DbContext
    {
        public DSC_MTRC_DEV_Entities()
            : base("name=DSC_MTRC_DEV_Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<DSC_CUST_ACCT> DSC_CUST_ACCT { get; set; }
        public virtual DbSet<DSC_LC> DSC_LC { get; set; }
        public virtual DbSet<MTRC_DATA_SRC> MTRC_DATA_SRC { get; set; }
        public virtual DbSet<MTRC_DATA_TYPE> MTRC_DATA_TYPE { get; set; }
        public virtual DbSet<MTRC_METRIC> MTRC_METRIC { get; set; }
        public virtual DbSet<MTRC_METRIC_HIERARCHY> MTRC_METRIC_HIERARCHY { get; set; }
        public virtual DbSet<MTRC_METRIC_PRODUCTS> MTRC_METRIC_PRODUCTS { get; set; }
        public virtual DbSet<MTRC_PRODUCT> MTRC_PRODUCT { get; set; }
        public virtual DbSet<MTRC_TIME_PERIOD_TYPE> MTRC_TIME_PERIOD_TYPE { get; set; }
        public virtual DbSet<DSC_MTRC_LC_BLDG> DSC_MTRC_LC_BLDG { get; set; }
        public virtual DbSet<MTRC_METRIC_PERIOD> MTRC_METRIC_PERIOD { get; set; }
        public virtual DbSet<MTRC_MGMT_AUTH> MTRC_MGMT_AUTH { get; set; }
    }
}
