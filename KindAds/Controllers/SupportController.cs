using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace KindAds.Controllers
{
    public class SupportController : BaseController
    {
        // GET: Support
        [Route("support/mailchimp-help")]
        [AllowAnonymous]
        public ActionResult MailchimpHelp()
        {
            return View();
        }
        [Route("support/campaignmonitor-help")]
        [AllowAnonymous]
        public ActionResult CampaignMonitorHelp()
        {
            return View();
        }
        [Route("support/sendgrid-help")]
        [AllowAnonymous]
        public ActionResult SendGridHelp()
        {
            return View();
        }
        [Route("support/aweber-help")]
        [AllowAnonymous]
        public ActionResult AWeberHelp()
        {
            return View();
        }
        [Route("support/gtm-help")]
        [AllowAnonymous]
        public ActionResult GTMHelp()
        {
            return View();
        }
    }
}
