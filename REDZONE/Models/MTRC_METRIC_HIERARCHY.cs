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
    
    public partial class MTRC_METRIC_HIERARCHY
    {
        public int mtrc_hier_id { get; set; }
        public int mtrc_period_id_parent { get; set; }
        public int mtrc_period_id_child { get; set; }
        public string mtrc_hier_type { get; set; }
    
        public virtual MTRC_METRIC_PERIOD MTRC_METRIC_PERIOD { get; set; }
        public virtual MTRC_METRIC_PERIOD MTRC_METRIC_PERIOD1 { get; set; }
    }
}
