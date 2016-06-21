using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace REDZONE.Controllers
{
    public class ErrorController : Controller
    {
        //// GET: Error
        //public ActionResult Index()
        //{
        //    Exception exception = Server.GetLastError();  //Not working Last Error is cleared at this point
        //    System.Diagnostics.Debug.WriteLine(exception);
        //    Session["ErrorMessage"] = exception.Message;
        //    return View("ErrorMsg");
        //}

        // GET: Error
        public ActionResult Index(string ErrorMsg)
        {
            Exception ex = new Exception(ErrorMsg);
            return View("Error", ex);
        }
        // POST: Error
        [HttpPost]
        public ActionResult IndexPost(string ErrorMsg)
        {
            Exception ex = new Exception(ErrorMsg);
            return View("Error", ex);
        }

        // GET: Error
        public ActionResult Index(Exception catchedException)
        {
            return View("Error", catchedException);
        }

        // GET: Error

        public ActionResult ErrorMsg()
        {
            string errorMessage = String.Empty;
            try {
                errorMessage = Session["ErrorMessage"].ToString();
            }
            catch {
                errorMessage = "No Information Available about the error condition. Please try again your reques or contact the Service Desk.";
            }

            Exception ex = new Exception(errorMessage);
            return View("Error", ex);
        }

        // GET: Error
        public ActionResult NotFound()
        {
            Exception ex = new Exception("Page not found or invalid URL Entry point used");
            return View("Error", ex);
        }

        // GET: Error
        public ActionResult BadRequest()
        {
            Exception ex = new Exception("Bad Request. Please verify your submission and try again.");
            return View("Error", ex);
        }

        // GET: Error
        public ActionResult NotAuthorized()
        {
            Exception ex = new UnauthorizedAccessException();
            return View("Error", ex);
        }

    }
}