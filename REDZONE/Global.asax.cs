using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.Http;

namespace REDZONE
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            GlobalFilters.Filters.Add(new System.Web.Mvc.AuthorizeAttribute());
            GlobalFilters.Filters.Add(new HandleErrorAttribute());
            ViewEngines.Engines.Add(new RazorViewEngine());
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpContextWrapper context = new HttpContextWrapper(this.Context);

            if (context.Request.IsAjaxRequest())
            {
                context.Response.SuppressFormsAuthenticationRedirect = true;
            }

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            HttpCookie authCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null || authCookie.Value == "")
                return;

            FormsAuthenticationTicket authTicket;
            try
            {
                authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            }
            catch
            {
                return;
            }

            //  ===================== SET UP THE USER CURRENT CONTEXT USER with ROLES ====================================
            //This logic will force the Application Authorization Request to retrieve the "User Data" from the authTicket 
            // split it into an array of 'Roles' that will be assigned as the current Logged on "User Identity Roles"
            if (Context.User != null) {
                //// retrieve roles from "UserData"  portion of the authTicket
                string[] roles = authTicket.UserData.Split(new string[] {"|"},StringSplitOptions.RemoveEmptyEntries);
                if (roles.Length == 0 || roles[0] == "") { roles = new string[] { "NONE" }; }
                //    Context.User = new GenericPrincipal(Context.User.Identity, roles);
                HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(
                      new System.Security.Principal.GenericIdentity(Context.User.Identity.Name, "Forms"), roles);
                ////Valid Roles are: "Admin", "Super User", "Editor", "Viewer"   (This will change)
            }  // ================= FINISHED SETTING UP THE CONTEXT USER with ROLES ===================================================
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs
            // Get the exception object.
            Exception exc = Server.GetLastError();

            if (exc == null)
            {
                exc = new Exception("An Unknown Error occurred.");
            }
            //System.Diagnostics.Debug.WriteLine(exc);
            string errorMessage = exc.Message.TrimEnd(System.Environment.NewLine.ToCharArray());  //Remove the Carriage returns from the String

            // ----------- Handle the HTTP errors ----------------------------
            if (exc.GetType() == typeof(HttpException))
            {
                // The Complete Error Handling Example generates some errors using URLs with "NoCatch" in them;
                // ignore these here to simulate what would happen if a global.asax handler were not implemented.
                if (exc.Message.Contains("NoCatch") || exc.Message.Contains("maxUrlLength"))
                    return;


                // Process exception...
                if (exc.Message.Contains("ServerExecuteHttpHandlerAsyncWrapper") || !Request.IsAuthenticated)
                {
                    Response.Redirect("/Account/Login");
                }


                //Redirect HTTP errors to HttpError page
                //Server.Transfer("HttpErrorPage.aspx");     //For Standard ASPX Error Handling Page
                //Or do a Response Redirect to a controller View (MVC)
            }


            // For other kinds of errors give the user some information but stay on the default page
            Response.Write("<h2>Global Page Error</h2>\n");
            Response.Write("<p>" + exc.Message + "</p>\n");
            Response.Write(@"Return to the <a href='\Home\Index\'>Default Page</a>\n");

            //// Log the exception and notify system operators
            //ExceptionUtility.LogException(exc, "DefaultPage");
            //ExceptionUtility.NotifySystemOps(exc);

            //// We've handled the error, so clear it from the Server. 
            ////Leaving the server in an error state can cause unintended side effects as the server continues its attempts to handle the error.
            Server.ClearError();

            //// Possible that a partially rendered page has already been written to response buffer before encountering error, so clear it.
            //Response.Clear();

            //// Finally redirect, transfer, or render an error view if Unhandled

            //Session["ErrorMessage"] = errorMessage;            
            //Server.Transfer("~/Error/ErrorMsg");
            //Server.Transfer("~/Account/Login");
            //Response.Redirect("~/Error/ErrorMsg");
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }


        // To push our user's Role details in this principle object we override the .NET "Application_PostAuthenticateRequest" method
        // For MVC 4 or later versions
        //protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        //{
        //    if (FormsAuthentication.CookiesSupported == true)
        //    {
        //        if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
        //        {
        //            try
        //            {
        //                //let us take out the username now                
        //                string username = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
        //                string roles = string.Empty;
        //                //retrieve roles from DB or some other way

        //                //using (userDbEntities entities = new userDbEntities())
        //                //{
        //                //    User user = entities.Users.SingleOrDefault(u => u.username == username);

        //                //    roles = user.Roles;
        //                //}

        //                //let us extract the roles from our own custom cookie or by any other method we chose

        //                ////Lets disable User.Identity Role Management for now as Role validation will be peformed at the EditView Control Action Level
        //                ////-----------------------------------------------
        //                //      roles = getUserRoles(username);
        //                //      //Let us set the Pricipal with our user specific details
        //                //      HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(
        //                //           new System.Security.Principal.GenericIdentity(username, "Forms"), roles.Split(';'));
        //                ////-----------------------------------------------------
        //            }
        //            catch (Exception)
        //            {
        //                //somehting went wrong
        //            }
        //        }
        //    }
        //}

        //private string getUserRoles(string username)
        //{
        //    //Get User Role from DB or from harcoded List
        //    string appUserRoles = String.Empty;
        //    //appUserRoles = "1;2;3;4;5;6;7;8";          //Temp Hardcoding
        //    switch (username.ToUpper())
        //    {
        //        // Set ADMIN Group Level
        //        case "DELGADO FELICIANO":
        //        case "RASUL ABDUGUEV":
        //        case "KEVIN POGANI":
        //            appUserRoles = "ADMIN;AUTO;MANUAL";
        //            break;
        //        case "GIRI GOPAL":
        //            appUserRoles = "AUTO";
        //            break;
        //        default:
        //            appUserRoles = "MANUAL";
        //            break;
        //    }
        //    return appUserRoles;
        //}

    }
}
