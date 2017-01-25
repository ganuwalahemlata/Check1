using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using KontinuityCRM.Models;
using System;
using System.Collections.Generic;
using KontinuityCRM.Helpers;
using System.Linq;
namespace KontinuityCRM.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        private readonly INLogger logger = null;
        public AccountController(IUnitOfWork uow, IWebSecurityWrapper wsw, INLogger _logger)
            : base(uow, wsw)
        {
            logger = _logger;
        }
        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.TimeZones = uow.TimeZoneRepository.Get().ToList();
            return View();
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgotPassword(ResetModel resetModel)
        {
            logger.LogInfo("step1: forgot password process started by user:" + resetModel.Email);
            var user = uow.UserProfileRepository.Get(e => e.Email == resetModel.Email).FirstOrDefault();
            if (user == null)
            {
                logger.LogInfo("step2: ERROR: Email provided is not registered in system");
                ModelState.AddModelError("", "The user email is no registered in system");
                return View();
            }
            var g = Guid.NewGuid();
            user.ForgotPasswordToken = g.ToString();
            uow.Save();
            string passwordLink = Request.Url.Scheme + "://" + Request.Url.Authority + "/Account/ResetPassword?Token=" + HttpUtility.UrlEncode(g.ToString()) + "&email=" + HttpUtility.UrlEncode(resetModel.Email); //HttpContext.Request.Url.AbsoluteUri;
            logger.LogInfo("step3: Email link has been generated:" + passwordLink);
            try
            {
                EmailHelper.SendForgotPasswordEmail(uow, null, NotificationType.VoidNotification, resetModel.Email, passwordLink,logger);
            }
            catch (Exception ex)
            {
                logger.LogException("ERROR: seding forgot password email:", ex);
            }
            resetModel.Email = "";
            ViewBag.Message = "Email sent";
            return View("Login");
        }


        [AllowAnonymous]
        public ActionResult ResetPassword(string token, string email)
        {

            var _email = HttpUtility.UrlDecode(email);
            var _token = HttpUtility.UrlDecode(token);
            var user = uow.UserProfileRepository.Get(e => e.Email == _email).FirstOrDefault();
            if (user != null && user.ForgotPasswordToken != null && user.ForgotPasswordToken == _token)
            {
                logger.LogInfo("User email and token are valid. Redirecting to reset form");
                ViewBag.email = email;
                TempData["email"] = email;
                return View();
            }

            logger.LogInfo("Process failed generating token. Reason could be either email is not registered or token is invalid");
            return RedirectToAction("Login");

        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ResetPassword(AccountModel model)
        {
            logger.LogInfo("Reset password called");
            if (ModelState.IsValid)
            {
                var emailAddress = TempData["email"];
                logger.LogInfo("Reset password model is valid and email is:" + emailAddress);
                var user = uow.UserProfileRepository.Get(o => o.Email == emailAddress).FirstOrDefault(); //repo.FindUserProfile(id);
                string token = WebSecurity.GeneratePasswordResetToken(user.UserName);
                WebSecurity.ResetPassword(token, model.ChangePasswordModel.ConfirmPassword);

                ViewBag.Message = "Password reset successfully!";
                return View("Login");
            }

            return View();
        }
        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult Login(AccountModel amodel, string returnUrl)
        {
            var model = amodel.LoginModel;
            var user = uow.UserProfileRepository.Get(a => a.UserName == model.UserName).FirstOrDefault();
            if (user == null)
            {
                ModelState.AddModelError("", "The user name or password provided is incorrect.");
                return View(amodel);
            }
            if (user.Status == false)
            {
                ModelState.AddModelError("", "The user name or password provided is incorrect.");
                return View(amodel);
            }

            else if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                return RedirectToLocal(returnUrl);
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(amodel);
        }

        //
        // POST: /Account/LogOff

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
            //return RedirectToAction("login");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            ViewBag.Register = true;
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult Register(AccountModel amodel, string returnUrl)
        {
            var model = amodel.RegisterModel;
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    var assembly = System.Reflection.Assembly.Load("EnumAssembly");
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password
                        , propertyValues: new
                            {
                                //Permissions = 0
                                Email = model.Email,
                                Permissions = KontinuityCRMHelper.CalculateAllPermissions(assembly, "ViewPermissiond"),
                                Permissions1 = KontinuityCRMHelper.CalculateAllPermissions(assembly, "ViewPermission"),
                                Permissions2 = KontinuityCRMHelper.CalculateAllPermissions(assembly, "ViewPermission2"),
                                APIKey = Guid.NewGuid(),
                                TimeZoneId = model.TimeZoneId
                            }
                         );

                    WebSecurity.Login(model.UserName, model.Password);
                    return RedirectToAction("Index", "Home");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            //return Redirect

            // If we got this far, something failed, redisplay form
            //return 
            //RedirectToAction(
            ViewBag.Register = true;
            //return View("login", new AccountModel { RegisterModel = model });
            return View(amodel);
        }

        //
        // POST: /Account/Disassociate

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Disassociate(string provider, string providerUserId)
        {
            string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
            ManageMessageId? message = null;

            // Only disassociate the account if the currently logged in user is the owner
            if (ownerAccount == User.Identity.Name)
            {
                // Use a transaction to prevent the user from deleting their last login credential
                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
                {
                    bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
                    if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name).Count > 1)
                    {
                        OAuthWebSecurity.DeleteAccount(provider, providerUserId);
                        scope.Complete();
                        message = ManageMessageId.RemoveLoginSuccess;
                    }
                }
            }

            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("", e);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            return new ExternalLoginResult(provider, Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback

        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            AuthenticationResult result = OAuthWebSecurity.VerifyAuthentication(Url.Action("ExternalLoginCallback", new { ReturnUrl = returnUrl }));
            if (!result.IsSuccessful)
            {
                return RedirectToAction("ExternalLoginFailure");
            }

            if (OAuthWebSecurity.Login(result.Provider, result.ProviderUserId, createPersistentCookie: false))
            {
                return RedirectToLocal(returnUrl);
            }

            if (User.Identity.IsAuthenticated)
            {
                // If the current user is logged in add the new account
                OAuthWebSecurity.CreateOrUpdateAccount(result.Provider, result.ProviderUserId, User.Identity.Name);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // User is new, ask for their desired membership name
                string loginData = OAuthWebSecurity.SerializeProviderUserId(result.Provider, result.ProviderUserId);
                ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(result.Provider).DisplayName;
                ViewBag.ReturnUrl = returnUrl;
                return View("ExternalLoginConfirmation", new RegisterExternalLoginModel { UserName = result.UserName, ExternalLoginData = loginData });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLoginConfirmation(RegisterExternalLoginModel model, string returnUrl)
        {
            string provider = null;
            string providerUserId = null;

            if (User.Identity.IsAuthenticated || !OAuthWebSecurity.TryDeserializeProviderUserId(model.ExternalLoginData, out provider, out providerUserId))
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Insert a new user into the database
                //using (UsersContext db = new UsersContext())
                //{
                //    UserProfile user = db.UserProfiles.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());
                //    // Check if user already exists
                //    if (user == null)
                //    {
                //        // Insert name into the profile table
                //        db.UserProfiles.Add(new UserProfile { UserName = model.UserName });
                //        db.SaveChanges();

                //        OAuthWebSecurity.CreateOrUpdateAccount(provider, providerUserId, model.UserName);
                //        OAuthWebSecurity.Login(provider, providerUserId, createPersistentCookie: false);

                //        return RedirectToLocal(returnUrl);
                //    }
                //    else
                //    {
                //        ModelState.AddModelError("UserName", "User name already exists. Please enter a different user name.");
                //    }
                //}
            }

            ViewBag.ProviderDisplayName = OAuthWebSecurity.GetOAuthClientData(provider).DisplayName;
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // GET: /Account/ExternalLoginFailure

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult ExternalLoginsList(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("_ExternalLoginsListPartial", OAuthWebSecurity.RegisteredClientData);
        }

        [ChildActionOnly]
        public ActionResult RemoveExternalLogins()
        {
            ICollection<OAuthAccount> accounts = OAuthWebSecurity.GetAccountsFromUserName(User.Identity.Name);
            List<ExternalLogin> externalLogins = new List<ExternalLogin>();
            foreach (OAuthAccount account in accounts)
            {
                AuthenticationClientData clientData = OAuthWebSecurity.GetOAuthClientData(account.Provider);

                externalLogins.Add(new ExternalLogin
                {
                    Provider = account.Provider,
                    ProviderDisplayName = clientData.DisplayName,
                    ProviderUserId = account.ProviderUserId,
                });
            }

            ViewBag.ShowRemoveButton = externalLogins.Count > 1 || OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            return PartialView("_RemoveExternalLoginsPartial", externalLogins);
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
