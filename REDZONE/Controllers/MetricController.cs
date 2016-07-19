using System;
using System.Web.Mvc;
using REDZONE.Models;
using REDZONE.AppCode;
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
            string returnURL = String.Format(@"/Metric/EditView/{0}?month={1}&year={2}", id, month, year);   // /Metric/EditView/6?month=June&year=2016
            int metricId = id ?? 0;
            if (metricId == 0) { returnURL = ""; }
            if (Session["username"] == null) { return RedirectToAction("LogOff", "Account", new { backUrl = returnURL }); }
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
        [HttpPost]
        public string closeRZMetricPeriod(string metricId, string metricMonth, string metricYear, string metricPeriodId)
        {
            string userName = Session["username"].ToString();
            return parcer.closeRZMetricPeriod(metricId, metricMonth, metricYear, userName, metricPeriodId);
        }

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =

        //GET: Metric/UploadPreview
        public ActionResult UploadPreview(RZ_Metric myData)
        {
            return PartialView("_UploadPreview", myData);
        }
      
        public ActionResult Upload(FormCollection formCollection)
        {
            int metricId = Convert.ToInt32(formCollection["metricId"]);
            string metricName = formCollection["metricName"];
            string metricMonth = formCollection["metricMonth"];
            string metricYear = formCollection["metricYear"];
            string allBuildings = formCollection["allBuildings"]; 
            string metricDataType = formCollection["metricDataType"];
            bool na_allowed = Convert.ToBoolean(formCollection["na_allowed"]);
            string mtrcMinVal = formCollection["mtrcMinVal"];
            string mtrcMaxVal = formCollection["mtrcMaxVal"];
            string maxDecPlaces = formCollection["maxDecPlaces"];
            string maxStrSize = formCollection["maxStrSize"];

            ExcelReader excelReader = new ExcelReader();
            ExcelMetric eMetric = new ExcelMetric();
            if (Request != null)
            {
                HttpPostedFileBase file = Request.Files["UploadedFile"];
                
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    eMetric = excelReader.readExcelFile(file, metricName, metricYear, metricMonth, allBuildings, metricDataType, na_allowed, mtrcMinVal, mtrcMaxVal, maxDecPlaces, maxStrSize);
                }
            }
            return PartialView("_Upload", eMetric);
            //return View("_Upload", eMetric);
        }

    
        [ValidateAntiForgeryToken]
        public ActionResult UploadValidated(FormCollection formCollection)
        {
            int metricId = Convert.ToInt32(formCollection["metricId"]);
            string metricName = formCollection["metricName"];
            string metricMonth = formCollection["metricMonth"];
            string metricYear = formCollection["metricYear"];
            RZ_Metric rz_metric = new RZ_Metric();
            if (Request != null)
            {
                HttpPostedFileBase file = Request.Files["UploadedFile"];

                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {                   
                        rz_metric = parcer.getRZ_Metric(metricId, metricMonth, metricYear, file);                    
                }
            }
            ViewBag.fileUploaded = "Y";
            return View("EditView", rz_metric);
        }

        public class MetricItem
        {
            public string BuildingName { get; set; }
            public string BldngMetricValue { get; set; }
            public override string ToString(){
                return BuildingName + "*" + BldngMetricValue + "~";
            }
        }

    }// ------ Metric Comntroller Ends -----
}//----- REDZONE.Controllers Namespace Ends -----