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
    
    public partial class DSC_LC_BLDG
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DSC_LC_BLDG()
        {
            this.MTRC_METRIC_VALUE = new HashSet<MTRC_METRIC_VALUE>();
            this.MTRC_BLDG_DATA_SRC = new HashSet<MTRC_BLDG_DATA_SRC>();
        }
    
        public short dsc_mtrc_lc_bldg_id { get; set; }
        public int dsc_lc_id { get; set; }
        public string dsc_mtrc_lc_bldg_name { get; set; }
        public string dsc_mtrc_lc_bldg_code { get; set; }
        public System.DateTime dsc_mtrc_lc_bldg_eff_start_dt { get; set; }
        public System.DateTime dsc_mtrc_lc_bldg_eff_end_dt { get; set; }
    
        public virtual DSC_LC DSC_LC { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MTRC_METRIC_VALUE> MTRC_METRIC_VALUE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MTRC_BLDG_DATA_SRC> MTRC_BLDG_DATA_SRC { get; set; }
    }
}
