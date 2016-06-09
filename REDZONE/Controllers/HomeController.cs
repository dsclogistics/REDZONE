using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace REDZONE.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {        
        public ActionResult Index()
        {
            //ServerAPIs.API_obs_getLC apiObject = new ServerAPIs.API_obs_getLC("DEV");
            ServerAPIs.API_obs_getObserver apiObject = new ServerAPIs.API_obs_getObserver("DEV", "Feliciano", "Delgado", "feliciano.delgado@dsc-logistics.com");

            ViewBag.APIresponse = apiObject.getJasonData();

            return View();
        }
   
    }

}