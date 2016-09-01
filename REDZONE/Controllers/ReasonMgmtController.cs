using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using REDZONE.Models;

namespace REDZONE.Controllers
{
    public class ReasonMgmtController : Controller
    {
        //**********> Get rid of this eventually <**********
        private DSC_MTRC_DEV_Entities db = new DSC_MTRC_DEV_Entities();

        // GET: ReasonMgmt
        public ActionResult Index(int? id)
        {
            id = (id == null) ? 0 : id;    //Assign a Zero value to Id if it's null

            //**********> Need API to get list of Metric Periods <**********
            ViewBag.metric_period_sel_list = new SelectList(db.MTRC_METRIC_PERIOD, "mtrc_period_id", "mtrc_period_name", id);
            ViewBag.id = id;

            return View();
        }

        // GET:_ReasonList
        public ActionResult _RsnList()
        {
            return PartialView();
        }

        // GET:_ReasonDetail
        public ActionResult _RsnDetail()
        {
            return PartialView();
        }

        // GET:_ReasonReorder
        public ActionResult _RsnReorder()
        {
            return PartialView();
        }

    }
}