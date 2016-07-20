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
            ExecutiveSummaryViewModel dashBoard = new ExecutiveSummaryViewModel();

            // Load the Metric Header Info
            dashBoard.month = "June";
            dashBoard.year = "2016";
            dashBoard.urlNextMonth = "";
            dashBoard.urlPrevMonth = "";
            dashBoard.statusNextMonth = "disabled";
            dashBoard.statusPrevMonth = "disabled";
            // Load the Goals Row (Use dummy values)
            dashBoard.goal = new BuildingMetricEntity("GOAL");
            //Load two sample building Rows
            BuildingMetricEntity sampleBuilding = new BuildingMetricEntity("AP");
            dashBoard.buildings.Add(sampleBuilding);
            sampleBuilding = new BuildingMetricEntity("PP");
            dashBoard.buildings.Add(sampleBuilding);
            sampleBuilding = new BuildingMetricEntity("PC");
            dashBoard.buildings.Add(sampleBuilding);

            return dashBoard;
        }
    }
}