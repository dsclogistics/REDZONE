using REDZONE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace REDZONE.Controllers
{
    public class ExecutiveSummaryController : Controller
    {
        // GET: ExecutiveSummary/MonthlySummary
        public ActionResult MonthlySummary(string rptMonth)
        {
            return View(testDashboard());
        }

        private ExecutiveSummaryViewModel testDashboard()
        {
            ExecutiveSummaryViewModel temp = new ExecutiveSummaryViewModel();

            // Load the Metric Header Info
            temp.month = "June";
            temp.year = "2016";
            temp.urlNextMonth = "";
            temp.urlPrevMonth = "";
            temp.statusNextMonth = "disabled";
            temp.statusPrevMonth = "disabled";
            // Load the Goals Row (Use dummy values)
            temp.goal = new BuildingMetricEntity("GOAL");
            //Load two sample building Rows
            BuildingMetricEntity sampleBuilding = new BuildingMetricEntity("AP");
            temp.buildings.Add(sampleBuilding);
            sampleBuilding = new BuildingMetricEntity("PP");
            temp.buildings.Add(sampleBuilding);
            sampleBuilding = new BuildingMetricEntity("PC");
            temp.buildings.Add(sampleBuilding);

            return temp;
        }
    }
}