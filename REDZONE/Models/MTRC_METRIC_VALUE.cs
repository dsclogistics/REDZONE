//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class MTRC_METRIC_VALUE
    {
        public long mtrc_val_id { get; set; }
        public int mtrc_period_id { get; set; }
        public short dsc_lc_bldg_id { get; set; }
        public string mtrc_val_value { get; set; }
        public System.DateTime mtrc_val_dtm_start { get; set; }
        public System.DateTime mtrc_val_dtm_end { get; set; }
        public string mtrc_value_period_desc { get; set; }
    
        public virtual DSC_LC_BLDG DSC_LC_BLDG { get; set; }
        public virtual MTRC_METRIC_PERIOD MTRC_METRIC_PERIOD { get; set; }
    }
}
