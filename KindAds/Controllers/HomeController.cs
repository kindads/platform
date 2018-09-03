using KindAds.Services;
using KindAds.Utils.Enums;
using KindAds.Utils.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using KindAds.Business.Partners.Mail;
using System.Configuration;
using KindAds.Models.Home;

namespace KindAds.Controllers {

    public class HomeController : BaseController {
        private AccessService _accessService;
        private PublisherService _publisherService;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public HomeController()
        {
            _accessService = new AccessService();
            _publisherService = new PublisherService();
        }

        [AuthorizeRoles(Roles.Publisher, Roles.Advertiser)]
        public async Task<ActionResult> Index()
        {
            var userRegister = await UserManager.FindByNameAsync(User.Identity.Name);
            var role = await UserManager.GetRolesAsync(userRegister.Id);
            if (role.Contains(Roles.Advertiser.ToString())) {
                return RedirectToAction("Home", "Advertiser");
            }
            else if (role.Contains(Roles.Publisher.ToString())) {
                if (_publisherService.ShowCreateProduct(userRegister.Id)) {
                    return RedirectToAction("Home", "Publisher");
                }
                else {
                    return RedirectToAction("Index", "Publisher");
                }

            }
            return View();
        }

        //[Route("")]//Se usa para que la página de inicio nunca muestre el controller en la ruta
        public ActionResult Home()
        {
            return View();
        }

       

        public ActionResult Lobby()
        {
            HomeViewModel viewModel = new HomeViewModel();
            return View(viewModel);
        }

        [Route("faq")]
        [AllowAnonymous]
        public ActionResult Faq()
        {
            return View();
        }

        [Route("team")]
        [AllowAnonymous]
        public ActionResult Team()
        {
            return View();
        }

        [Route("create-a-wallet")]
        [AllowAnonymous]
        public ActionResult CreateWallet()
        {
            return View();
        }

        [Route("terms-of-sevice")]
        [AllowAnonymous]
        public ActionResult TermsOfService()
        {
            return View();
        }

        [Route("privacy-policy")]
        [AllowAnonymous]
        public ActionResult PrivacyPolicy()
        {
            return View();
        }

        [Route("copyright")]
        [AllowAnonymous]
        public ActionResult Copyright()
        {
            return View();
        }

        public JsonResult Subscribe(string email)
        {
            string apiKey = ConfigurationManager.AppSettings["provider-appsubscribemailchimpapikey"];
            string idList = ConfigurationManager.AppSettings["provider-appsubscribemailchimpidlist"];
            MailChimpManager mailChimpManager = new MailChimpManager();

            return Json(new { success = mailChimpManager.addUserToList(apiKey, idList, email) });
        }

        public ApplicationUserManager UserManager {
            get {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set {
                _userManager = value;
            }
        }
    }
}
