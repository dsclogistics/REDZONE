using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REDZONE.Models
{
    public class ExecutiveSummaryViewModel
    {
        public bool isModelValid { get; set; }         // If the model cannot be retrieved or parsed this flag is "false"
        public string modelMessage { get; set; }       // For reporting any error or give activity message while parsing data
        public string month { get; set; }
        public string year { get; set; }
        public int total { get; set; }
        public string urlNextMonth { get; set; }
        public string urlPrevMonth { get; set; }
        public string statusNextMonth { get; set; }
        public string statusPrevMonth { get; set; }
        public BuildingMetricEntity goalsRow = new BuildingMetricEntity();
        public BuildingMetricEntity goalsMissedRow = new BuildingMetricEntity();
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
        public string metricID;
        public string metricName;
        public string metricDescription;
        public string url;
    }

    public class BuildingMetricEntity
    {// This represents a single Row in the Executive Summary Spreadsheet
        //private const string BLUECOLOR = "lightblue";
        //private const string GREENCOLOR = "lightgreen";
        //private const string REDCOLOR = "#ffbb8b";   //or #ffbb8b or "orangered"
        public string BuildingName { get; set; }
        public int rowScore { get; set; }
        //public string scoreColor { get; set; }       //We will not need this anymore
        public string scoreDisplayClass { get; set; }
        public string buildingId { get; set; }
        public string url { get; set; }
        public List<MeasuredCellEntity> entityMetrics = new List<MeasuredCellEntity>();

        //-------- Empty Constructor -----------
        public BuildingMetricEntity() { }

        //\-------- End of Constructor ----------/
    }

    public class MeasuredCellEntity
    {// This represents a single Cell in the Executive Summary Spreadsheet
        public string metricName { get; set; }
        public string metricValue { get; set; }
        public double metricDoubleValue { get; set; }
        public string mtrc_period_id { get; set; }
        public string metricMonth { get; set; }
        public string mtrc_id { get; set; }
        public string tm_period_id { get; set; }
        public string dsc_mtrc_lc_bldg_id { get; set; }
        public string cellValueURL { get; set; }
        public bool isViewable { get; set; }  
        public string isGoalMet { get; set; }
        public int score { get; set; }    
        public string displayClass { get; set; }
        public string cellStatus { get; set; }

        //-------- Empty Constructor -----------\
        public MeasuredCellEntity() { }
        //-------- Metric Specific Constructor -----------\
        public MeasuredCellEntity(string name)
        { metricName = name; }
        //-------- Instantiated Constructor -----------\
        public MeasuredCellEntity(string mName, string mValue) {
            metricName = mName;
            metricValue = mValue;
        }
        //-------- End of Constructor ----------
    }

   

    public class MeasuredRowEntity
    {// This represents a single Row in the Building Summary Spreadsheet
        public string rowName { get; set; }        //Row represents a "Metric"
        public string rowMeasuredId { get; set; }
        public string rowURL { get; set; }
        public string displayClass { get; set; }
        public string scoreGoal { get; set; }
        public int redTotals { get; set; }
        public string scoreGoalColor { get; set; }
        public string redTotalColor { get; set; }

        public List<MeasuredCellEntity> entityMetricCells = new List<MeasuredCellEntity>();

        //-------- Empty Constructor ------------\
        public MeasuredRowEntity() { }

        //\-------- End of Constructor ----------/
    }

  
}