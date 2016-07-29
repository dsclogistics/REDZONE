using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REDZONE.Models
{
    public class BuildingSummaryViewModel
    {
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
        public MeasuredRowEntity buildingHeadings = new MeasuredRowEntity();
        public List<MeasuredRowEntity> metricRows = new List<MeasuredRowEntity>();
        public MeasuredRowEntity buildingScoreRow = new MeasuredRowEntity();
        //public List<string> allBuildings = new List<string>();
        //public List<string> allMetrics = new List<string>();
        //public List<string> allMonths = new List<string>();

        public int activeColumns { get; set; }
        public string metricColWidth
        {
            get
            {
                activeColumns = (activeColumns == 0) ? 1 : activeColumns;   //Default a value of 1 if zero
                return (1.00 / activeColumns).ToString("0.0%");
            }
        }

        //-------- Empty Constructor -----------
        public BuildingSummaryViewModel() { }
        //\-------- End of Constructor ----------/
    }
}