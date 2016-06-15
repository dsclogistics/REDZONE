using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using REDZONE.App_Code;

namespace REDZONE.Controllers
{
    public class MenuController : Controller
    {
        DataRetrieval api = new DataRetrieval();
        // GET: Menu
        public ActionResult _RedZoneDM()
        {
            List<RZMetricMenu> RZMenu = new List<RZMetricMenu>();
            JObject parsed_result = JObject.Parse(api.getMetricname("Red Zone", "Month"));
            foreach (var res in parsed_result["metriclist"])
            {
                RZMetricMenu menuItem = new RZMetricMenu();
                menuItem.menuText = (string)res["mtrc_name"];
                menuItem.menuValue = "/BasicMetric/Volume/"+(string)res["mtrc_id"];
                RZMenu.Add(menuItem);
            }
            return View(RZMenu);
        }
    }


    public class RZMetricMenu
    {
        public string menuText { set; get; }
        public string menuValue { set; get; }

    }
}