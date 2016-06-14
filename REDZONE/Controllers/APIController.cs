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
    //-------------------------------------------------------------------------------------------
    // THIS CONTEOLLER IS FOR TESTING PURPOSES - NOT USED ON THE ACTUAL APPLICATION !!!!!!!!!!!
    //-------------------------------------------------------------------------------------------

    [AllowAnonymous]
    public class APIController : Controller
    {
        // GET: API/getCustInfo
        public JsonResult getCustInfo(int id)
        {
            DSC_MTRC_DEV_Entities db = new DSC_MTRC_DEV_Entities();
            DSC_CUST_ACCT dscCustomer = db.DSC_CUST_ACCT.Find(id);

            //var serializer = new JavaScriptSerializer();
            //string json = serializer.Serialize(dscCustomer);

            return Json(dscCustomer, JsonRequestBehavior.AllowGet); 
        }

        // GET: API/getCustomers
        public JsonResult getCustomers()
        {
            DSC_MTRC_DEV_Entities db = new DSC_MTRC_DEV_Entities();
            List<DSC_CUST_ACCT> dscCustomers = db.DSC_CUST_ACCT.ToList();

            return Json(dscCustomers, JsonRequestBehavior.AllowGet);
        }

    }
}