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

namespace captivate_express_webapp.Controllers
{
  [AuthorizeRoles(Roles.Publisher)]
  public class SiteController : Controller
  {
    SiteService _siteService;
    CatalogService _catalogService;
    public SiteController()
    {
      _siteService = new SiteService();
      _catalogService = new CatalogService();
    }

    public ActionResult Index()
    {
      return View();
    }

    public async Task<ActionResult> CreateSite()
    {
      CleanSession();
      await FillCategory();
      FillProtocols();
      return View();
    }

    [HttpPost]
    public async Task<ActionResult> CreateSite(Models.Publisher.CreateSiteModel _createsite)
    {
      try
      {
        await FillCategory();
        FillProtocols();

        if (ModelState.IsValid)
        {
          string protocolSelecc = (String)Session["protocolSelecc"];
          _createsite.CategoryTypeSelect = Session["CategorySelecc"] != null ? Convert.ToInt16(Session["CategorySelecc"].ToString()) : _createsite.CategoryTypeSelect;
          _createsite.URL = String.IsNullOrEmpty(_createsite.URL) ? "-" : (String.IsNullOrEmpty(protocolSelecc) ? "https://" : protocolSelecc) + _createsite.URL;

          var result = _siteService.CreateSite(_createsite, Microsoft.AspNet.Identity.IdentityExtensions.GetUserId(User.Identity));

          if (result != null && !result.Equals(Guid.Empty))
          {
            return Json(new { success = true, message = "Site created successfully", idSite = result.ToString() });
          }
          else
          {
            return Json(new { error = "Error creating site" });
          }
        }

      }
      catch (Exception)
      {
        return Json(new { error = "Error creating site" });
      }

      return View(_createsite);
    }

    public async Task<ActionResult> ValidateSite(string idSite)
    {
      SITE site = new SITE();
      if (idSite != null)
      {
        site = await _siteService.GetSiteById(idSite);
        Session["IdSite"] = site.IdSite;
      }
      return View(site);
    }

    public ActionResult DownloadFileSite()
    {
      return _siteService.CreateVerificationFile(new Guid(Session["IdSite"].ToString()));
    }

    public async Task<ActionResult> VerifySite()
    {
      var result = _siteService.VerifySite(new Guid(Session["IdSite"].ToString()));
      SITE site = await _siteService.GetSiteById(Session["IdSite"].ToString());
      return Json(new { success = result });
    }

    public ActionResult ShowSites()
    {
      return View();      
    }

    public ActionResult ShowSitesPending(int page = 1, string sort = "URL", string sortdir = "ASC")
    {
      string idUser = Microsoft.AspNet.Identity.IdentityExtensions.GetUserId(User.Identity);
      return PartialView("_PendingVerifySites", _siteService.GetSitesPendingByIdUser(idUser,page,sort,sortdir));
    }

    public ActionResult ShowSitesVerify(int page = 1, string sort = "URL", string sortdir = "ASC")
    {
      string idUser = Microsoft.AspNet.Identity.IdentityExtensions.GetUserId(User.Identity);
      return PartialView("_VerifiedSites", _siteService.GetSitesVerifyByIdUser(idUser, page, sort, sortdir));
    }

    private async Task<List<CATEGORY>> FillCategory()
    {
      var listCategory = await _catalogService.GetAllCategoryAsync();
      ViewData["categories"] = new SelectList(listCategory, "IdCategory", "Description");
      return listCategory;
    }

    private List<string> FillProtocols()
    {
      List<string> listProtocol = new List<string>();
      listProtocol.Add("https://");
      listProtocol.Add("http://");
      ViewData["protocols"] = new SelectList(listProtocol);
      return listProtocol;
    }

    public JsonResult CategorySelecc(string id)
    {
      Session["CategorySelecc"] = id;
      return Json("success");
    }

    public JsonResult SiteSelecc(string id)
    {
      Session["SiteSelecc"] = id;
      return Json("success");
    }

    public JsonResult ProtocolSelecc(string id)
    {
      Session["ProtocolSelecc"] = id;
      return Json("success");
    }

    public JsonResult DeleteSite(Guid IdSite)
    {
      Boolean isSuccessfulDelete = true;

      SiteEntity site = _siteService.GetSingleSiteById(IdSite);
      if (site.PRODUCTs.Any()) //Si el sitio tiene productos asociados no se puede borrar
      {
        isSuccessfulDelete = false;
      }
      else
      {
        site.IsActive = false;
        _siteService.UpdateSingleSite(site);
      }
      return Json(new { isDeleted = isSuccessfulDelete }, JsonRequestBehavior.AllowGet);
    }

    private void CleanSession()
    {
      Session["ProductTypeSelecc"] = null;
      Session["CategorySelecc"] = null;
      Session["TagSelecc"] = null;
      Session["PartnerSelecc"] = null;
      Session["SiteSelecc"] = null;
      Session["IdSite"] = null;
    }
  }
}
