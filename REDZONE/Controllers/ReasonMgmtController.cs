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
    public class ReasonMgmtController : Controller
    {
        //**********> Get rid of this eventually <**********
        //private DSC_MTRC_DEV_Entities db = new DSC_MTRC_DEV_Entities();
        private DataRetrieval api = new DataRetrieval();
        private APIDataParcer dataParcer = new APIDataParcer();

        //============================================================================================================
        // GET: ReasonMgmt
        public ActionResult Index(int? id)
        {
            id = (id == null) ? 0 : id;    //Assign a Zero value to Id if it's null

            //Get List of Metric Periods
            List<MetricPeriod> MetricPeriodList = new List<MetricPeriod>();

            MetricPeriodList = dataParcer.getMetricPeriodList();

            ViewBag.metric_period_sel_list = new SelectList(MetricPeriodList, "mtrc_period_id", "mtrc_period_name", id);
            ViewBag.id = id;

            //Get List of Metric Period Reasons
            List<MPReason> MPReasonList = new List<MPReason>();

            if (id > 0)
            {
                MPReasonList = dataParcer.getMPReasonList(id.ToString());

                List<MPReason> list1 = MPReasonList.Where(x => x.reason_order != "").OrderBy(x => x.reason_order).ToList();

                List<MPReason> list2 = MPReasonList.Where(x => x.reason_order == "").OrderBy(x => x.reason_text).ToList();

                MPReasonList = list1.Concat(list2).ToList();
            }

            return View(MPReasonList);
        }

        //============================================================================================================
        // GET: /ReasonMgmt/_ReasonDetail
        public PartialViewResult _RsnDetail()
        {
            return PartialView();
        }
        // POST: /ReasonMgmt/_ReasonDetail
        [HttpPost]
        public PartialViewResult _RsnDetail(MPReason postedMPReason)
        {
            return PartialView(postedMPReason);
        }

        // GET:_ReasonReorder
        public ActionResult _RsnReorder()
        {
            return PartialView();
        }

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
        //POST: /ReasonMgmt/saveMPReason
        [HttpPost]
        public string saveMPReason(string raw_json)
        {
            string status = api.saveMPReason(raw_json);

            return returnResultMessage(status);
        }

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =

        //POST: /ReasonMgmt/saveMPReason
        [HttpPost]
        public string addMPReason(string raw_json)
        {
            string status;
            string mpr_Id = "0";     //Default Value is zero (Will be returned if any error is found)
            try {
                status = api.saveMPReason(raw_json);
                //Parse the results and return either a zero or the Id Number of the newly added Reason
                //(A Zero result  means that the Add action failed- Reason not added to the database or the parsing of data failed)
                JObject parsed_API_result = JObject.Parse(status);

                if (parsed_API_result.GetValue("result").ToString().Equals("Success"))
                {//The API Call to Add the Reason was successful
                    JArray apiNewReason = (JArray)parsed_API_result["savedData"];   //Technicaly the array should contain a single item
                    if (apiNewReason.HasValues)
                    {
                        var reasonAdded = apiNewReason[0];
                        mpr_Id = (string)reasonAdded["mpr_id"];
                        if (String.IsNullOrEmpty(mpr_Id)) { mpr_Id = "0"; } // If the mpr_Id could not be determined, default back to "0"
                    }
                    else { return "0"; }
                }
                else {
                    return "ERROR: " + parsed_API_result.GetValue("message").ToString();
                }
            }
            catch(Exception ex) {//Return Error Message
                return "ERROR: " + ex.Message;
            }
            return mpr_Id;
        }

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =

        
        //POST: /ReasonMgmt/updateMPReason
        [HttpPost]
        public string updateMPReason(string raw_json)
        {
            string status = api.updateMPReason(raw_json);

            return returnResultMessage(status);
        }

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
        //POST: /ReasonMgmt/removeMPReason
        [HttpPost]
        public string removeMPReason(string mpr_id)
        {
            string status = api.removeMPReason(mpr_id);

            return returnResultMessage(status);
        }

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
        //POST: /ReasonMgmt/isReasonCreatorAdmin
        [HttpPost]
        public string isReasonCreatorAdmin(string user_id)
        {
            return "true";
        }

        //= = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = = =
        //POST: /ReasonMgmt/reorderMPReasons
        [HttpPost]
        public string reorderMPReasons(string raw_json)
        {
            string status = api.reorderMPReasons(raw_json);

            return returnResultMessage(status);
        }


        //*************************************************
        //**************PRIVATE METHODS********************
        //*************************************************
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
    }
}