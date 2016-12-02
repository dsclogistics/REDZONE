using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using REDZONE.Models;
using REDZONE.AppCode;
using Newtonsoft.Json.Linq;

namespace REDZONE.Controllers
{
    public class ActionPlanController : Controller
    {
        private DataRetrieval api = new DataRetrieval();
        private APIDataParcer dataParcer = new APIDataParcer();

        [HttpGet]
        public ActionResult viewEdit(int? mp_id, int? bldg_id, int? bapm_id)
        {
            //Valid Action Pan Status values are:
            // [rz_bapm_status] = 'Not Started'  OR 'WIP' OR 'Ready For Review' OR 'Approved' OR 'Rejected'

            // Testing: http://localhost:56551/ActionPlan/viewEdit/?mp_id=1&bldg_id=40&bapm_id=39

            // The "id" received as a parameter is the Building Action Plan Metric id ('bapm_id'). We will pass that to the API to get the action plan detail data.
            int bapmId = bapm_id ?? 0;
            int intBldgId = bldg_id ?? 0;
            int intMpId = mp_id ?? 0;
            string productName = "Red Zone";

            //-------------------------------
            //Populate Action Plan View Model
            //-------------------------------
            //List of Action Plans starting with the current as first item of the list
            //------------------------------------------------------------------------
            ActionPlanViewModel apViewModel = new ActionPlanViewModel();

            apViewModel = dataParcer.getActionPlansById(productName, bapmId.ToString());


            //ViewBag.curUserRole = REDZONE.AppCode.Util.getUserRoles(User.Identity.Name);
            ViewBag.bapmId = bapmId;

            //Add List of Reasons 
            //-------------------
            int mpv_id = 0;
            if (!Int32.TryParse(apViewModel.mpv_id, out mpv_id)) { mpv_id = 0; };
            apViewModel.reasonList = dataParcer.getAssignedMetricPeriodReasons(mpv_id.ToString());

            //Add List of Recent Action Plans
            //-------------------------------
            List<PriorActionPlan> recentAPList = new List<PriorActionPlan>();
            string mpId = intMpId.ToString();
            string bldgId = intBldgId.ToString();
            string begmonth = DateTime.Now.Month.ToString();
            string begyear = (DateTime.Now.Year - 1).ToString();
            string endmonth = begmonth;
            string endyear = DateTime.Now.Year.ToString();

            recentAPList = dataParcer.getRecentActionPlanList(productName, mpId, bldgId, begmonth, begyear, endmonth, endyear);

            PriorActionPlan currentAP = dataParcer.getMostRecentAP(productName, mpId, bldgId, DateTime.Today.AddMonths(-11).Month.ToString(), DateTime.Today.AddMonths(-11).Year.ToString(), 
                                                            DateTime.Today.AddMonths(-1).Month.ToString(), DateTime.Today.AddMonths(-1).Year.ToString());

            if (currentAP.bapm_id != null && currentAP.priorAPStatus != "Approved") { recentAPList.Add(currentAP); } 

            foreach (PriorActionPlan priorAP in recentAPList)
            {
                if(bapmId.ToString() == priorAP.bapm_id)
                {
                    //ViewBag.metricDate = priorAP.priorAPMonth + " " + priorAP.priorAPYear;
                    recentAPList.Remove(priorAP);
                    break;
                }
            }

            apViewModel.priorActionPlanList = recentAPList.OrderByDescending(x => dataParcer.monthToInt(x.priorAPMonth)).ThenByDescending(x => x.priorAPYear).ToList();


            //-----------
            //Model logic
            //-----------
            if (apViewModel.actionPlanList.Count > 0)
            {
                if (apViewModel.actionPlanList.First().apStatus == "Rejected")
                {
                    //When action plan list is NOT empty, but most recent version status is Rejected, pass in "empty" model
                    ActionPlan newActionPlan = new ActionPlan();
                    newActionPlan.apd_id = "";
                    newActionPlan.apVersion = (Int32.Parse(apViewModel.actionPlanList.First().apVersion) + 1).ToString();
                    newActionPlan.apStatus = "Rejected New";
                    newActionPlan.actionPlanAction = "";
                    newActionPlan.reviewerComments = "";
                    apViewModel.actionPlanList.Insert(0, newActionPlan);
                    apViewModel.currentAPStatus = "Rejected New";
                    apViewModel.currentAPVersion = newActionPlan.apVersion;
                }
                else
                {
                    //When action plan list is NOT empty, and most recent version status is 'WIP', 'Ready for Review', or 'Approved', pass model as is.
                    //apViewModel.currentAPVersion = apViewModel.actionPlanList.First().apVersion;
                }
            }
            else if (apViewModel.actionPlanList.Count == 0 && (apViewModel.bapmStatus == "Not Started" || apViewModel.bapmStatus == "Expired"))
            {
                //When action plan list is empty, pass in "empty" model
                ActionPlan newActionPlan = new ActionPlan();
                newActionPlan.apd_id = "";
                newActionPlan.apVersion = "1";
                newActionPlan.apStatus = "Not Started";
                newActionPlan.actionPlanAction = "";
                newActionPlan.reviewerComments = "";
                apViewModel.actionPlanList.Add(newActionPlan);
                apViewModel.currentAPStatus = "Not Started";
                apViewModel.currentAPVersion = newActionPlan.apVersion;
            }
            else
            {
                //When action plan list is empty, pass in "empty" model
                ActionPlan newActionPlan = new ActionPlan();
                newActionPlan.apd_id = "";
                newActionPlan.apVersion = "1";
                newActionPlan.apStatus = "";
                newActionPlan.actionPlanAction = "";
                newActionPlan.reviewerComments = "";
                apViewModel.actionPlanList.Add(newActionPlan);
                apViewModel.bapmStatus = "";
            }

            //----------------------
            //Set User Authorization
            //----------------------           
            dscUser actionPlanUser = new dscUser(User.Identity.Name);

            if ((actionPlanUser.hasRole("RZ_AP_SUBMITTER") || actionPlanUser.hasRole("RZ_BLDG_USER")) 
                && actionPlanUser.hasBuilding(intBldgId.ToString()))
            {
                apViewModel.canAccessAP = true;
                apViewModel.canEditReasons = (actionPlanUser.hasRole("RZ_BLDG_USER")) ? false : true;
                apViewModel.canViewFinishedAP = true;
                apViewModel.canViewWipAP = true;
                apViewModel.canSubmitAP = (actionPlanUser.hasRole("RZ_BLDG_USER")) ? false : true;
            }
            else if (actionPlanUser.hasRole("RZ_AP_REVIEWER") && actionPlanUser.hasReviewerMetric(intMpId.ToString()))
            {
                apViewModel.canAccessAP = true;
                apViewModel.canViewFinishedAP = true;
                apViewModel.canReviewAP = true;
            }
            else if (actionPlanUser.hasRole("RZ_ADMIN"))
            {
                apViewModel.canAccessAP = true;
                apViewModel.canEditReasons = true;
                apViewModel.canViewFinishedAP = true;
                apViewModel.canViewWipAP = true;
                apViewModel.canSubmitAP = true;
                apViewModel.canReviewAP = true;
            }

            if(apViewModel.actionPlanList.Count() > 0 && apViewModel.bapmStatus != "" && apViewModel.bapmStatus != "Expired")
            {
                apViewModel.currentAPVersion = apViewModel.actionPlanList.First().apVersion;
                apViewModel.currentAPStatus = apViewModel.actionPlanList.First().apStatus;
                apViewModel.AP_EditableStatus = (apViewModel.canSubmitAP) ? "" : "readonly";
                apViewModel.review_EditableStatus = "";
                apViewModel.displayStatus = "";
                apViewModel.displayColor = "green";
                apViewModel.pageHeader = "View Action Plan";

                switch (apViewModel.currentAPStatus)
                {
                    case "Not Started":
                        apViewModel.displayStatus = "New";
                        apViewModel.displayColor = "orange";
                        apViewModel.AP_EditableStatus = (apViewModel.canSubmitAP) ? "" : "readonly";
                        apViewModel.AP_EditableColor = (apViewModel.canSubmitAP) ? "transparent" : "#eee";
                        apViewModel.review_EditableStatus = "disabled";
                        apViewModel.pageHeader = (apViewModel.canSubmitAP) ? "Start Action Plan" : "View Action Plan";
                        break;
                    case "WIP":
                        apViewModel.displayStatus = "In Process";
                        apViewModel.displayColor = "orange";
                        apViewModel.AP_EditableStatus = (apViewModel.canSubmitAP) ? "" : "readonly";
                        apViewModel.AP_EditableColor = (apViewModel.canSubmitAP) ? "transparent" : "#eee";
                        apViewModel.review_EditableStatus = "disabled";
                        apViewModel.pageHeader = (apViewModel.canSubmitAP) ? "Manage Action Plan" : "View Action Plan";
                        break;
                    case "Ready For Review":
                        apViewModel.displayStatus = "Ready For Review";
                        apViewModel.displayColor = "orange";
                        apViewModel.AP_EditableStatus = "readonly";
                        apViewModel.AP_EditableColor = "#eee";
                        apViewModel.review_EditableStatus = (apViewModel.canReviewAP) ? "" : "disabled";
                        apViewModel.pageHeader = (apViewModel.canReviewAP) ? "Review Action Plan" : "View Action Plan";
                        break;
                    case "Rejected":
                        apViewModel.displayStatus = "Rejected";
                        apViewModel.displayColor = "red";
                        apViewModel.AP_EditableStatus = "readonly";
                        apViewModel.AP_EditableColor = "#eee";
                        apViewModel.review_EditableStatus = "disabled";
                        apViewModel.pageHeader = "View Action Plan";
                        break;
                    case "Rejected New":
                        apViewModel.displayStatus = "Rejected (New Plan Required)";
                        apViewModel.displayColor = "red";
                        apViewModel.AP_EditableStatus = (apViewModel.canSubmitAP) ? "" : "readonly";
                        apViewModel.AP_EditableColor = apViewModel.canSubmitAP ? "transparent" : "#eee";
                        apViewModel.review_EditableStatus = "disabled";
                        apViewModel.pageHeader = (apViewModel.canSubmitAP) ? "Rework Action Plan" : "View Action Plan";
                        break;
                    case "Approved":
                        apViewModel.displayStatus = "Approved";
                        apViewModel.displayColor = "green";
                        apViewModel.AP_EditableStatus = "readonly";
                        apViewModel.AP_EditableColor = "#eee";
                        apViewModel.review_EditableStatus = "disabled";
                        apViewModel.pageHeader = "View Action Plan";
                        break;
                    default:
                        apViewModel.displayStatus = apViewModel.currentAPStatus;
                        apViewModel.displayColor = "green";
                        apViewModel.AP_EditableStatus = "readonly";
                        apViewModel.AP_EditableColor = "#eee";
                        apViewModel.review_EditableStatus = "disabled";
                        apViewModel.pageHeader = "View Action Plan";
                        break;
                }
            }
            else if (apViewModel.bapmStatus == "Expired")
            {
                apViewModel.currentAPVersion = apViewModel.actionPlanList.First().apVersion;
                apViewModel.currentAPStatus = apViewModel.actionPlanList.First().apStatus;
                apViewModel.AP_EditableStatus = "readonly";
                apViewModel.AP_EditableColor = "#eee";
                apViewModel.review_EditableStatus = "disabled";
                apViewModel.displayStatus = "Expired";
                apViewModel.displayColor = "red";
                apViewModel.pageHeader = "View Action Plan";
            }

            if (!apViewModel.canAccessAP) { return View("NoAuth"); }
            return View(apViewModel);
        }

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =

