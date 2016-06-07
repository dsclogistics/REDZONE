using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace REDZONE.Controllers
{
    [AllowAnonymous]
    public class saySomethingController : Controller
    {
        // GET: sayHi
        public string Hi()
        {
            string myMessage = "Hi";
            return myMessage;
        }
    }
}