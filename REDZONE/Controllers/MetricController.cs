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
            string metricName = formCollection["metricName"];
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
                    using(var package = new ExcelPackage(file.InputStream))
                    {
                        //var currentSheet = package.Workbook.Worksheets;
                        //var workSheet = currentSheet.First();
                        var workSheet = package.Workbook.Worksheets[1];
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row - 5;


                        if (workSheet.Cells[1, 2].Value.ToString().Equals("Net FTE"))
                        { ViewBag.action = "You Have uploaded the correct Metric. Yay"; }
                        else { ViewBag.action = "You did not upload the correct Metric! Booh"; }

                        ViewBag.message = "Current Spreadsheet has " + noOfRow + " Rows and " + noOfCol + " columns.";
                        for (int rowIterator = 6; rowIterator <= noOfRow; rowIterator++)
                        {
                            var user = new MetricItem();
                            user.BuildingName = workSheet.Cells[rowIterator, 1].Value.ToString();
                            user.BldngMetricValue = workSheet.Cells[rowIterator, 2].Value.ToString();
                            metricItems.Add(user);
                            ViewBag.listofnames = ViewBag.listofnames + user.ToString();
                        }
                    }
                }
            }
            return View("Index", rz_metric);
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