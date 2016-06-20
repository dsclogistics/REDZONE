﻿using Newtonsoft.Json.Linq;
using REDZONE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REDZONE.App_Code
{

    public class APIDataParcer
    {
        DataRetrieval api = new DataRetrieval();

        public RZ_Metric getRZ_Metric(int metric_id, string month, string year)
        {
            RZ_Metric rz_metric = new RZ_Metric();
            //List<Building> buildings = new List<Building>();
            string raw_data = api.getMetricperiod("Red Zone", "Month", metric_id.ToString(), month, year);
            //if (raw_data.Length > 3) {
            //    // If the raw data retrieved has actual details and not just empty "{}" 
            //    rz_metric.detail_deleteme = @"{ ""dsc_mtrc_lc_bldg_name"": ""Allentown 2"",""dsc_mtrc_lc_bldg_id"": ""2"",""bmp_is_editable_yn"": ""Y"",""bmp_is_manual_yn"": ""Y"",""mtrc_period_val_id"": ""122"",""mtrc_period_val_value"": ""TEST""}";
            //    rz_metric.headerJson = raw_data.Substring(0, (raw_data.IndexOf('[') + 1)) + rz_metric.detail_deleteme + "]}";                
            //}
            JObject parsed_result = JObject.Parse(raw_data);

            try {
                rz_metric.prodName = (string)parsed_result["metricdetail"]["prod_name"];
                rz_metric.id = (int)parsed_result["metricdetail"]["mtrc_id"];
                rz_metric.metricName = (string)parsed_result["metricdetail"]["mtrc_name"];
                rz_metric.metricDataType = (string)parsed_result["metricdetail"]["data_type_token"];
                rz_metric.isNumeric = (string)parsed_result["metricdetail"]["data_type_num_yn"] == "Y" ? true : false;
                rz_metric.period_Type = METRICPERIODS.Month;
                rz_metric.metric_period_start_date = (DateTime)parsed_result["metricdetail"]["tm_per_start_dtm"];
                rz_metric.metric_period_end_date = (DateTime)parsed_result["metricdetail"]["tm_per_end_dtm"];
                rz_metric.periodName = (string)parsed_result["metricdetail"]["mtrc_period_name"] + " Metric Information";
                rz_metric.metricPeriodID = (int)parsed_result["metricdetail"]["mtrc_period_id"];
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
                    rz_metric.buildingList.Add(bldg);
                }
            }
            catch {
                // Default Some Dummy Values since valid data could not be retrieved
                DateTime currentMonth = new DateTime( Convert.ToInt16(year), monthToInt(month), 1);
                rz_metric.prodName = "Red Zone";
                rz_metric.id = metric_id;
                rz_metric.metricName = "Requested Metric Period does not Exist";
                rz_metric.period_Type = METRICPERIODS.Month;
                rz_metric.metric_period_start_date = currentMonth;
                rz_metric.metric_period_end_date = currentMonth;
                rz_metric.periodName = "Requested Metric Period does not Exist";
                rz_metric.metricPeriodID = 0;
            }

            return rz_metric;
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
    }
}