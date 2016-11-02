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
            TasksViewModel tasksViewModel = new TasksViewModel();
            dscUser actionPlanUser = new dscUser(User.Identity.Name);

            tasksViewModel = dataParcer.getUserTaskDetails(actionPlanUser.dbUserId);

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
