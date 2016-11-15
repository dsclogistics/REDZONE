using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace REDZONE.Models
{ 
    public class RZTasksViewModel
    {
        public List<RzApTaskBldg> apSubmitTaskList { get; set; }
        public List<RzApTaskMtrc> apReviewTaskList { get; set; }
        public List<RzMtrcTaskPeriod> mtrcTaskList { get; set; }

        public RZTasksViewModel()
        {
            apSubmitTaskList = new List<RzApTaskBldg>();
            apReviewTaskList = new List<RzApTaskMtrc>();
            mtrcTaskList = new List<RzMtrcTaskPeriod>();
        }
    }

    //Action Plan Submitter Task ViewModel
    public class RzApTaskBldg
    {
        [Display(Name = "Building")]
        public string bldgName { get; set; }
        public List<RzApTaskBldgPeriod> periodList { get; set; }

        public RzApTaskBldg()
        {
            periodList = new List<RzApTaskBldgPeriod>();
        }
    }
    public class RzApTaskBldgPeriod
    {
        [Display(Name = "Time Period")]
        public string periodName { get; set; }
        public List<RzApTaskBldgPeriodMtrc> mtrcList { get; set; }
        public RzApTaskBldgPeriod()
        {
            mtrcList = new List<RzApTaskBldgPeriodMtrc>();
        }
    }
    public class RzApTaskBldgPeriodMtrc
    {
        [Display(Name = "Metric")]
        public string mtrc_prod_display_text { get; set; }
        [Display(Name = "Status")]
        public string status { get; set; }
        [Display(Name = "Action")]
        public string action { get; set; }
        public int mtrc_period_id { get; set; }
        public int bldg_id { get; set; }
        public string rz_bapm_id { get; set; }
        public string month { get; set; }
        public string year { get; set; }
    }

    //Action Plan Reviewer Task ViewModel
    public class RzApTaskMtrc
    {
        [Display(Name = "Metric")]
        public string mtrc_prod_display_text { get; set; }

        public List<RzApTaskMtrcPeriod> periodList { get; set; }

        public RzApTaskMtrc()
        {
            periodList = new List<RzApTaskMtrcPeriod>();
        }
    }
    public class RzApTaskMtrcPeriod
    {
        [Display(Name = "Time Period")]
        public string periodName { get; set; }
        public List<RzApTaskMtrcPeriodBldg> bldgList { get; set; }

        public RzApTaskMtrcPeriod()
        {
            bldgList = new List<RzApTaskMtrcPeriodBldg>();
        }
    }
    public class RzApTaskMtrcPeriodBldg
    {
        [Display(Name = "Building")]
        public string bldgName { get; set; }
        [Display(Name = "Status")]
        public string status { get; set; }
        [Display(Name = "Action")]
        public string action { get; set; }
        public int mtrc_period_id { get; set; }
        public int bldg_id { get; set; }
        public string rz_bapm_id { get; set; }
        public string month { get; set; }
        public string year { get; set; }
    }

    //Metric Task ViewModel
    public class RzMtrcTaskPeriod
    {
        [Display(Name = "Time Period")]
        public string periodName { get; set; }
        public List<RzMtrcTaskPeriodMtrc> mtrcList { get; set; }

        public RzMtrcTaskPeriod()
        {
            mtrcList = new List<RzMtrcTaskPeriodMtrc>();
        }
    }
    public class RzMtrcTaskPeriodMtrc
    {
        [Display(Name = "Metric")]
        public string mtrc_prod_display_text { get; set; }
        [Display(Name = "Status")]
        public string status { get; set; }
        [Display(Name = "Action")]
        public string action { get; set; }
        public int mtrc_id { get; set; }
        public string month { get; set; }
        public string year { get; set; }
        public string month_name { get; set; }
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
        public string status { get; set; }
        public int mtrc_id { get; set; }
    }

    public class RZTaskCounts
    {
        public int mtrcCount { get; set; }
        public int actPlanCount { get; set; }
        public int totalTasks   { get { return mtrcCount + actPlanCount; } }
        public RZTaskCounts() {
            mtrcCount = 0;
            actPlanCount = 0;
        }
    }
}