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
            string curMonth = REDZONE.AppCode.Util.intToMonth(defaultDate.Month);
            string curYear = defaultDate.Year.ToString();
            month = String.IsNullOrEmpty(month) ? curMonth : month;
            year = String.IsNullOrEmpty(year) ? curYear : year;
            ExecutiveSummaryViewModel dashBoard = parcer.getExcecutiveSummaryView(null, month, year,null);
            return View(dashBoard);
        }

        public ActionResult BuildingSummary(string year, string buildingID)
        {
            BuildingSummaryViewModel bldngSummary = parcer.getBuildingSummaryView(year, buildingID);

            return View(bldngSummary);
        }

        public ActionResult MetricSummary(string year, string metricID, string sortMonth, string sortDir)
        {
            MetricSummaryViewModel dashBoard = parcer.getMetricSummaryView(year, metricID, sortDir);
            if (!String.IsNullOrEmpty(sortMonth)) {
                if (sortMonth.Equals("Building"))
                {
                    if (sortDir.Equals("ASC")) { dashBoard.metricRows = dashBoard.metricRows.OrderBy(row => row.rowName).ToList(); }
                    else { dashBoard.metricRows = dashBoard.metricRows.OrderByDescending(row => row.rowName).ToList(); }
                    dashBoard.rowHeadings.displayClass = sortDir;
                }
                else {
                    if (sortDir.Equals("ASC")) { dashBoard.metricRows = dashBoard.metricRows.OrderBy(row => row.entityMetricCells.Single(x => x.metricName == sortMonth).metricDoubleValue).ToList(); }
                    else { dashBoard.metricRows = dashBoard.metricRows.OrderByDescending(row => row.entityMetricCells.Single(x => x.metricName == sortMonth).metricDoubleValue).ToList(); }
                    var sortedHdrCol = dashBoard.rowHeadings.entityMetricCells.Find(p => p.metricName == sortMonth);
                    sortedHdrCol.displayClass = sortDir;  // To let the view know which column was sorted (if any) and in what way ('ASC' or 'DESC')                
                } 
            }
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
                    menuItem.menuText = (string)res["mtrc_prod_display_text"];
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