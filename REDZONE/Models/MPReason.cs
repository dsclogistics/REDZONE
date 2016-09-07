using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REDZONE.Models
{
    public class MPReason
    {
        //Model for Metric Period Reasons
        [Display(Name = "Reason ID")]
        public string reason_id { get; set; }
        [Display(Name = "Metric Period")]
        public string metric_period_id { get; set; }
        [Display(Name = "Reason Text")]
        public string reason_text { get; set; }
        [Display(Name = "Order")]
        public string reason_order { get; set; }
        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string reason_description { get; set; }
        [Display(Name = "Standardized (Y/N)")]
        public string reason_std_yn { get; set; }
        //public string reason_added_on_dtm { get; set; }
        //public string reason_added_by_uid { get; set; }
        [Display(Name = "Metric Values Used On")]
        public string times_used { get; set; }
    }
}