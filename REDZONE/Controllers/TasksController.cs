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
    public class TasksController : Controller
    {
        private DataRetrieval api = new DataRetrieval();
        private APIDataParcer dataParcer = new APIDataParcer();

        // GET: Tasks
        public ActionResult Index(string display)
        {
            RZTasksViewModel tasksViewModel = new RZTasksViewModel();
            RZTaskDetailList tasksDetailList = new RZTaskDetailList();
            dscUser actionPlanUser = new dscUser(User.Identity.Name);


            tasksDetailList = dataParcer.getUserTaskDetails(actionPlanUser.dbUserId);

            //Convert Action Plan submitter task list to nested list for view model
            List<RzApTaskBldg> apSubmitList = (
                from a in tasksDetailList.apSubmitTaskList
                group a by new { a.bldg_name } into grouped
                select new RzApTaskBldg
                {
                    bldgName = grouped.Key.bldg_name,
                    periodList = (from b in grouped
                                  group b by new { b.period_name } into grouped2
                                  select new RzApTaskBldgPeriod
                                  {
                                      periodName = grouped2.Key.period_name,
                                      mtrcList = (from c in grouped2
                                                  select new RzApTaskBldgPeriodMtrc
                                                  {
                                                      mtrc_prod_display_text = c.mtrc_prod_display_text,
                                                      status = c.status,
                                                      action = dataParcer.getMPV_NextAction(c.status, c.mtrc_period_id.ToString(), c.bldg_id.ToString(), actionPlanUser.SSO),
                                                      mtrc_period_id = c.mtrc_period_id,
                                                      bldg_id = c.bldg_id,
                                                      rz_bapm_id = c.rz_bapm_id,
                                                      month = c.month,
                                                      year = c.year
                                                  }).OrderBy(x => x.mtrc_prod_display_text).ToList()
                                  }).OrderByDescending(x => Int32.Parse(x.mtrcList.First().year)).ThenByDescending(x => Int32.Parse(x.mtrcList.First().month)).ToList()
                }).OrderBy(x => x.bldgName).ToList();

            tasksViewModel.apSubmitTaskList = apSubmitList;

            //Convert Action Plan reviewer task list to nested list for view model
            List<RzApTaskMtrc> apReviewList = (
                from a in tasksDetailList.apReviewTaskList
                group a by new { a.mtrc_prod_display_text } into grouped
                select new RzApTaskMtrc
                {
                    mtrc_prod_display_text = grouped.Key.mtrc_prod_display_text,
                    periodList = (from b in grouped
                                  group b by new { b.period_name } into grouped2
                                  select new RzApTaskMtrcPeriod
                                  {
                                      periodName = grouped2.Key.period_name,
                                      bldgList = (from c in grouped2
                                                  select new RzApTaskMtrcPeriodBldg
                                                  {
                                                      bldgName = c.bldg_name,
                                                      status = c.status,
                                                      action = dataParcer.getMPV_NextAction(c.status, c.mtrc_period_id.ToString(), c.bldg_id.ToString(), actionPlanUser.SSO),
                                                      mtrc_period_id = c.mtrc_period_id,
                                                      bldg_id = c.bldg_id,
                                                      rz_bapm_id = c.rz_bapm_id,
                                                      month = c.month,
                                                      year = c.year
                                                  }).OrderBy(x => x.bldgName).ToList()
                                  }).OrderByDescending(x => Int32.Parse(x.bldgList.First().year)).ThenByDescending(x => Int32.Parse(x.bldgList.First().month)).ToList()
                }).OrderBy(x => x.mtrc_prod_display_text).ToList();

            tasksViewModel.apReviewTaskList = apReviewList;

            //Convert Action Plan metric task list to nested list for view model
            List<RzMtrcTaskPeriod> mtrcCollectList = (
                from a in tasksDetailList.mtrcTaskList
                group a by new { a.period_name } into grouped
                select new RzMtrcTaskPeriod
                {
                    periodName = grouped.Key.period_name,
                    mtrcList = (from b in grouped
                                select new RzMtrcTaskPeriodMtrc
                                {
                                    mtrc_prod_display_text = b.mtrc_prod_display_text,
                                    status = b.status,
                                    mtrc_id = b.mtrc_id,
                                    month = b.month,
                                    year = b.year,
                                    month_name = b.month_name
                                }).OrderBy(x => x.mtrc_prod_display_text).ToList()
                }).OrderByDescending(x => Int32.Parse(x.mtrcList.First().year)).ThenByDescending(x => Int32.Parse(x.mtrcList.First().month)).ToList();

            tasksViewModel.mtrcTaskList = mtrcCollectList;





            ViewBag.display = display;

            return View(tasksViewModel);
        }

        [HttpPost] [OutputCache (Duration=10)]
        public PartialViewResult _myTaskSummary() {

            dscUser actionPlanUser = new dscUser(User.Identity.Name);
            RZTaskDetailList tasksDetailList = dataParcer.getUserTaskDetails(actionPlanUser.dbUserId);

            if (!actionPlanUser.hasRole("RZ_AP_SUBMITTER")) {
                tasksDetailList.apSubmitTaskList.Clear();
            }
            if (!actionPlanUser.hasRole("RZ_AP_REVIEWER"))
            {
                tasksDetailList.apReviewTaskList.Clear();
            }
            if (!actionPlanUser.hasRole("MTRC_COLLECTOR"))
            {
                tasksDetailList.mtrcTaskList.Clear();
            }

            return PartialView(tasksDetailList );
        }

        [HttpPost]
        public PartialViewResult _quickReview(string productname, int? rz_bapm_id)
        {
            dscUser actionPlanUser = new dscUser(User.Identity.Name);

            QuickActionPlan quickActionPlan = new QuickActionPlan();

            int rzBapmId = rz_bapm_id ?? 0;
            ViewBag.authorized = false;

            if(rzBapmId > 0)
            {
                quickActionPlan = dataParcer.getActionPlanById(productname, rzBapmId.ToString());
                if (actionPlanUser.hasRole("RZ_AP_REVIEWER") && actionPlanUser.hasReviewerMetric(quickActionPlan.mtrc_period_id.ToString()))
                {
                    ViewBag.authorized = true;
                }
                else if (actionPlanUser.hasRole("RZ_ADMIN"))
                {
                    ViewBag.authorized = true;
                }
            }
                
            return PartialView(quickActionPlan);
        }


        public ActionResult TeamActivities()
        {
            // FD Nov16,2016
            //CODE TO MODIFY!!!!!!! - THIS WAS COPIED FROM "INDEX" action, so we can replace with new code!!!!!!!!!
            TeamActivitiesViewModel teamActivitiesViewModel = new TeamActivitiesViewModel();
            List<TeamActivity> teamActivityList = new List<TeamActivity>();
            dscUser actionPlanUser = new dscUser(User.Identity.Name);

            teamActivityList = dataParcer.getUserTeamActivities(actionPlanUser.dbUserId);

            //Convert Action Plan submitter task list to nested list for view model
            //List<TeamActivityPeriod> periodList = (
            //    from a in teamActivityList
            //    group a by new { a.month, a.monthName, a.year, a.periodName } into grouped
            //    select new TeamActivityPeriod
            //    {
            //        month = grouped.Key.month,
            //        monthName = grouped.Key.monthName,
            //        year = grouped.Key.year,
            //        periodName = grouped.Key.periodName,
            //        periodActivityList = (from b in grouped
            //                              select new TeamActivity
            //                              {
            //                                  month = b.month,
            //                                  monthName = b.monthName,
            //                                  year = b.year,
            //                                  periodName = b.periodName,
            //                                  bldgName = b.bldgName,
            //                                  bldgId = b.bldgId,
            //                                  mtrcName = b.mtrcName,
            //                                  mtrcPeriodId = b.mtrcPeriodId,
            //                                  rzBapmId = b.rzBapmId,
            //                                  rzBapmStatus = b.rzBapmStatus
            //                              }).OrderBy(x => x.bldgName).ThenBy(x => x.mtrcName).ToList()
            //    }).OrderByDescending(x => Int32.Parse(x.year)).ThenByDescending(x => Int32.Parse(x.month)).ToList();

            List<TeamActivityPeriod> periodList = (
                from a in teamActivityList
                group a by new { a.month, a.monthName, a.year, a.periodName } into grouped
                select new TeamActivityPeriod
                {
                    month = grouped.Key.month,
                    monthName = grouped.Key.monthName,
                    year = grouped.Key.year,
                    periodName = grouped.Key.periodName,
                    periodActivityList = (from b in grouped
                                          select b).OrderBy(x => x.bldgName).ThenBy(x => x.mtrcName).ToList()
                }).OrderByDescending(x => Int32.Parse(x.year)).ThenByDescending(x => Int32.Parse(x.month)).ToList();

            teamActivitiesViewModel.periodList = periodList;

            return View(teamActivitiesViewModel);
        }


        // ========================== Auto Generated ==========================
#region AutoGenerated
        // GET: Tasks/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Tasks/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Tasks/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Tasks/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Tasks/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Tasks/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Tasks/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

#endregion 

    }
}
