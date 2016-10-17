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
        //Unrelated test changes merge

        private DataRetrieval api = new DataRetrieval();
        private APIDataParcer dataParcer = new APIDataParcer();

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

            ViewBag.curUserRole = REDZONE.AppCode.Util.getUserRoles(User.Identity.Name);
            ViewBag.bapmId = bapmId;

            //-------------------------------
            //Populate Action Plan View Model
            //-------------------------------
            //List of Action Plans starting with the current as first item of the list
            //------------------------------------------------------------------------
            ActionPlanViewModel apViewModel = new ActionPlanViewModel();

            apViewModel = dataParcer.getActionPlanList(productName, bapmId.ToString());

            apViewModel.actionPlanList = apViewModel.actionPlanList.OrderByDescending(x => Int32.Parse(x.apVersion)).ToList();

            //Add List of Reasons 
            //-------------------
            int mpv_id = 0;
            Int32.TryParse(apViewModel.mpv_id, out mpv_id);
            List<MPReason> mpReasonList = new List<MPReason>();

            mpReasonList = dataParcer.getAssignedMetricPeriodReasons(mpv_id.ToString());

            apViewModel.reasonList = mpReasonList;

            //Add List of Recent Action Plans
            //-------------------------------
            List<PriorActionPlan> recentAPList = new List<PriorActionPlan>();
            string mpId = intMpId.ToString();
            string bldgId = intBldgId.ToString();
            string begmonth = DateTime.Now.Month.ToString();
            string begyear = (DateTime.Now.Year - 1).ToString();
            string endmonth = begmonth;
            string endyear = DateTime.Now.Year.ToString();

            recentAPList = dataParcer.getPriorActionPlanList(productName, mpId, bldgId, begmonth, begyear, endmonth, endyear);

            foreach(PriorActionPlan priorAP in recentAPList)
            {
                if(bapmId.ToString() == priorAP.bapm_id)
                {
                    //ViewBag.metricDate = priorAP.priorAPMonth + " " + priorAP.priorAPYear;
                    recentAPList.Remove(priorAP);
                    break;
                }
            }

            apViewModel.priorActionPlanList = recentAPList;


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
                }
                else
                {
                    //When action plan list is NOT empty, and most recent version status is 'WIP', 'Ready for Review', or 'Approved', pass model as is.
                }

            }
            else if (apViewModel.actionPlanList.Count == 0 && apViewModel.bapmStatus == "Not Started")
            {
                //When action plan list is empty, pass in "empty" model
                ActionPlan newActionPlan = new ActionPlan();
                newActionPlan.apd_id = "";
                newActionPlan.apVersion = "1";
                newActionPlan.apStatus = "Not Started";
                newActionPlan.actionPlanAction = "";
                newActionPlan.reviewerComments = "";
                apViewModel.actionPlanList.Add(newActionPlan);
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

            return View(apViewModel);
        }



        // GET: ActionPlan
        //public ActionResult viewEdit2(int? bapm_id, int? mtrc_period_val_id)
        //{
        //    //Valid Action Pan Status values are:
        //    // [rz_bapm_status] = 'Not Started'  OR 'WIP' OR 'Ready For Review' OR 'Approved' OR 'Rejected'

        //    // Testing: http://localhost:56551/ActionPlan/viewEdit/?bapm_id=3&mtrc_period_val_id=3422

        //    // The "id" received as a parameter is the Building Action Plan Metric id ('bapm_id'). We will pass that to the API to get the data
        //    int bapmId = bapm_id ?? 0;
        //    int mpvId = mtrc_period_val_id ?? 0;
        //    string productName = "Red Zone";

        //    ViewBag.curUserRole = REDZONE.AppCode.Util.getUserRoles(User.Identity.Name);
        //    ViewBag.bapmId = bapmId;

        //    //-------------------------------
        //    //Populate Action Plan View Model
        //    //-------------------------------
        //    //List of Action Plans starting with the current as first item of the list
        //    ActionPlanViewModel apViewModel = new ActionPlanViewModel();
        //    apViewModel = dataParcer.getActionPlanList(productName, bapmId.ToString());
        //    apViewModel.actionPlanList = apViewModel.actionPlanList.OrderByDescending(x => Int32.Parse(x.apVersion)).ToList();

        //    //Add List of Reasons 
        //    List<MPReason> mpReasonList = new List<MPReason>();
        //    mpReasonList = dataParcer.getAssignedMetricPeriodReasons(mpvId.ToString());
        //    apViewModel.reasonList = mpReasonList;


        //    //-----------
        //    //Model logic
        //    //-----------
        //    if (apViewModel.actionPlanList.Count > 0)
        //    {
        //        if (apViewModel.actionPlanList.First().apStatus == "Rejected")
        //        {
        //            //When action plan list is NOT empty, but most recent version status is Rejected, pass in "empty" model
        //            ActionPlan newActionPlan = new ActionPlan();
        //            newActionPlan.apd_id = "";
        //            newActionPlan.apVersion = (Int32.Parse(apViewModel.actionPlanList.First().apVersion) + 1).ToString();
        //            newActionPlan.apStatus = "Rejected New";
        //            newActionPlan.actionPlanAction = "";
        //            newActionPlan.reviewerComments = "";
        //            apViewModel.actionPlanList.Insert(0, newActionPlan);
        //        }
        //        else
        //        {
        //            //When action plan list is NOT empty, and most recent version status is 'WIP', 'Ready for Review', or 'Approved', pass model as is.
        //        }

        //    }
        //    else if (apViewModel.actionPlanList.Count == 0 && apViewModel.bapmStatus == "Not Started")
        //    {
        //        //When action plan list is empty, pass in "empty" model
        //        ActionPlan newActionPlan = new ActionPlan();
        //        newActionPlan.apd_id = "";
        //        newActionPlan.apVersion = "1";
        //        newActionPlan.apStatus = "Not Started";
        //        newActionPlan.actionPlanAction = "";
        //        newActionPlan.reviewerComments = "";
        //        apViewModel.actionPlanList.Add(newActionPlan);
        //    }
        //    else
        //    {
        //        //When action plan list is empty, pass in "empty" model
        //        ActionPlan newActionPlan = new ActionPlan();
        //        newActionPlan.apd_id = "";
        //        newActionPlan.apVersion = "1";
        //        newActionPlan.apStatus = "";
        //        newActionPlan.actionPlanAction = "";
        //        newActionPlan.reviewerComments = "";
        //        apViewModel.actionPlanList.Add(newActionPlan);
        //    }

        //    return View(apViewModel);
        //}

        [HttpPost]
        // POST: _priorActionPlans
        public ActionResult _priorActionPlans(string productname, string mtrc_period_id, string dsc_mtrc_lc_bldg_id, string begmonth, string begyear, string endmonth, string endyear)
        {
            List<PriorActionPlan> priorAPList = new List<PriorActionPlan>();

            priorAPList = dataParcer.getPriorActionPlanList(productname, mtrc_period_id, dsc_mtrc_lc_bldg_id, begmonth, begyear, endmonth, endyear);

            return PartialView(priorAPList);
        }
        //= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =

            //POST: /ActionPlan/submitActionPlan
        [HttpPost]
        public string submitActionPlan(string raw_json)
        {
            string status = api.submitActionPlan(raw_json);

            return returnResultMessage(status);
        }

        //POST: /ActionPlan/submitActionPlanReview
        [HttpPost]
        public string submitActionPlanReview(string raw_json)
        {
            string status = api.submitAPReview (raw_json);

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