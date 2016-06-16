using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using REDZONE.ServerAPIs;

namespace REDZONE.Controllers
{
    
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //ServerAPIs.API_obs_getLC apiObject = new ServerAPIs.API_obs_getLC("DEV");

            //API_obs_getObserver apiObject = new API_obs_getObserver("DEV", "Feliciano", "Delgado", "feliciano.delgado@dsc-logistics.com");
            ////string resultData = apiObject.getJasonData();
            //ViewBag.APIresponse = apiObject.getJasonData();

            return View();
        }
   
    }

}