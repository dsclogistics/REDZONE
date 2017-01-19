using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace REDZONE.Models
{
    public class rzTable
    {
        public rzRow metricHeadingsRow = new rzRow();
        public rzRow goalHeadingsRow = new rzRow();
        public List<rzRow> dataRows = new List<rzRow>();
        public rzRow goalsMissedRow = new rzRow();
        public List<string> colWidths = new List<string>();
    }

    public class rzRow
    {  // This CLASS represents a single Row in the table
        public int rowOrder { get; set; }
        public rzCell rowHeaderCell = new rzCell();
        public List<rzCell> rowDataColumns = new List<rzCell>();
        public rzCell rowTotalsCell = new rzCell();
        //public string valueCellWidth { 
        //    get {
        //        return (rowColumns.Count == 0) ? "75%" : (75 / rowColumns.Count).ToString("00.00%");
        //    } 
        //}
        //-------- Empty Constructor ------------\
        public rzRow() {  }

        //\-------- End of Constructor ----------/

        //Column Widhts are: (They can be set only on the Header Row, no need to populate other rows cell widths)
        //[0] 16.66%
        //[1] 08.33%
        // Values Columns = (0.6668 / viewableColumns).ToString("0.00%");
        //[Last] 08.33%
    }

    public class rzCell
    {  // This CLASS represents a single Cell in a Table Row
        public int cellOrder {get;set;}            //Column Index in the Table
        public string cellValue { get; set; }      //Cell Actual Value
        public string cellDispValue { get; set; }      //Cell Display Value
        public string cellURL { get; set; }        //Cell URL http link when clicked
        public bool isViewable { get; set; }       //Whether it's value gets displayed or hidded
        public bool isValueMet {get; set; }        //i.e. "isGoalMet"
        public string displayClass { get; set; }   //Determines color Scheme of Cell
        public string cellStatus { get; set; }            //                      ???????????????????????????
        public string caption { get; set; }        //For Title Property (mouseover caption)
        public string cellOwner { get; set; }
        public string cellLastUpdt { get; set; }
        public metricInfo cellMetricInfo = new metricInfo();             //Metric Information Related to this cell         [For 'Value' Cells only]  
        public metricPeriodInfo mpInfo = new metricPeriodInfo();                 //Metric Period Information Related to this cell. [For 'Value' Cells only]        
        public actionPlanInfo apInfo = new actionPlanInfo();                 //Action Plan Information Related to this cell    [For 'Value' Cells only]

        //public string rowMeasuredId { get; set; }           //?????????????????
        //public string metricMonth { get; set; }
        //public string tm_period_id { get; set; }
        //public string dsc_mtrc_lc_bldg_id { get; set; }

        //-------- Empty Constructor -----------\
        public rzCell() { 
            //Initialize Properties with default Values
            isViewable = true;
            cellStatus = "Inactive";
            cellValue = "";
            cellDispValue = "";
            displayClass = "";              // Add The Default neutral gray color schema   !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!  TO DO !!!!!!!!!!!!!
        }
        //-------- End of Constructor ----------
        //public void setCell(){
        
        //}

    }

    public class metricInfo
    {
        public string mtrc_id { get; set; }
        public string metricName { get; set; }
        public string metricValue { get; set; }
        public double metricDoubleValue { get; set; }
    }

    public class metricPeriodInfo
    {
        public string mpId { get; set; }
        public string mpValue { get; set; }

    }

    public class actionPlanInfo
    {
        public bool hasReasons { get; set; }
        public string rz_bap_id { get; set; }        //Building Action Plan  Id
        public string rz_bapm_id { get; set; }       //Building Action Plan Metric Id
        public string rz_bapm_status { get; set; }   //Building Action Plan Metric Status
        public string nextCellAction { get; set; }   //Next action to take Based on the Cell Metric Value Status and the Current User Authority
        public string nextCellActionLink { get; set; }
    }
}
