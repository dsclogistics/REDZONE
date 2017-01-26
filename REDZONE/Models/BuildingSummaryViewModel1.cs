using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REDZONE.Models
{
    public class BuildingSummaryViewModel1
    {
        public bool isModelValid { get; set; }
        public string modelValidationMsg { get; set; }
        public string bName { get; set; }
        public string bId { get; set; }
        public string year { get; set; }
        public string urlNextPeriod { get; set; }
        public string urlPrevPeriod { get; set; }
        public string urlNextBuilding { get; set; }
        public string urlPrevBuilding { get; set; }
        public string statusNextPeriod { get; set; }
        public string statusPrevPeriod { get; set; }
        public string statusNextBuilding { get; set; }
        public string statusPrevBuilding { get; set; }
        public string monthViewMode { get; set; }           //blank or "display:none" to show or hide The Month View based on user choice
        public bool canFilterBuildings { get; set; }
        public int viewableColumns { get; set; }
        public MeasuredRowEntity buildingHeadings = new MeasuredRowEntity();
        public List<MeasuredRowEntity> metricRows = new List<MeasuredRowEntity>();
        public MeasuredRowEntity buildingScoreRow = new MeasuredRowEntity();
        public string applyFilter {
            get {
                return (HttpContext.Current.Session["buildingFilter"] != null && (string)HttpContext.Current.Session["buildingFilter"] == "Y") ? "checked" : "";
            }
        }
        //public List<string> allBuildings = new List<string>();
        //public List<string> allMetrics = new List<string>();
        //public List<string> allMonths = new List<string>();

        public rzTable summaryByMetric = new rzTable();

        public string metricColWidth
        {
            get
            {
                viewableColumns = (viewableColumns == 0) ? 1 : viewableColumns;   //Default a value of 1 if zero
                return (0.6668 / viewableColumns).ToString("0.00%");
            }
        }

        //-------- Empty Constructor -----------
        public BuildingSummaryViewModel1() { }
        //\-------- End of Constructor ----------/
    }
}