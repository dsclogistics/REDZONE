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

            ExecutiveSummaryViewModel dashBoard = new ExecutiveSummaryViewModel();
            //ExecutiveSummaryViewModel dashBoardNew = parcer.getExcecutiveSummaryView(0, "May", "2016");

            // Load the Metric Header Info
            dashBoard.month = "June";
            dashBoard.year = "2016";
            dashBoard.urlNextMonth = "";
            dashBoard.urlPrevMonth = "";
            dashBoard.statusNextMonth = "disabled";
            dashBoard.statusPrevMonth = "disabled";

            return View(dashBoard);
        }
    }
}