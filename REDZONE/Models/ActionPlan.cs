using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REDZONE.Models
{
    public class ActionPlan
    {
        public string apd_id { get; set; }
        public string apVersion { get; set; }
        public string apStatus { get; set; }
        public string actionPlanAction { get; set; }
        public string reviewerComments { get; set; }
        public string submittedBy { get; set; }      //aka Building/Metric Owner
        public string reviewedBy { get; set; }       //aka Building/Metric Reviewer        
        public string apMetric { get; set; }
        public string apMeetingDate { get; set; }
        public string apLastUpdt { get; set; }
    }

    public class monthActionPlan { 
        //Month Action Plan
        public string displayId { get; set; }
        public string apBuilding { get; set; }
        public string apMonth { get; set; }
        public List<ActionPlan> metricActPlans = new List<ActionPlan>();
    }

    public class PriorActionPlan
    {
        public string apd_id { get; set; }
        public string bapm_id { get; set; }
        public string mtrc_period_val_id { get; set; }
        public string mtrc_period_id { get; set; }
        public string dsc_mtrc_lc_bldg_id { get; set; }
        public string priorAPMonth { get; set; }
        public string priorAPYear { get; set; }
        public string priorAPMetricGoalText { get; set; }
        public string priorAPMetricValue { get; set; }
        public string priorAPStatus { get; set; }
        public string priorAPText { get; set; }
        public string priorAPReviewText { get; set; }
        public string submittedBy { get; set; }
        public string approvedBy { get; set; }
        public List<MPReason> priorAPReasonList { get; set; }
    }

}