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

        public ActionResult Index(string month, string year, string sortOrder)
        {
            //ExecutiveSummaryViewModel dashBoard = new ExecutiveSummaryViewModel();
            DateTime defaultDate = DateTime.Today.AddMonths(-1);
            string curMonth = REDZONE.AppCode.Util.intToMonth(defaultDate.Month);
            string curYear = defaultDate.Year.ToString();
            month = String.IsNullOrEmpty(month) ? curMonth : month;
            year = String.IsNullOrEmpty(year) ? curYear : year;
            ExecutiveSummaryViewModel dashBoard = parcer.getExcecutiveSummaryView(null, month, year,null);

            switch (sortOrder)
            {
                case "BD":
                    dashBoard.buildings = dashBoard.buildings.OrderByDescending(row => row.BuildingName).ToList();
                    //On click, specify the expected state of the building and score icons.
                    ViewData["bOrder"] = "BA";
                    ViewData["sOrder"] = "SA";
                    ViewData["bIcon"] = "glyphicon-sort-by-attributes-alt";
                    ViewData["sIcon"] = "glyphicon-sort";
                    ViewData["bntClassB"] = "btn-primary";
                    ViewData["bntClassS"] = "";
                    break;
                case "SA":
                    dashBoard.buildings = dashBoard.buildings.OrderBy(row => row.rowScore).ToList();
                    ViewData["bOrder"] = "BA";
                    ViewData["sOrder"] = "SD";
                    ViewData["bIcon"] = "glyphicon-sort";
                    ViewData["sIcon"] = "glyphicon-sort-by-attributes";
                    ViewData["bntClassB"] = "";
                    ViewData["bntClassS"] = "btn-primary";
                    break;
                case "SD":
                    dashBoard.buildings = dashBoard.buildings.OrderByDescending(row => row.rowScore).ToList();
                    ViewData["bOrder"] = "BA";
                    ViewData["sOrder"] = "SA";
                    ViewData["bIcon"] = "glyphicon-sort";
                    ViewData["sIcon"] = "glyphicon-sort-by-attributes-alt";
                    ViewData["bntClassB"] = "";
                    ViewData["bntClassS"] = "btn-primary";
                    break;
                default:
                    dashBoard.buildings = dashBoard.buildings.OrderBy(row => row.BuildingName).ToList();
                    ViewData["bOrder"] = "BD";
                    ViewData["sOrder"] = "SA";
                    ViewData["bIcon"] = "glyphicon-sort-by-attributes";
                    ViewData["sIcon"] = "glyphicon-sort";
                    ViewData["bntClassB"] = "btn-primary";
                    ViewData["bntClassS"] = "";
                    break;
            }

            return View(dashBoard);
        }

        public ActionResult BuildingSummary(string year, string buildingID)
        {
            BuildingSummaryViewModel bldngSummary = parcer.getBuildingSummaryView(year, buildingID);

            //Retrieve the Current Logged on User  Reviewer's metrics (If any)
            ViewBag.ReviewerMetrics = new dscUser(User.Identity.Name).getReviewerMetricIds();
            return View(bldngSummary);
        }

        public ActionResult MetricSummary(string year, string metricID, string sortMonth, string sortDir)
        {
           // sortMonth =  REDZONE.AppCode.Util.getMonthLongName(sortMonth);

            if (String.IsNullOrEmpty(sortDir)) { sortDir = "ASC"; }
            if (String.IsNullOrEmpty(sortMonth)) { sortMonth = "Building"; }

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
                    dashBoard.rowHeadings.displayClass = "";  //Reset the clss of the Building Colu, so it can be sorted ascending when clicked
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
            string userName = User.Identity.Name;
            List<RZMetricMenu> RZMenu = new List<RZMetricMenu>();

            try {
                List<int> allowedMetrics = parcer.getUserEditableMetrics(userName);  
            JObject parsed_result = JObject.Parse(api.getMetricname("Red Zone", "Month"));
            DateTime defDate = DateTime.Today.AddMonths(-1);
                foreach (var metricName in parsed_result["metriclist"])
            {
                    if (allowedMetrics.IndexOf((int)metricName["mtrc_period_id"]) != -1)
                {
                    RZMetricMenu menuItem = new RZMetricMenu();
                        menuItem.menuText = (string)metricName["mtrc_prod_display_text"];
                        menuItem.menuValue = "/Metric/EditView/" + (string)metricName["mtrc_id"] + "?month=" + defDate.ToString("MMMM") + "&year=" + defDate.ToString("yyyy");
                    RZMenu.Add(menuItem);
                }               
                }
            }
            catch(Exception e)
            {
                Session["globalAppError"] = "ERROR: Failed to access remote server." + Environment.NewLine + e.Message;
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