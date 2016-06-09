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
    public class DSC_MTRC_LC_BLDGController : Controller
    {
        private DSC_MTRC_DEV_Entities db = new DSC_MTRC_DEV_Entities();

        // GET: DSC_MTRC_LC_BLDG
        public ActionResult Index()
        {
            var dSC_MTRC_LC_BLDG = db.DSC_MTRC_LC_BLDG.Include(d => d.DSC_LC);
            return View(dSC_MTRC_LC_BLDG.ToList());
        }

        // GET: DSC_MTRC_LC_BLDG/Details/5
        public ActionResult Details(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DSC_MTRC_LC_BLDG dSC_MTRC_LC_BLDG = db.DSC_MTRC_LC_BLDG.Find(id);
            if (dSC_MTRC_LC_BLDG == null)
            {
                return HttpNotFound();
            }
            return View(dSC_MTRC_LC_BLDG);
        }

        // GET: DSC_MTRC_LC_BLDG/Create
        public ActionResult Create()
        {
            ViewBag.dsc_lc_id = new SelectList(db.DSC_LC, "dsc_lc_id", "dsc_lc_name");
            return View();
        }

        // POST: DSC_MTRC_LC_BLDG/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "dsc_mtrc_lc_bldg_id,dsc_lc_id,dsc_mtrc_lc_bldg_name,dsc_mtrc_lc_bldg_code,dsc_mtrc_lc_bldg_eff_start_dt,dsc_mtrc_lc_bldg_eff_end_dt")] DSC_MTRC_LC_BLDG dSC_MTRC_LC_BLDG)
        {
            if (ModelState.IsValid)
            {
                db.DSC_MTRC_LC_BLDG.Add(dSC_MTRC_LC_BLDG);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.dsc_lc_id = new SelectList(db.DSC_LC, "dsc_lc_id", "dsc_lc_name", dSC_MTRC_LC_BLDG.dsc_lc_id);
            return View(dSC_MTRC_LC_BLDG);
        }

        // GET: DSC_MTRC_LC_BLDG/Edit/5
        public ActionResult Edit(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DSC_MTRC_LC_BLDG dSC_MTRC_LC_BLDG = db.DSC_MTRC_LC_BLDG.Find(id);
            if (dSC_MTRC_LC_BLDG == null)
            {
                return HttpNotFound();
            }
            ViewBag.dsc_lc_id = new SelectList(db.DSC_LC, "dsc_lc_id", "dsc_lc_name", dSC_MTRC_LC_BLDG.dsc_lc_id);
            return View(dSC_MTRC_LC_BLDG);
        }

        // POST: DSC_MTRC_LC_BLDG/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "dsc_mtrc_lc_bldg_id,dsc_lc_id,dsc_mtrc_lc_bldg_name,dsc_mtrc_lc_bldg_code,dsc_mtrc_lc_bldg_eff_start_dt,dsc_mtrc_lc_bldg_eff_end_dt")] DSC_MTRC_LC_BLDG dSC_MTRC_LC_BLDG)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dSC_MTRC_LC_BLDG).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.dsc_lc_id = new SelectList(db.DSC_LC, "dsc_lc_id", "dsc_lc_name", dSC_MTRC_LC_BLDG.dsc_lc_id);
            return View(dSC_MTRC_LC_BLDG);
        }

        // GET: DSC_MTRC_LC_BLDG/Delete/5
        public ActionResult Delete(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DSC_MTRC_LC_BLDG dSC_MTRC_LC_BLDG = db.DSC_MTRC_LC_BLDG.Find(id);
            if (dSC_MTRC_LC_BLDG == null)
            {
                return HttpNotFound();
            }
            return View(dSC_MTRC_LC_BLDG);
        }

        // POST: DSC_MTRC_LC_BLDG/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            DSC_MTRC_LC_BLDG dSC_MTRC_LC_BLDG = db.DSC_MTRC_LC_BLDG.Find(id);
            db.DSC_MTRC_LC_BLDG.Remove(dSC_MTRC_LC_BLDG);
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
