using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REDZONE.Models
{
    public class ExecutiveSummaryView
    {
        public string month { get; set; }
        MetricEntity goal = new MetricEntity();
        MetricEntity redTotals = new MetricEntity();
        List<MetricEntity> buildings = new List<MetricEntity>();

        //-------- Empty Constructor -----------
        public ExecutiveSummaryView() { }
        //-------- End of Constructor ----------

    }

    public class MetricEntity
    {
        public string Name { get; set; }
        public string score { get; set; }
        public string scoreColor { get; set; }
        public List<MeasuredMetric> entityMetrics = new List<MeasuredMetric>();        

        //-------- Empty Constructor -----------
        public MetricEntity() { }
        //-------- End of Constructor ----------
    }

    public class MeasuredMetric{
        public string metricName { get; set; }
        public string metricValue { get; set; }
        public string metricColor { get; set; }
        //-------- Empty Constructor -----------
        public MeasuredMetric() { }
        //-------- End of Constructor ----------
    }
}