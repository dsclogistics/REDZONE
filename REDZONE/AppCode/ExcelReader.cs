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
        public ExcelMetric readExcelFile(HttpPostedFileBase file, string metricName, string metricYear, string metricMonth, string allBuildings, string metricDataType, bool na_allowed, string mtrcMinVal, string mtrcMaxVal, string maxDecPlaces, string maxStrSize)
        {
            const string ERRORCLASS = "alert-danger";
            const string VALIDCLASS = "valid";
            ExcelMetric eMetric = new ExcelMetric();
            eMetric.MetricName = String.Empty;
            eMetric.Month = String.Empty;
            eMetric.Year = String.Empty;
            string fileName = file.FileName;
            eMetric.isValidated = "True";
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
                    eMetric.MetricNameClass = VALIDCLASS;
                    if (!eMetric.MetricName.ToUpper().Equals(metricName.Trim().ToUpper()))
                    {
                        eMetric.MetricNameErrorMsg = "ERROR: Metric Uploaded is the incorrect Type. [ Metric Name '" + metricName + "' ] is Expected.";
                        eMetric.MetricNameClass = ERRORCLASS;
                        eMetric.isValidated = "False";
                        
                    }
                }
                catch (NullReferenceException)
                {
                    eMetric.MetricNameErrorMsg = "ERROR: Metric Name cannot be found in the spreadsheet. Invalid or incorrect Uploaded File Format ";
                    eMetric.MetricNameClass = ERRORCLASS;
                    eMetric.isValidated = "False";                    
                }
                try
                {
                    eMetric.Year = workSheet.Cells[2, 2].Value.ToString().Trim();
                    eMetric.MetricYearClass = VALIDCLASS;
                    if (!eMetric.Year.Equals(metricYear.Trim()))
                    {
                        eMetric.MetricYearErrorMsg = "ERROR: Year doesn't match Year in the SpreadSheet. Year '" + metricYear + "' was Expected.";
                        eMetric.MetricYearClass = ERRORCLASS;
                        eMetric.isValidated = "False";
                    }
                }
                catch (NullReferenceException)
                {
                    eMetric.MetricYearErrorMsg = "ERROR: Metric Year Value cannot be found in the spreadsheet";
                    eMetric.MetricYearClass = ERRORCLASS;
                    eMetric.isValidated = "False";
                }
                try
                {
                    eMetric.Month = workSheet.Cells[3, 2].Value.ToString().Trim();
                    eMetric.MetricMonthClass = VALIDCLASS;
                    if ((!eMetric.Month.Equals(metricMonth.Trim())))
                    {
                        eMetric.MetricMonthErrorMsg = "ERROR: Month doesn't match Month in the SpreadSheet.Year '" + metricMonth + "' was Expected.";
                        eMetric.MetricMonthClass = ERRORCLASS;
                        eMetric.isValidated = "False";
                    }
                }
                catch (NullReferenceException)
                {
                    eMetric.MetricMonthErrorMsg = "ERROR: Metric Month Value cannot be found in the spreadsheet";
                    eMetric.MetricMonthClass = ERRORCLASS;
                    eMetric.isValidated = "False";
                }
                if (eMetric.isValidated.Equals("False"))
                {
                    return eMetric;
                }
                for (int rowIterator = 6; rowIterator <= noOfRow; rowIterator++)
                {
                    Building bldg = new Building();
                    bldg.buildingName = String.Empty;
                    bldg.metricPeriodValue = String.Empty;
                    bldg.buildingErrorMsg = String.Empty;
                    bldg.valueErrorMsg = String.Empty;
                    bldg.buildingViewClass = VALIDCLASS;
                    try
                    {
                        bldg.buildingName = workSheet.Cells[rowIterator, 1].Value.ToString();
                        if (!Util.matchesSearchCriteria(bldg.buildingName.Trim().ToUpper(), allBuildings.ToUpper(), "Exact"))
                        {
                            bldg.buildingErrorMsg = "Building '" + bldg.buildingName + "' is not valid or it has not been set up.";
                            bldg.buildingViewClass = ERRORCLASS;
                            eMetric.isValidated = "False";
                        }
                    }
                    catch(NullReferenceException )
                    {
                        bldg.buildingErrorMsg = "Building name is not valid.";
                        bldg.buildingViewClass = ERRORCLASS;
                        eMetric.isValidated = "False";
                    }
                    try
                    {
                        bldg.metricPeriodValue = workSheet.Cells[rowIterator, 2].Value.ToString().Trim();
                        if (!Util.isValidDataType(metricDataType, bldg.metricPeriodValue, na_allowed))
                        {
                            bldg.valueErrorMsg = "Value must be Numeric. '" + bldg.metricPeriodValue + "' Is not valid.";
                            bldg.buildingViewClass = ERRORCLASS;
                            eMetric.isValidated = "False";
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
        public ExcelMetric readValidatedExcelFile(HttpPostedFileBase file)
        {

            ExcelMetric eMetric = new ExcelMetric();
            
            string fileName = file.FileName;
            string fileContentType = file.ContentType;
            byte[] fileBytes = new byte[file.ContentLength];
            var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
            using (var package = new ExcelPackage(file.InputStream))
            {
                var workSheet = package.Workbook.Worksheets[1];
                var noOfCol = workSheet.Dimension.End.Column;
                var noOfRow = workSheet.Dimension.End.Row;
                
                eMetric.MetricName = workSheet.Cells[1, 2].Value.ToString().Trim();
                eMetric.Year = workSheet.Cells[2, 2].Value.ToString().Trim();
                eMetric.Month = workSheet.Cells[3, 2].Value.ToString().Trim();


                for (int rowIterator = 6; rowIterator <= noOfRow; rowIterator++)
                {
                    Building bldg = new Building();
                    bldg.buildingName = String.Empty;
                    bldg.metricPeriodValue = String.Empty;
                    bldg.buildingErrorMsg = String.Empty;
                    bldg.valueErrorMsg = String.Empty;
                    bldg.buildingName = workSheet.Cells[rowIterator, 1].Value.ToString();
                    try
                    {

                        bldg.metricPeriodValue = workSheet.Cells[rowIterator, 2].Value.ToString().Trim();
                    }
                    catch (NullReferenceException)
                    {
                        bldg.metricPeriodValue = String.Empty;
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
        public string MetricNameClass { get; set; }
        public string MetricNameErrorMsg { get; set; }
        public string Year { get; set; }
        public string MetricYearClass { get; set; }
        public string MetricYearErrorMsg { get; set; }
        public string Month { get; set; }
        public string MetricMonthClass { get; set; }
        public string MetricMonthErrorMsg { get; set; }
        public string isValidated { get; set; }//this flag is "True" if excel file has no errors
        
        public List<Building> buildingList = new List<Building>();

    }
}