        //POST: /ActionPlan/submitActionPlan
        [HttpPost]
        public string submitActionPlan(string raw_json)
        {
            string status = api.submitActionPlan(raw_json.Replace("\n", "\\n"));

            return returnResultMessage(status);
        }

        //POST: /ActionPlan/submitActionPlanReview
        [HttpPost]
        public string submitActionPlanReview(string raw_json)
        {
            string status = api.submitAPReview (raw_json.Replace("\n", "\\n"));

            return returnResultMessage(status);
        }

        //POST: /ActionPlan/saveActionPlanDetail
        [HttpPost]
        public string saveActionPlan(string raw_json)
        {
            
            string status = api.saveActionPlan(raw_json.Replace("\n","\\n"));

            return returnResultMessage(status);
        }

        //public ActionResult AP_Add(int? bapm_id, int? mtrc_period_val_id)
        //{
        //    // Testing: http://localhost:56551/ActionPlan/viewEdit/?bapm_id=3&mtrc_period_val_id=3422
        //    //    //Valid Action Plan Status values are:
        //    //    // [rz_bapm_status] = 'Not Started'  OR 'WIP' OR 'Ready For Review' OR 'Approved' OR 'Rejected'

        //    // The "id" received as a parameter is the Building Action Plan Metric id ('bapm_id'). We will pass that to the API to get the data
        //    int bapmId = bapm_id ?? 0;
        //    int mpvId = mtrc_period_val_id ?? 0;
        //    string productName = "Red Zone";

