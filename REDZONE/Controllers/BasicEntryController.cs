using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using REDZONE.ServerAPIs;

namespace REDZONE.Controllers
{
    public class BasicEntryController : Controller
    {

        // GET: BasicEntry
        public ActionResult Index()
        {
            RZ_Metric1 mainMetric = new RZ_Metric1(DateTime.Today);
            API_RZ_MetricProducs apiObject = new API_RZ_MetricProducs("DEV", "test");
            ViewBag.APIresponse = apiObject.getJasonData();


            return View(mainMetric);
        }

        //================================= PRIVATE CONTROLLER MODELS ================================
        private class BuildingEntry
        {
            public string mBuildingName { get; set; }
            public string mBuildingValue { get; set; }
            //-------- Constructor -----------
            public BuildingEntry()
            {

            }//-------- END Constructor -----------
        }

        private class RZ_Metric1
        {
            public string month_Period { get; set; }
            public string tempProperty { get; set; }
            public List<string> periodTypes = new List<string>();
            public List<BuildingEntry> mBuildingEntries = new List<BuildingEntry>();

            //-------- Constructor -----------
            public RZ_Metric1(DateTime metricMonth)
            {
                month_Period = metricMonth.Month.ToString("mmm, yyyy");


            }//-------- END Constructor -----------
        }


    }

}