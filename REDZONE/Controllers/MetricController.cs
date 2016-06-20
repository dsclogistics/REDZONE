using System;
using System.Web.Mvc;
using REDZONE.Models;
using REDZONE.App_Code;

namespace REDZONE.Controllers
{
    public class MetricController : Controller
    {
        DataRetrieval api = new DataRetrieval();
        APIDataParcer parcer = new APIDataParcer();

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
        // GET: Metric
        public ActionResult EditView(int? id, string month, string year)
        {
            int metricId = id ?? 0;
            if (metricId == 0) { return RedirectToAction("NotFound", "Error"); }
            return View(parcer.getRZ_Metric(metricId, month, year));
        }
        //= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
        [HttpPost]
        public string saveRZMetric(string raw_json)
        {
            string status = api.saveRZMetric(raw_json);
            if (status.ToLower().Contains("success"))
            {
                return "Success";
            }
            else { return status; }
            
        }
        //= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =

    }
    

  
}