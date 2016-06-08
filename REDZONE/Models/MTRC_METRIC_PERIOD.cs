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
    
    public partial class MTRC_METRIC_PERIOD
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MTRC_METRIC_PERIOD()
        {
            this.MTRC_BLDG_DATA_SRC = new HashSet<MTRC_BLDG_DATA_SRC>();
            this.MTRC_METRIC_HIERARCHY = new HashSet<MTRC_METRIC_HIERARCHY>();
            this.MTRC_METRIC_HIERARCHY1 = new HashSet<MTRC_METRIC_HIERARCHY>();
            this.MTRC_METRIC_VALUE = new HashSet<MTRC_METRIC_VALUE>();
            this.MTRC_METRIC_PRODUCTS = new HashSet<MTRC_METRIC_PRODUCTS>();
        }
    
        public int mtrc_period_id { get; set; }
        public int mtrc_id { get; set; }
        public short tpt_id { get; set; }
        public string mtrc_period_name { get; set; }
        public string mtrc_period_desc { get; set; }
        public string mtrc_period_calc_yn { get; set; }
        public string mtrc_period_manual_yn { get; set; }
        public Nullable<decimal> mtrc_period_min_val { get; set; }
        public Nullable<decimal> mtrc_period_max_val { get; set; }
        public Nullable<short> mtrc_period_max_dec_places { get; set; }
        public Nullable<short> mtrc_period_max_str_size { get; set; }
        public string mtrc_period_na_allow_yn { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MTRC_BLDG_DATA_SRC> MTRC_BLDG_DATA_SRC { get; set; }
        public virtual MTRC_METRIC MTRC_METRIC { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MTRC_METRIC_HIERARCHY> MTRC_METRIC_HIERARCHY { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MTRC_METRIC_HIERARCHY> MTRC_METRIC_HIERARCHY1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MTRC_METRIC_VALUE> MTRC_METRIC_VALUE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MTRC_METRIC_PRODUCTS> MTRC_METRIC_PRODUCTS { get; set; }
        public virtual MTRC_TIME_PERIOD_TYPE MTRC_TIME_PERIOD_TYPE { get; set; }
    }
}