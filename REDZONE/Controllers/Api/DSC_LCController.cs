using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using REDZONE.Models;

namespace REDZONE.Controllers.Api
{
    public class DSC_LCController : ApiController
    {
        private DSC_MTRC_DEV_Entities db = new DSC_MTRC_DEV_Entities();

        // GET: api/DSC_LC
        public IQueryable<DSC_LC> GetDSC_LC()
        {
            return db.DSC_LC;
        }

        // GET: api/DSC_LC/5
        [ResponseType(typeof(DSC_LC))]
        public IHttpActionResult GetDSC_LC(int id)
        {
            DSC_LC dSC_LC = db.DSC_LC.Find(id);
            if (dSC_LC == null)
            {
                return NotFound();
            }

            return Ok(dSC_LC);
        }

        // PUT: api/DSC_LC/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDSC_LC(int id, DSC_LC dSC_LC)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dSC_LC.dsc_lc_id)
            {
                return BadRequest();
            }

            db.Entry(dSC_LC).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DSC_LCExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/DSC_LC
        [ResponseType(typeof(DSC_LC))]
        public IHttpActionResult PostDSC_LC(DSC_LC dSC_LC)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.DSC_LC.Add(dSC_LC);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = dSC_LC.dsc_lc_id }, dSC_LC);
        }

        // DELETE: api/DSC_LC/5
        [ResponseType(typeof(DSC_LC))]
        public IHttpActionResult DeleteDSC_LC(int id)
        {
            DSC_LC dSC_LC = db.DSC_LC.Find(id);
            if (dSC_LC == null)
            {
                return NotFound();
            }

            db.DSC_LC.Remove(dSC_LC);
            db.SaveChanges();

            return Ok(dSC_LC);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DSC_LCExists(int id)
        {
            return db.DSC_LC.Count(e => e.dsc_lc_id == id) > 0;
        }
    }
}