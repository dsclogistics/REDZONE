using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REDZONE.Models
{
    public class DSC_LC_Building
    {
        [Display(Name = "Building ID")]
        public int dsc_lc_bldg_id { set; get; }
        [Display(Name = "LC ID")]
        public int dsc_lc_id { set; get; }
        [Display(Name = "Building Name")]
        public string dsc_lc_bldg_name { set; get; }
        [Display(Name = "Code")]
        public string dsc_lc_bldg_code { set; get; }
        [Display(Name = "Effective Start Date")]
        public DateTime dsc_lc_bldg_eff_start_date { set; get; }
        [Display(Name = "Effective End Date")]
        public DateTime dsc_lc_bldg_eff_end_date { set; get; }
    }
}