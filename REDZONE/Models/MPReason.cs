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
        [Display(Name = "Metric Value Reason ID")]
        public string val_reason_id { get; set; }
        [Display(Name = "Metric Period")]
        public string metric_period_id { get; set; }

        [Display(Name = "Reason Text")]
        public string reason_text { get; set; }

        [Display(Name = "Order")]
        public string reason_order { get; set; }

        [Display(Name = "Order")]
        public int reason_order_int { get; set; }

        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string reason_description { get; set; }

        [Display(Name = "Standardized (Y/N)")]
        public string reason_std_yn { get; set; }
        
        [Display(Name = "Created On")]
        public string reason_created_on_dtm { get; set; }

        [Display(Name = "Created By")]
        public string reason_created_by_uid { get; set; }

        [Display(Name = "Standardized On")]
        public string reason_stdized_on_dtm { get; set; }

        [Display(Name = "Standardized By")]
        public string reason_stdized_by_uid { get; set; }

        [Display(Name = "Metric Values Used On")]
        public string times_used { get; set; }

        public bool isAssigned { get; set; }

        [Display(Name = "Assigned Reason Comment")]
        public string mpvr_Comment { get; set; }
    }

    public class MPReasonViewModel
    {
        public string month { get; set; }
        public string monthName { get; set; }
        public string year { get; set; }
        public string bldgName { get; set; }
        public string bldgId { get; set; }
        public string mtrc_prod_display_text { get; set; }
        public string mpId { get; set; }
        public string mpvVal { get; set; }
        public string mpvId { get; set; }
        public string goalTxt { get; set; }
        public string data_type_token { get; set; }
        public string goalMetYN { get; set; }
        public string displayClass { get; set; }
        public List<MPReason> mpReasonList { get; set; }

        public MPReasonViewModel()
        {
            mpReasonList = new List<MPReason>();
        }
    }
}