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
    
    public partial class MTRC_BLDG_DATA_SRC
    {
        public int bldg_data_src_id { get; set; }
        public Nullable<short> data_src_id { get; set; }
        public Nullable<short> dsc_lc_bldg_id { get; set; }
        public int mtrc_period_id { get; set; }
    
        public virtual DSC_MTRC_LC_BLDG DSC_MTRC_LC_BLDG { get; set; }
        public virtual MTRC_DATA_SRC MTRC_DATA_SRC { get; set; }
        public virtual MTRC_METRIC_PERIOD MTRC_METRIC_PERIOD { get; set; }
    }
}
