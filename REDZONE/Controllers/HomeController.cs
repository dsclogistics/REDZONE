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
        [AllowAnonymous]
        public ActionResult Index()
        {
            //ServerAPIs.API_obs_getLC apiObject = new ServerAPIs.API_obs_getLC("DEV");


            API_obs_getObserver apiObject = new API_obs_getObserver("DEV", "Feliciano", "Delgado", "feliciano.delgado@dsc-logistics.com");
            //string resultData = apiObject.getJasonData();
            ViewBag.APIresponse = apiObject.getJasonData();

            return View();
        }

        [AllowAnonymous]
     public ActionResult helloworld()
     {

         dscBuildingViewModel myBuilding = new dscBuildingViewModel();

         myBuilding.value = "0";
         myBuilding.employees.Add("John Smith");
         myBuilding.employees.Add("Peter Pan");
         myBuilding.employees.Add("Michael Jordan");
         myBuilding.employees.Add("I forgot");
         myBuilding.isList = false;
         return View( myBuilding);
     
     }

        //==============================================
        public class dscBuildingViewModel {
           public string name {get; set;}
           public string value {get; set;}
           public string code {get; set;}
           public List<string> employees = new List<string>();
           public bool isList { get; set; }

            public dscBuildingViewModel(){
                name = "Mira Loma";
                value = "Not set";
                code = "OT";
            }
        }
   
    }

}