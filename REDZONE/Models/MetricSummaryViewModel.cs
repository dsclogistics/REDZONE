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
        public string urlNextPeriod { get; set; }
        public string urlPrevPeriod { get; set; }

        public string statusNextMetric { get; set; }
        public string statusPrevMetric { get; set; }
        public string statusNextPeriod { get; set; }
        public string statusPrevPeriod { get; set; }


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
        public string tableDisplaySize
        {
            get
            {
                //Each Building Row uses about 28px. The max size we want to allow is 504px or 18 rows. Minimum size is 100px
                if (metricRows.Count < 2) { return "60px"; }
                if (metricRows.Count > 18) { return "504px"; }
                return (metricRows.Count * 28).ToString("0px");
                //string returnValue = (metricRows.Count * 28).ToString("0px");
                //return returnValue;
            }
        }

        //-------- Empty Constructor -----------
        public MetricSummaryViewModel() { }
        //\-------- End of Constructor ----------/
    }

}