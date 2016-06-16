using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using REDZONE.App_Code;
using System.Diagnostics;

namespace REDZONE.Controllers
{
    public class MenuController : Controller
    {
        DataRetrieval api = new DataRetrieval();
        // GET: Menu
        [ChildActionOnly]
        public ActionResult _RedZoneDM()
        {
            List<RZMetricMenu> RZMenu = new List<RZMetricMenu>();
            JObject parsed_result = JObject.Parse(api.getMetricname("Red Zone", "Month"));
            DateTime defDate = DateTime.Today.AddMonths(-1);

            foreach (var res in parsed_result["metriclist"])
            {
                RZMetricMenu menuItem = new RZMetricMenu();
                menuItem.menuText = (string)res["mtrc_name"];
                menuItem.menuValue = "/Metric/EditView/" + (string)res["mtrc_id"]+"?month="+ defDate.ToString("MMMM")+"&year="+ defDate.ToString("yyyy");
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