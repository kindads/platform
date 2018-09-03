using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using KindAds.Utils.Security;
using KindAds.Utils.Enums;
using KindAds.Models.Publisher;
using KindAds.Negocio.Managersv2;
using KindAds.AuthorizeAttributes;

namespace KindAds.Controllers {

    [AuthorizeRoles(Roles.Publisher)]
    [ValidProfile]
    public class PublisherController : BaseController {
        private Services.PublisherService _publisherService;

        private PublisherProfileManager publisherProfileManager;

        public PublisherController()
        {
            _publisherService = new Services.PublisherService();
            publisherProfileManager = new PublisherProfileManager();
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> ManageProfile()
        {
            var idUsuario = Microsoft.AspNet.Identity.IdentityExtensions.GetUserId(User.Identity);
            TransactionsViewModel model = new TransactionsViewModel { ListTransactions = await _publisherService.GetTransactionsByIdUser(idUsuario) };
            return View(model);
        }

        public PartialViewResult ShowDetailSite(string idSite)
        {
            var products = _publisherService.GetProductsByIdSite(new Guid(idSite));
            return PartialView("_DetailSite", products);
        }

        //public ActionResult GetBalancePublisherAsync()
        //{
        //  return PartialView("_BalancePublisher", _publisherService.GetPublisherBalanceAsync(Microsoft.AspNet.Identity.IdentityExtensions.GetUserId(User.Identity)));
        //}

        // *****************************************************  KindAdsV2 *********************************************************************************


    }
}
