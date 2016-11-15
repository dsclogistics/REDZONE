using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REDZONE.Models
{
    public class MetricSummaryViewModel
    {
        public string metricID { get; set; }
        public string metricName { get; set; }
        public string metricDescription { get; set; }
        public string year { get; set; }
        public string metricGoal { get; set; }             //???
        public bool canFilterRows { get; set; }
               
        public string urlNextMetric { get; set; }
        public string urlPrevMetric { get; set; }
        public string statusNextMetric { get; set; }
        public string statusPrevMetric { get; set; }
        public MeasuredRowEntity rowGoal = new MeasuredRowEntity();
        public MeasuredRowEntity rowHeadings = new MeasuredRowEntity();
        public List<MeasuredRowEntity> metricRows = new List<MeasuredRowEntity>();
        public MeasuredRowEntity missedGoals = new MeasuredRowEntity();

        public int viewableColumns { get; set; }
        public string metricColWidth
        {
            get
            {
                viewableColumns = (viewableColumns == 0) ? 1 : viewableColumns;   //Default a value of 1 if zero
                return (1.00 / viewableColumns).ToString("0.0%");
            }
        }
        //-------- Empty Constructor -----------
        public MetricSummaryViewModel() { }
        //\-------- End of Constructor ----------/
    }

}