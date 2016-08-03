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
        const string COLOR_GREEN = "#33cc00";
        const string COLOR_LIGHT_GREEN = "#b3ff99";      
        const string COLOR_RED = "#ff3300";
        const string COLOR_LIGHT_RED ="#ffbb8b";   
       
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
                rz_metric.metricPrevPeriodStatus = String.IsNullOrEmpty((string)parsed_result["metricdetail"]["previousperiod"]) || String.IsNullOrEmpty((string)parsed_result["metricdetail"]["previousperiod"]).Equals("Inactive") ? "disabled" : "";
                rz_metric.metricNextPeriodStatus = String.IsNullOrEmpty((string)parsed_result["metricdetail"]["nextperiod"]) || String.IsNullOrEmpty((string)parsed_result["metricdetail"]["nextperiod"]).Equals("Inactive") ? "disabled" : "";
               
                rz_metric.metricPrevPeriodStatus = String.IsNullOrEmpty((string)parsed_result["metricdetail"]["previousperiod"]) ? "disabled" : "";
                rz_metric.metricNextPeriodStatus = String.IsNullOrEmpty((string)parsed_result["metricdetail"]["nextperiod"]) ? "disabled" : "";
                if (!String.IsNullOrEmpty((string)parsed_result["metricdetail"]["previousperiod"]))
                {
                    //parsed_result["metricdetail"]["previousperiod"] has May-2016 format
                    string[] prev_date_time = ((string)parsed_result["metricdetail"]["previousperiod"]).Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    rz_metric.lastMonthUrl = String.Format("/Metric/EditView/{0}?month={1}&year={2}", metric_id, prev_date_time[0], prev_date_time[1]);
                }
                if (!String.IsNullOrEmpty((string)parsed_result["metricdetail"]["nextperiod"]))
                {
                    //parsed_result["metricdetail"]["nextperiod"] May-2016
                    string[] next_date_time = ((string)parsed_result["metricdetail"]["nextperiod"]).Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    rz_metric.nextMonthUrl = String.Format("/Metric/EditView/{0}?month={1}&year={2}", metric_id, next_date_time[0], next_date_time[1]);
                }
                rz_metric.prodName = (string)parsed_result["metricdetail"]["prod_name"];
                rz_metric.id = (int)parsed_result["metricdetail"]["mtrc_id"];
                rz_metric.metricName = (string)parsed_result["metricdetail"]["mtrc_prod_display_text"];
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
                if (rz_metric.metricPeriodStatus.ToUpper().Equals("CLOSED")) { rz_metric.isImportable = false; }
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
                rz_metric.metric_period_start_date = currentMonth;
                rz_metric.prodName = "Red Zone";
                rz_metric.id = metric_id;
                rz_metric.metricName = "Requested Metric Period invalid or does not Exist";
                rz_metric.period_Type = METRICPERIODS.Month;
               
                //rz_metric.metric_period_start_date = currentMonth;
                //rz_metric.metric_period_end_date = currentMonth;
               rz_metric.periodName = "Requested Metric Period invalid or does not Exist";
                
                rz_metric.metricPeriodStatus = "Closed";
                //rz_metric.metricPrevPeriodStatus = "disabled";
                //rz_metric.metricNextPeriodStatus = "disabled";
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
                rz_metric.metricPrevPeriodStatus = String.IsNullOrEmpty((string)parsed_result["metricdetail"]["previousperiod"]) || String.IsNullOrEmpty((string)parsed_result["metricdetail"]["previousperiod"]).Equals("Inactive") ? "disabled" : "";
                rz_metric.metricNextPeriodStatus = String.IsNullOrEmpty((string)parsed_result["metricdetail"]["nextperiod"]) || String.IsNullOrEmpty((string)parsed_result["metricdetail"]["nextperiod"]).Equals("Inactive") ? "disabled" : "";

                rz_metric.metricPrevPeriodStatus = String.IsNullOrEmpty((string)parsed_result["metricdetail"]["previousperiod"]) ? "disabled" : "";
                rz_metric.metricNextPeriodStatus = String.IsNullOrEmpty((string)parsed_result["metricdetail"]["nextperiod"]) ? "disabled" : "";
                if (!String.IsNullOrEmpty((string)parsed_result["metricdetail"]["previousperiod"]))
                {
                    //parsed_result["metricdetail"]["previousperiod"] has May-2016 format
                    string[] prev_date_time = ((string)parsed_result["metricdetail"]["previousperiod"]).Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    rz_metric.lastMonthUrl = String.Format("/Metric/EditView/{0}?month={1}&year={2}", metric_id, prev_date_time[0], prev_date_time[1]);
                }
                if (!String.IsNullOrEmpty((string)parsed_result["metricdetail"]["nextperiod"]))
                {
                    //parsed_result["metricdetail"]["nextperiod"] May-2016
                    string[] next_date_time = ((string)parsed_result["metricdetail"]["nextperiod"]).Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    rz_metric.nextMonthUrl = String.Format("/Metric/EditView/{0}?month={1}&year={2}", metric_id, next_date_time[0], next_date_time[1]);
                }
                rz_metric.prodName = (string)parsed_result["metricdetail"]["prod_name"];
                rz_metric.id = (int)parsed_result["metricdetail"]["mtrc_id"];
                rz_metric.metricName = (string)parsed_result["metricdetail"]["mtrc_prod_display_text"];
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
                            else if (!String.IsNullOrEmpty(rz_metric.metricDataType) &&(rz_metric.metricDataType == "pct"|| rz_metric.metricDataType == "cur" || rz_metric.metricDataType == "dec" ) &&!String.IsNullOrEmpty(rz_metric.maxDecPlaces))//if true we need to round the value from the spreadsheet
                            {
                                int decDigits = 0;
                                try
                                {
                                   decDigits = bldg.metricPeriodValue.IndexOf(".") == -1 ? decDigits : bldg.metricPeriodValue.Substring(bldg.metricPeriodValue.IndexOf(".") + 1).Length;
                                    bldg.metricPeriodValue = Math.Round(Convert.ToDouble(bldg.metricPeriodValue), Convert.ToInt32(rz_metric.maxDecPlaces)).ToString();
                                }
                                catch { }
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
                rz_metric.metric_period_start_date = currentMonth;
                rz_metric.prodName = "Red Zone";
                rz_metric.id = metric_id;
                rz_metric.metricName = "Requested Metric Period invalid or does not Exist";
                rz_metric.period_Type = METRICPERIODS.Month;

                //rz_metric.metric_period_start_date = currentMonth;
                //rz_metric.metric_period_end_date = currentMonth;
                rz_metric.periodName = "Requested Metric Period invalid or does not Exist";

                rz_metric.metricPeriodStatus = "Closed";
                //rz_metric.metricPrevPeriodStatus = "disabled";
                //rz_metric.metricNextPeriodStatus = "disabled";
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
            eSummary.goal.rowScore = 0;
            eSummary.goal.BuildingName = "Goal";
            eSummary.goalsMissedRow.BuildingName = "Goals not Met (By Metric)";        
            try
            {
                List<BuildingMetricEntity> buildings = new List<BuildingMetricEntity>();
                
                JObject parsed_result = JObject.Parse(raw_data);
                JArray apiBuidings = (JArray)parsed_result["buildings"];
                JArray apiBuildingsMetrics = (JArray)parsed_result["buildingsmetrics"];
                JArray allApiMetrics = (JArray)parsed_result["metrics"];
                List<MeasuredCellEntity> allAvailableMetrics = new List<MeasuredCellEntity>();
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
                    foreach (var mtrValue in allApiMetrics)
                    {
                        MetricHeader metricName = new MetricHeader();
                        metricName.metricName =(string)mtrValue["mtrc_prod_display_text"];
                        metricName.metricID = (string)mtrValue["mtrc_id"];
                        metricName.url = String.Format("/Home/MetricSummary/?year={0}&metricID={1}", year, metricName.metricID);
                        eSummary.allMetrics.Add(metricName);
                        MeasuredCellEntity goalMetric = new MeasuredCellEntity();
                        MeasuredCellEntity totalColMetric = new MeasuredCellEntity();
                        goalMetric.metricName = metricName.metricName;
                        goalMetric.metricValue = (string)mtrValue["mpg_display_text"];
                        totalColMetric.metricName = metricName.metricName;
                        eSummary.goal.entityMetrics.Add(goalMetric);
                        totalColMetric.score = 0;
                        eSummary.goalsMissedRow.entityMetrics.Add(totalColMetric);
                    }
                    //eSummary.allMetrics = eSummary.allMetrics.OrderBy(x => x.metricName).ToList();
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
                                MeasuredCellEntity temp = new MeasuredCellEntity();
                                //temp.metricColor = "#f8ffbe";          //Default backgroud for empty values
                                temp.metricName = mtr.metricName;
                                temp.mtrc_id = mtr.metricID;
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
                                    int tmpIndex = 0;      //To keep track of the Cell Index being processed by the foreach loop
                                    foreach(var tmp in b.entityMetrics)
                                    {
                                        if (tmp.mtrc_id == ((string)mtrc["mtrc_id"]))
                                        {   //The correct Cell Value was found to be inserted into the Building/Metric Dashboard Cell
                                            tmp.metricValue = (string)mtrc["mtrc_period_val_value"];
                                            tmp.mtrc_id = (string)mtrc["mtrc_id"];
                                            tmp.isGoalMet = (string)mtrc["mpg_mtrc_passyn"];
                                            tmp.mtrc_period_id = (string)mtrc["mtrc_period_id"];
                                            tmp.tm_period_id = (string)mtrc["tm_period_id"];
                                            tmp.dsc_mtrc_lc_bldg_id = (string)mtrc["dsc_mtrc_lc_bldg_id"];
                                            tmp.metricMonth = (string)mtrc["MonthName"];
                                            if((string)mtrc["data_type_token"]=="pct" && tmp.metricValue != "N/A")
                                            {
                                                tmp.metricValue = tmp.metricValue + "%";
                                            }
                                            if (eSummary.allMonths.IndexOf(tmp.metricMonth) == -1)
                                            {
                                                eSummary.allMonths.Add(tmp.metricMonth);
                                            }
                                            tmp.isGoalMet = (string)mtrc["mpg_mtrc_passyn"];
                                            //If the goal is not met, then increase the counter of the total goals not met
                                            if (tmp.isGoalMet.Equals("N")) { 
                                               eSummary.goalsMissedRow.entityMetrics[tmpIndex].score++;
                                               b.rowScore++;
                                            }
                                            tmp.metricColor = getMetricColor( tmp.metricValue, (string)mtrc["mpg_mtrc_passyn"], (string)mtrc["rz_mps_status"]);
                                          
                                            if (tmp.isGoalMet == "N")
                                            {
                                                eSummary.goalsMissedRow.entityMetrics.Single(x => x.metricName.ToUpper() == tmp.metricName.ToUpper()).score++;
                                                eSummary.goalsMissedRow.entityMetrics.Single(x => x.metricName.ToUpper() == tmp.metricName.ToUpper()).metricValue = eSummary.goalsMissedRow.entityMetrics.Single(x => x.metricName == tmp.metricName).score.ToString();
                                            }

                                        }
                                        //if (tmp.metricColor.Equals(COLOR_RED)) { bldngReds++; }

                                        //Increase the current Index of the Building Metric
                                        tmpIndex++;
                                    }
                                    if (bldngReds < 3)
                                    {
                                        b.scoreColor = COLOR_GREEN;
                                    }
                                    else if (bldngReds == 3) { b.scoreColor = COLOR_YELLOW; }
                                    else if (bldngReds == 4) { b.scoreColor = "orange"; }
                                    else { b.scoreColor = COLOR_RED; }
                                    b.rowScore = bldngReds;
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
            bSummary.year = year;
            try
            {
                //-------Temp hardcoding data until API provides correct Values -----------
                int intYear = 0;
                if (!(Int32.TryParse(year, out intYear))) { intYear = 9999; }

                bSummary.urlNextPeriod = String.Format("/Home/BuildingSummary/?year={0}&buildingID={1}", (intYear + 1).ToString(), buildingID);
                bSummary.urlPrevPeriod = String.Format("/Home/BuildingSummary/?year={0}&buildingID={1}", (intYear - 1).ToString(), buildingID);
                //bSummary.urlNextBuilding = String.Format("/Home/BuildingSummary/?year={0}&buildingID={1}", year, intBuilding + 1);
                //bSummary.urlPrevBuilding = String.Format("/Home/BuildingSummary/?year={0}&buildingID={1}", year, intBuilding - 1);
                bSummary.statusPrevPeriod = (intYear < 2016) ? "disabled" : "";
                bSummary.statusNextPeriod = (DateTime.Today.Year == intYear) ? "disabled" : "";
                //------- END of hardcoding data ------------------------------------------

                JObject parsed_result = JObject.Parse(raw_data);
                bSummary.bName = (string)parsed_result["dsc_mtrc_lc_bldg_name"];
                bSummary.bId = (string)parsed_result["dsc_mtrc_lc_bldg_id"];
                string[] prevNext = getPrevNextBuildingUrl(year, bSummary.bId);//prevNext[0]=prev url, prevNext[1]=next url
                bSummary.statusPrevBuilding = prevNext[0] == "disabled" ? prevNext[0] : "";
                bSummary.statusNextBuilding = prevNext[1] == "disabled" ? prevNext[1] : "";
                bSummary.urlPrevBuilding = prevNext[0];
                bSummary.urlNextBuilding = prevNext[1];
                JArray apiMetrics = (JArray)parsed_result["metrics"];
                JArray months = (JArray)parsed_result["Months"];
                JArray apiBuildingsMetrics = (JArray)parsed_result["buildingsmetrics"];
                List<MeasuredRowEntity> rowMetrics = new List<MeasuredRowEntity>();//all metrics
                List<MeasuredCellEntity> allAvailableMetrics = new List<MeasuredCellEntity>();//all metric values
                MeasuredRowEntity header = new MeasuredRowEntity();
                MeasuredRowEntity rowTotals = new MeasuredRowEntity();
                header.rowName = bSummary.bName;
                rowTotals.rowName = "Goals Missed";

                if (apiMetrics.HasValues)
                {
                    foreach (var mtr in apiMetrics)
                    {
                        MeasuredRowEntity row = new MeasuredRowEntity();
                        row.rowName = (string)mtr["mtrc_prod_display_text"];
                        //row.rowName = (string)mtr["mtrc_name"];
                        row.rowMeasuredId = (string)mtr["mtrc_id"];
                        row.scoreGoal = (string)mtr["mpg_display_text"];                              
                        row.rowURL = String.Format("/Home/MetricSummary/?year={0}&metricID={1}", year, row.rowMeasuredId);
                        if (months.HasValues)
                        {
                            foreach (var m in months)
                            {
                                MeasuredCellEntity temp = new MeasuredCellEntity();
                                MeasuredCellEntity totalCol = new MeasuredCellEntity();
                                temp.metricName = (string)m["Month"];
                                temp.metricValue = String.Empty;
                                temp.isViewable = false;
                                row.entityMetricCells.Add(temp);
                                if(header.entityMetricCells.Count< months.Count)
                                { 
                                    header.entityMetricCells.Add(temp);
                                    //We also add a corresponding Cell to the "Totals" Row
                                    totalCol.metricName = (string)m["Month"];
                                    totalCol.metricValue = "0";
                                    totalCol.score = 0;                              
                                    totalCol.isViewable = false;
                                    rowTotals.entityMetricCells.Add(totalCol);
                                }
                            }
                        }
                        if (apiBuildingsMetrics.HasValues)
                        {
                            foreach (var apiCellValue in apiBuildingsMetrics)  // For each element of the full values Array retrieved by API
                            {
                                if (row.rowMeasuredId == (string)apiCellValue["mtrc_id"])
                                {
                                    foreach(var tmp in row.entityMetricCells)
                                    {
                                        if(tmp.metricName.ToUpper()== ((string)apiCellValue["MonthName"]).ToUpper())
                                        {
                                            tmp.metricValue = (string)apiCellValue["mtrc_period_val_value"];
                                            tmp.isGoalMet = (string)apiCellValue["mpg_mtrc_passyn"];
                                            if ((string)apiCellValue["data_type_token"] =="pct"&& tmp.metricValue != "N/A")
                                            {
                                                tmp.metricValue = tmp.metricValue + "%";
                                            }
                                            tmp.metricColor = getMetricColor(tmp.metricValue, (string)apiCellValue["mpg_mtrc_passyn"], (string)apiCellValue["rz_mps_status"]);
                                            if(tmp.isGoalMet=="N")
                                            {
                                                rowTotals.entityMetricCells.Single(x => x.metricName.ToUpper() == tmp.metricName.ToUpper()).score++;
                                                rowTotals.entityMetricCells.Single(x => x.metricName.ToUpper() == tmp.metricName.ToUpper()).metricValue = rowTotals.entityMetricCells.Single(x => x.metricName == tmp.metricName).score.ToString();
                                            }
                                            //If value missed the Goal, increase the Missed Goals counter
                                            // ---- TO DO ----  ////
                                            // <--- Finished increasing th Missed Goal Counter
                                            tmp.isViewable = true;
                                        }
                                    }
                                }
                                //Set the correponding month column Goal as viewable, since there is data for that column
                                //var goalRow = rowTotals.entityMetricCells.Find(p => p.metricName == (string)apiCellValue["MonthName"]);
                                header.entityMetricCells.Find(x => x.metricName == (string)apiCellValue["MonthName"]).isViewable = true;
                                rowTotals.entityMetricCells.Find(p => p.metricName == (string)apiCellValue["MonthName"]).isViewable = true;
                                //goalRow.isViewable = true;                                
                            }
                            // Populate the number of columns that are "Viewable"
                        }
                        rowMetrics.Add(row);
                    }                  
                    bSummary.buildingHeadings = header;
                    bSummary.buildingScoreRow = rowTotals;
                    bSummary.activeColumns = bSummary.buildingScoreRow.entityMetricCells.Where(x => (x.isViewable == true)).Count();
                    bSummary.metricRows = rowMetrics;//at this point we should have all rows with metric ids and months in the model
                    // Loop through all the Goals Missed Row and set the color accordingly
                    //foreach (var goalRowColTotal in bSummary.buildingScoreRow.entityMetricCells)
                    //{ 
                    //    try{
                    //        int totalReds = Convert.ToInt16(goalRowColTotal.metricValue);
                    //        if ( goalRowColTotal.metricValue.Equals("---") || totalReds < 3) { goalRowColTotal.metricColor = "lightgreen"; }
                    //        else if (totalReds == 3) { goalRowColTotal.metricColor = "yellow"; }
                    //        else if (totalReds == 4) { goalRowColTotal.metricColor = "orange"; }
                    //        else { goalRowColTotal.metricColor = "red"; }
                    //    }
                    //    catch{
                    //        goalRowColTotal.metricColor = "lightgreen";
                    //    }                    
                    //}
                }

            }
            catch (Exception e) { string error = e.Message; }
            return bSummary;
        }

        public MetricSummaryViewModel getMetricSummaryView(string year, string metricID)
        {
            MetricSummaryViewModel mSummary = new MetricSummaryViewModel();
            string raw_data = api.getMetricSummary("Red Zone", "Month", metricID, year);
            mSummary.metricName = "";
            mSummary.year = year;
            try
            {
                JObject parsed_result = JObject.Parse(raw_data);
                JArray apiBuildings = (JArray)parsed_result["buildings"];
                JArray months = (JArray)parsed_result["Months"];
                JArray apiBuildingsMetrics = (JArray)parsed_result["buildingsmetrics"];
                List<MeasuredRowEntity> rowMetrics = new List<MeasuredRowEntity>();
                List<MeasuredCellEntity> allAvailableMetrics = new List<MeasuredCellEntity>();
                mSummary.metricName = (string)parsed_result["mtrc_prod_display_text"];
                mSummary.metricID = (string)parsed_result["mtrc_id"];
                string mGoalText = (string)parsed_result["mpg_display_text"];
                string[] prevNext = getPrevNextMetricsUrl(year, mSummary.metricID);//prevNext[0]=prev url, prevNext[1]=next url
                mSummary.statusPrevMetric = prevNext[0] == "disabled" ? prevNext[0] : "";
                mSummary.statusNextMetric = prevNext[1] == "disabled" ? prevNext[1] : "";
                mSummary.urlPrevMetric = getPrevNextMetricsUrl(year, mSummary.metricID)[0];
                mSummary.urlNextMetric = getPrevNextMetricsUrl(year, mSummary.metricID)[1];
                MeasuredRowEntity header = new MeasuredRowEntity();
                header.rowName = "Buildings";
                MeasuredRowEntity goal = new MeasuredRowEntity();
                goal.rowName = "Goal";
                if (apiBuildings.HasValues)
                {
                    foreach(var b in apiBuildings)
                    {
                        MeasuredRowEntity row = new MeasuredRowEntity();
                        row.rowName = (string)b["dsc_mtrc_lc_bldg_name"];
                        row.rowMeasuredId = (string)b["dsc_mtrc_lc_bldg_id"];
                        row.rowURL = String.Format("/Home/BuildingSummary/?year={0}&buildingID={1}", year, row.rowMeasuredId);
                        if (months.HasValues)
                        {
                            foreach (var m in months)
                            {
                                MeasuredCellEntity temp = new MeasuredCellEntity();
                                MeasuredCellEntity goalCell = new MeasuredCellEntity();
                                temp.metricName = (string)m["Month"];
                                temp.metricValue = String.Empty;
                                temp.metricDoubleValue = -99999;
                                temp.isViewable = false;                                
                                row.entityMetricCells.Add(temp);

                                goalCell.metricMonth = (string)m["Month"];
                                goalCell.metricValue = mGoalText;              //getMonthGoal((string)m["Month"]);
                                if (header.entityMetricCells.Count < months.Count)
                                { header.entityMetricCells.Add(temp); }
                                if (goal.entityMetricCells.Count < months.Count)
                                { goal.entityMetricCells.Add(goalCell); }
                            }
                        }
                        if (apiBuildingsMetrics.HasValues)
                        {
                            foreach (var apiCellValue in apiBuildingsMetrics)
                            {
                                if (row.rowMeasuredId == (string)apiCellValue["dsc_mtrc_lc_bldg_id"])
                                {
                                    foreach (var tmp in row.entityMetricCells)
                                    {
                                        if (tmp.metricName.ToUpper() == ((string)apiCellValue["MonthName"]).ToUpper())
                                        {
                                            tmp.metricValue = (string)apiCellValue["mtrc_period_val_value"];
                                            tmp.metricDoubleValue = tmp.metricValue == "N/A" ? 9999 : Convert.ToDouble(tmp.metricValue);
                                            tmp.isViewable = true;
                                            if ((string)apiCellValue["data_type_token"] == "pct" && tmp.metricValue != "N/A")
                                            {
                                                tmp.metricValue = tmp.metricValue + "%";
                                            }
                                            tmp.metricColor = getMetricColor(tmp.metricValue, (string)apiCellValue["mpg_mtrc_passyn"], (string)apiCellValue["rz_mps_status"]);
                                        }
                                        
                                    }
                                }
                                //Set the correponding month column Goal as viewable, since there is data for that column
                                var goalRow = goal.entityMetricCells.Find(p => p.metricMonth == (string)apiCellValue["MonthName"]);
                                goalRow.isViewable = true;
                            }
                        }
                        rowMetrics.Add(row);
                    }
                    mSummary.rowGoal = goal;
                    mSummary.rowHeadings = header;
                    mSummary.viewableColumns = mSummary.rowHeadings.entityMetricCells.Where(x => (x.isViewable == true)).Count();
                    mSummary.metricRows = rowMetrics;
                }


            }
            catch(Exception e)
            { string error = e.Message; }

            return mSummary;
        }



        //========= This Function "getMetricColor" returns the color the metric value cell should have based on the value, isGoalMet flag and metric status =========
        private string getMetricColor(string mValue, string isGoalMet,string status)
        {
            string mColor = "lightgray";

            if (String.IsNullOrEmpty(mValue)|| mValue == "N/A") return "lightgray";
            if (status == "Open")
            {
                if (isGoalMet == "Y") return COLOR_LIGHT_GREEN;
                if (isGoalMet == "N") return COLOR_LIGHT_RED;
            }
            if (isGoalMet == "Y") return COLOR_GREEN;
            if (isGoalMet == "N") return COLOR_RED;

            return mColor;
        }

        //This method returns the list of all metrics user is authorized to edit. 
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


        //This is a HELPER method that should determine what the next and prev url for metric summary.
        //It returns an array of 2 records. [0]=prev url, [1]=next url
        public string[] getPrevNextMetricsUrl(string year, string metricID)
        {
            string[] prevNext = new string[] { "disabled", "disabled" };
            try
            {
                JObject parsed_result = JObject.Parse(api.getMetricname("Red Zone", "Month"));
                JArray apiMetrics = (JArray)parsed_result["metriclist"];
                if (apiMetrics.HasValues)
                {
                    JToken current = (JToken)apiMetrics.FirstOrDefault(x => x["mtrc_id"].ToString() == metricID);
                    if (current == apiMetrics.Last)
                    {
                        JToken next = apiMetrics.First;
                        JToken prev = current.Previous;
                        prevNext[0] = String.Format("/Home/MetricSummary/?year={0}&metricID={1}", year, (string)prev["mtrc_id"]);
                        prevNext[1] = String.Format("/Home/MetricSummary/?year={0}&metricID={1}", year, (string)next["mtrc_id"]);
                    }
                    else if (current == apiMetrics.First)
                    {
                        JToken next = current.Next;
                        JToken prev = apiMetrics.Last;
                        prevNext[0] = String.Format("/Home/MetricSummary/?year={0}&metricID={1}", year, (string)prev["mtrc_id"]);
                        prevNext[1] = String.Format("/Home/MetricSummary/?year={0}&metricID={1}", year, (string)next["mtrc_id"]);
                    }
                    else
                    {
                        JToken next = current.Next;
                        JToken prev = current.Previous;
                        prevNext[0] = String.Format("/Home/MetricSummary/?year={0}&metricID={1}", year, (string)prev["mtrc_id"]);
                        prevNext[1] = String.Format("/Home/MetricSummary/?year={0}&metricID={1}", year, (string)next["mtrc_id"]);
                    }

                }
            }
            catch { }
            
            return prevNext;

        }

        //This is a HELPER method that should determine what the next and prev url for building summary.
        //It returns an array of 2 records. [0]=prev url, [1]=next url
        public string[] getPrevNextBuildingUrl(string year, string buildingID)
        {
            string[] prevNext = new string[] {"disabled","disabled"};
            try
            {
                JObject parsed_result = JObject.Parse(api.getAllBuildings("Red Zone", "Month", year));
                JArray apiBuildings = (JArray)parsed_result["resource"];
                if (apiBuildings.HasValues)
                {
                    JToken current = (JToken)apiBuildings.FirstOrDefault(x => x["dsc_mtrc_lc_bldg_id"].ToString() == buildingID);
                    JToken next;
                    JToken prev;
                    if (current == apiBuildings.Last)
                    {
                        next = apiBuildings.First;
                        prev = current.Previous;
                    }
                    else if (current == apiBuildings.First)
                    {
                        next = current.Next;
                        prev = apiBuildings.Last;
                        
                    }
                    else
                    {
                        next = current.Next;
                        prev = current.Previous;                        
                    }
                    prevNext[0] = String.Format("/Home/BuildingSummary/?year={0}&buildingID={1}", year, (string)prev["dsc_mtrc_lc_bldg_id"]);
                    prevNext[1] = String.Format("/Home/BuildingSummary/?year={0}&buildingID={1}", year, (string)next["dsc_mtrc_lc_bldg_id"]);
                }
            }//end of try
            catch{ }
            
            return prevNext;
        }
    }
}