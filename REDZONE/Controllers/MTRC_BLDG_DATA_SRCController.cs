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
    public class MTRC_BLDG_DATA_SRCController : Controller
    {
        private DSC_MTRC_DEV_Entities db = new DSC_MTRC_DEV_Entities();

        // GET: MTRC_BLDG_DATA_SRC
        public ActionResult Index()
        {
            var mTRC_BLDG_DATA_SRC = db.MTRC_BLDG_DATA_SRC.Include(m => m.DSC_MTRC_LC_BLDG).Include(m => m.MTRC_DATA_SRC).Include(m => m.MTRC_METRIC_PERIOD);
            return View(mTRC_BLDG_DATA_SRC.ToList());
        }

        // GET: MTRC_BLDG_DATA_SRC/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MTRC_BLDG_DATA_SRC mTRC_BLDG_DATA_SRC = db.MTRC_BLDG_DATA_SRC.Find(id);
            if (mTRC_BLDG_DATA_SRC == null)
            {
                return HttpNotFound();
            }
            return View(mTRC_BLDG_DATA_SRC);
        }

        // GET: MTRC_BLDG_DATA_SRC/Create
        public ActionResult Create()
        {
            ViewBag.dsc_lc_bldg_id = new SelectList(db.DSC_MTRC_LC_BLDG, "dsc_mtrc_lc_bldg_id", "dsc_mtrc_lc_bldg_name");
            ViewBag.data_src_id = new SelectList(db.MTRC_DATA_SRC, "data_src_id", "data_src_name");
            ViewBag.mtrc_period_id = new SelectList(db.MTRC_METRIC_PERIOD, "mtrc_period_id", "mtrc_period_name");
            return View();
        }

        // POST: MTRC_BLDG_DATA_SRC/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "bldg_data_src_id,data_src_id,dsc_lc_bldg_id,mtrc_period_id")] MTRC_BLDG_DATA_SRC mTRC_BLDG_DATA_SRC)
        {
            if (ModelState.IsValid)
            {
                db.MTRC_BLDG_DATA_SRC.Add(mTRC_BLDG_DATA_SRC);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.dsc_lc_bldg_id = new SelectList(db.DSC_MTRC_LC_BLDG, "dsc_mtrc_lc_bldg_id", "dsc_mtrc_lc_bldg_name", mTRC_BLDG_DATA_SRC.dsc_lc_bldg_id);
            ViewBag.data_src_id = new SelectList(db.MTRC_DATA_SRC, "data_src_id", "data_src_name", mTRC_BLDG_DATA_SRC.data_src_id);
            ViewBag.mtrc_period_id = new SelectList(db.MTRC_METRIC_PERIOD, "mtrc_period_id", "mtrc_period_name", mTRC_BLDG_DATA_SRC.mtrc_period_id);
            return View(mTRC_BLDG_DATA_SRC);
        }

        // GET: MTRC_BLDG_DATA_SRC/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MTRC_BLDG_DATA_SRC mTRC_BLDG_DATA_SRC = db.MTRC_BLDG_DATA_SRC.Find(id);
            if (mTRC_BLDG_DATA_SRC == null)
            {
                return HttpNotFound();
            }
            ViewBag.dsc_lc_bldg_id = new SelectList(db.DSC_MTRC_LC_BLDG, "dsc_mtrc_lc_bldg_id", "dsc_mtrc_lc_bldg_name", mTRC_BLDG_DATA_SRC.dsc_lc_bldg_id);
            ViewBag.data_src_id = new SelectList(db.MTRC_DATA_SRC, "data_src_id", "data_src_name", mTRC_BLDG_DATA_SRC.data_src_id);
            ViewBag.mtrc_period_id = new SelectList(db.MTRC_METRIC_PERIOD, "mtrc_period_id", "mtrc_period_name", mTRC_BLDG_DATA_SRC.mtrc_period_id);
            return View(mTRC_BLDG_DATA_SRC);
        }

        // POST: MTRC_BLDG_DATA_SRC/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "bldg_data_src_id,data_src_id,dsc_lc_bldg_id,mtrc_period_id")] MTRC_BLDG_DATA_SRC mTRC_BLDG_DATA_SRC)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mTRC_BLDG_DATA_SRC).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.dsc_lc_bldg_id = new SelectList(db.DSC_MTRC_LC_BLDG, "dsc_mtrc_lc_bldg_id", "dsc_mtrc_lc_bldg_name", mTRC_BLDG_DATA_SRC.dsc_lc_bldg_id);
            ViewBag.data_src_id = new SelectList(db.MTRC_DATA_SRC, "data_src_id", "data_src_name", mTRC_BLDG_DATA_SRC.data_src_id);
            ViewBag.mtrc_period_id = new SelectList(db.MTRC_METRIC_PERIOD, "mtrc_period_id", "mtrc_period_name", mTRC_BLDG_DATA_SRC.mtrc_period_id);
            return View(mTRC_BLDG_DATA_SRC);
        }

        // GET: MTRC_BLDG_DATA_SRC/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MTRC_BLDG_DATA_SRC mTRC_BLDG_DATA_SRC = db.MTRC_BLDG_DATA_SRC.Find(id);
            if (mTRC_BLDG_DATA_SRC == null)
            {
                return HttpNotFound();
            }
            return View(mTRC_BLDG_DATA_SRC);
        }

        // POST: MTRC_BLDG_DATA_SRC/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MTRC_BLDG_DATA_SRC mTRC_BLDG_DATA_SRC = db.MTRC_BLDG_DATA_SRC.Find(id);
            db.MTRC_BLDG_DATA_SRC.Remove(mTRC_BLDG_DATA_SRC);
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
