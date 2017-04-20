﻿using Newtonsoft.Json.Linq;
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
        const string COLOR_LIGHT_RED = "#ffbb8b";
        const string COLOR_ORANGE = "orange";
        //---------- END OF CONSTANTS SECTION -----------------

        public RZ_Metric getRZ_Metric(int metric_id, string month, string year)
        {
            RZ_Metric rz_metric = new RZ_Metric();
            rz_metric.allBuildings = String.Empty;
            rz_metric.maxDecPlaces = "0";
            string raw_data = api.getMetricperiod("Red Zone", "Month", metric_id.ToString(), month, year);


            // At this point the json result can be empty (If no data was found) or an Error if an exception was caught 
            // or an actual jason message if The API was successful

            //if (raw_data.Substring(0, 5).Equals("ERROR")) {
            //    HttpContext.Current.Server.Transfer("/Error/Index?ErrorMsg=" + raw_data.Substring(7));
            //}


            try
            {
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
                rz_metric.isModelValid = String.IsNullOrEmpty((string)parsed_result["metricdetail"]["prod_name"]) ? false : true;  // If jason product value can't be parse we assume no data could be retrieved.
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
                //if (rz_metric.isImportable)
                //{
                //    try
                //    {
                //        ExcelBuilder.CreateExcelTemplate(rz_metric.metricName, year, month, rz_metric.allBuildings.Split(new[] { '~' }, StringSplitOptions.RemoveEmptyEntries));
                //    }
                //    catch { }
                //}
                //Verify if the Entry Period for the metric has been exceeded is the metric is not closed yet
                if (!rz_metric.metricPeriodStatus.ToUpper().Equals("CLOSED"))
                {
                    DateTime now = DateTime.Now;
                    DateTime startOfMonth = new DateTime(now.Year,now.Month,1);
                    rz_metric.isEntryPeriodExceeded = !(rz_metric.metric_period_start_date.AddMonths(2) > startOfMonth);
                }
                else {
                    rz_metric.isEntryPeriodExceeded = false;
                }
            }
            catch
            {
                // Default Some Dummy Values since valid data could not be retrieved
                rz_metric.isModelValid = false;
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

        public RZ_Metric getRZ_Metric(int metric_id, string month, string year, HttpPostedFileBase file)
        {
            RZ_Metric rz_metric = new RZ_Metric();
            string raw_data = api.getMetricperiod("Red Zone", "Month", metric_id.ToString(), month, year);
            JObject parsed_result = JObject.Parse(raw_data);
            string eValue = String.Empty;
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
                rz_metric.isModelValid = String.IsNullOrEmpty((string)parsed_result["metricdetail"]["prod_name"]) ? false : true;  // If jason product value can't be parse we assume no data could be retrieved.
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
                rz_metric.metricPrevPeriodStatus = String.IsNullOrEmpty((string)parsed_result["metricdetail"]["previousperiod"]) ? "disabled" : "";
                rz_metric.metricNextPeriodStatus = String.IsNullOrEmpty((string)parsed_result["metricdetail"]["nextperiod"]) ? "disabled" : "";

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
                            eValue = eMetric.buildingList.First(x => x.buildingName.ToUpper() == bldg.buildingName.ToUpper()).metricPeriodValue;
                            if (!String.IsNullOrEmpty(eValue))
                            {
                                bldg.metricPeriodValue = eValue;
                                bldg.saveFlag = "Y";
                                eValue = String.Empty;
                            }
                            if (bldg.metricPeriodValue.Equals("na") || bldg.metricPeriodValue.Equals("n/a") || bldg.metricPeriodValue.Equals("NA"))
                            {
                                bldg.metricPeriodValue = "N/A";
                            }
                            else if (!String.IsNullOrEmpty(rz_metric.metricDataType) && (rz_metric.metricDataType == "pct" || rz_metric.metricDataType == "cur" || rz_metric.metricDataType == "dec") && !String.IsNullOrEmpty(rz_metric.maxDecPlaces))//if true we need to round the value from the spreadsheet
                            {
                                int decDigits = 0;
                                try
                                {
                                    decDigits = bldg.metricPeriodValue.IndexOf(".") == -1 ? decDigits : bldg.metricPeriodValue.Substring(bldg.metricPeriodValue.IndexOf(".") + 1).Length;
                                    bldg.metricPeriodValue = Math.Round(Convert.ToDouble(bldg.metricPeriodValue), Convert.ToInt32(rz_metric.maxDecPlaces)).ToString();
                                }
                                catch { }
                            }
                            //bldg.saveFlag = "Y";
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
                try
                {
                    JObject parsed_result = JObject.Parse(raw_data);
                    return (string)parsed_result["message"];
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
        }

        public ExecutiveSummaryViewModel getExcecutiveSummaryView(string metric_id, string month, string year, string buildingID, bool filterBuildings)
        {
            ExecutiveSummaryViewModel eSummary = new ExecutiveSummaryViewModel();
            List<BuildingMetricEntity> buildings = new List<BuildingMetricEntity>();
            eSummary.month = month;
            eSummary.year = year;
            eSummary.goalsRow.rowScore = 0;
            eSummary.total = 0;
            eSummary.goalsRow.BuildingName = "Goal";
            eSummary.goalsMissedRow.BuildingName = "Goals Missed";
            eSummary.goalsMissedRow.scoreDisplayClass = "";
            //Retrieve User Information and get alist of the buildings assigned to that user

            dscUser currentUser = new dscUser(HttpContext.Current.User.Identity.Name);
            List<string> userBuildings = new List<string>();
            if (currentUser.buildings.Count == 0)
            {
                eSummary.canFilterBuildings = false;
                filterBuildings = false;
            }
            else
            {
                eSummary.canFilterBuildings = true;
                userBuildings = currentUser.buildings.Select(x => x.id).ToList();
            }

            string raw_data = String.Empty;
            try
            {
                raw_data = api.getExecSummary("Red Zone", "Month", metric_id, month, year, buildingID);
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
                else { eSummary.statusPrevMonth = "disabled"; }
                if (!String.IsNullOrEmpty((string)parsed_result["nextperiod"]))
                {
                    //parsed_result["metricdetail"]["nextperiod"] May-2016
                    string[] next_date_time = ((string)parsed_result["nextperiod"]).Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    eSummary.urlNextMonth = String.Format("/Home/Index/?month={0}&year={1}", next_date_time[0], next_date_time[1]);
                    eSummary.statusNextMonth = String.Empty;
                }
                else { eSummary.statusNextMonth = "disabled"; }

                if (allApiMetrics.HasValues)
                {
                    foreach (var mtrValue in allApiMetrics)
                    {
                        MetricHeader metricName = new MetricHeader();
                        metricName.metricName = (string)mtrValue["mtrc_prod_display_text"];
                        metricName.metricDescription = (string)mtrValue["mtrc_period_desc"];
                        metricName.metricID = (string)mtrValue["mtrc_id"];
                        metricName.url = String.Format("/Home/MetricSummary/?year={0}&metricID={1}", year, metricName.metricID);
                        eSummary.allMetrics.Add(metricName);
                        MeasuredCellEntity goalMetric = new MeasuredCellEntity();
                        MeasuredCellEntity totalColMetric = new MeasuredCellEntity();
                        goalMetric.metricName = metricName.metricName;
                        goalMetric.metricValue = ((string)mtrValue["mpg_display_text"]).Replace("<=", "&le;").Replace(">=", "&ge;");
                        totalColMetric.metricName = metricName.metricName;
                        eSummary.goalsRow.entityMetrics.Add(goalMetric);
                        totalColMetric.score = 0;
                        //totalColMetric.metricColor = "lightgray";
                        eSummary.goalsMissedRow.entityMetrics.Add(totalColMetric);
                    }
                    //eSummary.allMetrics = eSummary.allMetrics.OrderBy(x => x.metricName).ToList();
                }
                if (apiBuidings.HasValues)
                {
                    foreach (var bldg in apiBuidings)
                    {
                        string building_Id = (string)bldg["dsc_mtrc_lc_bldg_id"];
                        //Apply Filter. Process the building only if it contains Metrics and there is no filter applied (or the builing Id is in the user's list of buildings)
                        if (apiBuildingsMetrics.HasValues && (!filterBuildings || userBuildings.Contains(building_Id)))
                        {
                            BuildingMetricEntity b = new BuildingMetricEntity();
                            b.scoreDisplayClass = "";
                            foreach (var mtr in eSummary.allMetrics)
                            {
                                MeasuredCellEntity temp = new MeasuredCellEntity();
                                //temp.metricColor = "#f8ffbe";          //Default backgroud for empty values
                                temp.metricName = mtr.metricName;
                                temp.mtrc_id = mtr.metricID;
                                temp.displayClass = "cell-NoValue";     //Default (Will remain if no value is not found)
                                b.entityMetrics.Add(temp);
                            }
                            b.BuildingName = (string)bldg["dsc_mtrc_lc_bldg_name"];
                            b.buildingId = (string)bldg["dsc_mtrc_lc_bldg_id"];
                            b.url = String.Format("/Home/BuildingSummary/?year={0}&buildingID={1}", year, b.buildingId);


                            foreach (var mtrc in apiBuildingsMetrics)
                            {

                                if ((string)mtrc["dsc_mtrc_lc_bldg_name"] == b.BuildingName)
                                {
                                    int tmpIndex = 0;      //To keep track of the Cell Index being processed by the foreach loop
                                    foreach (var tmp in b.entityMetrics)
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
                                            if ((string)mtrc["data_type_token"] == "pct" && tmp.metricValue != "N/A" && !String.IsNullOrEmpty(tmp.metricValue))
                                            {
                                                tmp.metricValue = tmp.metricValue + "%";
                                            }
                                            if (eSummary.allMonths.IndexOf(tmp.metricMonth) == -1)
                                            {
                                                eSummary.allMonths.Add(tmp.metricMonth);
                                            }
                                            tmp.isGoalMet = (string)mtrc["mpg_mtrc_passyn"];
                                            //If the goal is not met, then increase the counter of the total goals not met
                                            if (tmp.isGoalMet.Equals("N"))
                                            {
                                                eSummary.goalsMissedRow.entityMetrics[tmpIndex].score++;
                                                eSummary.total++;
                                                b.rowScore++;
                                            }
                                            //tmp.metricColor = getMetricColor( tmp.metricValue, (string)mtrc["mpg_mtrc_passyn"], (string)mtrc["rz_mps_status"]);
                                            tmp.displayClass = getMetricDisplayClass(tmp.metricValue, (string)mtrc["mpg_mtrc_passyn"], (string)mtrc["rz_mps_status"]);
                                        }
                                        //if (tmp.metricColor.Equals(COLOR_RED)) { bldngReds++; }

                                        //Increase the current Index of the Building Metric
                                        tmpIndex++;
                                    }
                                    // Set the Building Row Display Class based on the score it has
                                    if (b.rowScore == 0)
                                    {//Verify that the building has actual values (not only empty strings)
                                        foreach (var x in b.entityMetrics)
                                        {
                                            if (!String.IsNullOrEmpty(x.metricValue))
                                            {
                                                b.scoreDisplayClass = "Score-Met";
                                                break;
                                            }
                                        }
                                    }
                                    else if (b.rowScore < 3) { b.scoreDisplayClass = "Score-Met"; }
                                    else if (b.rowScore == 3) { b.scoreDisplayClass = "Score-Red3"; }
                                    else if (b.rowScore == 4) { b.scoreDisplayClass = "Score-Red4"; }
                                    else { b.scoreDisplayClass = "Score-Red5"; }
                                }
                            }
                            buildings.Add(b);
                        }

                    }
                    eSummary.buildings = buildings.OrderBy(x => x.BuildingName).ToList(); ;
                    eSummary.isModelValid = true;
                }
            }
            catch (Exception e)
            {
                eSummary.isModelValid = false;
                eSummary.modelMessage = e.Message;
            }

            return eSummary;
        }

        public BuildingSummaryViewModel1 getBuildingSummaryView(string p_year, string p_buildingID, string p_currentUserSSO, bool filterByBldng)
        {
            const bool showAllMonths = true;      // Flag to control whether al months are shown Vs only those that have data
            const int PRIORITY_METRICS = 4;        // First Four columns are priority Metrics and will have special display Classes (For showing "Black Box" border)

            //Define and Initializa Model Object Components
            BuildingSummaryViewModel1 bSummary = new BuildingSummaryViewModel1();   // Main Metric Model Object. Returned back to calling method
            MeasuredRowEntity rowHeader = new MeasuredRowEntity();                // Model Contains one Header Row
            MeasuredRowEntity rowTotals = new MeasuredRowEntity();                // Model Contains one Totals Row
            List<MeasuredRowEntity> metricsRowList = new List<MeasuredRowEntity>();         // Building Metrics (Row) List
            List<MeasuredCellEntity> metricValueCellList = new List<MeasuredCellEntity>();  // RowCellColection


            //-- User Building Filtering set up ---------------------
            dscUser currentUser = new dscUser(p_currentUserSSO);
            List<string> userBuildings = new List<string>();
            if (currentUser.buildings.Count == 0)
            {
                bSummary.canFilterBuildings = false;
                filterByBldng = false;
            }
            else
            {
                bSummary.canFilterBuildings = true;
                if (filterByBldng)
                {
                    userBuildings = currentUser.buildings.Select(x => x.id).ToList();     //Set the list of buildings for the user only if we need to apply selection filter by Building
                }
            }
            //-- End of User Building Filtering set up ----------------

            bSummary.year = p_year;
            rowHeader.rowName = bSummary.bName;
            rowTotals.rowName = "Building Score";
            string raw_data = String.Empty;
            try
            {
                //------- Initialize the System Current Date/Time Deppendent Values -----------
                int intYear = 0;
                if (!(Int32.TryParse(p_year, out intYear))) { intYear = 9999; }
                bSummary.urlNextPeriod = String.Format("/Home/BuildingSummary/?year={0}&buildingID={1}", (intYear + 1).ToString(), p_buildingID);
                bSummary.urlPrevPeriod = String.Format("/Home/BuildingSummary/?year={0}&buildingID={1}", (intYear - 1).ToString(), p_buildingID);
                bSummary.statusPrevPeriod = (intYear <= 2016) ? "disabled" : "";
                bSummary.statusNextPeriod = (DateTime.Today.Year == intYear) ? "disabled" : "";
                //------- END of hardcoding data ------------------------------------------

                raw_data = api.getBuildingSummary("Red Zone", "Month", null, null, p_year, p_buildingID);
                JObject parsed_result = JObject.Parse(raw_data);
                bSummary.bName = (string)parsed_result["dsc_mtrc_lc_bldg_name"];
                bSummary.bId = (string)parsed_result["dsc_mtrc_lc_bldg_id"];
                string[] prevNext = getPrevNextBuildingUrl(p_year, bSummary.bId, userBuildings);//prevNext[0]=prev url, prevNext[1]=next url
                bSummary.statusPrevBuilding = (prevNext[0] == "disabled") ? prevNext[0] : "";
                bSummary.statusNextBuilding = (prevNext[1] == "disabled") ? prevNext[1] : "";
                bSummary.urlPrevBuilding = prevNext[0];
                bSummary.urlNextBuilding = prevNext[1];
                JArray apiMetrics = (JArray)parsed_result["metrics"];
                JArray apiMonths = (JArray)parsed_result["Months"];
                JArray apiBuildingsMetricValues = (JArray)parsed_result["buildingsmetrics"];





                //========================================================================================================//
                //--------------- START OF PROCESS THAT CAPTURES THE METRIC VIEW (RZTABLE)

                if (apiMetrics.HasValues && apiMonths.HasValues)
                {                    
                    //Set up the Widths of the Table Columns
                    string metColWidth = (apiMetrics.Count == 0)?"71.00%":(0.7501 / apiMetrics.Count).ToString("0.00%");
                    bSummary.summaryByMetric.colWidths.Add("14.99%");      //Set up the width of The Heading Column (First Column)
                    for (int x=0; x < apiMetrics.Count; x++) {
                        bSummary.summaryByMetric.colWidths.Add(metColWidth);  //Set up the width of each of the Metric Columns
                    }
                    bSummary.summaryByMetric.colWidths.Add("14.01%");      //Set up the width of The Heading Column (First Column)

                    //Setups the headings and Totals cell for each of the mandatory Rows if Neeed
                    bSummary.summaryByMetric.metricHeadingsRow.rowTotalsCell = new rzCell { cellValue = "Building Score", cellDispValue = "Building Score", cellURL = "", cellOwner = "" };
                    bSummary.summaryByMetric.goalHeadingsRow.rowHeaderCell = new rzCell { cellValue = "Goal", cellDispValue = "Goal"};                    
                    bSummary.summaryByMetric.goalsMissedRow.rowHeaderCell = new rzCell { cellValue="Goals Missed", cellDispValue="Goals Missed", cellURL="", cellOwner=""};

                    //Add the Month Rows
                    int dataRowIndex = 0;
                    foreach (var m in apiMonths)
                    {
                        rzRow dRow = new rzRow { rowOrder = dataRowIndex};
                        //dRow.rowHeaderCell = new rzCell { cellValue = (string)m["Month"], cellDispValue = getMonthShortName((string)m["Month"])};
                        dRow.rowHeaderCell = new rzCell { cellValue = (string)m["Month"], cellDispValue = (string)m["Month"] };
                        bSummary.summaryByMetric.dataRows.Add(dRow);     //Add one data Row to the table for each month retrieved from the API                       
                        dataRowIndex++;
                    }
                    //------------ Finished Adding all Rows-------- //
                    // ------- Add all Metric Data Columns/cells for each Row ----------
                    int colIndex = 0;
                    foreach(var mtrc in apiMetrics){
                        //Each Metric will add a column to every single row in the Table
                        string m_Id = (string)mtrc["mtrc_id"];                           //Metric Id                  //Used on row1
                        string m_Name = (string)mtrc["mtrc_name"];                       //Metric Name                //Used on row1
                        string m_DispName = (string)mtrc["mtrc_prod_display_text"];      //Metric Display Name        //Used on Row1

                        //Add the Metric Col to the Header Row
                        bSummary.summaryByMetric.metricHeadingsRow.rowDataColumns.Add(new rzCell {
                            cellValue = m_Name,
                            cellDispValue = m_DispName,
                            cellURL = String.Format("/Home/MetricSummary/?year={0}&metricID={1}", p_year, m_Id),
                            cellOwner = AppCode.Util.getMetricMeetingOwner(m_DispName),
                            caption = (string)mtrc["mtrc_period_desc"]
                        });
                        //Add the Metric Col to the Goals Row
                        bSummary.summaryByMetric.goalHeadingsRow.rowDataColumns.Add(new rzCell {
                            cellValue = (string)mtrc["mpg_display_text"],
                            cellDispValue = encodeGoalValues( ((string)mtrc["mpg_display_text"])),
                            displayClass = ""     //To erase the "gray" backgroud default class color
                        });

                        //Add the Current Metric Column to Each and Every Data Row
                        int m_cellOrder = 0; //mtrc_prod_display_order
                        Int32.TryParse((string)mtrc["mpg_display_text"], out m_cellOrder);    //Try to get the Display Order of the column (Default is zero if orders can't be determined)
                        foreach (rzRow tableRow in bSummary.summaryByMetric.dataRows){
                            tableRow.rowDataColumns.Add(new rzCell());  //Add a default empty cell
                        }

                        //Add the Current Metric Column to the Totals Row
                        bSummary.summaryByMetric.goalsMissedRow.rowDataColumns.Add(new rzCell());
                        colIndex++;
                    }
                    // ------- Finished Adding all Metric Data Columns/cells for each Row, table array is now complete ----------

                    //----------------------------------------------------------------------------------------------------------
                    //----- Start populating all the Table Cell Values and properties from the "buildingmetrics" array ---------

                    int maxColumns = bSummary.summaryByMetric.dataColsCount;
                    int maxRows = bSummary.summaryByMetric.dataRowsCount;
                    foreach (var apiCellValue in apiBuildingsMetricValues)  // For each element of the full values Array retrieved by API
                    {  //Process this apiBuildingMetricValues entry into a new rzCell and add it to the Table on the correct row/col index
                        int valDecPlaces = 0;      //Harcoded value for now, not used. Number of Decimal places to display for numeric Values

                        // ------------------- Map Cell Values ------------------------------------- \
                        rzCell mvCell = new rzCell{
                            cellMPVid = (string)apiCellValue["mtrc_period_val_id"],
                            cellValue = (string)apiCellValue["mtrc_period_val_value"],
                            cellDispValue = getFormattedValue((string)apiCellValue["mtrc_period_val_value"], (string)apiCellValue["data_type_token"], valDecPlaces),
                            cellStatus = (string)apiCellValue["rz_mps_status"],
                            isGoalMet = ((string)apiCellValue["mpg_mtrc_passyn"]).ToUpper().Equals("Y"),
                            cellLastUpdt = AppCode.Util.formatDate((string)apiCellValue["last_updated"], "MMM dd, yyyy"),
                            displayClass = getMetricDisplayClass((string)apiCellValue["mtrc_period_val_value"], (string)apiCellValue["mpg_mtrc_passyn"], (string)apiCellValue["rz_mps_status"]),
                            cellMonth = (string)apiCellValue["MonthName"]
                        };
                        
                        int reasonCount;
                        if (!Int32.TryParse((string)apiCellValue["reason_count"], out reasonCount)) { reasonCount = 0; };
                        mvCell.apInfo.hasReasons = (reasonCount > 0);
                        // Get The Cell Metric Info
                        mvCell.cellMetricInfo.mtrc_id = (string)apiCellValue["mtrc_id"];
                        mvCell.cellMetricInfo.metricPeriodId = (string)apiCellValue["mtrc_period_id"];
                        mvCell.cellMetricInfo.metricName = (string)apiCellValue["mtrc_name"];
                        //mvCell.cellMetricInfo.metricDoubleValue = 0;

                        // Get The Cell 'Action Plan' Info  
                        mvCell.apInfo.rz_bap_id = (string)apiCellValue["rz_bap_id"];            //Building Action Plan Id
                        mvCell.apInfo.rz_bapm_id =(string)apiCellValue["rz_bapm_id"];           //Building Action Plan Metric Id
                        mvCell.apInfo.rz_bapm_status = (string)apiCellValue["rz_bapm_status"];  //Building Action Plan Metric Status

                        mvCell.cellNextAction = getMPV_NextAction(mvCell.apInfo.rz_bapm_status, mvCell.cellMetricInfo.metricPeriodId, p_buildingID, p_currentUserSSO);
                        mvCell.cellNextActionLink = mvCell.cellNextAction.Equals("View AP") ? "blue" : "red";
                        if (String.IsNullOrEmpty(mvCell.apInfo.rz_bapm_status)) {
                            mvCell.caption = "[STS: " + (string)apiCellValue["rz_mps_status"] + "]";
                        }
                        else {
                            mvCell.caption = "[STS: " + (string)apiCellValue["rz_mps_status"] + " - " + (string)apiCellValue["rz_bapm_status"] + "]";
                        }

                        ////Get The current Cells, Metric Meeting Date
                        //string currentMetric = (string)apiCellValue["mtrc_prod_display_text"];
                        //switch(currentMetric) {
                        //    case "Safety":  //Tuesday
                        //        mvCell.cellMetricInfo.metricMeetingDate = getNthDayofMonth(4, "Tuesday", (string)apiCellValue["MonthName"], p_year);
                        //        break;

                        //    case "OT":       //Wednesday
                        //    case "Net FTE":
                        //        mvCell.cellMetricInfo.metricMeetingDate = getNthDayofMonth(4, "Wednesday", (string)apiCellValue["MonthName"], p_year);
                        //        break;

                        //    case "Turnover":  //Thursday
                        //    case "Trainees":
                        //        mvCell.cellMetricInfo.metricMeetingDate = getNthDayofMonth(4, "Thursday", (string)apiCellValue["MonthName"], p_year);
                        //        break;

                        //    case "IT Tickets":  //Friday
                        //        mvCell.cellMetricInfo.metricMeetingDate = getNthDayofMonth(4, "Friday", (string)apiCellValue["MonthName"], p_year);
                        //        break;

                        //    case "Volume":
                        //    case "Financial":    //No meeting
                        //        mvCell.cellMetricInfo.metricMeetingDate = "Not Required";
                        //        break;
                        //    default:
                        //        break;                        
                        //}
                        // ------------------- Finished Mapping all indvidual Cell Values ----------------------------------- /


                        //  Populate all the Row Totals (Building Score for each Month Row which is the number of "reds" for closed metrics only)

                        //Add the Cell to the corresponding Row/Column Position
                        int iMonthRowIndex = bSummary.summaryByMetric.getRowIndex((string)apiCellValue["MonthName"]);        //Row Index of this Metric Period Value
                        int iMetricColIndex = bSummary.summaryByMetric.getMetricIndex((string)apiCellValue["mtrc_prod_display_text"]);    //Col Index of this Metric Period Value
                        bSummary.summaryByMetric.dataRows[iMonthRowIndex].rowDataColumns[iMetricColIndex] = mvCell;

                    }

                    //Loop throug all Rows and all Columns to populate the Row Monthly Score, The Row Status and the Column Metrics Missed Totals
                    int _rowIndex = 0;
                    foreach (var _month in bSummary.summaryByMetric.dataRows)
                    {
                        int _metricIndex = 0;
                        int bScore = 0;
                        string rowStatus = "Inactive";
                        foreach (var _metric in bSummary.summaryByMetric.dataRows[_rowIndex].rowDataColumns)
                        {
                            if (!_metric.isGoalMet && !_metric.cellStatus.Equals("Open") && !_metric.cellDispValue.Equals("N/A") && !_metric.cellDispValue.Equals(""))
                            {
                                bScore++;
                                //Increase Metric Goals missed in the year on the "Goals Missed Row"
                                bSummary.summaryByMetric.goalsMissedRow.rowDataColumns[_metricIndex].cellMetricInfo.metricDoubleValue++;
                            }
                            //Recalculate the Current Row Status based on the current Cell Status
                            if (!rowStatus.Equals("Mixed"))   //Don't bother recalculate the status if it's already in "Mixed"
                            {
                                if (_metric.cellStatus.Equals("Open"))
                                {
                                    rowStatus = rowStatus.Equals("Closed")?"Mixed":"Open";
                                }
                                else if (_metric.cellStatus.Equals("Closed"))
                                {
                                    rowStatus = rowStatus.Equals("Open")?"Mixed":"Closed";
                                }
                            }
                            _metricIndex++;
                        }
                        _month.rowTotalsCell.cellDispValue = bScore.ToString("0");
                        //Set The correct Row Totals Display Class
                        if (bScore < 3) { _month.rowTotalsCell.displayClass = "Score-Met"; }
                        else if (bScore == 3) { _month.rowTotalsCell.displayClass = "Score-Red3"; }
                        else if (bScore == 4) { _month.rowTotalsCell.displayClass = "Score-Red4"; }
                        else { _month.rowTotalsCell.displayClass = "Score-Red5"; }
                        _month.rowHeaderCell.cellStatus = rowStatus;
                        _month.rowHeaderCell.caption = !rowStatus.Equals("Closed") ? "Metrics for this month have not been finalized" : "";
                        _rowIndex++;
                    }

                    foreach (var _metric in bSummary.summaryByMetric.goalsMissedRow.rowDataColumns) {
                        _metric.cellDispValue = _metric.cellMetricInfo.metricDoubleValue.ToString("0");
                    }

                    //Finally Add the "Display Classes" for all the priority Metric Columns
                    for (int x = 0; x < PRIORITY_METRICS; x++) {
                        //Make sure The Data Columns are more than the priority Columns to avoid array out of Index errors
                        if (bSummary.summaryByMetric.metricHeadingsRow.rowDataColumns.Count() > x) {
                            bSummary.summaryByMetric.metricHeadingsRow.rowDataColumns[x].displayClass = bSummary.summaryByMetric.metricHeadingsRow.rowDataColumns[x].displayClass + " " + "dataHdrCol" + (x+1).ToString("00");
                            bSummary.summaryByMetric.goalHeadingsRow.rowDataColumns[x].displayClass = bSummary.summaryByMetric.goalHeadingsRow.rowDataColumns[x].displayClass + " " + "dataGoalCol" + (x+1).ToString("00");
                            foreach(rzRow valRow in bSummary.summaryByMetric.dataRows){
                              valRow.rowDataColumns[x].displayClass = valRow.rowDataColumns[x].displayClass + " " + "dataValCol" + (x+1).ToString("00");
                            }
                            bSummary.summaryByMetric.goalsMissedRow.rowDataColumns[x].displayClass = bSummary.summaryByMetric.goalsMissedRow.rowDataColumns[x].displayClass + " " + "dataFootCol" + (x+1).ToString("00");
                        }
                    }

                }
                else
                {
                    // There is no data to populate on the Table.
                    bSummary.isModelValid = false;
                    bSummary.modelValidationMsg = "Data Model Contains no Metric Data";
                    return bSummary;
                }
                //---------------END OF PROCESS THAT CAPTURES THE METRIC VIEW (RZTABLE)
                //========================================================================================================//





                
                if (apiMetrics.HasValues)
                {
                    foreach (var mtr in apiMetrics)
                    {
                        MeasuredRowEntity row = new MeasuredRowEntity();
                        row.rowName = (string)mtr["mtrc_prod_display_text"];
                        //row.rowName = (string)mtr["mtrc_name"];
                        row.rowMeasuredId = (string)mtr["mtrc_id"];
                        row.scoreGoal = ((string)mtr["mpg_display_text"]).Replace("<=", "&le;").Replace(">=", "&ge;");
                        row.rowURL = String.Format("/Home/MetricSummary/?year={0}&metricID={1}", p_year, row.rowMeasuredId);
                        row.rowOwner = AppCode.Util.getMetricMeetingOwner(row.rowName);
                        if (apiMonths.HasValues)
                        {
                            foreach (var m in apiMonths)
                            {
                                MeasuredCellEntity temp = new MeasuredCellEntity();
                                MeasuredCellEntity headerCol = new MeasuredCellEntity();
                                MeasuredCellEntity totalCol = new MeasuredCellEntity();
                                temp.metricName = (string)m["Month"];
                                temp.metricValue = String.Empty;
                                temp.isViewable = showAllMonths ? true : false;
                                temp.nextCellAction = "";
                                temp.nextCellActionLink = "";
                                row.entityMetricCells.Add(temp);
                                if (rowHeader.entityMetricCells.Count < apiMonths.Count)
                                {
                                    //Add a corresponding Cell to the "Headers", "Totals" and "Actions" Row                                    
                                    headerCol.metricName = (string)m["Month"];
                                    headerCol.metricValue = String.Empty;
                                    headerCol.isViewable = showAllMonths ? true : false;
                                    rowHeader.entityMetricCells.Add(headerCol);

                                    totalCol.metricName = (string)m["Month"];
                                    totalCol.metricValue = "0";
                                    totalCol.score = 0;
                                    totalCol.isViewable = showAllMonths ? true : false;
                                    rowTotals.entityMetricCells.Add(totalCol);
                                    //Set the Cell Value and URL to use for the Actions Row.
                                    //rowActions.entityMetricCells.Add(getActionDataforMonth((string)m["Month"], bSummary.year));
                                }
                            }
                        }
                        if (apiBuildingsMetricValues.HasValues)
                        {
                            foreach (var apiCellValue in apiBuildingsMetricValues)  // For each element of the full values Array retrieved by API
                            {
                                if (row.rowMeasuredId == (string)apiCellValue["mtrc_id"])
                                {
                                    foreach (var tmp in row.entityMetricCells)
                                    {
                                        if (tmp.metricName.ToUpper() == ((string)apiCellValue["MonthName"]).ToUpper())
                                        {
                                            string cellStatus = (string)apiCellValue["rz_mps_status"];
                                            int reasonCount;
                                            if (!Int32.TryParse((string)apiCellValue["reason_count"], out reasonCount)) { reasonCount = 0; };
                                            tmp.hasReasons = (reasonCount > 0);
                                            tmp.cellStatus = cellStatus;
                                            tmp.isGoalMet = (string)apiCellValue["mpg_mtrc_passyn"];
                                            tmp.cellValueId = (string)apiCellValue["mtrc_period_val_id"];

                                            tmp.rz_bap_id = (string)apiCellValue["rz_bap_id"];
                                            tmp.rz_bapm_id = (string)apiCellValue["rz_bapm_id"];
                                            tmp.rz_bapm_status = (string)apiCellValue["rz_bapm_status"];
                                            tmp.metricLastUpdt = AppCode.Util.formatDate((string)apiCellValue["last_updated"], "MMM dd, yyyy");
                                            tmp.mtrc_period_id = (string)apiCellValue["mtrc_period_id"];
                                            //..... Get the value in a formatted way
                                            int valDecPlaces = 0;      //Harcoded value for now, not used
                                            //if (!(Int32.TryParse("2", out valDecPlaces))) { valDecPlaces = 0; }
                                            tmp.metricValue = getFormattedValue((string)apiCellValue["mtrc_period_val_value"], (string)apiCellValue["data_type_token"], valDecPlaces);
                                            //......Enf of Getting Formatted Value

                                            //tmp.metricColor = getMetricColor(tmp.metricValue, (string)apiCellValue["mpg_mtrc_passyn"], (string)apiCellValue["rz_mps_status"]);
                                            tmp.displayClass = getMetricDisplayClass(tmp.metricValue, (string)apiCellValue["mpg_mtrc_passyn"], cellStatus);
                                            tmp.metricMonth = (string)apiCellValue["MonthName"];


                                            if (tmp.isGoalMet == "N" && !cellStatus.Equals("Open"))
                                            {// Open Periods do not count towards the totals as they are not complete yet
                                                rowTotals.entityMetricCells.Single(x => x.metricName.ToUpper() == tmp.metricName.ToUpper()).score++;
                                                rowTotals.entityMetricCells.Single(x => x.metricName.ToUpper() == tmp.metricName.ToUpper()).metricValue = rowTotals.entityMetricCells.Single(x => x.metricName == tmp.metricName).score.ToString();
                                                row.redTotals++;
                                            }
                                            //If value missed the Goal, increase the Missed Goals counter
                                            // ---- TO DO ----  ////
                                            // <--- Finished increasing the Missed Goal Counter
                                            tmp.isViewable = true;
                                            tmp.nextCellAction = getMPV_NextAction(tmp.rz_bapm_status, tmp.mtrc_period_id, p_buildingID, p_currentUserSSO);


                                            // *********************************************************
                                            if (tmp.nextCellAction.Equals("View AP")) { tmp.nextCellActionLink = "blue"; }
                                            else { tmp.nextCellActionLink = "red"; }

                                            //tmp.nextCellActionLink = "";          // ***** Add Logic Here to calculate the next action Link ******
                                            // *********************************************************

                                            //Set the Scheduled Owners Meeting Date for this metric cell
                                            string currentMetric = (string)apiCellValue["mtrc_prod_display_text"];
                                            switch (currentMetric)
                                            {
                                                case "Safety":  //Tuesday
                                                    tmp.metricMeetingDate = getNthDayofMonth(4, "Tuesday", (string)apiCellValue["MonthName"], p_year);
                                                    break;

                                                case "OT":       //Wednesday
                                                case "Net FTE":
                                                    tmp.metricMeetingDate = getNthDayofMonth(4, "Wednesday", (string)apiCellValue["MonthName"], p_year);
                                                    break;

                                                case "Turnover":  //Thursday
                                                case "Trainees":
                                                    tmp.metricMeetingDate = getNthDayofMonth(4, "Thursday", (string)apiCellValue["MonthName"], p_year);
                                                    break;

                                                case "IT Tickets":  //Friday
                                                    tmp.metricMeetingDate = getNthDayofMonth(4, "Friday", (string)apiCellValue["MonthName"], p_year);
                                                    break;

                                                case "Volume":
                                                case "Financial":    //No meeting
                                                    tmp.metricMeetingDate = "Not Required";
                                                    break;
                                                default:
                                                    break;
                                            }


                                        }
                                        if (String.IsNullOrEmpty(tmp.displayClass))
                                        {
                                            tmp.displayClass = "cell-NoValue";  //Default the display class to the "No value" color schema if none is defined
                                        }
                                    }
                                }
                                //Set the correponding month column Goal as viewable, since there is data for that column
                                //var goalRow = rowTotals.entityMetricCells.Find(p => p.metricName == (string)apiCellValue["MonthName"]);
                                rowHeader.entityMetricCells.Find(x => x.metricName == (string)apiCellValue["MonthName"]).isViewable = true;
                                rowTotals.entityMetricCells.Find(p => p.metricName == (string)apiCellValue["MonthName"]).isViewable = true;
                                //goalRow.isViewable = true;                                
                            }
                            //After all Metric Values have been processed, loop thorugh the (Metrics) Row and set the appropiate Score Color Display Class
                            int totalIndex = 0;
                            foreach (var temp in rowTotals.entityMetricCells)
                            {
                                if (temp.score == 0)
                                {
                                    temp.displayClass = "";  //Default to "Empty Class Display"
                                    //Loop through all the buildings at the current Index. If a value if found for any building, set it as score met
                                    foreach (var x in metricsRowList)
                                    {
                                        if (!String.IsNullOrEmpty(x.entityMetricCells[totalIndex].metricValue))
                                        {
                                            //Current Column contains at least one value. Set Dispay class and exit the loop
                                            temp.displayClass = "Score-Met";
                                            break;
                                        }
                                    }
                                }
                                else if (temp.score < 3) { temp.displayClass = "Score-Met"; }
                                else if (temp.score == 3) { temp.displayClass = "Score-Red3"; }
                                else if (temp.score == 4) { temp.displayClass = "Score-Red4"; }
                                else { temp.displayClass = "Score-Red5"; }
                                totalIndex++;
                            }
                        }
                        metricsRowList.Add(row);
                    }

                    ////Update the Column Month Header Row Status based on the Individual Building Cell Status
                    int headerIndex = 0;
                    foreach (MeasuredCellEntity headerColumn in rowHeader.entityMetricCells)
                    {  //Loop through all header metric cells 
                        headerColumn.metricMonth = headerColumn.metricName;
                        headerColumn.metricName = getMonthShortName(headerColumn.metricName);
                        string columStatus = "Inactive";
                        foreach (MeasuredRowEntity bMetricRow in metricsRowList)
                        {
                            //Loop thrugh all Metric rows to inspect each cell at the given index

                            string cellStatus = (bMetricRow.entityMetricCells[headerIndex].cellStatus == null) ? "Inactive" : bMetricRow.entityMetricCells[headerIndex].cellStatus;
                            if (columStatus != "Mixed")
                            {

                                if (cellStatus.Equals("Open"))
                                {
                                    if (columStatus.Equals("Closed")) { columStatus = "Mixed"; }
                                    else { columStatus = "Open"; }
                                }
                                else if (cellStatus.Equals("Closed"))
                                {
                                    if (columStatus.Equals("Open")) { columStatus = "Mixed"; }
                                    else { columStatus = "Closed"; }
                                }
                            }

                        }
                        headerColumn.cellStatus = columStatus;
                        if (columStatus.Equals("Inactive"))
                        {
                            //Set the corresponding values on the TotalsRow Column 
                            rowTotals.entityMetricCells[headerIndex].cellStatus = "Inactive";
                            rowTotals.entityMetricCells[headerIndex].metricValue = "";
                            rowTotals.entityMetricCells[headerIndex].displayClass = "cell-NoValue";
                        }
                        headerIndex++;
                    }
                    bSummary.buildingHeadings = rowHeader;
                    bSummary.buildingScoreRow = rowTotals;
                    bSummary.viewableColumns = bSummary.buildingHeadings.entityMetricCells.Where(x => (x.isViewable == true)).Count();
                    bSummary.metricRows = metricsRowList;//at this point we should have all rows with metric ids and months in the model
                    //bSummary.buildingActionsRow = rowActions;
                }
                bSummary.isModelValid = true;
            }
            catch (Exception e)
            {
                bSummary.isModelValid = false;
                bSummary.modelValidationMsg = e.Message;    //Log into the model itself Any error while parsing/populationg the model Data
            }
            return bSummary;
        }

        private string encodeGoalValues(string goalValue)
        {
            return goalValue.Replace("<=", "&le;").Replace(">=", "&ge;");
        }
        //--------------------------------------------------------------------------------------------------
        public string getMPV_NextAction(string ap_status, string mtrc_period_id, string building_ID, string currentUserSSO)
        {
            dscUser currentUser = new dscUser(currentUserSSO);  //Retrieve the currently Logged-on User Info
            bool hasBuildingAccess = currentUser.hasBuilding(building_ID);
            bool hasMetricAssigned = currentUser.hasReviewerMetric(mtrc_period_id);
            string nextAction = "---";

            switch (ap_status)
            {
                case "Not Started":
                    //Check if User is Submitter for this building or Admin
                    if ((currentUser.hasRole("RZ_AP_SUBMITTER") && hasBuildingAccess) || currentUser.hasRole("RZ_ADMIN"))
                    { nextAction = "Start AP"; }
                    break;
                case "WIP":         //Same as Rejected
                    if ((currentUser.hasRole("RZ_AP_SUBMITTER") && hasBuildingAccess) || currentUser.hasRole("RZ_ADMIN"))
                    { nextAction = "Continue AP"; }
                    else
                        if (currentUser.hasRole("RZ_BLDG_USER") && hasBuildingAccess)
                        {
                            nextAction = "View AP";
                        }
                        else if ((ap_status == "Rejected" && currentUser.hasRole("RZ_AP_REVIEWER") && hasMetricAssigned))
                        {
                            nextAction = "View AP";
                        }
                    break;
                case "Rejected":
                    if ((currentUser.hasRole("RZ_AP_SUBMITTER") && hasBuildingAccess) || currentUser.hasRole("RZ_ADMIN"))
                    { nextAction = "Rework AP"; }
                    else
                        if (currentUser.hasRole("RZ_BLDG_USER") && hasBuildingAccess)
                        {
                            nextAction = "View AP";
                        }
                        else if ((ap_status == "Rejected" && currentUser.hasRole("RZ_AP_REVIEWER") && hasMetricAssigned))
                        {
                            nextAction = "View AP";
                        }
                    break;
                case "Ready For Review":
                    if ((currentUser.hasRole("RZ_AP_REVIEWER") && hasMetricAssigned) || currentUser.hasRole("RZ_ADMIN"))
                    {
                        nextAction = "Review AP";
                    }
                    else if ((currentUser.hasRole("RZ_BLDG_USER") || currentUser.hasRole("RZ_AP_SUBMITTER")) && hasBuildingAccess)
                    {
                        nextAction = "View AP";
                    }
                    break;
                case "Approved":
                    if (((currentUser.hasRole("RZ_BLDG_USER") || currentUser.hasRole("RZ_AP_SUBMITTER")) && hasBuildingAccess) || currentUser.hasRole("RZ_ADMIN") || (currentUser.hasRole("RZ_AP_REVIEWER") && hasMetricAssigned))
                    {
                        nextAction = "View AP";
                    }
                    break;
                case "Expired":
                    if (((currentUser.hasRole("RZ_BLDG_USER") || currentUser.hasRole("RZ_AP_SUBMITTER")) && hasBuildingAccess) || currentUser.hasRole("RZ_ADMIN") || (currentUser.hasRole("RZ_AP_REVIEWER") && hasMetricAssigned))
                    {
                        nextAction = "View AP";
                    }
                    break;
                default:
                    nextAction = "N/A";
                    break;
            }

            //---- Test only to validate user class methods -----
            //bool userIsUser = currentUser.hasRole("RZ_USER");
            //bool userIsAdmin = currentUser.hasRole("RZ_ADMIN");
            //bool userHasOT = currentUser.hasReviewerMetric("3");
            //bool userHasVolume = currentUser.hasReviewerMetric("6");

            return nextAction;
        }
        private string getMPV_NextActionLink(string nextAction)
        {
            string nextActionLink = String.Empty;
            //switch (nextAction)
            //{
            //    case "Start AP":
            //        //When Clicked:
            //        //localStorage.setItem("mpvStatus", "Not Started");
            //        //localStorage.setItem("bapmId", $cellClicked.find('#bapm_id').val());
            //        nextActionLink = "/MPVreasons/Assigment/" + mpvId + "?mpId=" + getMPid() + "&returnUrl=" + backUrl;;
            //        break;
            //    case "Review AP":
            //        nextActionLink = "";
            //        break;
            //    case "Continue AP":  //Same as View AP
            //    case "View AP":
            //                localStorage.setItem("bapmId", $cellClicked.find('#bapm_id').val());
            //               var bapmId = $cellClicked.find('#bapm_id').val();
            //               window.location.href = "/ActionPlan/viewEdit/?" + "mp_id=" + getMPid() + "&bldg_id=" + getBuildingId() + "&bapm_id=" + bapmId;

            //               nextActionLink = "";
            //        break;
            //    default:
            //        break;
            //}
            return nextActionLink;
        }
        private string getFormattedValue(string pValue, string pDataType, int valDecPlaces)
        {
            //Data types are "str", "char", "int", "dec", "cur", "pct"
            //For now we only have logic to format Percent Types
            if (String.IsNullOrEmpty(pValue)) { return ""; }
            if (pValue.Equals("N/A")) { return pValue; }

            switch (pDataType)
            {
                case "pct":
                    return pValue + "%";
                case "int":
                case "dec":
                    break;
                case "cur":
                    return "$" + pValue;
                default:   //For all string tupes
                    break;
            }

            return pValue;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //THIS METHOD WILL BE DEPRECATED AS SOON AS THE GOOGLE SHEETS PROCESS IS REMOVED FROM THE BUSINESS REQUIREMENTS
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private MeasuredCellEntity getActionDataforMonth(string actionMonth, string actionYear)
        {
            const string urlBase = "http://goo.gl/forms/";
            MeasuredCellEntity actionCell = new MeasuredCellEntity();
            DateTime metricDate = new DateTime(Convert.ToInt16(actionYear), monthToInt(actionMonth), 1);
            DateTime todayDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            //The Action Cell is visible only if it's the last month or the current month as long as they are closed.
            actionCell.isViewable = ((metricDate.AddMonths(1) == todayDate) || (metricDate == todayDate)) ? true : false;
            // If it's before the 15th of the month then we allow the current and the last two months to be visible
            if (DateTime.Today.Day < 16) { if (metricDate.AddMonths(2) == todayDate) { actionCell.isViewable = true; } }

            actionCell.metricMonth = actionMonth;
            actionCell.metricName = getMonthShortName(actionMonth);
            actionCell.metricValue = actionCell.isViewable ? "Action Required [ Click Here ]" : "";

            switch (actionMonth.ToUpper())
            {
                case "JANUARY":
                    actionCell.cellValueURL = "";
                    break;
                case "FEBRUARY":
                    actionCell.cellValueURL = "";
                    break;
                case "MARCH":
                    actionCell.cellValueURL = urlBase + "nb12u9n1UO";
                    break;
                case "APRIL":
                    actionCell.cellValueURL = urlBase + "WDlp7YwLlZ";
                    break;
                case "MAY":
                    actionCell.cellValueURL = urlBase + "YNks1ixnyE";
                    break;
                case "JUNE":
                    actionCell.cellValueURL = urlBase + "S04aEDUqvW";
                    break;
                case "JULY":
                    actionCell.cellValueURL = urlBase + "F68sI8WHeS";
                    break;
                case "AUGUST":
                    actionCell.cellValueURL = urlBase + "HUoFwJS06H";
                    break;
                case "SEPTEMBER":
                    actionCell.cellValueURL = urlBase + "A0HgyAeQ9t";
                    break;
                case "OCTOBER":
                    actionCell.cellValueURL = urlBase + "QzlBPKTXFU";
                    break;
                case "NOVEMBER":
                    actionCell.cellValueURL = urlBase + "j3ehSK7uX2";
                    break;
                case "DECEMBER":
                    actionCell.cellValueURL = urlBase + "E9oK92nwL9";
                    break;
                default:
                    break;
            }
            return actionCell;
        }

        public MetricSummaryViewModel getMetricSummaryView(string year, string metricID, string sortDir, bool filterByBldng)
        {
            MetricSummaryViewModel mSummary = new MetricSummaryViewModel();
            string raw_data = api.getMetricSummary("Red Zone", "Month", metricID, year);
            mSummary.metricName = "";
            mSummary.year = year;

            //-- User Building Filtering set up ---------------------
            dscUser currentUser = new dscUser(HttpContext.Current.User.Identity.Name);
            List<string> userBuildings = new List<string>();
            if (currentUser.buildings.Count == 0)
            {   
                mSummary.canFilterRows = false;
                filterByBldng = false;
            }
            else
            {
                mSummary.canFilterRows = true;
                userBuildings = currentUser.buildings.Select(x => x.id).ToList();     //Set the list of buildings for the user only if we need to apply selection filter by Building
            }
            //-- End of User Building Filtering set up ----------------

            try
            {
                //------- Initialize the System Current Date/Time Deppendent Values -----------
                int intYear = 0;
                if (!(Int32.TryParse(year, out intYear))) { intYear = 9999; }
                //For next and previous period, the Metric Id remains static. Only the year will change
                mSummary.urlNextPeriod = String.Format("/Home/MetricSummary/?year={0}&metricID={1}", (intYear + 1).ToString(), metricID);
                mSummary.urlPrevPeriod = String.Format("/Home/MetricSummary/?year={0}&metricID={1}", (intYear - 1).ToString(), metricID);
                mSummary.statusPrevPeriod = (intYear <= 2016) ? "disabled" : "";
                //For the month of January, the current year is unavailable as there is no data available until February
                mSummary.statusNextPeriod = ((DateTime.Today.Year == intYear) || (DateTime.Today.Year - 1 == intYear && DateTime.Today.Month == 01 )) ? "disabled" : "";
                //------- END of hardcoding data ------------------------------------------



                JObject parsed_result = JObject.Parse(raw_data);
                JArray apiBuildings = (JArray)parsed_result["buildings"];
                JArray months = (JArray)parsed_result["Months"];
                JArray apiBuildingsMetrics = (JArray)parsed_result["buildingsmetrics"];
                List<MeasuredRowEntity> rowMetrics = new List<MeasuredRowEntity>();
                List<MeasuredCellEntity> allAvailableMetrics = new List<MeasuredCellEntity>();
                mSummary.metricName = (string)parsed_result["mtrc_prod_display_text"];
                mSummary.metricID = (string)parsed_result["mtrc_id"];
                string mGoalText = ((string)parsed_result["mpg_display_text"]).Replace("<=", "&le;").Replace(">=", "&ge;");
                string[] prevNext = getPrevNextMetricsUrl(year, mSummary.metricID);//prevNext[0]=prev url, prevNext[1]=next url
                mSummary.statusPrevMetric = prevNext[0] == "disabled" ? prevNext[0] : "";
                mSummary.statusNextMetric = prevNext[1] == "disabled" ? prevNext[1] : "";
                mSummary.urlPrevMetric = getPrevNextMetricsUrl(year, mSummary.metricID)[0];
                mSummary.urlNextMetric = getPrevNextMetricsUrl(year, mSummary.metricID)[1];
                MeasuredRowEntity header = new MeasuredRowEntity();
                mSummary.missedGoals.rowName = "Goals Missed";
                header.rowName = "Buildings";
                header.displayClass = "";
                MeasuredRowEntity goal = new MeasuredRowEntity();
                goal.rowName = "Goal";
                if (apiBuildings.HasValues)
                {
                    foreach (var b in apiBuildings)
                    {
                        string building_Id = (string)b["dsc_mtrc_lc_bldg_id"];
                        //Process/Save this building record only if No filter required or if the current builing Id is in the user's list of buildings
                        if (!filterByBldng || userBuildings.Contains(building_Id))
                        {

                            MeasuredRowEntity row = new MeasuredRowEntity();
                            row.rowName = (string)b["dsc_mtrc_lc_bldg_name"];
                            row.rowMeasuredId = (string)b["dsc_mtrc_lc_bldg_id"];
                            row.rowURL = String.Format("/Home/BuildingSummary/?year={0}&buildingID={1}", year, row.rowMeasuredId);
                            //row.displayClass = "cell-NoValue";
                            if (months.HasValues)
                            {
                                foreach (var m in months)
                                {
                                    MeasuredCellEntity hdrCell = new MeasuredCellEntity();
                                    MeasuredCellEntity goalCell = new MeasuredCellEntity();
                                    MeasuredCellEntity buildingValCell = new MeasuredCellEntity();
                                    MeasuredCellEntity missedGoalCell = new MeasuredCellEntity();
                                    buildingValCell.metricName = (string)m["Month"];
                                    buildingValCell.metricValue = String.Empty;
                                    buildingValCell.displayClass = "cell-NoValue";
                                    buildingValCell.metricDoubleValue = (sortDir == "ASC") ? 99999 : -99999;
                                    buildingValCell.isViewable = false;
                                    row.entityMetricCells.Add(buildingValCell);

                                    hdrCell.metricName = (string)m["Month"];
                                    hdrCell.metricValue = String.Empty;
                                    hdrCell.displayClass = "";
                                    hdrCell.metricDoubleValue = (sortDir == "ASC") ? 99999 : -99999;
                                    hdrCell.isViewable = false;
                                    goalCell.metricMonth = (string)m["Month"];
                                    goalCell.metricValue = mGoalText;              //getMonthGoal((string)m["Month"]);
                                    missedGoalCell.metricName = (string)m["Month"];
                                    missedGoalCell.score = 0;
                                    missedGoalCell.metricValue = "0";
                                    if (header.entityMetricCells.Count < months.Count)
                                    { header.entityMetricCells.Add(hdrCell); }
                                    if (goal.entityMetricCells.Count < months.Count)
                                    { goal.entityMetricCells.Add(goalCell); }
                                    if (mSummary.missedGoals.entityMetricCells.Count < months.Count)
                                    {
                                        mSummary.missedGoals.entityMetricCells.Add(missedGoalCell);
                                    }
                                }
                            }

                            if (apiBuildingsMetrics.HasValues)
                            {
                                foreach (var apiCellValue in apiBuildingsMetrics)
                                {
                                    if (row.rowMeasuredId == (string)apiCellValue["dsc_mtrc_lc_bldg_id"])
                                    {
                                        //int tmpIndex = 0;      //To keep track of the Cell Index being processed by the 'foreach' loop
                                        foreach (var tmp in row.entityMetricCells)
                                        {
                                            if (tmp.metricName.ToUpper() == ((string)apiCellValue["MonthName"]).ToUpper())
                                            {
                                                tmp.displayClass = "cell-NoValue";
                                                tmp.metricValue = (string)apiCellValue["mtrc_period_val_value"];
                                                if (tmp.metricValue == "N/A")
                                                {
                                                    if (sortDir == "ASC") { tmp.metricDoubleValue = 99998; }

                                                    if (sortDir == "DESC") { tmp.metricDoubleValue = -99998; }

                                                }
                                                else if (String.IsNullOrEmpty(tmp.metricValue))
                                                {
                                                    tmp.metricDoubleValue = sortDir == "ASC" ? 99999 : -99999;
                                                }
                                                else
                                                {
                                                    tmp.metricDoubleValue = Convert.ToDouble(tmp.metricValue);
                                                }
                                                tmp.isViewable = true;
                                                tmp.isGoalMet = (string)apiCellValue["mpg_mtrc_passyn"];
                                                if ((string)apiCellValue["data_type_token"] == "pct" && tmp.metricValue != "N/A" && !String.IsNullOrEmpty(tmp.metricValue))
                                                {
                                                    tmp.metricValue = tmp.metricValue + "%";
                                                }
                                                //tmp.metricColor = getMetricColor(tmp.metricValue, (string)apiCellValue["mpg_mtrc_passyn"], (string)apiCellValue["rz_mps_status"]);
                                                tmp.displayClass = getMetricDisplayClass(tmp.metricValue, (string)apiCellValue["mpg_mtrc_passyn"], (string)apiCellValue["rz_mps_status"]);
                                                if (tmp.isGoalMet == "N")
                                                {
                                                    mSummary.missedGoals.entityMetricCells.Single(x => x.metricName.ToUpper() == tmp.metricName.ToUpper()).score++;
                                                    mSummary.missedGoals.entityMetricCells.Single(x => x.metricName.ToUpper() == tmp.metricName.ToUpper()).metricValue = mSummary.missedGoals.entityMetricCells.Single(x => x.metricName == tmp.metricName).score.ToString();

                                                    //mSummary.missedGoals.entityMetricCells[tmpIndex].score++;
                                                    row.redTotals++;
                                                }
                                            }
                                        }
                                    }
                                    //Set the correponding month column Goal as viewable, since there is data for that column
                                    var goalRow = goal.entityMetricCells.Find(p => p.metricMonth == (string)apiCellValue["MonthName"]);
                                    header.entityMetricCells.Single(x => x.metricName == (string)apiCellValue["MonthName"]).isViewable = true;
                                    goalRow.isViewable = true;
                                }
                            }
                            rowMetrics.Add(row);
                        }
                        //Finished Processing one instance of the "apiBuildings" array
                    } //--- Foreach() ENDS

                    mSummary.rowGoal = goal;
                    //Before Assigning the Header Information to the model, switch the month display names to Short Names
                    int columnIndex = 0;
                    foreach (MeasuredCellEntity hdrMonth in header.entityMetricCells)
                    {
                        hdrMonth.metricMonth = getMonthShortName(hdrMonth.metricName);
                        //hdrMonth.score      //Use score field to store number of buildings that have values
                        int valuesFound = 0;
                        foreach (var x in rowMetrics)
                        {
                            if (!String.IsNullOrEmpty(x.entityMetricCells[columnIndex].metricValue))
                            {
                                valuesFound++;
                            }
                        }
                        hdrMonth.score = valuesFound;
                        columnIndex++;
                    }
                    //---- Finished populating nuber of values per column
                    mSummary.rowHeadings = header;
                    mSummary.viewableColumns = mSummary.rowHeadings.entityMetricCells.Where(x => (x.isViewable == true)).Count();
                    mSummary.metricRows = rowMetrics;
                }  // End of statement:  if(apiBuildings.HasValues)
            }
            catch (Exception e)
            { string error = e.Message; }

            return mSummary;
        }
        //=====================================================================================================================
        public List<MPReason> getMetricPeriodReasons(string mpId)
        {
            //Define and Initializa Model Object Components
            List<MPReason> reasonList = new List<MPReason>();
            MPReason mpReason;

            string raw_data = String.Empty;
            try
            {
                raw_data = api.getMetricPeriodReasons(mpId);
                JObject parsed_result = JObject.Parse(raw_data);
                JArray apiMPReasons = (JArray)parsed_result["reasons"];

                if (apiMPReasons.HasValues)
                {
                    foreach (var reason in apiMPReasons)
                    {
                        int order = 0;
                        mpReason = new MPReason();
                        mpReason.mpvr_Comment = "";
                        mpReason.reason_id = (string)reason["mpr_id"];                //Primary Table Key Field
                        mpReason.metric_period_id = (string)reason["mtrc_period_id"];
                        mpReason.reason_text = (string)reason["mpr_display_text"];
                        mpReason.reason_order = (string)reason["mpr_display_order"];
                        mpReason.reason_description = (string)reason["mpr_desc"];
                        mpReason.reason_std_yn = (string)reason["mpr_std_yn"];
                        mpReason.times_used = (string)reason["usedby"];
                        mpReason.reason_order_int = (Int32.TryParse(mpReason.reason_order, out order)) ? order : 99999;
                        mpReason.isAssigned = false;
                        reasonList.Add(mpReason);
                    }
                }
            }
            catch
            {
            }

            //Dictionary<String, MPReason> test = reasonList.ToDictionary(r => r.reason_id);
            //reasonList.Sort((x,y)=> x.reason_std_yn.CompareTo(y.reason_std_yn));   //Sort by the 'std_yn' code
            reasonList.Sort((x, y) => x.reason_order_int.CompareTo(y.reason_order_int));   //Sort by the Numeric Reason Order Value
            return reasonList;
        }

        //=====================================================================================================================
        public List<MPReason> getAssignedMetricPeriodReasons(string mtrc_period_val_id)
        {
            //Define and Initializa Model Object Components
            List<MPReason> reasonList = new List<MPReason>();
            MPReason mpReason;

            string raw_data = String.Empty;
            try
            {
                raw_data = api.getAssignedMetricPeriodReasons(mtrc_period_val_id);
                JObject parsed_result = JObject.Parse(raw_data);
                JArray apiMPReasons = (JArray)parsed_result["assignedreasons"];

                if (apiMPReasons.HasValues)
                {
                    foreach (var reason in apiMPReasons)
                    {
                        mpReason = new MPReason();
                        DateTime assigmentDate;

                        mpReason.val_reason_id = (string)reason["mpvr_id"];            //Primary Table Key Field
                        mpReason.reason_id = (string)reason["mpr_id"];
                        mpReason.metric_period_id = (string)reason["mtrc_period_id"];
                        mpReason.reason_text = (string)reason["mpr_display_text"];
                        mpReason.mpvr_Comment = (string)reason["mpvr_comment"];
                        mpReason.reason_description = (string)reason["mpr_desc"];
                        mpReason.reason_created_by_uid = (string)reason["mpvr_created_by_uid"];
                        try
                        {
                            assigmentDate = Convert.ToDateTime((string)reason["mpvr_created_on_dtm"]);
                            mpReason.reason_created_on_dtm = assigmentDate.ToString("MMM dd, yyyy");
                        }
                        catch
                        {
                            mpReason.reason_created_on_dtm = (string)reason["mpvr_created_on_dtm"];
                        }
                        reasonList.Add(mpReason);
                    }
                }
            }
            catch
            {
            }

            //Dictionary<String, MPReason> test = reasonList.ToDictionary(r => r.reason_id);
            //reasonList.Sort((x,y)=> x.reason_std_yn.CompareTo(y.reason_std_yn));   //Sort by the 'std_yn' code
            reasonList.Sort((x, y) => x.reason_order_int.CompareTo(y.reason_order_int));   //Sort by the Numeric Reason Order Value
            return reasonList;
        }
        public MPReasonViewModel getMetricPeriodValueInfo(string productname, string mtrc_period_val_id)
        {
            //Define and Initializa Model Object Components
            MPReasonViewModel mpReasonViewModel = new MPReasonViewModel();

            string raw_data = String.Empty;
            try
            {
                raw_data = api.getMetricPeriodValueInfo(productname, mtrc_period_val_id);

                JObject parsed_result = JObject.Parse(raw_data);

                mpReasonViewModel.month = (string)parsed_result["month"];
                mpReasonViewModel.monthName = intToMonth((int)parsed_result["month"]);
                mpReasonViewModel.year = (string)parsed_result["year"];
                mpReasonViewModel.bldgName = (string)parsed_result["dsc_mtrc_lc_bldg_name"];
                mpReasonViewModel.bldgId = (string)parsed_result["dsc_mtrc_lc_bldg_id"];
                mpReasonViewModel.mtrc_prod_display_text = (string)parsed_result["mtrc_prod_display_text"];
                mpReasonViewModel.mpId = (string)parsed_result["mtrc_period_id"];
                mpReasonViewModel.mpvVal = (string)parsed_result["mtrc_period_val_value"];
                mpReasonViewModel.mpvId = (string)parsed_result["mtrc_period_val_id"];
                mpReasonViewModel.goalTxt = (string)parsed_result["goal_txt"];
                mpReasonViewModel.data_type_token = (string)parsed_result["data_type_token"];
                mpReasonViewModel.goalMetYN = (string)parsed_result["rz_mpvg_goal_met_yn"];
                mpReasonViewModel.displayClass = getMetricDisplayClass(mpReasonViewModel.mpvVal, mpReasonViewModel.goalMetYN, "Closed");
            }
            catch
            {
            }

            if (String.IsNullOrEmpty(mpReasonViewModel.month)) mpReasonViewModel.month = "0";
            if (String.IsNullOrEmpty(mpReasonViewModel.monthName)) mpReasonViewModel.monthName = "";
            if (String.IsNullOrEmpty(mpReasonViewModel.year)) mpReasonViewModel.year = "0";
            if (String.IsNullOrEmpty(mpReasonViewModel.bldgName)) mpReasonViewModel.bldgName = "";
            if (String.IsNullOrEmpty(mpReasonViewModel.bldgId)) mpReasonViewModel.bldgId = "0";
            if (String.IsNullOrEmpty(mpReasonViewModel.mtrc_prod_display_text)) mpReasonViewModel.mtrc_prod_display_text = "";
            if (String.IsNullOrEmpty(mpReasonViewModel.mpId)) mpReasonViewModel.mpId = "0";
            if (String.IsNullOrEmpty(mpReasonViewModel.mpvVal)) mpReasonViewModel.mpvVal = "0";
            if (String.IsNullOrEmpty(mpReasonViewModel.mpvId)) mpReasonViewModel.mpvId = "0";
            if (String.IsNullOrEmpty(mpReasonViewModel.goalTxt)) mpReasonViewModel.goalTxt = "";
            if (String.IsNullOrEmpty(mpReasonViewModel.goalTxt)) mpReasonViewModel.goalTxt = "";
            if (String.IsNullOrEmpty(mpReasonViewModel.data_type_token)) mpReasonViewModel.data_type_token = "";
            if (String.IsNullOrEmpty(mpReasonViewModel.goalMetYN)) mpReasonViewModel.goalMetYN = "N";

            return mpReasonViewModel;
        }

        ////This method returns the list of all metrics Product Ids that the user is authorized to edit. 
        //public List<int> getUserEditableMetrics(string userName)
        //{
        //    List<int> accessibleMetrics = new List<int>();
        //    JObject parsed_result = JObject.Parse(api.authorizeUser(userName));
        //    if (!(parsed_result["authorizationdetails"] == null)) {
        //        foreach (var res in parsed_result["authorizationdetails"])
        //        {
        //            int mtrc_period_id = (int)res["mtrc_period_id"];
        //            accessibleMetrics.Add(mtrc_period_id);
        //        }            
        //    }
        //    return accessibleMetrics;
        //}

        //This method returns the list of all metrics Ids that user is authorized to edit. 
        public List<int> getUserMetrics(string userName)
        {
            List<int> authMetrics = new List<int>();
            JObject parsed_result = JObject.Parse(api.authorizeUser(userName));
            try
            {
                foreach (var res in parsed_result["authorizationdetails"])
                {
                    int mtrc_id = (int)res["mtrc_id"];
                    authMetrics.Add(mtrc_id);
                }
            }
            catch { }
            return authMetrics;
        }

        //This method returns the count of all types of tasks that the user may have (e.g. data collection or action plan submission/review)
        public RZTaskCounts getUserTasksCount(string app_user_id)
        {
            RZTaskCounts tempTaskCounts = new RZTaskCounts();
            JObject parsed_result = JObject.Parse(api.getUserTasksCount(app_user_id));

            try
            {
                tempTaskCounts.mtrcCount = (int)parsed_result["mtrc_count"];
                tempTaskCounts.actPlanCount = (int)parsed_result["act_plan_count"];
            }
            catch
            {

            }

            return tempTaskCounts;
        }

        //This method returns the count of all Ucompleted Activities for a User's Team (e.g. data collection or action plan submission/review)
        public TeamActivityCount getUserTeamActCount(string app_user_id, string begmonth, string begyear, string endmonth, string endyear)
        {
            TeamActivityCount tempActivityCount = new TeamActivityCount();
            try
            {
                //JObject parsed_result = JObject.Parse(DataRetrieval.executeAPI("endpoint","payload") );
                //string curYear = DateTime.Today.Year.ToString();
                //string curMonth = DateTime.Today.Month.ToString();

                //string jsonPayload = String.Format("{\"productname\":\"Red Zone\",\"begmonth\":\"1\",\"begyear\":\"{0}\",\"endmonth\":\"{1}\",\"endyear\":\"{0}\",\"app_user_id\":\"{2}\"}", curYear, curMonth, app_user_id);
                //string jsonPayload = @"{""productname"":""Red Zone"",""begmonth"":""1"",""begyear"":""" + curYear + @""",""endmonth"":""" + curMonth + @""",""endyear"":""" + curYear+ @""",""app_user_id"":""" + curYear+ @"""}";
                string jsonPayload = "{\"productname\":\"Red Zone\",\"begmonth\":\"" + begmonth + "\",\"begyear\":\"" + begyear + "\",\"endmonth\":\"" + endmonth + "\",\"endyear\":\"" + endyear + "\",\"app_user_id\":\"" + app_user_id + "\"}";

                JObject parsed_result = JObject.Parse(DataRetrieval.executeAPI("getmyteamtaskscount", jsonPayload));

                JArray jActivities = (JArray)parsed_result["tasks"];

                foreach (var res in jActivities)
                {
                    TeamActivityBuildingCount tempActivityBuildingCount = new TeamActivityBuildingCount();

                    tempActivityBuildingCount.bldgId = (string)res["dsc_mtrc_lc_bldg_id"];
                    tempActivityBuildingCount.bldgName = (string)res["dsc_mtrc_lc_bldg_name"];
                    tempActivityBuildingCount.submitCount = (int)res["submit_count"];
                    tempActivityBuildingCount.reviewCount = (int)res["review_count"];

                    tempActivityCount.buildingActivityList.Add(tempActivityBuildingCount);
                }
            }
            catch { }

            return tempActivityCount;
        }

        //This method returns all Ucompleted Activities for a User's Team (e.g. data collection or action plan submission/review)
        public List<TeamActivity> getUserTeamActivities(string app_user_id, string begmonth, string begyear, string endmonth, string endyear)
        {
            List<TeamActivity> teamActivityList = new List<TeamActivity>();

            try
            {
                //JObject parsed_result = JObject.Parse(DataRetrieval.executeAPI("endpoint","payload") );
                //string curYear = DateTime.Today.Year.ToString();
                //string curMonth = DateTime.Today.Month.ToString();

                string jsonPayload = "{\"productname\":\"Red Zone\",\"begmonth\":\"" + begmonth + " \",\"begyear\":\"" + begyear + "\",\"endmonth\":\"" + endmonth + "\",\"endyear\":\"" + endyear + "\",\"app_user_id\":\"" + app_user_id + "\"}";

                JObject parsed_result = JObject.Parse(DataRetrieval.executeAPI("getmyteamactivities", jsonPayload));
                JArray jActionPlans = (JArray)parsed_result["actionplans"];

                foreach (var res in jActionPlans)
                {
                    TeamActivity tempTeamActivity = new TeamActivity();

                    tempTeamActivity.month = (string)res["month"];
                    tempTeamActivity.monthName = intToMonth((int)res["month"]);
                    tempTeamActivity.year = (string)res["year"];
                    tempTeamActivity.periodName = intToMonth((int)res["month"]) + " " + (string)res["year"];
                    tempTeamActivity.bldgName = (string)res["dsc_mtrc_lc_bldg_name"];
                    tempTeamActivity.bldgId = (string)res["dsc_mtrc_lc_bldg_id"];
                    tempTeamActivity.mtrcName = (string)res["mtrc_prod_display_text"];
                    tempTeamActivity.mtrcPeriodId = (string)res["mtrc_period_id"];
                    tempTeamActivity.rzBapmId = (string)res["rz_bapm_id"];
                    tempTeamActivity.rzBapmStatus = (string)res["rz_bapm_status"];
                    tempTeamActivity.rzBapmStartDate = (string)res["rz_apd_ap_created_on_dtm"];
                    tempTeamActivity.rzBapmApprovedDate = (string)res["rz_bapm_approved_on_dtm"];
                    tempTeamActivity.rzBapmUpdateDate = (string)res["lastactivity_on_dtm"];
                    tempTeamActivity.viewStatus = getMyTeam_Status(tempTeamActivity.rzBapmStatus);
                    tempTeamActivity.accountableParty = getMyTeam_AccountableParty(tempTeamActivity.rzBapmStatus);

                    if (String.IsNullOrEmpty(tempTeamActivity.month)) tempTeamActivity.month = "";
                    if (String.IsNullOrEmpty(tempTeamActivity.monthName)) tempTeamActivity.monthName = "";
                    if (String.IsNullOrEmpty(tempTeamActivity.year)) tempTeamActivity.year = "";
                    if (String.IsNullOrEmpty(tempTeamActivity.periodName)) tempTeamActivity.periodName = "";
                    if (String.IsNullOrEmpty(tempTeamActivity.bldgName)) tempTeamActivity.bldgName = "";
                    if (String.IsNullOrEmpty(tempTeamActivity.bldgId)) tempTeamActivity.bldgId = "";
                    if (String.IsNullOrEmpty(tempTeamActivity.mtrcName)) tempTeamActivity.mtrcName = "";
                    if (String.IsNullOrEmpty(tempTeamActivity.mtrcPeriodId)) tempTeamActivity.mtrcPeriodId = "";
                    if (String.IsNullOrEmpty(tempTeamActivity.rzBapmId)) tempTeamActivity.rzBapmId = "";
                    if (String.IsNullOrEmpty(tempTeamActivity.rzBapmStatus)) tempTeamActivity.rzBapmStatus = "";
                    if (String.IsNullOrEmpty(tempTeamActivity.rzBapmStartDate)) tempTeamActivity.rzBapmStartDate = "";
                    if (String.IsNullOrEmpty(tempTeamActivity.rzBapmApprovedDate)) tempTeamActivity.rzBapmApprovedDate = "";
                    if (String.IsNullOrEmpty(tempTeamActivity.rzBapmUpdateDate)) tempTeamActivity.rzBapmUpdateDate = "";

                    //Parses dateTime strings to MM/dd/yyyy format if possible. Else returns "".
                    tempTeamActivity.rzBapmStartDate = parseToDate(tempTeamActivity.rzBapmStartDate);
                    tempTeamActivity.rzBapmApprovedDate = parseToDate(tempTeamActivity.rzBapmApprovedDate);
                    tempTeamActivity.rzBapmUpdateDate = parseToDate(tempTeamActivity.rzBapmUpdateDate);

                    teamActivityList.Add(tempTeamActivity);
                }

            }
            catch { }

            return teamActivityList;
        }

        //This method returns the accountable user for a specified bapm Id
        public AccountableUserViewModel getAccountableUsers(string rz_bapm_id)
        {
            AccountableUserViewModel accountableUserViewModel = new AccountableUserViewModel();
            try
            {
                string jsonPayload = "{\"rz_bapm_id\":\"" + rz_bapm_id + "\"}";

                JObject parsed_result = JObject.Parse(DataRetrieval.executeAPI("getusersforap", jsonPayload));

                if ((string)parsed_result["result"] == "Success")
                {
                    string msg = (string)parsed_result["message"];
                    if (msg == "Action Plan is Approved. No Action Required")
                    {
                        //If action plan approved message is displayed.
                        accountableUserViewModel.message = msg;
                    }
                    else
                    {
                        if ((JArray)parsed_result["users"] == null)
                        {
                            //If some other message is displayed.
                            accountableUserViewModel.message = (String.IsNullOrEmpty(msg)) ? "" : msg;
                        }
                        else
                        {
                            JArray jUsers = (JArray)parsed_result["users"];

                            if (jUsers.Count() > 0)
                            {
                                //If there is a user list.
                                accountableUserViewModel.message = "These are the assigned users.";
                                foreach (var user in jUsers)
                                {
                                    AccountableUser tempUser = new AccountableUser();
                                    tempUser.username = (string)user["username"];
                                    tempUser.app_user_id = (string)user["app_user_id"];
                                    tempUser.email = (string)user["email"];

                                    if (String.IsNullOrEmpty(tempUser.username)) tempUser.username = "";
                                    if (String.IsNullOrEmpty(tempUser.app_user_id)) tempUser.app_user_id = "";
                                    if (String.IsNullOrEmpty(tempUser.email)) tempUser.email = "";

                                    accountableUserViewModel.users.Add(tempUser);
                                }
                            }
                            else
                            {
                                //If the user list is empty.
                                accountableUserViewModel.message = "There are no assigned users.";
                            }
                        }
                    }

                }
            }
            catch { }

            return accountableUserViewModel;
        }

        //This method returns the count of all types of tasks that the user may have (e.g. data collection or action plan submission/review)
        public RZTaskDetailList getUserTaskDetails(string app_user_id)
        {
            RZTaskDetailList tasksViewModel = new RZTaskDetailList();
            JObject parsed_result = JObject.Parse(api.getUserTaskDetails(app_user_id));

            try
            {
                JArray jSubmitterTasks = (JArray)parsed_result["submittertasks"];

                foreach (var res in jSubmitterTasks)
                {
                    RZActionPlanTask tempAPTask = new RZActionPlanTask();

                    tempAPTask.month = (string)res["month"];
                    tempAPTask.year = (string)res["year"];
                    tempAPTask.period_name = intToMonth((int)res["month"]) + " " + (string)res["year"];
                    tempAPTask.mtrc_period_name = (string)res["period"];
                    tempAPTask.mtrc_prod_display_text = (string)res["mtrc_prod_display_text"];
                    tempAPTask.mtrc_period_id = (int)res["mtrc_period_id"];
                    tempAPTask.bldg_name = (string)res["building"];
                    tempAPTask.bldg_id = (int)res["dsc_mtrc_lc_bldg_id"];
                    tempAPTask.status = (string)res["status"];
                    tempAPTask.rz_bapm_id = (string)res["rz_bapm_id"];
                    tempAPTask.mtrc_period_val_id = (string)res["mtrc_period_val_id"];
                    if (String.IsNullOrEmpty(tempAPTask.month)) tempAPTask.month = "";
                    if (String.IsNullOrEmpty(tempAPTask.year)) tempAPTask.year = "";
                    if (String.IsNullOrEmpty(tempAPTask.period_name)) tempAPTask.period_name = "";
                    if (String.IsNullOrEmpty(tempAPTask.mtrc_period_name)) tempAPTask.mtrc_period_name = "";
                    if (String.IsNullOrEmpty(tempAPTask.mtrc_prod_display_text)) tempAPTask.mtrc_prod_display_text = "";
                    if (String.IsNullOrEmpty(tempAPTask.bldg_name)) tempAPTask.bldg_name = "";
                    if (String.IsNullOrEmpty(tempAPTask.status)) tempAPTask.status = "";
                    if (String.IsNullOrEmpty(tempAPTask.rz_bapm_id)) tempAPTask.rz_bapm_id = "";
                    if (String.IsNullOrEmpty(tempAPTask.mtrc_period_val_id)) tempAPTask.mtrc_period_val_id = "0";

                    tasksViewModel.apSubmitTaskList.Add(tempAPTask);
                }

                JArray jReviewerTasks = (JArray)parsed_result["reviewertasks"];
                foreach (var res in jReviewerTasks)
                {
                    RZActionPlanTask tempAPTask = new RZActionPlanTask();

                    tempAPTask.month = (string)res["month"];
                    tempAPTask.year = (string)res["year"];
                    tempAPTask.period_name = intToMonth((int)res["month"]) + " " + (string)res["year"];
                    tempAPTask.mtrc_period_name = (string)res["period"];
                    tempAPTask.mtrc_prod_display_text = (string)res["mtrc_prod_display_text"];
                    tempAPTask.mtrc_period_id = (int)res["mtrc_period_id"];
                    tempAPTask.bldg_name = (string)res["building"];
                    tempAPTask.bldg_id = (int)res["dsc_mtrc_lc_bldg_id"];
                    tempAPTask.status = (string)res["status"];
                    tempAPTask.rz_bapm_id = (string)res["rz_bapm_id"];
                    if (String.IsNullOrEmpty(tempAPTask.month)) tempAPTask.month = "";
                    if (String.IsNullOrEmpty(tempAPTask.year)) tempAPTask.year = "";
                    if (String.IsNullOrEmpty(tempAPTask.period_name)) tempAPTask.period_name = "";
                    if (String.IsNullOrEmpty(tempAPTask.mtrc_period_name)) tempAPTask.mtrc_period_name = "";
                    if (String.IsNullOrEmpty(tempAPTask.mtrc_prod_display_text)) tempAPTask.mtrc_prod_display_text = "";
                    if (String.IsNullOrEmpty(tempAPTask.bldg_name)) tempAPTask.bldg_name = "";
                    if (String.IsNullOrEmpty(tempAPTask.status)) tempAPTask.status = "";
                    if (String.IsNullOrEmpty(tempAPTask.rz_bapm_id)) tempAPTask.rz_bapm_id = "";

                    tasksViewModel.apReviewTaskList.Add(tempAPTask);
                }

                JArray jDataCollectorTasks = (JArray)parsed_result["datacollectortasks"];
                foreach (var res in jDataCollectorTasks)
                {
                    RZMetricTask tempMetricTask = new RZMetricTask();

                    tempMetricTask.month = (string)res["month"];
                    tempMetricTask.year = (string)res["year"];
                    tempMetricTask.month_name = intToMonth((int)res["month"]);
                    tempMetricTask.period_name = intToMonth((int)res["month"]) + " " + (string)res["year"];
                    tempMetricTask.mtrc_period_name = (string)res["period"];
                    tempMetricTask.mtrc_prod_display_text = (string)res["mtrc_prod_display_text"];
                    tempMetricTask.mtrc_id = (int)res["mtrc_id"];
                    tempMetricTask.status = (string)res["status"];
                    if (String.IsNullOrEmpty(tempMetricTask.month)) tempMetricTask.month = "";
                    if (String.IsNullOrEmpty(tempMetricTask.year)) tempMetricTask.year = "";
                    if (String.IsNullOrEmpty(tempMetricTask.month_name)) tempMetricTask.month_name = "";
                    if (String.IsNullOrEmpty(tempMetricTask.period_name)) tempMetricTask.period_name = "";
                    if (String.IsNullOrEmpty(tempMetricTask.mtrc_period_name)) tempMetricTask.mtrc_period_name = "";
                    if (String.IsNullOrEmpty(tempMetricTask.mtrc_prod_display_text)) tempMetricTask.mtrc_prod_display_text = "";
                    if (String.IsNullOrEmpty(tempMetricTask.status)) tempMetricTask.status = "";

                    tasksViewModel.mtrcTaskList.Add(tempMetricTask);
                }

            }
            catch
            {

            }

            return tasksViewModel;
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
        public string[] getPrevNextBuildingUrl(string year, string buildingID, List<string> usrBldgList)
        {
            string[] prevNext = new string[] { "disabled", "disabled" };
            //If usrBldgList is empty, then no filters will need to be applied
            try
            {
                if (usrBldgList.Count > 0)
                {   //Apply Building filters
                    if (usrBldgList.Count == 1) { return prevNext; }  //No previous nor next building exist
                    else
                    {  //There are at least Two building associated to the Current User
                        //Search for the current Building Id in the usrBldList and set the previous and next Building URLs
                        int curIndex = usrBldgList.FindIndex(x => x == buildingID);  //get the Index in the list of the current Building 

                        if (curIndex == (usrBldgList.Count - 1))
                        {   //This is the last element
                            prevNext[0] = String.Format("/Home/BuildingSummary/?year={0}&buildingID={1}", year, usrBldgList[curIndex - 1]);
                            prevNext[1] = String.Format("/Home/BuildingSummary/?year={0}&buildingID={1}", year, usrBldgList[0]);
                            return prevNext;
                        }

                        switch (curIndex)
                        {
                            case -1:    //Element not found
                                prevNext[0] = String.Format("/Home/BuildingSummary/?year={0}&buildingID={1}", year, usrBldgList[0]);
                                prevNext[1] = String.Format("/Home/BuildingSummary/?year={0}&buildingID={1}", year, usrBldgList[0]);
                                break;
                            case 0:   //This is the first element of the list
                                prevNext[0] = String.Format("/Home/BuildingSummary/?year={0}&buildingID={1}", year, usrBldgList[usrBldgList.Count - 1]);
                                prevNext[1] = String.Format("/Home/BuildingSummary/?year={0}&buildingID={1}", year, usrBldgList[curIndex + 1]);
                                break;
                            default:    //Not first nor last element
                                prevNext[0] = String.Format("/Home/BuildingSummary/?year={0}&buildingID={1}", year, usrBldgList[curIndex - 1]);
                                prevNext[1] = String.Format("/Home/BuildingSummary/?year={0}&buildingID={1}", year, usrBldgList[curIndex + 1]);
                                break;
                        }
                        return prevNext;
                    }
                }
            }
            catch { }


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
            catch { }

            return prevNext;
        }

        //Reasons
        //
        public List<MPReason> getMPReasonList(string metricPeriodId)
        {
            List<MPReason> MPReasonList = new List<MPReason>();
            MPReason tempMPReason;

            string raw_data = api.getMetricPeriodReasons(metricPeriodId);

            try
            {
                JObject parsed_result = JObject.Parse(raw_data);

                JArray jReasonList = (JArray)parsed_result["reasons"];
                foreach (var res in jReasonList)
                {
                    tempMPReason = new Models.MPReason();
                    tempMPReason.reason_id = (string)res["mpr_id"];
                    tempMPReason.metric_period_id = (string)res["mtrc_period_id"];
                    tempMPReason.reason_text = (string)res["mpr_display_text"];
                    tempMPReason.reason_order = (string)res["mpr_display_order"];
                    tempMPReason.reason_description = (string)res["mpr_desc"];
                    tempMPReason.reason_std_yn = (string)res["mpr_std_yn"];
                    tempMPReason.reason_created_on_dtm = (string)res["mpr_added_on_dtm"];
                    tempMPReason.reason_created_by_uid = (string)res["mpr_added_by_uid"];
                    tempMPReason.reason_stdized_on_dtm = (string)res["mpr_stdized_on_dtm"];
                    tempMPReason.reason_stdized_by_uid = (string)res["mpr_stdized_by_uid"];
                    tempMPReason.times_used = (string)res["usedby"];

                    MPReasonList.Add(tempMPReason);
                }

            }
            catch
            {

            }
            return MPReasonList;
        }
        public List<MetricPeriod> getMetricPeriodList()
        {
            List<MetricPeriod> MetricPeriodList = new List<MetricPeriod>();
            MetricPeriod tempMetricPeriod;

            string raw_data = api.getMetricPeriods();

            try
            {
                JObject parsed_result = JObject.Parse(raw_data);

                JArray jReasonList = (JArray)parsed_result["mmperiod"];
                foreach (var res in jReasonList)
                {
                    tempMetricPeriod = new Models.MetricPeriod();
                    tempMetricPeriod.mtrc_period_id = (string)res["mtrc_period_id"];
                    tempMetricPeriod.mtrc_period_name = (string)res["mtrc_period_name"];
                    tempMetricPeriod.mtrc_period_token = (string)res["mtrc_period_token"];
                    tempMetricPeriod.mtrc_prod_display_text = (string)res["mtrc_prod_display_text"];
                    tempMetricPeriod.prod_name = (string)res["prod_name"];

                    MetricPeriodList.Add(tempMetricPeriod);
                }

            }
            catch
            {

            }
            return MetricPeriodList;
        }

        //ACTION PLANS
        //
        public ActionPlanViewModel getActionPlanList(string productname, string rz_bapm_id)     //DEPRECATED
        {
            //This method returns a list of versioned action plans corresponding to a RZ_BAPM_ID (Red Zone Building Action Plan Metric Id).
            ActionPlanViewModel apViewModel = new ActionPlanViewModel();
            List<ActionPlan> actionPlanList = new List<ActionPlan>();
            ActionPlan tempActionPlan = new ActionPlan();

            string raw_data = api.getActionPlans(productname, rz_bapm_id);

            try
            {
                JObject parsed_result = JObject.Parse(raw_data);

                apViewModel.bapm_id = (string)parsed_result["rz_bapm_id"];
                apViewModel.bapmStatus = (string)parsed_result["rz_bapm_status"];
                apViewModel.mpv_id = (string)parsed_result["mtrc_period_val_id"];
                apViewModel.bldgName = (string)parsed_result["dsc_mtrc_lc_bldg_name"];
                apViewModel.mtrcDisplayText = (string)parsed_result["mtrc_prod_display_text"];
                apViewModel.apMonth = intToMonth((int)parsed_result["month"]);
                apViewModel.apYear = (string)parsed_result["year"];
                if (String.IsNullOrEmpty(apViewModel.bapm_id)) apViewModel.bapm_id = "0";
                if (String.IsNullOrEmpty(apViewModel.bapmStatus)) apViewModel.bapmStatus = "";
                if (String.IsNullOrEmpty(apViewModel.mpv_id)) apViewModel.mpv_id = "0";
                if (String.IsNullOrEmpty(apViewModel.bldgName)) apViewModel.bldgName = "";
                if (String.IsNullOrEmpty(apViewModel.mtrcDisplayText)) apViewModel.mtrcDisplayText = "";
                if (String.IsNullOrEmpty(apViewModel.apMonth)) apViewModel.apMonth = "";
                if (String.IsNullOrEmpty(apViewModel.apYear)) apViewModel.apYear = "";

                JArray jReasonList = (JArray)parsed_result["details"];
                foreach (var res in jReasonList)
                {
                    tempActionPlan = new Models.ActionPlan();

                    tempActionPlan.apd_id = (string)res["rz_apd_id"];
                    tempActionPlan.apVersion = (string)res["rz_apd_ap_ver"];
                    tempActionPlan.apStatus = (string)res["rz_apd_ap_status"];
                    tempActionPlan.actionPlanAction = (string)res["rz_apd_ap_text"];
                    tempActionPlan.reviewerComments = (string)res["rz_apd_ap_review_text"];
                    tempActionPlan.submittedBy = (string)res["submittedby"];
                    tempActionPlan.reviewedBy = (string)res["reviewedby"];

                    if (String.IsNullOrEmpty(tempActionPlan.reviewerComments)) tempActionPlan.reviewerComments = "";
                    if (String.IsNullOrEmpty(tempActionPlan.submittedBy)) tempActionPlan.submittedBy = "";
                    if (String.IsNullOrEmpty(tempActionPlan.reviewedBy)) tempActionPlan.reviewedBy = "";

                    actionPlanList.Add(tempActionPlan);
                }

                actionPlanList = actionPlanList.OrderByDescending(x => Int32.Parse(x.apVersion)).ToList();

                apViewModel.actionPlanList = actionPlanList;
            }
            catch
            {

            }
            return apViewModel;
        }
        public List<PriorActionPlan> getPriorActionPlanList(string productname, string mtrc_period_id, string dsc_mtrc_lc_bldg_id, string begmonth, string begyear, string endmonth, string endyear)
        {
            //This method returns a list of prior month action plans based on product, metric_period_id, dsc_mtrc_lc_bldg_id, begmonth, begyear, endmonth, and endyear.
            List<PriorActionPlan> priorActionPlanList = new List<PriorActionPlan>();
            PriorActionPlan tempPriorActionPlan = new PriorActionPlan();
            List<MPReason> reasonList = new List<MPReason>();
            MPReason tempMPReason = new MPReason();

            string raw_data = api.getPriorActionPlans(productname, mtrc_period_id, dsc_mtrc_lc_bldg_id, begmonth, begyear, endmonth, endyear);

            try
            {
                JObject parsed_result = JObject.Parse(raw_data);

                JArray jPriorActionPlanList = (JArray)parsed_result["actionplans"];
                foreach (var res in jPriorActionPlanList)
                {
                    tempPriorActionPlan = new Models.PriorActionPlan();
                    reasonList = new List<MPReason>();

                    tempPriorActionPlan.apd_id = (string)res["rz_apd_id"];
                    tempPriorActionPlan.bapm_id = (string)res["rz_bapm_id"];
                    tempPriorActionPlan.mtrc_period_val_id = (string)res["mtrc_period_val_id"];
                    tempPriorActionPlan.mtrc_period_id = (string)res["mtrc_period_id"];
                    tempPriorActionPlan.dsc_mtrc_lc_bldg_id = (string)res["dsc_mtrc_lc_bldg_id"];
                    tempPriorActionPlan.priorAPMonth = intToMonth((int)res["month"]);
                    tempPriorActionPlan.priorAPYear = (string)res["year"];
                    //tempPriorActionPlan.priorAPMetricGoalText = (string)res["goal_txt"];
                    tempPriorActionPlan.priorAPMetricGoalText = ((string)res["goal_txt"]).Replace("<=", "&le;").Replace(">=", "&ge;");
                    tempPriorActionPlan.priorAPMetricValue = (string)res["mtrc_period_val_value"];
                    tempPriorActionPlan.priorAPStatus = (string)res["rz_apd_ap_status"];
                    tempPriorActionPlan.priorAPText = (string)res["rz_apd_ap_text"];
                    tempPriorActionPlan.priorAPReviewText = (string)res["rz_apd_ap_review_text"];
                    tempPriorActionPlan.submittedBy = (string)res["submitted_by"];
                    tempPriorActionPlan.approvedBy = (string)res["approved_by"];

                    JArray jPriorReasonList = (JArray)res["assignedreasons"];

                    foreach (var rawReason in jPriorReasonList)
                    {
                        tempMPReason = new Models.MPReason();

                        tempMPReason.val_reason_id = (string)rawReason["mpvr_id"];
                        tempMPReason.reason_id = (string)rawReason["mpr_id"];
                        tempMPReason.reason_text = (string)rawReason["mpr_display_text"];
                        tempMPReason.mpvr_Comment = (string)rawReason["mpvr_comment"];

                        if (String.IsNullOrEmpty(tempMPReason.reason_text)) tempMPReason.reason_text = "";
                        if (String.IsNullOrEmpty(tempMPReason.mpvr_Comment)) tempMPReason.mpvr_Comment = "";

                        reasonList.Add(tempMPReason);
                    }

                    tempPriorActionPlan.priorAPReasonList = reasonList;
                    tempPriorActionPlan.priorAPStatusColor = "green";

                    if (String.IsNullOrEmpty(tempPriorActionPlan.priorAPMonth)) tempPriorActionPlan.priorAPMonth = "";
                    if (String.IsNullOrEmpty(tempPriorActionPlan.priorAPYear)) tempPriorActionPlan.priorAPYear = "";
                    if (String.IsNullOrEmpty(tempPriorActionPlan.priorAPMetricGoalText)) tempPriorActionPlan.priorAPMetricGoalText = "";
                    if (String.IsNullOrEmpty(tempPriorActionPlan.priorAPMetricValue)) tempPriorActionPlan.priorAPMetricValue = "";
                    if (String.IsNullOrEmpty(tempPriorActionPlan.priorAPStatus)) tempPriorActionPlan.priorAPStatus = "";
                    if (String.IsNullOrEmpty(tempPriorActionPlan.priorAPText)) tempPriorActionPlan.priorAPText = "";
                    if (String.IsNullOrEmpty(tempPriorActionPlan.priorAPReviewText)) tempPriorActionPlan.priorAPReviewText = "";
                    if (String.IsNullOrEmpty(tempPriorActionPlan.submittedBy)) tempPriorActionPlan.submittedBy = "";
                    if (String.IsNullOrEmpty(tempPriorActionPlan.approvedBy)) tempPriorActionPlan.approvedBy = "";

                    priorActionPlanList.Add(tempPriorActionPlan);
                }
            }
            catch
            {

            }
            return priorActionPlanList;
        }       //DEPRECATED
        public List<PriorActionPlan> getRecentActionPlanList(string productname, string mtrc_period_id, string dsc_mtrc_lc_bldg_id, string begmonth, string begyear, string endmonth, string endyear)
        {
            //This method returns a list of prior month action plans based on product, metric_period_id, dsc_mtrc_lc_bldg_id, begmonth, begyear, endmonth, and endyear.
            List<PriorActionPlan> priorActionPlanList = new List<PriorActionPlan>();
            PriorActionPlan tempPriorActionPlan = new PriorActionPlan();
            List<MPReason> reasonList = new List<MPReason>();
            MPReason tempMPReason = new MPReason();

            //string raw_data = api.getPriorActionPlans(productname, mtrc_period_id, dsc_mtrc_lc_bldg_id, begmonth, begyear, endmonth, endyear);
            string raw_data = api.lookUpActionPlans(productname, mtrc_period_id, dsc_mtrc_lc_bldg_id, begmonth, begyear, endmonth, endyear, "Approved");

            try
            {
                JObject parsed_result = JObject.Parse(raw_data);

                JArray jPriorActionPlanList = (JArray)parsed_result["actionplans"];
                foreach (var res in jPriorActionPlanList)
                {
                    tempPriorActionPlan = new Models.PriorActionPlan();
                    reasonList = new List<MPReason>();

                    JArray jPriorAPDetails = (JArray)res["details"];
                    var mostRecentAPDetail = jPriorAPDetails.OrderByDescending(x => (int)x["rz_apd_ap_ver"]).ToArray()[0];

                    tempPriorActionPlan.apd_id = (string)mostRecentAPDetail["rz_apd_id"];
                    tempPriorActionPlan.bapm_id = (string)res["rz_bapm_id"];
                    tempPriorActionPlan.mtrc_period_val_id = (string)res["mtrc_period_val_id"];
                    tempPriorActionPlan.mtrc_period_id = (string)res["mtrc_period_id"];
                    tempPriorActionPlan.dsc_mtrc_lc_bldg_id = (string)res["dsc_mtrc_lc_bldg_id"];
                    tempPriorActionPlan.priorAPMonth = intToMonth((int)res["month"]);
                    tempPriorActionPlan.priorAPYear = (string)res["year"];
                    //tempPriorActionPlan.priorAPMetricGoalText = (string)res["goal_txt"];
                    tempPriorActionPlan.priorAPMetricGoalText = ((string)res["goal_txt"]).Replace("<=", "&le;").Replace(">=", "&ge;");
                    tempPriorActionPlan.priorAPMetricValue = (string)res["mtrc_period_val_value"];
                    tempPriorActionPlan.priorAPStatus = (string)mostRecentAPDetail["rz_apd_ap_status"];
                    tempPriorActionPlan.priorAPText = (string)mostRecentAPDetail["rz_apd_ap_text"];
                    tempPriorActionPlan.priorAPReviewText = (string)mostRecentAPDetail["rz_apd_ap_review_text"];
                    tempPriorActionPlan.submittedBy = (string)mostRecentAPDetail["submitted_by"];
                    tempPriorActionPlan.approvedBy = (string)mostRecentAPDetail["approved_by"];

                    JArray jPriorReasonList = (JArray)res["assignedreasons"];

                    foreach (var rawReason in jPriorReasonList)
                    {
                        tempMPReason = new Models.MPReason();

                        tempMPReason.val_reason_id = (string)rawReason["mpvr_id"];
                        tempMPReason.reason_id = (string)rawReason["mpr_id"];
                        tempMPReason.reason_text = (string)rawReason["mpr_display_text"];
                        tempMPReason.mpvr_Comment = (string)rawReason["mpvr_comment"];

                        if (String.IsNullOrEmpty(tempMPReason.reason_text)) tempMPReason.reason_text = "";
                        if (String.IsNullOrEmpty(tempMPReason.mpvr_Comment)) tempMPReason.mpvr_Comment = "";

                        reasonList.Add(tempMPReason);
                    }

                    tempPriorActionPlan.priorAPReasonList = reasonList;
                    tempPriorActionPlan.priorAPStatusColor = "green";

                    if (String.IsNullOrEmpty(tempPriorActionPlan.priorAPMonth)) tempPriorActionPlan.priorAPMonth = "";
                    if (String.IsNullOrEmpty(tempPriorActionPlan.priorAPYear)) tempPriorActionPlan.priorAPYear = "";
                    if (String.IsNullOrEmpty(tempPriorActionPlan.priorAPMetricGoalText)) tempPriorActionPlan.priorAPMetricGoalText = "";
                    if (String.IsNullOrEmpty(tempPriorActionPlan.priorAPMetricValue)) tempPriorActionPlan.priorAPMetricValue = "";
                    if (String.IsNullOrEmpty(tempPriorActionPlan.priorAPStatus)) tempPriorActionPlan.priorAPStatus = "";
                    if (String.IsNullOrEmpty(tempPriorActionPlan.priorAPText)) tempPriorActionPlan.priorAPText = "";
                    if (String.IsNullOrEmpty(tempPriorActionPlan.priorAPReviewText)) tempPriorActionPlan.priorAPReviewText = "";
                    if (String.IsNullOrEmpty(tempPriorActionPlan.submittedBy)) tempPriorActionPlan.submittedBy = "";
                    if (String.IsNullOrEmpty(tempPriorActionPlan.approvedBy)) tempPriorActionPlan.approvedBy = "";

                    priorActionPlanList.Add(tempPriorActionPlan);
                }
            }
            catch
            {

            }
            return priorActionPlanList;
        }
        public PriorActionPlan getMostRecentAP(string productname, string mtrc_period_id, string dsc_mtrc_lc_bldg_id, string begmonth, string begyear, string endmonth, string endyear)
        {
            //This method returns a list of prior month action plans based on product, metric_period_id, dsc_mtrc_lc_bldg_id, begmonth, begyear, endmonth, endyear, and status.
            PriorActionPlan tempPriorActionPlan = new PriorActionPlan();
            List<MPReason> reasonList = new List<MPReason>();
            MPReason tempMPReason = new MPReason();

            string raw_data = api.lookUpActionPlans(productname, mtrc_period_id, dsc_mtrc_lc_bldg_id, begmonth, begyear, endmonth, endyear);

            try
            {
                JObject parsed_result = JObject.Parse(raw_data);

                JArray jPriorActionPlanList = (JArray)parsed_result["actionplans"];

                //var temp = jPriorActionPlanList.OrderByDescending(x => ((int)x["month"]).ToString("00") + (string)x["year"]).ToArray();

                var res = jPriorActionPlanList.OrderByDescending(x => ((int)x["month"]).ToString("00") + (string)x["year"]).ToArray()[0];
                //string temp2 = ((int)res["month"]).ToString("00") + (string)res["year"];

                tempPriorActionPlan = new Models.PriorActionPlan();
                reasonList = new List<MPReason>();
                tempPriorActionPlan.apd_id = "";
                tempPriorActionPlan.bapm_id = (string)res["rz_bapm_id"];
                tempPriorActionPlan.mtrc_period_val_id = (string)res["mtrc_period_val_id"];
                tempPriorActionPlan.mtrc_period_id = (string)res["mtrc_period_id"];
                tempPriorActionPlan.dsc_mtrc_lc_bldg_id = (string)res["dsc_mtrc_lc_bldg_id"];
                tempPriorActionPlan.priorAPMonth = intToMonth((int)res["month"]);
                tempPriorActionPlan.priorAPYear = (string)res["year"];
                tempPriorActionPlan.priorAPMetricGoalText = (string)res["goal_txt"];
                tempPriorActionPlan.priorAPMetricValue = (string)res["mtrc_period_val_value"];
                tempPriorActionPlan.priorAPStatus = (string)res["rz_bapm_status"];
                tempPriorActionPlan.priorAPText = "";
                tempPriorActionPlan.priorAPReviewText = "";
                tempPriorActionPlan.submittedBy = "";
                tempPriorActionPlan.approvedBy = "";

                JArray jPriorAPDetails = (JArray)res["details"];

                var mostRecentAPDetail = jPriorAPDetails.OrderByDescending(x => (int)x["rz_apd_ap_ver"]).ToArray()[0];

                tempPriorActionPlan.apd_id = (string)mostRecentAPDetail["rz_apd_id"];
                tempPriorActionPlan.submittedBy = (string)mostRecentAPDetail["submittedby"];
                tempPriorActionPlan.approvedBy = (string)mostRecentAPDetail["reviewedby"];

                JArray jPriorReasonList = (JArray)res["assignedreasons"];

                foreach (var rawReason in jPriorReasonList)
                {
                    tempMPReason = new Models.MPReason();

                    tempMPReason.val_reason_id = (string)rawReason["mpvr_id"];
                    tempMPReason.reason_id = (string)rawReason["mpr_id"];
                    tempMPReason.reason_text = (string)rawReason["mpr_display_text"];
                    tempMPReason.mpvr_Comment = (string)rawReason["mpvr_comment"];

                    if (String.IsNullOrEmpty(tempMPReason.reason_text)) tempMPReason.reason_text = "";
                    if (String.IsNullOrEmpty(tempMPReason.mpvr_Comment)) tempMPReason.mpvr_Comment = "";

                    reasonList.Add(tempMPReason);
                }

                tempPriorActionPlan.priorAPReasonList = reasonList;

                if (String.IsNullOrEmpty(tempPriorActionPlan.priorAPMonth)) tempPriorActionPlan.priorAPMonth = "";
                if (String.IsNullOrEmpty(tempPriorActionPlan.priorAPYear)) tempPriorActionPlan.priorAPYear = "";
                if (String.IsNullOrEmpty(tempPriorActionPlan.priorAPMetricGoalText)) tempPriorActionPlan.priorAPMetricGoalText = "";
                if (String.IsNullOrEmpty(tempPriorActionPlan.priorAPMetricValue)) tempPriorActionPlan.priorAPMetricValue = "";
                if (String.IsNullOrEmpty(tempPriorActionPlan.priorAPStatus)) tempPriorActionPlan.priorAPStatus = "";
                if (String.IsNullOrEmpty(tempPriorActionPlan.priorAPText)) tempPriorActionPlan.priorAPText = "";
                if (String.IsNullOrEmpty(tempPriorActionPlan.priorAPReviewText)) tempPriorActionPlan.priorAPReviewText = "";
                if (String.IsNullOrEmpty(tempPriorActionPlan.submittedBy)) tempPriorActionPlan.submittedBy = "";
                if (String.IsNullOrEmpty(tempPriorActionPlan.approvedBy)) tempPriorActionPlan.approvedBy = "";

                //Assign status color
                switch (tempPriorActionPlan.priorAPStatus)
                {
                    case "Not Started":
                        tempPriorActionPlan.priorAPStatusColor = "orange";
                        break;
                    case "WIP":
                        tempPriorActionPlan.priorAPStatusColor = "orange";
                        break;
                    case "Ready For Review":
                        tempPriorActionPlan.priorAPStatusColor = "orange";
                        tempPriorActionPlan.priorAPText = (string)mostRecentAPDetail["rz_apd_ap_text"];
                        break;
                    case "Rejected":
                        tempPriorActionPlan.priorAPStatusColor = "red";
                        tempPriorActionPlan.priorAPText = (string)mostRecentAPDetail["rz_apd_ap_text"];
                        tempPriorActionPlan.priorAPReviewText = (string)mostRecentAPDetail["rz_apd_ap_review_text"];
                        break;
                    case "Rejected New":
                        tempPriorActionPlan.priorAPStatusColor = "red";
                        break;
                    case "Approved":
                        tempPriorActionPlan.priorAPStatusColor = "green";
                        tempPriorActionPlan.priorAPText = (string)mostRecentAPDetail["rz_apd_ap_text"];
                        tempPriorActionPlan.priorAPReviewText = (string)mostRecentAPDetail["rz_apd_ap_review_text"];
                        break;
                    default:
                        tempPriorActionPlan.priorAPStatusColor = "green";
                        break;
                }

            }
            catch
            {

            }
            return tempPriorActionPlan;
        }
        public QuickActionPlan getActionPlanById(string productname, string rz_bapm_id)
        {
            //This method returns a list of prior month action plans based on product, metric_period_id, dsc_mtrc_lc_bldg_id, begmonth, begyear, endmonth, endyear, and status.
            QuickActionPlan tempActionPlan = new QuickActionPlan();
            List<MPReason> reasonList = new List<MPReason>();
            MPReason tempMPReason = new MPReason();

            string raw_data = api.lookUpActionPlans(productname, rz_bapm_id);

            try
            {
                JObject parsed_result = JObject.Parse(raw_data);

                JArray jActionPlanList = (JArray)parsed_result["actionplans"];

                var res = jActionPlanList.First();

                tempActionPlan.bapm_id = (string)res["rz_bapm_id"];
                tempActionPlan.mtrc_period_val_id = (string)res["mtrc_period_val_id"];
                tempActionPlan.mtrc_period_id = (string)res["mtrc_period_id"];
                tempActionPlan.mtrcDisplayText = (string)res["mtrc_prod_display_text"];
                tempActionPlan.dsc_mtrc_lc_bldg_id = (string)res["dsc_mtrc_lc_bldg_id"];
                tempActionPlan.bldgName = (string)res["dsc_mtrc_lc_bldg_name"];
                tempActionPlan.month = (string)res["month"];
                tempActionPlan.monthName = intToMonth((int)res["month"]);
                tempActionPlan.year = (string)res["year"];
                //tempActionPlan.mtrcGoalText = (string)res["goal_txt"];
                tempActionPlan.mtrcGoalText = ((string)res["goal_txt"]).Replace("<=", "&le;").Replace(">=", "&ge;");
                tempActionPlan.mtrcValue = (string)res["mtrc_period_val_value"];
                tempActionPlan.status = (string)res["rz_bapm_status"];

                JArray jActionPlanDetails = (JArray)res["details"];

                var apDetail = jActionPlanDetails.OrderByDescending(x => (int)x["rz_apd_ap_ver"]).ToArray()[0];

                tempActionPlan.apd_id = (string)apDetail["rz_apd_id"];
                tempActionPlan.apText = (string)apDetail["rz_apd_ap_text"];
                tempActionPlan.apReviewText = (string)apDetail["rz_apd_ap_review_text"];
                tempActionPlan.submittedBy = (string)apDetail["submittedby"];
                tempActionPlan.approvedBy = (string)apDetail["reviewedby"];

                JArray jPriorReasonList = (JArray)res["assignedreasons"];

                foreach (var rawReason in jPriorReasonList)
                {
                    tempMPReason = new Models.MPReason();

                    tempMPReason.val_reason_id = (string)rawReason["mpvr_id"];
                    tempMPReason.reason_id = (string)rawReason["mpr_id"];
                    tempMPReason.reason_text = (string)rawReason["mpr_display_text"];
                    tempMPReason.mpvr_Comment = (string)rawReason["mpvr_comment"];

                    if (String.IsNullOrEmpty(tempMPReason.reason_text)) tempMPReason.reason_text = "";
                    if (String.IsNullOrEmpty(tempMPReason.mpvr_Comment)) tempMPReason.mpvr_Comment = "";

                    reasonList.Add(tempMPReason);
                }

                tempActionPlan.reasonList = reasonList;

                if (String.IsNullOrEmpty(tempActionPlan.mtrcDisplayText)) tempActionPlan.mtrcDisplayText = "";
                if (String.IsNullOrEmpty(tempActionPlan.bldgName)) tempActionPlan.bldgName = "";
                if (String.IsNullOrEmpty(tempActionPlan.month)) tempActionPlan.month = "";
                if (String.IsNullOrEmpty(tempActionPlan.monthName)) tempActionPlan.monthName = "";
                if (String.IsNullOrEmpty(tempActionPlan.year)) tempActionPlan.year = "";
                if (String.IsNullOrEmpty(tempActionPlan.mtrcGoalText)) tempActionPlan.mtrcGoalText = "";
                if (String.IsNullOrEmpty(tempActionPlan.mtrcValue)) tempActionPlan.mtrcValue = "";
                if (String.IsNullOrEmpty(tempActionPlan.status)) tempActionPlan.status = "";
                if (String.IsNullOrEmpty(tempActionPlan.apText)) tempActionPlan.apText = "";
                if (String.IsNullOrEmpty(tempActionPlan.apReviewText)) tempActionPlan.apReviewText = "";
                if (String.IsNullOrEmpty(tempActionPlan.submittedBy)) tempActionPlan.submittedBy = "";
                if (String.IsNullOrEmpty(tempActionPlan.approvedBy)) tempActionPlan.approvedBy = "";

            }
            catch
            {

            }
            return tempActionPlan;
        }
        public ActionPlanViewModel getActionPlansById(string productname, string rz_bapm_id)
        {
            //This method returns a list of versioned action plans corresponding to a RZ_BAPM_ID (Red Zone Building Action Plan Metric Id).
            ActionPlanViewModel apViewModel = new ActionPlanViewModel();
            List<ActionPlan> actionPlanList = new List<ActionPlan>();
            ActionPlan tempActionPlan = new ActionPlan();

            string raw_data = api.lookUpActionPlans(productname, rz_bapm_id);

            try
            {
                JObject parsed_result = JObject.Parse(raw_data);

                JArray jActionPlanList = (JArray)parsed_result["actionplans"];

                var res = jActionPlanList.First();

                apViewModel.bapm_id = (string)res["rz_bapm_id"];
                apViewModel.bapmStatus = (string)res["rz_bapm_status"];
                apViewModel.mpv_id = (string)res["mtrc_period_val_id"];
                apViewModel.mp_id = (string)res["mtrc_period_id"];
                apViewModel.bldgName = (string)res["dsc_mtrc_lc_bldg_name"];
                apViewModel.bldgId = (string)res["dsc_mtrc_lc_bldg_id"];
                apViewModel.mtrcDisplayText = (string)res["mtrc_prod_display_text"];
                apViewModel.apMonth = (string)res["month"];
                apViewModel.apMonthName = intToMonth((int)res["month"]);
                apViewModel.apYear = (string)res["year"];
                //apViewModel.goalText = (string)res["goal_txt"];
                apViewModel.goalText = ((string)res["goal_txt"]).Replace("<=", "&le;").Replace(">=", "&ge;");
                apViewModel.mtrcPeriodValue = (string)res["mtrc_period_val_value"];

                if (String.IsNullOrEmpty(apViewModel.bapm_id)) apViewModel.bapm_id = "0";
                if (String.IsNullOrEmpty(apViewModel.bapmStatus)) apViewModel.bapmStatus = "";
                if (String.IsNullOrEmpty(apViewModel.mp_id)) apViewModel.mp_id = "0";
                if (String.IsNullOrEmpty(apViewModel.mpv_id)) apViewModel.mpv_id = "0";
                if (String.IsNullOrEmpty(apViewModel.bldgName)) apViewModel.bldgName = "";
                if (String.IsNullOrEmpty(apViewModel.bldgId)) apViewModel.bldgId = "0";
                if (String.IsNullOrEmpty(apViewModel.mtrcDisplayText)) apViewModel.mtrcDisplayText = "";
                if (String.IsNullOrEmpty(apViewModel.apMonth)) apViewModel.apMonth = "0";
                if (String.IsNullOrEmpty(apViewModel.apMonthName)) apViewModel.apMonthName = "0";
                if (String.IsNullOrEmpty(apViewModel.apYear)) apViewModel.apYear = "0";
                if (String.IsNullOrEmpty(apViewModel.goalText)) apViewModel.goalText = "0";
                if (String.IsNullOrEmpty(apViewModel.mtrcPeriodValue)) apViewModel.mtrcPeriodValue = "0";

                JArray jDetailList = (JArray)res["details"];
                foreach (var detail in jDetailList)
                {
                    tempActionPlan = new Models.ActionPlan();

                    tempActionPlan.apd_id = (string)detail["rz_apd_id"];
                    tempActionPlan.apVersion = (string)detail["rz_apd_ap_ver"];
                    tempActionPlan.apStatus = (string)detail["rz_apd_ap_status"];
                    tempActionPlan.actionPlanAction = (string)detail["rz_apd_ap_text"];
                    tempActionPlan.reviewerComments = (string)detail["rz_apd_ap_review_text"];
                    tempActionPlan.submittedBy = (string)detail["submittedby"];
                    tempActionPlan.reviewedBy = (string)detail["reviewedby"];

                    if (String.IsNullOrEmpty(tempActionPlan.reviewerComments)) tempActionPlan.reviewerComments = "";
                    if (String.IsNullOrEmpty(tempActionPlan.submittedBy)) tempActionPlan.submittedBy = "";
                    if (String.IsNullOrEmpty(tempActionPlan.reviewedBy)) tempActionPlan.reviewedBy = "";

                    actionPlanList.Add(tempActionPlan);
                }

                actionPlanList = actionPlanList.OrderByDescending(x => Int32.Parse(x.apVersion)).ToList();

                apViewModel.actionPlanList = actionPlanList;
            }
            catch
            {

            }
            return apViewModel;
        }

        // ====================== HELPER FUNCTIONS ================================
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
        public int monthToInt(string monthName)
        {
            int monthNo = 0;
            switch (monthName.ToUpper())
            {
                case "JAN":
                case "JANUARY": monthNo = 1;
                    break;
                case "FEB":
                case "FEBRUARY": monthNo = 2;
                    break;
                case "MARCH": monthNo = 3;
                    break;
                case "APRIL": monthNo = 4;
                    break;
                case "MAY": monthNo = 5;
                    break;
                case "JUNE": monthNo = 6;
                    break;
                case "JULY": monthNo = 7;
                    break;
                case "AUG":
                case "AUGUST": monthNo = 8;
                    break;
                case "SEPT":
                case "SEPTEMBER": monthNo = 9;
                    break;
                case "OCT":
                case "OCTOBER": monthNo = 10;
                    break;
                case "NOV":
                case "NOVEMBER": monthNo = 11;
                    break;
                case "DEC":
                case "DECEMBER": monthNo = 12;
                    break;
                default:
                    break;
            }
            return monthNo;
        }
        public string parseToDate(string dateTime)
        {
            string returnDate = "";
            if (dateTime == "" || dateTime == null)
            {

            }
            else
            {
                DateTime tempDateTime = new DateTime();
                string tempDate = dateTime.Substring(0, 10);
                returnDate = (DateTime.TryParse(tempDate, out tempDateTime)) ? tempDateTime.ToString("MM/dd/yyyy") : "";
            }
            return returnDate;
        }
        private string getMonthShortName(string monthLong)
        {
            string monthShort = String.Empty;
            switch (monthLong)
            {
                case "January": monthShort = "Jan";
                    break;
                case "February": monthShort = "Feb";
                    break;
                //case "March":    monthShort = "March"; break;
                //case "April":    monthShort = "April"; break;
                //case "May":      monthShort = "May";   break;
                //case "June":     monthShort = "June";  break;
                //case "July":     monthShort = "July";  break;
                case "August": monthShort = "Aug";
                    break;
                case "September": monthShort = "Sept";
                    break;
                case "October": monthShort = "Oct";
                    break;
                case "November": monthShort = "Nov";
                    break;
                case "December": monthShort = "Dec";
                    break;
                default: monthShort = monthLong;
                    break;
            }

            return monthShort;
        }
        private string getNextDayName(string dayName) {
            switch (dayName.ToUpper()) {
                case "SUNDAY":
                    return "Monday";
                case "MONDAY":
                    return "Tuesday";
                case "TUESDAY":
                    return "Wednesday";
                case "WEDNESDAY":
                    return "Thursday";
                case "THURSDAY":
                    return "Friday";
                case "FRIDAY":
                    return "Saturday";
                case "SATURDAY":
                    return "Sunday";
                default:
                    return "";
            }
        }

        private string getNthDayofMonth(int nthDay, string DayName, string Month, string Year)
        {
            if (nthDay < 1 || nthDay > 5) { return "[Invalid Date]";}   //No month can have more than 5 instances of a given Day

            DateTime calMonth;

            if (DateTime.TryParse(String.Format("{0} 01, {1}", Month, Year), out calMonth) && !getNextDayName(DayName).Equals(""))
            {   // The input Date and the Day Name must be valid  at this point
                //For Metrics purposes, the meeting date is the following month
                calMonth = calMonth.AddMonths(1);
                string compareDay = calMonth.DayOfWeek.ToString().ToUpper();

                for (int x = 0; x < 7; x++) {
                    if (compareDay.ToUpper().Equals(DayName.ToUpper()))
                    {//We found the day we are looking for
                        calMonth = calMonth.AddDays(x);
                        break;
                    }
                    else {
                        compareDay = getNextDayName(compareDay);
                    }
                }
                //At this point we know the date of the first Day Name that we are looking for.
                //Add (nthDay-1) weeks to the date we found
                int daystoAdd = (nthDay - 1) * 7;
                //calMonth.AddDays((nthDay - 1)* 7);
                calMonth = calMonth.AddDays(daystoAdd);
                return calMonth.ToString("MMM dd, yyyy");
            }            

            return "[Invalid Date]";
        }
        //========= This Function "getMetricDisplayClass" returns the html class to use for Color Display for metric value cell based on the value, isGoalMet flag and metric status =========
        private string getMetricDisplayClass(string mValue, string isGoalMet, string status)
        {
            //Possible Display Classes to use are:
            //Open-metGoal       Open-Missed      
            //Closed-metGoal     Closed-Missed
            //Empty String     (No Display Class)
            if (String.IsNullOrEmpty(mValue)) return "cell-NoValue";
            if (status == "Open")
            {
                if (mValue == "N/A") return "Open-NA";
                if (isGoalMet == "Y") return "Open-metGoal";
                if (isGoalMet == "N") return "Open-Missed";
            }
            if (mValue == "N/A") return "Closed-NA";
            if (isGoalMet == "Y") return "Closed-metGoal";
            if (isGoalMet == "N") return "Closed-Missed";

            return "cell-NoValue";
        }

        //Not used Anymore. We will use Display Class field to indicate the Display coloring schema instead.
        //========= This Function "getMetricColor" returns the color the metric value cell should have based on the value, isGoalMet flag and metric status =========
        private string getMetricColor(string mValue, string isGoalMet, string status)
        {
            string mColor = "lightgray";
            if (String.IsNullOrEmpty(mValue)) return "";
            //if (mValue == "N/A"|| String.IsNullOrEmpty(mValue)) return "lightgray";
            if (mValue == "N/A") return "lightgray";
            if (status == "Open")
            {
                if (isGoalMet == "Y") return COLOR_GREEN;               //Open
                if (isGoalMet == "N") return COLOR_RED;
            }
            if (isGoalMet == "Y") return COLOR_LIGHT_GREEN;             //Closed
            if (isGoalMet == "N") return COLOR_LIGHT_RED;

            return mColor;
        }

        public string getMyTeam_Status(string ap_status)
        {
            string nextAction = "---";

            switch (ap_status)
            {
                case "Not Started":
                    //Check if User is Submitter for this building or Admin
                    nextAction = "Not Started";
                    break;
                case "WIP":         //Same as Rejected
                    nextAction = "In Process";
                    break;
                case "Rejected":
                    nextAction = "Rejected - Requires Rework";
                    break;
                case "Ready For Review":
                    nextAction = "Waiting for Review";
                    break;
                case "Approved":
                    nextAction = "Approved";
                    break;
                default:
                    nextAction = "N/A";
                    break;
            }

            return nextAction;
        }
        public string getMyTeam_AccountableParty(string ap_status)
        {
            string nextAction = "---";

            switch (ap_status)
            {
                case "Not Started":
                    //Check if User is Submitter for this building or Admin
                    nextAction = "Submitter";
                    break;
                case "WIP":         //Same as Rejected
                    nextAction = "Submitter";
                    break;
                case "Rejected":
                    nextAction = "Submitter";
                    break;
                case "Ready For Review":
                    nextAction = "Reviewer";
                    break;
                case "Approved":
                    nextAction = "N/A";
                    break;
                default:
                    nextAction = "N/A";
                    break;
            }

            return nextAction;
        }

    }
}