using System;
using System.Web.Mvc;
using REDZONE.Models;

namespace REDZONE.Controllers
{
    public class MetricController : Controller
    {
        // GET: Metric
        public ActionResult EditView()
        {
            Building myBuilding;
            DateTime metricMonthDate = new DateTime(2016, 01, 01);
            RZ_Metric volumeMetric = new RZ_Metric(metricMonthDate, "Volume");

            myBuilding = new Building("Allentown 1", "20", "AP1", true);
            volumeMetric.buildingList.Add(myBuilding);
            myBuilding = new Building("Perris 3", "10000", "PC3", false);
            volumeMetric.buildingList.Add(myBuilding);
            return View(volumeMetric);
        }
    }
}