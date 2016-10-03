using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using REDZONE.Models;
using REDZONE.AppCode;

namespace REDZONE.Controllers
{
    public class ActionPlanController : Controller
    {
        private DataRetrieval api = new DataRetrieval();
        private APIDataParcer dataParcer = new APIDataParcer();

        // GET: ActionPlan
        public ActionResult viewEdit(int? bapm_id, int? mtrc_period_val_id)
        {
            // Testing: http://localhost:56551/ActionPlan/viewEdit/?bapm_id=3&mtrc_period_val_id=3422
            // The "id" receives as a parameter is the Building Action Plan id ('bap_id'). We will pass that to the API to get the data
            int bapmId = bapm_id ?? 0;
            int mpvId = mtrc_period_val_id ?? 0;
            string productName = "Red Zone";

            //List of Action Plans starting with the current as first Item of the list
            List<ActionPlan> actionPlanList = new List<ActionPlan>();

            actionPlanList = dataParcer.getActionPlanList(productName, bapmId.ToString());

            //List of Reasons
            List<MPReason> mpReasonList = new List<MPReason>();

            mpReasonList = dataParcer.getAssignedMetricPeriodReasons(mpvId.ToString());

            //Add reasons to each action plan model
            foreach (ActionPlan actionPlan in actionPlanList){
                actionPlan.reasonList = mpReasonList;
            }

            //actionPlanList.Add(dummyActionPlan(productName, bapmId, mpvId));

            actionPlanList = actionPlanList.OrderByDescending(x => x.apVersion).ToList();

            return View(actionPlanList);
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