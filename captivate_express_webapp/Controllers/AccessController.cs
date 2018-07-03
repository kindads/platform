using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using captivate_express_webapp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using captivate_express_webapp.Services;
using captivate_express_webapp.Utils.Enums;
using System.Xml;
using Captivate.Business.Email;
using Captivate.Comun.Enums;
using System.Resources;
using System.Collections;
using System.Globalization;
using Captivate.Common.Models;
using Captivate.Business;

namespace captivate_express_webapp.Controllers
{
  public class AccessController : BaseController
  {

    private ApplicationSignInManager _signInManager;
    private ApplicationUserManager _userManager;
    private AccessService _accessService;
    private PublisherService _publisherService;

    public AccessController()
    {
      _accessService = new AccessService();
      _publisherService = new PublisherService();
    }

    public ActionResult Index()
    {
      return View();
    }
    public ActionResult Login()
    {
      return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Login(Models.Access.LoginViewModel model)
    {

      if (!ModelState.IsValid)
      {
        return View(model);
      }

      if (await VerifyUserAccountConfirmedAsync(model.Email))
      {
        var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, false, shouldLockout: false);
        switch (result)
        {
          case SignInStatus.Success:
            var userRegister = await UserManager.FindByNameAsync(model.Email);
            var role = await UserManager.GetRolesAsync(userRegister.Id);
            if (role.Contains(Roles.Advertiser.ToString()))
            {
              return RedirectToAction("Home", "Advertiser");
            }
            else if (role.Contains(Roles.Publisher.ToString()))
            {
              if (_publisherService.ShowCreateSite(userRegister.Id))
              {
                return RedirectToAction("Index", "Publisher");
              }
              else
              {
                return RedirectToAction("ShowSites", "Site");
              }

            }
            return RedirectToAction("Index", "Home");
          case SignInStatus.LockedOut:
          case SignInStatus.RequiresVerification:
          case SignInStatus.Failure:
          default:
            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }
      }
      else
      {
        ModelState.AddModelError("", "Please enter your email address. You will receive a link to activate your account.");
      }

      return View();
    }

    private async Task<bool> VerifyUserAccountConfirmedAsync(string email)
    {
      var user = await UserManager.FindByNameAsync(email);
      if (user != null)
      {
        return await UserManager.IsEmailConfirmedAsync(user.Id);
      }
      return false;
    }

    [HttpGet]
    public ActionResult CreateAccount()
    {
      FillRoles();
      return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> CreateAccount(Models.Access.CreateAccountViewModel m)
    {
      if (ModelState.IsValid)
      {
        if (m.AgreeTerms)
        {
          var user = new ApplicationUser { UserName = m.Email, Email = m.Email, Hometown = string.Empty, TokenAddress = string.Empty, WalletAddress = string.Empty };
          var result = await UserManager.CreateAsync(user, m.Password);
          if (result.Succeeded)
          {
            UserDetail userDetail = new UserDetail(){ Name = m.Name ,UserId = user.Id ,IsPremium = false};
            var userRegister = UserManager.FindByEmail(user.Email);
            UserManager.AddToRole(userRegister.Id, m.Role);
            _accessService.InsertUserDetail(userDetail);
            var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);

            var callbackUrl = Url.Action(
                 "AccountVerified", "Access",
                 new { userId = user.Id, code = code },
                 protocol: Request.Url.Scheme);
            string mailContent = String.Format(new MailManager().GetMailContent(EMailType.createAccount), user.UserName, callbackUrl);

            //Enqueue
            // Creamos el objeto de notificacion
            Notification notification = new Notification();
            NotificationManager notificationManager = new NotificationManager();

            // Enviamos la notificacion para la validacion
            notification.IdUser = new Guid(user.Id);
            notification.MailContent = mailContent;

            // Encolamos la notificacion para que el webjob la procese
            notificationManager.EnqueueNewAccessUser(notification);

            //Enviamos el mail
            MailManager emailManager = new MailManager();
            MailMessage message = new MailMessage();
            string messageContent = String.Format(new MailManager().GetMailContent(EMailType.accountCreated), user.UserName, callbackUrl);

            //Config
            message.Body = messageContent;
            message.Destination = userRegister.Email;
            message.Subject = "Thanks for signing up.";


            await emailManager.SendAsync(message);

            return View("CheckEmailActivation");
          }
          AddErrors(result);
        }
        else
        {
          ModelState.AddModelError("", "You must accept terms and policy");
        }

      }

      return View(m);
    }

