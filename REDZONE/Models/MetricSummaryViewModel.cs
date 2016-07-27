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
               
        public string urlNextPeriod { get; set; }
        public string urlPrevPeriod { get; set; }
        public string statusNextPeriod { get; set; }
        public string statusPrevPeriod { get; set; }
        public MeasuredRowEntity rowGoal = new MeasuredRowEntity();
        public MeasuredRowEntity rowHeadings = new MeasuredRowEntity();
        public List<MeasuredRowEntity> metricRows = new List<MeasuredRowEntity>();

        //-------- Empty Constructor -----------
        public MetricSummaryViewModel() { }
        //\-------- End of Constructor ----------/
    }

}