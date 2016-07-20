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
        public List<string> allMetrics = new List<string>();

        //-------- Empty Constructor -----------
        public ExecutiveSummaryViewModel() { }
        //\-------- End of Constructor ----------/
    }

    public class BuildingMetricEntity
    {// This represents a single Row in the Executive Summary Spreadsheet
        private const string BLUECOLOR = "lightblue";
        private const string GREENCOLOR = "lightgreen";
        private const string REDCOLOR = "#ffbb8b";   //or #ffbb8b or "orangered"
        public string BuildingName { get; set; }
        public string score { get; set; }
        public string scoreColor { get; set; }

        public List<MeasuredMetric> entityMetrics = new List<MeasuredMetric>();        

        //-------- Empty Constructor -----------
        public BuildingMetricEntity() { }
        public BuildingMetricEntity(string action) {
            switch (action.ToUpper())
            {
                case "GOAL":
                    BuildingName = "Goal";
                    score = "<= 2";
                    scoreColor = BLUECOLOR;                
                    // Fill all the Cells For the Goal Row
                    addMetricValue("Net FTE", "0.00", BLUECOLOR);
                    addMetricValue("Turnover", "7.5%", BLUECOLOR);
                    addMetricValue("OT", "10.00 %", BLUECOLOR);
                    addMetricValue("Trainees", "20.00 %", BLUECOLOR);
                    addMetricValue("Safety", "1.45", BLUECOLOR);
                    addMetricValue("Volume", "+/- 20%", BLUECOLOR);
                    addMetricValue("IT Tickets", "25/M", BLUECOLOR);
                    addMetricValue("Financial", "+/- Goal", BLUECOLOR);
                    break;
                case "AP":
                    BuildingName = "Allemtown 1";
                    score = "2";
                    scoreColor = GREENCOLOR;
                    // Fill all the Cells For the Goal Row
                    addMetricValue("Net FTE", "0.00", GREENCOLOR);
                    addMetricValue("Turnover", "7.00%", GREENCOLOR);
                    addMetricValue("OT", "12.70 %", REDCOLOR);
                    addMetricValue("Trainees", "20.00 %", GREENCOLOR);
                    addMetricValue("Safety", "0.74", GREENCOLOR);
                    addMetricValue("Volume", "- 5.10%", GREENCOLOR);
                    addMetricValue("IT Tickets", "4", GREENCOLOR);
                    addMetricValue("Financial", "- 2.67%", REDCOLOR);
                    break;
                case "PP":
                    BuildingName = "Des Plaines";
                    score = "4";
                    scoreColor = REDCOLOR;
                    // Fill all the Cells For the Goal Row
                    addMetricValue("Net FTE", "-2.01", GREENCOLOR);
                    addMetricValue("Turnover", "11.00%", REDCOLOR);
                    addMetricValue("OT", "12.67 %", REDCOLOR);
                    addMetricValue("Trainees", "13.80 %", GREENCOLOR);
                    addMetricValue("Safety", "3.54", REDCOLOR);
                    addMetricValue("Volume", "- 12.86%", GREENCOLOR);
                    addMetricValue("IT Tickets", "41", REDCOLOR);
                    addMetricValue("Financial", "6.31%", GREENCOLOR);
                    break;
                case "PC":
                    BuildingName = "Perris";
                    score = "2";
                    scoreColor = GREENCOLOR;
                    // Fill all the Cells For the Goal Row
                    addMetricValue("Net FTE", "-1.96", GREENCOLOR);
                    addMetricValue("Turnover", "6.00%", GREENCOLOR);
                    addMetricValue("OT", "11.59 %", REDCOLOR);
                    addMetricValue("Trainees", "18.70 %", GREENCOLOR);
                    addMetricValue("Safety", "0.65", GREENCOLOR);
                    addMetricValue("Volume", "- 11.30%", GREENCOLOR);
                    addMetricValue("IT Tickets", "64", REDCOLOR);
                    addMetricValue("Financial", "10.41%", GREENCOLOR);
                    break;
                default:
                    break;
            }
        }

        private void addMetricValue(string p1, string p2, string p3)
        {
            MeasuredMetric goal = new MeasuredMetric(p1, p2, p3);
            entityMetrics.Add(goal);
        }
        //\-------- End of Constructor ----------/
    }

    public class MeasuredMetric
    {// This represents a single Cell in the Executive Summary Spreadsheet
        public string metricName { get; set; }
        public string metricValue { get; set; }
        public string metricColor { get; set; }
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
}