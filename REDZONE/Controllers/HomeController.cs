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
            bool filterByBuilding = (Session["buildingFilter"] != null && (string)Session["buildingFilter"] == "Y") ? true : false;
            string curMonth = REDZONE.AppCode.Util.intToMonth(defaultDate.Month);
            string curYear = defaultDate.Year.ToString();
            month = String.IsNullOrEmpty(month) ? curMonth : month;
            year = String.IsNullOrEmpty(year) ? curYear : year;
            ExecutiveSummaryViewModel dashBoard = parcer.getExcecutiveSummaryView(null, month, year,null, filterByBuilding);
            
            //dscUser currentUser = new dscUser(User.Identity.Name);
            //string[] userBuildingIdList ;
            //if(currentUser.buildings.Count > 0 && filterByBuilding){
            //    //The user has buildings assigned and the "filter by building" flag is turned on
            //    userBuildingIdList = currentUser.buildings.Select(x => x.id).ToArray();

            //    //Do an inner join of the Building list with the int list of building Ids assigned to the user to get the new Building list
            //    dashBoard.buildings = dashBoard.buildings.Join(userBuildingIdList, x => x.buildingId, id => id, (x, id) => x).ToList();
            //    //dashBoard.buildings = dashBoard.buildings.Where(x => userBuildingIdList.Contains(x.buildingId)).ToList();
            //}
            
            
            
            

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
            bool filterByBuilding = (Session["buildingFilter"] != null && (string)Session["buildingFilter"] == "Y") ? true : false;

            const string DFLT_BUILDING = "1";
            dscUser currentUser = new dscUser(User.Identity.Name);
            if (filterByBuilding && currentUser.buildings.Count == 0) { filterByBuilding = false; } //User does not have any buildings. Cannot apply filter

            if (filterByBuilding && currentUser.buildings.FirstOrDefault(x => x.id == buildingID) == null) { 
               //The requested Building is not in the user list of Building and we need to filter by the user buildings. Default to the first Building in the user list
                buildingID = currentUser.buildings[0].id;   //Exists at least one building. Default to the first one
            }

            if (String.IsNullOrEmpty(year)) { year = DateTime.Today.Year.ToString(); }  //Default to the current year
            if (String.IsNullOrEmpty(buildingID)) {    //Retrieve The current User list of building and select it's first                
                if (currentUser.buildings.Count > 0) { buildingID = currentUser.buildings[0].id; }
                else { buildingID = DFLT_BUILDING; }                
            }
            BuildingSummaryViewModel bldngSummary = parcer.getBuildingSummaryView(year, buildingID, User.Identity.Name, filterByBuilding);
            

            //Retrieve the Current Logged on User Reviewer's metrics (If any)
            ViewBag.ReviewerMetrics = currentUser.getReviewerMetricIds();
            return View(bldngSummary);
        }

        public ActionResult MetricSummary(string year, string metricID, string sortMonth, string sortDir)
        {
            bool filterByBuilding = (Session["buildingFilter"] != null && (string)Session["buildingFilter"] == "Y") ? true : false;

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

            MetricSummaryViewModel dashBoard = parcer.getMetricSummaryView(year, metricID, sortDir, filterByBuilding);

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
        
        public ActionResult Chat()
        {
            return View();
        }
    }

}