    //[HttpPost]
    //[AllowAnonymous]
    //[ValidateAntiForgeryToken]
    //public async Task<ActionResult> CreateAccount(Models.Access.CreateAccountViewModel m)
    //{
    //  if (ModelState.IsValid)
    //  {
    //    if (m.AgreeTerms)
    //    {
    //      Models.Wallet.CreateWalletModel _wallet = await new Helpers.NethereumHelper().CreateUserWallet();

    //      var user = new ApplicationUser { UserName = m.Email, Email = m.Email, Hometown = "", TokenAddress = _wallet.blobname, WalletAddress = _wallet.walletaddress };
    //      var result = await UserManager.CreateAsync(user, m.Password);
    //      if (result.Succeeded)
    //      {
    //        var userRegister = UserManager.FindByEmail(user.Email);
    //        UserManager.AddToRole(userRegister.Id, m.Role);
    //        //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
    //        var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
    //        var callbackUrl = Url.Action(
    //             "AccountVerified", "Access",
    //             new { userId = user.Id, code = code },
    //             protocol: Request.Url.Scheme);
    //        string mailContent = String.Format(new MailManager().GetMailContent(EMailType.createAccount), user.UserName, callbackUrl);
    //        await UserManager.SendEmailAsync(user.Id,
    //             "Confirm your email",
    //             //"Hi, thank you for join us!<br><br>Please click the following link to verify your email address. <a href=\"" + callbackUrl + "\">link</a><br><br><br> Kind Ads"
    //             //String.Format(GetMailBodyFromXML("createAccount"), callbackUrl)
    //             mailContent
    //             );
    //        return View("CheckEmailActivation");
    //        //return RedirectToAction("Index", "Home");
    //      }
    //      AddErrors(result);
    //    }
    //    else
    //    {
    //      ModelState.AddModelError("", "You must accept terms and policy");
    //    }

    //  }

    //  return View(m);
    //}

    private void FillRoles()
    {
      var roles = _accessService.GetAllRoles();
      if (roles != null && roles.Any())
      {
        var listaUsuarios = new SelectList(roles, "Name", "Name");
        Session["roles"] = listaUsuarios;
      }

    }

    [HttpGet]
    public ActionResult CheckEmailActivation()
    {
      return View();
    }

    [HttpGet]
    public ActionResult CheckEmailResetPassword()
    {
      return View();
    }

