using System;
using System.Web.Mvc;
using REDZONE.Models;
using REDZONE.App_Code;

namespace REDZONE.Controllers
{
    public class MetricController : Controller
    {
        DataRetrieval api = new DataRetrieval();
        // GET: Metric
        public ActionResult EditView(int id, string month, string year)
        {


            String json = api.getMetricperiod("Red Zone", "Month", id.ToString(), month, year);
            RZ_Metric metric = new RZ_Metric();
            metric.metricName = json;
            return View();
        }
    }
}