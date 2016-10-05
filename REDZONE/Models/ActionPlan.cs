using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REDZONE.Models
{
    public class ActionPlan
    {
        public List<MPReason> reasonList { get; set; }
        public string bapm_id { get; set; }
        public string apd_id { get; set; }
        public string apVersion { get; set; }
        public string apStatus { get; set; }
        public string actionPlanAction { get; set; }
        public string reviewerComments { get; set; }
        public string submittedBy { get; set; }
        public string reviewedBy { get; set; }

        public ActionPlan() { reasonList = new List<MPReason>(); }
    }
}