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
            string jPayload = "{\"assignedreasons\":[{";
            if (!String.IsNullOrEmpty(mpvr_id)) {
                jPayload = jPayload + "\"mpvr_id\":\"" + mpvr_id + "\",";
            }
            jPayload = jPayload + "\"mtrc_period_val_id\":\"" + mtrc_period_val_id + "\"," +
                    "\"mpr_id\":\"" + mpr_id + "\"," +
                    "\"user_id\":\"" + user_id + "\"," +
                    "\"mpvr_comment\":\"" + mpvr_comment + "\" }]}";

            string raw_data = api.addUpdateMPVReasons(jPayload);

            return raw_data;
        }


        public string modifyMPVReasons(string addList, string deleteList, string updateList)
        {//The order of the parameters on each list is ""
            // UPDATE: rMPValueReasonId , rMPValueId , rMPReasonId , rMPValueReasonComment  [~]
            // ADD:                       rMPValueId , rMPReasonId , rMPValueReasonComment  [~]
            // DELETE:  rMPValueReasonId [,]
            string user_id = User.Identity.Name;
            string mtrc_period_val_id = "";
            string mpr_id = "";
            string mpvr_id = "";
            string mpvr_comment = "";
            string addResults = "";
            string deleteResults = "";
            string updateResults = "";

            int counter = 0;
            // --------- Assign (ADD) New Metric Period Value Reason ------------------------------------------\
            if (!String.IsNullOrEmpty(addList))
            {
                string[] addReasonList = addList.Split('~');
                string jSonResponses = "";

                foreach (string item in addReasonList)
                { //Process each new reason to add
                    string[] reasonData = item.Split(',');
                    //Array order is:  rMPValueId , rMPReasonId , rMPValueReasonComment
                    mtrc_period_val_id = reasonData[0];
                    mpr_id = reasonData[1];
                    mpvr_comment = reasonData[2];

                    string jPayload = "{\"assignedreasons\":[{" + 
                            "\"mtrc_period_val_id\":\"" + mtrc_period_val_id + "\"," +
                            "\"mpr_id\":\"" + mpr_id + "\"," +
                            "\"user_id\":\"" + user_id + "\"," +
                            "\"mpvr_comment\":\"" + mpvr_comment + "\" }]}";

                    string raw_data = api.addUpdateMPVReasons(jPayload);
                    jSonResponses += raw_data;

                    counter++;
                    
                }
                addResults = counter + " Reasons to Add were found" + Environment.NewLine + jSonResponses;
            }
            else
            {  //Else bypass the process, there is nothing in the list.
                addResults = "No New Reasons have been assigned";
            }
            addResults += Environment.NewLine + "- - - - - - - - - - - - - - - - - - - - - - - - - - - - " + Environment.NewLine;
            //================ END OF THE "ADD" FUNCTIONALITY =================================================

            //-------- Remove (DELETE) Currently Assigned Metric Period Value Reasons ------------------------\
            if (!String.IsNullOrEmpty(deleteList))
            {
                counter = 0;
                string[] deleteReasonList = deleteList.Split(',');
                string jPayload = "";
                string jSonResponses = "";

                /// Json Payload to DELETE:
                /// {"reasonstodelete":
                ///   [
                ///     {"mpvr_id":"01"}
                ///     {"mpvr_id":"02"}
                ///   ]
                /// }
                /// 

                foreach (string item in deleteReasonList)
                { //Process each new reason to add
                    //Array order is:  rMPValueReasonId [,]
                    mpvr_id = item;
                    jPayload += "{\"mpvr_id\":\"" + mpvr_id + "\"},";
                    counter++;
                }
                jPayload = jPayload.Substring(0, (jPayload.Length - 1));
                jPayload = "{\"reasonstodelete\":[ " + jPayload + "]}";

                string raw_data = api.removeAssignedReason(jPayload);
                jSonResponses += raw_data;

                deleteResults = counter + " Reasons to Delete were found" + Environment.NewLine + jSonResponses;
            }
            else
            {  //Else bypass the process, there is nothing in the list.
                deleteResults = "No Assigned reasons have been deleted";
            }
            deleteResults += Environment.NewLine + "- - - - - - - - - - - - - - - - - - - - - - - - - - - - " + Environment.NewLine;
            //================ END OF THE "DELETE" FUNCTIONALITY ==============================================


            //-------- Update (UPDATE) Currently Assigned Metric Period Value Reasons ------------------------\
            if (!String.IsNullOrEmpty(updateList))
            {
                counter = 0;
                string[] updateReasonList = updateList.Split('~');
                string jSonResponses = "";

                foreach (string item in updateReasonList)
                { //Process each reason to Update
                    string[] reasonData = item.Split(',');
                    //Array order is:  rMPValueReasonId , rMPValueId , rMPReasonId , rMPValueReasonComment
                    mpvr_id = reasonData[0];
                    mtrc_period_val_id = reasonData[1];
                    mpr_id = reasonData[2];
                    mpvr_comment = reasonData[3];

                    string jPayload = "{\"assignedreasons\":[{" +
                            "\"mpvr_id\":\"" + mpvr_id + "\"," +
                            "\"mtrc_period_val_id\":\"" + mtrc_period_val_id + "\"," +
                            "\"mpr_id\":\"" + mpr_id + "\"," +
                            "\"user_id\":\"" + user_id + "\"," +
                            "\"mpvr_comment\":\"" + mpvr_comment + "\" }]}";

                    string raw_data = api.addUpdateMPVReasons(jPayload);

                    jSonResponses += raw_data;
                    counter++;

                }
                updateResults = counter + " Assigned Reason to updated were found." + Environment.NewLine + jSonResponses;
            }
            else
            {  //Else bypass the process, there is nothing in the list.
                updateResults = "No Assigned reasons have been Updated";
            }
            updateResults += Environment.NewLine + "- - - - - - - - - - - - - - - - - - - - - - - - - - - - " + Environment.NewLine;
            //================ END OF THE "UPDATE" FUNCTIONALITY =================================================

            // If "mpvr_id" is null or blank, then we are adding else we are updating
            //string jPayload = "{\"assignedreasons\":[{";
            //if (!String.IsNullOrEmpty(mpvr_id))
            //{
            //    jPayload = jPayload + "\"mpvr_id\":\"" + mpvr_id + "\",";
            //}
            //jPayload = jPayload + "\"mtrc_period_val_id\":\"" + mtrc_period_val_id + "\"," +
            //        "\"mpr_id\":\"" + mpr_id + "\"," +
            //        "\"user_id\":\"" + user_id + "\"," +
            //        "\"mpvr_comment\":\"" + mpvr_comment + "\" }]}";

            //string raw_data = api.addUpdateMPVReasons(jPayload);

            //return raw_data;

            return addResults + Environment.NewLine + deleteResults + Environment.NewLine + updateResults;
        }
    }
}