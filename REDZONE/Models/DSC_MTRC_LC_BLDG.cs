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
    
    public partial class DSC_MTRC_LC_BLDG
    {
        public short dsc_mtrc_lc_bldg_id { get; set; }
        public int dsc_lc_id { get; set; }
        public string dsc_mtrc_lc_bldg_name { get; set; }
        public string dsc_mtrc_lc_bldg_code { get; set; }
        public System.DateTime dsc_mtrc_lc_bldg_eff_start_dt { get; set; }
        public System.DateTime dsc_mtrc_lc_bldg_eff_end_dt { get; set; }
    
        public virtual DSC_LC DSC_LC { get; set; }
    }
}
