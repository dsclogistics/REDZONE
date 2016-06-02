using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REDZONE.Models
{
    public class DSC_LC
    {
        [Display(Name = "LC ID")]
        public int dsc_lc_id { set; get; }
        [Display(Name = "Name")]
        public string dsc_lc_name { set; get; }
        [Display(Name = "LC Code")]
        public string dsc_lc_code { set; get; }
        [Display(Name = "Time Zone")]
        public string dsc_lc_timezone { set; get; }
        [Display(Name = "Effective End Date")]
        public DateTime dsc_lc_eff_end_date { set; get; }
    }
}