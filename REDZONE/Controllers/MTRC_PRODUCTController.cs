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
    public class MTRC_PRODUCTController : Controller
    {
        private DSC_MTRC_DEV_Entities db = new DSC_MTRC_DEV_Entities();

        // GET: MTRC_PRODUCT
        public ActionResult Index()
        {
            return View(db.MTRC_PRODUCT.ToList());
        }

        // GET: MTRC_PRODUCT/Details/5
        public ActionResult Details(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MTRC_PRODUCT mTRC_PRODUCT = db.MTRC_PRODUCT.Find(id);
            if (mTRC_PRODUCT == null)
            {
                return HttpNotFound();
            }
            return View(mTRC_PRODUCT);
        }

        // GET: MTRC_PRODUCT/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MTRC_PRODUCT/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "prod_id,prod_name")] MTRC_PRODUCT mTRC_PRODUCT)
        {
            if (ModelState.IsValid)
            {
                db.MTRC_PRODUCT.Add(mTRC_PRODUCT);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mTRC_PRODUCT);
        }

        // GET: MTRC_PRODUCT/Edit/5
        public ActionResult Edit(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MTRC_PRODUCT mTRC_PRODUCT = db.MTRC_PRODUCT.Find(id);
            if (mTRC_PRODUCT == null)
            {
                return HttpNotFound();
            }
            return View(mTRC_PRODUCT);
        }

        // POST: MTRC_PRODUCT/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "prod_id,prod_name")] MTRC_PRODUCT mTRC_PRODUCT)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mTRC_PRODUCT).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mTRC_PRODUCT);
        }

        // GET: MTRC_PRODUCT/Delete/5
        public ActionResult Delete(short? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MTRC_PRODUCT mTRC_PRODUCT = db.MTRC_PRODUCT.Find(id);
            if (mTRC_PRODUCT == null)
            {
                return HttpNotFound();
            }
            return View(mTRC_PRODUCT);
        }

        // POST: MTRC_PRODUCT/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(short id)
        {
            MTRC_PRODUCT mTRC_PRODUCT = db.MTRC_PRODUCT.Find(id);
            db.MTRC_PRODUCT.Remove(mTRC_PRODUCT);
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
