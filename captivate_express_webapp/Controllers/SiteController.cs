using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using captivate_express_webapp.Models;
using captivate_express_webapp.Services;
using captivate_express_webapp.Utils.Enums;
using captivate_express_webapp.Utils.Security;
using System.Linq;
using System.Web.Helpers;
using Captivate.DataAccess;
using Captivate.Comun.Models.Entities;
using Captivate.Comun.Models.ViewModel;
using Captivate.Comun.Models;
using Captivate.Negocio.ViewModels;
using captivate_express_webapp.Models.Publisher;
using SitesVM = Captivate.Comun.Models.ViewModel.ShowSitesViewModel;
using System.Web.Http;

namespace captivate_express_webapp.Controllers
{
  [AuthorizeRoles(Roles.Publisher)]
  public class SiteController : Controller
  {
    #region properties
    SiteViewModelManager manager;
    string IdUser { set; get; }
    #endregion

    #region public methods

    [System.Web.Mvc.HttpGet]
    public ActionResult CreateSite()
    {
      manager = new SiteViewModelManager();
      ModelState.Clear();
      return View(manager);
    }


    [System.Web.Mvc.HttpPost]
    public JsonResult CreateSite([FromBody] SiteViewModel viewModel)
    {
      if (ModelState.IsValid)
      {
        return CreateSiteWithToken(viewModel);
      }
      return Json(new { success = false });
    }

    public ActionResult ValidateSite(string idSite)
    {
      SiteEntity site = new SiteEntity();
      SiteViewModel model = new SiteViewModel();

      if (idSite != null)
      {
        site = manager.GetSiteById(idSite);      
        model.IdSite = site.IdSite.ToString();
        model.URL = site.URL;
      }
      return View(model);
    }

    public FileStreamResult DownloadFileSite(string IdSite)
    {
      FileStreamResult result = manager.CreateVerificationFile(new Guid(IdSite));
      return result;
    }

    public FileStreamResult DownloadScriptSite(string IdSite)
    {
      FileStreamResult result = manager.CreateScriptFile(new Guid(IdSite));
      return result;
    }

    // Call from siteModule.js ( To generate copy and paste script in google tag manager )
    public JsonResult GetGoogleTagManagerToken([FromBody] string IdSite)
    {
      string token = string.Empty;
      token = manager.GetGoogleTagManagerToken(IdSite);
      return Json(new { success = true, Token = token });
    }

    // Call from siteModule.js
    public ActionResult VerifySiteTxtAndGTM([FromBody] string IdSite, int Type)
    {
      var Token = string.Empty;
      var result = manager.VerifySite(new Guid(IdSite), Type);
      return Json(new { success = result });
    }

    // Call from siteModule.js
    public ActionResult VerifySiteAzure([FromBody] string IdSite, string ClientAppId, string SubscriptionId, string TenantId, string AppKey, int Type)
    {
      //Encapsulamos
      AzureADSiteValidation Properties = new AzureADSiteValidation
      {
        ClientAppId= ClientAppId,
        SubscriptionId= SubscriptionId,
        TenantId= TenantId,
        AppKey= AppKey
      };

      var result = manager.VerifyAzureSite(new Guid(IdSite), Properties, Type);
      return Json(new { success = result });
    }


    public ActionResult ShowSites()
    {
      return View();      
    }
   
    public ActionResult ShowSitesPending(int page = 1, string sort = "URL", string sortdir = "ASC")
    {
      IdUser = Microsoft.AspNet.Identity.IdentityExtensions.GetUserId(User.Identity);
      SitesVM sitesPending = manager.GetSitesPendingByIdUser(IdUser, page, sort, sortdir);
      return PartialView("_PendingVerifySites", sitesPending );
    }
      
    public ActionResult ShowSitesVerify(int page = 1, string sort = "URL", string sortdir = "ASC")
    {
      IdUser = Microsoft.AspNet.Identity.IdentityExtensions.GetUserId(User.Identity);
      SitesVM sitesVerify = manager.GetSitesVerifyByIdUser(IdUser, page, sort, sortdir);
      return PartialView("_VerifiedSites", sitesVerify );
    }

    public JsonResult DeleteSite(Guid IdSite)
    {
      bool result = manager.DeleteSite(IdSite);
      return Json(new { isDeleted = result }, JsonRequestBehavior.AllowGet);
    }

    #endregion

    #region Helpers de Site view model
   

    private JsonResult CreateSiteWithToken(SiteViewModel viewModel)
    {
      SiteViewModelManager manager = new SiteViewModelManager();
      IdUser = Microsoft.AspNet.Identity.IdentityExtensions.GetUserId(User.Identity);
      string messageOut = string.Empty;
      string siteId = string.Empty;
      bool result = false;

      //Falta hacerlo transaccional
      result = manager.CreateSite(viewModel,IdUser, out messageOut, out siteId);
      if (result)
      {
        var resultToken = manager.AddToken(new Guid(siteId));
        return Json(new { success = result, message = messageOut, idSite = siteId });
      }
      else
      {
        return Json(new { error = "Error creating site" });
      }
    }

   

    #endregion

    public SiteController()
    {
      manager = new SiteViewModelManager();
    }
  }
}
