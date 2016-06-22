using OfficeOpenXml;
using REDZONE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REDZONE.AppCode
{
    public class ExcelReader
    {
        public ExcelMetric readExcelFile(HttpPostedFileBase file, string metricName, string metricYear, string metricMonth, string allBuildings, string metricDataType, bool na_allowed)
        {
            ExcelMetric eMetric = new ExcelMetric();
            eMetric.MetricName = String.Empty;
            eMetric.Month = String.Empty;
            eMetric.Year = String.Empty;
            string fileName = file.FileName;
            string fileContentType = file.ContentType;
            byte[] fileBytes = new byte[file.ContentLength];
            var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
            using (var package = new ExcelPackage(file.InputStream))
            {
                var workSheet = package.Workbook.Worksheets[1];
                var noOfCol = workSheet.Dimension.End.Column;
                var noOfRow = workSheet.Dimension.End.Row;
                try
                {
                    eMetric.MetricName = workSheet.Cells[1, 2].Value.ToString().Trim();
                    if (!eMetric.MetricName.ToUpper().Equals(metricName.Trim().ToUpper()))
                    {
                        eMetric.MetricNameErrorMsg = "Metric Name doesn't match Metric name in the SpreadSheet";
                    }
                }
                catch(NullReferenceException )
                {
                    eMetric.MetricNameErrorMsg = "Metric Name cannot be found in the spreadsheet";
                }
                try
                {
                    eMetric.Year= workSheet.Cells[2, 2].Value.ToString().Trim();
                    if (!eMetric.Year.Equals(metricYear.Trim()))
                    {
                        eMetric.MetricYearErrorMsg = "Year doesn't match Year in the SpreadSheet";
                    }
                }
                catch( NullReferenceException )
                {
                    eMetric.MetricYearErrorMsg = "Metric Year Value cannot be found in the spreadsheet";
                }
                try
                {
                    eMetric.Month = workSheet.Cells[3, 2].Value.ToString().Trim();
                    if ((!eMetric.Month.Equals(metricMonth.Trim())))
                    {
                        eMetric.MetricMonthErrorMsg = "Month doesn't match Month in the SpreadSheet";
                    }
                }
                catch(NullReferenceException )
                {
                    eMetric.MetricMonthErrorMsg = "Metric Month Value cannot be found in the spreadsheet";
                }                               
               
                for (int rowIterator = 6; rowIterator <= noOfRow; rowIterator++)
                {
                    Building bldg = new Building();
                    bldg.buildingName = String.Empty;
                    bldg.metricPeriodValue = String.Empty;
                    bldg.buildingErrorMsg = String.Empty;
                    bldg.valueErrorMsg = String.Empty;
                    try
                    {
                        bldg.buildingName = workSheet.Cells[rowIterator, 1].Value.ToString();
                        if (!Util.matchesSearchCriteria(bldg.buildingName.Trim().ToUpper(), allBuildings.ToUpper(), "Exact"))
                        {
                            bldg.buildingErrorMsg = "Can't find " + bldg.buildingName + " in the existing list of buildings";
                        }
                    }
                    catch(NullReferenceException )
                    {
                        bldg.buildingErrorMsg = "Can't find building name";
                    }
                    try
                    {
                        bldg.metricPeriodValue = workSheet.Cells[rowIterator, 2].Value.ToString();
                        if (!Util.isValidDataType(metricDataType, bldg.metricPeriodValue, na_allowed))
                        {
                            bldg.valueErrorMsg = "Value: " + bldg.metricPeriodValue + " is invalid for this metric";
                        }
                    }
                    catch (NullReferenceException)
                    {
                        
                    }                    
                    eMetric.buildingList.Add(bldg);

                }
                return eMetric;
            }

        }

    }

    public class ExcelMetric
    {
        public string MetricName { get; set; }
        public string MetricNameErrorMsg { get; set; }
        public string Year { get; set; }
        public string MetricYearErrorMsg { get; set; }
        public string Month { get; set; }
        public string MetricMonthErrorMsg { get; set; }

        public List<Building> buildingList = new List<Building>();

    }
}
