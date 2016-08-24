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
    
    public partial class MTRC_METRIC_PRODUCTS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MTRC_METRIC_PRODUCTS()
        {
            this.MTRC_MGMT_AUTH = new HashSet<MTRC_MGMT_AUTH>();
        }
    
        public int mtrc_prod_id { get; set; }
        public short prod_id { get; set; }
        public int mtrc_period_id { get; set; }
        public string mtrc_prod_top_lvl_parent_yn { get; set; }
        public string mtrc_prod_display_text { get; set; }
        public Nullable<short> mtrc_prod_display_order { get; set; }
        public System.DateTime mtrc_prod_eff_start_dt { get; set; }
        public System.DateTime mtrc_prod_eff_end_dt { get; set; }
    
        public virtual MTRC_PRODUCT MTRC_PRODUCT { get; set; }
        public virtual MTRC_METRIC_PERIOD MTRC_METRIC_PERIOD { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MTRC_MGMT_AUTH> MTRC_MGMT_AUTH { get; set; }
    }
}
