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

            if (actionPlanUser.dbUserId == null)
            {

            }
            else
            {
                tasksDetailList = dataParcer.getUserTaskDetails(actionPlanUser.dbUserId);

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
                                                          mtrc_period_id = c.mtrc_period_id,
                                                          bldg_id = c.bldg_id,
                                                          rz_bapm_id = c.rz_bapm_id
                                                      }).ToList()
                                      }).ToList()
                    }).ToList();

                tasksViewModel.apSubmitTaskList = apSubmitList;
            }

            



            //tasksViewModel.apSubmitTaskList = tasksViewModel.apSubmitTaskList.OrderBy(x => x.bldg_name)
            //                                                                 .ThenBy(x => x.year)
            //                                                                 .ThenBy(x => x.month)
            //                                                                 .ThenBy(x => x.mtrc_prod_display_text).ToList();

            //tasksViewModel.apReviewTaskList = tasksViewModel.apReviewTaskList.OrderBy(x => x.bldg_name)
            //                                                     .ThenBy(x => x.year)
            //                                                     .ThenBy(x => x.month)
            //                                                     .ThenBy(x => x.mtrc_prod_display_text).ToList();

            //tasksViewModel.mtrcTaskList = tasksViewModel.mtrcTaskList.OrderBy(x => x.year)
            //                                                     .ThenBy(x => x.month)
            //                                                     .ThenBy(x => x.mtrc_prod_display_text).ToList();

            ViewBag.display = display;

            return View(tasksViewModel);
        }









        // ========================== Auto Generated ==========================
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
    }
}
