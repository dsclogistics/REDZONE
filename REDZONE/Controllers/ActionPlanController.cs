using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using REDZONE.Models;

namespace REDZONE.Controllers
{
    public class ActionPlanController : Controller
    {
        // GET: ActionPlan
        public ActionResult viewEdit()
        {
            //List of Action Plans sctarting with the current as first Item of the list
            List<actionPlan> actionPlanList = new List<actionPlan>();

            actionPlanList.Add(dummyActionPlan());

            return View(actionPlanList);
        }



        ///////////////////////////////////////////////////////////////////////////////////////////////////
        // ==================== TEMPORARY HELPER FUNTIONS FOR TESTING ================================ ////
        ///////////////////////////////////////////////////////////////////////////////////////////////////
        private actionPlan dummyActionPlan()
        {
            //Valid Action Pan Status values are:
            // [rz_bapm_status] = 'Rejected' OR 'Approved' OR 'Ready For Review' OR 'WIP' OR 'Not Started'
            actionPlan tempAP = new actionPlan();
            tempAP.apStatus = "Not Started";
            tempAP.actionPlanAction = "I am planning on working harder.";
            tempAP.reviewerComments = "";

            tempAP.reasonList.Add(addDummyReason("Reason 1"));
            tempAP.reasonList.Add(addDummyReason("Reason 2"));
            tempAP.reasonList.Add(addDummyReason("Reason 3"));
            tempAP.reasonList.Add(addDummyReason("Reason 4"));
            tempAP.reasonList.Add(addDummyReason("Reason 5"));
            return tempAP;
        }

        private MPReason addDummyReason(string reasonText)
        {
            MPReason dummyR = new MPReason();
            dummyR.reason_text = reasonText;
            dummyR.mpvr_Comment = "Some Comments for " + reasonText;
            return dummyR;
        }
        // ==================== END OF TEMPORARY HELPER FUNTIONS FOR TESTING =====================================
    }

    // ---------------- ACTION PLAN CLASS FOR TESTING---- Move to Models Folder once implemented for real --------------
    public class actionPlan
    {
        public List<MPReason> reasonList { get; set; }
        public string apStatus { get; set; }
        public string actionPlanAction { get; set; }
        public string reviewerComments { get; set; }

        public actionPlan() {reasonList = new List<MPReason>(); }

    }

}