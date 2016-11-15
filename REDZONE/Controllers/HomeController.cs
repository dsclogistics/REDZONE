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
            const string DFLT_BUILDING = "1";
            dscUser currentUser = new dscUser(User.Identity.Name);

            if (String.IsNullOrEmpty(year)) { year = DateTime.Today.Year.ToString(); }  //Default to the current year
            if (String.IsNullOrEmpty(buildingID)) {    //Retrieve The current User list of building and select it's first                
                if (currentUser.buildings.Count > 0) { buildingID = currentUser.buildings[0].id; }
                else { buildingID = DFLT_BUILDING; }                
            }
            BuildingSummaryViewModel bldngSummary = parcer.getBuildingSummaryView(year, buildingID, User.Identity.Name);
            

            //Retrieve the Current Logged on User Reviewer's metrics (If any)
            ViewBag.ReviewerMetrics = currentUser.getReviewerMetricIds();
            return View(bldngSummary);
        }

        public ActionResult MetricSummary(string year, string metricID, string sortMonth, string sortDir)
        {
            dscUser currentUser = new dscUser(User.Identity.Name);

            //currentUser.buildings;

            const string DFLT_MPID = "3";

           // sortMonth =  REDZONE.AppCode.Util.getMonthLongName(sortMonth);
            // ------ SET DEFAULT VALUES ------
            if (String.IsNullOrEmpty(year)) { year = DateTime.Today.Year.ToString(); }  //Default to the current year
            if (String.IsNullOrEmpty(metricID))
            {    //Retrieve The current User list of Reviewer Met5rics and select it's first
                //dscUser currentUser = new dscUser(User.Identity.Name);
                //userRole reviewerRole = currentUser.roles.FirstOrDefault(x => x.roleName == "RZ_AP_REVIEWER");
                //if (reviewerRole == null || reviewerRole.roleMetrics.Count == 0) { metricID = DFLT_MPID; }
                //else { metricID = reviewerRole.roleMetrics[0].mpId; }
                metricID = DFLT_MPID;
            }
            if (String.IsNullOrEmpty(sortDir)) { sortDir = "ASC"; }
            if (String.IsNullOrEmpty(sortMonth)) { sortMonth = "Building"; }
            //------- Done setting up default values ------

            MetricSummaryViewModel dashBoard = parcer.getMetricSummaryView(year, metricID, sortDir);
            dashBoard.canFilterRows = (currentUser.buildings.Count > 0) ? true : false;
            if (dashBoard.canFilterRows) {
                //If the User Role allows Building Rows to be filtered, mark those that belong to the User
                foreach (MeasuredRowEntity buildingRow in dashBoard.metricRows)
                {
                    if (currentUser.buildings.Find(x => x.id == buildingRow.rowMeasuredId) != null)
                    { //This building is in the list of building that are assing to the Current user
                        buildingRow.rowOwner = "MyBuilding";
                    }
                    else {
                        buildingRow.rowOwner = "NotMyBuilding";
                    }
                }
            }

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


    }


}