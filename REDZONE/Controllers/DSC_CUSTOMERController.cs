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
    public class DSC_CUSTOMERController : Controller
    {
        private DSC_OBS_DEV_Server db = new DSC_OBS_DEV_Server();

        // GET: DSC_CUSTOMER
        public ActionResult Index()
        {
            return View(db.DSC_CUSTOMER.ToList());
        }

        // GET: DSC_CUSTOMER/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DSC_CUSTOMER dSC_CUSTOMER = db.DSC_CUSTOMER.Find(id);
            if (dSC_CUSTOMER == null)
            {
                return HttpNotFound();
            }
            return View(dSC_CUSTOMER);
        }

        // GET: DSC_CUSTOMER/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DSC_CUSTOMER/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "dsc_cust_id,dsc_cust_name,dsc_cust_parent_name,dsc_cust_eff_end_date")] DSC_CUSTOMER dSC_CUSTOMER)
        {
            if (ModelState.IsValid)
            {
                db.DSC_CUSTOMER.Add(dSC_CUSTOMER);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(dSC_CUSTOMER);
        }

        // GET: DSC_CUSTOMER/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DSC_CUSTOMER dSC_CUSTOMER = db.DSC_CUSTOMER.Find(id);
            if (dSC_CUSTOMER == null)
            {
                return HttpNotFound();
            }
            return View(dSC_CUSTOMER);
        }

        // POST: DSC_CUSTOMER/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "dsc_cust_id,dsc_cust_name,dsc_cust_parent_name,dsc_cust_eff_end_date")] DSC_CUSTOMER dSC_CUSTOMER)
        {
            if (ModelState.IsValid)
            {
                db.Entry(dSC_CUSTOMER).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(dSC_CUSTOMER);
        }

        // GET: DSC_CUSTOMER/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DSC_CUSTOMER dSC_CUSTOMER = db.DSC_CUSTOMER.Find(id);
            if (dSC_CUSTOMER == null)
            {
                return HttpNotFound();
            }
            return View(dSC_CUSTOMER);
        }

        // POST: DSC_CUSTOMER/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DSC_CUSTOMER dSC_CUSTOMER = db.DSC_CUSTOMER.Find(id);
            db.DSC_CUSTOMER.Remove(dSC_CUSTOMER);
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
