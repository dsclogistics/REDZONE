using System;
using System.Collections.Generic;
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

    public class RZActionPlanTask
    {
        public string month { get; set; }
        public string year { get; set; }
        public string mtrc_period_name { get; set; }
        public string bldg_name { get; set; }
        public string status { get; set; }
        public string rz_bapm_id { get; set; }
    }

    public class RZMetricTask
    {
        public string month { get; set; }
        public string year { get; set; }
        public string mtrc_period_name { get; set; }
    }

    public class RZTaskCounts
    {
        public int mtrcCount { get; set; }
        public int actPlanCount { get; set; }
    }
}