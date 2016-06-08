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
    public class DSC_CUST_ACCTController : Controller
    {
        private DSC_MTRC_DEV_Entities db = new DSC_MTRC_DEV_Entities();

        // GET: DSC_CUST_ACCT
        public ActionResult Index()
        {
            return View(db.DSC_CUST_ACCT.ToList());
        }

        // GET: DSC_CUST_ACCT/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DSC_CUST_ACCT dSC_CUST_ACCT = db.DSC_CUST_ACCT.Find(id);
            if (dSC_CUST_ACCT == null)
            {
                return HttpNotFound();
            }
            return View(dSC_CUST_ACCT);
        }

        // GET: DSC_CUST_ACCT/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DSC_CUST_ACCT/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "dsc_cust_id,dsc_cust_acct1,dsc_cust_name,dsc_cust_parent_name,dsc_cust_eff_end_date")] DSC_CUST_ACCT dSC_CUST_ACCT)
        {
            if (ModelState.IsValid)
            {
                db.DSC_CUST_ACCT.Add(dSC_CUST_ACCT);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dSC_CUST_ACCT);
        }

        // GET: DSC_CUST_ACCT/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DSC_CUST_ACCT dSC_CUST_ACCT = db.DSC_CUST_ACCT.Find(id);
            if (dSC_CUST_ACCT == null)
            {
                return HttpNotFound();
            }
            return View(dSC_CUST_ACCT);
        }

        // POST: DSC_CUST_ACCT/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "dsc_cust_id,dsc_cust_acct1,dsc_cust_name,dsc_cust_parent_name,dsc_cust_eff_end_date")] DSC_CUST_ACCT dSC_CUST_ACCT)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dSC_CUST_ACCT).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dSC_CUST_ACCT);
        }

        // GET: DSC_CUST_ACCT/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DSC_CUST_ACCT dSC_CUST_ACCT = db.DSC_CUST_ACCT.Find(id);
            if (dSC_CUST_ACCT == null)
            {
                return HttpNotFound();
            }
            return View(dSC_CUST_ACCT);
        }

        // POST: DSC_CUST_ACCT/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DSC_CUST_ACCT dSC_CUST_ACCT = db.DSC_CUST_ACCT.Find(id);
            db.DSC_CUST_ACCT.Remove(dSC_CUST_ACCT);
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
