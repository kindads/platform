using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using KindAds.Models;
using KindAds.Models.Advertiser;
using KindAds.Services;
using KindAds.Utils.Security;
using KindAds.Utils.Enums;
using KindAds.Models.Publisher;
using KindAds.AuthorizeAttributes;

namespace KindAds.Controllers
{
  [AuthorizeRoles(Roles.Advertiser)]
    [ValidProfile]
    public class AdvertiserController : BaseController
  {
    private AdvertiseService _service;
    public AdvertiserController()
    {
      _service = new AdvertiseService();
    }

    public ActionResult Index()
    {
      return View();
    }

    public async Task<ActionResult> Home()
    {
      CleanSession();
      await FillProductTypeAsync();
      await FillCategory();
      FillPrice();
      return View(new HomeProductsViewModel { ListProducts = await FillProductPurchasedAsync() });
    }

    public JsonResult GetPartners(string id)
    {
      Session["ProductTypeSelecc"] = id;
      if (id != null && !String.IsNullOrEmpty(id))
      {
        var listPartners = _service.GetPartnersByIdProductType(id);
        return Json(new SelectList(listPartners, "IdPartner", "Name"));
      }
      else
      {
        return Json(new SelectList(new List<PARTNER>()));
      }
    }

    public JsonResult GetTags(string id)
    {
      Session["CategorySelecc"] = id;
      if (id != null && !String.IsNullOrEmpty(id))
      {
        var listCategories = _service.GetTagsByIdCategory(id);
        return Json(new SelectList(listCategories, "IdTag", "Description"));
      }
      else
      {
        return Json(new SelectList(new List<TAG>()));
      }
    }

    public void TagSelecc(string id)
    {
      Session["TagSelecc"] = id;
    }

    public void PartnerSelecc(string id)
    {
      Session["PartnerSelecc"] = id;
    }

    public void PriceSelecc(string id)
    {
      Session["PriceSelecc"] = id;
    }

    public PartialViewResult SearchProducts()
    {
      string productTypeSelecc = (String)Session["ProductTypeSelecc"];
      string categorySelecc = (String)Session["CategorySelecc"];
      string tagSelecc = (String)Session["TagSelecc"];
      string partnerSelecc = (String)Session["PartnerSelecc"];
      string priceSelecc = (String)Session["PriceSelecc"];

      var products = _service.GetProductsByFilters(productTypeSelecc, categorySelecc, tagSelecc, partnerSelecc, priceSelecc);
      return PartialView("SearchProducts", products);
    }

    public ActionResult CreateCampaign(string idProduct)
    {
      ViewBag.IdProduct = idProduct;
      return View();
    }

    public ActionResult ShowProductBuy()
    {
      return View();
    }

    public async Task<ActionResult> ManageProfile()
    {
      var idUsuario = Microsoft.AspNet.Identity.IdentityExtensions.GetUserId(User.Identity);
      TransactionsViewModel model = new TransactionsViewModel { ListTransactions = await (new PublisherService().GetTransactionsByIdUser(idUsuario))};
      return View(model);
    }

    private async Task<List<ProductPurchasedViewModel>> FillProductPurchasedAsync()
    {
      var listProducts = await _service.GetAllProductsPurchasedAsync();
      ViewBag.NoRecords = (listProducts != null && listProducts.Any()) ? (from r in listProducts where r.Product.IsPremium == false select r).Count().ToString() : "0";
      ViewBag.NoRecordsPremium = (listProducts != null && listProducts.Any()) ? (from r in listProducts where r.Product.IsPremium == true select r).Count().ToString() : "0";
      return listProducts;
    }

    private async Task<List<PRODUCT_TYPE>> FillProductTypeAsync()
    {
      var listProductType = await _service.GetAllProductTypeAsync();
      ViewData["products"] = new SelectList(listProductType, "IdProductType", "Name");
      return listProductType;
    }

    private async Task<List<CATEGORY>> FillCategory()
    {
      var listCategory = await _service.GetAllCategoryAsync();
      ViewData["categories"] = new SelectList(listCategory, "IdCategory", "Description");
      return listCategory;
    }

    private List<String> FillPrice()
    {
      List<string> listPrice = new List<string>();
      for (int i = 1; i <= 5; i++)
      {
        listPrice.Add(Convert.ToString(i));
      }
      ViewData["prices"] = new SelectList(listPrice);
      return listPrice;
    }

    private void CleanSession()
    {
      Session["ProductTypeSelecc"] = null;
      Session["CategorySelecc"] = null;
      Session["TagSelecc"] = null;
      Session["PartnerSelecc"] = null;
      Session["PriceSelecc"] = null;
    }
  }
}
