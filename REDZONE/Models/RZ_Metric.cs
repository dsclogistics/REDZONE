using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace REDZONE.Models
{
    // Enumerated Data Type - It can be replaced later on with actual DB retrieved Values
    public enum METRICPERIODS { Second, Minute, Hour, Day, Week, Month, Quarter, Year };

    public class RZ_Metric
    {
        [Display(Name = "Metric ID")]                       // numeric, pk for mtrc_metric table
        public int id { set; get; }
        [Display(Name = "Metric Name")]                     //i.e. Net FTE (mtrc_metric.mtrc_name)
        public string metricName { set; get; }

        [Display(Name = "Metric Data Type")]
        public string metricDataType { get; set; }              //Decimal=dec, Currency...

        [Display(Name = "Period Type")] 
        public METRICPERIODS period_Type { get; set; }      //second, minute, day, month..., example: "tpt_name": "Month"
        [Display(Name = "Period Name")]
        public string displayPeriodName { get{ return metric_period_start_date.ToString("MMMM yyyy");          } }             // i.e. "June, 2016"
     
        public string periodName { set; get; }       // Monthly Net FTE (mtrc_period_name)
        [Display(Name = "Product Name")]
        public string prodName { set; get; }
        [Display(Name = "Period Start Date")]
        public DateTime metric_period_start_date { set; get; }
        [Display(Name = "Period End Date")]
        public DateTime metric_period_end_date { set; get; }
        [Display(Name = "N/A Allowed?")]
        public bool na_allowed { set; get; }
        public bool isImportable { set; get; }
        public int metricPeriodID { set; get; }
        public bool isNumeric { set; get; }
        public bool isAuto { set; get; }

        public string mtrcMinVal { set; get; }
        public string mtrcMaxVal { set; get; }
        public string maxDecPlaces { set; get; }
        public string maxStrSize { set; get; }

        public string lastMonthUrl { get { return String.Format("/Metric/EditView/{0}?month={1}&year={2}",id,metric_period_start_date.AddMonths(-1).ToString("MMMM"), metric_period_start_date.AddMonths(-1).ToString("yyyy")); } }
        public string nextMonthUrl { get { return String.Format("/Metric/EditView/{0}?month={1}&year={2}", id, metric_period_start_date.AddMonths(1).ToString("MMMM"), metric_period_start_date.AddMonths(1).ToString("yyyy")); } }
  
        public List<Building> buildingList = new List<Building>();
        public string allBuildings { get; set; }
        //public List<SelectListItem> periodTypesSL = new List<SelectListItem>() { 
        //        new SelectListItem() {Text="- Select -", Value=""},
        //        new SelectListItem() {Text="Second", Value="0"},
        //        new SelectListItem() { Text="Minute", Value="1"},
        //        new SelectListItem() { Text="Hour", Value="2"},
        //        new SelectListItem() { Text="Day", Value="3"},
        //        new SelectListItem() { Text="Week", Value="4"},
        //        new SelectListItem() { Text="Month", Value="5"},
        //        new SelectListItem() { Text="Quarter", Value="6"},
        //        new SelectListItem() { Text="Year", Value="7"}
        //};  //Select List used to populate a drop down list with period types in the MVC view

        //-------- Constructor -----------
        public RZ_Metric()
        {
            //id = 123;                                                 // Testing Dummy Value
            //metricName = metric_Type + " Metric";
            //period_Type = METRICPERIODS.Month;
            //metricType = metric_Type;            
            //metricDesc = "Monthly Metric for Volume of Cases moved by LC Building";
            //metric_eff_start_date = new DateTime(metricMonth.Year, metricMonth.Month, 1);
            //metric_eff_end_date = new DateTime(metricMonth.Year, metricMonth.Month, DateTime.DaysInMonth(metricMonth.Year, metricMonth.Month), 23, 59, 59, 999);
            //period_Name = metricMonth.ToString("MMM, yyyy");
            //na_allowed = "YES";
        }//-------- END Constructor -----------

    }
}