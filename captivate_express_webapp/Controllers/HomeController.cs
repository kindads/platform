using captivate_express_webapp.Services;
using captivate_express_webapp.Utils.Enums;
using captivate_express_webapp.Utils.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace captivate_express_webapp.Controllers
{
  [AuthorizeRoles(Roles.Publisher, Roles.Advertiser)]
  public class HomeController : Controller
  {
    private AccessService _accessService;
    private PublisherService _publisherService;
    private ApplicationSignInManager _signInManager;
    private ApplicationUserManager _userManager;
    public HomeController()
    {
      _accessService = new AccessService();
      _publisherService = new PublisherService();
    }
    public async Task<ActionResult> Index()
    {
      var userRegister = await UserManager.FindByNameAsync(User.Identity.Name);
      var role = await UserManager.GetRolesAsync(userRegister.Id);
      if (role.Contains(Roles.Advertiser.ToString()))
      {
        return RedirectToAction("Home", "Advertiser");
      }
      else if (role.Contains(Roles.Publisher.ToString()))
      {
        if (_publisherService.ShowCreateProduct(userRegister.Id))
        {
          return RedirectToAction("Home", "Publisher");
        }
        else
        {
          return RedirectToAction("Index", "Publisher");
        }

      }
      return View();
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
  }
}
