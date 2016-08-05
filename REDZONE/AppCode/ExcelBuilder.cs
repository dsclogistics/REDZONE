using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using System.IO;

namespace REDZONE.AppCode
{
    public class ExcelBuilder
    {
        public static void CreateExcelTemplate(string metricName, string year, string month, string[] buildings )
        {
            string path = @"C:\RedZone\RZ_"+metricName+"_"+month+"_"+year+".xlsx";
            FileInfo newFile = new FileInfo(path);
            int curCell = 6;         
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet ws = package.Workbook.Worksheets.Add(metricName+" "+month+", "+year);
                package.File = newFile;
                ws.Cells["A1"].Value = "R/Z Metric Name";
                ws.Cells["A1"].Style.Font.Bold=true;
                ws.Cells["B1"].Value = metricName;            
                ws.Cells["A2"].Value = "Year";
                ws.Cells["A2"].Style.Font.Bold = true;
                ws.Cells["B2"].Value = year;
                ws.Cells["A3"].Value = "Month";
                ws.Cells["A3"].Style.Font.Bold = true;
                ws.Cells["B3"].Value = month;               
                ws.Cells["A5"].Value = "Building";
                ws.Cells["A5"].Style.Font.Bold = true;
                ws.Cells["B5"].Value = "Metric Value";
                ws.Cells["B5"].Style.Font.Bold = true;
                for (int i =0; i < buildings.Length; i++)
                {
                    ws.Cells["A" + curCell.ToString()].Value = buildings[i];
                    curCell++;
                }
                ws.Column(1).AutoFit();
                ws.Column(2).AutoFit();
                //newFile.Create();
                //newFile.MoveTo(@"C:/testSheet.xlsx");
                //package.SaveAs(newFile);
                package.Save();
                //package.SaveAs(newFile);
            }
        }
    }
}