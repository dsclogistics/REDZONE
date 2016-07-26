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
        public string statusNextMonth { get; set; }
        public string statusPrevMonth { get; set; }
        public BuildingMetricEntity goal = new BuildingMetricEntity();
        public BuildingMetricEntity redTotals = new BuildingMetricEntity();
        public List<BuildingMetricEntity> buildings = new List<BuildingMetricEntity>();
        public List<string> allBuildings = new List<string>();
        public List<MetricHeader> allMetrics = new List<MetricHeader>();
        public List<string> allMonths = new List<string>();

        //-------- Empty Constructor -----------
        public ExecutiveSummaryViewModel() { }
        //\-------- End of Constructor ----------/
    }

    public class MetricHeader
    {
        public string metricName;
        public string metricID;
        public string url;
    }

    public class BuildingMetricEntity
    {// This represents a single Row in the Executive Summary Spreadsheet
        private const string BLUECOLOR = "lightblue";
        private const string GREENCOLOR = "lightgreen";
        private const string REDCOLOR = "#ffbb8b";   //or #ffbb8b or "orangered"
        public string BuildingName { get; set; }
        public string score { get; set; }
        public string scoreColor { get; set; }
        public string buildingId { get; set; }

        public string url { get; set; }
        public List<MeasuredMetric> entityMetrics = new List<MeasuredMetric>();        

        //-------- Empty Constructor -----------
        public BuildingMetricEntity() { }

        //\-------- End of Constructor ----------/
    }

    public class MeasuredMetric
    {// This represents a single Cell in the Executive Summary Spreadsheet
        public string metricName { get; set; }
        public string metricValue { get; set; }
        public string metricColor { get; set; }
        public string mtrc_period_id { get; set; }
        public string metricMonth { get; set; }
        public string mtrc_id { get; set; }
        public string tm_period_id { get; set; }
        public string dsc_mtrc_lc_bldg_id { get; set; }
        public string cellValueURL { get; set; }


        //-------- Empty Constructor -----------\
        public MeasuredMetric() { }
        //\------- End of Constructor ----------/
        public MeasuredMetric(string mName, string mValue, string mColor) {
            metricName = mName;
            metricValue = mValue;
            metricColor = mColor;
        }
        //-------- End of Constructor ----------
    }

   

    public class MeasuredRowEntity
    {// This represents a single Row in the Building Summary Spreadsheet
        private const string BLUECOLOR = "lightblue";
        private const string GREENCOLOR = "lightgreen";
        private const string REDCOLOR = "#ffbb8b";   //or #ffbb8b or "orangered"
        public string rowName { get; set; }        //Row represents a "Metric"
        public string rowMeasuredId { get; set; }
        public string rowURL { get; set; }
        public string scoreGoal { get; set; }
        public string redTotals { get; set; }
        public string scoreGoalColor { get; set; }
        public string redTotalColor { get; set; }        

        public List<MeasuredMetric> entityMetricCells = new List<MeasuredMetric>();

        //-------- Empty Constructor ------------\
        public MeasuredRowEntity() { }
        //\-------- End of Constructor ----------/
    }

  
}