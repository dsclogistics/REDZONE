using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using REDZONE.ServerAPIs;
using REDZONE.Models;

namespace REDZONE.Controllers
{
    public class BasicMetricController : Controller
    {

        // GET: BasicEntry
        public ActionResult Index()
        {
            Building dscBldng;
            RZ_Metric mainMetric = new RZ_Metric(DateTime.Today, "Volume");
            dscBldng = new Building("Desplaines", "100", "PP", true);
            mainMetric.buildingList.Add(dscBldng);
            dscBldng = new Building("Perris", "150", "PC", true);
            mainMetric.buildingList.Add(dscBldng);
            dscBldng = new Building("Elwood", "125", "EL", true);
            mainMetric.buildingList.Add(dscBldng);

            //API_RZ_MetricProducs apiObject = new API_RZ_MetricProducs("DEV", "test");
            //ViewBag.APIresponse = apiObject.getJasonData();

            return View();
        }
        public ActionResult Volume()
        {
            Building dscBldng;
            RZ_Metric volumeMetric = new RZ_Metric(DateTime.Today, "Volume");
            dscBldng = new Building("Desplaines", "100", "PP", true);
            volumeMetric.buildingList.Add(dscBldng);
            dscBldng = new Building("Perris", "150", "PC", false);
            volumeMetric.buildingList.Add(dscBldng);
            dscBldng = new Building("Elwood", "125", "EL", true);
            volumeMetric.buildingList.Add(dscBldng);

            //API_RZ_MetricProducs apiObject = new API_RZ_MetricProducs("DEV", "test");
            //ViewBag.APIresponse = apiObject.getJasonData();

            return View(volumeMetric);
        }

    }

}