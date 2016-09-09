using REDZONE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace REDZONE.Controllers
{
    public class MPVreasonsController : Controller
    {
        // GET: /MPVreasons/Assigment
        public ActionResult Assigment(int id, string mpId)
        {

            // Get (From API call) the list of Existing Reasons for this metric period id (param: mpId)
            // Get  (From API call) the list of Reason assigned to metric period Value id (parem: id)

            //Merge both lists into a single list

            //Return the merged list to the view for display
            List<MetricValueReason> list1 = new List<MetricValueReason>();

            MetricValueReason reason1 = new MetricValueReason("First Reason", "This reason is hardcoded", false);
            list1.Add(reason1);
            reason1 = new MetricValueReason("Second Reason", "No Comments", true);
            list1.Add(reason1);

            return View(list1);
        }     

    }
}