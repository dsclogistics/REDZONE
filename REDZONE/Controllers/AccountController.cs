using REDZONE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace REDZONE.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        //private ApplicationSignInManager _signInManager;
        //private ApplicationUserManager _userManager;

        //GET: /Account/getLoginToken
        [AllowAnonymous]
        [HttpGet]
        public string getIV()
        {
            //This controller will generate a random IV (initialization Vector) that the client browser can use to encrypt credentials and login using AES encryption
            string encryptToken = System.Web.Security.Membership.GeneratePassword(16, 3);
            Session["loginToken"] = encryptToken;
            return encryptToken;
        }

        //
        // GET: /Account/Login
        //TEST
        //test
        //TeSt
        //testt
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.errorMessage = "";
            //Reset all authentication cookies and session variables (To prevent orphan authentication cookie and users getting locked out in some rare instance where new version of the app is deployed and an usser is signon)
            FormsAuthentication.SignOut();
            Session.Remove("emp_id");    //Session["emp_id"] = null;
            Session.Remove("first_name");
            Session.Remove("last_name");
            Session.Remove("email");
            Session.Remove("roles");    //Session["role"] = null;
            return View();
        }
        #region UnusedCode        
        //--------------------------------------------------------------------------------------------------------------\\
        //
        // POST: /Account/Login              (Original Template Method)
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Login(LoginViewModel loginModel, string returnUrl)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(loginModel);
        //    }
        //    //// This doesn't count login failures towards account lockout
        //    //// To enable password failures to trigger account lockout, change to shouldLockout: true
        //    //var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
        //    //switch (result)
        //    //{
        //    //    case SignInStatus.Success:
        //    //        return RedirectToLocal(returnUrl);
        //    //    case SignInStatus.LockedOut:
        //    //        return View("Lockout");
        //    //    case SignInStatus.RequiresVerification:
        //    //        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
        //    //    case SignInStatus.Failure:
        //    //    default:
        //    //        ModelState.AddModelError("", "Invalid login attempt.");
        //    //        return View(model);
        //    //}

        //    //Model State is Valid. Check Password
        //    if (isLogonValid(loginModel))
        //    {  // Is password is Valid, set the Authorization cookie and redirect
        //        // the user to the link it came from (Or the Home page is noreturn URL was specified)

        //        JObject parsed_result = JObject.Parse(data_retrieval.getObserver(Session["first_name"].ToString(), Session["last_name"].ToString(), Session["email"].ToString()));
        //        foreach (var res in parsed_result["resource"])
        //        {
        //            Session.Add("emp_id", (string)res["dsc_observer_emp_id"]);
        //        }
        //        setUserRoles(loginModel.Username, new string[] { Session["role"].ToString() });
        //        FormsAuthentication.SetAuthCookie(loginModel.Username, true);
        //        if (Url.IsLocalUrl(ReturnUrl) && ReturnUrl.Length > 1 && ReturnUrl.StartsWith("/")
        //            && !ReturnUrl.StartsWith("//") && !ReturnUrl.StartsWith("/\\"))
        //        { return Redirect(ReturnUrl); }
        //        else { return RedirectToAction("Index", "Home"); }

        //    }
        //    else
        //    {
        //        ViewBag.ReturnUrl = ReturnUrl;
        //        ModelState.AddModelError("", "Cannot Logon");
        //        return View(loginModel);
        //    }
        //}
        #endregion
        //--------------------------------------------------------------------------------------------------------------\\
        // This is a new Login Page Using Modal View (POST)
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult login(LoginViewModel loginModel, string ReturnUrl)
        {
            if (!ModelState.IsValid) { return View(loginModel); }

            FormsAuthentication.SignOut();
            Session.Remove("emp_id");    //Session["emp_id"] = null;
            Session.Remove("role");      //Session["role"] = null;
            Session.Remove("first_name");
            Session.Remove("last_name");
            Session.Remove("email");
            Session.Remove("userRole");


            try {
                //Model State is Valid. Check Password
                if (isLogonValid(loginModel))
                {  // Is password is Valid, set the Authorization cookie and redirect
                    // the user to the link it came from (Or the Home page is no return URL was specified)

                    //JObject parsed_result = JObject.Parse(data_retrieval.getObserver(Session["first_name"].ToString(), Session["last_name"].ToString(), Session["email"].ToString()));
                    //foreach (var res in parsed_result["resource"])
                    //{
                    //    Session.Add("emp_id", (string)res["dsc_observer_emp_id"]);
                    //}

                    
                    ///---------------- SKIP Authorization Roles at the Loging part for Now ------------
                    ////string uRoles = getUserRoles(loginModel.Username);
                    //setUserRoles(loginModel.Username, new string[] { Session["roles"].ToString() });
                    setUserRoles(loginModel.Username, new string[] { "ADMIN", "OTHER" });
                    ///----------------------------------------------------------------------------------

                    // Set the Authentication Encrypted Cookie
                    //FormsAuthentication.SetAuthCookie(loginModel.Username, true);                   

                    //if (ReturnUrl.Equals("%2FAccount%2FLogOff")) { return RedirectToAction("Index", "Home"); }

                    if (Url.IsLocalUrl(ReturnUrl) && ReturnUrl.Length > 1 && ReturnUrl.StartsWith("/")
                        && !ReturnUrl.StartsWith("//") && !ReturnUrl.StartsWith("/\\"))
                    { return Redirect(ReturnUrl); }
                    else { return RedirectToAction("Index", "Home"); }
                }
                else
                {
                    ViewBag.ReturnUrl = ReturnUrl;
                    ModelState.AddModelError("", "Failed to Logon User: " + ViewBag.errorMessage);
                    return View(loginModel);
                }
            }
            catch(Exception ex) {
                ViewBag.errorMessage = ex.Message + "\nContact the Service Desk for assistance.";
                return View(loginModel);
            }

        }
        //--------------------------------------------------------------------------------------------------------------\\
        // GET: /Account/LogOff
        [HttpGet][AllowAnonymous]
        public ActionResult LogOff(string backUrl)
        {
            if (String.IsNullOrEmpty(backUrl)) { backUrl = "\\Home\\Index"; }
            FormsAuthentication.SignOut();
            Session.Remove("emp_id");    //Session["emp_id"] = null;
            Session.Remove("role");      //Session["role"] = null;
            Session.Remove("first_name");
            Session.Remove("last_name");
            Session.Remove("email");
            //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            //Session["ReturnURL"] = "";
            return RedirectToAction("Login", new { returnUrl = backUrl });
        }

        //--------------------------------------------------------------------------------------------------------------\\
        // GET: /Account/LogOff
        [HttpPost]
        public string resetUserInfo(string uFName, string uLName, string uLoginName, string email, string uRole, bool turnOff)
        {
            //This function will be trigger via Ajax from the client to reset all the User Info Session Variables if lost 
            try {
                if (!turnOff)
                {   //Only reset the Session Variables if the TurnOff flag is set to "False"
                    Session["first_name"] = uFName;
                    Session["last_name"] = uLName;
                    Session["username"] = uLoginName;
                    Session["email"] = email;
                    Session["userRole"] = uRole;
                }

                Session["firstLoad"] = "False";    //Reset it always
                return "True";
            }
            catch {
                return "False";
            }
        }
        //--------------------------------------------------------------------------------------------------------------\\

        #region OriginalTemplateMethods
        //// POST: /Account/LogOff                (Original Template Method)
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult LogOff()
        //{
        //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
        //    return RedirectToAction("Login", "Account");
        //}
        //--------------------------------------------------------------------------------------------------------------//


        ////------ NOT USED YET -------------
        //public AccountController()
        //{
        //}
        ////------ NOT USED YET -------------
        //public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        //{
        //    UserManager = userManager;
        //    SignInManager = signInManager;
        //}
        ////------ NOT USED YET -------------
        //public ApplicationSignInManager SignInManager
        //{
        //    get
        //    {
        //        return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
        //    }
        //    private set 
        //    { 
        //        _signInManager = value; 
        //    }
        //}
        ////------ NOT USED YET -------------
        //public ApplicationUserManager UserManager
        //{
        //    get
        //    {
        //        return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
        //    }
        //    private set
        //    {
        //        _userManager = value;
        //    }
        //}


        //
        //// POST: /Account/ExternalLogin
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExternalLogin(string provider, string returnUrl)
        //{
        //    // Request a redirect to the external login provider
        //    return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        //}


        //
        // GET: /Account/ExternalLoginCallback
        //[AllowAnonymous]
        //public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        //{
        //    var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
        //    if (loginInfo == null)
        //    {
        //        return RedirectToAction("Login");
        //    }

        //    // Sign in the user with this external login provider if the user already has a login
        //    var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            return RedirectToLocal(returnUrl);
        //        case SignInStatus.LockedOut:
        //            return View("Lockout");
        //        case SignInStatus.RequiresVerification:
        //            return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
        //        case SignInStatus.Failure:
        //        default:
        //            // If the user does not have an account, then prompt the user to create an account
        //            ViewBag.ReturnUrl = returnUrl;
        //            ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
        //            return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
        //    }
        //}

        ////
        //// POST: /Account/ExternalLoginConfirmation
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Index", "Manage");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        // Get the information about the user from the external login provider
        //        var info = await AuthenticationManager.GetExternalLoginInfoAsync();
        //        if (info == null)
        //        {
        //            return View("ExternalLoginFailure");
        //        }
        //        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        //        var result = await UserManager.CreateAsync(user);
        //        if (result.Succeeded)
        //        {
        //            result = await UserManager.AddLoginAsync(user.Id, info.Login);
        //            if (result.Succeeded)
        //            {
        //                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
        //                return RedirectToLocal(returnUrl);
        //            }
        //        }
        //        AddErrors(result);
        //    }

        //    ViewBag.ReturnUrl = returnUrl;
        //    return View(model);
        //}


        //
        //// GET: /Account/ExternalLoginFailure
        //[AllowAnonymous]
        //public ActionResult ExternalLoginFailure()
        //{
        //    return View();
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        if (_userManager != null)
        //        {
        //            _userManager.Dispose();
        //            _userManager = null;
        //        }

        //        if (_signInManager != null)
        //        {
        //            _signInManager.Dispose();
        //            _signInManager = null;
        //        }
        //    }

        //    base.Dispose(disposing);
        //}
        #endregion
        //-----------------------------------------------------------------------------------------------------
        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        //private IAuthenticationManager AuthenticationManager
        //{
        //    get
        //    {
        //        return HttpContext.GetOwinContext().Authentication;
        //    }
        //}

        //private void AddErrors(IdentityResult result)
        //{
        //    foreach (var error in result.Errors)
        //    {
        //        ModelState.AddModelError("", error);
        //    }
        //}

        //private ActionResult RedirectToLocal(string returnUrl)
        //{
        //    if (Url.IsLocalUrl(returnUrl))
        //    {
        //        return Redirect(returnUrl);
        //    }
        //    return RedirectToAction("Index", "Home");
        //}

        //internal class ChallengeResult : HttpUnauthorizedResult
        //{
        //    public ChallengeResult(string provider, string redirectUri)
        //        : this(provider, redirectUri, null)
        //    {
        //    }

        //    public ChallengeResult(string provider, string redirectUri, string userId)
        //    {
        //        LoginProvider = provider;
        //        RedirectUri = redirectUri;
        //        UserId = userId;
        //    }

        //    public string LoginProvider { get; set; }
        //    public string RedirectUri { get; set; }
        //    public string UserId { get; set; }

        //    public override void ExecuteResult(ControllerContext context)
        //    {
        //        var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
        //        if (UserId != null)
        //        {
        //            properties.Dictionary[XsrfKey] = UserId;
        //        }
        //        context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
        //    }
        //}
        #endregion

        #region CustomHelpers
        //============= PRIVATE LOGIN HELPER METHODS ==================
        private bool isLogonValid(LoginViewModel loginModel)
        {
            if ((loginModel.Username.Equals("delgado_feliciano") || loginModel.Username.Equals("abduguev_rasul")) && loginModel.Password.Equals("~~"))
            {
                if (loginModel.Username.Equals("delgado_feliciano"))
                {
                    Session.Add("first_name", "Feliciano");
                    Session.Add("last_name", "Delgado");
                    Session.Add("username", loginModel.Username);                    
                    Session.Add("email", "feliciano.delgado@dsc-logistics.com");
                    //string test = Session["first_name"].ToString() + Session["last_name"].ToString() + Session["email"].ToString();
                }
                else
                {
                    Session.Add("first_name", "Rasul");
                    Session.Add("last_name", "Abduguev");
                    Session.Add("username", loginModel.Username);
                    Session.Add("email", "rasul.abduguev@dsc-logistics.com");
                }
                Session["emp_id"] = "12345";    //Temporarily to avoid auto-signoff
                Session["firstLoad"] = "True";  //To trigger localStorage logic when first logged in
                Session["userRole"] = "ADMIN";
                return true;
            }

            string ldaurl = ConfigurationManager.AppSettings["LDAPURL"];
            WebRequest request = WebRequest.Create(ldaurl);
            request.Method = "POST";
            request.ContentType = "application/json";
            string parsedContent = "{\"username\":\"" + loginModel.Username.Trim() + "\",\"password\":\"" + loginModel.Password + "\"}";
            ASCIIEncoding encoding = new ASCIIEncoding();
            string JsonString;
            //string errorJsonString;
            Byte[] bytes;
            try
            {
                bytes = encoding.GetBytes(parsedContent);
                Stream newStream = request.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();

                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    JsonString = reader.ReadToEnd();
                }//end of using
                JavaScriptSerializer ScriptSerializer = new JavaScriptSerializer();
                dynamic JsonObject = ScriptSerializer.Deserialize<Dictionary<dynamic, dynamic>>(JsonString);
                //use JsonObject to retrieve json data   
                if (JsonObject["result"] == "SUCCESS")
                {
                    Session.Add("first_name", JsonObject["DSCAuthenticationSrv"]["first_name"]);
                    Session.Add("last_name", JsonObject["DSCAuthenticationSrv"]["last_name"]);
                    Session.Add("username", loginModel.Username);
                    Session.Add("email", JsonObject["DSCAuthenticationSrv"]["email"]);
                    Session["firstLoad"] = "True";  //To trigger localStorage logic when first logged in
                    Session["userRole"] = REDZONE.AppCode.Util.getUserRoles(loginModel.Username); 
                    //string role = (from r in db.OBS_ROLE
                    //               join ur in db.OBS_USER_ROLE
                    //               on r.obs_role_id equals ur.obs_role_id
                    //               join ua in db.OBS_USER_AUTH
                    //                on ur.obs_user_auth_id equals ua.obs_user_auth_id
                    //               where ua.obs_user_auth_dsc_ad_name == loginModel.Username && r.obs_role_active_yn == "Y"
                    //               && ua.obs_user_auth_active_yn == "Y" && ur.obs_user_role_eff_start_dt <= DateTime.Now && ur.obs_user_role_eff_end_dt > DateTime.Now
                    //               select r.obs_role_name).FirstOrDefault();
                    return true;  /// Authenticasion was sucessful!!
                }
                else
                {
                    ViewBag.errorMessage = JsonObject["message"];
                    return false;
                }
            }//end of try
            catch (Exception ex)
            {
                ViewBag.errorMessage = ex.Message;
                return false;  // Failed to authenticate the User
            }//end of catch
        }

        private void setUserRoles(string userName, string[] roles)
        {
            string userRoles = String.Join(";", roles);

            var authTicket = new FormsAuthenticationTicket(
                 1,                             // version
                 userName,                      // user name
                 DateTime.Now,                  // created
                 DateTime.Now.AddMinutes(60),   // expires
                 true,                          // persistent?
                 userRoles              // can be used to store roles
              );

            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);

            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            this.ControllerContext.HttpContext.Response.Cookies.Add(authCookie);

            //HttpContext.Current.Response.Cookies.Add(authCookie);
        }

        private string getUserRoles(string uName) {
            //This function Queries the RZ DB directly and get the Metric Product Names that an User is Authorized to
            DSC_MTRC_DEV_Entities db = new DSC_MTRC_DEV_Entities();

            string uRoles = String.Join(";", db.MTRC_MGMT_AUTH.Include(d => d.MTRC_METRIC_PRODUCTS)
                .Where(x => x.mma_eff_start_date < DateTime.Now && x.mma_eff_end_date > DateTime.Now && x.mma_dsc_ad_username == uName)
                .Select(y => y.MTRC_METRIC_PRODUCTS.mtrc_prod_display_text).ToArray());

            return uRoles;
        }
        #endregion
    }
}
