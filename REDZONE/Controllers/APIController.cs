using REDZONE.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace REDZONE.Controllers
{
    [AllowAnonymous]
    public class APIController : Controller
    {
        // GET: API/getCustInfo
        public JsonResult getCustInfo(int id)
        {
            DSC_OBS_DEV_Server db = new DSC_OBS_DEV_Server();
            DSC_CUSTOMER dscCustomer = db.DSC_CUSTOMER.Find(id);

            //var serializer = new JavaScriptSerializer();
            //string json = serializer.Serialize(dscCustomer);

            return Json(dscCustomer, JsonRequestBehavior.AllowGet); 
        }

        // GET: API/getCustomers
        public JsonResult getCustomers()
        {
            DSC_OBS_DEV_Server db = new DSC_OBS_DEV_Server();
            List<DSC_CUSTOMER>  dscCustomers = db.DSC_CUSTOMER.ToList();

            return Json(dscCustomers, JsonRequestBehavior.AllowGet);
        }

    }
}