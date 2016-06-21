using System;
using System.Web.Mvc;
using REDZONE.Models;
using REDZONE.App_Code;
using System.Web;
using OfficeOpenXml;
using System.Collections.Generic;

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



        public ActionResult Upload(FormCollection formCollection)
        {
            RZ_Metric rz_metric = new RZ_Metric();
            if (Request != null)
            {
                HttpPostedFileBase file = Request.Files["UploadedFile"];
                
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                   rz_metric = parcer.getRZ_Metric(6, "May", "2016", file);
                }
            }
            return View("Index", rz_metric);
        }

        public class Users
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public override string ToString(){
                return FirstName + " " + LastName + "<br/>";
            }
        }

    }// ------ Metric Comntroller Ends -----
}//----- REDZONE.Controllers Namespace Ends -----