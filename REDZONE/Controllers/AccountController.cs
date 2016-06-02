using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using REDZONE.Models;
using System.Web.Security;
using System.Web.Script.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Configuration;
using System.Text;

namespace REDZONE.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        //------ NOT USED YET -------------
        public AccountController()
        {
        }
        //------ NOT USED YET -------------
        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
        //------ NOT USED YET -------------
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }
        //------ NOT USED YET -------------
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous] [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
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

        ////
        //// GET: /Account/VerifyCode
        //[AllowAnonymous]
        //public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        //{
        //    // Require that the user has already logged in via username/password or external login
        //    if (!await SignInManager.HasBeenVerifiedAsync())
        //    {
        //        return View("Error");
        //    }
        //    return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        //}

        //
        //// POST: /Account/VerifyCode
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }

        //    // The following code protects for brute force attacks against the two factor codes. 
        //    // If a user enters incorrect codes for a specified amount of time then the user account 
        //    // will be locked out for a specified amount of time. 
        //    // You can configure the account lockout settings in IdentityConfig
        //    var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
        //    switch (result)
        //    {
        //        case SignInStatus.Success:
        //            return RedirectToLocal(model.ReturnUrl);
        //        case SignInStatus.LockedOut:
        //            return View("Lockout");
        //        case SignInStatus.Failure:
        //        default:
        //            ModelState.AddModelError("", "Invalid code.");
        //            return View(model);
        //    }
        //}

        // ----- NOT USED YET------
        //// GET: /Account/Register
        //[AllowAnonymous]
        //public ActionResult Register()
        //{
        //    return View();
        //}

        //// ----- NOT USED YET------
        //// POST: /Account/Register
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Register(RegisterViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
        //        var result = await UserManager.CreateAsync(user, model.Password);
        //        if (result.Succeeded)
        //        {
        //            await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);
                    
        //            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
        //            // Send an email with this link
        //            // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
        //            // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
        //            // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

        //            return RedirectToAction("Index", "Home");
        //        }
        //        AddErrors(result);
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        //// ----- NOT USED YET------
        //// GET: /Account/ConfirmEmail
        //[AllowAnonymous]
        //public async Task<ActionResult> ConfirmEmail(string userId, string code)
        //{
        //    if (userId == null || code == null)
        //    {
        //        return View("Error");
        //    }
        //    var result = await UserManager.ConfirmEmailAsync(userId, code);
        //    return View(result.Succeeded ? "ConfirmEmail" : "Error");
        //}

        //
        //// GET: /Account/ForgotPassword
        //[AllowAnonymous]
        //public ActionResult ForgotPassword()
        //{
        //    return View();
        //}

        //
        //// POST: /Account/ForgotPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await UserManager.FindByNameAsync(model.Email);
        //        if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
        //        {
        //            // Don't reveal that the user does not exist or is not confirmed
        //            return View("ForgotPasswordConfirmation");
        //        }

        //        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
        //        // Send an email with this link
        //        // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
        //        // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
        //        // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
        //        // return RedirectToAction("ForgotPasswordConfirmation", "Account");
        //    }

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        //
        //// GET: /Account/ForgotPasswordConfirmation
        //[AllowAnonymous]
        //public ActionResult ForgotPasswordConfirmation()
        //{
        //    return View();
        //}

        //
        //// GET: /Account/ResetPassword
        //[AllowAnonymous]
        //public ActionResult ResetPassword(string code)
        //{
        //    return code == null ? View("Error") : View();
        //}

        ////
        //// POST: /Account/ResetPassword
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model);
        //    }
        //    var user = await UserManager.FindByNameAsync(model.Email);
        //    if (user == null)
        //    {
        //        // Don't reveal that the user does not exist
        //        return RedirectToAction("ResetPasswordConfirmation", "Account");
        //    }
        //    var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
        //    if (result.Succeeded)
        //    {
        //        return RedirectToAction("ResetPasswordConfirmation", "Account");
        //    }
        //    AddErrors(result);
        //    return View();
        //}

        ////
        //// GET: /Account/ResetPasswordConfirmation
        //[AllowAnonymous]
        //public ActionResult ResetPasswordConfirmation()
        //{
        //    return View();
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
        // GET: /Account/SendCode
        //[AllowAnonymous]
        //public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        //{
        //    var userId = await SignInManager.GetVerifiedUserIdAsync();
        //    if (userId == null)
        //    {
        //        return View("Error");
        //    }
        //    var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
        //    var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
        //    return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        //}

        //
        // POST: /Account/SendCode
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> SendCode(SendCodeViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return View();
        //    }

        //    // Generate the token and send it
        //    if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
        //    {
        //        return View("Error");
        //    }
        //    return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
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
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }

        //
        //// GET: /Account/ExternalLoginFailure
        //[AllowAnonymous]
        //public ActionResult ExternalLoginFailure()
        //{
        //    return View();
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }


        //============= PRIVATE LOGIN HELPER METHODS ==================
        private bool isLogonValid(LoginViewModel loginModel)
        {

            return true;
            //if (loginModel.Password.Equals("~~") && (loginModel.Username.Equals("delgado_feliciano") || loginModel.Username.Equals("abduguev_rasul")))
            //{
            //    Session.Add("role", "Admin");
            //    if (loginModel.Username.Equals("delgado_feliciano"))
            //    {
            //        Session.Add("first_name", "Feliciano");
            //        Session.Add("last_name", "Delgado");
            //        Session.Add("username", loginModel.Username);
            //        Session.Add("email", "feliciano.delgado@dsc-logistics.com");

            //    }
            //    else
            //    {

            //        Session.Add("first_name", "Rasul");
            //        Session.Add("last_name", "Abduguev");
            //        Session.Add("username", loginModel.Username);
            //        Session.Add("email", "rasul.abduguev@dsc-logistics.com");
            //    }

            //    return true;
            //}
            //string ldaurl = ConfigurationManager.AppSettings["LDAPURL"];
            //WebRequest request = WebRequest.Create(ldaurl);
            //request.Method = "POST";
            //request.ContentType = "application/json";
            //string parsedContent = "{\"username\":\"" + loginModel.Username.Trim() + "\",\"password\":\"" + loginModel.Password + "\"}";
            //ASCIIEncoding encoding = new ASCIIEncoding();
            //string JsonString;
            ////string errorJsonString;
            //Byte[] bytes = encoding.GetBytes(parsedContent);
            //try
            //{
            //    Stream newStream = request.GetRequestStream();
            //    newStream.Write(bytes, 0, bytes.Length);
            //    newStream.Close();

            //    WebResponse response = request.GetResponse();
            //    using (Stream responseStream = response.GetResponseStream())
            //    {
            //        StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
            //        JsonString = reader.ReadToEnd();
            //    }//end of using
            //    JavaScriptSerializer ScriptSerializer = new JavaScriptSerializer();
            //    dynamic JsonObject = ScriptSerializer.Deserialize<Dictionary<dynamic, dynamic>>(JsonString);
            //    //use JsonObject to retrieve json data   
            //    if (JsonObject["result"] == "SUCCESS")
            //    {
            //        Session.Add("first_name", JsonObject["DSCAuthenticationSrv"]["first_name"]);
            //        Session.Add("last_name", JsonObject["DSCAuthenticationSrv"]["last_name"]);
            //        Session.Add("username", loginModel.Username);
            //        Session.Add("email", JsonObject["DSCAuthenticationSrv"]["email"]);
            //        string role = (from r in db.OBS_ROLE
            //                       join ur in db.OBS_USER_ROLE
            //                       on r.obs_role_id equals ur.obs_role_id
            //                       join ua in db.OBS_USER_AUTH
            //                        on ur.obs_user_auth_id equals ua.obs_user_auth_id
            //                       where ua.obs_user_auth_dsc_ad_name == loginModel.Username && r.obs_role_active_yn == "Y"
            //                       && ua.obs_user_auth_active_yn == "Y" && ur.obs_user_role_eff_start_dt <= DateTime.Now && ur.obs_user_role_eff_end_dt > DateTime.Now
            //                       select r.obs_role_name).FirstOrDefault();
            //        if (!String.IsNullOrEmpty(role))
            //        {
            //            Session.Add("role", role);
            //        }
            //        else
            //        {
            //            Session.Add("role", "Not Authorized");
            //        }
            //        return true;  /// Authenticasion was sucessful!!
            //    }
            //    else
            //    {
            //        ViewBag.errorMessage = JsonObject["message"];
            //        ModelState.AddModelError("", JsonObject["message"]);
            //        return false;
            //    }
            //}//end of try
            //catch (Exception ex)
            //{
            //    ViewBag.errorMessage = ex.Message;
            //    ModelState.AddModelError("", ex.Message);
            //    return false;  // Failed to authenticate the User
            //}//end of catch
        }

        private void setUserRoles(string userName, string[] roles)
        {
            string userRoles = String.Join(";", roles);

            var authTicket = new FormsAuthenticationTicket(
                 1,                             // version
                 userName,                      // user name
                 DateTime.Now,                  // created
                 DateTime.Now.AddMinutes(20),   // expires
                 true,                          // persistent?
                 userRoles              // can be used to store roles
              );

            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);

            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            this.ControllerContext.HttpContext.Response.Cookies.Add(authCookie);

            //HttpContext.Current.Response.Cookies.Add(authCookie);
        }

        #endregion
    }
}
