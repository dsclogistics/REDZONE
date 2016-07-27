using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REDZONE.Models
{
    public class MetricSummaryViewModel
    {
        public string metricName { get; set; }
        public string metricID { get; set; }
        public string year { get; set; }               
        public string metricGoal { get; set; }
               
        public string urlNextMetric { get; set; }
        public string urlPrevMetric { get; set; }
        public string statusNextMetric { get; set; }
        public string statusPrevMetric { get; set; }
        public MeasuredRowEntity rowGoal = new MeasuredRowEntity();
        public MeasuredRowEntity rowHeadings = new MeasuredRowEntity();
        public List<MeasuredRowEntity> metricRows = new List<MeasuredRowEntity>();

        //-------- Empty Constructor -----------
        public MetricSummaryViewModel() { }
        //\-------- End of Constructor ----------/
    }

}