        //    //List of Action Plans starting with the current as first Item of the list
        //    List<ActionPlan> actionPlanList = new List<ActionPlan>();

        //    actionPlanList = dataParcer.getActionPlanList(productName, bapmId.ToString());

        //    //List of Reasons
        //    List<MPReason> mpReasonList = new List<MPReason>();

        //    mpReasonList = dataParcer.getAssignedMetricPeriodReasons(mpvId.ToString());

        //    //Add reasons to each action plan model
        //    foreach (ActionPlan actionPlan in actionPlanList)
        //    {
        //        actionPlan.reasonList = mpReasonList;
        //    }

        //    //actionPlanList.Add(dummyActionPlan(productName, bapmId, mpvId));

        //    actionPlanList = actionPlanList.OrderByDescending(x => x.apVersion).ToList();

        //    return View(actionPlanList);
        //}

        /////////////////////////////////////////////////////////////////
        // ==================== PRIVATE METHODS ==================== ////
        /////////////////////////////////////////////////////////////////

        private string returnResultMessage(string status)
        {
            if (status.ToLower().Contains("success"))
            {
                //Session["metricSaveMsg"] = "Data Saved Successfully.";
                return "Success";
            }
            else if (status.Contains("The remote server returned an error: (400) Bad Request."))
            {
                return "The remote server returned an error: (400) Bad Request.";
            }
            else
            {
                JObject res = JObject.Parse(status);
                try
                {
                    return res.GetValue("message").ToString();
                }
                catch
                {
                    return status;
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////
        // ==================== TEMPORARY HELPER FUNTIONS FOR TESTING ================================ ////
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        //private ActionPlan dummyActionPlan(string productName, int someBIP_id, int mpvId)
        //{
        //    //Valid Action Pan Status values are:
        //    // [rz_bapm_status] = 'Not Started'  OR 'WIP' OR 'Ready For Review' OR 'Approved' OR 'Rejected'
        //
        //    ActionPlan tempAP = new ActionPlan();

        //    //tempAP.apStatus = "Not Started";
        //    //tempAP.actionPlanAction = "I am planning on working harder.";
        //    //tempAP.reviewerComments = "";

        //    List<MPReason> mpReasonList = new List<MPReason>();
        //    mpReasonList = dataParcer.getAssignedMetricPeriodReasons(mpvId.ToString());

        //    tempAP.reasonList = mpReasonList;
        //    //tempAP.reasonList.Add(addDummyReason("Reason 1"));
        //    return tempAP;
        //}

        //private MPReason addDummyReason(string reasonText)
        //{
        //    MPReason dummyR = new MPReason();
        //    dummyR.reason_text = reasonText;
        //    dummyR.mpvr_Comment = "Some Comments for " + reasonText;
        //    return dummyR;
        //}
        // ==================== END OF TEMPORARY HELPER FUNTIONS FOR TESTING =====================================
    }

}