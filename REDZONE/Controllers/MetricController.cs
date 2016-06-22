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
            bool na_allowed= true;
            //bool na_allowed = formCollection["na_allowed"];
            ExcelReader excelReader = new ExcelReader();
            ExcelMetric eMetric = new ExcelMetric();
            if (Request != null)
            {
                HttpPostedFileBase file = Request.Files["UploadedFile"];
                
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    eMetric = excelReader.readExcelFile(file, metricName, metricYear, metricMonth, allBuildings, metricDataType, na_allowed);
                }
            }
            return View("_Upload", eMetric);
        }

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
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                    var metricItems = new List<MetricItem>();
                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var workSheet = package.Workbook.Worksheets[1];
                        string excelMetricName = "";
                        string excelMonth = "";
                        string excelYear = "";
                        try
                        {
                            excelMetricName = workSheet.Cells[1, 2].Value.ToString().Trim().ToUpper();
                            excelYear = workSheet.Cells[2, 2].Value.ToString().Trim();
                            excelMonth = workSheet.Cells[3, 2].Value.ToString().Trim().ToUpper();
                        }
                        catch
                        {
                            Session["ErrorMessage"] = "Metric Name or Year or Month cannot be found in the SpreadSheet";
                            return RedirectToAction("ErrorMsg", "Error");
                        }
                        if (!excelMetricName.Equals(metricName.Trim().ToUpper()))
                        {
                            Session["ErrorMessage"] = "Metric Name doesn't match Metric name in the SpreadSheet";
                            return RedirectToAction("ErrorMsg", "Error");
                        }
                        else if (!excelMonth.Equals(metricMonth.Trim().ToUpper()))
                        {
                            Session["ErrorMessage"] = "Month doesn't match Month in the SpreadSheet";
                            return RedirectToAction("ErrorMsg", "Error");
                        }
                        else if (!excelYear.Equals(metricYear.Trim()))
                        {
                            Session["ErrorMessage"] = "Year doesn't match Year in the SpreadSheet";
                            return RedirectToAction("ErrorMsg", "Error");
                        }
                        rz_metric = parcer.getRZ_Metric(metricId, metricMonth, metricYear, file);
                    }
                }
            }
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