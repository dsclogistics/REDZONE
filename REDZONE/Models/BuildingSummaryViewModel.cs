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
        public string statusNextPeriod { get; set; }
        public string statusPrevPeriod { get; set; }
        public MeasuredRowEntity buildingHeadings = new MeasuredRowEntity();
        public MeasuredRowEntity buildingScore = new MeasuredRowEntity();
        public List<MeasuredRowEntity> metricRows = new List<MeasuredRowEntity>();
        //public List<string> allBuildings = new List<string>();
        //public List<string> allMetrics = new List<string>();
        //public List<string> allMonths = new List<string>();

        //-------- Empty Constructor -----------
        public BuildingSummaryViewModel() { }
        //\-------- End of Constructor ----------/
    }
}