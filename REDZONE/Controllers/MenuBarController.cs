using Newtonsoft.Json.Linq;
using REDZONE.AppCode;
using REDZONE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace REDZONE.Controllers
{
    public class MenuBarController : Controller
    {

        //================== GLOBAL CONTROLLER PROPERTIES =================
        DataRetrieval api = new DataRetrieval();
        APIDataParcer parcer = new APIDataParcer();
        //=================================================================


        // GET: Menu
        [ChildActionOnly]
        public ActionResult _RZDM_MenuOption()
        { 
            string userName = User.Identity.Name;
            dscUser loggedonUser = new dscUser(userName);      //Create a user instance of the Loggedon user
            RZMetricMenu RZMenu = new RZMetricMenu();          //Create an instance of a Metric Data Collection Menu object
            RZMenu.canCollect = false;
            string message = "";  ///////////////Do we need it????????????????????????????????????????????????????
            try
            {
                //Load the Data Collection Menu from the Logged User "MTRC_COLLECTOR" Role's Metric List (If it exist)                
                userRole collectorRole = loggedonUser.roles.FirstOrDefault(r => r.roleName == "MTRC_COLLECTOR");
                if (collectorRole != null)
                { //Load the Menu Metrics
                    DateTime defDate = DateTime.Today.AddMonths(-1);
                    RZMenu.canCollect = true;
                    foreach (roleMetric metricName in collectorRole.roleMetrics)
                    {
                        RZMetricMenuItem metricMenuItem = new RZMetricMenuItem();
                        metricMenuItem.menuText = metricName.mpDisplayName;
                        metricMenuItem.menuValue = "/Metric/EditView/" + metricName.metricId + "?month=" + defDate.ToString("MMMM") + "&year=" + defDate.ToString("yyyy");

                        RZMenu.DM_MetricItems.Add(metricMenuItem);
                    }
                }
                Session["globalAppError"] = "";
             }
            catch (Exception e)
            {
                Session["globalAppError"] = "ERROR: " + message + Environment.NewLine + " ==>" + e.Message;
            }

            return PartialView(RZMenu);
        }

        // GET: Tasks Menu
        [ChildActionOnly]
        public ActionResult _RZ_MyTask_MenuItems()
        {
            //dscUser user = new Models.dscUser(User.Identity.Name);
            //RZTaskCounts rzTaskCounts = new RZTaskCounts();
            //try
            //{
            //    rzTaskCounts = parcer.getUserTasksCount(user.dbUserId);
            //}
            //catch (Exception e)
            //{
            //    Session["globalAppError"] = "ERROR: Failed to access remote server." + Environment.NewLine + e.Message;
            //}

            //return PartialView(rzTaskCounts);
            return PartialView();
        }
        
        // GET: Tasks Menu
        [ChildActionOnly]
        public ActionResult _RZ_MyTeamACT_MenuItems()
        { 
            //dscUser user = new Models.dscUser(User.Identity.Name);
            //RZTaskCounts rzTaskCounts = new RZTaskCounts();
            //try
            //{
            //    rzTaskCounts = parcer.getUserTasksCount(user.dbUserId);
            //}
            //catch (Exception e)
            //{
            //    Session["globalAppError"] = "ERROR: Failed to access remote server." + Environment.NewLine + e.Message;
            //}

            //return PartialView(rzTaskCounts);
            return PartialView();
        }



        // GET: Tasks Menu
        [ChildActionOnly] [OutputCache(Duration=5)]
        public ActionResult _RZ_TasksFlag()
        {
            dscUser user = new Models.dscUser(User.Identity.Name);
            RZTaskCounts rzTaskCounts = new RZTaskCounts();

            try
            {
                //if (user.hasRole("RZ_BLDG_USER"))
                //{
                //    rzTaskCounts = parcer.getUserTeamActCount(user.dbUserId);
                //}
                //else {
                    rzTaskCounts = parcer.getUserTasksCount(user.dbUserId);
                //}                
                    Session["globalAppError"] = "";
            }
            catch (Exception e)
            {
                Session["globalAppError"] = "ERROR: Failed to retrieve the 'Task Count' for this user." + Environment.NewLine + e.Message;
            }

            return PartialView(rzTaskCounts);
        }

        // GET: Teams Menu
        [ChildActionOnly]
        public ActionResult _RZ_TeamsFlag()
        {
            dscUser user = new Models.dscUser(User.Identity.Name);
            TeamActivityCount teamActivityCount = new TeamActivityCount();

            try
            {
                string begYear = DateTime.Today.AddMonths(-3).Year.ToString();
                string begMonth = DateTime.Today.AddMonths(-3).Month.ToString();
                string endYear = DateTime.Today.Year.ToString();
                string endMonth = DateTime.Today.Month.ToString();

                teamActivityCount = parcer.getUserTeamActCount(user.dbUserId, begMonth, begYear, endMonth, endYear);
                Session["globalAppError"] = "";
            }
            catch (Exception e)
            {
                Session["globalAppError"] = "ERROR: Failed to retrieve the user's 'Team Task Count'." + Environment.NewLine + e.Message;
            }

            return PartialView(teamActivityCount);
        }
    }

    public class RZMetricMenu
    {
        public bool canCollect { set; get; }
        public List<RZMetricMenuItem> DM_MetricItems = new List<RZMetricMenuItem>();
    }

    public class RZMetricMenuItem 
    {
        public string menuText { set; get; }
        public string menuValue { set; get; }

    }
}