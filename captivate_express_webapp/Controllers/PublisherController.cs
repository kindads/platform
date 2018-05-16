using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using captivate_express_webapp.Utils.Security;
using captivate_express_webapp.Utils.Enums;
using captivate_express_webapp.Models.Publisher;

namespace captivate_express_webapp.Controllers
{
  [AuthorizeRoles(Roles.Publisher)]
  public class PublisherController : Controller
  {
    private Services.PublisherService _publisherService;

    public PublisherController()
    {
      _publisherService = new Services.PublisherService();
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

    public ActionResult GetBalancePublisherAsync()
    {
      return PartialView("_BalancePublisher", _publisherService.GetPublisherBalanceAsync(Microsoft.AspNet.Identity.IdentityExtensions.GetUserId(User.Identity)));
    }

  }
}
