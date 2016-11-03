using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REDZONE.Models
{ 
    public class TasksViewModel
    {
        public List<RZActionPlanTask> apSubmitTaskList { get; set; }
        public List<RZActionPlanTask> apReviewTaskList { get; set; }
        public List<RZMetricTask> mtrcTaskList { get; set; }

        public TasksViewModel()
        {
            apSubmitTaskList = new List<RZActionPlanTask>();
            apReviewTaskList = new List<RZActionPlanTask>();
            mtrcTaskList = new List<RZMetricTask>();
        }
    }

    //ViewModel WIP
    public class RZActionPlanTaskBldg
    {
        public List<RZActionPlanTaskBldgPeriod> periodList { get; set; }

        public RZActionPlanTaskBldg()
        {
            periodList = new List<RZActionPlanTaskBldgPeriod>();
        }
    }

    public class RZActionPlanTaskBldgPeriod
    {
        public List<RZActionPlanTaskBldgPeriodMtrc> mtrcList { get; set; }
    }

    public class RZActionPlanTaskBldgPeriodMtrc
    {

    }



    //Model to parse Task Detail data from API
    public class RZTaskDetailList
    {
        public List<RZActionPlanTask> apSubmitTaskList { get; set; }
        public List<RZActionPlanTask> apReviewTaskList { get; set; }
        public List<RZMetricTask> mtrcTaskList { get; set; }

        public RZTaskDetailList()
        {
            apSubmitTaskList = new List<RZActionPlanTask>();
            apReviewTaskList = new List<RZActionPlanTask>();
            mtrcTaskList = new List<RZMetricTask>();
        }
    }

    public class RZActionPlanTask
    {
        public string month { get; set; }
        public string year { get; set; }
        [Display(Name = "Time Period")]
        public string period_name { get; set; }
        [Display(Name = "Metric")]
        public string mtrc_prod_display_text { get; set; }
        public string mtrc_period_name { get; set; }
        public int mtrc_period_id { get; set; }
        [Display(Name = "Building")]
        public string bldg_name { get; set; }
        public int bldg_id { get; set; }
        [Display(Name = "Status")]
        public string status { get; set; }
        public string rz_bapm_id { get; set; }
    }

    public class RZMetricTask
    {
        public string month { get; set; }
        public string year { get; set; }
        public string month_name { get; set; }
        [Display(Name = "Time Period")]
        public string period_name { get; set; }
        [Display(Name = "Metric")]
        public string mtrc_prod_display_text { get; set; }
        public string mtrc_period_name { get; set; }
        [Display(Name = "Status")]
        public int mtrc_id { get; set; }
        public string status { get; set; }
    }

    public class RZTaskCounts
    {
        public int mtrcCount { get; set; }
        public int actPlanCount { get; set; }
        public int totalTasks   { get { return mtrcCount + actPlanCount; } }
    }
}