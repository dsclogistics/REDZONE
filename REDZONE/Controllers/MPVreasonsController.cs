﻿using Newtonsoft.Json.Linq;
using REDZONE.AppCode;
using REDZONE.Models;
using REDZONE.ModelViews;
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


        [HttpPost]
        public PartialViewResult viewReasons(int? id, string buildingId, string mpId)
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
            dscUser currentUser = new dscUser(User.Identity.Name);
            //The Reason Comments can be viewed only if the user is one of these:
                //RZ_AP_SUBMITTER or RZ_BLDG_USER with correct building access 
                //RZ_AP_REVIEWER with correct metric access
                //RZ_ADMIN
            if (((currentUser.hasRole("RZ_AP_SUBMITTER") || currentUser.hasRole("RZ_BLDG_USER")) && currentUser.hasBuilding(buildingId)) || currentUser.hasRole("RZ_ADMIN"))
            {
                Session["viewComments"] = "Y";
            }
            else
                if (currentUser.hasRole("RZ_AP_REVIEWER") && currentUser.hasReviewerMetric(mpId))
                {
                    Session["viewComments"] = "Y";
                }
                else { Session["viewComments"] = "N";  }
           
            return PartialView(MPAssignedReasons);
        }     


        // GET: /MPVreasons/Assigment
        public ActionResult Assigment(int? id, string mpId, string returnUrl, string buildingID)
        {
            ViewBag.ReturnUrl = returnUrl + @"&buildingID=" + buildingID;
            int mtrc_period_val_id = id ?? 0;
            MPReasonViewModel mpReasonViewModel = new MPReasonViewModel();
            List<MPReason> MPReasons;
            List<MPReason> MPAssignedReasons;

            //Perform Basic Validation
            //Metric Period Id is mandatory. Yhrow an Exception if it is mising or invalid
            int intMPid = 0;
            if (!Int32.TryParse(mpId, out intMPid)) {
                throw new Exception("The provided Metric Id '" + mpId + "' is not valid." + Environment.NewLine + "Review your input and try again.");
            }

            // Get (From API call) the time, building and metric display information for this mpv Id
            mpReasonViewModel = parcer.getMetricPeriodValueInfo("Red Zone", mtrc_period_val_id.ToString());
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

            mpReasonViewModel.mpReasonList = MPReasons;

            //Return the merged list to the view for display

            //MetricValueReason reason1 = new MetricValueReason("First Reason", "This reason is hardcoded", false);
            //MPReasons.Add(reason1);
            //reason1 = new MetricValueReason("Second Reason", "No Comments", true);
            //MPReasons.Add(reason1);
            //=======================================================


            //getMetricPeriodReasons
            
            

            //

            return View(mpReasonViewModel);
        }

        // GET: /MPVreasons/_assignedNoStdrReason
        public ActionResult _assignedNoStdrReason(int? nsChkNameIndex, string reasonId, string valRsnId, string reasonText, string reasonComment, string wasUpdated, string ddItems)
        { 
            int iChkIndex = nsChkNameIndex ?? 0;
            string nsChkName = "fancy-checkbox-default-" + (iChkIndex).ToString("00");      //The control 'nsChkNameIndex' is increased by One and used as part of the name
            
            NonStdrReasonViewModel nsReason = new NonStdrReasonViewModel();
            //Initialize nsReason
            nsReason.checkBoxName = nsChkName;     //Name Must be unique for each Reason Check Box
            nsReason.originalStatus = String.IsNullOrEmpty(reasonId)?"":"checked";
            nsReason.reasonId = reasonId;
            nsReason.valueReasonId = valRsnId;
            nsReason.reasonText = reasonText;
            nsReason.wasUpdated = wasUpdated;
            nsReason.reasonComment = reasonComment;
            dropDownItem ddI;

            string[] ddListItems = ddItems.Split(new char[] {'~'}, StringSplitOptions.RemoveEmptyEntries);
            //Parse and load all the drop down Items to display
            foreach (string ddlElement in ddListItems) {
                //Split the properties of each List Item:   //"Reason Id","Reason Text", "is visible"
                string[] itemProperties = ddlElement.Split(',');
                ddI = new dropDownItem(itemProperties[0], itemProperties[1], itemProperties[2], itemProperties[3].Equals("true"));  
                nsReason.ddItems.Add(ddI);
            }
            return PartialView(nsReason);
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

        [HttpPost]
        public string modifyMPVReasons(string addList, string deleteList, string updateList)
        {//The order of the parameters on each list is ""
            // DELETE:  rMPValueReasonId [,]
            // ADD:                       rMPValueId , rMPReasonId , rMPValueReasonComment  [~]
            // UPDATE: rMPValueReasonId , rMPValueId , rMPReasonId , rMPValueReasonComment  [~]

            //The order of operations are: "Deletes" are performed first, "Add" second (Since sdome Ids need to be removed and then readded new
            // as to avoid any db insert errors. "Updates" are performed last
            string user_id = User.Identity.Name;
            string mtrc_period_val_id = "";
            string mpr_id = "";
            string mpvr_id = "";
            string mpvr_comment = "";
            string addResults = "";
            string deleteResults = "";
            string updateResults = "";
            int counter = 0;


            //-------- Remove (DELETE) Currently Assigned Metric Period Value Reasons ------------------------\
            deleteResults = "DELETE Operation:  --> ";
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
                
                JObject parsed_result = JObject.Parse(raw_data);
                string resultColor = ((string)parsed_result["result"]).ToUpper().Equals("SUCCESS") ? "blue" : "red";
                string actionMsg = "<span style=\"color:" + resultColor + "\">" + (string)parsed_result["result"] + ": " + counter + " " + (string)parsed_result["message"] + "</span>";
                deleteResults += "  [ " + counter + " Reasons to Delete Found ]" + Environment.NewLine + actionMsg;//jSonResponses + Environment.NewLine;
            }
            else
            {  //Else bypass the process, there is nothing in the list.
                deleteResults = "  [ No Assigned reasons were deleted ]" + Environment.NewLine;
            }
            deleteResults += Environment.NewLine + "- - - - - - - - - - - - - - - - - - - - - - - - - - - - " + Environment.NewLine;
            //================ END OF THE "DELETE" FUNCTIONALITY ==============================================


            // --------- Assign (ADD) New Metric Period Value Reason ------------------------------------------\
            addResults = "ADD Operation:  --> ";
            if (!String.IsNullOrEmpty(addList))
            {
                string[] addReasonList = addList.Split('~');
                string jSonResponses = "";
                string actionMsg = "";
                counter = 0;
                foreach (string item in addReasonList)
                { //Process each new reason to add
                    string[] reasonData = item.Split('^');
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


                    JObject parsed_result = JObject.Parse(raw_data);
                    if( ((string)parsed_result["result"]).ToUpper().Equals("SUCCESS")){
                        actionMsg += "<span style=\"color:blue\">Reason ID '" + mpr_id + "' Was added Successfully </span>" + Environment.NewLine;
                    }
                    else{
                        actionMsg += "<span style=\"color:red\">Reason ID '" + mpr_id + "' could not be added: " + (string)parsed_result["result"] + "</span>" + Environment.NewLine;
                    }
                }
                addResults += "  [ " + counter + " Reasons to Add Found ]" + Environment.NewLine + actionMsg; //jSonResponses + Environment.NewLine;
            }
            else
            {  //Else bypass the process, there is nothing in the list.
                addResults = "  [ No New Reasons to Add ]" + Environment.NewLine;
            }
            addResults += "- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - " + Environment.NewLine;
            //================ END OF THE "ADD" FUNCTIONALITY =================================================


            //-------- Update (UPDATE) Currently Assigned Metric Period Value Reasons ------------------------\
            updateResults = "UPDATE Operation:  --> ";
            if (!String.IsNullOrEmpty(updateList))
            {
                string actionMsg = "";
                try {
                    counter = 0;
                    string[] updateReasonList = updateList.Split('~');
                    string jSonResponses = "";
                    
                    foreach (string item in updateReasonList)
                    { //Process each reason to Update
                        string[] reasonData = item.Split('^');
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

                        JObject parsed_result = JObject.Parse(raw_data);
                        if (((string)parsed_result["result"]).ToUpper().Equals("SUCCESS"))
                        {
                            actionMsg += "<span style=\"color:blue\">Reason ID '" + mpr_id + "' Was Updated Successfully </span>" + Environment.NewLine;
                        }
                        else
                        {
                            actionMsg += "<span style=\"color:red\">Reason ID '" + mpr_id + "' could not be updated: " + (string)parsed_result["result"] + "</span>" + Environment.NewLine;
                        }

                    }
                    updateResults += "  [ " + counter + " Assigned Reason to updated Found ]" + Environment.NewLine + actionMsg; // jSonResponses + Environment.NewLine;
                }
                catch (Exception ex) {
                    actionMsg += "<span style=\"color:red\">Failed to Update Reasons: " + ex.Message + " </span>" + Environment.NewLine;
                    updateResults += "  [ " + counter + " Assigned Reasons were updated ]" + Environment.NewLine + actionMsg; // jSonResponses + Environment.NewLine;
                }
            }
            else
            {  //Else bypass the process, there is nothing in the list.
                updateResults = "  [ No Assigned reasons need Updating ]" + Environment.NewLine;
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