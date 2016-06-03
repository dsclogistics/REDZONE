using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REDZONE.Models
{
    public class Metric
    {
        [Display(Name = "Metric ID")]
        public int metricID { set; get; }
        [Display(Name = "Name")]
        public string metricName { set; get; }
        [Display(Name = "Description")]
        public string description { set; get; }
        [Display(Name = "Start Date")]
        public DateTime metric_eff_start_date { set; get; }
        [Display(Name = "End Date")]
        public DateTime metric_eff_end_date { set; get; }
        public string na_allowed { set; get; }


    }
}