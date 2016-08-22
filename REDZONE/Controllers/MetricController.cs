using System;
using System.Web.Mvc;
using REDZONE.Models;
using REDZONE.AppCode;
using System.Web;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using OfficeOpenXml.Style;

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
            RZ_Metric rz_metric = parcer.getRZ_Metric(metricId, month, year);
            return View(rz_metric);
        }
        //= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
        [HttpPost]
        public ActionResult downloadTemplate(string metricName, string month, string year, string allBuildings) 
        {
            string[] buildings = allBuildings.Split(new[] { '~' }, StringSplitOptions.RemoveEmptyEntries);
            string fileName = @"RZ_" + metricName + "_" + month + "_" + year + ".xlsx";
            //string path = @"C:\DSC\RZ_" + metricName + "_" + month + "_" + year + ".xlsx";            
            //FileInfo newFile = new FileInfo(downloadPath);
            int curRow = 6;
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet ws = package.Workbook.Worksheets.Add(metricName + " " + month + ", " + year);

                //package.File = newFile;

                ws.View.ShowGridLines = false;
                ws.Cells["A1"].Value = "R/Z METRIC NAME";
                ws.Cells["A1"].Style.Font.Bold = true;
                ws.Cells["A1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells["A1"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                ws.Cells["A2"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells["A2"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                ws.Cells["A3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells["A3"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                ws.Cells["A5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells["A5"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                ws.Cells["B5"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                ws.Cells["B5"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                ws.Cells["B1"].Value = metricName;
                ws.Cells["A2"].Value = "YEAR";
                ws.Cells["A2"].Style.Font.Bold = true;
                ws.Cells["B2"].Value = year;
                ws.Cells["A3"].Value = "MONTH";
                ws.Cells["A3"].Style.Font.Bold = true;
                ws.Cells["B3"].Value = month;
                ws.Cells["A5"].Value = "Building";
                ws.Cells["A5"].Style.Font.Bold = true;
                ws.Cells["B5"].Value = "Metric Value";
                ws.Cells["B5"].Style.Font.Bold = true;
                //ws.Column(1).AutoFit();
                //ws.Column(2).AutoFit();
                ws.Column(1).Width = 22;
                ws.Column(2).Width = 18;
                ws.Cells[1,1,3,2].Style.Border.BorderAround(ExcelBorderStyle.Thin);
                ws.Cells[5, 1].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                ws.Cells[5,2].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                
                for (int i = 0; i < buildings.Length; i++)
                {
                    ws.Cells[curRow, 1].Value = buildings[i];
                    ws.Cells[curRow, 1].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                    ws.Cells[curRow, 2].Style.Border.BorderAround(ExcelBorderStyle.Medium);
                    ws.Row(curRow).Height = 16;
                    curRow++;
                }

                //newFile.Create();
                //newFile.MoveTo(@"C:/testSheet.xlsx");
                //package.SaveAs(newFile);
                
                //package.Save();
                
                //package.SaveAs(newFile);


                MemoryStream stream = new MemoryStream(package.GetAsByteArray());
                FileStreamResult result = new FileStreamResult(stream, "application/vnd.ms-excel")
                {
                    FileDownloadName = fileName
                };

                return result;
            }            
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
        
        [HttpPost]
        public string reloadVolume(string metricId, string month, string year)
        {
            string status = api.reloadVolume("volume", "Month", metricId, month, year);
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