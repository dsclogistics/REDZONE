using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace REDZONE.Controllers
{
    public class AppErrorController : Controller
    {
        //// GET: Error
        //public ActionResult Index()
        //{
        //    Exception exception = Server.GetLastError();  //Not working Last Error is cleared at this point
        //    System.Diagnostics.Debug.WriteLine(exception);
        //    Session["ErrorMessage"] = exception.Message;
        //    return View("ErrorMsg");
        //}

        // POST: Error
        [HttpPost]
        public ActionResult Message(string ErrorMsg)
        {
            Exception ex = new Exception(ErrorMsg);
            return View("AppError", ex);
        }

        // GET: Error
        public ActionResult Exception(Exception catchedException)
        {
            return View("AppError", catchedException);
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
            return View("AppError", ex);
        }

        // GET: Error
        public ActionResult NotFound()
        {
            Exception ex = new Exception("Page not found or invalid URL Entry point used");
            return View("AppError", ex);
        }

        // GET: Error
        public ActionResult BadRequest()
        {
            Exception ex = new Exception("Bad Request. Please verify your submission and try again.");
            return View("AppError", ex);
        }

        // GET: Error
        public ActionResult NotAuthorized()
        {
            Exception ex = new UnauthorizedAccessException();
            return View("AppError", ex);
        }

    }
}