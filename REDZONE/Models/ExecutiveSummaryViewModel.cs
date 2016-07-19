using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REDZONE.Models
{
    public class ExecutiveSummaryViewModel
    {
        public string month { get; set; }
        public string year { get; set; }
        public string urlNextMonth { get; set; }
        public string urlPrevMonth { get; set; }
        BuildingMetricEntity goal = new BuildingMetricEntity();
        BuildingMetricEntity redTotals = new BuildingMetricEntity();
        List<BuildingMetricEntity> buildings = new List<BuildingMetricEntity>();

        //-------- Empty Constructor -----------
        public ExecutiveSummaryViewModel() { }
        //-------- End of Constructor ----------
    }

    public class BuildingMetricEntity
    {// This represents a single Row in the Executive Summary Spreadsheet
        public string BuildingName { get; set; }
        public string score { get; set; }
        public string scoreColor { get; set; }
        public List<MeasuredMetric> entityMetrics = new List<MeasuredMetric>();        

        //-------- Empty Constructor -----------
        public BuildingMetricEntity() { }
        //-------- End of Constructor ----------
    }

    public class MeasuredMetric
    {// This represents a single Cell in the Executive Summary Spreadsheet
        public string metricName { get; set; }
        public string metricValue { get; set; }
        public string metricColor { get; set; }
        //-------- Empty Constructor -----------
        public MeasuredMetric() { }
        //-------- End of Constructor ----------
    }
}