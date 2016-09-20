using REDZONE.AppCode;
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

        //================== GLOBAL CONTROLLER PROPERTIES =================
        DataRetrieval api = new DataRetrieval();
        APIDataParcer parcer = new APIDataParcer();
        //=================================================================



        public PartialViewResult viewReasons(int? id)
        {
            int mtrc_period_val_id = id ?? 0;
            List<MPReason> MPAssignedReasons;

            // Get  (From API call) the list of Reason assigned to metric period Value id (param: id)
            MPAssignedReasons = parcer.getAssignedMetricPeriodReasons(mtrc_period_val_id.ToString());

            //Merge both lists into a single list
            //Return the merged list to the view for display

            //MetricValueReason reason1 = new MetricValueReason("First Reason", "This reason is hardcoded", false);
            //MPReasons.Add(reason1);
            //reason1 = new MetricValueReason("Second Reason", "No Comments", true);
            //MPReasons.Add(reason1);
            //=======================================================

            return PartialView(MPAssignedReasons);
        }     


        // GET: /MPVreasons/Assigment
        public ActionResult Assigment(int? id, string mpId, string returnUrl, string buildingID)
        {
            ViewBag.ReturnUrl = returnUrl + "&buildingID=" + buildingID;
            int mtrc_period_val_id = id ?? 0;
            List<MPReason> MPReasons;
            List<MPReason> MPAssignedReasons;

            // Get (From API call) the list of Existing Reasons for this metric period id (param: mpId)
            MPReasons = parcer.getMetricPeriodReasons(mpId);
            // Get  (From API call) the list of Reason assigned to metric period Value id (param: id)
            MPAssignedReasons = parcer.getAssignedMetricPeriodReasons(mtrc_period_val_id.ToString());

            //Merge both lists into a single list

            foreach (MPReason assignedReason in MPAssignedReasons) { 
                MPReason matchMPVR = MPReasons.Find(x => x.reason_id == assignedReason.reason_id);
                matchMPVR.isAssigned = true;
                matchMPVR.val_reason_id = assignedReason.val_reason_id;
                matchMPVR.mpvr_Comment = assignedReason.mpvr_Comment;
            }

            //Return the merged list to the view for display

            //MetricValueReason reason1 = new MetricValueReason("First Reason", "This reason is hardcoded", false);
            //MPReasons.Add(reason1);
            //reason1 = new MetricValueReason("Second Reason", "No Comments", true);
            //MPReasons.Add(reason1);
            //=======================================================


            //getMetricPeriodReasons
            
            

            //

            return View(MPReasons);
        }

        public string addUpdateMPVReasons(string mpvr_id, string mtrc_period_val_id, string mpr_id, string mpvr_comment)
        {
            // If "mpvr_id" is null or blank, then we are adding else we are updating
            string user_id = User.Identity.Name;
            string jPayload = "{\"assignedreasons\":[{\"mpvr_id\":\"" + mpvr_id + "\"," +
                   "\"mtrc_period_val_id\":\"" + mtrc_period_val_id + "\"," +
                   "\"mpr_id\":\"" + mpr_id + "\"," +
                   "\"user_id\":\"" + user_id + "\"," +
                   "\"mpvr_comment\":\"" + mpvr_comment + "\" }]}";

            string raw_data = api.addUpdateMPVReasons(jPayload);

            return raw_data;
        }

    }
}