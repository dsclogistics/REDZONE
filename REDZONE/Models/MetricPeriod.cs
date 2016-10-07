﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REDZONE.Models
{
    public class MetricPeriod
    {
        //Model for Metric Periods for Dropdown
        [Display(Name = "Metric Period ID")]
        public string mtrc_period_id { get; set; }
        [Display(Name = "Metric Period Name")]
        public string mtrc_period_name { get; set; }
        [Display(Name = "Token")]
        public string mtrc_period_token { get; set; }
        [Display(Name = "Display Text")]
        public string mtrc_prod_display_text { get; set; }
        [Display(Name = "Product Name")]
        public string prod_name { get; set; }
    }
}