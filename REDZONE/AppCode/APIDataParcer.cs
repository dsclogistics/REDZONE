using Newtonsoft.Json.Linq;
using REDZONE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REDZONE.AppCode
{

    public class APIDataParcer
    {
        DataRetrieval api = new DataRetrieval();
        ExcelReader excelReader = new ExcelReader();
        //--------------------- CONSTANTS ---------------------
        const string COLOR_YELLOW = "yellow";
        const string COLOR_GREEN = "lightgreen";
        const string COLOR_RED = "#ffbb8b";   //or #ffbb8b or "orangered"
        //---------- END OF CONSTANTS SECTION -----------------


        public RZ_Metric getRZ_Metric(int metric_id, string month, string year)
        {
            RZ_Metric rz_metric = new RZ_Metric();
            rz_metric.allBuildings = String.Empty;
            string raw_data = api.getMetricperiod("Red Zone", "Month", metric_id.ToString(), month, year);
            
            // At this point the json result can be empty (If no data was found) or an Error if an exception was caught 
            // or an actual jason message if The API was successful

            //if (raw_data.Substring(0, 5).Equals("ERROR")) {
            //    HttpContext.Current.Server.Transfer("/Error/Index?ErrorMsg=" + raw_data.Substring(7));
            //}


            try {
                JObject parsed_result = JObject.Parse(raw_data);

                rz_metric.prodName = (string)parsed_result["metricdetail"]["prod_name"];
                rz_metric.id = (int)parsed_result["metricdetail"]["mtrc_id"];
                rz_metric.metricName = (string)parsed_result["metricdetail"]["mtrc_name"];
                rz_metric.metricDataType = (string)parsed_result["metricdetail"]["data_type_token"];
                rz_metric.isNumeric = (string)parsed_result["metricdetail"]["data_type_num_yn"] == "Y" ? true : false;
                rz_metric.period_Type = METRICPERIODS.Month;
                rz_metric.metric_period_start_date = (DateTime)parsed_result["metricdetail"]["tm_per_start_dtm"];
                rz_metric.metric_period_end_date = (DateTime)parsed_result["metricdetail"]["tm_per_end_dtm"];
                rz_metric.periodName = (string)parsed_result["metricdetail"]["mtrc_period_name"] ;
                rz_metric.metricPeriodID = (int)parsed_result["metricdetail"]["mtrc_period_id"];
                rz_metric.na_allowed = (string)parsed_result["metricdetail"]["mtrc_period_na_allow_yn"] == "Y" ? true : false;
                rz_metric.isImportable = (string)parsed_result["metricdetail"]["mtrc_period_can_import_yn"] == "Y" ? true : false;
                rz_metric.isAuto = (string)parsed_result["metricdetail"]["mtrc_period_is_auto_yn"] == "Y" ? true : false;
                rz_metric.mtrcMinVal = (string)parsed_result["metricdetail"]["mtrc_min_val"];
                rz_metric.mtrcMaxVal = (string)parsed_result["metricdetail"]["mtrc_max_val"];
                rz_metric.maxDecPlaces = (string)parsed_result["metricdetail"]["mtrc_max_dec_places"];
                rz_metric.maxStrSize = (string)parsed_result["metricdetail"]["mtrc_max_str_size"];
                rz_metric.metricPeriodStatus = (string)parsed_result["metricdetail"]["rz_mps_status"];
                rz_metric.metricPrevPeriodStatus = String.IsNullOrEmpty((string)parsed_result["metricdetail"]["previousperiod"])|| String.IsNullOrEmpty((string)parsed_result["metricdetail"]["previousperiod"]).Equals("Inactive")?"disabled":"";
                rz_metric.metricNextPeriodStatus = String.IsNullOrEmpty((string)parsed_result["metricdetail"]["nextperiod"]) || String.IsNullOrEmpty((string)parsed_result["metricdetail"]["nextperiod"]).Equals("Inactive") ? "disabled" : "";
                if (rz_metric.metricPeriodStatus.ToUpper().Equals("CLOSED")) { rz_metric.isImportable = false; }
                rz_metric.metricPrevPeriodStatus = String.IsNullOrEmpty((string)parsed_result["metricdetail"]["previousperiod"]) ? "disabled" : "";
                rz_metric.metricNextPeriodStatus = String.IsNullOrEmpty((string)parsed_result["metricdetail"]["nextperiod"]) ? "disabled" : "";
                if (!String.IsNullOrEmpty((string)parsed_result["metricdetail"]["previousperiod"]))
                {
                    //parsed_result["metricdetail"]["previousperiod"] has May-2016 format
                    string[] prev_date_time = ((string)parsed_result["metricdetail"]["previousperiod"]).Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    rz_metric.lastMonthUrl = String.Format("/Metric/EditView/{0}?month={1}&year={2}", rz_metric.id, prev_date_time[0], prev_date_time[1]);
                }
                if (!String.IsNullOrEmpty((string)parsed_result["metricdetail"]["nextperiod"]))
                {
                    //parsed_result["metricdetail"]["nextperiod"] May-2016
                    string[] next_date_time = ((string)parsed_result["metricdetail"]["nextperiod"]).Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    rz_metric.nextMonthUrl = String.Format("/Metric/EditView/{0}?month={1}&year={2}", rz_metric.id, next_date_time[0], next_date_time[1]);
                }

                JArray jbldg = (JArray)parsed_result["locationdetails"];
                foreach (var res in jbldg)
                {
                    Building bldg = new Building();
                    bldg.buildingName = (string)res["dsc_mtrc_lc_bldg_name"];
                    bldg.buildingCode = (string)res["dsc_mtrc_lc_bldg_id"];
                    bldg.metricPeriodValue = (string)res["mtrc_period_val_value"];
                    bldg.metricPeriodValueID = (string)res["mtrc_period_val_id"];
                    bldg.isEditable = (string)res["bmp_is_editable_yn"] == "Y" ? true : false;
                    bldg.isManual = (string)res["bmp_is_manual_yn"] == "Y" ? true : false;
                    bldg.naAllowed = (string)res["bmp_na_allow_yn"] == "Y" ? true : false;
                    rz_metric.allBuildings = rz_metric.allBuildings + bldg.buildingName + "~";
                    rz_metric.buildingList.Add(bldg);
                }
            }
            catch {
                // Default Some Dummy Values since valid data could not be retrieved
                DateTime currentMonth = new DateTime( Convert.ToInt16(year), monthToInt(month), 1);
                rz_metric.prodName = "Red Zone";
                rz_metric.id = metric_id;
                rz_metric.metricName = "Requested Metric Period invalid or does not Exist";
                rz_metric.period_Type = METRICPERIODS.Month;
                rz_metric.metric_period_start_date = currentMonth;
                rz_metric.metric_period_end_date = currentMonth;
                rz_metric.periodName = "Requested Metric Period invalid or does not Exist";
                rz_metric.metricPeriodID = 0;
                rz_metric.metricPeriodStatus = "Closed";
                rz_metric.metricPrevPeriodStatus = "disabled";
                rz_metric.metricNextPeriodStatus = "disabled";
            }
            rz_metric.buildingList = rz_metric.buildingList.OrderBy(x => x.buildingName).ToList();
            return rz_metric;
        }

        public RZ_Metric getRZ_Metric(int metric_id, string month, string year, HttpPostedFileBase file)
        {
            RZ_Metric rz_metric = new RZ_Metric();
            string raw_data = api.getMetricperiod("Red Zone", "Month", metric_id.ToString(), month, year);
            JObject parsed_result = JObject.Parse(raw_data);
            ExcelMetric eMetric = excelReader.readValidatedExcelFile(file);
            try
            {
                rz_metric.prodName = (string)parsed_result["metricdetail"]["prod_name"];
                rz_metric.id = (int)parsed_result["metricdetail"]["mtrc_id"];
                rz_metric.metricName = (string)parsed_result["metricdetail"]["mtrc_name"];
                rz_metric.metricDataType = (string)parsed_result["metricdetail"]["data_type_token"];
                rz_metric.isNumeric = (string)parsed_result["metricdetail"]["data_type_num_yn"] == "Y" ? true : false;
                rz_metric.period_Type = METRICPERIODS.Month;
                rz_metric.metric_period_start_date = (DateTime)parsed_result["metricdetail"]["tm_per_start_dtm"];
                rz_metric.metric_period_end_date = (DateTime)parsed_result["metricdetail"]["tm_per_end_dtm"];
                rz_metric.periodName = (string)parsed_result["metricdetail"]["mtrc_period_name"];
                rz_metric.metricPeriodID = (int)parsed_result["metricdetail"]["mtrc_period_id"];
                rz_metric.na_allowed = (string)parsed_result["metricdetail"]["mtrc_period_na_allow_yn"] == "Y" ? true : false;
                rz_metric.isImportable = (string)parsed_result["metricdetail"]["mtrc_period_can_import_yn"] == "Y" ? true : false;
                rz_metric.isAuto = (string)parsed_result["metricdetail"]["mtrc_period_is_auto_yn"] == "Y" ? true : false;
                rz_metric.mtrcMinVal = (string)parsed_result["metricdetail"]["mtrc_min_val"];
                rz_metric.mtrcMaxVal = (string)parsed_result["metricdetail"]["mtrc_max_val"];
                rz_metric.maxDecPlaces = (string)parsed_result["metricdetail"]["mtrc_max_dec_places"];
                rz_metric.maxStrSize = (string)parsed_result["metricdetail"]["mtrc_max_str_size"];
                rz_metric.metricPeriodStatus = (string)parsed_result["metricdetail"]["rz_mps_status"];
                rz_metric.metricPrevPeriodStatus = String.IsNullOrEmpty((string)parsed_result["metricdetail"]["previousperiod"])? "disabled" : "";
                rz_metric.metricNextPeriodStatus = String.IsNullOrEmpty((string)parsed_result["metricdetail"]["nextperiod"])? "disabled" : "";
                if (!String.IsNullOrEmpty((string)parsed_result["metricdetail"]["previousperiod"]))
                {
                    //parsed_result["metricdetail"]["previousperiod"] has May-2016 format
                    string[] prev_date_time = ((string)parsed_result["metricdetail"]["previousperiod"]).Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    rz_metric.lastMonthUrl = String.Format("/Metric/EditView/{0}?month={1}&year={2}", rz_metric.id, prev_date_time[0], prev_date_time[1]);
                }
                if (!String.IsNullOrEmpty((string)parsed_result["metricdetail"]["nextperiod"]))
                {
                    //parsed_result["metricdetail"]["nextperiod"] has May-2016 format
                    string[] next_date_time = ((string)parsed_result["metricdetail"]["nextperiod"]).Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    rz_metric.nextMonthUrl = String.Format("/Metric/EditView/{0}?month={1}&year={2}", rz_metric.id, next_date_time[0], next_date_time[1]);
                }

                JArray jbldg = (JArray)parsed_result["locationdetails"];
                foreach (var res in jbldg)
                {
                    Building bldg = new Building();
                    bldg.buildingName = (string)res["dsc_mtrc_lc_bldg_name"];
                    bldg.buildingCode = (string)res["dsc_mtrc_lc_bldg_id"];
                    bldg.metricPeriodValue = (string)res["mtrc_period_val_value"];
                    bldg.isEditable = (string)res["bmp_is_editable_yn"] == "Y" ? true : false;
                    bldg.isManual = (string)res["bmp_is_manual_yn"] == "Y" ? true : false;
                    bldg.naAllowed = (string)res["bmp_na_allow_yn"] == "Y" ? true : false;
                    if (bldg.isEditable)
                    {
                        try
                        {
                            bldg.metricPeriodValue = eMetric.buildingList.First(x => x.buildingName.ToUpper() == bldg.buildingName.ToUpper()).metricPeriodValue;
                            if(bldg.metricPeriodValue.Equals("na")|| bldg.metricPeriodValue.Equals("n/a")|| bldg.metricPeriodValue.Equals("NA"))
                            {
                                bldg.metricPeriodValue = "N/A";
                            }
                            bldg.saveFlag = "Y";
                        }
                        catch { }
                    }                  
                    bldg.metricPeriodValueID = (string)res["mtrc_period_val_id"];
                    
                    rz_metric.allBuildings = rz_metric.allBuildings + bldg.buildingName + "~";
                    rz_metric.buildingList.Add(bldg);
                }
            }
            catch
            {
                // Default Some Dummy Values since valid data could not be retrieved
                DateTime currentMonth = new DateTime(Convert.ToInt16(year), monthToInt(month), 1);
                rz_metric.prodName = "Red Zone";
                rz_metric.id = metric_id;
                rz_metric.metricName = "Requested Metric Period does not Exist";
                rz_metric.period_Type = METRICPERIODS.Month;
                rz_metric.metric_period_start_date = currentMonth;
                rz_metric.metric_period_end_date = currentMonth;
                rz_metric.periodName = "Requested Metric Period does not Exist";
                rz_metric.metricPeriodID = 0;
            }
            rz_metric.buildingList = rz_metric.buildingList.OrderBy(x => x.buildingName).ToList();
            return rz_metric;
        }

        public string closeRZMetricPeriod(string metricId, string metricMonth, string metricYear, string userId, string metricPeriodId)
        {
            string raw_data = api.closePeriod("Red Zone", "Month", metricId, metricMonth, metricYear, userId, metricPeriodId);
            if (raw_data.ToLower().Contains("success"))
            {
                return "Success";
            }
            else
            {
                try {
                    JObject parsed_result = JObject.Parse(raw_data);
                    return (string)parsed_result["message"];
                }
                catch(Exception e)
                {
                    return e.Message;
                }
            }
        }

        public int monthToInt(string monthName) {
            int monthNo = 0;
            switch (monthName) {
                case "January": monthNo = 1;
                    break;
                case "February": monthNo = 2;
                    break;
                case "March": monthNo = 3;
                    break;
                case "April": monthNo = 4;
                    break;
                case "May": monthNo = 5;
                    break;
                case "June": monthNo = 6;
                    break;
                case "July": monthNo = 7;
                    break;
                case "August": monthNo = 8;
                    break;
                case "September": monthNo = 9;
                    break;
                case "October": monthNo = 10;
                    break;
                case "November": monthNo = 11;
                    break;
                case "December": monthNo = 12;
                    break;
                default:
                    break;
            }
            return monthNo;
        }

        public static string intToMonth(int monthNo)
        {
            string monthName = "";
            switch (monthNo)
            {
                case 1:
                    monthName = "January";
                    break;
                case 2:
                    monthName = "February";
                    break;
                case 3:
                    monthName = "March";
                    break;
                case 4:
                    monthName = "April";
                    break;
                case 5:
                    monthName = "May";
                    break;
                case 6:
                    monthName = "June";
                    break;
                case 7:
                    monthName = "July";
                    break;
                case 8:
                    monthName = "August";
                    break;
                case 9:
                    monthName = "September";
                    break;
                case 10:
                    monthName = "October";
                    break;
                case 11:
                    monthName = "November";
                    break;
                case 12:
                    monthName = "December";
                    break;
                default:
                    break;
            }
            return monthName;
        }

        public ExecutiveSummaryViewModel getExcecutiveSummaryView(string metric_id, string month, string year, string buildingID)
        {
            ExecutiveSummaryViewModel eSummary = new ExecutiveSummaryViewModel();          
            string raw_data = api.getExecSummary("Red Zone", "Month", metric_id, month, year, buildingID);
            eSummary.month = month;
            eSummary.year = year;
            eSummary.goal.score = "";
            try
            {
                List<BuildingMetricEntity> buildings = new List<BuildingMetricEntity>();
                
                JObject parsed_result = JObject.Parse(raw_data);
                JArray apiBuidings = (JArray)parsed_result["buildings"];
                JArray apiBuildingsMetrics = (JArray)parsed_result["buildingsmetrics"];
                JArray allApiMetrics = (JArray)parsed_result["metrics"];
                List<MeasuredMetric> allAvailableMetrics = new List<MeasuredMetric>();
                if (!String.IsNullOrEmpty((string)parsed_result["previousperiod"]))
                {
                    //parsed_result["metricdetail"]["previousperiod"] has May-2016 format
                    string[] prev_date_time = ((string)parsed_result["previousperiod"]).Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    eSummary.urlPrevMonth = String.Format("/Home/Index/?month={0}&year={1}", prev_date_time[0], prev_date_time[1]);
                    eSummary.statusPrevMonth = String.Empty;
                }
                else
                {
                    eSummary.statusPrevMonth = "disabled";
                }
                if (!String.IsNullOrEmpty((string)parsed_result["nextperiod"]))
                {
                    //parsed_result["metricdetail"]["nextperiod"] May-2016
                    string[] next_date_time = ((string)parsed_result["nextperiod"]).Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    eSummary.urlNextMonth = String.Format("/Home/Index/?month={0}&year={1}", next_date_time[0], next_date_time[1]);
                    eSummary.statusNextMonth = String.Empty;
                }
                else
                {
                    eSummary.statusNextMonth = "disabled";
                }
                if (allApiMetrics.HasValues)
                {
                    eSummary.goal.BuildingName = "Goal";
                    foreach (var mtr in allApiMetrics)
                    {
                        MetricHeader metricName = new MetricHeader();
                        metricName.metricName =(string)mtr["mtrc_name"];
                        metricName.metricID = (string)mtr["mtrc_id"];
                        metricName.url = String.Format("/Home/MetricSummary/?year={0}&metricID={1}", year, metricName.metricID);
                        eSummary.allMetrics.Add(metricName);
                        MeasuredMetric goalMetric = new MeasuredMetric();
                        goalMetric.metricName = metricName.metricName;
                        goalMetric.metricValue = getGoalforMetric(metricName.metricName);
                        eSummary.goal.entityMetrics.Add(goalMetric);
                    }
                    eSummary.allMetrics = eSummary.allMetrics.OrderBy(x => x.metricName).ToList();
                }
                if (apiBuidings.HasValues)
                {
                    foreach(var bldg in apiBuidings)
                    {
                       
                        if (apiBuildingsMetrics.HasValues)
                        {
                            BuildingMetricEntity b = new BuildingMetricEntity();
                            foreach (var mtr in eSummary.allMetrics)
                            {                             
                                MeasuredMetric temp = new MeasuredMetric();
                                //temp.metricColor = "#f8ffbe";          //Default backgroud for empty values
                                temp.metricName = mtr.metricName;
                                b.entityMetrics.Add(temp);
                            }
                            b.BuildingName = (string)bldg["dsc_mtrc_lc_bldg_name"];
                            b.buildingId = (string)bldg["dsc_mtrc_lc_bldg_id"];
                            b.url = String.Format("/Home/BuildingSummary/?year={0}&buildingID={1}", year, b.buildingId);


                            foreach (var mtrc in apiBuildingsMetrics)
                            {
                                
                                if ((string)mtrc["dsc_mtrc_lc_bldg_name"]== b.BuildingName)
                                {
                                    int bldngReds = 0;
                                    foreach(var tmp in b.entityMetrics)
                                    {
                                        
                                        if (tmp.metricName == ((string)mtrc["mtrc_name"]))
                                        {                                            
                                            tmp.metricValue = (string)mtrc["mtrc_period_val_value"];
                                            tmp.mtrc_id = (string)mtrc["mtrc_id"];
                                            tmp.mtrc_period_id = (string)mtrc["mtrc_period_id"];
                                            tmp.tm_period_id = (string)mtrc["tm_period_id"];
                                            tmp.dsc_mtrc_lc_bldg_id = (string)mtrc["dsc_mtrc_lc_bldg_id"];
                                            tmp.metricMonth = (string)mtrc["MonthName"];
                                            if (eSummary.allMonths.IndexOf(tmp.metricMonth) == -1)
                                            {
                                                eSummary.allMonths.Add(tmp.metricMonth);
                                            }
                                            
                                            // tmp.metricColor = getMetricColor(tmp.metricName, tmp.metricValue);
                                        }
                                        //if (tmp.metricColor.Equals(COLOR_RED)) { bldngReds++; }
                                    }
                                    if (bldngReds < 3)
                                    {
                                        b.scoreColor = COLOR_GREEN;
                                    }
                                    else if (bldngReds == 3) { b.scoreColor = COLOR_YELLOW; }
                                    else if (bldngReds == 4) { b.scoreColor = "orange"; }
                                    else { b.scoreColor = COLOR_RED; }
                                    b.score = bldngReds.ToString();
                                }
                            }
                            buildings.Add(b);
                        }
                                               
                    }      
                                 
                    eSummary.buildings = buildings.OrderBy(x => x.BuildingName).ToList(); ;
                }
               
               
            }
            catch(Exception e) { string error = e.Message; }

            return eSummary;
        }

        public BuildingSummaryViewModel getBuildingSummaryView(string year, string buildingID)
        {
            BuildingSummaryViewModel bSummary = new BuildingSummaryViewModel();
            string raw_data = api.getBuildingSummary("Red Zone", "Month", null, null, year, buildingID);
            bSummary.bName = "";
            bSummary.year = year;
            try
            {
                JObject parsed_result = JObject.Parse(raw_data);
                JArray apiMetrics = (JArray)parsed_result["metrics"];
                JArray apiBuildingsMetrics = (JArray)parsed_result["buildingsmetrics"];
                List<MeasuredRowEntity> rowMetrics = new List<MeasuredRowEntity>();//all metrics
                List<MeasuredMetric> allAvailableMetrics = new List<MeasuredMetric>();//all metric values
                if (apiMetrics.HasValues)
                {
                    foreach(var mtr in apiMetrics)
                    {
                        MeasuredRowEntity row = new MeasuredRowEntity();
                        row.rowName = (string)mtr["mtrc_name"];
                        row.rowMeasuredId = (string)mtr["mrtc_id"];
                        row.scoreGoal = "";
                        rowMetrics.Add(row);
                    }
                    bSummary.metricRows =rowMetrics;//at this point we should have all rows with metric ids in the model
                }
            }
            catch { }
            

        
            return bSummary;
        }



        //========= This Function "getMetricColor" will be replaced by either some other logic or a value returned by an API =========
        private string getMetricColor(string mName, string mValue)
        {
            string mColor = "lightgray";
            //string mColor = "blue";
            //double dValue = 0.00;
            //if (String.IsNullOrEmpty(mValue)) return "lightgray";

            //switch (mName)
            //{
            //    case "Net FTE":  //LOGIC: Any value below > 0 is Red
            //        if (double.TryParse(mValue, out dValue)) { mColor = (dValue > 0) ? COLOR_RED : COLOR_GREEN; }
            //        else { mColor = "gray"; }
            //        break;
            //    case "Turnover %":  //LOGIC: More than 7.5% is Red
            //        if (double.TryParse(mValue, out dValue)) { mColor = (dValue > 7.5) ? COLOR_RED : COLOR_GREEN; }
            //        else { mColor = "gray"; }
            //        break;
            //    case "Contribution Margin % Variance":     //LOGIC:  Ant Negative Value is Red
            //        if (double.TryParse(mValue, out dValue)) { mColor = (dValue < 0) ? COLOR_RED : COLOR_GREEN; }
            //        else { mColor = "gray"; }
            //        break;
            //    case "IT Ticket Volume":   //LOGIC: More than 25 Tickets per month are RED
            //        if (double.TryParse(mValue, out dValue)) { mColor = (dValue > 25) ? COLOR_RED : COLOR_GREEN; }
            //        else { mColor = "gray"; }
            //        break;
            //    case "Safety (TIR)":   //LOGIC: Any Number Greater than 1.45 is Red
            //        if (double.TryParse(mValue, out dValue)) { mColor = (dValue > 1.45) ? COLOR_RED : COLOR_GREEN; }
            //        else { mColor = "gray"; }
            //        break;
            //    case "Overtime %":  //LOGIC:  More than 10% is Red
            //        if (double.TryParse(mValue, out dValue)) { mColor = (dValue > 10) ? COLOR_RED : COLOR_GREEN; }
            //        else { mColor = "gray"; }
            //        break;
            //    case "Trainees %":  //LOGIC:  More than 20% is Red
            //        if (double.TryParse(mValue, out dValue)) { mColor = (dValue > 20) ? COLOR_RED : COLOR_GREEN; }
            //        else { mColor = "gray"; }
            //        break;
            //    case "Throughput Chg %":  //LOGIC: More than 20% deviation is Red 
            //        if (double.TryParse(mValue, out dValue)) { mColor = (dValue > 20 || dValue < -0.2) ? COLOR_RED : COLOR_GREEN; }
            //        else { mColor = "gray"; }
            //        break;
            //    default:
            //        mColor = "N/A";
            //        break;
            //}
            return mColor;
        }
        //========= This Function "getGoalforMetric" will be replaced by either some other logic or a value returned by an API =========
        private string getGoalforMetric(string metricName)
        {
            string value = "";
            //switch (metricName) { 
            //    case "Net FTE":
            //        value = "0.00";
            //        break;
            //    case "Turnover %":
            //        value = "7.5%";
            //        break;
            //    case "Contribution Margin % Variance ":
            //        value = "+/- Goal";
            //        break;
            //    case "IT Ticket Volume ":
            //        value = "25 month";
            //        break;
            //    case "Safety (TIR)":
            //        value = "1.45";
            //        break;
            //    case "Overtime %":
            //        value = "10.00 %";
            //        break;
            //    case "Trainees %":
            //        value = "20 %";
            //        break;
            //    case "Throughput Chg %":
            //        value = "+/- 20%";
            //        break;
            //    default:
            //        value = "N/A";
            //        break;
            //}
            return value;
        }


        public List<int> getEditableMetrics(string userName)
        {
            List<int> accessibleMetrics = new List<int>();
            JObject parsed_result = JObject.Parse(api.authorizeUser(userName));
            try {
                foreach (var res in parsed_result["authorizationdetails"])
                {
                    int mtrc_period_id = (int)res["mtrc_period_id"];
                    accessibleMetrics.Add(mtrc_period_id);
                }
            }
            catch {}            
            return accessibleMetrics;
        }
    }
}