    [HttpGet]
    public ActionResult VerifyAccount()
    {
      return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public ActionResult VerifyAccount(Models.Access.VerifyAccountViewModel m)
    {
      if (!ModelState.IsValid)
      {
        return View(m);
      }

      return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> NewPassword(Models.Access.NewPasswordViewModel m)
    {
      var u = (Models.Access.NewPasswordViewModel)TempData["PASSWORD_NEW_PASSWORD"];
      TempData["PASSWORD_NEW_PASSWORD"] = null;

      if (!ModelState.IsValid)
      {
        return View(m);
      }
      var user = await UserManager.FindByNameAsync(u.Email);
      if (user == null)
      {
        // Don't reveal that the user does not exist
        return RedirectToAction("ResetPasswordConfirmation", "Access");
      }
      var result = await UserManager.ResetPasswordAsync(user.Id, u.Code, m.Password);
      if (result.Succeeded)
      {
        return RedirectToAction("ResetPasswordConfirmation", "Access");
      }
      AddErrors(result);
      return View();
    }

    [HttpGet]
    [AllowAnonymous]
    public ActionResult NewPassword(string userId, string code, Models.Access.NewPasswordViewModel model)
    {
      if (code != null)
      {
        TempData["PASSWORD_NEW_PASSWORD"] = model;
      }
      else
      {
        return RedirectToAction("Login");
      }

      return View();
    }

    [HttpGet]
    public ActionResult RecoverPassword()
    {
      return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> RecoverPassword(Models.Access.RecoverPasswordViewModel m)
    {
      if (ModelState.IsValid)
      {
        var user = await UserManager.FindByNameAsync(m.Email);
        if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
        {
          return View("Login");
        }

        var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
        var callbackUrl = Url.Action("NewPassword", "Access", new { UserId = user.Id, code = code, Email = user.Email, Password = "" }, protocol: Request.Url.Scheme);
        string mailContent = String.Format(new MailManager().GetMailContent(EMailType.recoverPassword), user.UserName, callbackUrl);
        await UserManager.SendEmailAsync(user.Id, "Reset Password",
          //"Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>"
          //String.Format(GetMailBodyFromXML("recoverPassword"), callbackUrl)
          mailContent
          );
        return RedirectToAction("CheckEmailResetPassword", "Access");
      }

      return View(m);
    }

    [HttpPost]
    public ActionResult LogOff()
    {
      AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
      return View("~/Views/Home/Home.cshtml");
    }

    // GET: /Access/ConfirmEmail
    [AllowAnonymous]
    public async Task<ActionResult> AccountVerified(string userId, string code)
    {
      if (userId == null || code == null)
      {
        return View("Error");
      }
      var result = await UserManager.ConfirmEmailAsync(userId, code);

      AccessService accessService = new AccessService();

      //Si el resultado es positivo, se envía el correo con los quick steps
      if (result.Succeeded)
      {
        //String usrID = User.Identity.GetUserId();
        var user = accessService.GetUserById(userId); //UserManager.FindById(usrID);
        //userId
        if (user != null)
        {
          String body = "";
          String rol = "";
          var role = await UserManager.GetRolesAsync(user.Id);
          var correo = await UserManager.GetEmailAsync(user.Id);
          if (role.Contains(Roles.Advertiser.ToString()))
          {
            body = String.Format(new MailManager().GetMailContent(EMailType.advertiser), user.UserName);
            rol = "Advertiser";
          }
          else if (role.Contains(Roles.Publisher.ToString()))
          {
            body = String.Format(new MailManager().GetMailContent(EMailType.publisher), user.UserName);
            rol = "Publisher";
          }

          IdentityMessage identityMessage = new IdentityMessage()
          {
            Body = body,
            Destination = correo,
            Subject = rol + ", welcome to Kind ads"
          };

          EmailService mailService = new EmailService();

          //DESCOMENTAR PARA ENVÍO DE CORREO DE QUICKSTEPS
          await mailService.SendAsync(identityMessage);
        }
      }

      return View(result.Succeeded ? "AccountVerified" : "Error");
    }

    [HttpGet]
    public ActionResult Authorize()
    {
      var claims = new ClaimsPrincipal(User).Claims.ToArray();
      var identity = new ClaimsIdentity(claims, "Bearer");
      AuthenticationManager.SignIn(identity);
      return new EmptyResult();
    }

    [AllowAnonymous]
    public ActionResult ResetPasswordConfirmation()
    {
      return View();
    }
    public ActionResult TermsOfService()
    {
      return View();
    }
    public ActionResult PrivacyPolicy()
    {
      return View();
    }

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

    private string GetUserId()
    {
      return Microsoft.AspNet.Identity.IdentityExtensions.GetUserId(User.Identity);
    }

    private String GetMailBodyFromXML(string selector)//Obtiene el cuerpo de un correo dado en el xml del proyecto
    {
      //Obtiene la ruta del bin
      var path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
      path = path.Substring(6) + @"\captivate_express_webapp_mail.xml";
      //Lee el xml
      string xmlstring = System.IO.File.ReadAllText(path);
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(xmlstring);
      //Dado el parámetro selector, obtiene el cuerpo del correo
      XmlNode messageNode = doc.DocumentElement.SelectSingleNode("/correo/" + selector);
      //DESCOMENTAR PARA INSERTAR FOOTER
      //XmlNode footerNode = doc.DocumentElement.SelectSingleNode("/correo/" + "footer");
      String completeMessage = messageNode.InnerText;// + footerNode.InnerText;
      return completeMessage;
    }

  }
}
