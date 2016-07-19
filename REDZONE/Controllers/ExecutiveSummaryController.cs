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
            ExecutiveSummaryViewModel testDashboard = new ExecutiveSummaryViewModel();

            testDashboard = dummyData();
            return View(testDashboard);
        }

        private ExecutiveSummaryViewModel dummyData()
        {
            ExecutiveSummaryViewModel temp = new ExecutiveSummaryViewModel();

            temp.month = "June";
            return temp;
        }
    }
}