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
    
    public partial class DSC_OBS_DEV_Server : DbContext
    {
        public DSC_OBS_DEV_Server()
            : base("name=DSC_OBS_DEV_Server")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<DSC_CUSTOMER> DSC_CUSTOMER { get; set; }
        public virtual DbSet<DSC_LC> DSC_LC { get; set; }
    }
}
