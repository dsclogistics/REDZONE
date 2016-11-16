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
        public ActionResult _RZDM_MenuItems()
        {
            string userName = User.Identity.Name;
            List<RZMetricMenu> RZMenu = new List<RZMetricMenu>();

            try
            {
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
            catch (Exception e)
            {
                Session["globalAppError"] = "ERROR: Failed to access remote server." + Environment.NewLine + e.Message;
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
        [ChildActionOnly]
        public ActionResult _RZ_TasksFlag()
        {
            dscUser user = new Models.dscUser(User.Identity.Name);
            RZTaskCounts rzTaskCounts = new RZTaskCounts();

            try
            {
                if (user.hasRole("RZ_BLDG_USER"))
                {
                    rzTaskCounts = parcer.getUserTeamActCount(user.dbUserId);
                }
                else {
                    rzTaskCounts = parcer.getUserTasksCount(user.dbUserId);
                }                
            }
            catch (Exception e)
            {
                Session["globalAppError"] = "ERROR: Failed to access remote server." + Environment.NewLine + e.Message;
            }

            return PartialView(rzTaskCounts);
        }
    }

    public class RZMetricMenu
    {
        public string menuText { set; get; }
        public string menuValue { set; get; }

    }
}