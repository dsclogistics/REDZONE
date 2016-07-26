using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using REDZONE.ServerAPIs;
using Newtonsoft.Json.Linq;
using REDZONE.AppCode;
using REDZONE.Models;

namespace REDZONE.Controllers
{
    
    public class HomeController : Controller
    {
      //================== GLOBAL CONTROLLER PROPERTIES =================
        DataRetrieval api = new DataRetrieval();
        APIDataParcer parcer = new APIDataParcer();
        //=================================================================

        public ActionResult Index(string month, string year)
        {
            //ExecutiveSummaryViewModel dashBoard = new ExecutiveSummaryViewModel();
            DateTime defaultDate = DateTime.Today.AddMonths(-1);
            string curMonth = REDZONE.AppCode.APIDataParcer.intToMonth(defaultDate.Month);
            string curYear = defaultDate.Year.ToString();
            month = String.IsNullOrEmpty(month) ? curMonth : month;
            year = String.IsNullOrEmpty(year) ? curYear : year;
            ExecutiveSummaryViewModel dashBoard = parcer.getExcecutiveSummaryView(null, month, year,null);

            //ExecutiveSummaryViewModel dashBoard = new ExecutiveSummaryViewModel();
            //// Load the Metric Header Info
            //dashBoard.month = curMonth;
            //dashBoard.year = curYear;
            //dashBoard.urlNextMonth = "";
            //dashBoard.urlPrevMonth = "";
            //dashBoard.statusNextMonth = "disabled";
            //dashBoard.statusPrevMonth = "disabled";
            //// Load the Goals Row (Use dummy values)
            //dashBoard.goal = new BuildingMetricEntity("GOAL");
            ////Load two sample building Rows
            //BuildingMetricEntity sampleBuilding = new BuildingMetricEntity("AP");
            //dashBoard.buildings.Add(sampleBuilding);
            //sampleBuilding = new BuildingMetricEntity("PP");
            //dashBoard.buildings.Add(sampleBuilding);
            //sampleBuilding = new BuildingMetricEntity("PC");
            //dashBoard.buildings.Add(sampleBuilding);

            //dashBoardNew.goal = dashBoard.goal;
            return View(dashBoard);
        }
        public ActionResult BuildingSummary(string year, string buildingID)
        {
            ExecutiveSummaryViewModel dashBoard = parcer.getExcecutiveSummaryView(null, null, year, buildingID);
            return View(dashBoard);
        }
        public ActionResult MetricSummary(string year, string metricID)
        {
            ExecutiveSummaryViewModel dashBoard = parcer.getExcecutiveSummaryView(metricID, null, year, null);
            return View(dashBoard);
        }


        // GET: Menu
        [ChildActionOnly]
        public ActionResult _RZDM_MenuItems()
        { 
            List<RZMetricMenu> RZMenu = new List<RZMetricMenu>();
            JObject parsed_result = JObject.Parse(api.getMetricname("Red Zone", "Month"));
            DateTime defDate = DateTime.Today.AddMonths(-1);
            string userName = User.Identity.Name;

            List<int> allowedMetrics = parcer.getEditableMetrics(userName);
            foreach (var res in parsed_result["metriclist"])
            {
                if (allowedMetrics.IndexOf((int)res["mtrc_period_id"]) != -1)
                {
                    RZMetricMenu menuItem = new RZMetricMenu();
                    menuItem.menuText = (string)res["mtrc_name"];
                    menuItem.menuValue = "/Metric/EditView/" + (string)res["mtrc_id"] + "?month=" + defDate.ToString("MMMM") + "&year=" + defDate.ToString("yyyy");
                    RZMenu.Add(menuItem);
                }               
            }
            return PartialView(RZMenu);
        }
    }

    public class RZMetricMenu
    {
        public string menuText { set; get; }
        public string menuValue { set; get; }

    }
}