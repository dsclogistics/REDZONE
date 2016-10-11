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
        public string priorMetricGoal { get; set; }
        public string priorMetricValue { get; set; }
        public List<MPReason> priorReasonList { get; set; }
    }
}