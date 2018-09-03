using KindAds.Comun.Models.ViewModel;
using KindAds.Negocio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace KindAds.Controllers
{
  public class MoneyAdsController : Controller
  {
    public MoneyAdsManager manager { set; get; }

    public MoneyAdsController()
    {
      manager = new MoneyAdsManager();
    }

    // GET: MoneyAdds
    public ActionResult Index()
    {
      return View();
    }

    public ActionResult Settings()
    {
      MoneyAdsViewModel model = new MoneyAdsViewModel();
      return View(model);
    }

    public ActionResult TimeLine()
    {
      return View();
    }

    [HttpGet]
    public ActionResult CreateAds()
    {
      var IdUser = Microsoft.AspNet.Identity.IdentityExtensions.GetUserId(User.Identity);
      MoneyAdsManager manager = new MoneyAdsManager();
      manager.IdUser = IdUser;
      return View(manager);
    }

    private Models.Core.FileUpload GetFileUpload()
    {
      Models.Core.FileUpload fileUpload = null;
      for (int i = 0; i < Request.Files.Count; i++)
      {
        fileUpload = new Models.Core.FileUpload();
        var file = Request.Files[i];
        if (file != null && !String.IsNullOrEmpty(file.FileName))
        {
          fileUpload.Filextension = Helpers.FileHelper.GetFileExtension(file.FileName);

          if (file != null && file.ContentLength > 0)
          {
            using (var binaryReader = new BinaryReader(file.InputStream))
            {
              fileUpload.FileData = binaryReader.ReadBytes(file.ContentLength);
            }
          }
        }
      }
      return fileUpload;
    }

    private string GetImageAzure(Models.Core.FileUpload fileUpload)
    {
      if (fileUpload.FileData != null && fileUpload.Filextension != null)
      {
        return Helpers.AzureStorageHelper.CreateBlobImageFile(fileUpload.FileData, fileUpload.Filextension);
      }
      else { return ""; }
    }

    [HttpPost]
    public ActionResult CreateAds(MoneyAdsViewModel viewModel)
    {
      MoneyAdsManager manager = new MoneyAdsManager();
      var IdUser = Microsoft.AspNet.Identity.IdentityExtensions.GetUserId(User.Identity);
      if (viewModel.defaultAd.typeSelected == "1")
      {
        Models.Core.FileUpload image = GetFileUpload();
        viewModel.defaultAd.image = GetImageAzure(image);
      }
      else
      {
        viewModel.defaultAd.image = string.Empty;
      }
      manager.AddAds(IdUser, viewModel.defaultAd);
      return RedirectToAction("Settings");
    }

    public ActionResult GetDefaultAds(int page = 1, string sort = "URL", string sortdir = "ASC")
    {
      var IdUser = Microsoft.AspNet.Identity.IdentityExtensions.GetUserId(User.Identity);
      MoneyAdsViewModel viewModel = manager.GetDefaultAds(IdUser, page, sort, sortdir);
      return PartialView("ShowDefaultAds", viewModel);
    }

    public ActionResult GetStickyAds(int page = 1, string sort = "URL", string sortdir = "ASC")
    {
      var IdUser = Microsoft.AspNet.Identity.IdentityExtensions.GetUserId(User.Identity);
      MoneyAdsViewModel viewModel = manager.GetStickyAds(IdUser, page, sort, sortdir);
      return PartialView("ShowStickyAds", viewModel);
    }

    

    public ActionResult Activate(int Id)
    {
      bool result = true;
      MoneyAdsManager manager = new MoneyAdsManager();
      result = manager.IsActive(Id);
      return RedirectToAction("Settings");
    }

    public ActionResult Delete(int Id)
    {
      bool result = true;
      MoneyAdsManager manager = new MoneyAdsManager();
      result = manager.IsAlive(Id);
      return RedirectToAction("Settings");
    }

    [HttpGet]
    public ActionResult ShowDownloadScriptView()
    {
      var IdUser = Microsoft.AspNet.Identity.IdentityExtensions.GetUserId(User.Identity);
      MoneyAdsManager manager = new MoneyAdsManager();
      manager.IdUser = IdUser;
      manager.viewModel = new MoneyAdsViewModel();
      manager.viewModel.defaultAd = new DefaultAds();
      manager.viewModel.defaultAd.IdSite = string.Empty;
      return View("DownloadScriptView", manager);
    }

    [HttpPost]
    public ActionResult DownloadFile(MoneyAdsManager manager)
    {
      var IdUser = Microsoft.AspNet.Identity.IdentityExtensions.GetUserId(User.Identity);
      if (manager.ContainOneActiveAds(IdUser))
      {
        string IdSite = manager.viewModel.defaultAd.IdSite;
        FileStreamResult result = manager.CreateTelemetryScript(IdUser, IdSite);
        return result;
      }
      else
      {
        return RedirectToAction("Settings");
      }
    }

  

  }
}
