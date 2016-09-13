using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using REDZONE.Models;
using REDZONE.AppCode;

namespace REDZONE.Controllers
{
    public class ReasonMgmtController : Controller
    {
        //**********> Get rid of this eventually <**********
        private DSC_MTRC_DEV_Entities db = new DSC_MTRC_DEV_Entities();

        //============================================================================================================
        // GET: ReasonMgmt
        public ActionResult Index(int? id)
        {
            id = (id == null) ? 0 : id;    //Assign a Zero value to Id if it's null

            //Get 
            List<MetricPeriod> MetricPeriodList = new List<MetricPeriod>();
            APIDataParcer dataParcer = new APIDataParcer();

            MetricPeriodList = dataParcer.getMetricPeriodList();

            ViewBag.metric_period_sel_list = new SelectList(MetricPeriodList, "mtrc_period_id", "mtrc_period_name", id);
            ViewBag.id = id;

            List<MPReason> MPReasonList = new List<MPReason>();
            dataParcer = new APIDataParcer();

            if (id > 0)
            {
                MPReasonList = dataParcer.getMPReasonList(id.ToString());
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

    }
}