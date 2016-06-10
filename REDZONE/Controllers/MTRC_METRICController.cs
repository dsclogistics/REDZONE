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
    public class MTRC_METRICController : Controller
    {
        private DSC_MTRC_DEV_Entities db = new DSC_MTRC_DEV_Entities();

        // GET: MTRC_METRIC
        public ActionResult Index()
        {
            var mTRC_METRIC = db.MTRC_METRIC.Include(m => m.MTRC_DATA_TYPE);
            return View(mTRC_METRIC.ToList());
        }

        // GET: MTRC_METRIC/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MTRC_METRIC mTRC_METRIC = db.MTRC_METRIC.Find(id);
            if (mTRC_METRIC == null)
            {
                return HttpNotFound();
            }
            return View(mTRC_METRIC);
        }

        // GET: MTRC_METRIC/Create
        public ActionResult Create()
        {
            ViewBag.data_type_id = new SelectList(db.MTRC_DATA_TYPE, "data_type_id", "data_type_name");
            return View();
        }

        // POST: MTRC_METRIC/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "mtrc_id,data_type_id,mtrc_name,mtrc_desc,mtrc_eff_start_dt,mtrc_eff_end_dt,mtrc_min_val,mtrc_max_val,mtrc_max_dec_places,mtrc_max_str_size,mtrc_na_allow_yn")] MTRC_METRIC mTRC_METRIC)
        {
            if (ModelState.IsValid)
            {
                db.MTRC_METRIC.Add(mTRC_METRIC);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.data_type_id = new SelectList(db.MTRC_DATA_TYPE, "data_type_id", "data_type_name", mTRC_METRIC.data_type_id);
            return View(mTRC_METRIC);
        }

        // GET: MTRC_METRIC/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MTRC_METRIC mTRC_METRIC = db.MTRC_METRIC.Find(id);
            if (mTRC_METRIC == null)
            {
                return HttpNotFound();
            }
            ViewBag.data_type_id = new SelectList(db.MTRC_DATA_TYPE, "data_type_id", "data_type_name", mTRC_METRIC.data_type_id);
            return View(mTRC_METRIC);
        }

        // POST: MTRC_METRIC/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "mtrc_id,data_type_id,mtrc_name,mtrc_desc,mtrc_eff_start_dt,mtrc_eff_end_dt,mtrc_min_val,mtrc_max_val,mtrc_max_dec_places,mtrc_max_str_size,mtrc_na_allow_yn")] MTRC_METRIC mTRC_METRIC)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mTRC_METRIC).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.data_type_id = new SelectList(db.MTRC_DATA_TYPE, "data_type_id", "data_type_name", mTRC_METRIC.data_type_id);
            return View(mTRC_METRIC);
        }

        // GET: MTRC_METRIC/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MTRC_METRIC mTRC_METRIC = db.MTRC_METRIC.Find(id);
            if (mTRC_METRIC == null)
            {
                return HttpNotFound();
            }
            return View(mTRC_METRIC);
        }

        // POST: MTRC_METRIC/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MTRC_METRIC mTRC_METRIC = db.MTRC_METRIC.Find(id);
            db.MTRC_METRIC.Remove(mTRC_METRIC);
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
