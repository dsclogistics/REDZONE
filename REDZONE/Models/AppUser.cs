using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REDZONE.Models
{
    // This class defines and user object and initializes all its properties to empty or to the current Browser Context Session User
    public class AppUser
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string email { get; set; }
        public string emp_id { get; set; }
        public string role { get; set; }
        public string identityToken { get; set; }
        public bool isDefined { get; set; }

        // ---- "USER" Class Constructor --------
        public AppUser()
        {
            setSessionUser();
        }// --- End of Constructor Definition ---

        public void setSessionUser(){
            try
            {
                Username = HttpContext.Current.Session["first_name"].ToString();
                LastName = HttpContext.Current.Session["last_name"].ToString();
                email = HttpContext.Current.Session["email"].ToString();
                emp_id = HttpContext.Current.Session["emp_id"].ToString();
                role = HttpContext.Current.Session["role"].ToString();
                identityToken = "&^%o*t%7(d#4r";
                isDefined = true;
            }
            catch
            {
                Username = "";
                FirstName = "";
                LastName = "";
                email = "";
                emp_id = "";
                role = "";
                identityToken = "";
                isDefined = false;
            }
        }
    }//------------------------ END OF "USER" CLASS DEFINITION -----------------------------------

}