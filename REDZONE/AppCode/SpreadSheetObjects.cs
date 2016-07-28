using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REDZONE.AppCode
{
    public class SpreadSheetObjects
    {
    }

    //=========== This Class represents a single Row for each Row in a Spreadsheet Object =============================
    public class MetricRow
    {// This represents a single Row for each Spreadsheet Object
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

        public List<MetricCell> entityMetricCells = new List<MetricCell>();

        //-------- Empty Constructor ------------\
        public MetricRow() { }

        //\-------- End of Constructor ----------/

    }

    //=========== This Class represents a single Cell for each Row in a Spreadsheet Object =============================
    public class MetricCell
    {
        public string metricName { get; set; }
        public string metricValue { get; set; }
        public string metricColor { get; set; }
        public string mtrc_period_id { get; set; }
        public string metricMonth { get; set; }
        public string mtrc_id { get; set; }
        public string tm_period_id { get; set; }
        public string dsc_mtrc_lc_bldg_id { get; set; }
        public string cellValueURL { get; set; }
        public bool isViewable { get; set; }


        //-------- Empty Constructor -----------\
        public MetricCell() { }
        //\------- End of Constructor ----------/
        public MetricCell(string name)
        { metricName = name; }
        public MetricCell(string mName, string mValue, string mColor)
        {
            metricName = mName;
            metricValue = mValue;
            metricColor = mColor;
        }
        //-------- End of Constructor ----------
    }
}