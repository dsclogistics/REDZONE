using OfficeOpenXml;
using REDZONE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REDZONE.App_Code
{
    public class ExcelReader
    {
        public ExcelMetric readExcelFile(HttpPostedFileBase file)
        {
            ExcelMetric Emetric = new ExcelMetric();

            //if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
            //{
                //ExcelMetric Emetric = new ExcelMetric();
                string fileName = file.FileName;
                string fileContentType = file.ContentType;
                byte[] fileBytes = new byte[file.ContentLength];
                var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                using (var package = new ExcelPackage(file.InputStream))
                {
                    //var currentSheet = package.Workbook.Worksheets;
                    //var workSheet = currentSheet.First();
                    var workSheet = package.Workbook.Worksheets[1];
                    var noOfCol = workSheet.Dimension.End.Column;
                    var noOfRow = workSheet.Dimension.End.Row;
                    Emetric.MetricName = workSheet.Cells[1, 2].Value.ToString();
                    Emetric.Year = workSheet.Cells[2, 2].Value.ToString();
                    Emetric.Month = workSheet.Cells[3, 2].Value.ToString();                    
                    for (int rowIterator = 6; rowIterator <= noOfRow; rowIterator++)
                    {
                        Building bldg = new Building();
                        bldg.buildingName = workSheet.Cells[rowIterator, 1].Value.ToString();
                        bldg.metricPeriodValue = workSheet.Cells[rowIterator, 2].Value.ToString();
                        Emetric.buildingList.Add(bldg);

                    }
                    return Emetric;
                }

        }
       
    }
    public class ExcelMetric
    {
        public string MetricName { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }

        public List<Building> buildingList = new List<Building>();

    }
}
