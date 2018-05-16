using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using captivate_express_webapp.Models;
using captivate_express_webapp.Models.Advertiser;
using captivate_express_webapp.Services;
using captivate_express_webapp.Utils.Security;
using captivate_express_webapp.Utils.Enums;
using captivate_express_webapp.Models.Publisher;

namespace captivate_express_webapp.Controllers
{
  [AuthorizeRoles(Roles.Advertiser)]
  public class AdvertiserController : Controller
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
      ViewBag.NoRecords = (listProducts != null && listProducts.Any()) ? listProducts.Count.ToString() : "0";
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
