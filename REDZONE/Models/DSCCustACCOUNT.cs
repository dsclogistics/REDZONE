using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REDZONE.Models
{
    public class DSCCustAccount
    {
        [Display(Name = "Customer ID")]
        public int dsc_cust_id { set; get; }
        [Display(Name = "Account")]
        public string dsc_cust_acct { set; get; }
        [Display(Name = "Name")]
        public string dsc_cust_name { set; get; }
        [Display(Name = "Parent Name")]
        public string dsc_cust_parent_name { set; get; }
        [Display(Name = "Effective End Date")]
        public DateTime dsc_cust_eff_end_date { set; get; }
    }
}