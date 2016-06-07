using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using REDZONE.Models;

namespace REDZONE.Controllers
{
[AllowAnonymous]
    public class DSC_LCController : Controller
    {
        private DSC_OBS_DEV_Server db = new DSC_OBS_DEV_Server();

        // GET: DSC_LC
        public ActionResult Index()
        {
            return View(db.DSC_LC.ToList());
        }

        // GET: DSC_LC/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DSC_LC dSC_LC = db.DSC_LC.Find(id);
            if (dSC_LC == null)
            {
                return HttpNotFound();
            }
            return View(dSC_LC);
        }

        // GET: DSC_LC/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DSC_LC/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "dsc_lc_id,dsc_lc_name,dsc_lc_code,dsc_lc_timezone,dsc_lc_eff_end_date")] DSC_LC dSC_LC)
        {
            if (ModelState.IsValid)
            {
                db.DSC_LC.Add(dSC_LC);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dSC_LC);
        }

        // GET: DSC_LC/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DSC_LC dSC_LC = db.DSC_LC.Find(id);
            if (dSC_LC == null)
            {
                return HttpNotFound();
            }
            return View(dSC_LC);
        }

        // POST: DSC_LC/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "dsc_lc_id,dsc_lc_name,dsc_lc_code,dsc_lc_timezone,dsc_lc_eff_end_date")] DSC_LC dSC_LC)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dSC_LC).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dSC_LC);
        }

        // GET: DSC_LC/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DSC_LC dSC_LC = db.DSC_LC.Find(id);
            if (dSC_LC == null)
            {
                return HttpNotFound();
            }
            return View(dSC_LC);
        }

        // POST: DSC_LC/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DSC_LC dSC_LC = db.DSC_LC.Find(id);
            db.DSC_LC.Remove(dSC_LC);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
