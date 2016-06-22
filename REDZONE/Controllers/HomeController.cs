using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using REDZONE.ServerAPIs;
using Newtonsoft.Json.Linq;
using REDZONE.AppCode;

namespace REDZONE.Controllers
{
    
    public class HomeController : Controller
    {
      //================== GLOBAL CONTROLLER PROPERTIES =================
        DataRetrieval api = new DataRetrieval();
      //=================================================================

        public ActionResult Index()
        {
            //ServerAPIs.API_obs_getLC apiObject = new ServerAPIs.API_obs_getLC("DEV");

            //API_obs_getObserver apiObject = new API_obs_getObserver("DEV", "Feliciano", "Delgado", "feliciano.delgado@dsc-logistics.com");
            ////string resultData = apiObject.getJasonData();
            //ViewBag.APIresponse = apiObject.getJasonData();

            return View();
        }


        // GET: Menu
        [ChildActionOnly]
        public ActionResult _RZDM_MenuItems()
        { 
            List<RZMetricMenu> RZMenu = new List<RZMetricMenu>();
            JObject parsed_result = JObject.Parse(api.getMetricname("Red Zone", "Month"));
            DateTime defDate = DateTime.Today.AddMonths(-1);

            foreach (var res in parsed_result["metriclist"])
            {
                RZMetricMenu menuItem = new RZMetricMenu();
                menuItem.menuText = (string)res["mtrc_name"];
                menuItem.menuValue = "/Metric/EditView/" + (string)res["mtrc_id"] + "?month=" + defDate.ToString("MMMM") + "&year=" + defDate.ToString("yyyy");
                RZMenu.Add(menuItem);
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