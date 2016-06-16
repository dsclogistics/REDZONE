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
        // GET: Metric
        public ActionResult EditView(int id, string month, string year)
        {
          
   
            return View(parcer.getRZ_Metric(id,month,year));
        }
    }
}