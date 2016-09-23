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

        public ExecutiveSummaryViewModel getExcecutiveSummaryView(string metric_id, string month, string year, string buildingID)
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
                        goalMetric.metricValue = (string)mtrValue["mpg_display_text"];
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

                        if (apiBuildingsMetrics.HasValues)
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

        public BuildingSummaryViewModel getBuildingSummaryView(string year, string buildingID)
        {
            const bool showAllMonths = true;        //Flag to control whether al months are shown Vs only those that have data

            //Define and Initializa Model Object Components
            BuildingSummaryViewModel bSummary = new BuildingSummaryViewModel();   // Main Metric Model Object. Returned back to calling method
            MeasuredRowEntity rowHeader = new MeasuredRowEntity();                //Model Contains one Header Row
            MeasuredRowEntity rowTotals = new MeasuredRowEntity();                //Model Contains one Totals Row
            MeasuredRowEntity rowActions = new MeasuredRowEntity();               //Model Contains one Row with Link Actions
            List<MeasuredRowEntity> metricsRowList = new List<MeasuredRowEntity>();         // Building Metrics (Row) List
            List<MeasuredCellEntity> metricValueCellList = new List<MeasuredCellEntity>();  // RowCellColection
            bSummary.year = year;
            rowHeader.rowName = bSummary.bName;
            rowTotals.rowName = "Goals Missed";
            rowActions.rowName = "ACTIONS PLAN";
            string raw_data = String.Empty;
            try
            {
                //------- Initialize the System Current Date/Time Deppendent Values -----------
                int intYear = 0;
                if (!(Int32.TryParse(year, out intYear))) { intYear = 9999; }
                bSummary.urlNextPeriod = String.Format("/Home/BuildingSummary/?year={0}&buildingID={1}", (intYear + 1).ToString(), buildingID);
                bSummary.urlPrevPeriod = String.Format("/Home/BuildingSummary/?year={0}&buildingID={1}", (intYear - 1).ToString(), buildingID);
                bSummary.statusPrevPeriod = (intYear <= 2016) ? "disabled" : "";
                bSummary.statusNextPeriod = (DateTime.Today.Year == intYear) ? "disabled" : "";
                //------- END of hardcoding data ------------------------------------------

                raw_data = api.getBuildingSummary("Red Zone", "Month", null, null, year, buildingID);
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
                                MeasuredCellEntity headerCol = new MeasuredCellEntity();
                                MeasuredCellEntity totalCol = new MeasuredCellEntity();
                                temp.metricName = (string)m["Month"];
                                temp.metricValue = String.Empty;
                                temp.isViewable = showAllMonths ? true : false;
                                row.entityMetricCells.Add(temp);
                                if (rowHeader.entityMetricCells.Count < months.Count)
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
                                    rowActions.entityMetricCells.Add(getActionDataforMonth((string)m["Month"], bSummary.year));
                                }
                            }
                        }
                        if (apiBuildingsMetrics.HasValues)
                        {
                            foreach (var apiCellValue in apiBuildingsMetrics)  // For each element of the full values Array retrieved by API
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

                                            tmp.mtrc_period_id = (string)apiCellValue["mtrc_period_id"];
                                            //..... Get the value in a formatted way
                                            int valDecPlaces = 0;      //Harcoded value for now, not used
                                            //if (!(Int32.TryParse("2", out valDecPlaces))) { valDecPlaces = 0; }
                                            tmp.metricValue = getFormattedValue((string)apiCellValue["mtrc_period_val_value"], (string)apiCellValue["data_type_token"], valDecPlaces);
                                            //......Enf of Getting Formatted Value

                                            //tmp.metricColor = getMetricColor(tmp.metricValue, (string)apiCellValue["mpg_mtrc_passyn"], (string)apiCellValue["rz_mps_status"]);
                                            tmp.displayClass = getMetricDisplayClass(tmp.metricValue, (string)apiCellValue["mpg_mtrc_passyn"], cellStatus);

                                            if (!cellStatus.Equals("Open"))
                                            {
                                                //The "score" field on the Action Row holds the number of metrics that are closed for that specific month
                                                try { rowActions.entityMetricCells.Find(p => p.metricMonth == (string)apiCellValue["MonthName"]).score++; }
                                                catch { }
                                            }

                                            if (tmp.isGoalMet == "N")
                                            {
                                                rowTotals.entityMetricCells.Single(x => x.metricName.ToUpper() == tmp.metricName.ToUpper()).score++;
                                                rowTotals.entityMetricCells.Single(x => x.metricName.ToUpper() == tmp.metricName.ToUpper()).metricValue = rowTotals.entityMetricCells.Single(x => x.metricName == tmp.metricName).score.ToString();
                                                row.redTotals++;
                                            }
                                            //If value missed the Goal, increase the Missed Goals counter
                                            // ---- TO DO ----  ////
                                            // <--- Finished increasing th Missed Goal Counter
                                            tmp.isViewable = true;
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
                    bSummary.buildingActionsRow = rowActions;
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

        private MeasuredCellEntity getActionDataforMonth(string actionMonth, string actionYear)
        {
            const string urlBase = "http://goo.gl/forms/";
            MeasuredCellEntity actionCell = new MeasuredCellEntity();
            DateTime metricDate = new DateTime(Convert.ToInt16(actionYear), monthToInt(actionMonth), 1);
            DateTime todayDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            //The Action Cell is visible only if it's the last month or the currnet month as long as they are closed
            actionCell.isViewable = ((metricDate.AddMonths(1) == todayDate) || (metricDate.AddMonths(2) == todayDate) || (metricDate == todayDate)) ? true : false;
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

        public MetricSummaryViewModel getMetricSummaryView(string year, string metricID, string sortDir)
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
                mSummary.missedGoals.rowName = "Goals Missed";
                header.rowName = "Buildings";
                MeasuredRowEntity goal = new MeasuredRowEntity();
                goal.rowName = "Goal";
                if (apiBuildings.HasValues)
                {
                    foreach (var b in apiBuildings)
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
                                hdrCell.displayClass = "cell-NoValue";
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
                }
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
                        try{ 
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

        //This method returns the list of all metrics Product Ids that the user is authorized to edit. 
        public List<int> getEditableMetrics(string userName)
        {
            List<int> accessibleMetrics = new List<int>();
            JObject parsed_result = JObject.Parse(api.authorizeUser(userName));
            try
            {
                foreach (var res in parsed_result["authorizationdetails"])
                {
                    int mtrc_period_id = (int)res["mtrc_prod_id"];
                    accessibleMetrics.Add(mtrc_period_id);
                }
            }
            catch { }
            return accessibleMetrics;
        }

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
            string[] prevNext = new string[] { "disabled", "disabled" };
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

                    MetricPeriodList.Add(tempMetricPeriod);
                }

            }
            catch
            {

            }
            return MetricPeriodList;
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
            switch (monthName)
            {
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

    }
}