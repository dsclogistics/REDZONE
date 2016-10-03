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

        // GET: ActionPlan
        public ActionResult viewEdit(int? bapm_id, int? mtrc_period_val_id)
        {
            //Valid Action Pan Status values are:
            // [rz_bapm_status] = 'Not Started'  OR 'WIP' OR 'Ready For Review' OR 'Approved' OR 'Rejected'

            // Testing: http://localhost:56551/ActionPlan/viewEdit/?bapm_id=3&mtrc_period_val_id=3422
            //    //Valid Action Plan Status values are:
            //    // [rz_bapm_status] = 'Not Started'  OR 'WIP' OR 'Ready For Review' OR 'Approved' OR 'Rejected'

            // The "id" received as a parameter is the Building Action Plan Metric id ('bapm_id'). We will pass that to the API to get the data
            int bapmId = bapm_id ?? 0;
            int mpvId = mtrc_period_val_id ?? 0;
            string productName = "Red Zone";
            ViewBag.bapmId = bapmId;

            //List of Action Plans starting with the current as first item of the list
            List<ActionPlan> actionPlanList = new List<ActionPlan>();
            actionPlanList = dataParcer.getActionPlanList(productName, bapmId.ToString());
            actionPlanList = actionPlanList.OrderByDescending(x => x.apVersion).ToList();

            //List of Reasons
            List<MPReason> mpReasonList = new List<MPReason>();
            mpReasonList = dataParcer.getAssignedMetricPeriodReasons(mpvId.ToString());

            //Model logic
            if (actionPlanList.Count > 0)
            {
                //Add reasons to each action plan model
                foreach (ActionPlan actionPlan in actionPlanList)
                {
                    actionPlan.reasonList = mpReasonList;
                }

                if (actionPlanList.First().apStatus == "Rejected")
                {
                    //When action plan list is NOT empty, but most recent version status is Rejected, pass in "empty" model
                    ActionPlan newActionPlan = new ActionPlan();
                    newActionPlan.reasonList = mpReasonList;
                    newActionPlan.bapm_id = bapmId.ToString();
                    newActionPlan.apd_id = "";
                    newActionPlan.apVersion = (string)actionPlanList.First().apVersion + 1;
                    newActionPlan.apStatus = "WIP";
                    newActionPlan.actionPlanAction = "";
                    newActionPlan.reviewerComments = "";
                    actionPlanList.Insert(0, newActionPlan);
                }
                else
                {
                    //When action plan list is NOT empty, and most recent version status is 'WIP', 'Ready for Review', or 'Approved', pass model as is.
                }

            }
            else
            {
                //When action plan list is empty, pass in "empty" model
                ActionPlan newActionPlan = new ActionPlan();
                newActionPlan.reasonList = mpReasonList;
                newActionPlan.bapm_id = bapmId.ToString();
                newActionPlan.apd_id = "";
                newActionPlan.apVersion = "1";
                newActionPlan.apStatus = "WIP";
                newActionPlan.actionPlanAction = "";
                newActionPlan.reviewerComments = "";
                actionPlanList.Add(newActionPlan);
            }

            return View(actionPlanList);
        }


        //= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =

        //POST: /ActionPlan/submitActionPlanDetail
        [HttpPost]
        public string submitActionPlan(string raw_json)
        {
            string status = api.submitActionPlan(raw_json);

            return returnResultMessage(status);
        }


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